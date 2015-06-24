using System.Linq;
using uWebshop.Domain.BaseClasses;
using uWebshop.Domain.ContentTypes;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Domain
{
	[ContentType(ParentContentType = typeof(EmailStoreSectionContentType), Name = "Store Email", Description = "#StoreEmailDescription", Alias = "uwbsEmailTemplateStore", IconClass = IconClass.envelope, Icon = ContentIcon.MailOpenDocumentText, Thumbnail = ContentThumbnail.Folder)]
	internal class EmailStore : Email
	{
		internal static string NodeAlias;
	}

	[ContentType(ParentContentType = typeof(EmailCustomerSectionContentType), Name = "Customer Email", Description = "#CustomerEmailDescription", Alias = "uwbsEmailTemplateCustomer", IconClass = IconClass.envelope, Icon = ContentIcon.MailOpenDocumentText, Thumbnail = ContentThumbnail.Folder)]
	internal class EmailCustomer : Email
	{
		internal static string NodeAlias;
	}


	/// <summary>
	/// 
	/// </summary>
	public class Email : uWebshopEntity, IEmail
	{
		/// <summary>
		/// The email repository node alias
		/// </summary>
		public static string EmailRepositoryNodeAlias { get { return EmailRepositoryContentType.NodeAlias; } }

		/// <summary>
		/// The email template store section node alias
		/// </summary>
		public static string EmailTemplateStoreSectionNodeAlias { get { return EmailStoreSectionContentType.NodeAlias; } }

		/// <summary>
		/// The email template customer section node alias
		/// </summary>
		public static string EmailTemplateCustomerSectionNodeAlias { get { return EmailCustomerSectionContentType.NodeAlias; } }

		/// <summary>
		/// The email template store node alias
		/// </summary>
		public static string EmailTemplateStoreNodeAlias { get { return EmailStore.NodeAlias; } }

		/// <summary>
		/// The email template customer node alias
		/// </summary>
		public static string EmailTemplateCustomerNodeAlias { get { return EmailCustomer.NodeAlias; } }

		/// <summary>
		/// Initializes a new instance of the uWebshop.Domain.Email class
		/// </summary>
		/// <param name="id">NodeId of the email</param>
		public Email(int id) : base(id)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Email"/> class.
		/// </summary>
		public Email()
		{
		}

		/// <summary>
		/// Gets or sets the title.
		/// </summary>
		/// <value>
		/// The title.
		/// </value>
		[ContentPropertyType(Alias = "title", DataType = DataType.String, Tab = ContentTypeTab.Global, Name = "#Title", Description = "#TitleDescription")]
		public string Title
		{
			get { return StoreHelper.GetMultiStoreItem(Id, "title"); }
			set { }
		}

		/// <summary>
		/// Gets or sets the description.
		/// </summary>
		/// <value>
		/// The description.
		/// </value>
		[ContentPropertyType(Alias = "description", DataType = DataType.RichText, Tab = ContentTypeTab.Global, Name = "#Description", Description = "#DescriptionDescription")]
		public string Description
		{
			get { return StoreHelper.GetMultiStoreItem(Id, "description"); }
			set { }
		}

		/// <summary>
		/// Gets or sets the template.
		/// </summary>
		/// <value>
		/// The template.
		/// </value>
		[ContentPropertyType(Alias = "emailtemplate", DataType = DataType.TemplatePicker, Tab = ContentTypeTab.Global, Name = "#Template", Description = "#TemplateDescription")]
		public string Template
		{
			get
			{
				var template = StoreHelper.GetMultiStoreItem(Id, "emailtemplate");

				if (string.IsNullOrEmpty(template))
				{
					template = StoreHelper.GetMultiStoreItem(Id, "template");
				}

				if (string.IsNullOrEmpty(template) || template.All(char.IsDigit) 
					|| (template[0] == '-' && template.Skip(1).All(char.IsDigit)) || template == "-1")
				{
					template = StoreHelper.GetMultiStoreItem(Id, "xslttemplate");
				}

				return template;
			}
			set { }
		}

		/// <summary>
		/// Unused
		/// </summary>
		/// <value>
		/// Unused
		/// </value>
		[ContentPropertyType(Alias = "templatePreview", DataType = DataType.EmailDetails, Tab = ContentTypeTab.Global, Name = "#TemplatePreview", Description = "#TemplatePreviewDescription", Umbraco6Only = true)]
		public string TemplatePreview
		{
			set { }
		}
	}
}