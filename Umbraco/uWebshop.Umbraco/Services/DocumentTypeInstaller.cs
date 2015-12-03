using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Umbraco.Core;
using Umbraco.Core.Configuration;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using uWebshop.Domain;
using uWebshop.Domain.ContentTypes;
using uWebshop.Domain.Core;
using uWebshop.Domain.Interfaces;
using uWebshop.Umbraco.Businesslogic;
using uWebshop.Umbraco.Interfaces;
using Constants = uWebshop.Common.Constants;
using DataTypeDefinition = Umbraco.Core.Models.DataTypeDefinition;
using Language = umbraco.cms.businesslogic.language.Language;

namespace uWebshop.Umbraco.Services
{
	public class DocumentTypeInstaller
	{
		public void InstallDocumentTypes()
		{
			var contentTypeService = ApplicationContext.Current.Services.ContentTypeService;
			var dataTypeService = ApplicationContext.Current.Services.DataTypeService;

			//var DiscountTypeDataTypeDef = dataTypeService.GetDataTypeDefinitionById(new Guid("2d89188e-33a7-4885-a6d0-1caef40320a7"));
			//if (DiscountTypeDataTypeDef == null)
			//{
			//	DiscountTypeDataTypeDef = new DataTypeDefinition(-1, new Guid("c66b5dea-00f7-49df-bc5a-074e5802bce2"));
			//	DiscountTypeDataTypeDef.Name = "Discount Picker";
			//	DiscountTypeDataTypeDef.Key = new Guid("2d89188e-33a7-4885-a6d0-1caef40320a7");
			//	DiscountTypeDataTypeDef.DatabaseType = DataTypeDatabaseType.Ntext;
			//}

			IO.Container.Resolve<ICMSInstaller>().Install();
			//ContentInstaller.InstallContent();
			return;
		}

		public void DeleteAllDocTypes()
		{
			var contentTypeService = ApplicationContext.Current.Services.ContentTypeService;
			// contentTypeService.GetAllContentTypes().Where(c => c.Level)
			var allContentTypes = contentTypeService.GetAllContentTypes().ToList();
			contentTypeService.Delete(contentTypeService.GetAllContentTypes().Where(c => !allContentTypes.Any(d => d.ParentId == c.Id)));

			allContentTypes = contentTypeService.GetAllContentTypes().ToList();
			contentTypeService.Delete(contentTypeService.GetAllContentTypes().Where(c => !allContentTypes.Any(d => d.ParentId == c.Id)));
			allContentTypes = contentTypeService.GetAllContentTypes().ToList();
			contentTypeService.Delete(contentTypeService.GetAllContentTypes().Where(c => !allContentTypes.Any(d => d.ParentId == c.Id)));

			var dataTypeService = ApplicationContext.Current.Services.DataTypeService;

			//dataTypeService.GetAllDataTypeDefinitions().Where(d => !new List<int>{-87, 1041, -49}.Contains(d.Id)).ForEach(dt => dataTypeService.Delete(dt));
		}

		public string RunT4Code()
		{
			Initialize.Reboot();
			var mapping = new Dictionary<Type, ContentTypeAttribute>();
			var lazyParentIdMapping = new List<Tuple<Type, Type>>();
			var childTypesMapping = new List<Tuple<Type, Type[]>>();

			Write("var contentTypeService = ApplicationContext.Current.Services.ContentTypeService;\r\n");
			Write("var dataTypeService = ApplicationContext.Current.Services.DataTypeService;\r\n");
			Write("var contentTypeList = new List<IContentType>();\r\n");
			Write("var newDataTypesList = new List<IDataTypeDefinition>();\r\n");
			Write("var umbracoVersionMajor = UmbracoVersion.Current.Major;\r\n");

			if (IO.Container == null) throw new Exception("No container");
			
			//var umbracoVersionCode = IO.Container.Resolve<IUmbracoVersion>();
			//var umbracoVersion = UmbracoVersion.Current.Major;

		    var dataTypeDefinitions = new DataTypes.DataTypes(); // todo should use dependency injection but currently fails: IO.Container.Resolve<IDataTypeDefinitions>();
			if (dataTypeDefinitions == null) throw new Exception("IDataTypeDefinitions not declare, DLL missing?");
			var dataTypesToGenerate = dataTypeDefinitions.LoadDataTypeDefinitions();
            
			foreach (var def in dataTypesToGenerate.OrderBy(d => d.Name))
			{
				Write("var " + def.DataType + "DataTypeDef = umbracoVersion.GetDataTypeDefinition(\"" + def.Alias + "\");\r\n");
				if (def.Name != null)
				{
					Write(@"			if (" + def.DataType + @"DataTypeDef == null)
			{				
				" + def.DataType + @"DataTypeDef = umbracoVersion.CreateDataTypeDefinition(-1, """ + def.Alias + @""");
				" + def.DataType + @"DataTypeDef.Name = """ + def.Name + @""";
				" + def.DataType + @"DataTypeDef.Key = new Guid(""" + def.KeyGuid + @""");
				" + def.DataType + @"DataTypeDef.DatabaseType = DataTypeDatabaseType." + def.Type + @";
				
				newDataTypesList.Add(" + def.DataType + @"DataTypeDef);
			}
			");
				}
				else
				{
					Write("if (" + def.DataType + "DataTypeDef == null) throw new Exception(\"Could not load default umbraco " + def.DataType + " datatype\");\r\n");
				}
			}

			Write("if (newDataTypesList.Any()) dataTypeService.Save(newDataTypesList);\r\n");

			Write("\r\n");
			foreach (var def in dataTypesToGenerate.Where(def => def.Name != null).OrderBy(d => d.Name))
			{
				Write("" + def.DataType + "DataTypeDef = dataTypeService.GetDataTypeDefinitionById(new Guid(\"" + def.KeyGuid + "\"));\r\n");
				Write("if (" + def.DataType + "DataTypeDef == null) throw new Exception(\"Could not create and/or load " + def.DataType + " datatype\");\r\n");
			}
			Write("\r\n");

			foreach (var def in dataTypesToGenerate.Where(def => def.PreValues != null && def.PreValues.Any()).OrderBy(d => d.Name))
			{
			    Write("	var " + def.DataType + "Dict = new Dictionary<string, PreValue>();");
                
                foreach (var preValue in def.PreValues)
			    {
                    Write(def.DataType + "Dict.Add(\"" + preValue.Key + "\", new PreValue(\"" + preValue.Value.Value + "\"));");
                }
			    Write("	dataTypeService.SavePreValues(" + def.DataType + "DataTypeDef.Id," + def.DataType + "Dict);\r\n");
			}

			Write("\r\n");
			Write("if (newDataTypesList.Any()) dataTypeService.Save(newDataTypesList);\r\n");
			Write("\r\n");

			foreach (var type in Assembly.GetAssembly(typeof(Settings)).GetTypes().Where(x => Attribute.IsDefined(x, typeof(ContentTypeAttribute), false)).OrderBy(d => d.Name))
			{
				var attribute = (ContentTypeAttribute)type.GetCustomAttributes(typeof(ContentTypeAttribute), false).Single();
				mapping.Add(type, attribute);
				var contentTypeVarName = attribute.Alias + "ContentType";
				Write("\r\n");
				Write("var " + contentTypeVarName + " = contentTypeService.GetContentType(\"" + attribute.Alias + "\") ?? new ContentType(-1) {\r\n");
				Write("Alias = \"" + attribute.Alias + "\",\r\n");
				if (type == typeof(UwebshopRootContentType))
					Write("AllowedAsRoot = true,\r\n");
				Write("Name = \"" + attribute.Name + "\",\r\n");
				Write("Description = \"" + attribute.Description + "\",\r\n");
				Write("Thumbnail = \"" + attribute.Thumbnail + ".png\",\r\n");
				Write("Icon = umbracoVersionMajor > 6 ? \"icon-uwebshop-" + attribute.IconClass + "\" : \"" + InternalHelpers.GetFileNameForIcon(attribute.Icon) + "\",\r\n");
				Write("SortOrder = 1,\r\n");
				Write("AllowedContentTypes = new List<ContentTypeSort>(),\r\n");
				Write("AllowedTemplates = new List<ITemplate>(),\r\n");
				Write("PropertyGroups = new PropertyGroupCollection(new List<PropertyGroup>()),};\r\n");
				Write("contentTypeList.Add(" + contentTypeVarName + ");\r\n");
				Write("\r\n");

				if (attribute.ParentContentType != null)
				{
					lazyParentIdMapping.Add(new Tuple<Type, Type>(type, attribute.ParentContentType));
				}
				if (attribute.AllowedChildTypes != null && attribute.AllowedChildTypes.Any())
				{
					childTypesMapping.Add(new Tuple<Type, Type[]>(type, attribute.AllowedChildTypes));
				}

				var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
					.Where(x => Attribute.IsDefined(x, typeof(ContentPropertyTypeAttribute), false))
					.Select(property => (ContentPropertyTypeAttribute)property.GetCustomAttributes(typeof(ContentPropertyTypeAttribute), false).Single());
				foreach (var property in properties.OrderBy(p => p.SortOrder + ((int)p.Tab * 50)))
				{
					Write("if (" + contentTypeVarName + ".PropertyTypes.All(p => p.Alias != \"" + property.Alias + "\")" +
						(property.Umbraco6Only ? "&& umbracoVersionMajor == 6" : "") +
						" && createMissingProperties == true){\r\n");
					Write("GetOrAddPropertyGroup(" + contentTypeVarName + ", \"" + property.Tab + "\").PropertyTypes.Add(");
					Write("new PropertyType(" + property.DataType + "DataTypeDef) { ");
					Write("Alias = \"" + property.Alias + "\", ");
					Write("Name = \"" + property.Name + "\", ");
					Write("Description = \"" + property.Description + "\",");
					if (property.Mandatory)
						Write("Mandatory = true, ");
					Write("});\r\n");
					Write("}\r\n");
				}
				Write("\r\n");
			}

			Write("contentTypeService.Save(contentTypeList);\r\n");
			Write("\r\n");

			foreach (var tuple in lazyParentIdMapping.OrderBy(d => d.Item1.Name))
			{
				Write(mapping[tuple.Item1].Alias + "ContentType.SetLazyParentId(new Lazy<int>(() => " + mapping[tuple.Item2].Alias + "ContentType.Id));\r\n");
			}
			Write("\r\n");
			Write("contentTypeService.Save(contentTypeList);\r\n");
			Write("\r\n");
			foreach (var tuple in childTypesMapping.OrderBy(d => d.Item1.Name))
			{
				Write(mapping[tuple.Item1].Alias + "ContentType.AllowedContentTypes = new List<ContentTypeSort> { "); // + mapping[tuple.Item2].Alias + "ContentType.Id));");
				foreach (var childType in tuple.Item2)
				{
					if (!mapping.ContainsKey(childType))
						throw new Exception("NOT FOUND: " + childType.Name);
					var alias = mapping[childType].Alias;
					Write("new ContentTypeSort { Alias = \"" + alias + "\", Id = new Lazy<int>(() => " + alias + "ContentType.Id) },");
				}
				Write("};\r\n");
			}
			Write("\r\n");
			Write("contentTypeService.Save(contentTypeList);");
			return generatedCode.ToString();
		}

		private readonly StringBuilder generatedCode = new StringBuilder();

		private void Write(string s)
		{
			generatedCode.Append(s);
		}
	}

	internal partial class CMSInstaller : ICMSInstaller
	{
		private readonly IStockService _stockService;
		private readonly IUmbracoVersion _umbracoVersion;

		public CMSInstaller(IStockService stockService, IUmbracoVersion umbracoVersion)
		{
			_stockService = stockService;
			_umbracoVersion = umbracoVersion;
		}
		partial void InstallGenerated(IUmbracoVersion umbracoVersion, bool createMissingProperties = false);
	    public void Install(bool createMissingProperties = false)
		{
			InstallGenerated(_umbracoVersion, createMissingProperties);
		}

		public bool InstallStorePickerOnNodeWithId(int nodeId, out string feedbackSmall, out string feedbackLarge)
		{
			var contentService = ApplicationContext.Current.Services.ContentService;
			var contentTypeService = ApplicationContext.Current.Services.ContentTypeService;

			var storePickerDataTypeDef = LoadOrCreateStorePickerDataTypeDefinition();

			var homepage = contentService.GetById(nodeId);
			var contentType = homepage.ContentType;
			if (contentType.PropertyTypes.All(p => p.Alias != Constants.StorePickerAlias))
			{
				contentType.AddPropertyType(new PropertyType(storePickerDataTypeDef) { Alias = Constants.StorePickerAlias, Name = "#StorePicker", Description = "#StorePickerDescription", Mandatory = false, });
				contentTypeService.Save(new[] { contentType });
				feedbackSmall = "StorePicker installed!";
				feedbackLarge = "StorePicker installed on " + contentType.Alias + " document type";
				return true;
			}
			feedbackSmall = "StorePicker install ignored";
			feedbackLarge = "StorePicker install ignored on " + contentType.Alias + " document type, property with alias " + Constants.StorePickerAlias + " already existed";
			return true;
		}

		private IDataTypeDefinition LoadOrCreateStorePickerDataTypeDefinition()
		{
			var dataTypeService = ApplicationContext.Current.Services.DataTypeService;
			var newDataTypesList = new List<IDataTypeDefinition>();
			var storePickerDataTypeDef = _umbracoVersion.GetDataTypeDefinition("uWebshop.StorePicker");
			if (storePickerDataTypeDef == null)
			{
				storePickerDataTypeDef = new DataTypeDefinition(-1, new Guid("5fa345e3-9352-45d6-adaa-2da6cdc9aca3"));
				storePickerDataTypeDef.Name = "uWebshop Store Picker";
				storePickerDataTypeDef.Key = new Guid("1e8cdc0b-436e-46f5-bfec-57be45745771");
				storePickerDataTypeDef.DatabaseType = DataTypeDatabaseType.Integer;

				newDataTypesList.Add(storePickerDataTypeDef);
			}
			if (newDataTypesList.Any()) dataTypeService.Save(newDataTypesList);

			storePickerDataTypeDef = _umbracoVersion.GetDataTypeDefinition("uWebshop.StorePicker");
			return storePickerDataTypeDef;
		}

        public bool InstallDemoShopContent()
        {
            var umbracoVersion = IO.Container.Resolve<IUmbracoVersion>();

            var contentService = ApplicationContext.Current.Services.ContentService;

            var contentTypeService = ApplicationContext.Current.Services.ContentTypeService;
            var fileService = ApplicationContext.Current.Services.FileService;

            var stringDataTypeDef = umbracoVersion.GetDataTypeDefinition("Umbraco.Textbox");
            var richTextDataTypeDef = umbracoVersion.GetDataTypeDefinition("Umbraco.TinyMCEv3");
            var storePickerDataTypeDef = umbracoVersion.GetDataTypeDefinition("uWebshop.StorePicker");
            var trueFalseDataTypeDef = umbracoVersion.GetDataTypeDefinition("Umbraco.TrueFalse");

            var homepageTemplate = fileService.GetTemplate("uwbsHomepage");
            var basketTemplate = fileService.GetTemplate("uwbsBasket");
          
            var chekoutCustomerTemplate = fileService.GetTemplate("uwbsCheckoutCustomer");
            var chekoutOverviewTemplate = fileService.GetTemplate("uwbsCheckoutOverview");
            
            var profileLoginCreateTemplate = fileService.GetTemplate("uwbsProfileLoginCreate");
            var profileOrdersTemplate = fileService.GetTemplate("uwbsProfileOrders");
            var profilePasswordForgottenTemplate = fileService.GetTemplate("uwbsProfilePasswordForgotten");
            var profileProfileWishlistTemplate = fileService.GetTemplate("uwbsProfileWishlist");
            var textpageTemplate = fileService.GetTemplate("uwbsTextpage");
            
            var contentTypeList = new List<IContentType>();

            #region icons
            var uwbsDemoMasterIcon = string.Format("icon-uwebshop-{0}", IconClass.uwebshoplogo);
            if (UmbracoVersion.Current.Major < 7)
            {
                uwbsDemoMasterIcon = "bank.png";
            }

            var uwbsDemoHomePageIcon = "icon-home";
            if (UmbracoVersion.Current.Major < 7)
            {
                uwbsDemoHomePageIcon = "bank.png";
            }
         
            var uwbsDemoTextPageIcon = "icon-document";
            if (UmbracoVersion.Current.Major < 7)
            {
                uwbsDemoTextPageIcon = ".sprTreeDoc";
            }

            var uwbsDemoBasketPageIcon = string.Format("icon-uwebshop-{0}", IconClass.shoppingcart);
            if (UmbracoVersion.Current.Major < 7)
            {
                uwbsDemoBasketPageIcon = "wallet.png";
            }

            var uwbsDemoCheckoutCustomerPageIcon = "icon-users";
            if (UmbracoVersion.Current.Major < 7)
            {
                uwbsDemoCheckoutCustomerPageIcon = ".sprTreeUser";
            }

            var uwbsDemoCheckoutPageIcon = string.Format("icon-uwebshop-{0}", IconClass.shoppingcart);
            if (UmbracoVersion.Current.Major < 7)
            {
                uwbsDemoCheckoutPageIcon = "credit-card.png";
            }

            var uwbsDemoProfilePageIcon = string.Format("icon-uwebshop-{0}", IconClass.shoppingcart);
            if (UmbracoVersion.Current.Major < 7)
            {
                uwbsDemoProfilePageIcon = ".sprTreeUser";
            }

            var uwbsDemoProfileOrdersIcon = string.Format("icon-uwebshop-{0}", IconClass.dollar);
            if (UmbracoVersion.Current.Major < 7)
            {
                uwbsDemoProfileOrdersIcon = "clipboard-invoice.png";
            }

            var uwbsDemoProfileForgottenIcon = "icon-bell";
            if (UmbracoVersion.Current.Major < 7)
            {
                uwbsDemoProfileForgottenIcon = ".sprTreeUser";
            }
            var uwbsDemoProfileWishlistIcon = string.Format("icon-uwebshop-{0}", IconClass.clipboard);
            if (UmbracoVersion.Current.Major < 7)
            {
                uwbsDemoProfileWishlistIcon = "clipboard-invoice.png";
            }
            
            #endregion


            // create master
            var uwbsDemoMaster = contentTypeService.GetContentType("uwbsDemoMaster") ?? new ContentType(-1)
            {
                Alias = "uwbsDemoMaster",
                Name = "uWebshop Demosite Master",
                Description = "#DemositeMasterDescription",
                Thumbnail = "Folder.png",
                Icon = uwbsDemoMasterIcon,
                SortOrder = 1,
                AllowedContentTypes = new List<ContentTypeSort>(),
                AllowedTemplates = new List<ITemplate>(),
                PropertyGroups = new PropertyGroupCollection(new List<PropertyGroup>()),
            };

            

            contentTypeList.Add(uwbsDemoMaster);
            if (uwbsDemoMaster.PropertyTypes.All(p => p.Alias != "bodyText"))
            {
                GetOrAddPropertyGroup(uwbsDemoMaster, "Content").PropertyTypes.Add(new PropertyType(richTextDataTypeDef) { Alias = "bodyText", Name = "#BodyText", Description = "#BodyTextDescription", });
            }
            if (uwbsDemoMaster.PropertyTypes.All(p => p.Alias != "umbracoNaviHide"))
            {
                GetOrAddPropertyGroup(uwbsDemoMaster, "Content").PropertyTypes.Add(new PropertyType(trueFalseDataTypeDef) { Alias = "umbracoNaviHide", Name = "#NaviHide", Description = "#NaviHideDescription", });
            }
            
            var uwbsDemoHomePage = contentTypeService.GetContentType("uwbsDemoHome") ?? new ContentType(-1)
            {
                Alias = "uwbsDemoHome",
                Name = "uWebshop Demosite Home",
                Description = "#DemositeHomeDescription",
                Thumbnail = "Folder.png",
                Icon = uwbsDemoHomePageIcon,
                SortOrder = 1,
                AllowedContentTypes = new List<ContentTypeSort>(),
                AllowedTemplates = new List<ITemplate>() { homepageTemplate },
                PropertyGroups = new PropertyGroupCollection(new List<PropertyGroup>()),
            };

            contentTypeList.Add(uwbsDemoHomePage);

            if (uwbsDemoHomePage.PropertyTypes.All(p => p.Alias != "siteName"))
            {
                GetOrAddPropertyGroup(uwbsDemoHomePage, "Site").PropertyTypes.Add(new PropertyType(stringDataTypeDef) { Alias = "siteName", Name = "#SiteName", Description = "#SiteNameDescription", });
            }
            if (uwbsDemoHomePage.PropertyTypes.All(p => p.Alias != "siteDescription"))
            {
                GetOrAddPropertyGroup(uwbsDemoHomePage, "Site").PropertyTypes.Add(new PropertyType(stringDataTypeDef) { Alias = "siteDescription", Name = "#SiteDescription", Description = "#SiteDescriptionDescription", });
            }
            if (uwbsDemoHomePage.PropertyTypes.All(p => p.Alias != "uwbsStorePicker"))
            {
                GetOrAddPropertyGroup(uwbsDemoHomePage, "Site").PropertyTypes.Add(new PropertyType(storePickerDataTypeDef) { Alias = "uwbsStorePicker", Name = "#StorePicker", Description = "#StorePickerDescription", });
            }

            var uwbsTextPage = contentTypeService.GetContentType("uwbsTextpage") ?? new ContentType(-1)
            {
                Alias = "uwbsTextpage",
                Name = "uWebshop Demosite Textpage",
                Description = "#DemositeTextpageDescription",
                Thumbnail = "Folder.png",
                Icon = uwbsDemoTextPageIcon,
                SortOrder = 1,
                AllowedContentTypes = new List<ContentTypeSort>(),
                AllowedTemplates = new List<ITemplate>(){ textpageTemplate },
                PropertyGroups = new PropertyGroupCollection(new List<PropertyGroup>()),
            };
            contentTypeList.Add(uwbsTextPage);

            var uwbsBasketPage = contentTypeService.GetContentType("uwbsBasket") ?? new ContentType(-1)
            {
                Alias = "uwbsBasket",
                Name = "uWebshop Demosite Basket",
                Description = "#DemositeBasketPageDescription",
                Thumbnail = "Folder.png",
                Icon = uwbsDemoBasketPageIcon,
                SortOrder = 1,
                AllowedContentTypes = new List<ContentTypeSort>(),
                AllowedTemplates = new List<ITemplate>() { basketTemplate },
                PropertyGroups = new PropertyGroupCollection(new List<PropertyGroup>()),
            };
            contentTypeList.Add(uwbsBasketPage);

            var uwbsCheckoutPage = contentTypeService.GetContentType("uwbsCheckout") ?? new ContentType(-1)
            {
                Alias = "uwbsCheckout",
                Name = "uWebshop Demosite Checkout",
                Description = "#DemositeCheckoutPageDescription",
                Thumbnail = "Folder.png",
                Icon = uwbsDemoCheckoutPageIcon,
                SortOrder = 1,
                AllowedContentTypes = new List<ContentTypeSort>(),
                AllowedTemplates = new List<ITemplate>() { chekoutOverviewTemplate },
                PropertyGroups = new PropertyGroupCollection(new List<PropertyGroup>()),
            };
            contentTypeList.Add(uwbsCheckoutPage);

            var uwbsCustomerCheckoutPage = contentTypeService.GetContentType("uwbsCustomerCheckout") ?? new ContentType(-1)
            {
                Alias = "uwbsCustomerCheckout",
                Name = "uWebshop Demosite Customer Checkout",
                Description = "#DemositeCustomerCheckoutPageDescription",
                Thumbnail = "Folder.png",
                Icon = uwbsDemoCheckoutCustomerPageIcon,
                SortOrder = 1,
                AllowedContentTypes = new List<ContentTypeSort>(),
                AllowedTemplates = new List<ITemplate>() { chekoutCustomerTemplate },
                PropertyGroups = new PropertyGroupCollection(new List<PropertyGroup>()),
            };
            contentTypeList.Add(uwbsCustomerCheckoutPage);

            var uwbsProfile = contentTypeService.GetContentType("uwbsProfile") ?? new ContentType(-1)
            {
                Alias = "uwbsProfile",
                Name = "uWebshop Demosite Profile",
                Description = "#DemositeProfilePageDescription",
                Thumbnail = "Folder.png",
                Icon = uwbsDemoProfilePageIcon,
                SortOrder = 1,
                AllowedContentTypes = new List<ContentTypeSort>(),
                AllowedTemplates = new List<ITemplate>() { profileLoginCreateTemplate },
                PropertyGroups = new PropertyGroupCollection(new List<PropertyGroup>()),
            };
            contentTypeList.Add(uwbsProfile);

            var uwbsProfileOrders = contentTypeService.GetContentType("uwbsProfileOrders") ?? new ContentType(-1)
            {
                Alias = "uwbsProfileOrders",
                Name = "uWebshop Demosite Profile Orders",
                Description = "#DemositeProfileOrdersPageDescription",
                Thumbnail = "Folder.png",
                Icon = uwbsDemoProfileOrdersIcon,
                SortOrder = 1,
                AllowedContentTypes = new List<ContentTypeSort>(),
                AllowedTemplates = new List<ITemplate>() { profileOrdersTemplate },
                PropertyGroups = new PropertyGroupCollection(new List<PropertyGroup>()),
            };
            contentTypeList.Add(uwbsProfileOrders);

            var uwbsProfilePasswordForgotten = contentTypeService.GetContentType("uwbsProfilePasswordForgotten") ?? new ContentType(-1)
            {
                Alias = "uwbsProfilePasswordForgotten",
                Name = "uWebshop Demosite Password Forgotten",
                Description = "#DemositePasswordForgottenPageDescription",
                Thumbnail = "Folder.png",
                Icon = uwbsDemoProfileForgottenIcon,
                SortOrder = 1,
                AllowedContentTypes = new List<ContentTypeSort>(),
                AllowedTemplates = new List<ITemplate>() { profilePasswordForgottenTemplate },
                PropertyGroups = new PropertyGroupCollection(new List<PropertyGroup>()),
            };
            contentTypeList.Add(uwbsProfilePasswordForgotten);

            var uwbsProfileWishlist = contentTypeService.GetContentType("uwbsProfileWishlist") ?? new ContentType(-1)
            {
                Alias = "uwbsProfileWishlist",
                Name = "uWebshop Demosite Profile Wishlist",
                Description = "#DemositeOrdersWishlistPageDescription",
                Thumbnail = "Folder.png",
                Icon = uwbsDemoProfileWishlistIcon,
                SortOrder = 1,
                AllowedContentTypes = new List<ContentTypeSort>(),
                AllowedTemplates = new List<ITemplate>() { profileProfileWishlistTemplate },
                PropertyGroups = new PropertyGroupCollection(new List<PropertyGroup>()),
            };

            contentTypeList.Add(uwbsProfileWishlist);

            contentTypeService.Save(contentTypeList);

            uwbsDemoHomePage.SetLazyParentId(new Lazy<int>(() => uwbsDemoMaster.Id));
            uwbsTextPage.SetLazyParentId(new Lazy<int>(() => uwbsDemoMaster.Id));
            uwbsBasketPage.SetLazyParentId(new Lazy<int>(() => uwbsTextPage.Id));
            uwbsCheckoutPage.SetLazyParentId(new Lazy<int>(() => uwbsTextPage.Id));
            uwbsCustomerCheckoutPage.SetLazyParentId(new Lazy<int>(() => uwbsTextPage.Id));
            uwbsProfile.SetLazyParentId(new Lazy<int>(() => uwbsTextPage.Id));
            uwbsProfileOrders.SetLazyParentId(new Lazy<int>(() => uwbsTextPage.Id));
            uwbsProfilePasswordForgotten.SetLazyParentId(new Lazy<int>(() => uwbsTextPage.Id));
            uwbsProfileWishlist.SetLazyParentId(new Lazy<int>(() => uwbsTextPage.Id));

            contentTypeService.Save(contentTypeList);

            uwbsDemoHomePage.AddContentType(uwbsDemoMaster);
            uwbsTextPage.AddContentType(uwbsDemoMaster);
            uwbsBasketPage.AddContentType(uwbsDemoMaster);
            uwbsCheckoutPage.AddContentType(uwbsDemoMaster);
            uwbsCustomerCheckoutPage.AddContentType(uwbsDemoMaster);
            uwbsProfile.AddContentType(uwbsDemoMaster);
            uwbsProfileOrders.AddContentType(uwbsDemoMaster);
            uwbsProfilePasswordForgotten.AddContentType(uwbsDemoMaster);
            uwbsProfileWishlist.AddContentType(uwbsDemoMaster);
            
            contentTypeService.Save(contentTypeList);

            uwbsDemoHomePage.AllowedContentTypes = new List<ContentTypeSort>
            {
                new ContentTypeSort { Alias = uwbsBasketPage.Alias, Id = new Lazy<int>(() => uwbsBasketPage.Id) },
                new ContentTypeSort { Alias = uwbsCheckoutPage.Alias, Id = new Lazy<int>(() => uwbsCheckoutPage.Id) },
                new ContentTypeSort { Alias = uwbsProfile.Alias, Id = new Lazy<int>(() => uwbsProfile.Id) },
                new ContentTypeSort { Alias = uwbsTextPage.Alias, Id = new Lazy<int>(() => uwbsTextPage.Id) },
            };
            
            uwbsTextPage.AllowedContentTypes = new List<ContentTypeSort>
            {
                new ContentTypeSort { Alias = uwbsBasketPage.Alias, Id = new Lazy<int>(() => uwbsBasketPage.Id) },
                new ContentTypeSort { Alias = uwbsCheckoutPage.Alias, Id = new Lazy<int>(() => uwbsCheckoutPage.Id) },
                new ContentTypeSort { Alias = uwbsProfile.Alias, Id = new Lazy<int>(() => uwbsProfile.Id) },
                new ContentTypeSort { Alias = uwbsTextPage.Alias, Id = new Lazy<int>(() => uwbsTextPage.Id) },
            };

            uwbsBasketPage.AllowedContentTypes = new List<ContentTypeSort>
            {
                new ContentTypeSort { Alias = uwbsCustomerCheckoutPage.Alias, Id = new Lazy<int>(() => uwbsCustomerCheckoutPage.Id) },
                new ContentTypeSort { Alias = uwbsTextPage.Alias, Id = new Lazy<int>(() => uwbsTextPage.Id) },
            };

            uwbsCustomerCheckoutPage.AllowedContentTypes = new List<ContentTypeSort>
            {
                new ContentTypeSort { Alias = uwbsCheckoutPage.Alias, Id = new Lazy<int>(() => uwbsCheckoutPage.Id) },
                new ContentTypeSort { Alias = uwbsTextPage.Alias, Id = new Lazy<int>(() => uwbsTextPage.Id) },
            };

            uwbsCheckoutPage.AllowedContentTypes = new List<ContentTypeSort>
            {
                new ContentTypeSort { Alias = uwbsTextPage.Alias, Id = new Lazy<int>(() => uwbsTextPage.Id) },
            };

            uwbsProfile.AllowedContentTypes = new List<ContentTypeSort>
            {
                new ContentTypeSort { Alias = uwbsProfilePasswordForgotten.Alias, Id = new Lazy<int>(() => uwbsProfilePasswordForgotten.Id) },
                new ContentTypeSort { Alias = uwbsProfileOrders.Alias, Id = new Lazy<int>(() => uwbsProfileOrders.Id) },
                new ContentTypeSort { Alias = uwbsProfileWishlist.Alias, Id = new Lazy<int>(() => uwbsProfileWishlist.Id) },
                new ContentTypeSort { Alias = uwbsTextPage.Alias, Id = new Lazy<int>(() => uwbsTextPage.Id) },
            };

            uwbsProfileOrders.AllowedContentTypes = new List<ContentTypeSort>
            {
                new ContentTypeSort { Alias = uwbsTextPage.Alias, Id = new Lazy<int>(() => uwbsTextPage.Id) },
            };

            uwbsProfileWishlist.AllowedContentTypes = new List<ContentTypeSort>
            {
                new ContentTypeSort { Alias = uwbsTextPage.Alias, Id = new Lazy<int>(() => uwbsTextPage.Id) },
            };
            uwbsProfilePasswordForgotten.AllowedContentTypes = new List<ContentTypeSort>
            {
                new ContentTypeSort { Alias = uwbsTextPage.Alias, Id = new Lazy<int>(() => uwbsTextPage.Id) },
            };

            contentTypeService.Save(contentTypeList);


            #region bah vies oude API stuff

			var ctuwbsDemoHomePage = contentTypeService.GetContentType(uwbsDemoHomePage.Id);
			ctuwbsDemoHomePage.ParentId = uwbsDemoMaster.Id;
			contentTypeService.Save(ctuwbsDemoHomePage);

            #endregion


                var contentList = new List<IContent>();

            var homePage = ContentInstaller.GetOrCreateContent(uwbsDemoHomePage, "Home", contentTypeService, contentService, null, contentList);

            if (homePage.PropertyTypes.Any(x => x.Alias == "siteName"))
            {
                homePage.SetValue("siteName", "uWebshop Demoshop");
            }
            if (homePage.PropertyTypes.Any(x => x.Alias == "siteDescription"))
            {
                homePage.SetValue("siteDescription", "uWebshop Demo Starterstore");
            }
            if (homePage.PropertyTypes.Any(x => x.Alias == "bodyText"))
            {
                homePage.SetValue("bodyText",
                    @"<p>The uWebshop Demoshop you ready-to-start uWebshop store that introduces you to a set of well-defined conventions for building an uWebshop based webshop!</p>
<p>uWebshop can be easily integrated with any existing website or starterkit already available for Umbraco. In a few easy steps explained during installation you will have a fully working webshop without having to learn anything new or make big changes to your current site!</p>
<p>If you see some errors try to publish the uWebshop Node and all children.</p>");
            }

            var basketPage = ContentInstaller.GetOrCreateContent(uwbsBasketPage, "Basket", contentTypeService, contentService, homePage, contentList);
            
            var customerDetailsPage = ContentInstaller.GetOrCreateContent(uwbsCustomerCheckoutPage, "Customer Details", contentTypeService, contentService, basketPage, contentList);

            var checkoutPage = ContentInstaller.GetOrCreateContent(uwbsCheckoutPage, "Checkout", contentTypeService, contentService, customerDetailsPage, contentList);

            var successPage = ContentInstaller.GetOrCreateContent(uwbsTextPage, "Success", contentTypeService, contentService, checkoutPage, contentList);
            if (successPage.PropertyTypes.Any(x => x.Alias == "bodyText"))
            {
                successPage.SetValue("bodyText", "<p>Successfully placed the order!</p>");
            }

            var errorPage = ContentInstaller.GetOrCreateContent(uwbsTextPage, "Failed", contentTypeService, contentService, checkoutPage, contentList);
            if (errorPage.PropertyTypes.Any(x => x.Alias == "bodyText"))
            {
                errorPage.SetValue("bodyText", "<p>Failed to confirm the order :(</p>");
            }

            var profilePage = ContentInstaller.GetOrCreateContent(uwbsProfile, "Profile", contentTypeService, contentService, homePage, contentList);

            var passwordForgottenPage = ContentInstaller.GetOrCreateContent(uwbsProfilePasswordForgotten, "Password Forgotten", contentTypeService, contentService, profilePage, contentList);
            if (passwordForgottenPage.PropertyTypes.Any(x => x.Alias == "bodyText"))
            {
                passwordForgottenPage.SetValue("bodyText",
                    "<p>If you forgot your password you can request a new one to be send to the e-mailaddres in your account, using the form below.</p>");
            }
            var wishlistsPage = ContentInstaller.GetOrCreateContent(uwbsProfileWishlist, "Wishlists", contentTypeService, contentService, profilePage, contentList);

            var ordersPage = ContentInstaller.GetOrCreateContent(uwbsProfileOrders, "Orders", contentTypeService, contentService, profilePage, contentList);

            var logoutPage = ContentInstaller.GetOrCreateContent(uwbsTextPage, "Logout", contentTypeService, contentService, profilePage, contentList);
            if (logoutPage.PropertyTypes.Any(x => x.Alias == "bodyText"))
            {
                logoutPage.SetValue("bodyText", "<p>You are now logged-out.</p>");
            }
            contentService.Save(contentList);
            contentService.PublishWithChildrenWithStatus(homePage);

	        return true;
	    }

	    public bool InstallStarterkit(string starterkit, out bool storePresent)
	    {
            InstallDemoShopContent();

	        storePresent = false;
	        var contentService = ApplicationContext.Current.Services.ContentService;
	        var contentTypeService = ApplicationContext.Current.Services.ContentTypeService;
	        var fileService = ApplicationContext.Current.Services.FileService;

	        var categoryDocType = contentTypeService.GetContentType(Category.NodeAlias);
	        var productDocType = contentTypeService.GetContentType(Product.NodeAlias);
	        var storeDocType = contentTypeService.GetContentType(Store.NodeAlias);

	        if (storeDocType == null || categoryDocType == null || productDocType == null) return false;

	        storePresent = contentService.GetContentOfContentType(storeDocType.Id).Any(x => !x.Trashed);

	        var categoryTemplate = fileService.GetTemplate(Category.NodeAlias) as Template;
	        categoryDocType.AllowedTemplates =
	            categoryDocType.AllowedTemplates.Concat(new List<ITemplate> {categoryTemplate});
	        categoryDocType.SetDefaultTemplate(categoryTemplate);

	        var productTemplate = fileService.GetTemplate(Product.NodeAlias) as Template;
	        productDocType.AllowedTemplates = productDocType.AllowedTemplates.Concat(new List<ITemplate> {productTemplate});
	        productDocType.SetDefaultTemplate(productTemplate);

	        contentTypeService.Save(new[] {categoryDocType, productDocType});

	        var contentToSaveAndPublish = new List<IContent>(15);

	        if (!storePresent)
	        {
	            Umbraco.Helpers.InstallStore("uWebshop");
	        }
	        var stores = contentService.GetContentOfContentType(storeDocType.Id);
	        var store = stores.FirstOrDefault(x => !x.Trashed);

	        var firstLanguage = Language.GetAllAsList().FirstOrDefault();

	        if (store != null && (!storePresent || stores.All(s => !s.Published)))
	        {
	            var email = string.Format(storePresent ? "starterkitdemo@{0}.com" : "info@{0}.com", store.Name);

	            if (firstLanguage != null) store.SetValue("storeCulture", firstLanguage.id);
	            store.SetValue("countryCode", "NL");
	            store.SetValue("currencies", "EUR|1#USD|1.3");
	            store.SetValue("orderNumberPrefix", store.Name);
	            store.SetValue("globalVat", "0");
	            store.SetValue("storeEmailFrom", email);
	            store.SetValue("storeEmailTo", email);
	            store.SetValue("storeEmailFromName", "uWebshop Demo");
	            contentToSaveAndPublish.Add(store);
	        }

            var homepagePackageDocType = contentTypeService.GetContentType("uwbsDemoHome");

	        if (homepagePackageDocType == null)
	        {
	            homepagePackageDocType = contentTypeService.GetContentType("uwbsHomepage"); 
	        }

	if (starterkit.ToLowerInvariant() == "sandbox" || string.IsNullOrEmpty(starterkit))
			{

				if (homepagePackageDocType.PropertyTypes.All(p => p.Alias != Constants.StorePickerAlias))
				{
					homepagePackageDocType.AddPropertyType(new PropertyType(LoadOrCreateStorePickerDataTypeDefinition())
					{
						Alias =
							Constants
							.StorePickerAlias,
						Name =
							"#StorePicker",
						Description =
							"#StorePickerDescription",
						Mandatory =
							false,
					});
					contentTypeService.Save(new[] { homepagePackageDocType });
					homepagePackageDocType = contentTypeService.GetContentType("uwbsHomepage");
				}



				var categoryRepositoryDocType = contentTypeService.GetContentType(Catalog.CategoryRepositoryNodeAlias);
				var categoryRepository =
					contentService.GetContentOfContentType(categoryRepositoryDocType.Id).FirstOrDefault(x => !x.Trashed);

				var testCategory = contentService.CreateContent("Your First Category", categoryRepository, Category.NodeAlias);
				testCategory.SetValue("title", "Your First Category");
				testCategory.SetValue("url", "Your First Category");
				testCategory.SetValue("metaDescription", "Your First Category");
				contentToSaveAndPublish.Add(testCategory);

				var productContent = CreateProductContent(contentService, testCategory, "Your First Product", "PROD001", "10000", 5, "Your First Product", "Your First Product Description", false);
				contentToSaveAndPublish.Add(productContent);

			    var productContent2 = CreateProductContent(contentService, testCategory, "Your Second Product", "PROD002",
			        "5000", 10, "Your Second Product", "Your Second Product Description", false);

                contentToSaveAndPublish.Add(productContent2);

                var variantGroupColor = CreateVariantGroupContent(contentService, productContent2, "Color", true);
                contentToSaveAndPublish.Add(variantGroupColor);

                var variantGroupType = CreateVariantGroupContent(contentService, productContent2, "Type", true);
                contentToSaveAndPublish.Add(variantGroupType);

                contentToSaveAndPublish.Add(CreateVariantContent(contentService, variantGroupColor, "Orange", "VARCOL001",
					"1000", 5, "Your First Color Variant", true));
                contentToSaveAndPublish.Add(CreateVariantContent(contentService, variantGroupColor, "Blue", "VARCOL002",
					"2000", 11, "Your Second Color Variant", true));
                contentToSaveAndPublish.Add(CreateVariantContent(contentService, variantGroupType, "Manual", "VARTYP001",
					"500", 14, "Your First Type Variant", true));
                contentToSaveAndPublish.Add(CreateVariantContent(contentService, variantGroupType, "Automatic", "VARTYP002",
					"200", 9, "Your Second Type Variant", true));
			}

			if (starterkit.ToLowerInvariant() == "demo")
			{
				// create 2 extra properties on the product document type for the demo starterkit.
				var productDoctype = contentTypeService.GetContentType(Product.NodeAlias);

				var version = IO.Container.Resolve<IUmbracoVersion>();
				var textboxMultipleDataTypeDef = version.GetDataTypeDefinition("Umbraco.TextboxMultiple");
				var RTEDataTypeDef = version.GetDataTypeDefinition("Umbraco.TinyMCEv3");

				if (textboxMultipleDataTypeDef == null) throw new Exception("Umbraco.TextboxMultiple DataType not found");
				if (RTEDataTypeDef == null) throw new Exception("Umbraco.TinyMCEv3 DataType not found");

				// add introduction
				if (productDoctype.PropertyTypes.All(x => x.Alias.ToLowerInvariant() != "introduction"))
				{
					GetOrAddPropertyGroup(productDoctype, "Details")
						.PropertyTypes.Add(new PropertyType(textboxMultipleDataTypeDef)
						{
							Alias = "introduction",
							Name = "Introduction",
							Description = "Product Introduction",
							Mandatory = false,
							SortOrder = 1,
						});
				}
				// add features
				if (productDoctype.PropertyTypes.All(x => x.Alias.ToLowerInvariant() != "features"))
				{
					GetOrAddPropertyGroup(productDoctype, "Details")
						.PropertyTypes.Add(new PropertyType(RTEDataTypeDef)
						{
							Alias = "features",
							Name = "Features",
							Description = "Product Features",
							Mandatory = false,
							SortOrder = 2,
						});
				}

				contentTypeService.Save(productDoctype);

				var categoryRepositoryDocType = contentTypeService.GetContentType(Catalog.CategoryRepositoryNodeAlias);
				var categoryRepository =
					contentService.GetContentOfContentType(categoryRepositoryDocType.Id).FirstOrDefault(x => !x.Trashed);

				var testCategory = contentService.CreateContent("Clothing", categoryRepository, Category.NodeAlias);
				testCategory.SetValue("title", "Clothing");
				testCategory.SetValue("url", "clothing");
				testCategory.SetValue("metaDescription", "The Clothing Category");
				contentToSaveAndPublish.Add(testCategory);

				// dummy text
				const string lipsumDescription = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Fusce orci diam, laoreet eu mauris id, tempor dictum velit. Mauris sed lectus pulvinar, placerat lectus vitae, rhoncus lorem. Fusce laoreet nisi elit, ut accumsan ligula aliquam ac. Integer dignissim sollicitudin iaculis. Donec scelerisque nibh at leo molestie, a blandit odio pharetra. Fusce eu porta quam. Vestibulum accumsan congue mi sollicitudin luctus. Proin ac purus eget neque placerat adipiscing at et augue. Sed tempor, est ut imperdiet accumsan, orci dolor pretium lorem, ac porta enim turpis ut est. Morbi volutpat bibendum ullamcorper. Cras lacinia varius tincidunt.";
				const string lipsumIntroduction = "Fusce orci diam, laoreet eu mauris id, tempor dictum velit. Mauris sed lectus pulvinar, placerat lectus vitae, rhoncus lorem.";
				const string lipsumDescription2 = "In faucibus, orci ut aliquam imperdiet, lectus leo gravida tellus, non fringilla dui justo sed dui. Nunc vitae sollicitudin orci. In hac habitasse platea dictumst. Donec mattis vitae elit vitae lacinia. Morbi et dui sed nibh pellentesque consectetur at vitae elit. Curabitur vitae lacus arcu. Morbi non turpis commodo, accumsan turpis at, tincidunt velit. Nulla nec justo nec urna malesuada molestie sit amet eget lacus. Vestibulum facilisis varius aliquam. Praesent pellentesque odio eu elit semper suscipit. Morbi ac massa nec odio sollicitudin aliquet. Donec blandit luctus purus vel pulvinar. Aenean ultrices, purus non congue auctor, nibh eros tincidunt erat, sit amet interdum tellus diam ut metus. Donec ac hendrerit augue.";
				const string lipsumIntroduction2 = "Mauris id laoreet erat, ac egestas arcu. Aliquam et ipsum ut tortor suscipit pretium et ut quam. In nec consequat massa, ut placerat nulla. Mauris bibendum, arcu nec lacinia varius, nibh arcu tempus lectus, sed cursus justo enim eu risus.";

				// dummy features
				const string fullfeature = "<ul class=\"list-grou\"><li class=\"list-group-item\"> <span class=\"badge\">1</span><h4 class=\"list-group-item-heading\">Feature One</h4><p class=\"list-group-item-text\">Fusce orci diam, laoreet eu mauris id, tempor dictum velit.</p></li><li class=\"list-group-item\"> <span class=\"glyphicon glyphicon-ok pull-right\"></span><h4 class=\"list-group-item-heading\">Feature Two</h4></li><li class=\"list-group-item\"> <span class=\"badge\">2</span><h4 class=\"list-group-item-heading\">Feature Three</h4><p class=\"list-group-item-text\">Fusce orci diam, laoreet eu mauris id, tempor dictum velit.</p></li><li class=\"list-group-item\"> <span class=\"glyphicon glyphicon-ok pull-right\"></span><h4 class=\"list-group-item-heading\">Feature Four</h4></li><li class=\"list-group-item\"> <span class=\"glyphicon glyphicon-ok pull-right\"></span><h4 class=\"list-group-item-heading\">Feature Six</h4></li><li class=\"list-group-item\"> <span class=\"badge\">10</span><h4 class=\"list-group-item-heading\">Feature Five</h4><p class=\"list-group-item-text\">Fusce orci diam, laoreet eu mauris id, tempor dictum velit.</p></li></ul>";
				const string halffeature = "<ul class=\"list-grou\"><li class=\"list-group-item\"> <span class=\"badge\">1</span><h4 class=\"list-group-item-heading\">Feature One</h4><p class=\"list-group-item-text\">Fusce orci diam, laoreet eu mauris id, tempor dictum velit.</p></li><li class=\"list-group-item\"> <span class=\"glyphicon glyphicon-ok pull-right\"></span><h4 class=\"list-group-item-heading\">Feature Two</h4></li><li class=\"list-group-item\"> <span class=\"badge\">2</span><h4 class=\"list-group-item-heading\">Feature Three</h4><p class=\"list-group-item-text\">Fusce orci diam, laoreet eu mauris id, tempor dictum velit.</p></li></ul>";



				var firstProduct = CreateProductContent(contentService, testCategory, "T-Shirt", "PROD001", "2500", 0, "A nice T-Shirt", lipsumDescription, true, lipsumIntroduction, halffeature);
				contentToSaveAndPublish.Add(firstProduct);

                var variantGroupSizeColor = CreateVariantGroupContent(contentService, firstProduct, " Size and Color", true);
                contentToSaveAndPublish.Add(variantGroupSizeColor);

                contentToSaveAndPublish.Add(CreateVariantContent(contentService, variantGroupSizeColor, "Small Blue", "VARCOL001", "1000", 8, "Small Blue", true));
                contentToSaveAndPublish.Add(CreateVariantContent(contentService, variantGroupSizeColor, "Medium Blue", "VARCOL002", "1000", 3, "Medium Blue", true));
                contentToSaveAndPublish.Add(CreateVariantContent(contentService, variantGroupSizeColor, "Large Blue", "VARCOL003", "1000", 2, "Large Blue", true));
                contentToSaveAndPublish.Add(CreateVariantContent(contentService, variantGroupSizeColor, "Small Red", "VARCOL004", "1000", 15, "Small Red", true));
                contentToSaveAndPublish.Add(CreateVariantContent(contentService, variantGroupSizeColor, "Medium Red", "VARCOL005", "1000", 4, "Medium Red", true));
                contentToSaveAndPublish.Add(CreateVariantContent(contentService, variantGroupSizeColor, "Large Red", "VARCOL006", "1000", 7, "Large Red", true));

				var secondProduct = CreateProductContent(contentService, testCategory, "Sweater", "PROD002", "7500", 5, "A beatiful sweater", lipsumDescription2, true, lipsumIntroduction2, halffeature);
				contentToSaveAndPublish.Add(secondProduct);
                var variantGroupSize = CreateVariantGroupContent(contentService, secondProduct, "Size", true);
                contentToSaveAndPublish.Add(variantGroupSize);

                contentToSaveAndPublish.Add(CreateVariantContent(contentService, variantGroupSize, "Small", "VARSIZ001", "0", 8, "Small", false));
                contentToSaveAndPublish.Add(CreateVariantContent(contentService, variantGroupSize, "Medium", "VARSIZ002", "0", 3, "Medium", false));
                contentToSaveAndPublish.Add(CreateVariantContent(contentService, variantGroupSize, "Large", "VARSIZ003", "0", 2, "Large", false));

				var secondCategory = contentService.CreateContent("Shoes", categoryRepository, Category.NodeAlias);
				secondCategory.SetValue("title", "Shoes");
				secondCategory.SetValue("url", "shoes");
				secondCategory.SetValue("metaDescription", "The Shoes Category");
				contentToSaveAndPublish.Add(secondCategory);

				var shoeProduct1 = CreateProductContent(contentService, secondCategory, "Running Shoe", "PROD003", "12000", 0, "Run fast with this amazing running shoe!", lipsumDescription2, true, lipsumIntroduction, fullfeature);
				contentToSaveAndPublish.Add(shoeProduct1);

                var variantGroupSizeShoe1 = CreateVariantGroupContent(contentService, shoeProduct1, "Size", true);
                contentToSaveAndPublish.Add(variantGroupSizeShoe1);

                contentToSaveAndPublish.Add(CreateVariantContent(contentService, variantGroupSizeShoe1, "Size 4", "VARSIZ001", "0", 8, "Size 4", true));
                contentToSaveAndPublish.Add(CreateVariantContent(contentService, variantGroupSizeShoe1, "Size 5", "VARSIZ002", "0", 3, "Size 5", true));
                contentToSaveAndPublish.Add(CreateVariantContent(contentService, variantGroupSizeShoe1, "Size 6", "VARSIZ003", "0", 2, "Size 6", true));
                contentToSaveAndPublish.Add(CreateVariantContent(contentService, variantGroupSizeShoe1, "Size 7", "VARSIZ004", "0", 15, "Size 7", true));
                contentToSaveAndPublish.Add(CreateVariantContent(contentService, variantGroupSizeShoe1,"Size 8", "VARSIZ005", "0", 4, "Size 8", true));
                contentToSaveAndPublish.Add(CreateVariantContent(contentService, variantGroupSizeShoe1,"Size 9", "VARSIZ006", "0", 7, "Size 9", true));

				var shoeProduct2 = CreateProductContent(contentService, secondCategory, "Sneaker", "PROD005", "8000", 0, "Very nice sneaker to impress others!", lipsumDescription, true, lipsumIntroduction2, halffeature);
				contentToSaveAndPublish.Add(shoeProduct2);

                var variantGroupSizeShoe2 = CreateVariantGroupContent(contentService, shoeProduct2, "Size", true);
                contentToSaveAndPublish.Add(variantGroupSizeShoe2);

                contentToSaveAndPublish.Add(CreateVariantContent(contentService, variantGroupSizeShoe2, "Size 4", "VARSIZ001", "0", 8, "Size 4", true));
                contentToSaveAndPublish.Add(CreateVariantContent(contentService, variantGroupSizeShoe2, "Size 5", "VARSIZ002", "0", 3, "Size 5", true));
                contentToSaveAndPublish.Add(CreateVariantContent(contentService, variantGroupSizeShoe2, "Size 6", "VARSIZ003", "0", 2, "Size 6", true));
                contentToSaveAndPublish.Add(CreateVariantContent(contentService, variantGroupSizeShoe2, "Size 7", "VARSIZ004", "0", 15, "Size 7", true));
                contentToSaveAndPublish.Add(CreateVariantContent(contentService, variantGroupSizeShoe2, "Size 8", "VARSIZ005", "0", 4, "Size 8", true));
                contentToSaveAndPublish.Add(CreateVariantContent(contentService, variantGroupSizeShoe2, "Size 9", "VARSIZ006", "0", 7, "Size 9", true));

				var thirdCategory = contentService.CreateContent("Electronics", categoryRepository, Category.NodeAlias);
				thirdCategory.SetValue("title", "Electronics");
				thirdCategory.SetValue("url", "electronics");
				thirdCategory.SetValue("metaDescription", "The Electronics Category");
				contentToSaveAndPublish.Add(thirdCategory);

				var elecProduct1 = CreateProductContent(contentService, thirdCategory, "Smartphone", "PROD006", "40000", 400, "Fast, light, big screen fantastic smarphone!", lipsumDescription, false, lipsumIntroduction2, halffeature);
				contentToSaveAndPublish.Add(elecProduct1);

				var elecProduct2 = CreateProductContent(contentService, thirdCategory, "OLED Television", "PROD007", "650000", 8, "This OLED TV has the most amazing screen available!", lipsumDescription2, false, lipsumIntroduction, fullfeature);
				contentToSaveAndPublish.Add(elecProduct2);
			    var variantGroup2 = CreateVariantGroupContent(contentService, elecProduct2, " Installation", false);
                contentToSaveAndPublish.Add(variantGroup2);
			    var variant2 = CreateVariantContent(contentService, variantGroup2, "Home Installation", "INST001", "5000", 0, "Home Installation", true);
                contentToSaveAndPublish.Add(variant2);

				var elecProduct3 = CreateProductContent(contentService, thirdCategory, "Tablet Device", "PROD008", "41500", 500, "Big 10inch tablet device, the perfect companion on the couch!", lipsumDescription, false, lipsumIntroduction2, fullfeature);
				contentToSaveAndPublish.Add(elecProduct3);
			}

			var uwebshopDocType = contentTypeService.GetContentType("uWebshop");
			var uwebshop = contentService.GetContentOfContentType(uwebshopDocType.Id).FirstOrDefault(x => !x.Trashed);
			if (uwebshop != null)
			{
				uwebshop.SortOrder = 10;
				contentToSaveAndPublish.Add(uwebshop);
			}


			var homepage = contentService.GetContentOfContentType(homepagePackageDocType.Id).OrderByDescending(x => x.CreateDate).FirstOrDefault(x => !x.Trashed);
			if (homepage != null)
			{
				if (store != null)
				{
					if (homepage.HasProperty(Constants.StorePickerAlias))
					{
						homepage.SetValue(Constants.StorePickerAlias, store.Id);
					}
				}
				homepage.SortOrder = 1;
				//contentToSaveAndPublish.Add(homepage);
				contentService.PublishWithChildrenWithStatus(homepage);
			}

			contentService.Save(contentToSaveAndPublish);
			contentToSaveAndPublish.ForEach(content => contentService.PublishWithStatus(content));

            

			contentService.RePublishAll();

			return true;
		}

		private IContent CreateProductContent(IContentService contentService, IContent testCategory, string title, string sku, string price, int stock, string metaDescription, string description, bool useVariantStock, string introduction = null, string features = null)
		{
			var product = contentService.CreateContent(title, testCategory, Product.NodeAlias);
			product.SetValue("title", title);
			product.SetValue("url", title);
			product.SetValue("sku", sku);
			product.SetValue("price", price);
			_stockService.SetStock(product.Id, stock, false, string.Empty);

			product.SetValue("metaDescription", metaDescription);
			product.SetValue("description", description);

			if (useVariantStock)
			{
				product.SetValue("useVariantStock", true);
			}

			if (introduction != null)
			{
				if (product.HasProperty("introduction"))
				{
					product.SetValue("introduction", introduction);
				}
			}

			if (features != null)
			{
				if (product.HasProperty("features"))
				{
					product.SetValue("features", features);
				}
			}

			return product;
		}

        private static IContent CreateVariantGroupContent(IContentService contentService, IContent testProduct, string variantGroup, bool requiredVariant)
        {
            var testProductVariantGroup = contentService.CreateContent(variantGroup, testProduct, ProductVariantGroup.NodeAlias);
            testProductVariantGroup.SetValue("title", variantGroup);
            if (requiredVariant)
            {
                testProductVariantGroup.SetValue("required", true);
            }


            return testProductVariantGroup;
        }

		private IContent CreateVariantContent(IContentService contentService, IContent testProductVariantGroup, string color, string sku, string price, int stock, string variantName, bool enableBackorders = false)
		{
		    var testProductVariant = contentService.CreateContent(variantName, testProductVariantGroup, ProductVariant.NodeAlias);
			testProductVariant.SetValue("title", color);
			testProductVariant.SetValue("sku", sku);
			testProductVariant.SetValue("price", price);
			testProductVariant.SetValue("backorderStatus", "disable");
			_stockService.SubstractStock(testProductVariant.Id, stock, false, string.Empty);

			if (enableBackorders)
			{
				testProductVariant.SetValue("backorderStatus", "enable");
			}

			return testProductVariant;
		}

		public static PropertyGroup GetOrAddPropertyGroup(IContentType contentType, string groupName)
		{
			var group = contentType.PropertyGroups.FirstOrDefault(g => g.Name == groupName);
			if (group == null)
			{
				var sortOrder = contentType.PropertyGroups.Select(g => g.SortOrder).OrderByDescending(x => x).FirstOrDefault() + 1;
				group = new PropertyGroup { Name = groupName, SortOrder = sortOrder };
				contentType.PropertyGroups.Add(group);
			}
			return group;
		}
	}
}