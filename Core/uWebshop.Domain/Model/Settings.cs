using uWebshop.Domain.BaseClasses;
using uWebshop.Domain.ContentTypes;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Domain
{
	/// <summary>
	/// the uWebshop settings
	/// </summary>
    [ContentType(ParentContentType = typeof(UwebshopRootContentType), Name = "Settings", Description = "#SettingsSectionDescription", Alias = "uwbsSettings", IconClass = IconClass.briefcase, Icon = ContentIcon.Toolbox, Thumbnail = ContentThumbnail.Folder, AllowedChildTypes = new[] { typeof(Settings) })]
	public class Settings : uWebshopEntity, ISettings
	{
		/// <summary>
		/// The node alias
		/// </summary>
		public static string NodeAlias;

		internal Settings()
		{
		}

		/// <summary>
		/// Are all prices in the store including VAT?
		/// </summary>
		/// <value>
		///   <c>true</c> if [including vat]; otherwise, <c>false</c>.
		/// </value>
		[ContentPropertyType(Alias = "includingVat", DataType = DataType.TrueFalse, Tab = ContentTypeTab.Global, Name = "#IncludingVat", Description = "#IncludingVatSettingDescription")]
		public bool IncludingVat { get; internal set; }

		/// <summary>
		/// Render all url's lowercase instead of Case Sensitive
		/// </summary>
		/// <value>
		///   <c>true</c> if [use lowercase urls]; otherwise, <c>false</c>.
		/// </value>
		[ContentPropertyType(Alias = "lowercaseUrls", DataType = DataType.TrueFalse, Tab = ContentTypeTab.Global, Name = "#LowercaseUrls", Description = "#LowercaseUrlsSettingDescription")]
		public bool UseLowercaseUrls { get; internal set; }

		/// <summary>
		/// Lifetime of an incomplete order in minutes
		/// </summary>
		/// <value>
		/// The incomple order lifetime.
		/// </value>
		[ContentPropertyType(Alias = "incompleteOrderLifetime", DataType = DataType.String, Tab = ContentTypeTab.Global, Name = "#IncompleteOrderLifetime", Description = "#OrderLifetimeSettingDescription", Mandatory = true)]
		public int IncompleteOrderLifetime { get; internal set; }
	}
}