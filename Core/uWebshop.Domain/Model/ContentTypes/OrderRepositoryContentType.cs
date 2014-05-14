namespace uWebshop.Domain.ContentTypes
{
	/// <summary>
	/// 
	/// </summary>
	[ContentType(ParentContentType = typeof(UwebshopRootContentType), Name = "Order Repository", Description = "#OrderRepositoryDescription", Alias = "uwbsOrderRepository", IconClass = IconClass.archive, Icon = ContentIcon.Drawer, Thumbnail = ContentThumbnail.Folder, AllowedChildTypes = new[] { typeof(OrderSection) })]
	public class OrderRepositoryContentType
	{
		internal static string NodeAlias;
	}
}