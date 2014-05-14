using uWebshop.Domain.Model.ContentTypes;

namespace uWebshop.Domain.ContentTypes
{
	/// <summary>
	/// 
	/// </summary>
	[ContentType(ParentContentType = typeof (UwebshopRootContentType), Name = "Discount Repository", Description = "#DiscountRepositoryDescription", Alias = "uwbsDiscountRepository", IconClass = IconClass.bullhorn, Icon = ContentIcon.MegaPhone, Thumbnail = ContentThumbnail.Folder, AllowedChildTypes = new[] {typeof (DiscountOrderSectionContentType), typeof (DiscountProductSectionContentType)})]
	public class DiscountRepositoryContentType
	{
		internal static string NodeAlias;
	}

	/// <summary>
	/// 
	/// </summary>
	[ContentType(ParentContentType = typeof(DiscountRepositoryContentType), Name = "Discount Order Section", Description = "#DiscountOrderSectionDescription", Alias = "uwbsDiscountOrderSection", IconClass = IconClass.folder, Icon = ContentIcon.InboxDocument, Thumbnail = ContentThumbnail.Folder, AllowedChildTypes = new[] { typeof(DiscountOrder), typeof(DiscountCouponContentType), typeof(DiscountBasicContentType), typeof(DiscountRangedContentType), typeof(DiscountMemberGroupContentType), typeof(DiscountCounterContentType) })]
	public class DiscountOrderSectionContentType
	{
		internal static string NodeAlias;
	}

	/// <summary>
	/// 
	/// </summary>
	[ContentType(ParentContentType = typeof(DiscountRepositoryContentType), Name = "Discount Product Section", Description = "#DiscountProductSectionDescription", Alias = "uwbsDiscountProductSection", IconClass = IconClass.folder, Icon = ContentIcon.InboxDocument, Thumbnail = ContentThumbnail.Folder, AllowedChildTypes = new[] { typeof(DiscountProduct) })]
	public class DiscountProductSectionContentType
	{
		internal static string NodeAlias;
	}
}