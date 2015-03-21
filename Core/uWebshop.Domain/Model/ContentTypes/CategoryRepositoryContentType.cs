namespace uWebshop.Domain.ContentTypes
{
	/// <summary>
	/// 
	/// </summary>
	[ContentType(ParentContentType = typeof(Catalog), Name = "Category Repository", Description = "#CategoryRepositoryDescription", Alias = "uwbsCategoryRepository", IconClass = IconClass.clipboard, Icon = ContentIcon.FolderSearchResults, Thumbnail = ContentThumbnail.Folder, AllowedChildTypes = new[] { typeof(Category) })]
	public class CategoryRepositoryContentType
	{
		internal static string NodeAlias;
	}
}