using System;
using System.Linq;
using System.Collections.Generic;
using System.Xml.Serialization;
using uWebshop.Common;
using uWebshop.Domain.Businesslogic;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Domain.OrderDTO
{
#pragma warning disable 1591
	public class OrderDiscount : Interfaces.IOrderDiscount
	{
		internal ILocalization Localization;

		public OrderDiscount()
		{
			
		}
		public OrderDiscount(ILocalization localization)
		{
			Localization = localization;
			if (AffectedProductTags == null) AffectedProductTags = Enumerable.Empty<string>();
		}
		
		internal OrderDiscount(ILocalization localization, IOrderDiscount discountOrder, OrderInfo order) : this(localization)
		{
			Id = discountOrder.Id;
			OriginalId = discountOrder.OriginalId;
            // IIS Hangs, need fix
            //Key = discountOrder.Key;
			DiscountType = discountOrder.DiscountType;
			DiscountValue = discountOrder.DiscountValue;
			Condition = discountOrder.Condition;
			NumberOfItemsCondition = discountOrder.NumberOfItemsCondition;
			RequiredItemIds = discountOrder.RequiredItemIds ?? new List<int>();
			MinimalOrderAmount = discountOrder.MinimumOrderAmount.ValueInCents();
			// todo: efficiency
			CouponCode = !string.IsNullOrWhiteSpace(discountOrder.CouponCode) ? discountOrder.CouponCode : 
				IO.Container.Resolve<ICouponCodeService>().GetAllForDiscount(discountOrder.Id).Select(c => c.CouponCode).Intersect(order.CouponCodes).FirstOrDefault();
			MemberGroups = discountOrder.MemberGroups;
			AffectedOrderlines = discountOrder.AffectedOrderlines;
			AffectedProductTags = discountOrder.AffectedProductTags == null ? Enumerable.Empty<string>() : discountOrder.AffectedProductTags.ToArray();
			Ranges = discountOrder.Ranges;
			OncePerCustomer = discountOrder.OncePerCustomer;
			IncludeShippingInOrderDiscountableAmount = discountOrder.IncludeShippingInOrderDiscountableAmount;
			TypeAlias = discountOrder.TypeAlias;
		}

		public int OriginalId { get; set; }
        public Guid Key { get; set; }
        public DiscountType DiscountType { get; set; }
		public int DiscountValue { get; set; }
		public string RangesString { get; set; }

		// conditions
		public DiscountOrderCondition Condition { get; set; }
		public int NumberOfItemsCondition { get; set; }

		public bool Disabled
		{
			get { return false; }
		}

		[XmlIgnore]
		public DateTime CreateDate { get; set; }
		[XmlIgnore]
		public DateTime UpdateDate { get; set; }
		[XmlIgnore]
		public int SortOrder { get; set; }

		public List<int> RequiredItemIds { get; set; }
		public int MinimalOrderAmount { get; set; }

		[XmlIgnore]
		public IVatPrice MinimumOrderAmount
		{
			// todo: better way for resolve ISettingsService
			get { return Price.CreateSimplePrice(MinimumOrderAmountInCents, IO.Container.Resolve<ISettingsService>().IncludingVat, Localization.Store.GlobalVat, Localization); }
		}

		public bool IncludeShippingInOrderDiscountableAmount { get;  set; }

		public string CouponCode { get; set; }
		public List<string> MemberGroups { get; set; }

		// interface compliance
		public int MinimumOrderAmountInCents
		{
			get { return MinimalOrderAmount; }
		}

		[XmlIgnore]
		public bool CounterEnabled
		{
			get { return false; }
		}

		[XmlIgnore]
		public int Counter
		{
			get { return 1; }
		}

		[XmlIgnore]
		public DiscountType Type { get { return DiscountType; } }

		[XmlIgnore]
		public List<Range> Ranges
		{
			get { return Range.CreateFromString(RangesString); }
			set { RangesString = value.ToRangesString(); }
		}

		public string Title
		{
			get
			{
				var originalDiscount = IO.Container.Resolve<IOrderDiscountService>().GetById(OriginalId, Localization);
				if (originalDiscount != null)
				{
					return originalDiscount.Title;
				}
				return string.Empty;
			}
		}

		public string Description
		{
			get
			{
				var originalDiscount = IO.Container.Resolve<IOrderDiscountService>().GetById(OriginalId, Localization);
				if (originalDiscount != null)
				{
					return originalDiscount.Description;
				}
				return string.Empty;
			}
		}

		public List<int> AffectedOrderlines { get; set; }

		[XmlIgnore]
		public IEnumerable<string> AffectedProductTags { get { return Tags ?? Enumerable.Empty<string>(); } set { Tags = value.ToArray(); } }
		public string[] Tags { get; set; }

		public bool OncePerCustomer { get; set; }

		public int Id { get; set; }
		public string TypeAlias { get; set; }
	}
}