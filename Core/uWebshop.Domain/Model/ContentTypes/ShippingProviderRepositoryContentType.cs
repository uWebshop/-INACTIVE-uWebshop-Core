namespace uWebshop.Domain.ContentTypes
{
	/// <summary>
	/// 
	/// </summary>
	[ContentType(ParentContentType = typeof(UwebshopRootContentType), Name = "Shipping Provider Repository", Description = "#ShippingProviderRepositoryDescription", Alias = "uwbsShippingProviderRepository", IconClass = IconClass.plane, Icon = ContentIcon.TruckBoxLabel, Thumbnail = ContentThumbnail.Folder, AllowedChildTypes = new[] { typeof(ShippingProviderSectionContentType), typeof(ShippingProviderZoneSectionContentType) })]
	public class ShippingProviderRepositoryContentType
	{
		internal static string NodeAlias;
	}

	/// <summary>
	/// 
	/// </summary>
	[ContentType(ParentContentType = typeof(ShippingProviderRepositoryContentType), Name = "Shipping Provider Section", Description = "#ShippingProviderSectionDescription", Alias = "uwbsShippingProviderSection", IconClass = IconClass.folder, Icon = ContentIcon.BaggageCart, Thumbnail = ContentThumbnail.Folder, AllowedChildTypes = new[] { typeof(ShippingProvider) })]
	public class ShippingProviderSectionContentType
	{
		internal static string NodeAlias;
	}

	/// <summary>
	/// 
	/// </summary>
	[ContentType(ParentContentType = typeof(ShippingProviderRepositoryContentType), Name = "Shipping Provider Zone Section", Description = "#ShippingProviderZoneSectionDescription", Alias = "uwbsShippingProviderZoneSection", IconClass = IconClass.globe, Icon = ContentIcon.GlobeGreen, Thumbnail = ContentThumbnail.Folder, AllowedChildTypes = new[] { typeof(ShippingProviderZone) })]
	public class ShippingProviderZoneSectionContentType
	{
		internal static string NodeAlias;
	}
}