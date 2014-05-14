using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using uWebshop.Common;
using uWebshop.Domain.ContentTypes;

namespace uWebshop.Domain.Model.ContentTypes
{

    [ContentType(ParentContentType = typeof(DiscountOrderSectionContentType), Name = "Basic Order Discount", Description = "#DiscountOrderBasicDescription", Alias = "uwbsDiscountOrderBasic", IconClass = IconClass.cut, Icon = ContentIcon.Scissors, Thumbnail = ContentThumbnail.Folder, InstallerOnly = true)]
	public class DiscountBasicContentType
	{
		[ContentPropertyType(Alias = "title", DataType = DataType.String, Tab = ContentTypeTab.Global, Name = "#Title", Description = "#TitleDescription", Mandatory = true, SortOrder = 1)]
		public string Title { get; set; }

		[ContentPropertyType(Alias = "description", DataType = DataType.RichText, Tab = ContentTypeTab.Global, Name = "#Description", Description = "#DescriptionDescription", Mandatory = false, SortOrder = 2)]
		public string Description { get; set; }

		[ContentPropertyType(Alias = "disable", DataType = DataType.TrueFalse, Tab = ContentTypeTab.Global, Name = "#Disable", Description = "#DisableDescription", SortOrder = 3)]
		public bool Disabled { get; set; }

		[ContentPropertyType(Alias = "discountType", DataType = DataType.DiscountType, Tab = ContentTypeTab.Details, Name = "#DiscountType", Description = "#DiscountTypeDescription", SortOrder = 4)]
		public DiscountType DiscountType { get; set; }

		[ContentPropertyType(Alias = "discount", DataType = DataType.Price, Tab = ContentTypeTab.Details, Name = "#Discount", Description = "#DiscountDescription", SortOrder = 5)]
		public int DiscountValue { get; set; }
	}

    [ContentType(ParentContentType = typeof(DiscountOrderSectionContentType), Name = "Countdown Order Discount", Description = "#DiscountOrderCountdownDescription", Alias = "uwbsDiscountOrderCountdown", IconClass = IconClass.cut, Icon = ContentIcon.Scissors, Thumbnail = ContentThumbnail.Folder, InstallerOnly = true)]
	public class DiscountCounterContentType
	{
		[ContentPropertyType(Alias = "title", DataType = DataType.String, Tab = ContentTypeTab.Global, Name = "#Title", Description = "#TitleDescription", Mandatory = true, SortOrder = 1)]
		public string Title { get; set; }

		[ContentPropertyType(Alias = "description", DataType = DataType.RichText, Tab = ContentTypeTab.Global, Name = "#Description", Description = "#DescriptionDescription", Mandatory = false, SortOrder = 2)]
		public string Description { get; set; }

		[ContentPropertyType(Alias = "disable", DataType = DataType.TrueFalse, Tab = ContentTypeTab.Global, Name = "#Disable", Description = "#DisableDescription", SortOrder = 3)]
		public bool Disabled { get; set; }

		[ContentPropertyType(Alias = "discountType", DataType = DataType.DiscountType, Tab = ContentTypeTab.Details, Name = "#DiscountType", Description = "#DiscountTypeDescription", SortOrder = 4)]
		public DiscountType DiscountType { get; set; }

		[ContentPropertyType(Alias = "discount", DataType = DataType.Price, Tab = ContentTypeTab.Details, Name = "#Discount", Description = "#DiscountDescription", SortOrder = 5)]
		public int DiscountValue { get; set; }

		[ContentPropertyType(Alias = "countdownEnabled", DataType = DataType.TrueFalse, Tab = ContentTypeTab.Details, Name = "#CountdownEnabled", Description = "#CountdownEnabledDescription", SortOrder = 6)]
		public bool CounterEnabled { get; set; }

		[ContentPropertyType(Alias = "countdown", DataType = DataType.Stock, Tab = ContentTypeTab.Details, Name = "#Countdown", Description = "#CountdownDescription", SortOrder = 7)]
		public int Counter { get; set; }
	}

    [ContentType(ParentContentType = typeof(DiscountOrderSectionContentType), Name = "Coupon Order Discount", Description = "#DiscountOrderCouponDescription", Alias = "uwbsDiscountOrderCoupon", IconClass = IconClass.cut, Icon = ContentIcon.Scissors, Thumbnail = ContentThumbnail.Folder, InstallerOnly = true)]
	public class DiscountCouponContentType
	{
		[ContentPropertyType(Alias = "title", DataType = DataType.String, Tab = ContentTypeTab.Global, Name = "#Title", Description = "#TitleDescription", Mandatory = true, SortOrder = 1)]
		public string Title { get; set; }

		[ContentPropertyType(Alias = "description", DataType = DataType.RichText, Tab = ContentTypeTab.Global, Name = "#Description", Description = "#DescriptionDescription", Mandatory = false, SortOrder = 2)]
		public string Description { get; set; }

		[ContentPropertyType(Alias = "disable", DataType = DataType.TrueFalse, Tab = ContentTypeTab.Global, Name = "#Disable", Description = "#DisableDescription", SortOrder = 3)]
		public bool Disabled { get; set; }

		[ContentPropertyType(Alias = "discountType", DataType = DataType.DiscountType, Tab = ContentTypeTab.Details, Name = "#DiscountType", Description = "#DiscountTypeDescription", SortOrder = 4)]
		public DiscountType DiscountType { get; set; }

		[ContentPropertyType(Alias = "discount", DataType = DataType.Price, Tab = ContentTypeTab.Details, Name = "#Discount", Description = "#DiscountDescription", SortOrder = 5)]
		public int DiscountValue { get; set; }
		
		[ContentPropertyType(Alias = "couponCodes", DataType = DataType.CouponCodes, Tab = ContentTypeTab.Conditions, Name = "#CouponCodes", Description = "#CouponCodesDescription", SortOrder = 22)]
		public string CouponCodes { get; set; }
	}

    [ContentType(ParentContentType = typeof(DiscountOrderSectionContentType), Name = "Ranged Order Discount", Description = "#DiscountOrderRangedDescription", Alias = "uwbsDiscountOrderRanged", IconClass = IconClass.cut, Icon = ContentIcon.Scissors, Thumbnail = ContentThumbnail.Folder, InstallerOnly = true)]
	public class DiscountRangedContentType
	{
		[ContentPropertyType(Alias = "title", DataType = DataType.String, Tab = ContentTypeTab.Global, Name = "#Title", Description = "#TitleDescription", Mandatory = true, SortOrder = 1)]
		public string Title { get; set; }

		[ContentPropertyType(Alias = "description", DataType = DataType.RichText, Tab = ContentTypeTab.Global, Name = "#Description", Description = "#DescriptionDescription", Mandatory = false, SortOrder = 2)]
		public string Description { get; set; }

		[ContentPropertyType(Alias = "disable", DataType = DataType.TrueFalse, Tab = ContentTypeTab.Global, Name = "#Disable", Description = "#DisableDescription", SortOrder = 3)]
		public bool Disabled { get; set; }

		[ContentPropertyType(Alias = "discountType", DataType = DataType.DiscountType, Tab = ContentTypeTab.Details, Name = "#DiscountType", Description = "#DiscountTypeDescription", SortOrder = 4)]
		public DiscountType DiscountType { get; set; }

		[ContentPropertyType(Alias = "discount", DataType = DataType.Price, Tab = ContentTypeTab.Details, Name = "#Discount", Description = "#DiscountDescription", SortOrder = 5)]
		public int DiscountValue { get; set; }

		[ContentPropertyType(Alias = "ranges", DataType = DataType.Ranges, Tab = ContentTypeTab.Details, Name = "#Ranges", Description = "#RangesDescription", SortOrder = 5)]
		public string RangesString { get; set; }
	}

    [ContentType(ParentContentType = typeof(DiscountOrderSectionContentType), Name = "Membergroup Order Discount", Description = "#DiscountOrderMemberGroupDescription", Alias = "uwbsDiscountOrderMembergroup", IconClass = IconClass.cut, Icon = ContentIcon.Scissors, Thumbnail = ContentThumbnail.Folder, InstallerOnly = true)]
	public class DiscountMemberGroupContentType
	{
		[ContentPropertyType(Alias = "title", DataType = DataType.String, Tab = ContentTypeTab.Global, Name = "#Title", Description = "#TitleDescription", Mandatory = true, SortOrder = 1)]
		public string Title { get; set; }

		[ContentPropertyType(Alias = "description", DataType = DataType.RichText, Tab = ContentTypeTab.Global, Name = "#Description", Description = "#DescriptionDescription", Mandatory = false, SortOrder = 2)]
		public string Description { get; set; }

		[ContentPropertyType(Alias = "disable", DataType = DataType.TrueFalse, Tab = ContentTypeTab.Global, Name = "#Disable", Description = "#DisableDescription", SortOrder = 3)]
		public bool Disabled { get; set; }

		[ContentPropertyType(Alias = "discountType", DataType = DataType.DiscountType, Tab = ContentTypeTab.Details, Name = "#DiscountType", Description = "#DiscountTypeDescription", SortOrder = 4)]
		public DiscountType DiscountType { get; set; }

		[ContentPropertyType(Alias = "discount", DataType = DataType.Price, Tab = ContentTypeTab.Details, Name = "#Discount", Description = "#DiscountDescription", SortOrder = 5)]
		public int DiscountValue { get; set; }

		[ContentPropertyType(Alias = "memberGroups", DataType = DataType.MemberGroups, Tab = ContentTypeTab.Conditions, Name = "#MemberGroups", Description = "#MemberGroupsDescription", SortOrder = 99)]
		public List<string> MemberGroups { get; set; }
	}
}
