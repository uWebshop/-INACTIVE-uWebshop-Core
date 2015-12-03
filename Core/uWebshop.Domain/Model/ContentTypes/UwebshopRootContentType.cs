namespace uWebshop.Domain.ContentTypes
{
	/// <summary>
	/// 
	/// </summary>
	[ContentType(Name = "uWebshop", Description = "#uWebshopSectionDescription", Alias = "uWebshop", IconClass = IconClass.uwebshoplogo, Icon = ContentIcon.Uwebshop, Thumbnail = ContentThumbnail.Folder, AllowedChildTypes = new[] {typeof (Catalog), typeof (Settings), typeof (PaymentProviderRepositoryContentType), typeof (ShippingProviderRepositoryContentType), typeof (StoreRepositoryContentType), typeof (OrderRepositoryContentType), typeof (DiscountRepositoryContentType), typeof (EmailRepositoryContentType)})]
	public class UwebshopRootContentType
	{
		internal static string NodeAlias;

        /// <summary>
        /// The shop dashboard on the uWebshop node
        /// </summary>
        [ContentPropertyType(Alias = "shopDashboard", DataType = DataType.ShopDashboard, Tab = ContentTypeTab.Global, Name = "#ShopDashboard", Description = "#ShopDashboardDescription")]
        public string ShopDashboard { get; set; }
    }
}