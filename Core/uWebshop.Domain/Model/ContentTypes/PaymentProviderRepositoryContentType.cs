namespace uWebshop.Domain.ContentTypes
{
	/// <summary>
	/// 
	/// </summary>
	[ContentType(ParentContentType = typeof(UwebshopRootContentType), Name = "Payment Provider Repository", Description = "#PaymentProviderRepositoryDescription", Alias = "uwbsPaymentProviderRepository", IconClass = IconClass.piggybank, Icon = ContentIcon.Wallet, Thumbnail = ContentThumbnail.Folder, AllowedChildTypes = new[] { typeof(PaymentProviderSectionContentType), typeof(PaymentProviderZoneSectionContentType) })]
	public class PaymentProviderRepositoryContentType
	{
		internal static string NodeAlias;
	}

	/// <summary>
	/// 
	/// </summary>
	[ContentType(ParentContentType = typeof(PaymentProviderRepositoryContentType), Name = "Payment Provider Section", Description = "#PaymentProviderSectionDescription", Alias = "uwbsPaymentProviderSection", IconClass= IconClass.folder, Icon = ContentIcon.Bank, Thumbnail = ContentThumbnail.Folder, AllowedChildTypes = new[] { typeof(PaymentProvider) })]
	public class PaymentProviderSectionContentType
	{
		/// <summary>
		/// The node alias
		/// </summary>
		public static string NodeAlias;
	}

	/// <summary>
	/// 
	/// </summary>
	[ContentType(ParentContentType = typeof(PaymentProviderRepositoryContentType), Name = "Payment Provider Zone Section", Description = "#PaymentProviderZoneSectionDescription", Alias = "uwbsPaymentProviderZoneSection", IconClass = IconClass.globe, Icon = ContentIcon.GlobeGreen, Thumbnail = ContentThumbnail.Folder, AllowedChildTypes = new[] { typeof(PaymentProviderZone) })]
	public class PaymentProviderZoneSectionContentType
	{
		internal static string NodeAlias;
	}
}