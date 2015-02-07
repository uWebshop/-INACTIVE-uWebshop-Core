namespace uWebshop.Domain.ContentTypes
{
	/// <summary>
	/// 
	/// </summary>
	[ContentType(ParentContentType = typeof(Catalog), Name = "Product Repository", Description = "#ProductRepositoryDescription", Alias = "uwbsProductRepository", IconClass = IconClass.barcode, Icon = ContentIcon.BoxSearchResults, Thumbnail = ContentThumbnail.Folder, AllowedChildTypes = new[] { typeof(Product), typeof(ProductVariant) })]
	public class ProductRepositoryContentType
	{
		internal static string NodeAlias;

		/// <summary>
		/// Sets the product overview.
		/// </summary>
		/// <value>
		/// The product overview.
		/// </value>
		[ContentPropertyType(Alias = "productOverview", DataType = DataType.ProductOverview, Tab = ContentTypeTab.Global, Name = "#ProductOverview", Description = "#ProductOverviewDescription", Mandatory = false, SortOrder = 1)]
		public string ProductOverview
		{
			set { }
		}
	}
}