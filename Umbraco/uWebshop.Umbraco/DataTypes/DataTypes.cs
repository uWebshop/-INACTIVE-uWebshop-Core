using System;
using System.Collections.Generic;
using uWebshop.Common;
using uWebshop.Domain;
using uWebshop.Domain.ContentTypes;
using uWebshop.Domain.Core;
using uWebshop.Domain.Interfaces;
using uWebshop.Umbraco.DataTypes.CountySelector;
using uWebshop.Umbraco.DataTypes.CouponCodeEditor;
using uWebshop.Umbraco.DataTypes.Currencies;
using uWebshop.Umbraco.DataTypes.DiscountOrderCondition;
using uWebshop.Umbraco.DataTypes.DiscountType;
using uWebshop.Umbraco.DataTypes.EmailPreview;
using uWebshop.Umbraco.DataTypes.EmailTemplateSelector;
using uWebshop.Umbraco.DataTypes.EnableDisable;
using uWebshop.Umbraco.DataTypes.LanguagePicker;
using uWebshop.Umbraco.DataTypes.MemberGroupPicker;
using uWebshop.Umbraco.DataTypes.OrderCountViewer;
using uWebshop.Umbraco.DataTypes.OrderStatusPicker;
using uWebshop.Umbraco.DataTypes.PaymentProviderAmountType;
using uWebshop.Umbraco.DataTypes.PaymentProviderType;
using uWebshop.Umbraco.DataTypes.Price;
using uWebshop.Umbraco.DataTypes.Ranges;
using uWebshop.Umbraco.DataTypes.RazorWrapper;
using uWebshop.Umbraco.DataTypes.ShippingProviderType;
using uWebshop.Umbraco.DataTypes.ShippingRangeType;
using uWebshop.Umbraco.DataTypes.StockUpdate;
using uWebshop.Umbraco.DataTypes.StorePicker;
using uWebshop.Umbraco.DataTypes.StoreTemplatePicker;
using uWebshop.Umbraco.DataTypes.ZoneSelector;
using umbraco.editorControls;

namespace uWebshop.DataTypes
{
	class Addon : SimpleAddon
	{
		public override string Name()
		{
			return "DataTypeDefinitions";
		}
		public override void DependencyRegistration(IRegistrationControl control)
		{
			control.RegisterType<IDataTypeDefinitions, DataTypes>();
		}
		public override int DependencyRegistrationOrder()
		{
			return base.DependencyRegistrationOrder() + 11;
		}
	}
	class DataTypes : IDataTypeDefinitions
	{
		private readonly List<UwebshopDataTypeDefinition> dataTypes = new List<UwebshopDataTypeDefinition>();

		private void Define(DataType type, string alias, Guid defGuid)
		{
			Define(type, alias, defGuid, null, null);
		}

		private void Define(DataType type, Guid? defGuid, Guid? key, string name, DatabaseType dbType = DatabaseType.Ntext, List<string> preValues = null)
		{
			Define(type, "uWebshop." + type.ToString(), defGuid, key, name, dbType, preValues);
		}

		private void Define(DataType type, string alias, Guid? defGuid, Guid? key, string name, DatabaseType dbType = DatabaseType.Ntext, List<string> preValues = null)
		{
			dataTypes.Add(new UwebshopDataTypeDefinition { DataType = type, Alias = alias, DefinitionGuid = defGuid.HasValue ? defGuid.ToString() : null, KeyGuid = key.HasValue ? key.ToString() : null, Name = name, Type = dbType, PreValues = preValues });
		}


		public List<UwebshopDataTypeDefinition> LoadDataTypeDefinitions()
		{
			Define(DataType.TrueFalse, "Umbraco.TrueFalse", new Guid("92897bc6-a5f3-4ffe-ae27-f2e7e33dda49"));
			Define(DataType.String, "Umbraco.Textbox", new Guid("0cc0eba1-9960-42c9-bf9b-60e150b429ae"));
			Define(DataType.Numeric, "Umbraco.Integer", new Guid("2e6d3631-066e-44b8-aec4-96f09099b2b5"));
			Define(DataType.Tags, "Umbraco.Tags", new Guid("b6b73142-b9c1-4bf8-a16d-e1c23320b549"));
			Define(DataType.Label, "Umbraco.NoEdit", new Guid("f0bc4bfb-b499-40d6-ba86-058885a5178c"));
			Define(DataType.RichText, "Umbraco.TinyMCEv3", new Guid("ca90c950-0aff-4e72-b976-a30b1ac57dad"));
			Define(DataType.TextboxMultiple, "Umbraco.TextboxMultiple", new Guid("c6bac0dd-4ab9-45b1-8e30-e4b619ee5da3"));
			Define(DataType.ContentPicker, "Umbraco.ContentPickerAlias", new Guid("a6857c73-d6e9-480c-b6e6-f15f6ad11125"));
			Define(DataType.MediaPicker, "Umbraco.MediaPicker", new Guid("93929b9a-93a2-4e2a-b239-d99334440a59"));

			Define(DataType.Cultures, LanguagePickerDataType.DefId, LanguagePickerDataType.Key, LanguagePickerDataType.Name, LanguagePickerDataType.DatabaseType);
			Define(DataType.Countries, CountrySelectorDataType.DefId, CountrySelectorDataType.Key, CountrySelectorDataType.Name, CountrySelectorDataType.DatabaseType);
			Define(DataType.DiscountOrderCondition, DiscountOrderConditionDataType.DefId, DiscountOrderConditionDataType.Key, DiscountOrderConditionDataType.Name, DiscountOrderConditionDataType.DatabaseType);
			Define(DataType.DiscountType, DiscountTypeDataType.DefId, DiscountTypeDataType.Key, DiscountTypeDataType.Name, DiscountTypeDataType.DatabaseType);
			Define(DataType.EnableDisable, EnableDisableDataType.DefId, EnableDisableDataType.Key, EnableDisableDataType.Name, EnableDisableDataType.DatabaseType);
			Define(DataType.MemberGroups, MemberGroupPickerDataType.DefId, MemberGroupPickerDataType.Key, MemberGroupPickerDataType.Name, MemberGroupPickerDataType.DatabaseType);
			Define(DataType.CouponCodes, CouponCodeDataType.DefId, CouponCodeDataType.Key, CouponCodeDataType.Name, CouponCodeDataType.DatabaseType);
			Define(DataType.OrderedCount, OrderCountDataType.DefId, OrderCountDataType.Key, OrderCountDataType.Name, OrderCountDataType.DatabaseType);
			Define(DataType.OrderStatusPicker, OrderStatusPickerDataType.DefId, OrderStatusPickerDataType.Key, OrderStatusPickerDataType.Name, OrderStatusPickerDataType.DatabaseType);
			//Define(DataType.OrderSection, OrderStatusSectionDataType.DefId, OrderStatusSectionDataType.Key, OrderStatusSectionDataType.Name, OrderStatusSectionDataType.DatabaseType);
			Define(DataType.PaymentProviderType, PaymentProviderTypeDataType.DefId, PaymentProviderTypeDataType.Key, PaymentProviderTypeDataType.Name, PaymentProviderTypeDataType.DatabaseType);
			Define(DataType.PaymentProviderAmountType, PaymentProviderAmountTypeDataType.DefId, PaymentProviderAmountTypeDataType.Key, PaymentProviderAmountTypeDataType.Name, PaymentProviderAmountTypeDataType.DatabaseType);
			Define(DataType.Price, PriceDataType.DefId, PriceDataType.Key, PriceDataType.Name, PriceDataType.DatabaseType);
			Define(DataType.Ranges, RangesDataType.DefId, RangesDataType.Key, RangesDataType.Name, RangesDataType.DatabaseType);
			Define(DataType.ShippingProviderType, ShippingProviderTypeDataType.DefId, ShippingProviderTypeDataType.Key, ShippingProviderTypeDataType.Name, ShippingProviderTypeDataType.DatabaseType);
			Define(DataType.ShippingProviderRangeType, ShippingRangeTypeDataType.DefId, ShippingRangeTypeDataType.Key, ShippingRangeTypeDataType.Name, ShippingRangeTypeDataType.DatabaseType);
			Define(DataType.Stock, StockUpdateDataType.DefId, StockUpdateDataType.Key, StockUpdateDataType.Name, StockUpdateDataType.DatabaseType);
			Define(DataType.StorePicker, StorePickerDataType.DefId, StorePickerDataType.Key, StorePickerDataType.Name, StorePickerDataType.DatabaseType);
			Define(DataType.StoreTemplatePicker, StoreTemplatePickerDataType.DefId, StoreTemplatePickerDataType.Key, StoreTemplatePickerDataType.Name, StoreTemplatePickerDataType.DatabaseType);
			Define(DataType.TemplatePicker, EmailTemplateSelectorDataType.DefId, EmailTemplateSelectorDataType.Key, EmailTemplateSelectorDataType.Name, EmailTemplateSelectorDataType.DatabaseType);
			Define(DataType.Zones, ZoneSelectorDataType.DefId, ZoneSelectorDataType.Key, ZoneSelectorDataType.Name, ZoneSelectorDataType.DatabaseType);
			Define(DataType.Currencies, CurrenciesDataType.DefId, CurrenciesDataType.Key, CurrenciesDataType.Name, CurrenciesDataType.DatabaseType);

			// todo: Mark deze zijn leeg bij install van pacakge... betere fallback realiseren
			var catagoryRepositoryXPath = "//" + Catalog.CategoryRepositoryNodeAlias;
			if (string.IsNullOrEmpty(Catalog.CategoryRepositoryNodeAlias))
			{
				catagoryRepositoryXPath = "//uwbsCategoryRepository";
			}

			var catalogXPath = "//" + Catalog.NodeAlias;
			if (string.IsNullOrEmpty(Catalog.NodeAlias))
			{
				catalogXPath = "//uwbsCatalog";
			}

			var categoryNodeAlias = Category.NodeAlias;
			if (string.IsNullOrEmpty(categoryNodeAlias))
			{
				categoryNodeAlias = "uwbsCategory";
			}

			var productNodeAlias = Product.NodeAlias;
			if (string.IsNullOrEmpty(productNodeAlias))
			{
				productNodeAlias = "uwbsProduct";
			}

			var paymentZoneNodeAlias = Zone.PaymentZoneNodeAlias;
			if (string.IsNullOrEmpty(paymentZoneNodeAlias))
			{
				paymentZoneNodeAlias = "uwbsPaymentProviderZone";
			}

			var shippingZoneNodeAlias = Zone.ShippingZoneNodeAlias;
			if (string.IsNullOrEmpty(shippingZoneNodeAlias))
			{
				shippingZoneNodeAlias = "uwbsShippingProviderZone";
			}


			var paymentproviderzoneXpath = "//" + PaymentProvider.PaymentProviderZoneSectionNodeAlias;
			if (string.IsNullOrEmpty(PaymentProvider.PaymentProviderZoneSectionNodeAlias))
			{
				paymentproviderzoneXpath = "//uwbsPaymentProviderZoneSection";
			}

			var shippingproviderzoneXpath = "//" + ShippingProvider.ShippingProviderZoneSectionNodeAlias;
			if (string.IsNullOrEmpty(ShippingProvider.ShippingProviderZoneSectionNodeAlias))
			{
				shippingproviderzoneXpath = "//uwbsShippingProviderZoneSection";
			}


            Define(DataType.MultiContentPickerCatalog, new Guid(DataTypeGuids.MultiNodeTreePickerId), new Guid("ea745beb-271c-4542-a5be-5fba2be86f07"), "uWebshop Catalog Picker", DatabaseType.Ntext, GetPrevaluesForMultiNodeTreePicker(catalogXPath));
            Define(DataType.MultiContentPickerCategories, new Guid(DataTypeGuids.MultiNodeTreePickerId), new Guid("c0d85cb4-6e4d-4f10-9107-abdee89b5d7d"), "uWebshop Category Picker", DatabaseType.Ntext, GetPrevaluesForMultiNodeTreePicker(catagoryRepositoryXPath, categoryNodeAlias));
			Define(DataType.MultiContentPickerImages, new Guid(DataTypeGuids.MultiNodeTreePickerId), new Guid("d61cbc2c-87d3-49d9-8e58-87996d22d97f"), "uWebshop Image Picker", DatabaseType.Ntext, GetPrevaluesForImagesFiles());
			Define(DataType.MultiContentPickerFiles, new Guid(DataTypeGuids.MultiNodeTreePickerId), new Guid("83c13f11-95d8-4f10-b510-581983fe8c19"), "uWebshop File Picker", DatabaseType.Ntext, GetPrevaluesForImagesFiles());
			Define(DataType.MultiContentPickerProducts, new Guid(DataTypeGuids.MultiNodeTreePickerId), new Guid("23806a76-6a36-468c-8188-f25308a71cdb"), "uWebshop Product Picker", DatabaseType.Ntext, GetPrevaluesForMultiNodeTreePicker(catalogXPath, productNodeAlias));
			Define(DataType.MultiContentPickerPaymentZones, new Guid(DataTypeGuids.MultiNodeTreePickerId), new Guid("406a4f2d-dd31-43be-9114-8077f43a6151"), "uWebshop Payment Zone Picker", DatabaseType.Ntext, GetPrevaluesForMultiNodeTreePicker(paymentproviderzoneXpath, paymentZoneNodeAlias));
			Define(DataType.MultiContentPickerShippingZones, new Guid(DataTypeGuids.MultiNodeTreePickerId), new Guid("18ee35a7-7931-4d80-822c-ffe2bfb40f6e"), "uWebshop Shipping Zone Picker", DatabaseType.Ntext, GetPrevaluesForMultiNodeTreePicker(shippingproviderzoneXpath, shippingZoneNodeAlias));
            

            
			// uWebshop OrderDetails
			// uWebshop OrderOverview
			// EmailDetails
			// ProductOverview

			Define(DataType.OrderInfoViewer, RazorWrapperDataType.DefId, new Guid("6f455770-3677-4c6c-843d-2c76d7b33893"), "uWebshop OrderDetails", DatabaseType.Ntext, new List<string> { "/uWebshopBackend/uWebshopUmbracoOrderDetails.cshtml" });
			Define(DataType.OrderSection, RazorWrapperDataType.DefId, new Guid("de47d313-9364-472b-8ee7-9b002cc204b9"), "uWebshop OrderOverview", DatabaseType.Ntext, new List<string> { "/uWebshopBackend/uWebshopUmbracoOrderOverview.cshtml" });
			Define(DataType.EmailDetails, EmailPreviewDataType.DefId, new Guid("9d87378c-5864-4ae2-bd83-1cd14b0c3290"), "uWebshop EmailDetails");
			Define(DataType.ProductOverview, RazorWrapperDataType.DefId, new Guid("3873106b-fbb8-4d55-8e91-a07680e796d7"), "uWebshop ProductOverview", DatabaseType.Ntext, new List<string> { "/uWebshopBackend/uWebshopUmbracoProductOverview.cshtml" });


			Define(DataType.MultiNodePicker, new Guid(DataTypeGuids.MultiNodeTreePickerId), new Guid("97600235-acf7-4ade-9ba9-6cad4743cb6d"), "uWebshop Global Picker", DatabaseType.Ntext, GetPrevaluesForMultiNodeTreePicker("/*"));
			//Define(DataType.MultiNodePicker, new Guid(DataTypeGuids.MultiNodeTreePickerId));

			Define(DataType.VatPicker, new Guid("a74ea9c9-8e18-4d2a-8cf6-73c6206c5da6"), new Guid("69d3f953-b565-4269-9d68-4b39e13c70e5"), "uWebshop Vat Picker", DatabaseType.Integer, new List<string> { "0", "6", "15", "19", "21" });

			//<DataType Name="uWebshop Vat Picker" Id="a74ea9c9-8e18-4d2a-8cf6-73c6206c5da6" Definition="69d3f953-b565-4269-9d68-4b39e13c70e5" DatabaseType="Ntext">
			//  <PreValues>
			//	<PreValue Id="54" Value="0" />
			//	<PreValue Id="55" Value="6" />
			//	<PreValue Id="56" Value="15" />
			//	<PreValue Id="57" Value="19" />
			//	<PreValue Id="58" Value="21" />
			//  </PreValues>
			//</DataType>

			return dataTypes;
		}

		private List<string> GetPrevaluesForMultiNodeTreePicker(string startXPath, string nodeTypeAliasFilter = null)
		{
			return new List<string> { "content", string.IsNullOrWhiteSpace(nodeTypeAliasFilter) ? "*" : "/*[starts-with(name(),'" + nodeTypeAliasFilter + "')]", "-1", "False", "1", "1", "0", "False", "1", "0", startXPath, "200", "0", };
		}

		private List<string> GetPrevaluesForImagesFiles()
		{
			return new List<string> { "media", "*", "-1", "False", "1", "1", "-1", "False", "0", "0", "/", "200", "0", };
		}
	}

	public enum DataTypez
	{
		VatPicker,

		TrueFalse,
		String,
		Numeric,
		ContentPicker,
		Tags,
		MultiNodePicker,
		Label,
		RichText,
		MediaPicker,
		TextboxMultiple,

		MultiContentPickerCategories,
		MultiContentPickerImages,
		MultiContentPickerFiles,
		MultiContentPickerProducts,
		MultiContentPickerPaymentZones,

		Price,
		OrderedCount,
		EnableDisable,
		Ranges,
		Stock,
		DiscountType,
		MemberGroups,
		DiscountOrderCondition,
		XSLTTemplatePicker,
		PaymentProviderType,
		Zones,
		ShippingProviderType,
		ShippingProviderRangeType,
		OrderSection,
		OrderStatusPicker,
		Cultures,
		Countries
	}

	//<PreValues>
	//  <PreValue Id="10" Value="content" />
	//  <PreValue Id="11" Value="/*[self::uwbsCategory]" />
	//  <PreValue Id="12" Value="-1" />
	//  <PreValue Id="13" Value="False" />
	//  <PreValue Id="14" Value="1" />
	//  <PreValue Id="15" Value="1" />
	//  <PreValue Id="16" Value="0" />
	//  <PreValue Id="17" Value="False" />
	//  <PreValue Id="18" Value="1" />
	//  <PreValue Id="19" Value="0" />
	//  <PreValue Id="20" Value="//uwbsCategoryRepository" />
	//  <PreValue Id="21" Value="200" />
	//  <PreValue Id="22" Value="0" />
	//</PreValues>


	//	if (CulturesDataType == null)
	//{
	//	CulturesDataType = new DataTypeDefinition(-1, new Guid("24a53e42-4bb4-4c9a-a16c-3651e3083f30"));
	//	CulturesDataType.Name = "uWebshop Language Picker";
	//	CulturesDataType.Key = new Guid("4235f880-64cc-4d78-8fc2-6e6e5ee72010");
	//	CulturesDataType.DatabaseType = DataTypeDatabaseType.Ntext;

	//	newDataTypesList.Add(CulturesDataType);
	//		//}
	//public enum DataType
	//{
	//	[DataTypeDefinition(DefinitionGuid = "92897bc6-a5f3-4ffe-ae27-f2e7e33dda49")]
	//	TrueFalse,
	//	String,
	//	[DataTypeDefinition(DefinitionGuid = "24a53e42-4bb4-4c9a-a16c-3651e3083f30")] // reference issue => move naar Umbraco[6] project
	//}
	//(-90, 0, -1, 0, 11, '-1,-90', 35, '84c6b441-31df-4ffe-b67e-67d5bc3ae65a', 'Upload', '30a2a501-1978-4ddb-a57b-f7efed43ba3c', '2004/09/30 14:01:49.920'),
	//(-43, 0, -1, 0, 1, '-1,-43', 2, 'fbaf13a8-4036-41f2-93a3-974f678c312a', 'Checkbox list', '30a2a501-1978-4ddb-a57b-f7efed43ba3c', '2004/10/15 14:11:04.367'),
	//(-42, 0, -1, 0, 1, '-1,-42', 2, '0b6a45e7-44ba-430d-9da5-4e46060b9e03', 'Dropdow', '30a2a501-1978-4ddb-a57b-f7efed43ba3c', '2004/10/15 14:10:59.000'),
	//(-41, 0, -1, 0, 1, '-1,-41', 2, '5046194e-4237-453c-a547-15db3a07c4e1', 'Date Picker', '30a2a501-1978-4ddb-a57b-f7efed43ba3c', '2004/10/15 14:10:54.303'),
	//(-40, 0, -1, 0, 1, '-1,-40', 2, 'bb5f57c9-ce2b-4bb9-b697-4caca783a805', 'Radiobox', '30a2a501-1978-4ddb-a57b-f7efed43ba3c', '2004/10/15 14:10:49.253'),
	//(-39, 0, -1, 0, 1, '-1,-39', 2, 'f38f0ac7-1d27-439c-9f3f-089cd8825a53', 'Dropdown multiple', '30a2a501-1978-4ddb-a57b-f7efed43ba3c', '2004/10/15 14:10:44.480'),
	//(-38, 0, -1, 0, 1, '-1,-38', 2, 'fd9f1447-6c61-4a7c-9595-5aa39147d318', 'Folder Browser', '30a2a501-1978-4ddb-a57b-f7efed43ba3c', '2004/10/15 14:10:37.020'),
	//(-37, 0, -1, 0, 1, '-1,-37', 2, '0225af17-b302-49cb-9176-b9f35cab9c17', 'Approved Color', '30a2a501-1978-4ddb-a57b-f7efed43ba3c', '2004/10/15 14:10:30.580'),
	//(-36, 0, -1, 0, 1, '-1,-36', 2, 'e4d66c0f-b935-4200-81f0-025f7256b89a', 'Date Picker with time', '30a2a501-1978-4ddb-a57b-f7efed43ba3c', '2004/10/15 14:10:23.007'),
	//(1031, 0, -1, 1, 1, '-1,1031', 2, 'f38bd2d7-65d0-48e6-95dc-87ce06ec2d3d', 'Folder', '4ea4382b-2f5a-4c2b-9587-ae9b3cf3602e', '2004/12/01 00:13:40.743'),
	//(1032, 0, -1, 1, 1, '-1,1032', 2, 'cc07b313-0843-4aa8-bbda-871c8da728c8', 'Image', '4ea4382b-2f5a-4c2b-9587-ae9b3cf3602e', '2004/12/01 00:13:43.737'),
	//(1033, 0, -1, 1, 1, '-1,1033', 2, '4c52d8ab-54e6-40cd-999c-7a5f24903e4d', 'File', '4ea4382b-2f5a-4c2b-9587-ae9b3cf3602e', '2004/12/01 00:13:46.210'),
	//(1036, 0, -1, 0, 1, '-1,1036', 2, '2b24165f-9782-4aa3-b459-1de4a4d21f60', 'Member Picker', '30a2a501-1978-4ddb-a57b-f7efed43ba3c', '2006/01/03 13:07:40.260'),
	//(1038, 0, -1, 0, 1, '-1,1038', 2, '1251c96c-185c-4e9b-93f4-b48205573cbd', 'Simple Editor', '30a2a501-1978-4ddb-a57b-f7efed43ba3c', '2006/01/03 13:07:55.250'),
	//(1039, 0, -1, 0, 1, '-1,1039', 2, '06f349a9-c949-4b6a-8660-59c10451af42', 'Ultimate Picker', '30a2a501-1978-4ddb-a57b-f7efed43ba3c', '2006/01/03 13:07:55.250'),
	//(1040, 0, -1, 0, 1, '-1,1040', 2, '21e798da-e06e-4eda-a511-ed257f78d4fa', 'Related Links', '30a2a501-1978-4ddb-a57b-f7efed43ba3c', '2006/01/03 13:07:55.250'),
	//(1042, 0, -1, 0, 1, '-1,1042', 2, '0a452bd5-83f9-4bc3-8403-1286e13fb77e', 'Macro Container', '30a2a501-1978-4ddb-a57b-f7efed43ba3c', '2006/01/03 13:07:55.250'),
	//(1043, 0, -1, 0, 1, '-1,1042', 2, '1df9f033-e6d4-451f-b8d2-e0cbc50a836f', 'Image Cropper', '30a2a501-1978-4ddb-a57b-f7efed43ba3c', '2006/01/03 13:07:55.250')
}