using System;
using System.Configuration;
using System.Linq;
using System.Web.Configuration;
using System.Web.UI;
using umbraco.BasePages;
using umbraco.BusinessLogic;
using umbraco.cms.businesslogic.web;
using uWebshop.Domain;
using uWebshop.Domain.Interfaces;
using uWebshop.Umbraco.Interfaces;
using Umbraco.Core;
using Umbraco.Core.Models;
using Log = uWebshop.Domain.Log;
using MemberGroup = umbraco.cms.businesslogic.member.MemberGroup;
using MemberType = umbraco.cms.businesslogic.member.MemberType;

namespace uWebshop.Starterkits.DemoStore
{
	public class uWebshopDemoStarterkitInstaller : UserControl
	{
		protected global::System.Web.UI.WebControls.TextBox txtStoreAlias;
		protected global::System.Web.UI.WebControls.Button btnInstallStore;
		protected global::umbraco.uicontrols.Pane panel5;
		protected global::umbraco.uicontrols.Pane panel2;

		protected void Page_Load(object sender, EventArgs e)
		{
			var contentTypeService = ApplicationContext.Current.Services.ContentTypeService;
			var contentService = ApplicationContext.Current.Services.ContentService;
			var dataTypeService = ApplicationContext.Current.Services.DataTypeService;

			var umbracoVersion = IO.Container.Resolve<IUmbracoVersion>();

			bool storePresent;
			IO.Container.Resolve<ICMSInstaller>().InstallStarterkit("demo", out storePresent);

			var admin = User.GetUser(0);
			var configuration = WebConfigurationManager.OpenWebConfiguration("~");

			//  change UmbracoMembershipProvider to this:
			// <add name="UmbracoMembershipProvider" minRequiredPasswordLength="4" minRequiredNonalphanumericCharacters="0" type="umbraco.providers.members.UmbracoMembershipProvider" enablePasswordRetrieval="false" enablePasswordReset="false" requiresQuestionAndAnswer="false" defaultMemberTypeAlias="Customers" passwordFormat="Hashed" />

			var systemWeb = configuration.SectionGroups["system.web"];

			var membershipSection = (MembershipSection)systemWeb.Sections["membership"];
			
			var umbracoMembershipProvider = membershipSection.Providers["UmbracoMembershipProvider"];

			if (umbracoMembershipProvider.Parameters["defaultMemberTypeAlias"] != "Customers")
			{
				if (umbracoMembershipProvider.Parameters.Get("minRequiredPasswordLength") == null)
				{
					umbracoMembershipProvider.Parameters.Add("minRequiredPasswordLength", "4");
				}

				if (umbracoMembershipProvider.Parameters.Get("minRequiredNonalphanumericCharacters") == null)
				{
					umbracoMembershipProvider.Parameters.Add("minRequiredNonalphanumericCharacters", "0");
				}

				umbracoMembershipProvider.Parameters.Set("defaultMemberTypeAlias", "Customers");
			}
			
			
			// add profile to system.web, or add new fields if they are not there yet
			var profileSection = (ProfileSection)systemWeb.Sections["profile"];

			if (profileSection.DefaultProvider != "UmbracoMemberProfileProvider")
			{

				profileSection.DefaultProvider = "UmbracoMemberProfileProvider";
				profileSection.Enabled = true;
				profileSection.AutomaticSaveEnabled = false;

				//	<profile defaultProvider="UmbracoMemberProfileProvider" enabled="true" automaticSaveEnabled="false">
				//	<providers>
				//		<clear />
				//		<add name="UmbracoMemberProfileProvider" type="umbraco.providers.members.UmbracoProfileProvider, umbraco.providers" />
				//	</providers>
				//	<properties>
				//		<clear />
				//		<add name="customerFirstName" allowAnonymous="false" provider="UmbracoMemberProfileProvider" type="System.String" />
				//		<add name="customerLastName" allowAnonymous="false" provider="UmbracoMemberProfileProvider" type="System.String" />
				//		<add name="customerStreet" allowAnonymous="false" provider="UmbracoMemberProfileProvider" type="System.String" />
				//		<add name="customerStreetNumber" allowAnonymous="false" provider="UmbracoMemberProfileProvider" type="System.String" />
				//		<add name="customerZipCode" allowAnonymous="false" provider="UmbracoMemberProfileProvider" type="System.String" />
				//		<add name="customerCity" allowAnonymous="false" provider="UmbracoMemberProfileProvider" type="System.String" />
				//		<add name="customerCountry" allowAnonymous="false" provider="UmbracoMemberProfileProvider" type="System.String" />
				//	</properties>
				//</profile>
				
				var providerSettings = new ProviderSettings
									   {
										   Name = "UmbracoMemberProfileProvider",
										   Type =
											   "umbraco.providers.members.UmbracoProfileProvider, umbraco.providers"
									   };
				
				profileSection.Providers.Add(providerSettings);
				profileSection.PropertySettings.Clear();
				var customerFirstName = new ProfilePropertySettings("customerFirstName", false, SerializationMode.String,
					"UmbracoMemberProfileProvider", string.Empty, "System.String", false, string.Empty);
				profileSection.PropertySettings.Add(customerFirstName);
				var customerLastName = new ProfilePropertySettings("customerLastName", false, SerializationMode.String,
					"UmbracoMemberProfileProvider", string.Empty, "System.String", false, string.Empty);
				profileSection.PropertySettings.Add(customerLastName);
				var customerStreet = new ProfilePropertySettings("customerStreet", false, SerializationMode.String,
					"UmbracoMemberProfileProvider", string.Empty, "System.String", false, string.Empty);
				profileSection.PropertySettings.Add(customerStreet);
				var customerStreetNumber = new ProfilePropertySettings("customerStreetNumber", false, SerializationMode.String,
					"UmbracoMemberProfileProvider", string.Empty, "System.String", false, string.Empty);
				profileSection.PropertySettings.Add(customerStreetNumber);
				var customerZipCode = new ProfilePropertySettings("customerZipCode", false, SerializationMode.String,
					"UmbracoMemberProfileProvider", string.Empty, "System.String", false, string.Empty);
				profileSection.PropertySettings.Add(customerZipCode);
				var customerCity = new ProfilePropertySettings("customerCity", false, SerializationMode.String,
					"UmbracoMemberProfileProvider", string.Empty, "System.String", false, string.Empty);
				profileSection.PropertySettings.Add(customerCity);
				var customerCountry = new ProfilePropertySettings("customerCountry", false, SerializationMode.String,
					"UmbracoMemberProfileProvider", string.Empty, "System.String", false, string.Empty);
				profileSection.PropertySettings.Add(customerCountry);
			}

			configuration.Save();

			// generate new membertype based on properties above
			// add them to both customer profile and order

			var customersType = MemberType.GetByAlias("Customers");

			if (customersType == null)
			{
				try
				{
					customersType = MemberType.MakeNew(admin, "Customers");
				}
				catch
				{
					Log.Instance.LogError("Umbraco Failed to create 'Customers' MemberType");
					// Umbraco bug with SQLCE + MemberType.MakeNew requires this catch, membertype will not be created...
				}
			}

			var uwbsOrdersType = contentTypeService.GetContentType(Order.NodeAlias);

			if (customersType != null && uwbsOrdersType != null)
			{
				var customerTab = uwbsOrdersType.PropertyGroups.FirstOrDefault(x => x.Name.ToLowerInvariant() == "customer");
				
				if (customerTab == null)
				{
					uwbsOrdersType.AddPropertyGroup("Customer");
				}
				
				var shippingTab = uwbsOrdersType.PropertyGroups.FirstOrDefault(x => x.Name.ToLowerInvariant() == "shipping");
				
				if (shippingTab == null)
				{
					uwbsOrdersType.AddPropertyGroup("Shipping");
				}

				contentTypeService.Save(uwbsOrdersType);
				

				// todo V7 version!
				var stringDataType = umbracoVersion.GetDataTypeDefinition("Umbraco.Textbox");
				var stringDataTypeDef = dataTypeService.GetDataTypeDefinitionById(stringDataType.Id);
				var textboxMultipleDataType = umbracoVersion.GetDataTypeDefinition("Umbraco.TextboxMultiple");
				var textboxMultipleDataTypeDef = dataTypeService.GetDataTypeDefinitionById(textboxMultipleDataType.Id);

				foreach (var propertyKey in profileSection.PropertySettings.AllKeys)
				{

					//todo: test if line should be commented or not
					//customersType.AddPropertyType(stringDataTypeDef, propertyKey, "#" + UppercaseFirstCharacter(propertyKey));

					if (uwbsOrdersType.PropertyTypes.All(x => x.Alias.ToLowerInvariant() != propertyKey.ToLowerInvariant()))
					{
						var propertyType = new PropertyType(stringDataTypeDef)
						{
							Alias = propertyKey,
							Name = "#" + UppercaseFirstCharacter(propertyKey)
						};

						uwbsOrdersType.AddPropertyType(propertyType);

						var propertyShippingKey = propertyKey.Replace("customer", "shipping");
						var shipppingPropertyType = new PropertyType(stringDataTypeDef)
						{
							Alias = propertyShippingKey,
							Name = "#" + UppercaseFirstCharacter(propertyShippingKey)
						};

						uwbsOrdersType.AddPropertyType(shipppingPropertyType);

						if (customerTab != null) customerTab.PropertyTypes.Add(propertyType);
						if (shippingTab != null) shippingTab.PropertyTypes.Add(shipppingPropertyType);
					}
				}

				customersType.Save();

				var extraPropertyType = new PropertyType(stringDataTypeDef)
				{
					Alias = "extraMessage",
					Name = "#ExtraMessage"
				};

				uwbsOrdersType.AddPropertyType(extraPropertyType);
				if (customerTab != null) customerTab.PropertyTypes.Add(extraPropertyType);

				contentTypeService.Save(uwbsOrdersType);
			}

			// create membergroup "customers", as set in the UmbracoMembershipProvider -> defaultMemberTypeAlias
			if (MemberGroup.GetByName("Customers") == null)
			{
				MemberGroup.MakeNew("Customers", admin);
			}

			contentService.RePublishAll();
		}

		private static string UppercaseFirstCharacter(string s)
		{
			// Check for empty string.
			if (string.IsNullOrEmpty(s))
			{
				return string.Empty;
			}
			// Return char and concat substring.
			return char.ToUpper(s[0]) + s.Substring(1);
		}
		
		protected void BtnInstallStoreClick(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(txtStoreAlias.Text))
			{
				BasePage.Current.ClientTools.ShowSpeechBubble(BasePage.speechBubbleIcon.error, "Error", "No Store name entered");
			}
			else
			{
				Umbraco.Helpers.InstallStore(txtStoreAlias.Text);
				BasePage.Current.ClientTools.ShowSpeechBubble(BasePage.speechBubbleIcon.success, "Store Installed!", "Your " + txtStoreAlias.Text + " Store is installed!");
			}
		}

		public static bool PublishWithChildrenWithResult(Document doc, User u)
		{
			//todo: find out usage in starterkit
			if (doc.PublishWithResult(u))
			{
				foreach (var dc in doc.Children.ToList())
				{
					dc.PublishWithChildrenWithResult(u);
				}
				return true;
			}
			return false;
		}

		[Obsolete("Umbraco now updates cache after publish")]
		public static void UpdateDocumentCache(Document doc)
		{
			//todo: remove?
		}
	}

	public class uWebshopStarterkitInstaller
	{
		/// <summary>
		/// panel2 control.
		/// </summary>
		/// <remarks>
		/// Auto-generated field.
		/// To modify move field declaration from designer file to code-behind file.
		/// </remarks>
		protected global::umbraco.uicontrols.Pane panel2;

		/// <summary>
		/// Label1 control.
		/// </summary>
		/// <remarks>
		/// Auto-generated field.
		/// To modify move field declaration from designer file to code-behind file.
		/// </remarks>
		protected global::System.Web.UI.WebControls.Label Label1;

		/// <summary>
		/// txtStoreAlias control.
		/// </summary>
		/// <remarks>
		/// Auto-generated field.
		/// To modify move field declaration from designer file to code-behind file.
		/// </remarks>
		protected global::System.Web.UI.WebControls.TextBox txtStoreAlias;

		/// <summary>
		/// btnInstallStore control.
		/// </summary>
		/// <remarks>
		/// Auto-generated field.
		/// To modify move field declaration from designer file to code-behind file.
		/// </remarks>
		protected global::System.Web.UI.WebControls.Button btnInstallStore;

		/// <summary>
		/// panel5 control.
		/// </summary>
		/// <remarks>
		/// Auto-generated field.
		/// To modify move field declaration from designer file to code-behind file.
		/// </remarks>
		protected global::umbraco.uicontrols.Pane panel5;
	}
}