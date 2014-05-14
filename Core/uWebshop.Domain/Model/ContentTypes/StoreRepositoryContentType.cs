namespace uWebshop.Domain.ContentTypes
{
	/// <summary>
	/// 
	/// </summary>
	[ContentType(ParentContentType = typeof(UwebshopRootContentType), Name = "Store Repository", Description = "#StoreRepositoryDescription", Alias = "uwbsStoreRepository", IconClass = IconClass.building, Icon = ContentIcon.StoreNetwork, Thumbnail = ContentThumbnail.Folder, AllowedChildTypes = new[] { typeof(Store) })]
	public class StoreRepositoryContentType
	{
		internal static string NodeAlias;
	}
}