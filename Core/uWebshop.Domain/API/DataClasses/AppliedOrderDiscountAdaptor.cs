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
	[DataContract(Name = "Discount", Namespace= "")]
	internal class AppliedOrderDiscountAdaptor : IAppliedOrderDiscount
	{
		private readonly Domain.Interfaces.IOrderDiscount _discount;
		private readonly OrderInfo _order;
		
		public AppliedOrderDiscountAdaptor(Domain.Interfaces.IOrderDiscount discount, OrderInfo order)
		{
			_discount = discount;
			_order = order;
		}

		[IgnoreDataMember]
		public IVatPrice AmountForOrder
		{
			get
			{
				// todo: test
				var applicableOrderLines = !_discount.AffectedOrderlines.Any() ? _order.OrderLines : _order.OrderLines.Where(line => _discount.AffectedOrderlines.Contains(line.ProductInfo.Id) || _discount.AffectedOrderlines.Contains(line.ProductInfo.ProductVariants.Select(v => v.Id).FirstOrDefault())).ToList();
				var weightedAverageVat = applicableOrderLines.Sum(line => line.AmountInCents * line.ProductInfo.Vat) / applicableOrderLines.Sum(line => line.AmountInCents);
				return Price.CreateSimplePrice(IO.Container.Resolve<IDiscountCalculationService>().DiscountAmountForOrder(_discount, _order), _order.PricesAreIncludingVAT, weightedAverageVat, _order.Localization);
			}
		}
		[DataMember(Name = "AmountForOrder")]
		[JsonProperty(PropertyName = "AmountForOrder")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public FlatVatPrice AmountForOrderFlat { get { return new FlatVatPrice(AmountForOrder); } set { } }

		#region Pure interface forward

		[DataMember]
		public string Description { get { return _discount.Description; } set { } }

		[DataMember]
		public bool OncePerCustomer { get { return _discount.OncePerCustomer; } set { } }
		[DataMember]
		public bool CounterEnabled { get { return _discount.CounterEnabled; } set { } }
		[IgnoreDataMember]
		public DiscountType Type { get { return _discount.Type; } set { } }
		[DataMember(Name = "Type")]
		public string TypeText { get { return Type.ToString(); } set { } }
		[DataMember]
		public List<int> RequiredItemIds { get { return _discount.RequiredItemIds; } set { } }
		[DataMember]
		public DiscountOrderCondition Condition { get { return _discount.Condition; } set { } }
		[DataMember]
		public int NumberOfItemsCondition { get { return _discount.NumberOfItemsCondition; } set { } }
		[DataMember]
		public int DiscountValue { get { return _discount.DiscountValue; } set { } }
		[DataMember]
		public List<Range> Ranges { get { return _discount.Ranges; } set { } }

		[DataMember]
		public string CouponCode
		{
			get
			{

				var coupons = IO.Container.Resolve<ICouponCodeService>().GetAllForDiscount(_discount.OriginalId);
				if (coupons.Any()) //
				{
					var availableCoupons = coupons.Where(c => c.NumberAvailable > 0).Select(c => c.CouponCode);
					return availableCoupons.Intersect(_order.CouponCodes).FirstOrDefault();
				}
				return null;
			}
			set { }
		}

		[DataMember]
		public int OriginalId { get { return _discount.OriginalId; } set { } }


		[IgnoreDataMember]
		public DiscountType DiscountType { get { return _discount.Type; } set { } }
		[DataMember(Name = "DiscountType")]
		public string DiscountTypeText { get { return Type.ToString(); } set { } }

		[DataMember]
		public List<string> MemberGroups { get { return _discount.MemberGroups; } set { } }
		[DataMember]
		public List<int> AffectedOrderlines { get { return _discount.AffectedOrderlines; } set { } }
		[DataMember]
		public IEnumerable<string> AffectedProductTags { get { return _discount.AffectedProductTags; } set{} }

		[DataMember]
		public string Title { get { return _discount.Title; } set { } }
		
		[DataMember]
		public bool IncludeShippingInOrderDiscountableAmount { get { return _discount.IncludeShippingInOrderDiscountableAmount; } set { } }

		[IgnoreDataMember]
		public IVatPrice MinimumOrderAmount { get { return _discount.MinimumOrderAmount; } }
		[DataMember(Name = "MinimumOrderAmount")]
		[JsonProperty(PropertyName = "MinimumOrderAmount")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public FlatVatPrice MinimumOrderAmountFlat { get { return new FlatVatPrice(MinimumOrderAmount); } set { } }

		#endregion

	}
}