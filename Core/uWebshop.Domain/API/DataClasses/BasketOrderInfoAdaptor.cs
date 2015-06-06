using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using uWebshop.Common;
using uWebshop.Domain;
using uWebshop.Domain.Businesslogic;
using uWebshop.Domain.Interfaces;
using uWebshop.Newtonsoft.Json;

namespace uWebshop.API
{
	[KnownType(typeof(OrderFields))]
	[KnownType(typeof(BasketCustomer))]
	[KnownType(typeof(BasketPayment))]
	[KnownType(typeof(Localization))]
	[KnownType(typeof(AppliedOrderDiscountAdaptor))]
	[KnownType(typeof(OrderLineBasketAdaptor))]
	[DataContract(Name = "Order",Namespace = "")]
	public class BasketOrderInfoAdaptor : IBasket, IOrder, IWishlist
	{
		private readonly OrderInfo _source;
		internal OrderInfo GetOrderInfo() { return _source; }

		public BasketOrderInfoAdaptor(OrderInfo source)
		{
			if (source == null) throw new ArgumentNullException("source");
			_source = source;
		}

		private int BasketTotalBeforeDiscount()
		{
			return _source.OrderLines.Sum(orderline => orderline.AmountInCents) + _source.ShippingProviderAmountInCents + _source.PaymentProviderPriceInCents;
		}

		[DataMember]
		public Guid UniqueId { get { return _source.UniqueOrderId; } set { } }

		[DataMember]
		public string Name { get { return _source.Name; } set { } }

		[DataMember]
		public int ContentId { get { return _source.OrderNodeId; } set { } }

		IVatPrice IOrderBasketWishlistShared.OrderAmount { get { return OrderAmount; } }

		[IgnoreDataMember]
		public IDiscountedPrice OrderAmount
		{
			get { return new SimplePrice(_source, _source.Localization); }
			//get
			//{
			//	var totalPrice = _source.OrderLineTotalInCents + _source.PaymentProviderPriceInCents + _source.ActiveShippingProviderAmountInCents;
			//	var discount = _source.DiscountAmountInCents;
			//	return Price.CreateDiscountedRanged(totalPrice, null, _source.PricesAreIncludingVAT, _source.AverageOrderVatPercentage, null
			//		, i => i - discount, _source.Localization);
			//}
		}

		[DataMember(Name = "OrderAmount")]
		[JsonProperty(PropertyName = "OrderAmount")]
		[Browsable(false)][EditorBrowsable(EditorBrowsableState.Never)][DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public FlatDiscountedPrice OrderAmountFlat { get { return new FlatDiscountedPrice(OrderAmount); } set { } }

		[DataMember]
		public bool IsVatCharged { get { return _source.VATCharged; } set { } }
		
		[IgnoreDataMember]
		public IPrice RegionalVat { get { return Price.CreateBasicPrice(_source.RegionalVatInCents, _source.Localization);  } }
		[DataMember(Name = "RegionalVat")] [JsonProperty(PropertyName = "RegionalVat")] [Browsable(false)] [EditorBrowsable(EditorBrowsableState.Never)] [DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public FlatPrice RegionalVatFlat { get { return new FlatPrice(RegionalVat); } set { } }

		[DataMember]
		public IEnumerable<IOrderLine> OrderLines { get { return _source.OrderLines.Select(i => new OrderLineBasketAdaptor(i, _source)); } set { } }

		[DataMember]
		public IEnumerable<IAppliedOrderDiscount> Discounts
		{
			get { return _source.Discounts.Select(i => new AppliedOrderDiscountAdaptor(i, _source)); }
			set { }
		}

		[DataMember]
		public IEnumerable<string> UsedCouponCodes { get { return _source.CouponCodes; } set { } }
		[IgnoreDataMember]
		public IFulfillment Fulfillment { get { return new BasketFulfillment { Providers = new List<IChosenFulfillmentProvider> { new ShippingChosenFulfillmentAdaptor(IO.Container.Resolve<IShippingProviderService>().GetById(_source.ShippingInfo.Id, _source.Localization), _source.ShippingInfo.MethodId, _source.ShippingInfo.MethodTitle) } }; } set { } }

		[DataMember]
		public IPayment Payment
		{
			get
			{
				var iChosenPaymentProviderList = new List<IChosenPaymentProvider>();
				if (_source.PaymentInfo.Id != 0)
				{
					iChosenPaymentProviderList = new List<IChosenPaymentProvider>
						{
							new BasketChosenPaymentProviderAdaptor
								{
									Title =
										IO.Container.Resolve<IPaymentProviderService>()
										  .GetById(_source.PaymentInfo.Id, _source.Localization)
										  .Title,
									Id = _source.PaymentInfo.Id,
									MethodId = _source.PaymentInfo.MethodId,
									MethodTitle = _source.PaymentInfo.MethodTitle,
									TransactionId = _source.PaymentInfo.TransactionId
								}
						};
				}
				
				return new BasketPayment
					{
						Providers = iChosenPaymentProviderList
					};
				
			}
			set { }
		}
		[DataMember]
		public IOrderFields OrderFields { get { return new OrderFields(_source.CustomerInfo.ExtraInformation); } set { } }
		[DataMember]
		public ICustomer Customer { get { return new BasketCustomer(_source.CustomerInfo); } set { } }
		[DataMember]
		public IOrderSeries OrderSeries { get { return new OrderSeries(_source.ToOrderData()); } set { } }

		[IgnoreDataMember]
		public IStore Store { get { return new BasketStore(_source.StoreInfo); } set { } }

		[IgnoreDataMember]
		public IPrice ChargedOrderAmount { get { return Price.CreateBasicPrice(_source.ChargedAmountInCents, _source.Localization); } }
		[DataMember(Name = "ChargedOrderAmount")]
		[JsonProperty(PropertyName = "ChargedOrderAmount")]
		[Browsable(false)][EditorBrowsable(EditorBrowsableState.Never)][DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public FlatPrice ChargedOrderAmountFlat { get { return new FlatPrice(ChargedOrderAmount); } set { } }

		[IgnoreDataMember]
		public IPrice ChargedShippingAmount { get { return _source.VATCharged ? ShippingProviderAmount.WithVat : ShippingProviderAmount.WithoutVat; } }
		[DataMember(Name = "ChargedShippingAmount")]
		[JsonProperty(PropertyName = "ChargedShippingAmount")]
		[Browsable(false)][EditorBrowsable(EditorBrowsableState.Never)][DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public FlatPrice ChargedShippingAmountFlat { get { return new FlatPrice(ChargedShippingAmount); } set { } }

		[IgnoreDataMember]
		public IPrice ChargedPaymentAmount { get { return _source.VATCharged ? PaymentProviderAmount.WithVat : PaymentProviderAmount.WithoutVat; } }
		[DataMember(Name = "ChargedPaymentAmount")]
		[JsonProperty(PropertyName = "ChargedPaymentAmount")]
		[Browsable(false)][EditorBrowsable(EditorBrowsableState.Never)][DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public FlatPrice ChargedPaymentAmountFlat { get { return new FlatPrice(ChargedPaymentAmount); } set { } }

		[IgnoreDataMember]
		public IPrice GrandTotal { get { return Price.CreateBasicPrice(_source.GrandtotalInCents, _source.Localization); } }
		[DataMember(Name = "GrandTotal")]
		[JsonProperty(PropertyName = "GrandTotal")]
		[Browsable(false)][EditorBrowsable(EditorBrowsableState.Never)][DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public FlatPrice GrandTotalFlat { get { return new FlatPrice(GrandTotal); } set { } }

		[IgnoreDataMember]
		public IPrice SubTotal { get { return Price.CreateBasicPrice(_source.SubtotalInCents, _source.Localization); } }
		[DataMember(Name = "SubTotal")]
		[JsonProperty(PropertyName = "SubTotal")]
		[Browsable(false)][EditorBrowsable(EditorBrowsableState.Never)][DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public FlatPrice SubTotalFlat { get { return new FlatPrice(SubTotal); } set { } }
		[DataMember]
		public decimal AverageOrderVatPercentage
		{
			get { return _source.AverageOrderVatPercentage; }
			set { }
		}

		[IgnoreDataMember]
		public IDiscountedPrice OrderLineTotal
		{
			get
			{
				return new SummedPrice(OrderLines.Select(l => l.Amount), _source.PricesAreIncludingVAT, AverageOrderVatPercentage, _source.Localization, _source.VatCalculationStrategy);
			}
		}

		[IgnoreDataMember]
		IVatPrice IOrderBasketWishlistShared.OrderLineTotal { get { return OrderLineTotal; } }

		[DataMember(Name = "OrderLineTotal")]
		[JsonProperty(PropertyName = "OrderLineTotal")]
		[Browsable(false)][EditorBrowsable(EditorBrowsableState.Never)][DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public FlatDiscountedPrice OrderLineTotalFlat { get { return new FlatDiscountedPrice(OrderLineTotal); } set { } }

		[DataMember]
		[JsonProperty]
		public int Quantity
		{
			get { return OrderLines.Sum(x => x.Quantity); }
			set { }
		}

		[IgnoreDataMember]
		public IVatPrice PaymentProviderAmount { get { return Price.CreateSimplePrice(_source.PaymentProviderPriceInCents, _source.PricesAreIncludingVAT, _source.AverageOrderVatPercentage, _source.Localization); } }
		[DataMember(Name = "PaymentProviderAmount")]
		[JsonProperty(PropertyName = "PaymentProviderAmount")]
		[Browsable(false)][EditorBrowsable(EditorBrowsableState.Never)][DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public FlatVatPrice PaymentProviderAmountFlat { get { return new FlatVatPrice(PaymentProviderAmount); } set { } }
		
		[IgnoreDataMember]
		public IDiscountedPrice ShippingProviderAmount
		{
			get
			{
				return Price.CreateDiscountedRanged(_source.ActiveShippingProviderAmountInCents, null, _source.PricesAreIncludingVAT, _source.AverageOrderVatPercentage, null, i => _source.ChargedShippingCostsInCents, _source.Localization);
				//return Price.CreateSimplePrice(_source.ShippingProviderAmountInCents, _source.PricesAreIncludingVAT, _source.AverageOrderVatPercentage, _source.Localization);
			}
		}

		[DataMember(Name = "ShippingProviderAmount")]
		[JsonProperty(PropertyName = "ShippingProviderAmount")]
		[Browsable(false)][EditorBrowsable(EditorBrowsableState.Never)][DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public FlatVatPrice ShippingProviderAmountFlat { get { return new FlatVatPrice(ShippingProviderAmount); } set { } }

		[DataMember]
		public bool TermsAccepted { get { return _source.TermsAccepted; } set { } }
		[IgnoreDataMember]
		public IValidationResults ValidationResults { get { return new BasketValidationResults(_source, IO.Container.Resolve<IOrderService>()); } }
		[IgnoreDataMember]
		public ILocalization Localization { get { return _source.Localization; } set { } }

		[DataMember(Name = "Localization")]
		[JsonProperty(PropertyName = "Localization")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public Localization LocalizationFlat { get { return new Localization { CurrencySymbol = _source.Localization.Currency.CurrencySymbol, CurrencyCode = _source.Localization.CurrencyCode, Ratio = _source.Localization.Currency.Ratio, StoreAlias = _source.Localization.StoreAlias }; } set { } }
		[DataMember]
		public string OrderReference { get { return _source.OrderNumber; } set { } }
		[DataMember]
		public int StoreOrderReferenceId { get { return _source.StoreOrderReferenceId.GetValueOrDefault(); } set { } }
		
		[IgnoreDataMember]
		public OrderStatus Status { get { return _source.Status; } set { } }
		[DataMember(Name = "Status")]
		[JsonProperty(PropertyName = "Status")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public string StatusText { get { return Status.ToString(); } set { } }
		
		[DataMember]
		public DateTime ConfirmDate { get { return _source.ConfirmDate.GetValueOrDefault(); } set { } }
		[DataMember]
		public DateTime PaidDate { get { return _source.PaidDate.GetValueOrDefault(); } set { } }
		[DataMember]
		public bool Paid { get { return _source.Paid.GetValueOrDefault(false); } set { } }
		[DataMember]
		public DateTime ShippedDate { get { return default(DateTime); } set { } }
		[DataMember]
		public bool IsPaid { get { return _source.Paid.GetValueOrDefault(false); } set { } }
		[DataMember]
		public bool IsFulfilled { get { return _source.Status == OrderStatus.Closed || _source.Status == OrderStatus.Dispatched; } set { } }
	}
}