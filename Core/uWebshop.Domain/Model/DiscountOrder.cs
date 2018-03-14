using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using uWebshop.Common;
using uWebshop.Domain.BaseClasses;
using uWebshop.Domain.Businesslogic;
using uWebshop.Domain.ContentTypes;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Domain
{
	/// <summary>
	/// 
	/// </summary>
	[ContentType(ParentContentType = typeof(DiscountOrderSectionContentType), Name = "Discount Order", Description = "#DiscountOrderDescription", Alias = "uwbsDiscountOrder", IconClass = IconClass.cut, Icon = ContentIcon.Scissors, Thumbnail = ContentThumbnail.Folder)]
	public class DiscountOrder : DiscountBase, Interfaces.IOrderDiscount
	{
		internal ILocalization Localization;

		/// <summary>
		/// The node alias
		/// </summary>
		public static string NodeAlias;

		/// <summary>
		/// The section node alias
		/// </summary>
		public static string SectionNodeAlias { get { return DiscountOrderSectionContentType.NodeAlias; } }

		/// <summary>
		/// The repository node alias
		/// </summary>
		public static string RepositoryNodeAlias { get { return DiscountRepositoryContentType.NodeAlias; } }

		/// <summary>
		/// Gets or sets the items.
		/// </summary>
		/// <value>
		/// The items.
		/// </value>
		[Browsable(false)]
		[Obsolete("Use RequiredItemIds")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public List<int> Items
		{
			get { return RequiredItemIds; }
			set { RequiredItemIds = value; }
		}

		/// <summary>
		/// Gets or sets the discount order condition.
		/// </summary>
		/// <value>
		/// The discount order condition.
		/// </value>
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[Obsolete("Use Condition")]
		public DiscountOrderCondition DiscountOrderCondition
		{
			get { return Condition; }
			set { Condition = value; }
		}

		/// <summary>
		/// List of products this discount applies to
		/// </summary>
		/// <value>
		/// The required item ids.
		/// </value>
		[ContentPropertyType(Alias = "items", DataType = DataType.MultiNodePicker, Tab = ContentTypeTab.Filter, Name = "#Items", Description = "#ItemsDescription", SortOrder = 3)]
		public List<int> RequiredItemIds { get; set; }

		/// <summary>
		/// Gets the affected orderlines.
		/// </summary>
		/// <value>
		/// The affected orderlines.
		/// </value>
		[ContentPropertyType(Alias = "affectedOrderlines", DataType = DataType.MultiContentPickerCatalog, Tab = ContentTypeTab.Filter, Name = "#AffectedOrderlines", Description = "#AffectedOrderlinesDescription", SortOrder = 4)]
		public List<int> AffectedOrderlines { get; set; }

		[ContentPropertyType(Alias = "affectedTags", DataType = DataType.Tags, Tab = ContentTypeTab.Filter, Name = "#AffectedTags", Description = "#AffectedTagsDescription", Mandatory = false, SortOrder = 5)]
		public IEnumerable<string> AffectedProductTags { get; set; }

		/// <summary>
		/// Discount condition (None, OnTheXthItem, PerSetOfItems)
		/// </summary>
		/// <value>
		/// The condition.
		/// </value>
		[ContentPropertyType(Alias = "orderCondition", DataType = DataType.DiscountOrderCondition, Tab = ContentTypeTab.Conditions, Name = "#OrderCondition", Description = "#OrderConditionDescription")]
		public DiscountOrderCondition Condition { get; set; }

		/// <summary>
		/// Number of items required for OnTheXthItem and PerSetOfItems conditions
		/// </summary>
		/// <value>
		/// The number of items condition.
		/// </value>
		[ContentPropertyType(Alias = "numberOfItemsCondition", DataType = DataType.Numeric, Tab = ContentTypeTab.Conditions, Name = "#NumberOfItemsCondition", Description = "#NumberOfItemsConditionDescription")]
		public int NumberOfItemsCondition { get; set; }

		/// <summary>
		/// Minimum amount of the order before discount is valid
		/// </summary>
		/// <value>
		/// The minimal order amount.
		/// </value>
		public int MinimalOrderAmount
		{
			get { return MinimumOrderAmountInCents; }
			set { MinimumOrderAmountInCents = value; }
		}

		public IVatPrice MinimumOrderAmount
		{
			// todo: better way for resolve ISettingsService
			get { return Price.CreateSimplePrice(MinimumOrderAmountInCents, IO.Container.Resolve<ISettingsService>().IncludingVat, Localization.Store.GlobalVat, Localization); }
		}

		[ContentPropertyType(Alias = "shippingDiscountable", DataType = DataType.TrueFalse, Tab = ContentTypeTab.Details, Name = "#ShippingDiscountable", Description = "#ShippingDiscountableDescription")]
		public bool IncludeShippingInOrderDiscountableAmount { get; set; }

		#region condition tab

		/// <summary>
		/// Minimum amount of the order before discount is valid
		/// </summary>
		/// <value>
		/// The minimum order amount in cents.
		/// </value>
		[ContentPropertyType(Alias = "minimumAmount", DataType = DataType.Price, Tab = ContentTypeTab.Conditions, Name = "#MinimumAmount", Description = "#MinimumAmountDescription", SortOrder = 20)]
		public int MinimumOrderAmountInCents { get; set; }

		/// <summary>
		/// Code of the coupon
		/// </summary>
		/// <value>
		/// The coupon code.
		/// </value>
		//[ContentPropertyType(Alias = "couponCode", DataType = DataType.String, Tab = ContentTypeTab.Conditions, Name = "#CouponCode", Description = "#CouponCodeDescription", SortOrder = 21)]
		public string CouponCode { get; set; }

		/// <summary>
		/// Code of the coupon
		/// </summary>
		/// <value>
		/// The coupon codes.
		/// </value>
		[ContentPropertyType(Alias = "couponCodes", DataType = DataType.CouponCodes, Tab = ContentTypeTab.Conditions, Name = "#CouponCodes", Description = "#CouponCodesDescription", SortOrder = 22)]
		public IEnumerable<string> CouponCodes
		{
			get { return IO.Container.Resolve<ICouponCodeService>().GetAllForDiscount(Id).Select(c => c.CouponCode); }
			set { }
		}

		/// <summary>
		/// Can this discount be only used once per customer?
		/// </summary>
		/// <value>
		/// <c>true</c> if the discount is once per customer; otherwise, <c>false</c>.
		/// </value>
		[ContentPropertyType(Alias = "oncePerCustomer", DataType = DataType.TrueFalse, Tab = ContentTypeTab.Conditions, Name = "#OncePerCustomer", Description = "#OncePerCustomerDescription", SortOrder = 23)]
		public bool OncePerCustomer { get; set; }

		public DiscountType Type { get { return DiscountType; } }

		#endregion

		/// <summary>
		/// Gets the original unique identifier.
		/// </summary>
		/// <value>
		/// The original unique identifier.
		/// </value>
		public int OriginalId
		{
			get { return Id; }
			set { Id = value; }
		}

        public Guid Key
        {
            get { return Key; }
            set { Key = value; }
        }

        internal static bool IsAlias(string alias)
		{
			return alias.StartsWith(NodeAlias);
		}
	}
}