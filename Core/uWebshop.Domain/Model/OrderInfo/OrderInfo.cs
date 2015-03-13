using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Xml.Linq;
using System.Xml.Serialization;
using uWebshop.API;
using uWebshop.Common;
using uWebshop.Common.Interfaces;
using uWebshop.DataAccess;
using uWebshop.Domain.Businesslogic.VATChecking;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;
using uWebshop.Domain.Model;
using uWebshop.Domain.Services;
using uWebshop.Domain.XMLRendering;
using IOrderDiscount = uWebshop.Domain.Interfaces.IOrderDiscount;

namespace uWebshop.Domain
{
	/// <summary>
	/// 
	/// </summary>
	[DataContract(Namespace = "")]
	[Serializable]
	public class OrderInfo : IOrderInfo, IDatabaseOrder, IDiscountableUnit, IAmountUnit
	{
		#region events

		/// <summary>
		/// 
		/// </summary>
		/// <param name="orderInfo">The order information.</param>
		/// <param name="e">The <see cref="AfterOrderLineCreatedEventArgs"/> instance containing the event data.</param>
		public delegate void AfterOrderLineCreatedEventHandler(OrderInfo orderInfo, AfterOrderLineCreatedEventArgs e);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="orderInfo">The order information.</param>
		/// <param name="e">The <see cref="AfterOrderLineDeletedEventArgs"/> instance containing the event data.</param>
		public delegate void AfterOrderLineDeletedEventHandler(OrderInfo orderInfo, AfterOrderLineDeletedEventArgs e);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="orderInfo">The order information.</param>
		/// <param name="e">The <see cref="AfterOrderLineUpdatedEventArgs"/> instance containing the event data.</param>
		public delegate void AfterOrderLineUpdatedEventHandler(OrderInfo orderInfo, AfterOrderLineUpdatedEventArgs e);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="orderInfo">The order information.</param>
		/// <param name="e">The <see cref="AfterOrderStatusChangedEventArgs"/> instance containing the event data.</param>
		public delegate void AfterOrderStatusChangedEventHandler(OrderInfo orderInfo, AfterOrderStatusChangedEventArgs e);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="orderInfo">The order information.</param>
		/// <param name="e">The <see cref="AfterOrderUpdatedEventArgs"/> instance containing the event data.</param>
		public delegate void AfterOrderUpdatedEventHandler(OrderInfo orderInfo, AfterOrderUpdatedEventArgs e);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="orderInfo">The order information.</param>
		/// <param name="e">The <see cref="BeforeOrderLineCreatedEventArgs"/> instance containing the event data.</param>
		public delegate void BeforeOrderLineCreatedEventHandler(OrderInfo orderInfo, BeforeOrderLineCreatedEventArgs e);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="orderInfo">The order information.</param>
		/// <param name="e">The <see cref="BeforeOrderLineDeletedEventArgs"/> instance containing the event data.</param>
		public delegate void BeforeOrderLineDeletedEventHandler(OrderInfo orderInfo, BeforeOrderLineDeletedEventArgs e);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="orderInfo">The order information.</param>
		/// <param name="e">The <see cref="BeforeOrderLineUpdatedEventArgs"/> instance containing the event data.</param>
		public delegate void BeforeOrderLineUpdatedEventHandler(OrderInfo orderInfo, BeforeOrderLineUpdatedEventArgs e);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="orderInfo">The order information.</param>
		/// <param name="e">The <see cref="BeforeOrderStatusChangedEventArgs"/> instance containing the event data.</param>
		public delegate void BeforeOrderStatusChangedEventHandler(OrderInfo orderInfo, BeforeOrderStatusChangedEventArgs e);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="orderInfo">The order information.</param>
		/// <param name="e">The <see cref="BeforeOrderUpdatedEventArgs"/> instance containing the event data.</param>
		public delegate void BeforeOrderUpdatedEventHandler(OrderInfo orderInfo, BeforeOrderUpdatedEventArgs e);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="orderInfo">The order information.</param>
		public delegate void OrderLoadedEventHandler(OrderInfo orderInfo);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="orderInfo">The order information.</param>
		/// <param name="e">The <see cref="OrderPaidChangedEventArgs"/> instance containing the event data.</param>
		public delegate void OrderPaidChangedEventHandler(OrderInfo orderInfo, OrderPaidChangedEventArgs e);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="orderInfo">The order information.</param>
		/// <param name="e">The <see cref="OrderFulfilledChangedEventArgs"/> instance containing the event data.</param>
		public delegate void OrderFulfilledChangedEventHandler(OrderInfo orderInfo, OrderFulfillChangedEventArgs e);

		/// <summary>
		/// Occurs when [before order updated].
		/// </summary>
		public static event BeforeOrderUpdatedEventHandler BeforeOrderUpdated;

		/// <summary>
		/// Occurs when [after order updated].
		/// </summary>
		public static event AfterOrderUpdatedEventHandler AfterOrderUpdated;

		/// <summary>
		/// Occurs when [before status changed].
		/// </summary>
		public static event BeforeOrderStatusChangedEventHandler BeforeStatusChanged;

		/// <summary>
		/// Occurs when [after status changed].
		/// </summary>
		public static event AfterOrderStatusChangedEventHandler AfterStatusChanged;

		/// <summary>
		/// Occurs when [before order line created].
		/// </summary>
		public static event BeforeOrderLineCreatedEventHandler BeforeOrderLineCreated;

		/// <summary>
		/// Occurs when [after order line created].
		/// </summary>
		public static event AfterOrderLineCreatedEventHandler AfterOrderLineCreated;

		/// <summary>
		/// Occurs when [before order line updated].
		/// </summary>
		public static event BeforeOrderLineUpdatedEventHandler BeforeOrderLineUpdated;

		/// <summary>
		/// Occurs when [after order line updated].
		/// </summary>
		public static event AfterOrderLineUpdatedEventHandler AfterOrderLineUpdated;

		/// <summary>
		/// Occurs when [before order line deleted].
		/// </summary>
		public static event BeforeOrderLineDeletedEventHandler BeforeOrderLineDeleted;

		/// <summary>
		/// Occurs when [after order line deleted].
		/// </summary>
		public static event AfterOrderLineDeletedEventHandler AfterOrderLineDeleted;

		/// <summary>
		/// Occurs when [order paid changed].
		/// </summary>
		public static event OrderPaidChangedEventHandler OrderPaidChanged;

		/// <summary>
		/// Occurs when [order fulfilled changed].
		/// </summary>
		public static event OrderFulfilledChangedEventHandler OrderFulfillChanged;

		/// <summary>
		/// Occurs when [order loaded].
		/// </summary>
		public static event OrderLoadedEventHandler OrderLoaded;

		#endregion

		// dependencies

		private readonly List<CustomOrderValidation> _customValidations = new List<CustomOrderValidation>();

		/// <summary>
		/// The coupon codes data
		/// </summary>
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[XmlArray("CouponCodes")] //[XmlArrayItem("CouponCode")]
		public List<string> CouponCodesData = new List<string>();
		
		/// <summary>
		/// The customer information
		/// </summary>
		[DataMember]
		public CustomerInfo CustomerInfo;

		internal bool EventsOn = false;
		internal bool LegacyDataReadBackMode = false;

		/// <summary>
		/// [Obsolete!! Use ConfirmDate] Gets the unique orderdate for the order
		/// </summary>
		//[Obsolete("Use ConfirmDate")] can't use [Obsolete] because of serialization
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[DataMember]
		public string OrderDate;

		internal Func<List<IOrderDiscount>> OrderDiscountsFactory;

		/// <summary>
		/// Gets the vat calculation strategy.
		/// </summary>
		/// <value>
		/// The vat calculation strategy.
		/// </value>
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[IgnoreDataMember]
		[XmlIgnore]
		public IVatCalculationStrategy VatCalculationStrategy { get; set; }

		/// <summary>
		/// Gets the unique ordernumber for the order
		/// </summary>
		[DataMember]
		public string OrderNumber;

		/// <summary>
		/// The payment information
		/// </summary>
		[DataMember]
		public PaymentInfo PaymentInfo;

		/// <summary>
		/// Gets the payment provider costs of the order
		/// </summary>
		/// <value>
		/// The payment provider price in cents.
		/// </value>
		[DataMember]
		public int PaymentProviderPriceInCents
		{
			get
			{
                //if (PaymentProviderOrderPercentage > 0)
                //{
                //    return Convert.ToInt32(OrderTotalWithoutPaymentInCents * PaymentProviderOrderPercentage / 10000);
                //}
                //return PaymentProviderAmount;

                if (PaymentProviderAmount > 0 && PaymentProviderOrderPercentage > 0)
                {
                    return Convert.ToInt32(PaymentProviderAmount + OrderTotalWithoutPaymentInCents * PaymentProviderOrderPercentage / 100);
                }
                if (PaymentProviderOrderPercentage > 0)
                {
                    return Convert.ToInt32(OrderTotalWithoutPaymentInCents * PaymentProviderOrderPercentage / 100);
                }
                return PaymentProviderAmount;
			}
			set { }
		}

		internal int PaymentProviderAmount;
		internal decimal PaymentProviderOrderPercentage;

		/// <summary>
		/// The preference validate save action
		/// </summary>
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public ValidateSaveAction ReValidateSaveAction; // version 2.1 hack

		/// <summary>
		/// The revalidate order configuration load
		/// </summary>
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public bool? RevalidateOrderOnLoad; // version 2.1 hack

		/// <summary>
		/// The shipping information
		/// </summary>
		[DataMember]
		public ShippingInfo ShippingInfo;

		/// <summary>
		/// Gets the shipping costs of the order
		/// </summary>
		[DataMember]
		public int ShippingProviderAmountInCents;

		/// <summary>
		/// The store information
		/// </summary>
		[DataMember]
		public StoreInfo StoreInfo { get; set; }

		/// <summary>
		/// Gets the localization.
		/// </summary>
		/// <value>
		/// The localization.
		/// </value>
		[IgnoreDataMember]
		[XmlIgnore]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ILocalization Localization { get; set; }

		/// <summary>
		/// The unique per store id for the order
		/// </summary>
		[DataMember]
		public int? StoreOrderReferenceId;

		/// <summary>
		/// Unique Order Id
		/// </summary>
		[DataMember]
		public Guid UniqueOrderId;

		/// <summary>
		/// Gets the name given to this order.
		/// </summary>
		/// <value>
		/// The name.
		/// </value>
		[DataMember]
		public string Name { get; set; }

		internal IVATCheckService VATCheckService;
		private DateTime? _confirmDate;
		private bool? _pricesAreIncludingVAT;
		internal int? _regionalVatInCents;
		private bool? _vatCharged;

		#region Status

		private OrderStatus _status;

		/// <summary>
		/// Gets the status of the order
		/// </summary>
		/// <value>
		/// The status.
		/// </value>
		[DataMember]
		public OrderStatus Status
		{
			get { return _status; }
			set { SetStatus(value); }
		}

		/// <summary>
		/// Sets the status.
		/// </summary>
		/// <param name="newStatus">The new status.</param>
		/// <param name="sendEmails">if set to <c>true</c> [send emails].</param>
		public void SetStatus(OrderStatus newStatus, bool sendEmails = true)
		{
			//Log.Instance.LogDebug("SetStatus Start SetStatus :" + newStatus);
			if (EventsOn && BeforeStatusChanged != null)
			{
				BeforeStatusChanged(this, new BeforeOrderStatusChangedEventArgs { OrderInfo = this, OrderStatus = _status });
			}
			//Log.Instance.LogDebug("SetStatus After BeforeStatusChanged :" + newStatus);

			//Log.Instance.LogDebug("SetStatus Before datetime :" + newStatus);
			if (_status == OrderStatus.Incomplete && newStatus != OrderStatus.Incomplete)
			{
				var discounts = Discounts.ToList();
				OrderDiscountsFactory = () => discounts;
				ConfirmDate = DateTime.Now;
			}
			//Log.Instance.LogDebug("SetStatus After datetime :" + newStatus);

			_status = newStatus;

			//Log.Instance.LogDebug("SetStatus Start AfterStatusChanged :" + newStatus);
			if (EventsOn && AfterStatusChanged != null)
			{
				AfterStatusChanged(this, new AfterOrderStatusChangedEventArgs { OrderInfo = this, OrderStatus = _status, SendEmails = sendEmails });
			}
			//Log.Instance.LogDebug("SetStatus End  :" + newStatus);
		}

		#endregion

		internal OrderInfo()
		{
			OrderLines = new List<OrderLine>();
			CouponCodesData = new List<string>();

			CustomerInfo = new CustomerInfo();
			CustomerInfo.Order = this;
			ShippingInfo = new ShippingInfo();
			PaymentInfo = new PaymentInfo();
			StoreInfo = new StoreInfo();

			ResetDiscounts();

			if (OrderLoaded != null)
			{
				OrderLoaded(this);
			}
			OrderValidationErrors = new List<OrderValidationError>();
		}

		/// <summary>
		/// Returns the ID of the order document node when it is created
		/// if not this is 0
		/// </summary>
		[DataMember]
		public int OrderNodeId { get; set; }

		/// <summary>
		/// Gets the unique orderdate for the order
		/// </summary>
		[DataMember]
		public DateTime? ConfirmDate
		{
			get { return _confirmDate; }
			set
			{
				OrderDate = value.GetValueOrDefault().ToString("f");
				_confirmDate = value;
			}
		}

		/// <summary>
		/// Gets or sets whether this order is paid.
		/// </summary>
		[DataMember(IsRequired = false)]
		public bool? Paid
		{
			get { return PaidDate.HasValue; }
			set
			{
				if (Paid != value && OrderPaidChanged != null)
				{
					try
					{
						OrderPaidChanged(this, new OrderPaidChangedEventArgs { OrderInfo = this, Paid = value.GetValueOrDefault() });
					}
					catch
					{
						Log.Instance.LogError("OrderPaidChanged Event Failed for Order: " + UniqueOrderId);
					}
				}
				PaidDate = value.GetValueOrDefault() ? (DateTime?)DateTime.Now : null;
			}
		}

		[DataMember(IsRequired = false)]
		public bool? Fulfilled
		{
			get { return FulfillDate.HasValue; }
			set
			{
				if (Fulfilled != value && OrderFulfillChanged != null)
				{
					try
					{
						OrderFulfillChanged(this, new OrderFulfillChangedEventArgs { OrderInfo = this, Fulfilled = value.GetValueOrDefault() });
					}
					catch
					{
						Log.Instance.LogError("OrderFulfillChanged Event Failed for Order: " + UniqueOrderId);
					}
				}
				FulfillDate = value.GetValueOrDefault() ? (DateTime?)DateTime.Now : null;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether [terms accepted].
		/// </summary>
		/// <value>
		///   <c>true</c> if [terms accepted]; otherwise, <c>false</c>.
		/// </value>
		[DataMember(IsRequired = false)]
		public bool TermsAccepted { get; set; }

		/// <summary>
		/// Is the stock updated for this order?
		/// </summary>
		[DataMember(IsRequired = false)]
		public bool StockUpdated { get; set; }

		/// <summary>
		/// Gets or sets the paid date.
		/// </summary>
		/// <value>
		/// The paid date.
		/// </value>
		[DataMember(IsRequired = false)]
		public DateTime? PaidDate { get; set; }

		/// <summary>
		/// Gets or sets the paid date.
		/// </summary>
		/// <value>
		/// The paid date.
		/// </value>
		[DataMember(IsRequired = false)]
		public DateTime? FulfillDate { get; set; }

		/// <summary>
		///     Gets a list with the coupons of the order
		/// </summary>
		[XmlIgnore]
		public ReadOnlyCollection<string> CouponCodes
		{
			get { return CouponCodesData.AsReadOnly(); }
		}

		/// <summary>
		/// Gets or sets the applied discounts information.
		/// </summary>
		/// <value>
		/// The applied discounts information.
		/// </value>
		[DataMember]
		public List<OrderDiscount> AppliedDiscountsInformation
		{
			// for the email XML for example
			get { return Discounts.Select(discount => new OrderDiscount(discount, this)).ToList(); }
			set { }
		}

		/// <summary>
		/// Gets or sets the charged shipping costs.
		/// </summary>
		/// <value>
		/// The charged shipping costs.
		/// </value>
		[DataMember]
		public ChargedShippingCosts ChargedShippingCosts
		{
			get { return new ChargedShippingCosts(ChargedShippingCostsInCents, PricesAreIncludingVAT, AverageOrderVatPercentage); }
			set { }
		}

		internal int ChargedShippingCostsInCents
		{
			get { return FreeShipping ? 0 : ActiveShippingProviderAmountInCents; }
		}

		internal bool FreeShipping
		{
			get { return Discounts.Any(discount => discount.DiscountType == DiscountType.FreeShipping); }
		}

		/// <summary>
		/// Gets or sets the payment costs.
		/// </summary>
		/// <value>
		/// The payment costs.
		/// </value>
		[DataMember]
		public PaymentCosts PaymentCosts
		{
			get { return new PaymentCosts(PaymentProviderPriceInCents, PricesAreIncludingVAT, AverageOrderVatPercentage); }
			set { }
		}

		/// <summary>
		/// Gets or sets the order line total.
		/// </summary>
		/// <value>
		/// The order line total.
		/// </value>
		[DataMember]
		public OrderLineTotal OrderLineTotal
		{
			get { return new OrderLineTotal(OrderLineTotalInCents, PricesAreIncludingVAT, AverageOrderVatPercentage); }
			set { }
		}

		/// <summary>
		///     Are shippingcosts still up to date?
		///     TRUE = shipping costs needs to be updated
		///     FALSE = shipping costs are correct!
		/// </summary>
		[DataMember]
		public bool ShippingCostsMightBeOutdated { get; set; }

		internal IEnumerable<CustomOrderValidation> CustomOrderValiations
		{
			get { return _customValidations; }
		}

		/// <summary>
		/// Gets the order discounts.
		/// </summary>
		/// <value>
		/// The order discounts.
		/// </value>
		[XmlIgnore]
		public List<API.IOrderDiscount> OrderDiscounts
		{
			get { return Discounts.Select(d => new DiscountAdaptor(d)).Cast<API.IOrderDiscount>().ToList(); }
		}

		[XmlIgnore]
		internal List<Interfaces.IOrderDiscount> Discounts
		{
			get
			{
				if (OrderDiscountsFactory == null) return new List<Interfaces.IOrderDiscount>(); // is this correct? (needed for deserialization process of legacy orderinfo xml)
				return OrderDiscountsFactory();
			}
		}

		[XmlIgnore]
		internal IEnumerable<IOrderDiscount> OrderDiscountsWithoutFreeShipping
		{
			get { return Discounts.Where(d => d.DiscountType != DiscountType.FreeShipping); }
		}

		/// <summary>
		/// Is this order discounted?
		/// </summary>
		[DataMember]
		public string IsDiscounted
		{
			get { return (Discounts.Any()).ToString(); } // string?
			set { }
		}

		/// <summary>
		/// Gets or sets a value indicating whether [vat charged].
		/// </summary>
		/// <value>
		///   <c>true</c> if [vat charged]; otherwise, <c>false</c>.
		/// </value>
		[DataMember]
		public bool VATCharged
		{
			get
			{
				if (_vatCharged.HasValue)
				{
					return _vatCharged.GetValueOrDefault();
				}

				// if customer country == store country ==> pay VAT
				if (!string.IsNullOrEmpty(CustomerInfo.CountryCode) && CustomerInfo.CountryCode.ToLowerInvariant() == StoreInfo.CountryCode.ToLowerInvariant())
				{
					//Log.Instance.LogDebug("VATCharged TRUE: CustomerInfo.CountryCode != null && StoreInfo.CountryCode != null && CustomerInfo.CountryCode.ToLowerInvariant() == StoreInfo.CountryCode.ToLowerInvariant()");
					return (bool)(_vatCharged = true);
				}

				// if customer country != store country && customer country does not exsists in vatCountryCodes ==> No VAT Charged
				var vatCountryCodes = IO.Container.Resolve<IVATCountryRepository>().GetAllCountries(Localization).Select(c => c.Code);

				if (!vatCountryCodes.Contains(CustomerInfo.CountryCode))
				{
					//Log.Instance.LogDebug("VATCharged FALSE: !vatCountryCodes.Contains(CustomerInfo.CountryCode)");
					return (bool)(_vatCharged = false);
				}

				// If customer is from different country then the shop, and the country is in the vat country list

				// if there is no vat number on the order, there does not have to be a vat check done and VAT shoudl be paid
				if (string.IsNullOrEmpty(CustomerInfo.VATNumber))
				{
					return (bool)(_vatCharged = true);
				}

				// check if member has vat al valid set to profile
				var cmsApplication = IO.Container.Resolve<ICMSApplication>();
				if (cmsApplication.MemberLoggedIn())
				{
					if (cmsApplication.CurrentMemberInfo().VATNumberCheckedAsValid)
					{
						var value = StoreInfo.CountryCode == CustomerInfo.CountryCode;
						Log.Instance.LogDebug("VATCharged " + value + ": Member VATNumberCheckedAsValid");
						return (bool)(_vatCharged = value);
					}
				}

				// do a vat check to the EU vat service
				return (bool)(_vatCharged = !VATCheckService.VATNumberValid(CustomerInfo.VATNumber, this));
			}
			set { }
		}

		/// <summary>
		/// Gets or sets the charged amount in cents.
		/// </summary>
		/// <value>
		/// The charged amount in cents.
		/// </value>
		[DataMember]
		public int ChargedAmountInCents
		{
			get { return VATCharged ? GrandtotalInCents : SubtotalInCents; }
			set { }
		}

		/// <summary>
		/// Gets or sets the order validation errors.
		/// </summary>
		/// <value>
		/// The order validation errors.
		/// </value>
		[DataMember]
		public List<OrderValidationError> OrderValidationErrors { get; set; }

		/// <summary>
		/// Gets or sets the regional vat in cents.
		/// </summary>
		/// <value>
		/// The regional vat in cents.
		/// </value>
		[DataMember]
		public int RegionalVatInCents
		{
			get
			{
				if (_regionalVatInCents != null)
				{
					return _regionalVatInCents.GetValueOrDefault();
				}

				if (StoreInfo.Alias != null)
				{
					var region = StoreHelper.GetAllRegionsForStore(StoreInfo.Alias).FirstOrDefault(x => x.Code == CustomerInfo.RegionCode);
					if (region != null)
					{
						decimal tax;
						if (decimal.TryParse(region.Tax, out tax))
						{
							// todo: fout, voegt eerst vat toe, daarna terugberekenen voor vat
							var orderTotalWithTax = PricesAreIncludingVAT ? OrderTotalInCents : VatCalculator.WithVat(OrderTotalInCents, AverageOrderVatPercentage);
							_regionalVatInCents = VatCalculator.VatAmountFromWithVat(orderTotalWithTax, tax);
							//_regionalVatInCents = VatCalculator.VatAmountFromOriginal(PricesAreIncludingVAT, OrderTotalInCents, AverageOrderVatPercentage);

							return _regionalVatInCents.GetValueOrDefault();
						}
					}
				}
				return 0;
			}
			set { _regionalVatInCents = value; } // test?
		}

		/// <summary>
		/// Gets or sets the total vat in cents.
		/// </summary>
		/// <value>
		/// The total vat in cents.
		/// </value>
		[DataMember]
		public decimal TotalVatInCents
		{
			get { return GrandtotalInCents - SubtotalInCents; }
			set { }
		}

		/// <summary>
		/// Gets the order total in cents.
		/// </summary>
		/// <value>
		/// The order total in cents.
		/// </value>
		[DataMember]
		public int OrderTotalInCents
		{
			get { return GetAmount(true, true, true); }
			set { }
		}

		/// <summary>
		/// Gets the order total without payment in cents.
		/// </summary>
		/// <value>
		/// The order total without payment in cents.
		/// </value>
		[IgnoreDataMember]
		public int OrderTotalWithoutPaymentInCents
		{
            get { return GetAmount(true, true, true, false); }
		}

		/// <summary>
		/// Gets the grandtotal in cents.
		/// </summary>
		/// <value>
		/// The grandtotal in cents.
		/// </value>
		[DataMember]
		public int GrandtotalInCents
		{
			get { return GetAmount(true, true, true); }
			set { }
		}

		/// <summary>
		/// Gets or sets the vat total in cents.
		/// </summary>
		/// <value>
		/// The vat total in cents.
		/// </value>
		[Obsolete("Use VatTotalInCents")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public int VatTotal
		{
			get { return VatTotalInCents; }
			set { }
		}

		/// <summary>
		/// Gets or sets the vat total in cents.
		/// </summary>
		/// <value>
		/// The vat total in cents.
		/// </value>
		public int VatTotalInCents
		{
			get { return GrandtotalInCents - SubtotalInCents - RegionalVatInCents; }
			set { }
		}

		/// <summary>
		/// Gets or sets the order line total in cents.
		/// </summary>
		/// <value>
		/// The order line total in cents.
		/// </value>
		public int OrderLineTotalInCents
		{
			get { return GetOrderLineTotalAmount(true, true, true); }
			set { }
		}

		public string RedirectUrl { get; set; }

		/// <summary>
		/// Gets or sets the database unique identifier.
		/// </summary>
		/// <value>
		/// The database unique identifier.
		/// </value>
		public int DatabaseId { get; set; }

		/// <summary>
		///     Gets a list with the orderlines of the order
		/// </summary>
		[DataMember]
		public List<OrderLine> OrderLines { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the prices are including vat.
		/// </summary>
		/// <value>
		/// <c>true</c> if the prices are including vat; otherwise, <c>false</c>.
		/// </value>
		public bool PricesAreIncludingVAT
		{
			get { return _pricesAreIncludingVAT ?? IO.Container.Resolve<ISettingsService>().IncludingVat; }
			set { _pricesAreIncludingVAT = value; }
		}

		/// <summary>
		/// Registers the custom order validation.
		/// </summary>
		/// <param name="condition">The condition.</param>
		/// <param name="errorDictionaryItem">The error dictionary item.</param>
		public void RegisterCustomOrderValidation(Predicate<OrderInfo> condition, Func<OrderInfo, string> errorDictionaryItem)
		{
			_customValidations.Add(new CustomOrderValidation { Condition = condition, ErrorDictionaryItem = errorDictionaryItem });
		}

		internal static OrderInfo CreateOrderInfoFromLegacyXmlString(string orderInfoXml, int databaseId = 0, string orderReferenceNumber = "")
		{
			//try
			{
				var orderInfo = DomainHelper.DeserializeXmlStringToObject<OrderInfo>(orderInfoXml);
				if (orderInfo.CustomerInfo != null) orderInfo.CustomerInfo.Order = orderInfo;
				orderInfo.LegacyDataReadBackMode = true;
				orderInfo.SetOrderReferenceOnOrderLinesAndProductInfos();
				if (orderInfo.OrderLines.Any())
				{
					// determine whether PricesAreIncludingVAT by comparing ProductRangePriceInCents with ProductRangePriceWithVatInCents
					var productInfo = orderInfo.OrderLines.First().ProductInfo;
#pragma warning disable 612,618
					orderInfo.PricesAreIncludingVAT = productInfo.ProductRangePriceInCents == productInfo.ProductRangePriceWithVatInCents; // sic!! legacy readback
#pragma warning restore 612,618
				}

				// create custom discount for backwards compatibility
				var discounts = new List<IOrderDiscount>();
				if (orderInfo.DiscountAmountInCents != 0 || orderInfo._discountAmount != null)
				{
					var discount = orderInfo.DiscountAmountInCents > 0 ? orderInfo.DiscountAmountInCents : (int)(orderInfo._discountAmount.GetValueOrDefault() * 100);
					discounts.Add(new OrderDTO.OrderDiscount(orderInfo.Localization) { DiscountType = DiscountType.Amount, DiscountValue = discount, RequiredItemIds = new List<int>(), AffectedOrderlines = new List<int>(), MemberGroups = new List<string>() });
				}

				IO.Container.Resolve<IOrderService>().UseStoredDiscounts(orderInfo, discounts);

				orderInfo.VATCheckService = new FixedValueIvatChecker(false);
				orderInfo.VatCalculationStrategy = new OverTotalVatCalculationStrategy();

				// generate product discount information for backwards compatibility
				foreach (var orderLine in orderInfo.OrderLines.Where(line => line.ProductInfo.PriceInCents != line.ProductInfo.ProductRangePriceInCents))
				{
					orderLine.ProductInfo.DiscountAmountInCents = orderLine.ProductInfo.ProductRangePriceInCents - orderLine.ProductInfo.PriceInCents;
				}

				if (orderInfo.Status != OrderStatus.Incomplete)
				{
					DateTime val;
					if (!DateTime.TryParse(orderInfo.OrderDate, orderInfo.StoreInfo.CultureInfo.DateTimeFormat, DateTimeStyles.None, out val))
						if (!DateTime.TryParse(orderInfo.OrderDate, new CultureInfo("EN-us").DateTimeFormat, DateTimeStyles.None, out val))
							if (!DateTime.TryParse(orderInfo.OrderDate, new CultureInfo("NL-nl").DateTimeFormat, DateTimeStyles.None, out val))
								DateTime.TryParse(orderInfo.OrderDate, out val);
					orderInfo.ConfirmDate = val;
				}
				orderInfo.LegacyDataReadBackMode = false;

				return orderInfo;
			}
			//catch (Exception ex)
			//{
			//	// todo: for debugging (developing!) you want a greater visibility of exceptions
			//	throw new Exception("Failed to load order data, id: " + databaseId + ", ordernumber: " + orderReferenceNumber, ex);
			//}
		}

		private static OrderInfo CreateOrderInfoFromOrderDataObject<T>(string orderInfoXml) where T : IConvertibleToOrderInfo
		{
			var order = DomainHelper.DeserializeXmlStringToObject<T>(orderInfoXml);
			OrderInfo orderInfo = order.ToOrderInfo();
			return orderInfo;
		}

		internal void SetOrderReferenceOnOrderLinesAndProductInfos()
		{
			OrderLines.ForEach(line =>
				{
					line.Order = this;
					line.ProductInfo.Order = this;
					line.ProductInfo.ProductVariants.ForEach(variant => variant.Product = line.ProductInfo);
				});
		}

		// todo: move to orderservice
		internal static OrderInfo CreateOrderInfoFromOrderData(OrderData orderData)
		{
			var cmsApplication = IO.Container.Resolve<ICMSApplication>();

			if (orderData == null || string.IsNullOrEmpty(orderData.OrderXML))
			{
				throw new Exception("Trying to load order without data (xml), id: " + (orderData == null ? "no data!" : orderData.DatabaseId.ToString()) + ", ordernumber: " + (orderData == null ? "no data!" : orderData.OrderReferenceNumber));
			}

			OrderInfo orderInfo;
			try
			{
				orderInfo = orderData.OrderXML.Contains("<OrderInfo ")
					? CreateOrderInfoFromLegacyXmlString(orderData.OrderXML)
					: CreateOrderInfoFromOrderDataObject<OrderDTO.Order>(orderData.OrderXML);
			}
			catch (Exception ex)
			{
				var message = "Failed to load order data, id: " + orderData.DatabaseId + ", ordernumber: " + orderData.OrderReferenceNumber;
				Log.Instance.LogError(ex, message);
				throw new Exception(message);
			}

			if (orderInfo == null) throw new Exception("Problem with parsing order from database xml");
			if (orderInfo.CustomerInfo == null) throw new Exception("Problem with parsing order from database xml, no CustomerInfo");
			if (orderInfo.PaymentInfo == null) throw new Exception("Problem with parsing order from database xml, no PaymentInfo");
			if (orderInfo.StoreInfo == null) throw new Exception("Problem with parsing order from database xml, no StoreInfo");

			orderInfo.DatabaseId = orderData.DatabaseId;
			orderInfo.UniqueOrderId = orderData.UniqueId;

			orderInfo.StoreInfo.Alias = orderData.StoreAlias; // todo: create a fallback for when a store has been deleted or renamed
			orderInfo.StoreOrderReferenceId = orderData.StoreOrderReferenceId;
			orderInfo.OrderNumber = orderData.OrderReferenceNumber;

			if (HttpContext.Current != null && (!IO.Container.Resolve<ICMSApplication>().IsBackendUserAuthenticated || UwebshopRequest.Current.PaymentProvider != null))
			{
				var currentUserIp = HttpContext.Current.Request.UserHostAddress;
				if (orderInfo.CustomerInfo.CustomerIPAddress != currentUserIp)
				{
					Log.Instance.LogWarning("Order: " + orderInfo.UniqueOrderId + " User IP address changed from: " + orderInfo.CustomerInfo.CustomerIPAddress + " to: " + currentUserIp);
					orderInfo.CustomerInfo.CustomerIPAddress = currentUserIp;
				}
			}

			OrderStatus orderStatus;
			Enum.TryParse(orderData.OrderStatus, out orderStatus);

			// if Incomplete and in frontend, refer to current discounts instead of stored
			if (orderStatus == OrderStatus.Incomplete && !cmsApplication.RequestIsInCMSBackend(HttpContext.Current))
			{
				orderInfo.EventsOn = false; // somehow a loop with the events keeps popping up, therefore an extra safeguard
				orderInfo.Status = orderStatus;

				var currentStore = StoreHelper.StoreService.GetCurrentStoreNoFallback();

				var orderUpdatingService = IO.Container.Resolve<IOrderUpdatingService>();
				if (currentStore != null && UwebshopConfiguration.Current.ShareBasketBetweenStores && orderInfo.StoreInfo.Alias != currentStore.Alias)
				{
					orderUpdatingService.ChangeLocalization(orderInfo, StoreHelper.CurrentLocalization);
				}
				// todo: might not be the correct place to do this
				else if (orderInfo.Localization.CurrencyCode != StoreHelper.CurrentLocalization.CurrencyCode)
				{
					orderUpdatingService.ChangeLocalization(orderInfo, Model.Localization.CreateLocalization(orderInfo.Localization.Store, StoreHelper.CurrentLocalization.CurrencyCode));
				}

				orderUpdatingService.SetCurrentMember(orderInfo);

				//orderInfo._pricesAreIncludingVAT = null; todo: yes or no? (if yes, you could argue you need to look at current prices aswell..)

				var orderService = IO.Container.Resolve<IOrderService>();
				orderService.UseDatabaseDiscounts(orderInfo);
				orderInfo.ResetDiscounts();

				if (orderInfo.RevalidateOrderOnLoad.GetValueOrDefault())
				{
					if (orderInfo.ReValidateSaveAction == ValidateSaveAction.Order)
					{
						orderService.ValidateOrder(orderInfo); // tricky (recursiveness because of GetStore() -> GetOrderInfo())
					}
					if (orderInfo.ReValidateSaveAction == ValidateSaveAction.Customer)
					{
						OrderHelper.ValidateCustomer(orderInfo, true);
					}
					if (orderInfo.ReValidateSaveAction == ValidateSaveAction.Stock)
					{
						OrderHelper.ValidateStock(orderInfo, true);
					}
					if (orderInfo.ReValidateSaveAction == ValidateSaveAction.Orderlines)
					{
						OrderHelper.ValidateOrderLines(orderInfo, true);
					}
					if (orderInfo.ReValidateSaveAction == ValidateSaveAction.CustomValidation)
					{
						OrderHelper.ValidateCustomValidation(orderInfo, true);
					}

					// todo: prevent unusefull save in ValidateOrder
				}
			}
			else
			{
				orderInfo.EventsOn = false; // somehow a loop with the events keeps popping up, therefore an extra safeguard
				orderInfo.Status = orderStatus;
			}

			orderInfo.CustomerInfo.CustomerId = orderData.CustomerId.GetValueOrDefault();

			if (orderData.CustomerUsername != null) orderInfo.CustomerInfo.LoginName = orderData.CustomerUsername;
			if (orderData.CustomerEmail != null) orderInfo.CustomerEmail = orderData.CustomerEmail;
			if (orderData.CustomerFirstName != null) orderInfo.CustomerFirstName = orderData.CustomerFirstName;
			if (orderData.CustomerLastName != null) orderInfo.CustomerLastName = orderData.CustomerLastName;
			if (orderData.TransactionId != null) orderInfo.PaymentInfo.TransactionId = orderData.TransactionId;

			orderInfo.EventsOn = true;

			return orderInfo;
		}

		internal OrderData ToOrderData()
		{
			var orderData = new OrderData();
			orderData.Order = this;
			orderData.DatabaseId = DatabaseId;
			orderData.UniqueId = UniqueOrderId;
			orderData.StoreAlias = StoreInfo.Alias;
			orderData.StoreOrderReferenceId = StoreOrderReferenceId;
			orderData.OrderReferenceNumber = OrderNumber;
			orderData.OrderStatus = Status.ToString();
			orderData.CustomerId = CustomerInfo.CustomerId;
			orderData.CustomerUsername = CustomerInfo.LoginName;
			orderData.CustomerEmail = CustomerEmail;
			orderData.CustomerFirstName = CustomerFirstName;
			orderData.CustomerLastName = CustomerLastName;
			orderData.TransactionId = PaymentInfo.TransactionId;

			orderData.OrderXML = DomainHelper.SerializeObjectToXmlString(new OrderDTO.Order(this));

			return orderData;
		}

		internal void ClearCachedValues()
		{
			_regionalVatInCents = null;
		}

		/// <summary>
		/// Sets the vat number.
		/// </summary>
		/// <param name="vatNumber">The vat number.</param>
		public void SetVATNumber(string vatNumber)
		{
			if (CustomerInfo.VATNumber != vatNumber)
			{
				_vatCharged = null;
				VATCheckService = IO.Container.Resolve<IVATCheckService>();
			}
			CustomerInfo.VATNumber = vatNumber;
		}

		/// <summary>
		/// Add extra fields to any orderline
		/// </summary>
		/// <param name="fields">The fields.</param>
		/// <param name="orderLineId">The order line unique identifier.</param>
		/// <returns></returns>
		public bool AddOrderLineDetails(Dictionary<string, string> fields, int orderLineId)
		{
			var orderline = OrderLines.FirstOrDefault(x => x.OrderLineId == orderLineId);

			if (orderline == null)
			{
				return false;
			}

			if (IO.Container.Resolve<IOrderUpdatingService>().ChangeOrderToIncompleteAndReturnTrueIfNotAllowed(this))
			{
				return false;
			}

			var documentTypeAlias = orderline.ProductInfo.CatalogProduct.NodeTypeAlias().Replace(Product.NodeAlias, OrderedProduct.NodeAlias);

			var xDoc = new XDocument(new XElement("Fields"));
			OrderUpdatingService.AddFieldsToXDocumentBasedOnCMSDocumentType(xDoc, fields, documentTypeAlias);
			orderline._customData = xDoc;

			return true;
		}

		/// <summary>
		/// Adds the store.
		/// </summary>
		/// <param name="store">The store.</param>
		public void AddStore(IStore store)
		{
			StoreInfo.Culture = store.Culture;
			StoreInfo.Alias = store.Alias;
			StoreInfo.CountryCode = store.CountryCode;
			StoreInfo.CurrencyCulture = store.CurrencyCulture;
			StoreInfo.LanguageCode = store.Culture;
		}

		/// <summary>
		///     Remove a coupon from the order
		/// </summary>
		/// <param name="couponCode"></param>
		public CouponCodeResult RemoveCoupon(string couponCode)
		{
			if (!CouponCodesData.Contains(couponCode))
			{
				return CouponCodeResult.NotFound;
			}

			CouponCodesData.Remove(couponCode);
			// place to redo discount calculation

			return CouponCodeResult.Success;
		}

		internal void SetCouponCode(string couponCode)
		{
			CouponCodesData.Add(couponCode);
			ApplyDiscounts();
			// place to redo discount calculation
		}

		#region Façade to IOrderUpdatingService

		/// <summary>
		///     Change orderstatus
		/// </summary>
		public bool ConfirmOrder(bool termsAccepted, int confirmationNodeId)
		{
			return IO.Container.Resolve<IOrderUpdatingService>().ConfirmOrder(this, termsAccepted, confirmationNodeId);
		}

		/// <summary>
		///     Save the order
		/// </summary>
		/// <param name="revalidateOrderOnLoadHack">check again validation</param>
		/// <param name="validateSaveAction"></param>
		public void Save(bool revalidateOrderOnLoadHack = false, ValidateSaveAction validateSaveAction = ValidateSaveAction.Order)
		{
			IO.Container.Resolve<IOrderUpdatingService>().Save(this, revalidateOrderOnLoadHack, validateSaveAction);
		}

		/// <summary>
		///     Create, Add, Update the orderline
		/// </summary>
		/// <param name="orderLineId">The Id of the orderline</param>
		/// <param name="productId">The productId</param>
		/// <param name="action">The action (add, update, delete, deleteall)</param>
		/// <param name="itemCount">The amount of items to be added</param>
		/// <param name="variantsList">The variants ID's added to the pricing</param>
		/// <param name="fields">custom fields added to this orderline</param>
		public void AddOrUpdateOrderLine(int orderLineId, int productId, string action, int itemCount, IEnumerable<int> variantsList, Dictionary<string, string> fields = null)
		{
			IO.Container.Resolve<IOrderUpdatingService>().AddOrUpdateOrderLine(this, orderLineId, productId, action, itemCount, variantsList, fields);
		}

		/// <summary>
		///     Add a payment provider to the order
		/// </summary>
		/// <param name="paymentProviderId">The Id of the Payment Provider Node</param>
		/// <param name="paymentProviderMethodId">The Id of the Paymetn Method</param>
		public ProviderActionResult AddPaymentProvider(int paymentProviderId, string paymentProviderMethodId)
		{
			return IO.Container.Resolve<IOrderUpdatingService>().AddPaymentProvider(this, paymentProviderId, paymentProviderMethodId, StoreHelper.CurrentLocalization);
		}

		/// <summary>
		///     Add a shipping provider to the order
		/// </summary>
		/// <param name="shippingProviderId">The Id of the Shipping Provider Node</param>
		/// <param name="shippingProviderMethodId">The Id of the Shipping Provider Method</param>
		public ProviderActionResult AddShippingProvider(int shippingProviderId, string shippingProviderMethodId)
		{
			return IO.Container.Resolve<IOrderUpdatingService>().AddShippingProvider(this, shippingProviderId, shippingProviderMethodId, StoreHelper.CurrentLocalization);
		}

		/// <summary>
		///     Add customer/shipping/extra fields to the order
		/// </summary>
		/// <param name="fields"></param>
		/// <param name="customerDataType"></param>
		/// <param name="ingnoreNotAllowed">Ignore if order is not allowed to be written to</param>
		public bool AddCustomerFields(Dictionary<string, string> fields, CustomerDatatypes customerDataType, bool ingnoreNotAllowed = false)
		{
			return IO.Container.Resolve<IOrderUpdatingService>().AddCustomerFields(this, fields, customerDataType, ingnoreNotAllowed);
		}

		/// <summary>
		///     Add coupon to the order
		/// </summary>
		/// <param name="couponCode"></param>
		public CouponCodeResult AddCoupon(string couponCode)
		{
			return IO.Container.Resolve<IOrderUpdatingService>().AddCoupon(this, couponCode);
		}

		#endregion

		#region Façade to CustomerInfo XML

		/// <summary>
		/// Gets or sets the customer email.
		/// </summary>
		/// <value>
		/// The customer email.
		/// </value>
		public string CustomerEmail
		{
			get
			{
				if (CustomerInfo == null || CustomerInfo.CustomerInformation == null)
					return string.Empty;
				XElement element = CustomerInfo.CustomerInformation.Element("customerEmail");
				return element != null ? element.Value : string.Empty;
			}
			set
			{
				if (CustomerInfo == null || CustomerInfo.CustomerInformation == null) return;
				XElement element = CustomerInfo.CustomerInformation.Element("customerEmail");
				if (element != null) element.SetValue(value);
			}
		}

		/// <summary>
		/// Gets or sets the last name of the customer.
		/// </summary>
		/// <value>
		/// The last name of the customer.
		/// </value>
		public string CustomerLastName
		{
			get
			{
				if (CustomerInfo == null || CustomerInfo.CustomerInformation == null)
					return string.Empty;
				XElement element = CustomerInfo.CustomerInformation.Element("customerLastName");
				return element != null ? element.Value : string.Empty;
			}
			set
			{
				if (CustomerInfo == null || CustomerInfo.CustomerInformation == null) return;
				XElement element = CustomerInfo.CustomerInformation.Element("customerLastName");
				if (element != null) element.SetValue(value);
			}
		}

		/// <summary>
		/// Gets or sets the first name of the customer.
		/// </summary>
		/// <value>
		/// The first name of the customer.
		/// </value>
		public string CustomerFirstName
		{
			get
			{
				if (CustomerInfo == null || CustomerInfo.CustomerInformation == null)
					return string.Empty;
				XElement element = CustomerInfo.CustomerInformation.Element("customerFirstName");
				return element != null ? element.Value : string.Empty;
			}
			set
			{
				if (CustomerInfo == null || CustomerInfo.CustomerInformation == null) return;
				XElement element = CustomerInfo.CustomerInformation.Element("customerFirstName");
				if (element != null) element.SetValue(value);
			}
		}

		/// <summary>
		/// Gets or sets the customer country.
		/// </summary>
		/// <value>
		/// The customer country.
		/// </value>
		public string CustomerCountry
		{
			get
			{
				if (CustomerInfo == null || CustomerInfo.CustomerInformation == null)
					return string.Empty;
				XElement element = CustomerInfo.CustomerInformation.Element("customerCountry");
				return element != null ? element.Value : string.Empty;
			}
			set
			{
				if (CustomerInfo == null || CustomerInfo.CustomerInformation == null) return;

				XElement element = CustomerInfo.CustomerInformation.Element("customerCountry");
				if (element != null) element.SetValue(value);
			}
		}

		#endregion

		#region Conversions to NonCents and With/Without VAT

		private decimal? _discountAmount;

		/// <summary>
		///     Gets the total amount of the order, including shippingcosts, including paymentcosts: including Vat
		/// </summary>
		[DataMember]
		public decimal Grandtotal
		{
			get { return GrandtotalInCents / 100m; }
			set { }
		}

		/// <summary>
		///     Gets the total of all the orderlines
		/// </summary>
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[Obsolete("Use OrderLineTotalWithVat")]
		public decimal OrderLineWithVatTotal
		{
			get { return OrderLineTotalWithVat; }
			set { }
		}

		/// <summary>
		///     Gets the total of all the orderlines
		/// </summary>
		[DataMember]
		public decimal OrderLineTotalWithVat
		{
			get { return OrderLineTotalWithVatInCents / 100m; }
			set { }
		}

		/// <summary>
		///     Gets the total of all the orderlines without VAT
		/// </summary>
		[DataMember]
		public decimal OrderLineTotalWithoutVat
		{
			get { return OrderLineTotalWithoutVatInCents / 100m; }
			set { }
		}

		/// <summary>
		///     Gets the total tax of the order, excluding shippingcosts
		/// </summary>
		[DataMember]
		public decimal TotalVat
		{
			get { return TotalVatInCents / 100m; }
			set { }
		}

		/// <summary>
		///     The shipping vat amount of this order
		/// </summary>
		[DataMember]
		public decimal ShippingProviderVatAmount
		{
			get { return ShippingProviderVatAmountInCents / 100m; }
			set { }
		}

		/// <summary>
		///     The shipping provider costs of this order EXCLUDING Vat
		/// </summary>
		[DataMember]
		public decimal ShippingProviderCostsWithoutVat
		{
			get { return ShippingProviderCostsWithoutVatInCents / 100m; }
			set { }
		}

		/// <summary>
		///     The shipping provider costs of this order INCLUDING Vat
		/// </summary>
		[DataMember]
		public decimal ShippingProviderCostsWithVat
		{
			get { return ShippingProviderCostsWithVatInCents / 100m; }
			set { }
		}

		/// <summary>
		///     the payment provider vat amount
		/// </summary>
		[DataMember]
		public decimal PaymentProviderVatAmount
		{
			get { return PaymentProviderVatAmountInCents / 100m; }
			set { }
		}

		/// <summary>
		///     the payment provider costs EXCLUDING vat
		/// </summary>
		[DataMember]
		public decimal PaymentProviderCostsWithoutVat
		{
			get { return PaymentProviderCostsWithoutVatInCents / 100m; }
			set { }
		}

		/// <summary>
		///     the payment provider costs INCLUDING vat
		/// </summary>
		[DataMember]
		public decimal PaymentProviderCostsWithVat
		{
			get { return PaymentProviderCostsWithVatInCents / 100m; }
			set { }
		}

		/// <summary>
		/// Gets or sets the discount amount.
		/// </summary>
		/// <value>
		/// The discount amount.
		/// </value>
		[DataMember]
		public decimal DiscountAmount
		{
			get { return _discountAmount ?? DiscountAmountInCents / 100m; }
			set { _discountAmount = value; }
		}

		/// <summary>
		/// Gets or sets the discount amount with vat.
		/// </summary>
		/// <value>
		/// The discount amount with vat.
		/// </value>
		[DataMember]
		public decimal DiscountAmountWithVat
		{
			get { return DiscountAmountWithVatInCents / 100m; }
			set { }
		}

		/// <summary>
		/// Gets or sets the discount amount without vat.
		/// </summary>
		/// <value>
		/// The discount amount without vat.
		/// </value>
		[DataMember]
		public decimal DiscountAmountWithoutVat
		{
			get { return DiscountAmountWithoutVatInCents / 100m; }
			set { }
		}

		/// <summary>
		///     Gets the regional vat for this order
		/// </summary>
		[DataMember]
		public decimal RegionalVat
		{
			get { return RegionalVatInCents / 100m; }
			set { }
		}

		/// <summary>
		///     Gets the subtotal of the order
		/// </summary>
		[DataMember]
		public decimal Subtotal
		{
			get { return SubtotalInCents / 100m; }
			set { }
		}

		/// <summary>
		/// Gets or sets the charged amount.
		/// </summary>
		/// <value>
		/// The charged amount.
		/// </value>
		[DataMember]
		public decimal ChargedAmount
		{
			get { return ChargedAmountInCents / 100m; }
			set { }
		}

		/// <summary>
		///     Gets the total of all the orderlines in cents with VAT
		/// </summary>
		[DataMember]
		public int OrderLineTotalWithVatInCents
		{
			get { return OrderLines.Sum(line => line.DiscountedAmountInCents); }
			set { }
		}

		/// <summary>
		///     Gets the subtotal (orderlines + shipping + payment including discounts
		/// </summary>
		[DataMember]
		public int SubtotalInCents
		{
			get { return GetAmount(false, true, true); }
			set { }
		}

		/// <summary>
		///     Gets the total of all the orderlines in cents without VAT
		/// </summary>
		[DataMember]
		public int OrderLineTotalWithoutVatInCents
		{
			get { return OrderLines.Sum(line => line.DiscountedAmountWithoutVatInCents); }
			set { }
		}

		/// <summary>
		/// Gets or sets the payment provider costs without vat in cents.
		/// </summary>
		/// <value>
		/// The payment provider costs without vat in cents.
		/// </value>
		[DataMember]
		public int PaymentProviderCostsWithoutVatInCents
		{
			get { return PricesAreIncludingVAT ? VatCalculator.WithoutVat(PaymentProviderPriceInCents, AverageOrderVatPercentage) : PaymentProviderPriceInCents; }
			set { }
		}

		/// <summary>
		/// Gets or sets the payment provider costs with vat in cents.
		/// </summary>
		/// <value>
		/// The payment provider costs with vat in cents.
		/// </value>
		[DataMember]
		public int PaymentProviderCostsWithVatInCents
		{
			get { return PricesAreIncludingVAT ? PaymentProviderPriceInCents : VatCalculator.WithVat(PaymentProviderPriceInCents, AverageOrderVatPercentage); }
			set { }
		}

		/// <summary>
		/// Gets or sets the shipping provider costs without vat in cents.
		/// </summary>
		/// <value>
		/// The shipping provider costs without vat in cents.
		/// </value>
		[DataMember]
		public int ShippingProviderCostsWithoutVatInCents
		{
			get { return PricesAreIncludingVAT ? VatCalculator.WithoutVat(ActiveShippingProviderAmountInCents, AverageOrderVatPercentage) : ActiveShippingProviderAmountInCents; }
			set { }
		}

		/// <summary>
		/// Gets or sets the shipping provider costs with vat in cents.
		/// </summary>
		/// <value>
		/// The shipping provider costs with vat in cents.
		/// </value>
		[DataMember]
		public int ShippingProviderCostsWithVatInCents
		{
			get { return PricesAreIncludingVAT ? ActiveShippingProviderAmountInCents : VatCalculator.WithVat(ActiveShippingProviderAmountInCents, AverageOrderVatPercentage); }
			set { }
		}

		#endregion

		#region Event firing

		internal BeforeOrderLineUpdatedEventArgs FireBeforeOrderLineUpdatedEvent(OrderLine orderLine)
		{
			BeforeOrderLineUpdatedEventArgs beforeUpdatedEventArgs = null;
			if (BeforeOrderLineUpdated != null)
			{
				beforeUpdatedEventArgs = new BeforeOrderLineUpdatedEventArgs { OrderLine = orderLine };
				BeforeOrderLineUpdated(this, beforeUpdatedEventArgs);
			}
			return beforeUpdatedEventArgs;
		}

		internal void FireBeforeOrderLineCreatedEvent()
		{
			if (BeforeOrderLineCreated != null)
			{
				var afterCreatedEventArgs = new BeforeOrderLineCreatedEventArgs { OrderLine = null };

				BeforeOrderLineCreated(this, afterCreatedEventArgs);
			}
		}

		internal void FireAfterOrderLineCreatedEvent(OrderLine orderLine)
		{
			if (AfterOrderLineCreated != null)
			{
				var afterCreatedEventArgs = new AfterOrderLineCreatedEventArgs { OrderLine = orderLine };

				AfterOrderLineCreated(this, afterCreatedEventArgs);
			}
		}

		internal void FireAfterOrderLineUpdatedEvent(OrderLine orderLine)
		{
			if (AfterOrderLineUpdated != null)
			{
				AfterOrderLineUpdated(this, new AfterOrderLineUpdatedEventArgs { OrderLine = orderLine });
			}
		}

		internal BeforeOrderLineDeletedEventArgs FireBeforeOrderLineDeletedEvent(OrderLine orderLine)
		{
			BeforeOrderLineDeletedEventArgs beforeDeletedEventArgs = null;
			if (BeforeOrderLineDeleted != null)
			{
				beforeDeletedEventArgs = new BeforeOrderLineDeletedEventArgs { OrderLine = orderLine };
				BeforeOrderLineDeleted(this, beforeDeletedEventArgs);
			}
			return beforeDeletedEventArgs;
		}

		internal void FireAfterOrderLineDeletedEvent()
		{
			if (AfterOrderLineDeleted != null)
			{
				var afterDeletedEventArgs = new AfterOrderLineDeletedEventArgs();

				AfterOrderLineDeleted(this, afterDeletedEventArgs);
			}
		}

		internal void FireBeforeOrderUpdatedEvent()
		{
			if (BeforeOrderUpdated != null)
			{
				BeforeOrderUpdated(this, new BeforeOrderUpdatedEventArgs { OrderInfo = this });
			}
		}

		internal void FireAfterOrderUpdatedEvent()
		{
			if (AfterOrderUpdated != null)
			{
				AfterOrderUpdated(this, new AfterOrderUpdatedEventArgs { OrderInfo = this });
			}
		}

		#endregion

		#region Amount Properties

		internal int GetOrderLineTotalAmount(bool inclVat, bool discounted, bool ranged)
		{
			return OrderLines.Sum(su => su.GetAmount(inclVat, discounted, ranged));
		}

        public int GetAmount(bool inclVat, bool discounted, bool ranged)
        {
            return GetAmount(inclVat,discounted,ranged,true);
        }

		// Mark 2014/2: todo: test
        // this is Mark's method, Thom has added the includePaymentProvider attr to prevent a SO, the IAmountUnit interface requires the /\ above method.
		public int GetAmount(bool inclVat, bool discounted, bool ranged, bool includePaymentProvider = true)
		{
			if (discounted)
			{
				ApplyDiscounts(); // todo: inefficient at this location, but secure (until a better architecture is built for this)
			}
			var total = GetOrderLineTotalAmount(inclVat, discounted, ranged);
			var originalTotal = OrderLines.Sum(su => su.GetOriginalAmount(discounted, ranged));

			var discountedTotal = discounted ? OrderDiscountEffects.GetDiscountedPrice(total, originalTotal) : total;
			discountedTotal = Math.Max(0, discountedTotal);

			discountedTotal = inclVat ? VatCalculationStrategy.WithVat(PricesAreIncludingVAT, originalTotal, AverageOrderVatPercentage, discountedTotal) 
				: VatCalculationStrategy.WithoutVat(PricesAreIncludingVAT, originalTotal, AverageOrderVatPercentage, discountedTotal);

            if(includePaymentProvider)
			    discountedTotal += (inclVat ? PaymentProviderCostsWithVatInCents : PaymentProviderCostsWithoutVatInCents);
			
            if (!FreeShipping)
			{
				discountedTotal += (inclVat ? ShippingProviderCostsWithVatInCents : ShippingProviderCostsWithoutVatInCents);
			}
			return discountedTotal + (inclVat ? RegionalVatInCents : 0);
		}

		/// <summary>
		///     The Active shipping provider amount without Vat
		/// </summary>
		internal int ActiveShippingProviderAmountInCents
		{
			// todo: check in conjunction with Discounts!!
			get { return ShippingProviderAmountInCents; }
		}

		/// <summary>
		/// Gets or sets the shipping provider vat amount in cents.
		/// </summary>
		/// <value>
		/// The shipping provider vat amount in cents.
		/// </value>
		[DataMember]
		public int ShippingProviderVatAmountInCents
		{// todo: check (does Shipping need an GetAmount() ?)
			get { return VatCalculator.VatAmountFromOriginal(PricesAreIncludingVAT, ActiveShippingProviderAmountInCents, AverageOrderVatPercentage); }
			set { }
		}

		/// <summary>
		/// Gets the average order vat percentage.
		/// </summary>
		/// <value>
		/// The average order vat percentage.
		/// </value>
		public decimal AverageOrderVatPercentage
		{
			// weighted average
			get
			{
				if (OrderLineTotalInCents != 0)
				{
					var totalPrice = GetOrderLineTotalAmount(true, false, true);
					return OrderLines.Sum(line => line.Vat * line.SellableUnits.Sum(su => su.GetAmount(true, false, true)) / totalPrice);
				}

				return 0;
			}
		}

		/// <summary>
		/// the payment provider vat percentage
		/// </summary>
		/// <value>
		/// The payment provider vat amount in cents.
		/// </value>
		//[DataMember] public decimal PaymentProviderVat;
		[DataMember]
		public decimal PaymentProviderVatAmountInCents
		{
			get { return PaymentProviderCostsWithVatInCents - PaymentProviderCostsWithoutVatInCents; }
			set { }
		}

		/// <summary>
		/// The total discount amount over the order
		/// </summary>
		/// <value>
		/// The discount amount in cents.
		/// </value>
		[DataMember]
		public int DiscountAmountInCents
		{
			get { return DiscountAmountWithVatInCents; } //PricesAreIncludingVAT ? DiscountAmountWithVatInCents : DiscountAmountWithoutVatInCents; }
			set { }
		}

		/// <summary>
		/// Gets or sets the discount amount with vat in cents.
		/// </summary>
		/// <value>
		/// The discount amount with vat in cents.
		/// </value>
		[DataMember]
		public int DiscountAmountWithVatInCents
		{
			get
			{
				var total = GetAmount(true, false, true);
				return Math.Min(total, total - GetAmount(true, true, true));
			}
			set { }
		}

		/// <summary>
		/// Gets or sets the discount amount without vat in cents.
		/// </summary>
		/// <value>
		/// The discount amount without vat in cents.
		/// </value>
		[DataMember]
		public int DiscountAmountWithoutVatInCents
		{
			get
			{
				var total = GetAmount(false, false, true);
				return Math.Min(total, total - GetAmount(false, true, true));
			}
			set { }
		}
		
		private void ApplyDiscounts()
		{
			// todo: this function and the way it's used needs refactoring
			if (OrderLines.Count == 0) return;

			ResetDiscounts();

			foreach (var discount in Discounts)
			{
				// will side-effect on OrderDiscountEffects and OrderLines->SellableUnits->DiscountEffects, todo: refactor
				IO.Container.Resolve<IDiscountCalculationService>().DiscountAmountForOrder(discount, this, true);
			}
		}

		#endregion

		internal class CustomOrderValidation
		{
			/// <summary>
			/// The condition
			/// </summary>
			public Predicate<OrderInfo> Condition;

			/// <summary>
			/// The error dictionary item
			/// </summary>
			public Func<OrderInfo, string> ErrorDictionaryItem;
		}

		internal void ResetDiscounts()
		{
			if (OrderDiscountEffects == null)
			{
				OrderDiscountEffects = new DiscountEffects();
			}

			// clear all discount effects
			OrderDiscountEffects.Clear();
			foreach (var su in OrderLines.SelectMany(l => l.SellableUnits))
			{
				su.GetDiscountEffects().Clear();
			}


			//OrderDiscountEffects = DiscountEffects.CreateSellableUnitOrOrderDiscountEffects( // future build up using OrderlineDiscountEffects
			//	OrderLines.SelectMany(l => l.SellableUnits).Cast<IDiscountableUnit>().ToArray());
			//OrderDiscountEffects = new DiscountEffects();
			// place to redo discount calculation
		}

		/// <summary>
		/// Gets or sets a value indicating whether [created information test mode].
		/// </summary>
		/// <value>
		/// <c>true</c> if [created information test mode]; otherwise, <c>false</c>.
		/// </value>
		public bool CreatedInTestMode { get; set; }

		internal bool ConfirmValidationFailed
		{
			get
			{
				if (HttpContext.Current != null && HttpContext.Current.Session != null)
				{
					var o = HttpContext.Current.Session["ConfirmFailed"];
					if (o != null)
					{
						return (bool)o;
					}
				}

				return false;
			}
			set { if (HttpContext.Current != null && HttpContext.Current.Session != null) HttpContext.Current.Session["ConfirmFailed"] = value; }
		}

		internal bool CustomerValidationFailed
		{
			get
			{
				if (HttpContext.Current != null && HttpContext.Current.Session != null)
				{
					var o = HttpContext.Current.Session["CustomerFailed"];
					if (o != null)
					{
						return (bool)o;
					}
				}

				return false;
			}
			set { if (HttpContext.Current != null && HttpContext.Current.Session != null) HttpContext.Current.Session["CustomerFailed"] = value; }
		}

		[IgnoreDataMember]
		[XmlIgnore]
		internal DiscountEffects OrderDiscountEffects;

		internal void ResetCachedValues()
		{
			_confirmDate = null;
			_regionalVatInCents = null;
			_vatCharged = null;
		}

		public DiscountEffects GetDiscountEffects()
		{
			return OrderDiscountEffects;
		}
	}

	#region Event Argument Classes

	/// <summary>
	/// 
	/// </summary>
	public class OrderPaidChangedEventArgs : EventArgs
	{
		/// <summary>
		/// Gets or sets the order information.
		/// </summary>
		/// <value>
		/// The order information.
		/// </value>
		public OrderInfo OrderInfo { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether [paid].
		/// </summary>
		/// <value>
		///   <c>true</c> if [paid]; otherwise, <c>false</c>.
		/// </value>
		public bool Paid { get; set; }
	}

	public class OrderFulfillChangedEventArgs : EventArgs
	{
		/// <summary>
		/// Gets or sets the order information.
		/// </summary>
		/// <value>
		/// The order information.
		/// </value>
		public OrderInfo OrderInfo { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether [paid].
		/// </summary>
		/// <value>
		///   <c>true</c> if [paid]; otherwise, <c>false</c>.
		/// </value>
		public bool Fulfilled { get; set; }
	}


	/// <summary>
	/// 
	/// </summary>
	public class BeforeOrderStatusChangedEventArgs : EventArgs
	{
		/// <summary>
		/// Gets or sets the order information.
		/// </summary>
		/// <value>
		/// The order information.
		/// </value>
		public OrderInfo OrderInfo { get; set; }

		/// <summary>
		/// Gets or sets the order status.
		/// </summary>
		/// <value>
		/// The order status.
		/// </value>
		public OrderStatus OrderStatus { get; set; }
	}

	/// <summary>
	/// 
	/// </summary>
	public class AfterOrderStatusChangedEventArgs : EventArgs
	{
		/// <summary>
		/// Gets or sets the order information.
		/// </summary>
		/// <value>
		/// The order information.
		/// </value>
		public OrderInfo OrderInfo { get; set; }

		/// <summary>
		/// Gets or sets the order status.
		/// </summary>
		/// <value>
		/// The order status.
		/// </value>
		public OrderStatus OrderStatus { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether [send emails].
		/// </summary>
		/// <value>
		///   <c>true</c> if [send emails]; otherwise, <c>false</c>.
		/// </value>
		public bool SendEmails { get; set; }
	}

	/// <summary>
	/// 
	/// </summary>
	public class BeforeOrderLineCreatedEventArgs : EventArgs
	{
		/// <summary>
		/// Gets or sets the order line.
		/// </summary>
		/// <value>
		/// The order line.
		/// </value>
		public OrderLine OrderLine { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether [cancel].
		/// </summary>
		/// <value>
		///   <c>true</c> if [cancel]; otherwise, <c>false</c>.
		/// </value>
		public bool Cancel { get; set; }
	}

	/// <summary>
	/// 
	/// </summary>
	public class AfterOrderLineCreatedEventArgs : EventArgs
	{
		/// <summary>
		/// Gets or sets the order line.
		/// </summary>
		/// <value>
		/// The order line.
		/// </value>
		public OrderLine OrderLine { get; set; }
	}

	/// <summary>
	/// 
	/// </summary>
	public class BeforeOrderLineUpdatedEventArgs : EventArgs
	{
		/// <summary>
		/// Gets or sets the order line.
		/// </summary>
		/// <value>
		/// The order line.
		/// </value>
		public OrderLine OrderLine { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether [cancel].
		/// </summary>
		/// <value>
		///   <c>true</c> if [cancel]; otherwise, <c>false</c>.
		/// </value>
		public bool Cancel { get; set; }
	}

	/// <summary>
	/// 
	/// </summary>
	public class AfterOrderLineUpdatedEventArgs : EventArgs
	{
		/// <summary>
		/// Gets or sets the order line.
		/// </summary>
		/// <value>
		/// The order line.
		/// </value>
		public OrderLine OrderLine { get; set; }
	}

	/// <summary>
	/// 
	/// </summary>
	public class BeforeOrderLineDeletedEventArgs : EventArgs
	{
		/// <summary>
		/// Gets or sets the order line.
		/// </summary>
		/// <value>
		/// The order line.
		/// </value>
		public OrderLine OrderLine { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether [cancel].
		/// </summary>
		/// <value>
		///   <c>true</c> if [cancel]; otherwise, <c>false</c>.
		/// </value>
		public bool Cancel { get; set; }
	}

	/// <summary>
	/// 
	/// </summary>
	public class AfterOrderLineDeletedEventArgs : EventArgs
	{
		/// <summary>
		/// Gets or sets the order line.
		/// </summary>
		/// <value>
		/// The order line.
		/// </value>
		public OrderLine OrderLine { get; set; }
	}

	/// <summary>
	/// 
	/// </summary>
	public class BeforeOrderUpdatedEventArgs : EventArgs
	{
		/// <summary>
		/// Gets or sets the order information.
		/// </summary>
		/// <value>
		/// The order information.
		/// </value>
		public OrderInfo OrderInfo { get; set; }
	}

	/// <summary>
	/// 
	/// </summary>
	public class AfterOrderUpdatedEventArgs : EventArgs
	{
		/// <summary>
		/// Gets or sets the order information.
		/// </summary>
		/// <value>
		/// The order information.
		/// </value>
		public OrderInfo OrderInfo { get; set; }
	}

	#endregion
}