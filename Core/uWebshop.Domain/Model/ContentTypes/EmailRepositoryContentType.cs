namespace uWebshop.Domain.ContentTypes
{
	/// <summary>
	/// 
	/// </summary>
	[ContentType(ParentContentType = typeof(UwebshopRootContentType), Name = "Email Repository", Description = "#EmailRepositoryDescription", Alias = "uwbsEmailRepository", IconClass= IconClass.inbox, Icon = ContentIcon.MailsStack, Thumbnail = ContentThumbnail.Folder, AllowedChildTypes = new[] { typeof(EmailStoreSectionContentType), typeof(EmailCustomerSectionContentType) })]
	public class EmailRepositoryContentType
	{
		internal static string NodeAlias;
	}

	/// <summary>
	/// 
	/// </summary>
    [ContentType(ParentContentType = typeof(EmailRepositoryContentType), Name = "Email Template Store Section", Description = "#EmailTemplateStoreSectionDescription", Alias = "uwbsEmailTemplateStoreSection", IconClass = IconClass.folder, Icon = ContentIcon.MailAir, Thumbnail = ContentThumbnail.Folder, AllowedChildTypes = new[] { typeof(EmailStore) })]
	public class EmailStoreSectionContentType
	{
		internal static string NodeAlias;
	}

	/// <summary>
	/// 
	/// </summary>
    [ContentType(ParentContentType = typeof(EmailRepositoryContentType), Name = "Email Template Customer Section", Description = "#EmailTemplateCustomerSectionDescription", Alias = "uwbsEmailTemplateCustomerSection", IconClass = IconClass.folder, Icon = ContentIcon.MailAir, Thumbnail = ContentThumbnail.Folder, AllowedChildTypes = new[] { typeof(EmailCustomer) })]
	public class EmailCustomerSectionContentType
	{
		internal static string NodeAlias;
	}
}