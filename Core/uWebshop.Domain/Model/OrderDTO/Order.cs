using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Linq;
using System.Xml.Serialization;
using uWebshop.Common;
using uWebshop.Domain.Businesslogic.VATChecking;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;
using uWebshop.Domain.Model;

namespace uWebshop.Domain.OrderDTO
{
#pragma warning disable 1591
	public class Order : IConvertibleToOrderInfo
	{
		// deze 5 zijn belangrijk, maarr, zitten al in de tabel als kolommen
		//public string OrderNumber;
		//public int? StoreOrderReferenceId;
		//public Guid UniqueOrderId;
		//public DateTime? ConfirmDate;
		//public OrderStatus HandlingStatus;

		public int XMLVersion = 111;

		public string CurrencyCode;

		public bool? CreatedInTestMode;

		public string Name;
		public int ChargedAmount;
		public DateTime? ConfirmDate = DateTime.Now;
		public int? CorrespondingOrderDocumentId;
		public List<string> CouponCodes = new List<string>();
		public CustomerInfo CustomerInfo;
		public List<OrderDiscount> Discounts;

		public bool IncludingVAT;
		public bool? VatCalculatedOverParts;

		public List<OrderLine> OrderLines;
		public DateTime? PaidDate;
	    public DateTime? FulfillDate;
		public PaymentInfo PaymentInfo;
		public int PaymentProviderPrice;
		public int? PaymentProviderOrderPercentage;
		public ValidateSaveAction ReValidateSaveAction;
		public int RegionalVatAmount;
		public bool? RevalidateOrderOnLoad;

		public ShippingInfo ShippingInfo;
		public int ShippingProviderPrice;
		public bool? StockUpdated;
		public StoreInfo StoreInfo;
		public bool TermsAccepted;
		public bool VATCharged;
		[XmlIgnore] private XDocument _customData;

		public Order()
		{
		}

		public Order(OrderInfo orderInfo)
		{
			Name = orderInfo.Name;
			TermsAccepted = orderInfo.TermsAccepted;
			PaidDate = orderInfo.PaidDate;
		    FulfillDate = orderInfo.FulfillDate;
			ConfirmDate = orderInfo.ConfirmDate;
			OrderLines = orderInfo.OrderLines.Select(line => new OrderLine(line)).ToList();
			CouponCodes = orderInfo.CouponCodesData;
			CustomerInfo = orderInfo.CustomerInfo;
			ShippingInfo = orderInfo.ShippingInfo;
			PaymentInfo = orderInfo.PaymentInfo;
			StoreInfo = orderInfo.StoreInfo;
			CurrencyCode = orderInfo.Localization.CurrencyCode;
			// todo: onderstaand kan waarschijnlijk anders (geen check = dubbel)
			Discounts = orderInfo.Discounts.Select(discount => new OrderDiscount(orderInfo.Localization, discount, orderInfo)).ToList(); // todo: add/save used coupon code
			IncludingVAT = orderInfo.PricesAreIncludingVAT;

			VatCalculatedOverParts = orderInfo.VatCalculationStrategy is OverSmallestPartsVatCalculationStrategy;

			PaymentProviderPrice = orderInfo.PaymentProviderAmount;
			PaymentProviderOrderPercentage = orderInfo.PaymentProviderOrderPercentage;

			ShippingProviderPrice = orderInfo.ShippingProviderAmountInCents;
			RegionalVatAmount = orderInfo.RegionalVatInCents;

			VATCharged = orderInfo.VATCharged;
			ChargedAmount = orderInfo.ChargedAmountInCents;

			CorrespondingOrderDocumentId = orderInfo.OrderNodeId;

			RevalidateOrderOnLoad = orderInfo.RevalidateOrderOnLoad; // version 2.1 hack
			ReValidateSaveAction = orderInfo.ReValidateSaveAction; // version 2.1 hack

			StockUpdated = orderInfo.StockUpdated;
			CreatedInTestMode = orderInfo.CreatedInTestMode;
		}

		[DataMember]
		public XElement CustomData
		{
			get { return _customData != null ? _customData.Root : null; }
			set
			{
				_customData = new XDocument();
				_customData.Add(value);
			}
		}

		public OrderInfo ToOrderInfo()
		{
			var orderInfo = new OrderInfo();
			orderInfo.CreatedInTestMode = CreatedInTestMode.GetValueOrDefault();
			orderInfo.PaidDate = PaidDate;
		    orderInfo.FulfillDate = FulfillDate;
			orderInfo.ConfirmDate = ConfirmDate;
			orderInfo.OrderLines = OrderLines.Select(line => line.ToOrderLine(orderInfo)).ToList();
			orderInfo.SetOrderReferenceOnOrderLinesAndProductInfos();
			orderInfo.CouponCodesData = CouponCodes;
			orderInfo.CustomerInfo = CustomerInfo;
			orderInfo.ShippingInfo = ShippingInfo;
			orderInfo.PaymentInfo = PaymentInfo;
			orderInfo.StoreInfo = StoreInfo;
			orderInfo.Name = Name;

			if (CurrencyCode == null)
			{
				orderInfo.Localization = StoreHelper.GetLocalization(StoreInfo.Alias, null);
			}
			if (orderInfo.Localization == null)
			{
				orderInfo.Localization = Localization.ForceCreateLocalization(StoreInfo.Store, CurrencyCode);
			}
			
			if (orderInfo.Localization == null)
			{
				throw new Exception();
			}
			Discounts.ForEach(d => d.Localization = orderInfo.Localization);

			orderInfo.TermsAccepted = TermsAccepted;

			IO.Container.Resolve<IOrderService>().UseStoredDiscounts(orderInfo, new List<IOrderDiscount>(Discounts));
			orderInfo.VatCalculationStrategy = VatCalculatedOverParts.GetValueOrDefault() ? 
				(IVatCalculationStrategy)new OverSmallestPartsVatCalculationStrategy() : new OverTotalVatCalculationStrategy();

			orderInfo.VATCheckService = new FixedValueIvatChecker(!VATCharged);

			orderInfo.PricesAreIncludingVAT = IncludingVAT;
			orderInfo.PaymentProviderAmount = PaymentProviderPrice;
			orderInfo.PaymentProviderOrderPercentage = PaymentProviderOrderPercentage.GetValueOrDefault();
			orderInfo.ShippingProviderAmountInCents = ShippingProviderPrice;
			orderInfo.RegionalVatInCents = RegionalVatAmount; // todo!!!
			orderInfo.EventsOn = true;

			orderInfo.OrderNodeId = CorrespondingOrderDocumentId.GetValueOrDefault(0);

			orderInfo.RevalidateOrderOnLoad = RevalidateOrderOnLoad; // version 2.1 hack
			orderInfo.ReValidateSaveAction = ReValidateSaveAction;

			orderInfo.StockUpdated = StockUpdated.GetValueOrDefault();

			return orderInfo;
		}
	}
}