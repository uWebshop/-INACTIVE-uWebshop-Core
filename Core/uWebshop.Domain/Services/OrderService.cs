using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using System.Xml.Linq;
using uWebshop.Common;
using uWebshop.DataAccess;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Domain.Services
{
	internal class OrderService : IOrderService
	{
		private readonly IOrderRepository _orderRepository;
		private readonly IVatCalculationStrategy _newOrdersDefaultVatCalculationStrategy;
		private readonly IStoreService _storeService;

		public OrderService(IStoreService storeService, IOrderRepository orderRepository, IVatCalculationStrategy newOrdersDefaultVatCalculationStrategy)
		{
			_storeService = storeService;
			_orderRepository = orderRepository;
			_newOrdersDefaultVatCalculationStrategy = newOrdersDefaultVatCalculationStrategy;
		}

		public OrderInfo CreateOrder()
		{
			return CreateOrder(StoreHelper.GetCurrentStore()); //_storeService.GetCurrentStore()
		}

		public OrderInfo CreateOrder(Store store)
		{
			if (store == null)
			{
				throw new Exception("Trying to create order without store");
			}
			if (string.IsNullOrEmpty(store.Alias))
			{
				throw new Exception("Store without alias. Please (re)publish store and re-index examine External index");
			}

			var order = new OrderInfo();

			order.CreatedInTestMode = store.EnableTestmode;
			order.CustomerInfo.CustomerIPAddress = HttpContext.Current.Request.UserHostAddress;

			order.ShippingCostsMightBeOutdated = true;

			order.VATCheckService = IO.Container.Resolve<IVATCheckService>();
			InitializeNewOrder(order);
			UseDatabaseDiscounts(order);

			order.CustomerInfo.CountryCode = !string.IsNullOrEmpty(store.DefaultCountryCode) ? store.DefaultCountryCode : store.CountryCode;

			var currentMember = Membership.GetUser(); // TODO: dip

			if (currentMember != null)
			{
				//uWebshopOrders.SetCustomer(order.UniqueOrderId, currentMember.UserName); heeft toch geen effect (geen row in db)
				order.CustomerInfo.LoginName = currentMember.UserName;
				if (currentMember.ProviderUserKey != null)
				{
					//uWebshopOrders.SetCustomerId(order.UniqueOrderId, (int)currentMember.ProviderUserKey); heeft toch geen effect (geen row in db)
					order.CustomerInfo.CustomerId = (int)currentMember.ProviderUserKey;
				}
			}

			order.Localization = StoreHelper.CurrentLocalization; // todo clean
			order.AddStore(store);

			order.Save();

			return order;
		}

		public void UseStoredDiscounts(OrderInfo order, List<IOrderDiscount> discounts)
		{
			order.OrderDiscountsFactory = () => new List<IOrderDiscount>(discounts);
		}

		public void UseDatabaseDiscounts(OrderInfo order)
		{
			var discountService = IO.Container.Resolve<IOrderDiscountService>();
			order.OrderDiscountsFactory = () => discountService.GetApplicableDiscountsForOrder(order, order.Localization).ToList();
		}

		public Guid GetOrderIdFromOrderIdCookie()
		{
			var httpContext = HttpContext.Current;
			if (httpContext == null)
			{
				return Guid.Empty;
			}
			var cookieName = OrderHelper.GetOrderCookieName();

			var orderIdCookie = httpContext.Request.Cookies[cookieName];

			if (orderIdCookie != null && !string.IsNullOrEmpty(orderIdCookie.Value))
			{
				Guid uniqueOrderId;
				if (Guid.TryParse(orderIdCookie.Value, out uniqueOrderId))
				{
					return uniqueOrderId;
				}
			}

			return Guid.Empty;
		}

		public OrderInfo CreateCopyOfOrder(OrderInfo order)
		{
			InitializeNewOrder(order);
			order.PaymentInfo.TransactionId = string.Empty;

			order.Save();

			return order;
		}

		/// <summary>
		///     Does the give order contains items that are out of stock?
		/// </summary>
		/// <param name="order"></param>
		/// <returns></returns>
		public bool OrderContainsOutOfStockItem(OrderInfo order)
		{
			return !ValidateStock(order, true, false);
		}

		public bool OrderContainsItem(OrderInfo orderinfo, IEnumerable<int> itemIdsToCheck)
		{
			return GetApplicableOrderLines(orderinfo, itemIdsToCheck).Any();
		}

		public List<OrderLine> GetApplicableOrderLines(OrderInfo orderinfo, IEnumerable<int> itemIdsToCheck)
		{
			var productIds = new List<int>();
			var productVariantIds = new List<int>();

			var objects = itemIdsToCheck.Select(id => IO.Container.Resolve<ICMSEntityRepository>().GetByGlobalId(id)).Where(o => o != null);
			if (!objects.Any())
			{
				return new List<OrderLine>();
			}

			foreach (var node in objects)
			{
				var itemId = node.Id;
				if (Category.IsAlias(node.NodeTypeAlias))
				{
					var category = DomainHelper.GetCategoryById(itemId);
					if (category == null || category.Disabled) continue;
					productIds.AddRange(category.ProductsRecursive.Select(product => product.Id));
					productIds.Add(itemId);
				}

				if (Product.IsAlias(node.NodeTypeAlias))
				{
					productIds.Add(node.Id);
				}

				if (node.NodeTypeAlias.StartsWith(ProductVariant.NodeAlias))
				{
					productVariantIds.Add(node.Id);
				}

				if (node.NodeTypeAlias.StartsWith(PaymentProvider.NodeAlias) && orderinfo.PaymentInfo.Id != node.Id)
				{
					return new List<OrderLine>();
				}

				if (node.NodeTypeAlias == PaymentProviderMethod.NodeAlias && orderinfo.PaymentInfo.MethodId != node.Id.ToString())
				{
					return new List<OrderLine>();
				}

				if (node.NodeTypeAlias.StartsWith(ShippingProvider.NodeAlias) && orderinfo.ShippingInfo.Id != node.Id)
				{
					return new List<OrderLine>();
				}

				if (node.NodeTypeAlias == ShippingProviderMethod.NodeAlias && orderinfo.ShippingInfo.MethodId != node.Id.ToString())
				{
					return new List<OrderLine>();
				}

				if (node.NodeTypeAlias.StartsWith(DiscountProduct.NodeAlias) && orderinfo.OrderLines.Any(x => x.ProductInfo.CatalogProduct != null && x.ProductInfo.CatalogProduct.Discount.Id != node.Id))
				{
					return new List<OrderLine>();
				}
			}

			//todo:test
			return orderinfo.OrderLines.Where(orderLine => !productIds.Any() || productIds.Contains(orderLine.ProductInfo.Id)).Where(orderLine => !productVariantIds.Any() || orderLine.ProductInfo.ProductVariants.Any(x => productVariantIds.Contains(x.Id))).ToList();
		}

		public bool ValidateCustomer(OrderInfo orderInfo, bool clearValidation, bool writeToOrderValidation = true)
		{
			if (clearValidation)
			{
				orderInfo.OrderValidationErrors.Clear();
			}
			var errors = ValidateCustomer(orderInfo);
			if (writeToOrderValidation)
			{
				orderInfo.OrderValidationErrors.AddRange(errors);
			}
			return !errors.Any();
		}

		public List<OrderValidationError> ValidateCustomer(OrderInfo orderInfo)
		{
			var errors = new List<OrderValidationError>();
			if (string.IsNullOrEmpty(OrderHelper.CustomerInformationValue(orderInfo, "customerEmail")))
			{
				// no customer address available in the order
				//ClientErrorHandling.uWebshopException(dicCustomerEmailEmpty);
				//Log.Instance.LogDebug( "ORDERVALIDATIONERROR: customerEmail is Empty");

				orderInfo.OrderValidationErrors.Add(new OrderValidationError { Key = "ValidationCustomerEmailEmpty", Value = "CustomerEmail Is Not Set" });
			}

			var orderDocumentType = IO.Container.Resolve<ICMSDocumentTypeService>().GetByAlias(Order.NodeAlias);

			errors.AddRange(ValidateInformationDoc(orderInfo.CustomerInfo.customerInformation, orderDocumentType.Properties.Where(property => property.Alias.StartsWith("customer"))));
			errors.AddRange(ValidateInformationDoc(orderInfo.CustomerInfo.shippingInformation, orderDocumentType.Properties.Where(property => property.Alias.StartsWith("shipping"))));
			errors.AddRange(ValidateInformationDoc(orderInfo.CustomerInfo.extraInformation, orderDocumentType.Properties.Where(property => property.Alias.StartsWith("extra"))));

			return errors;
		}

		public bool ValidateOrderlines(OrderInfo orderInfo, bool clearValidation, bool writeToOrderValidation = true)
		{
			if (clearValidation)
			{
				orderInfo.OrderValidationErrors.Clear();
			}
			var errors = ValidateOrderlines(orderInfo);
			if (writeToOrderValidation)
			{
				orderInfo.OrderValidationErrors.AddRange(errors);
			}
			return errors.Any();
		}
		public List<OrderValidationError> ValidateOrderlines(OrderInfo orderInfo)
		{
			var errors = new List<OrderValidationError>();

			foreach (var orderline in orderInfo.OrderLines)
			{
				var replacedOrderlineDocTypeAlias = orderline.ProductInfo.CatalogProduct.NodeTypeAlias().Replace(Product.NodeAlias, OrderedProduct.NodeAlias);

				var orderlineDocType = IO.Container.Resolve<ICMSDocumentTypeService>().GetByAlias(replacedOrderlineDocTypeAlias) ?? IO.Container.Resolve<ICMSDocumentTypeService>().GetByAlias(OrderedProduct.NodeAlias);

				var propertiesToValidate = orderlineDocType.Properties.Where(x => !OrderedProduct.DefaultProperties.Contains(x.Alias)).ToList();

				errors.AddRange(ValidateInformationDoc(new XDocument(orderline.CustomData), propertiesToValidate));

				var product = orderline.ProductInfo.CatalogProduct;
				if (product == null) continue;
				// for each productvariantgroup on the product node
				foreach (var productvariantGroup in product.VariantGroups)
				{
					// test if the productgroup is required
					if (!productvariantGroup.Variants.Any(x => x.Required) || orderline.ProductInfo.ProductVariants.Count(x => x.Group == productvariantGroup.Title) != 0)
						continue;
					// productvariantgroup has required variants (so whole group is required!) but non are in the productinfo == validation error
					Log.Instance.LogWarning("ORDERVALIDATIONERROR: Product Variant Group " + productvariantGroup.Title + " Is Required");
					errors.Add(new OrderValidationError { Id = orderline.ProductInfo.Id, Key = "ValidationProductVariantRequired", Value = "Product Variant Group " + productvariantGroup.Title + " Is Required" });
				}
			}

			return errors;
		}

		/// <summary>
		/// Validate stock
		/// </summary>
		/// <param name="orderInfo"></param>
		/// <param name="clearValidation"></param>
		/// <param name="writeToOrderValidation"></param>
		/// <returns>True = valid, False = Invalid</returns>
		public bool ValidateStock(OrderInfo orderInfo, bool clearValidation, bool writeToOrderValidation = true)
		{
			if (clearValidation)
			{
				orderInfo.OrderValidationErrors.Clear();
			}
			var errors = ValidateStock(orderInfo);
			if (writeToOrderValidation)
			{
				orderInfo.OrderValidationErrors.AddRange(errors);
			}
			return !errors.Any();
		}
		public List<OrderValidationError> ValidateStock(OrderInfo orderInfo)
		{
			var errors = new List<OrderValidationError>();
			var allProducts = new Dictionary<int, int>();

			foreach (var orderline in orderInfo.OrderLines)
			{
				var productId = orderline.ProductInfo.Id;

				if (allProducts.ContainsKey(productId))
				{
					var currentItemCount = allProducts[productId];
					var itemCount = currentItemCount + orderline.ProductInfo.ItemCount.GetValueOrDefault(1);

					allProducts.Remove(productId);
					allProducts.Add(productId, itemCount);
				}
				else
				{
					var itemCount = orderline.ProductInfo.ItemCount.GetValueOrDefault(1);

					allProducts.Remove(productId);
					allProducts.Add(productId, itemCount);
				}
			}

			var allVariants = new Dictionary<int, int>();

			foreach (var orderline in orderInfo.OrderLines)
			{
				foreach (var variant in orderline.ProductInfo.ProductVariants)
				{
					var variantId = variant.Id;

					if (allVariants.ContainsKey(variantId))
					{
						var currentItemCount = allVariants[variantId];
						var itemCount = currentItemCount + orderline.ProductInfo.ItemCount.GetValueOrDefault(1);

						allVariants.Remove(variantId);
						allVariants.Add(variantId, itemCount);
					}
					else
					{
						var itemCount = orderline.ProductInfo.ItemCount.GetValueOrDefault(1);

						allVariants.Remove(variantId);
						allVariants.Add(variantId, itemCount);
					}
				}
			}

			// todo: a bit slow and untestable because of ProductInfo.Product
			foreach (var productKey in allProducts)
			{
				var product = DomainHelper.GetProductById(productKey.Key);
				if (product == null) continue;

				// if stockstatus is enabled, use stock
				if (product.StockStatus && !product.UseVariantStock)
				{
					// does the product has stock
					if (product.Stock > 0)
					{
						// is the stock equal or more then the itemcount
						//if (product.Stock >= orderline.ProductInfo.ItemCount.GetValueOrDefault(1))
						//{
						//	// ok!
						//}

						// is the stock less then the itemcount, but backorder is enabled
						//if (product.Stock < orderline.ProductInfo.ItemCount.GetValueOrDefault(1) && product.BackorderStatus)
						//{
						//	// ok!
						//}

						if (product.Stock < productKey.Value && !product.BackorderStatus)
						{
							errors.Add(new OrderValidationError { Id = product.Id, Key = "ValidationProductOutOfStock", Value = "Product " + product.Title + " Is Out Of Stock", });
						}
						// if usevariantstock is true, do not update product stock
						//if (product.UseVariantStock)
						//{
						//	// ok!
						//}
					}
					// if product has no stock, but backorder is enabled
					//if (product.Stock <= 0 && product.BackorderStatus)
					//{
					//	// ok!
					//}
					// if variant has no stock, but backorder is disabled
					if (product.Stock <= 0 && !product.BackorderStatus)
					{
						errors.Add(new OrderValidationError { Id = product.Id, Key = "ValidationProductOutOfStock", Value = "Product " + product.Title + " Is Out Of Stock", });
					}
				}

				foreach (var variantkey in allVariants)
				{
					var variant = DomainHelper.GetProductVariantById(variantkey.Key);

					// if stockstatus is enabled, use stock
					if (variant.StockStatus)
					{
						// does the variant has stock
						if (variant.Stock > 0)
						{
							// is the stock equal or more then the itemcount
							//if (variant.Variant.Stock >= orderline.ProductInfo.ItemCount.GetValueOrDefault(1))
							//{
							//	// ok!
							//}

							// is the stock less then the itemcount, but backorder is enabled
							//if (variant.Variant.Stock < orderline.ProductInfo.ItemCount.GetValueOrDefault(1) && variant.Variant.BackorderStatus)
							//{
							//	// ok!
							//}

							// is the stock less then the itemcount, but backorder is disabled
							if (variant.Stock < variantkey.Value && !variant.BackorderStatus)
							{
								errors.Add(new OrderValidationError { Id = variant.Id, Key = "ValidationProductVariantOutOfStock", Value = "Product " + product.Title + " variant " + variant.Title + " Is Out Of Stock", });
							}
						}
						// if variant has no stock, but backorder is enabled
						//if (variant.Variant.Stock <= 0 && variant.Variant.BackorderStatus)
						//{
						//	// ok!
						//}
						// if variant has no stock, but backorder is disabled
						if (variant.Stock <= 0 && !variant.BackorderStatus)
						{
							errors.Add(new OrderValidationError { Id = variant.Id, Key = "ValidationProductVariantOutOfStock", Value = "Product " + product.Title + " variant " + variant.Title + " Is Out Of Stock", });
						}
					}

					// if stockstatus is disabled, stock is not needed
					//if (!variant.Variant.StockStatus)
					//{
					//	// ok!
					//}
				}
			}

			return errors;
		}

		public List<OrderValidationError> ValidateOrder(OrderInfo orderInfo, bool confirmValidation = false)
		{
			var errors = new List<OrderValidationError>();
			orderInfo.OrderValidationErrors.Clear();

			if (confirmValidation)
			{
				orderInfo.ConfirmValidationFailed = true;
				errors.AddRange(ValidateGlobalValidations(orderInfo));
				errors.AddRange(ValidatePayment(orderInfo));
				errors.AddRange(ValidateShipping(orderInfo));
			}

			errors.AddRange(ValidateCustomer(orderInfo));
			errors.AddRange(ValidateStock(orderInfo));
			errors.AddRange(ValidateOrderlines(orderInfo));
			errors.AddRange(ValidateCustomValidations(orderInfo));


			return errors;
		}

		public List<OrderValidationError> ValidateGlobalValidations(OrderInfo orderInfo)
		{
			var errors = new List<OrderValidationError>();
			if (orderInfo.ConfirmValidationFailed)
			{
				if (!orderInfo.TermsAccepted)
				{
					Log.Instance.LogWarning("ORDERVALIDATIONERROR: TERMS NOT ACCEPTED");
					errors.Add(new OrderValidationError { Key = "AcceptTermsError", Value = "Terms Not Accepted" });
				}

				var shippingProvidersForOrder = ShippingProviderHelper.GetShippingProvidersForOrder(orderInfo);
				if (orderInfo.Status == OrderStatus.Confirmed && orderInfo.ShippingCostsMightBeOutdated &&
					shippingProvidersForOrder.Count > 0
					&& shippingProvidersForOrder.All(shipPro => shipPro.Id != orderInfo.ShippingInfo.Id))
				{
					Log.Instance.LogWarning("ORDERVALIDATIONERROR: ShippingCostOutdatedError");
					errors.Add(new OrderValidationError
							   {
								   Key = "ShippingCostOutdatedError",
								   Value = "Shipping Not Updated Since Last Basket Change"
							   });
				}

				if (orderInfo.Status == OrderStatus.Confirmed && orderInfo.StoreInfo.Store == null)
				{
					Log.Instance.LogWarning("ORDERVALIDATIONERROR: NoStoreConnectedToThisOrder");
					errors.Add(new OrderValidationError
							   {
								   Key = "NoStoreConnectedToThisOrder",
								   Value = "There Is No Store Connected To This Order"
							   });
				}
			}
			return errors;
		}

		public bool ValidateCustomValidations(OrderInfo orderInfo, bool writeToOrderValidation = true)
		{
			var errors = ValidateCustomValidations(orderInfo);
			if (writeToOrderValidation)
			{
				orderInfo.OrderValidationErrors.AddRange(errors);
			}
			return errors.Any();
		}

		public List<OrderValidationError> ValidateCustomValidations(OrderInfo orderInfo)
		{
			var errors = new List<OrderValidationError>();
			foreach (var customValidation in orderInfo.CustomOrderValiations.Where(customValidation => !customValidation.Condition(orderInfo)))
			{
				Log.Instance.LogWarning("VALIDATECUSTOMER ERROR CustomOrderValiations: " + customValidation.ErrorDictionaryItem);
				errors.Add(new OrderValidationError { Key = customValidation.ErrorDictionaryItem(orderInfo) });
			}
			return errors;
		}

		public List<OrderValidationError> ValidatePayment(OrderInfo orderInfo)
		{
			try
			{
				if (orderInfo == null) throw new ArgumentNullException("orderInfo", "Geen order?!");
				if (orderInfo.PaymentInfo == null) throw new NullReferenceException("orderInfo.PaymentInfo");

				var errors = new List<OrderValidationError>();
				if (orderInfo.PaymentInfo.Id != 0)
				{
					var paymentProvider = PaymentProvider.GetPaymentProvider(orderInfo.PaymentInfo.Id);
					if (paymentProvider == null) throw new NullReferenceException("paymentProvider");
					if (paymentProvider.Zones == null) throw new NullReferenceException("paymentProvider.Zones");

					if ((paymentProvider.Type != PaymentProviderType.OfflinePaymentAtCustomer && paymentProvider.Type != PaymentProviderType.OfflinePaymentInStore) && !paymentProvider.Zones.SelectMany(x => x.CountryCodes).Contains(orderInfo.CustomerInfo.CountryCode))
					{
						// country code for customer does not match zones for payment provider.
						Log.Instance.LogWarning("ORDERVALIDATIONERROR: CUSTOMER COUNTRY DOES NOT MATCH PAYMENT PROVIDER");
						errors.Add(new OrderValidationError { Id = orderInfo.PaymentInfo.Id, Key = "ValidationCustomerCountryPaymentProviderMismatch", Value = "The Customer Country Does Not Match Countries Allowed For The Chosen Payment Provider" });
					}
					errors.AddRange(PaymentProviderHelper.GetPaymentValidationResults(orderInfo).Where(e => e.Id == orderInfo.PaymentInfo.Id));
				}
				if (orderInfo.ConfirmValidationFailed && (orderInfo.PaymentInfo.Id == 0 && PaymentProviderHelper.GetPaymentProvidersForOrder(orderInfo).Count > 0))
				{
					Log.Instance.LogWarning("ORDERVALIDATIONERROR: PAYMENT PROVIDERS AVAILABLE BUT NOT CHOSEN");
					errors.Add(new OrderValidationError { Id = 0, Key = "ValidationNoPaymentProviderChosen", Value = "No Payment Provider Chosen" });
				}
				return errors;
			}
			catch (Exception)
			{
				return new List<OrderValidationError>();
			}
		}

		public List<OrderValidationError> ValidateShipping(OrderInfo orderInfo)
		{
			var errors = new List<OrderValidationError>();
			if (orderInfo.ShippingInfo.Id != 0)
			{
				var shippingProvider = ShippingProviderHelper.GetShippingProvider(orderInfo.ShippingInfo.Id);

				var shippingCountryCode = orderInfo.CustomerInfo.ShippingCountryCode;

				// if shipping country is empty on the order, then use customercountry for validation
				if (string.IsNullOrEmpty(shippingCountryCode))
				{
					shippingCountryCode = orderInfo.CustomerInfo.CountryCode;
				}

				if (shippingProvider.Type != ShippingProviderType.Pickup && !shippingProvider.Zone.CountryCodes.Contains(shippingCountryCode))
				{
					Log.Instance.LogWarning("ORDERVALIDATIONERROR: SHIPPING COUNTRY DOES NOT MATCH SHIPPING PROVIDER");
					errors.Add(new OrderValidationError { Id = orderInfo.ShippingInfo.Id, Key = "ValidationShippingCountryShippingProviderMismatch", Value = "The Shipping Country Does Not Match Countries Allowed For The Chosen Shipping Provider" });
				}
				errors.AddRange(ShippingProviderHelper.GetPaymentValidationResults(orderInfo).Where(e => e.Id == orderInfo.ShippingInfo.Id));
			}

			if (orderInfo.ConfirmValidationFailed && orderInfo.ShippingInfo.Id == 0 && ShippingProviderHelper.GetShippingProvidersForOrder(orderInfo).Count > 0)
			{
				Log.Instance.LogWarning("ORDERVALIDATIONERROR: SHIPPING PROVIDERS AVAILABLE BUT NOT CHOSEN");
				errors.Add(new OrderValidationError { Id = 0, Key = "ValidationNoShippingProviderChosen", Value = "No Shipping Provider Chosen" });
			}
			return errors;
		}

		private void InitializeNewOrder(OrderInfo order)
		{
			order.UniqueOrderId = Guid.NewGuid();
			order.OrderNumber = GenerateIncompleteOrderNumber();
			order.StoreOrderReferenceId = null;
			order.DatabaseId = 0;

			order.VatCalculationStrategy = _newOrdersDefaultVatCalculationStrategy;
			//order.EventsOn = false;
			order.Paid = false;
			order.Status = OrderStatus.Incomplete;
			order.EventsOn = true;
		}

		internal string GenerateIncompleteOrderNumber()
		{
			int lastOrderId = _orderRepository.DetermineLastOrderId();

			const string prefix = "[INCOMPLETE]-";
			return string.Format("{0}{1}", prefix, (lastOrderId + 1).ToString("0000"));
		}

		private static List<OrderValidationError> ValidateInformationDoc(XDocument customerInformationdoc, IEnumerable<IDocumentProperty> customerProperties)
		{
			var errors = new List<OrderValidationError>();
			if (customerInformationdoc == null) return errors;

			foreach (var propertyType in customerProperties)
			{
				var xmlElement = customerInformationdoc.Descendants().FirstOrDefault(x => x.Name.LocalName == propertyType.Alias);
				if (propertyType != null && xmlElement != null && (!string.IsNullOrEmpty(propertyType.ValidationRegularExpression) && !Regex.IsMatch(xmlElement.Value, propertyType.ValidationRegularExpression)))
				{
					Log.Instance.LogWarning("VALIDATECUSTOMER ERROR ValidationErrorRegEx: " + propertyType.Alias + " Is Not Correctly Set");
					errors.Add(new OrderValidationError { Key = "ValidationErrorRegEx", Name = propertyType.Name, Alias = propertyType.Alias, Value = propertyType.Alias + " Is Not Correctly Set", });
				}

				if (propertyType != null && propertyType.Mandatory && (xmlElement == null || string.IsNullOrEmpty(xmlElement.Value)))
				{
					Log.Instance.LogWarning("VALIDATECUSTOMER ERROR ValidationErrorMandatory: " + propertyType.Alias + " Is Not Set");
					errors.Add(new OrderValidationError { Key = "ValidationErrorMandatory", Name = propertyType.Name, Alias = propertyType.Alias, Value = propertyType.Alias + " Is Not Set", });
				}
			}
			return errors;
		}
	}
}