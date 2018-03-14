﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Xml.Linq;
using uWebshop.Common;
using uWebshop.Common.Interfaces;
using uWebshop.Common.ServiceInterfaces;
using uWebshop.DataAccess;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Domain.Services
{
	internal sealed class OrderUpdatingService : IOrderUpdatingService
	{
		private readonly ICMSApplication _cmsApplication;
		private readonly IProductService _productService;
		private readonly IProductVariantService _productVariantService;
		private readonly IOrderService _orderService;
		private readonly IOrderNumberService _orderNumberService;
		private readonly IOrderRepository _orderRepository;
		private readonly IUwebshopConfiguration _uwebshopConfiguration;

		public OrderUpdatingService(ICMSApplication cmsApplication, IProductService productService, IProductVariantService productVariantService, IOrderService orderService, IOrderNumberService orderNumberService, IOrderRepository orderRepository, IUwebshopConfiguration uwebshopConfiguration)
		{
			_cmsApplication = cmsApplication;
			_productService = productService;
			_productVariantService = productVariantService;
			_orderService = orderService;
			_orderNumberService = orderNumberService;
			_orderRepository = orderRepository;
			_uwebshopConfiguration = uwebshopConfiguration;
		}

        private HttpSessionState Session
        {
            get
            {
                if (HttpContext.Current == null)
                {
                    throw new ApplicationException("No Http Context, No Session to Get!");
                }

                return HttpContext.Current.Session;
            }
        }

        /// <summary>
        /// Create, Add, Update the orderline
        /// </summary>
        /// <param name="order"></param>
        /// <param name="orderLineId">The Id of the orderline</param>
        /// <param name="productId">The productId</param>
        /// <param name="action">The action (add, update, delete, deleteall)</param>
        /// <param name="itemCount">The amount of items to be added</param>
        /// <param name="variantsList">The variants ID's added to the pricing</param>
        /// <param name="fields">Custom Fields</param>
        /// <exception cref="System.ArgumentException">
        /// productId and orderLineId equal or lower to 0
        /// or
        /// itemCountToAdd can't be smaller than 0
        /// </exception>
        /// <exception cref="System.Exception">
        /// Orderline not found
        /// </exception>
        public void AddOrUpdateOrderLine(OrderInfo order, int orderLineId, int productId, string action, int itemCount, IEnumerable<int> variantsList, Dictionary<string, string> fields = null)
		{
			//todo: function is too long
			//   separate control and logic
			//todo: function needs testing
			// todo: DIP

			var isWishList = order != null && order.Status == OrderStatus.Wishlist;

			if (!_cmsApplication.RequestIsInCMSBackend(HttpContext.Current) && !isWishList)
			{
				if (order == null || order.Paid.GetValueOrDefault(false) || (order.Status != OrderStatus.PaymentFailed && order.IsConfirmed() && order.Status != OrderStatus.WaitingForPayment))
				{
					Log.Instance.LogDebug("Starting new order for user" + (order == null ? "" : ", previous order: " + order.UniqueOrderId));
					order = OrderHelper.CreateOrder();
				}

				if (order.Status == OrderStatus.PaymentFailed || order.Status == OrderStatus.WaitingForPayment)
				{
					Log.Instance.LogDebug("Orderstatus WILL BE CHANGED FROM: " + order.Status);
					order.Status = OrderStatus.Incomplete;
				}
			}
			if (order == null) return;

			// clear some stored values so they can be recalculated
			order.ClearCachedValues();

			if (itemCount == 0 && action != "update")
				itemCount = 1;

			if (productId <= 0 && orderLineId <= 0)
			{
				throw new ArgumentException("productId <= 0 && orderLineId <= 0");
			}

			if (itemCount < 0)
			{
				throw new ArgumentException("itemCount can't be smaller than 0");
			}
			
			Log.Instance.LogDebug("AddOrUpdateOrderLine Before action");

			var variants = variantsList.Where(v => v != 0).OrderBy(v => v);

			OrderLine orderLine = null;

			if (orderLineId != 0 && action != "new")
			{
				orderLine = order.OrderLines.FirstOrDefault(line => line.OrderLineId == orderLineId);
			}
			if (orderLine == null && action != "new")
			{
				orderLine = order.OrderLines.FirstOrDefault(line => line.ProductInfo.Id == productId && line.VariantsMatch(variants));
			}
			if (orderLineId != 0 && orderLine == null && action != "new") throw new Exception("Orderline not found");

			if (productId == 0 && orderLine != null) productId = orderLine.ProductInfo.Id;

			if (action == "add" || action == "new")
			{
				action = "update";
				if (orderLine != null)
				{
					itemCount = (int) (orderLine.ProductInfo.ItemCount + itemCount);
				}
			}

			Log.Instance.LogDebug("AddOrUpdateOrderLine Before stock");

			if (productId != 0 && !isWishList)
			{
                var requestedItemCount = itemCount;
				var tooMuchStock = false;

				var localization = StoreHelper.CurrentLocalization;

                if (localization == null)
                {
                    Log.Instance.LogError("localization null");
                }

				var product = _productService.GetById(productId, localization);
				if (product == null)
				{
					Log.Instance.LogError("AddOrUpdateOrderLine can't find product with Id " + productId);
				}

                var higherItemList = new List<int>();
				foreach (var variant in variantsList.Select(variantId => _productVariantService.GetById(variantId, localization)))
				{
					if (variant != null && variant.StockStatus && !variant.BackorderStatus && variant.Stock < requestedItemCount)
					{
						higherItemList.Add(variant.Id);
						var stock = variant.Stock;
						itemCount = stock;
						tooMuchStock = true;
					}
				}

                if (product != null && !product.UseVariantStock && product.StockStatus && !product.BackorderStatus && product.Stock < itemCount)
				{
					higherItemList.Add(product.Id);

					itemCount = product.Stock;
					tooMuchStock = true;
				}

                if (tooMuchStock && HttpContext.Current != null)
                {
                    try
                    {
                        Session.Add(Constants.OrderedItemcountHigherThanStockKey, "Ordered higher quantity than available stock.");
                    } catch(Exception ex)
                    {
                        Log.Instance.LogError(ex, "Error on session");
                    }
                    ClientErrorHandling.SetOrClearErrorMessage(!tooMuchStock, "Ordered higher quantity than available stock. Updated the basked to available stock count", "Stock", requestedItemCount.ToString());

                    return;
                    //throw new ArgumentException("Ordered higher quantity than available stock. Updated the basked to available stock count.");
                }

                //if (HttpContext.Current != null) // todo: better decoupling
                //ClientErrorHandling.SetOrClearErrorMessage(!tooMuchStock, "Ordered higher quantity than available stock. Updated the basked to available stock count", "Stock", requestedItemCount.ToString());
            }

			if (itemCount < 1)
			{
				itemCount = 0;
			}

			if (action == "update" && itemCount == 0)
			{
				action = "delete";
			}

			Log.Instance.LogDebug("AddOrUpdateOrderLine Before update");

			#region update

			if (action == "update")
			{
				var beforeUpdatedEventArgs = order.FireBeforeOrderLineUpdatedEvent(orderLine);

				if (beforeUpdatedEventArgs == null || !beforeUpdatedEventArgs.Cancel) // todo: test the cancel
				{
					if (orderLine == null)
					{
						order.FireBeforeOrderLineCreatedEvent();

						if (orderLineId == 0)
						{
							orderLine = OrderProduct(productId, variants, itemCount, order);

							if (!order.OrderLines.Any())
							{
								orderLine.OrderLineId = 1;
							}
							else
							{
								var firstOrDefault = order.OrderLines.OrderByDescending(x => x.OrderLineId).FirstOrDefault();
								if (firstOrDefault != null)
								{
									var highestOrderLineId = firstOrDefault.OrderLineId;
									orderLine.OrderLineId = highestOrderLineId + 1;
								}
							}
						}
						if (orderLine == null)
						{
							throw new Exception("Order line not found");
						}

						order.OrderLines.Add(orderLine);

						order.FireAfterOrderLineCreatedEvent(orderLine);
					}

					if (orderLineId != 0)
					{
						orderLine.ProductInfo.ItemCount = itemCount; // todo: double with a few lines below?

						// onderstaande regel gooit variants weg als ze niet in de lijst met ids zitten, dat is by design
						orderLine.ProductInfo.ProductVariants = variants.Select(
							variant => new ProductVariantInfo(DomainHelper.GetProductVariantById(variant), orderLine.ProductInfo, itemCount)).ToList();
					}



					orderLine.ProductInfo.ChangedOn = DateTime.Now;
					orderLine.ProductInfo.ItemCount = itemCount;

					UpdateProductInfoDiscountInformation(orderLine.ProductInfo);

					foreach (var variant in orderLine.ProductInfo.ProductVariants)
						variant.ChangedOn = DateTime.Now;

					order.FireAfterOrderLineUpdatedEvent(orderLine);
				}

				//Log.Instance.LogDebug("AddOrUpdateOrderLine() UPDATE END: " + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"));
			}

			#endregion

			#region delete

			if (action == "delete")
			{
				BeforeOrderLineDeletedEventArgs beforeDeletedEventArgs = order.FireBeforeOrderLineDeletedEvent(orderLine);

				if (beforeDeletedEventArgs == null || !beforeDeletedEventArgs.Cancel)
				{
					order.OrderLines.Remove(orderLine);

					order.FireAfterOrderLineDeletedEvent();
				}
			}

			#endregion

			// UPDATE SHIPPING & SET UPDATESHIPPINGCOSTS TO TRUE AFTER BASKET UPDATE
			//Log.Instance.LogDebug( "AddOrUpdateOrderLine() AutoSelectShippingProvider START: " + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"));
			//AutoSelectShippingProvider();
			//Log.Instance.LogDebug( "AddOrUpdateOrderLine() AutoSelectShippingProvider END: " + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"));
			order.ShippingCostsMightBeOutdated = true;
			//Log.Instance.LogDebug( "AddOrUpdateOrderLine() function END: " + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"));

			if (fields == null) return;

			var xDoc = new XDocument(new XElement("Fields"));
			Log.Instance.LogDebug("AddOrUpdateOrderLine Before xdoc");

			if (orderLine != null && orderLine.ProductInfo != null && orderLine.ProductInfo.CatalogProduct != null)
			{
				AddFieldsToXDocumentBasedOnCMSDocumentType(xDoc, fields, orderLine.ProductInfo.CatalogProduct.NodeTypeAlias());
				orderLine._customData = xDoc;
			}
		}

		public void UpdateProductInfoDiscountInformation(ProductInfo product)
		{
			if (product.DiscountId > 0)
			{
				var discount = IO.Container.Resolve<IProductDiscountService>().GetById(product.DiscountId, product.Order.Localization);
				if (discount == null) return;
				if (discount.DiscountType == DiscountType.Amount)
					product.DiscountAmountInCents = discount.RangedDiscountValue(product.ItemCount.GetValueOrDefault(1));
				else if (discount.DiscountType == DiscountType.Percentage)
					product.DiscountPercentage = discount.RangedDiscountValue(product.ItemCount.GetValueOrDefault(1))/100m;
				else if (discount.DiscountType == DiscountType.NewPrice)
				{
					product.DiscountAmountInCents = product.OriginalPriceInCents - discount.RangedDiscountValue(product.ItemCount.GetValueOrDefault(1));
					product.Ranges = new List<Range>(); // NewPrice overrules any ranges
				}

				product.DiscountExcludingVariants = discount.ExcludeVariants;
			}
		}

		public void Save(OrderInfo order, bool revalidateOrderOnLoadHack = false, ValidateSaveAction validateSaveAction = ValidateSaveAction.Order)
		{
			// todo: DIP
			order.ReValidateSaveAction = validateSaveAction;
			order.RevalidateOrderOnLoad = revalidateOrderOnLoadHack;

			order.FireBeforeOrderUpdatedEvent();

			if (order.IsBasket())
			{
              //  try
                //{
                    OrderHelper.SetOrderCookie(order);
               // }
                //catch { } // Fail gracefully
            }

			if (order.IsBasket())
			{
				order.OrderDate = DateTime.Now.ToString("f");

				RemoveDiscountsWithCounterZeroFromOrder(order);
			}

			_orderRepository.SaveOrderInfo(order);

			order.FireAfterOrderUpdatedEvent();
		}

		public void SetCurrentMember(OrderInfo order)
		{
			if (order.IsConfirmed())
			{
				Log.Instance.LogError("SetCurrentMember for NOT incomplete order: " + order.UniqueOrderId + " status: " + order.Status);
			}
			// If no member connected to the order, add member to the order
			//if (string.IsNullOrEmpty(order.CustomerInfo.LoginName))
			{
				var currentMember = UwebshopRequest.Current.User; // TODO: dip

				if (currentMember != null && 
					(order.CustomerInfo.LoginName != currentMember.UserName || currentMember.ProviderUserKey != null && order.CustomerInfo.CustomerId != (int) currentMember.ProviderUserKey))
				{
					_orderRepository.SetCustomer(order.UniqueOrderId, currentMember.UserName);
					order.CustomerInfo.LoginName = currentMember.UserName;
					if (currentMember.ProviderUserKey != null)
					{
						_orderRepository.SetCustomerId(order.UniqueOrderId, (int) currentMember.ProviderUserKey);
						order.CustomerInfo.CustomerId = (int) currentMember.ProviderUserKey;
					}
					ReloadOrderData(order, order.Localization);
				}
			}
		}

		public void RemoveDiscountsWithCounterZeroFromOrder(OrderInfo order)
		{
			// already done automatically
		}

		public CouponCodeResult AddCoupon(OrderInfo order, string couponCode)
		{
            if (order == null)
                return CouponCodeResult.Failed;

            if (ChangeOrderToIncompleteAndReturnTrueIfNotAllowed(order))
				return CouponCodeResult.NotPermitted;

            if (string.IsNullOrEmpty(couponCode))
			{
				return CouponCodeResult.Failed;
			}

			if (order.CouponCodes.Contains(couponCode))
			{
				return CouponCodeResult.AlreadyUsed;
			}

			// todo: services
			var orderDiscountService = IO.Container.Resolve<IOrderDiscountService>();

            var allCoupondiscounts = IO.Container.Resolve<ICouponCodeService>().GetAllWithCouponcode(couponCode);

            var discounts = allCoupondiscounts.Where(coupon => coupon.NumberAvailable > 0)
				.Select(coupon => orderDiscountService.GetById(coupon.DiscountId, order.Localization ?? StoreHelper.CurrentLocalization)).Where(c => c != null).ToList();

            // Get all discount available on the order
            var currentOrderDiscounts = orderDiscountService.GetAll(order.Localization);

            var discountSelected = discounts.FirstOrDefault();

            //if (couponOrderDiscount != null)
            //{
               // Log.Instance.LogError("Coupon Debug1 : " + couponOrderDiscount.Title);

                //discounts.Add(couponOrderDiscount);
            //}

            if (discountSelected == null)
			{
				return CouponCodeResult.NotFound;
			}

            if (discountSelected.MinimumOrderAmount.WithVat.ValueInCents > 0 && order.GrandtotalInCents < discountSelected.MinimumOrderAmount.WithVat.ValueInCents)
            {
                return CouponCodeResult.MinimumOrderAmount;
            }

            if (discountSelected.CounterEnabled && discountSelected.Counter <= 0)
            {
                return CouponCodeResult.OutOfStock;
            }

            
            // Check if there is any other discount that has higher value in the order
            var calcServices = IO.Container.Resolve<IDiscountCalculationService>();
            var discountAmount = calcServices.DiscountAmountForOrder(discountSelected, order, false);
            if (order.AllDiscounts.Any(x => calcServices.DiscountAmountForOrder(x,order,false) > discountAmount)) {
                return CouponCodeResult.Failed;
            }

            var member = UwebshopRequest.Current.User;
			var oncePerCustomer = discountSelected.OncePerCustomer;
			if (oncePerCustomer)//&& member != null)
			{
                var customerEmail = member != null ? member.Email : order.CustomerEmail;

                if (!string.IsNullOrEmpty(customerEmail))
                {
                    var ordersOfMember = OrderHelper.GetOrdersForCustomerOrEmail(customerEmail).Where(x => x.CouponCodes != null && x.CouponCodes.Contains(couponCode));

                    if (ordersOfMember.Any(x => (x.Discounts.Any(d => d.OriginalId == discountSelected.Id)) || x.OrderLines.Any(l => l.ProductInfo.DiscountId == discountSelected.Id)))
                    {
                        return CouponCodeResult.OncePerCustomer;
                    }
                    
                } else
                {
                    return CouponCodeResult.EmailRequired;
                }

			}

			order.SetCouponCode(couponCode);

			return CouponCodeResult.Success;
		}

		public ProviderActionResult AddShippingProvider(OrderInfo order, int shippingProviderId, string shippingProviderMethodId, ILocalization localization)
		{
			if (ChangeOrderToIncompleteAndReturnTrueIfNotAllowed(order))
			{
				Log.Instance.LogDebug("AddShippingMethod: " + ProviderActionResult.NotPermitted);
				return ProviderActionResult.NotPermitted;
			}

			if (shippingProviderId == 0)
			{
				Log.Instance.LogDebug("AddShippingMethod: " + ProviderActionResult.ProviderIdZero);
				return ProviderActionResult.ProviderIdZero;
			}

			// todo: move logic and update API
			var shippingProvider = ShippingProviderHelper.GetAllShippingProviders(localization.StoreAlias, localization.CurrencyCode).FirstOrDefault(x => x.Id == shippingProviderId);

            if (shippingProvider == null)
			{
				Log.Instance.LogDebug("AddShippingMethod shippingProvider " + ProviderActionResult.NoCorrectInput + " shippingProviderId: " + shippingProviderId);
				return ProviderActionResult.NoCorrectInput;
			}
            order.ShippingInfo.Id = shippingProviderId;
            order.ShippingInfo.Key = shippingProvider.Key;
            order.ShippingInfo.Title = shippingProvider.Title;

			order.ShippingInfo.ShippingType = shippingProvider.Type;

			var shippingMethod = shippingProvider.ShippingProviderMethods.FirstOrDefault(x => x.Id == shippingProviderMethodId);

			if (shippingMethod == null)
			{
				Log.Instance.LogDebug("AddShippingMethod shippingMethod " + ProviderActionResult.NoCorrectInput + " shippingProviderMethodId: " + shippingProviderMethodId);
				return ProviderActionResult.NoCorrectInput;
			}
            
            order.ShippingInfo.MethodId = shippingProviderMethodId;
			order.ShippingInfo.MethodTitle = shippingMethod.Title;
            order.ShippingInfo.MethodKey = shippingMethod.Key;
            shippingMethod.UpdatePriceFromCustomShippingProvider(order);
			
			order.ResetDiscounts();

			return ProviderActionResult.Success;
		}

		public bool AddCustomerFields(OrderInfo order, Dictionary<string, string> fields, CustomerDatatypes customerDataType, bool ingnoreNotAllowed = false)
		{
			if (ChangeOrderToIncompleteAndReturnTrueIfNotAllowed(order) && ingnoreNotAllowed == false)
			{
				return false;
			}

			var xDoc = new XDocument(new XElement(customerDataType.ToString()));

			if (customerDataType == CustomerDatatypes.Customer)
			{
				if (order.CustomerInfo.customerInformation != null) xDoc = order.CustomerInfo.customerInformation;
			}

			if (customerDataType == CustomerDatatypes.Shipping)
			{
				if (order.CustomerInfo.shippingInformation != null) xDoc = order.CustomerInfo.shippingInformation;
			}

			if (customerDataType == CustomerDatatypes.Extra)
			{
				if (order.CustomerInfo.extraInformation != null) xDoc = order.CustomerInfo.extraInformation;
			}

			foreach (var field in fields.Where(field => xDoc.Root != null && xDoc.Root.Descendants(field.Key).Any()))
			{
				if (xDoc.Root != null) xDoc.Root.Descendants(field.Key).Remove();
			}

			AddFieldsToXDocumentBasedOnCMSDocumentType(xDoc, fields, Order.NodeAlias);

			UpdateRepeatFields(order, fields);

			// special fields that also work when field is not created on order document type
			foreach (var field in fields)
			{
				if (field.Key.ToLower() == "customercountry")
				{
					if (field.Value != "0")
					{
						order.CustomerInfo.CountryCode = field.Value;

						if (field.Value.Length > 4)
						{
							Log.Instance.LogDebug(
								string.Format("customerCountry Length == {0} Value: {1}, Possible added Country.Name instead of Country.Code?",
									field.Key.Length, field.Value));
						}
					}
				}

				if (field.Key.ToLower() == "shippingcountry")
				{
					if (field.Value != "0")
					{
						order.CustomerInfo.ShippingCountryCode = field.Value;

						if (field.Value.Length > 4)
						{
							Log.Instance.LogDebug(
								string.Format("shippingcountry Length == {0} Value: {1}, Possible added Country.Name instead of Country.Code?",
									field.Key.Length, field.Value));
						}
					}
				}

				if (field.Key.ToLower() == "shippingdeliverydatetime")
				{
					var deliveryDate = field.Value; // 2015-05-26T16:03:35
					if (deliveryDate != null)
					{
						var date = Common.Helpers.DateTimeMultiCultureParse(deliveryDate, order.StoreInfo.CultureInfo);
						if (date == null)
						{
							Log.Instance.LogError("Could not parse delivery date string: " + deliveryDate);
						}
						else
						{
							order.DeliveryDate = date;
						}
					}
				}

				if (field.Key.ToLower() == "customervatnumber")
				{
					order.SetVATNumber(field.Value);
				}

				if (field.Key.ToLower() == "customerregion")
				{
					order.CustomerInfo.RegionCode = field.Value;
					order._regionalVatInCents = null;

					if (field.Value.Length > 4)
					{
						Log.Instance.LogDebug(string.Format("customerregion Length == {0} Value: {1}, Possible added Country.Name instead of Country.Code?", field.Key.Length, field.Value));
					}
				}

				if (field.Key.ToLower() == "acceptsmarketing" || field.Key.ToLower() == "customeracceptsmarketing")
				{
					if (field.Value == "1" || field.Value.ToLowerInvariant() == "true" || field.Value == "on" || field.Value == "acceptsmarketing" || field.Value == "customeracceptsmarketing")
					{
						order.CustomerInfo.AcceptsMarketing = true;
					}
					else
					{
						order.CustomerInfo.AcceptsMarketing = false;
					}
				}

				// 'hack' because if you an empty checkbox is not send to the browser, by supporting this option the developer can add a hidden input 'false' field and make it checked using javascript.
				if (field.Key.ToLower() == "acceptsmarketingfalse" || field.Key.ToLower() == "customeracceptsmarketingfalse")
				{
					if (field.Value == "1" || field.Value.ToLowerInvariant() == "true" || field.Value == "on" || field.Value == "acceptsmarketingfalse" || field.Value == "customeracceptsmarketingfalse")
					{
						order.CustomerInfo.AcceptsMarketing = false;
					}
				}
			}

			if (customerDataType == CustomerDatatypes.Customer)
			{
				order.CustomerInfo.customerInformation = xDoc;
				order.CustomerValidationFailed = _orderService.ValidateCustomer(order).Any();
				return true;
			}

			if (customerDataType == CustomerDatatypes.Shipping)
			{
				order.CustomerInfo.shippingInformation = xDoc;
				order.CustomerValidationFailed = _orderService.ValidateCustomer(order).Any();
				return true;
			}

			if (customerDataType == CustomerDatatypes.Extra)
			{
				order.CustomerInfo.extraInformation = xDoc;
				order.CustomerValidationFailed = _orderService.ValidateCustomer(order).Any();
				return true;
			}

			return false;
		}

		private void UpdateRepeatFields(OrderInfo order, Dictionary<string, string> fields)
		{
			if(!fields.Any(x => x.Key.ToLowerInvariant().StartsWith("repeat")))
			{
				// if there are only shipping fields posted, and non repeat fields, return.
				// this is to prevent repeats being emptied when they should not.
				return;
			}
			var repeatOrder = fields.TryGetValue("repeatOrder"); //never sameday weekly monthly
			if (repeatOrder == null || repeatOrder == "never")
			{
				if (order.OrderSeries != null && order.OrderSeries.Id > 0)
				{
					RemoveAllScheduledOrders(order.OrderSeries);
				}
				order.OrderSeries = null;
				return;
			}

			order.OrderSeries = new OrderSeries();
			var seriesStart = fields.TryGetValue("shippingDeliveryDateTime"); // 2015-05-26T16:03:35
			if (seriesStart == null)
			{
				order.OrderValidationErrors.Add(new OrderValidationError { Alias = "ShippingDeliveryDateTimeNotSet", Value = "ShippingDeliveryDateTime is not set" });
				return;
			}
			var repeatDays = fields.TryGetValue("repeatDays") ?? string.Empty; // mon,tue,wed
			var repeatTimes = fields.TryGetValue("repeatTimes"); // 00:00,15:00,17:00
			var repeatInterval = fields.TryGetValue("repeatInterval"); // 2
			//var repeatEndsOn = fields["repeatEndsOn"]; // never/count/date => not needed, only frontend
			var repeatEndAfterInstances = fields.TryGetValue("repeatEndAfterInstances"); // 10
			var repeatEndDate = fields.TryGetValue("repeatEndDate"); // 2015-05-26 or 2015-05-26T16:03:35

			int intVal;

			var splitRepeatDays = repeatDays.Split(',');
			var splitRepeatTime = repeatTimes.Split(',');

			// empty values
			repeatDays = splitRepeatDays.Any(x => !string.IsNullOrEmpty(x)) ? string.Join(",", splitRepeatDays.Where(x=> !string.IsNullOrEmpty(x))) : string.Empty;
			repeatTimes = splitRepeatTime.Any(x => !string.IsNullOrEmpty(x)) ? string.Join(",", splitRepeatTime.Where(x => !string.IsNullOrEmpty(x))) : string.Empty;

			var startDate = Common.Helpers.DateTimeMultiCultureParse(seriesStart, order.StoreInfo.CultureInfo);
			if (startDate == null) throw new ApplicationException("Could not parse orderseries start date string: " + seriesStart);
			order.OrderSeries.Start = startDate.Value;

			var endDate = Common.Helpers.DateTimeMultiCultureParse(repeatEndDate, order.StoreInfo.CultureInfo);
			if (!string.IsNullOrWhiteSpace(repeatEndDate) && endDate == null) throw new ApplicationException("Could not parse orderseries end date string: " + repeatEndDate);
			order.OrderSeries.End = endDate;
			order.OrderSeries.EndAfterInstances = int.TryParse(repeatEndAfterInstances, out intVal) ? intVal : 1;
			var interval = int.TryParse(repeatInterval, out intVal) ? intVal : 0;

			order.OrderSeries.CronInterval = CreateCronInterval(order.OrderSeries.Start, repeatOrder, repeatTimes, interval, repeatDays).Item1;
		}

		private static void RemoveAllScheduledOrders(OrderSeries series)
		{
			if (series == null) throw new ArgumentNullException("series");
			uWebshopOrders.RemoveScheduledOrdersWithSeriesId(series.Id);
		}

		private void ScheduleOrdersOneYearInAdvance(OrderInfo order)
		{
			if (order.OrderSeries == null)
			{
				return;
			}

			if (order.OrderSeries.Id == 0)
			{
				order.Save();
				var newOrder = OrderHelper.GetOrder(order.DatabaseId);
				if (newOrder == null)
				{
					throw new ApplicationException("Can't load order with id " + order.DatabaseId);
				}
				order = newOrder;
			}

			if (order.OrderSeries.Id == 0)
			{
				throw new ApplicationException("OrderSeries without id loaded");
			}

			foreach (var date in CronHelper.GenerateDateTimeInstancesFromOrderSeries(order.OrderSeries))
			{
				var newOrder = _orderService.CreateCopyOfOrder(order);
				newOrder.DeliveryDate = date;
				newOrder.Status = OrderStatus.Scheduled;
				newOrder.Save();
			}
		}

		/// <summary>
		/// Creates a custom cron interval.
		/// </summary>
		/// <param name="seriesStart">The series start date.</param>
		/// <param name="repeatNature">The repeat nature (weekly/monthly/other=undefined).</param>
		/// <param name="repeatTimes">The repeat times, additional times at which the cron interval will also hit, structure: 10:00,11:30,15:45</param>
		/// <param name="repeatInterval">The repeat interval. Repeat every x weeks/months</param>
		/// <param name="repeatDays">The repeat weekdays. Format: mon,tue,fri</param>
		/// <returns>First part of the tuple is a custom Cron extension (might be as custom as: w2|0 10 * * mon,tue|0 12 * * mon,tue), second part is an Enlish explanation (don't depend on the latter to stay exactly the same)</returns>
		public static Tuple<string, string> CreateCronInterval(DateTime seriesStart, string repeatNature, string repeatTimes, int repeatInterval, string repeatDays)
		{
			// w2|0 10 * * mon,tue|0 12 * * mon,tue

			var cron = string.Empty;
			string cronExplanation = null;
			if (repeatInterval > 1)
			{
				if (repeatNature == "weekly")
				{
					cron = "w" + repeatInterval + "|";
					cronExplanation = "Every " + repeatInterval + " weeks";
				}
				if (repeatNature == "monthly")
				{
					cronExplanation = "Every " + repeatInterval + " months";
				}
			}
			var days = "*";
			var weekDays = "*";
			var month = "*";
			if (repeatNature == "monthly")
			{
				cronExplanation = cronExplanation ?? "Every month";
				var day = seriesStart.Day;
				var dayOfWeek = seriesStart.DayOfWeek;
				weekDays = dayOfWeek.ToString().ToLowerInvariant().Substring(0, 3); // todo: test with culture != EN
				if (day < 8) // first Monday/etc of the month
				{
					days = "1-7";
					cronExplanation += " on the first " + dayOfWeek;
				}
				else if (day < 15)
				{
					days = "8-14";
					cronExplanation += " on the second " + dayOfWeek;
				}
				else if (day < 22)
				{
					days = "15-21";
					cronExplanation += " on the third " + dayOfWeek;
				}
				else if (day < 29)
				{
					days = "22-28";
					cronExplanation += " on the fourth " + dayOfWeek;
				}
				else // temporary behaviour, don't depend on this remaining stable! would like this to be last Monday of the month
				{
					//weekDays += "L"; NCronTab doesn't support this.. (neither satL nor 6L)
					//days = "22-31"; //this is wrong todo: depends on month
					//cronExplanation += " on the last " + dayOfWeek;

					days = "29-31";
					cronExplanation += " on the fifth " + dayOfWeek;
				}
			}
			else if (repeatNature == "weekly")
			{
				if (string.IsNullOrWhiteSpace(repeatDays))
				{
					repeatDays = seriesStart.DayOfWeek.ToString().ToLowerInvariant().Substring(0, 3); // todo: test with culture != EN
				}
				cronExplanation = cronExplanation ?? "Every week";
				cronExplanation += " on " + repeatDays;
				weekDays = repeatDays;
			}
			else
			{
				days = seriesStart.Day.ToString();
				month = seriesStart.Month.ToString();
				cronExplanation += "on " + days + " " + seriesStart.ToString("MMM");
			}

			cronExplanation += " at " + seriesStart.ToString("HH:mm");
			var cronDatePart = " " + days + (repeatNature == "monthly" && repeatInterval > 1 ? " */" + repeatInterval + " " : " " + month + " ") + weekDays;
			cron += seriesStart.ToString("mm") + " " + seriesStart.ToString("HH") + cronDatePart;
			
			if (!string.IsNullOrWhiteSpace(repeatTimes))
			{
				cron += "|" + repeatTimes;
				foreach (var time in repeatTimes.Split(new []{','},StringSplitOptions.RemoveEmptyEntries))
				{
					//var timeSplit = time.Split(':');
					//cron += "|" + timeSplit[1] + " " + timeSplit[0] + cronDatePart;
					cronExplanation += " and at " + time;
				}
			}

			return new Tuple<string, string>(cron, cronExplanation);
		}

		public ProviderActionResult AddPaymentProvider(OrderInfo order, int paymentProviderId, string paymentProviderMethodId, ILocalization store)
		{
			if (ChangeOrderToIncompleteAndReturnTrueIfNotAllowed(order))
				return ProviderActionResult.NotPermitted;

			if (paymentProviderId == 0)
			{
				Log.Instance.LogError("AddPaymentProvider AddPaymentMethod ProviderActionResult.ProviderIdZero");
				return ProviderActionResult.ProviderIdZero;
			}
			var paymentProvider = PaymentProviderHelper.GetPaymentProvidersForOrder(order).SingleOrDefault(x => x.Id == paymentProviderId);

			if (paymentProvider == null)
			{	
				Log.Instance.LogError("AddPaymentProvider AddPaymentMethod paymentProvider == null");
				return ProviderActionResult.NoCorrectInput;
			}

			order.PaymentInfo.Id = paymentProviderId;
            order.PaymentInfo.Key = paymentProvider.Key;
            order.PaymentInfo.Title = paymentProvider.Title;

			order.PaymentInfo.PaymentType = paymentProvider.Type;

			var paymentMethod = paymentProvider.PaymentProviderMethods.SingleOrDefault(x => x.Id == paymentProviderMethodId);

			if (paymentMethod == null)
			{
				Log.Instance.LogError("AddPaymentProvider AddPaymentMethod paymentMethod == null");
				return ProviderActionResult.NoCorrectInput;
			}

			order.PaymentInfo.MethodId = paymentProviderMethodId;
            order.PaymentInfo.MethodKey = paymentMethod.Key;
            order.PaymentInfo.MethodTitle = paymentMethod.Title;

			if (paymentMethod.AmountType == PaymentProviderAmountType.Amount)
				order.PaymentProviderAmount = paymentMethod.PriceInCents;
			else
				order.PaymentProviderOrderPercentage = paymentMethod.PriceInCents;

			order.ResetDiscounts();

			return ProviderActionResult.Success;
		}

		public bool ChangeOrderToIncompleteAndReturnTrueIfNotAllowed(OrderInfo order)
		{
			if (!_cmsApplication.RequestIsInCMSBackend(HttpContext.Current))
			{
				if (order.Paid.GetValueOrDefault() || order.Status != OrderStatus.PaymentFailed && order.IsConfirmed() && order.Status != OrderStatus.WaitingForPayment)
				{
					return true;
				}

				if (order.Status == OrderStatus.PaymentFailed || order.Status == OrderStatus.WaitingForPayment)
				{
					Log.Instance.LogDebug("Orderstatus WILL BE CHANGED FROM: " + order.Status);
					order.Status = OrderStatus.Incomplete;
				}
			}
			return false;
		}

		public bool ConfirmOrder(OrderInfo order, bool termsAccepted, int confirmationNodeId, bool dontScheduleAlwaysConfirm = false)
		{
            Log.Instance.LogDebug("Confirm Order. confirmationNodeId: " + confirmationNodeId);

            // todo: refactor, split into Schedule and Confirm
            order.TermsAccepted = termsAccepted;

			order.ConfirmValidationFailed = _orderService.ValidateOrder(order, true).Any();
			if (order.ConfirmValidationFailed)
			{
				Save(order, true);
				Log.Instance.LogDebug("ValidateOrder == false");
				RemoveDiscountsWithCounterZeroFromOrder(order);
				return false;
			}

			// #confirm
			//   GenerateAndPersist
			//   PaymentProvider

			// #confirmUnlessPayprovFailed + ExactFinalNumberToPayProv  (current situation)
			//   Generate
			//   PaymentProvider
			//   IfSucceeded Persist Otherwise Rollback

			// #confirmUnlessPayprovFailed + uniqueNumberButNotFinalToPayProv
			//   PaymentProvider
			//   IfSucceeded GenerateAndPersist

			if (dontScheduleAlwaysConfirm || !(_uwebshopConfiguration.UseDeliveryDateAsConfirmDateForScheduledOrders && order.DeliveryDate.HasValue))
			{
				using (var orderNrTransaction = _orderNumberService.GetTransaction(order))
				{
					// set final ordernumber for payment provider communication, but not yet save it in the database
					orderNrTransaction.Generate();

					var paymentRedirectUrl = OrderHelper.HandlePaymentRequest(order, confirmationNodeId);
					// todo: this may be inappropriate as it will worsen overall responsiveness (because of the transaction)
					if (paymentRedirectUrl == "failed")
					{
						orderNrTransaction.Rollback();
						return false;
					}

				   
					order.RedirectUrl = paymentRedirectUrl;

					orderNrTransaction.Persist();
				}
			}
			else
			{
				var paymentRedirectUrl = OrderHelper.HandlePaymentRequest(order, confirmationNodeId);
				order.RedirectUrl = paymentRedirectUrl;
			}

            order.Status = !dontScheduleAlwaysConfirm && _uwebshopConfiguration.UseDeliveryDateAsConfirmDateForScheduledOrders && order.DeliveryDate.HasValue
				? OrderStatus.Scheduled : OrderStatus.Confirmed;
			Save(order, true);

			if (!dontScheduleAlwaysConfirm)
			{
				ScheduleOrdersOneYearInAdvance(order);
			}

			return true;
		}

		public void ChangeLocalization(OrderInfo order, ILocalization localization)
		{
			if (order.IsConfirmed())
			{
				Log.Instance.LogError("ChangeLocalization for NOT incomplete order: " + order.UniqueOrderId + " status: " + order.Status);
			}
			order.Localization = localization;
			order.AddStore(localization.Store);

			ReloadOrderData(order, localization);
		}

		private void ReloadOrderData(OrderInfo order, ILocalization localization)
		{
			foreach (var line in order.OrderLines)
			{
				var product = new ProductInfo(_productService.GetById(line.ProductInfo.Id, localization), order, line.ProductInfo.ItemCount.GetValueOrDefault(1));
				product.ProductVariants = line.ProductInfo.ProductVariants.Select(variant => new ProductVariantInfo(_productVariantService.GetById(variant.Id, localization), product, line.ProductInfo.ItemCount.GetValueOrDefault(1))).ToList();
				line.ProductInfo = product;
			}

			AddPaymentProvider(order, order.PaymentInfo.Id, order.PaymentInfo.MethodId, localization);
			AddShippingProvider(order, order.ShippingInfo.Id, order.ShippingInfo.MethodId, localization);

			if (order.IsConfirmed())
			{
				Log.Instance.LogError("ReloadOrderData for NOT incomplete order: " + order.UniqueOrderId + " status: " +order.Status);
			}
			
			IO.Container.Resolve<IOrderService>().UseDatabaseDiscounts(order);
			
			order.ResetCachedValues();
			order.Save();
		}

		public static void AddFieldsToXDocumentBasedOnCMSDocumentType(XDocument xDoc, Dictionary<string, string> fields, string documentAlias)
		{
			var replacedOrderlineDocTypeAlias = documentAlias.Replace(Product.NodeAlias, OrderedProduct.NodeAlias);

			var documentType = IO.Container.Resolve<ICMSDocumentTypeService>().GetByAlias(replacedOrderlineDocTypeAlias) ?? IO.Container.Resolve<ICMSDocumentTypeService>().GetByAlias(OrderedProduct.NodeAlias);


			foreach (var field in fields.Where(x => documentType.Properties.Any(y => y.Alias.ToLower() == x.Key.ToLower())))
			{
				var propertyType = documentType.Properties.FirstOrDefault(prop => prop.Alias.ToLower() == field.Key.ToLower());

				if (propertyType != null)
					ClientErrorHandling.SetOrClearErrorMessage(!string.IsNullOrEmpty(propertyType.ValidationRegularExpression) && new Regex(propertyType.ValidationRegularExpression).IsMatch(field.Value), "Error in field: " + field.Key, field.Key, field.Value);
				// todo: customize error message
			}

			var xNodeList = fields.Where(field => documentType.Properties.Any(x => x.Alias.ToLower() == field.Key.ToLower())).Select(field => field.Value != null ? new XElement(field.Key, new XCData(field.Value)) : null).Cast<XNode>().ToList();

			if (xDoc.Root != null)
			{
				xDoc.Root.Add(xNodeList);
			}
		}

		private static OrderLine OrderProduct(int productId, IOrderedEnumerable<int> variants, int itemCount, OrderInfo order)
		{
			var currentLocalization = StoreHelper.CurrentLocalization;
			var productService = IO.Container.Resolve<IProductService>();
			var productInfo = productService.CreateProductInfoByProductId(productId, order, currentLocalization, itemCount);
			if (variants != null && variants.Any())
			{
				var productVariantService = IO.Container.Resolve<IProductVariantService>();
				var variantClasses = variants.Select(variant => productVariantService.GetById(variant, currentLocalization)).Where(variant => variant != null).GroupBy(a => a.Group).Select(g => g.FirstOrDefault()).Where(variant => variant != null);

				productInfo.ProductVariants = variantClasses.Select(variant => new ProductVariantInfo(variant, productInfo, itemCount)).ToList();
			}
			return new OrderLine(productInfo, order);
		}
	}
}