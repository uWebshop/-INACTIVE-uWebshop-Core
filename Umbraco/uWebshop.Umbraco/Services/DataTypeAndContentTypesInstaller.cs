using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Models;
using uWebshop.Umbraco.Interfaces;
using Umbraco.Core.Configuration;

namespace uWebshop.Umbraco6
{
	internal partial class CMSInstaller
	{
		partial void InstallGenerated(IUmbracoVersion umbracoVersion)
		{
var contentTypeService = ApplicationContext.Current.Services.ContentTypeService;
var dataTypeService = ApplicationContext.Current.Services.DataTypeService;
var contentTypeList = new List<IContentType>();
var newDataTypesList = new List<IDataTypeDefinition>();
var umbracoVersionMajor = UmbracoVersion.Current.Major;
var TrueFalseDataTypeDef = umbracoVersion.GetDataTypeDefinition("Umbraco.TrueFalse", new Guid("92897bc6-a5f3-4ffe-ae27-f2e7e33dda49"));
if (TrueFalseDataTypeDef == null) throw new Exception("Could not load default umbraco TrueFalse datatype");
var StringDataTypeDef = umbracoVersion.GetDataTypeDefinition("Umbraco.Textbox", new Guid("0cc0eba1-9960-42c9-bf9b-60e150b429ae"));
if (StringDataTypeDef == null) throw new Exception("Could not load default umbraco String datatype");
var NumericDataTypeDef = umbracoVersion.GetDataTypeDefinition("Umbraco.Integer", new Guid("2e6d3631-066e-44b8-aec4-96f09099b2b5"));
if (NumericDataTypeDef == null) throw new Exception("Could not load default umbraco Numeric datatype");
var TagsDataTypeDef = umbracoVersion.GetDataTypeDefinition("Umbraco.Tags", new Guid("b6b73142-b9c1-4bf8-a16d-e1c23320b549"));
if (TagsDataTypeDef == null) throw new Exception("Could not load default umbraco Tags datatype");
var LabelDataTypeDef = umbracoVersion.GetDataTypeDefinition("Umbraco.NoEdit", new Guid("f0bc4bfb-b499-40d6-ba86-058885a5178c"));
if (LabelDataTypeDef == null) throw new Exception("Could not load default umbraco Label datatype");
var RichTextDataTypeDef = umbracoVersion.GetDataTypeDefinition("Umbraco.TinyMCEv3", new Guid("ca90c950-0aff-4e72-b976-a30b1ac57dad"));
if (RichTextDataTypeDef == null) throw new Exception("Could not load default umbraco RichText datatype");
var TextboxMultipleDataTypeDef = umbracoVersion.GetDataTypeDefinition("Umbraco.TextboxMultiple", new Guid("c6bac0dd-4ab9-45b1-8e30-e4b619ee5da3"));
if (TextboxMultipleDataTypeDef == null) throw new Exception("Could not load default umbraco TextboxMultiple datatype");
var ContentPickerDataTypeDef = umbracoVersion.GetDataTypeDefinition("Umbraco.ContentPickerAlias", new Guid("a6857c73-d6e9-480c-b6e6-f15f6ad11125"));
if (ContentPickerDataTypeDef == null) throw new Exception("Could not load default umbraco ContentPicker datatype");
var MediaPickerDataTypeDef = umbracoVersion.GetDataTypeDefinition("Umbraco.MediaPicker", new Guid("93929b9a-93a2-4e2a-b239-d99334440a59"));
if (MediaPickerDataTypeDef == null) throw new Exception("Could not load default umbraco MediaPicker datatype");
var EnableDisableDataTypeDef = umbracoVersion.GetDataTypeDefinition("uWebshop.EnableDisable", new Guid("63c6fa9a-975f-4474-9155-62a229bafaef"));
			if (EnableDisableDataTypeDef == null)
			{				
				EnableDisableDataTypeDef = umbracoVersion.CreateDataTypeDefinition(-1, "uWebshop.EnableDisable", new Guid("6e62bca5-8d73-435e-8a26-78ef0e7b6c58"));
				EnableDisableDataTypeDef.Name = "Enable/Disable";
				EnableDisableDataTypeDef.Key = new Guid("63c6fa9a-975f-4474-9155-62a229bafaef");
				EnableDisableDataTypeDef.DatabaseType = DataTypeDatabaseType.Ntext;
				
				newDataTypesList.Add(EnableDisableDataTypeDef);
			}
			var MemberGroupsDataTypeDef = umbracoVersion.GetDataTypeDefinition("uWebshop.MemberGroups", new Guid("a99d5614-8b33-4a63-891a-a254c87af481"));
			if (MemberGroupsDataTypeDef == null)
			{				
				MemberGroupsDataTypeDef = umbracoVersion.CreateDataTypeDefinition(-1, "uWebshop.MemberGroups", new Guid("bd6054bc-86ec-4f2c-bcc8-447766f1d569"));
				MemberGroupsDataTypeDef.Name = "MemberGroup Picker";
				MemberGroupsDataTypeDef.Key = new Guid("a99d5614-8b33-4a63-891a-a254c87af481");
				MemberGroupsDataTypeDef.DatabaseType = DataTypeDatabaseType.Ntext;
				
				newDataTypesList.Add(MemberGroupsDataTypeDef);
			}
			var MultiContentPickerCatalogDataTypeDef = umbracoVersion.GetDataTypeDefinition("uWebshop.MultiContentPickerCatalog", new Guid("ea745beb-271c-4542-a5be-5fba2be86f07"));
			if (MultiContentPickerCatalogDataTypeDef == null)
			{				
				MultiContentPickerCatalogDataTypeDef = umbracoVersion.CreateDataTypeDefinition(-1, "uWebshop.MultiContentPickerCatalog", new Guid("7e062c13-7c41-4ad9-b389-41d88aeef87c"));
				MultiContentPickerCatalogDataTypeDef.Name = "uWebshop Catalog Picker";
				MultiContentPickerCatalogDataTypeDef.Key = new Guid("ea745beb-271c-4542-a5be-5fba2be86f07");
				MultiContentPickerCatalogDataTypeDef.DatabaseType = DataTypeDatabaseType.Ntext;
				
				newDataTypesList.Add(MultiContentPickerCatalogDataTypeDef);
			}
			var MultiContentPickerCategoriesDataTypeDef = umbracoVersion.GetDataTypeDefinition("uWebshop.MultiContentPickerCategories", new Guid("c0d85cb4-6e4d-4f10-9107-abdee89b5d7d"));
			if (MultiContentPickerCategoriesDataTypeDef == null)
			{				
				MultiContentPickerCategoriesDataTypeDef = umbracoVersion.CreateDataTypeDefinition(-1, "uWebshop.MultiContentPickerCategories", new Guid("7e062c13-7c41-4ad9-b389-41d88aeef87c"));
				MultiContentPickerCategoriesDataTypeDef.Name = "uWebshop Category Picker";
				MultiContentPickerCategoriesDataTypeDef.Key = new Guid("c0d85cb4-6e4d-4f10-9107-abdee89b5d7d");
				MultiContentPickerCategoriesDataTypeDef.DatabaseType = DataTypeDatabaseType.Ntext;
				
				newDataTypesList.Add(MultiContentPickerCategoriesDataTypeDef);
			}
			var CountriesDataTypeDef = umbracoVersion.GetDataTypeDefinition("uWebshop.Countries", new Guid("a6d19ee8-ab93-42d6-a61a-8d4aaa759207"));
			if (CountriesDataTypeDef == null)
			{				
				CountriesDataTypeDef = umbracoVersion.CreateDataTypeDefinition(-1, "uWebshop.Countries", new Guid("cdf9f220-ea26-4f37-9505-4b65dfdd59e8"));
				CountriesDataTypeDef.Name = "uWebshop Country Selector";
				CountriesDataTypeDef.Key = new Guid("a6d19ee8-ab93-42d6-a61a-8d4aaa759207");
				CountriesDataTypeDef.DatabaseType = DataTypeDatabaseType.Ntext;
				
				newDataTypesList.Add(CountriesDataTypeDef);
			}
			var CouponCodesDataTypeDef = umbracoVersion.GetDataTypeDefinition("uWebshop.CouponCodes", new Guid("0dbc0113-a084-44d1-8fef-f8ef0cd8453b"));
			if (CouponCodesDataTypeDef == null)
			{				
				CouponCodesDataTypeDef = umbracoVersion.CreateDataTypeDefinition(-1, "uWebshop.CouponCodes", new Guid("4db2a111-36e1-4f96-a021-fd98be96aeaf"));
				CouponCodesDataTypeDef.Name = "uWebshop Couponcode Editor";
				CouponCodesDataTypeDef.Key = new Guid("0dbc0113-a084-44d1-8fef-f8ef0cd8453b");
				CouponCodesDataTypeDef.DatabaseType = DataTypeDatabaseType.Ntext;
				
				newDataTypesList.Add(CouponCodesDataTypeDef);
			}
			var CurrenciesDataTypeDef = umbracoVersion.GetDataTypeDefinition("uWebshop.Currencies", new Guid("7e6cab81-528d-4b00-8321-72c36f131eea"));
			if (CurrenciesDataTypeDef == null)
			{				
				CurrenciesDataTypeDef = umbracoVersion.CreateDataTypeDefinition(-1, "uWebshop.Currencies", new Guid("9dbebe55-72b2-4573-b16c-9bfe3c4d24f6"));
				CurrenciesDataTypeDef.Name = "uWebshop Currencies";
				CurrenciesDataTypeDef.Key = new Guid("7e6cab81-528d-4b00-8321-72c36f131eea");
				CurrenciesDataTypeDef.DatabaseType = DataTypeDatabaseType.Ntext;
				
				newDataTypesList.Add(CurrenciesDataTypeDef);
			}
			var DiscountOrderConditionDataTypeDef = umbracoVersion.GetDataTypeDefinition("uWebshop.DiscountOrderCondition", new Guid("e159f935-d7f4-4277-b5d0-adf74b003849"));
			if (DiscountOrderConditionDataTypeDef == null)
			{				
				DiscountOrderConditionDataTypeDef = umbracoVersion.CreateDataTypeDefinition(-1, "uWebshop.DiscountOrderCondition", new Guid("2b946d78-256c-48f6-98f5-435ac946a220"));
				DiscountOrderConditionDataTypeDef.Name = "uWebshop Discount Condition Picker";
				DiscountOrderConditionDataTypeDef.Key = new Guid("e159f935-d7f4-4277-b5d0-adf74b003849");
				DiscountOrderConditionDataTypeDef.DatabaseType = DataTypeDatabaseType.Ntext;
				
				newDataTypesList.Add(DiscountOrderConditionDataTypeDef);
			}
			var DiscountTypeDataTypeDef = umbracoVersion.GetDataTypeDefinition("uWebshop.DiscountType", new Guid("2d89188e-33a7-4885-a6d0-1caef40320a7"));
			if (DiscountTypeDataTypeDef == null)
			{				
				DiscountTypeDataTypeDef = umbracoVersion.CreateDataTypeDefinition(-1, "uWebshop.DiscountType", new Guid("c66b5dea-00f7-49df-bc5a-074e5802bce2"));
				DiscountTypeDataTypeDef.Name = "uWebshop Discount Picker";
				DiscountTypeDataTypeDef.Key = new Guid("2d89188e-33a7-4885-a6d0-1caef40320a7");
				DiscountTypeDataTypeDef.DatabaseType = DataTypeDatabaseType.Ntext;
				
				newDataTypesList.Add(DiscountTypeDataTypeDef);
			}
			var TemplatePickerDataTypeDef = umbracoVersion.GetDataTypeDefinition("uWebshop.TemplatePicker", new Guid("0a10d9dd-ebbc-48f5-be93-9fac239ac876"));
			if (TemplatePickerDataTypeDef == null)
			{				
				TemplatePickerDataTypeDef = umbracoVersion.CreateDataTypeDefinition(-1, "uWebshop.TemplatePicker", new Guid("20e19862-f52d-4c56-88c3-244238eafffc"));
				TemplatePickerDataTypeDef.Name = "uWebshop Email Template Picker";
				TemplatePickerDataTypeDef.Key = new Guid("0a10d9dd-ebbc-48f5-be93-9fac239ac876");
				TemplatePickerDataTypeDef.DatabaseType = DataTypeDatabaseType.Nvarchar;
				
				newDataTypesList.Add(TemplatePickerDataTypeDef);
			}
			var EmailDetailsDataTypeDef = umbracoVersion.GetDataTypeDefinition("uWebshop.EmailDetails", new Guid("9d87378c-5864-4ae2-bd83-1cd14b0c3290"));
			if (EmailDetailsDataTypeDef == null)
			{				
				EmailDetailsDataTypeDef = umbracoVersion.CreateDataTypeDefinition(-1, "uWebshop.EmailDetails", new Guid("77345447-e3b4-4891-907f-bc0e0d5831a7"));
				EmailDetailsDataTypeDef.Name = "uWebshop EmailDetails";
				EmailDetailsDataTypeDef.Key = new Guid("9d87378c-5864-4ae2-bd83-1cd14b0c3290");
				EmailDetailsDataTypeDef.DatabaseType = DataTypeDatabaseType.Ntext;
				
				newDataTypesList.Add(EmailDetailsDataTypeDef);
			}
			var MultiContentPickerFilesDataTypeDef = umbracoVersion.GetDataTypeDefinition("uWebshop.MultiContentPickerFiles", new Guid("83c13f11-95d8-4f10-b510-581983fe8c19"));
			if (MultiContentPickerFilesDataTypeDef == null)
			{				
				MultiContentPickerFilesDataTypeDef = umbracoVersion.CreateDataTypeDefinition(-1, "uWebshop.MultiContentPickerFiles", new Guid("7e062c13-7c41-4ad9-b389-41d88aeef87c"));
				MultiContentPickerFilesDataTypeDef.Name = "uWebshop File Picker";
				MultiContentPickerFilesDataTypeDef.Key = new Guid("83c13f11-95d8-4f10-b510-581983fe8c19");
				MultiContentPickerFilesDataTypeDef.DatabaseType = DataTypeDatabaseType.Ntext;
				
				newDataTypesList.Add(MultiContentPickerFilesDataTypeDef);
			}
			var MultiNodePickerDataTypeDef = umbracoVersion.GetDataTypeDefinition("uWebshop.MultiNodePicker", new Guid("97600235-acf7-4ade-9ba9-6cad4743cb6d"));
			if (MultiNodePickerDataTypeDef == null)
			{				
				MultiNodePickerDataTypeDef = umbracoVersion.CreateDataTypeDefinition(-1, "uWebshop.MultiNodePicker", new Guid("7e062c13-7c41-4ad9-b389-41d88aeef87c"));
				MultiNodePickerDataTypeDef.Name = "uWebshop Global Picker";
				MultiNodePickerDataTypeDef.Key = new Guid("97600235-acf7-4ade-9ba9-6cad4743cb6d");
				MultiNodePickerDataTypeDef.DatabaseType = DataTypeDatabaseType.Ntext;
				
				newDataTypesList.Add(MultiNodePickerDataTypeDef);
			}
			var MultiContentPickerImagesDataTypeDef = umbracoVersion.GetDataTypeDefinition("uWebshop.MultiContentPickerImages", new Guid("d61cbc2c-87d3-49d9-8e58-87996d22d97f"));
			if (MultiContentPickerImagesDataTypeDef == null)
			{				
				MultiContentPickerImagesDataTypeDef = umbracoVersion.CreateDataTypeDefinition(-1, "uWebshop.MultiContentPickerImages", new Guid("7e062c13-7c41-4ad9-b389-41d88aeef87c"));
				MultiContentPickerImagesDataTypeDef.Name = "uWebshop Image Picker";
				MultiContentPickerImagesDataTypeDef.Key = new Guid("d61cbc2c-87d3-49d9-8e58-87996d22d97f");
				MultiContentPickerImagesDataTypeDef.DatabaseType = DataTypeDatabaseType.Ntext;
				
				newDataTypesList.Add(MultiContentPickerImagesDataTypeDef);
			}
			var OrderedCountDataTypeDef = umbracoVersion.GetDataTypeDefinition("uWebshop.OrderedCount", new Guid("2f6e0aa8-8291-4544-8aeb-db78cfb42b07"));
			if (OrderedCountDataTypeDef == null)
			{				
				OrderedCountDataTypeDef = umbracoVersion.CreateDataTypeDefinition(-1, "uWebshop.OrderedCount", new Guid("1779c16d-1b5d-4312-974b-782590caca3e"));
				OrderedCountDataTypeDef.Name = "uWebshop Item Ordered Count";
				OrderedCountDataTypeDef.Key = new Guid("2f6e0aa8-8291-4544-8aeb-db78cfb42b07");
				OrderedCountDataTypeDef.DatabaseType = DataTypeDatabaseType.Integer;
				
				newDataTypesList.Add(OrderedCountDataTypeDef);
			}
			var CulturesDataTypeDef = umbracoVersion.GetDataTypeDefinition("uWebshop.Cultures", new Guid("4235f880-64cc-4d78-8fc2-6e6e5ee72010"));
			if (CulturesDataTypeDef == null)
			{				
				CulturesDataTypeDef = umbracoVersion.CreateDataTypeDefinition(-1, "uWebshop.Cultures", new Guid("24a53e42-4bb4-4c9a-a16c-3651e3083f30"));
				CulturesDataTypeDef.Name = "uWebshop Language Picker";
				CulturesDataTypeDef.Key = new Guid("4235f880-64cc-4d78-8fc2-6e6e5ee72010");
				CulturesDataTypeDef.DatabaseType = DataTypeDatabaseType.Ntext;
				
				newDataTypesList.Add(CulturesDataTypeDef);
			}
			var OrderStatusPickerDataTypeDef = umbracoVersion.GetDataTypeDefinition("uWebshop.OrderStatusPicker", new Guid("8fa38a1b-7854-43ce-b03e-c7cc2ffd3d20"));
			if (OrderStatusPickerDataTypeDef == null)
			{				
				OrderStatusPickerDataTypeDef = umbracoVersion.CreateDataTypeDefinition(-1, "uWebshop.OrderStatusPicker", new Guid("f1ad9252-933b-4c44-8496-b24318dac2ed"));
				OrderStatusPickerDataTypeDef.Name = "uWebshop Order Status Picker";
				OrderStatusPickerDataTypeDef.Key = new Guid("8fa38a1b-7854-43ce-b03e-c7cc2ffd3d20");
				OrderStatusPickerDataTypeDef.DatabaseType = DataTypeDatabaseType.Ntext;
				
				newDataTypesList.Add(OrderStatusPickerDataTypeDef);
			}
			var OrderInfoViewerDataTypeDef = umbracoVersion.GetDataTypeDefinition("uWebshop.OrderInfoViewer", new Guid("6f455770-3677-4c6c-843d-2c76d7b33893"));
			if (OrderInfoViewerDataTypeDef == null)
			{				
				OrderInfoViewerDataTypeDef = umbracoVersion.CreateDataTypeDefinition(-1, "uWebshop.OrderInfoViewer", new Guid("3c148cf9-de63-4bc6-860e-81fe9546fa61"));
				OrderInfoViewerDataTypeDef.Name = "uWebshop OrderDetails";
				OrderInfoViewerDataTypeDef.Key = new Guid("6f455770-3677-4c6c-843d-2c76d7b33893");
				OrderInfoViewerDataTypeDef.DatabaseType = DataTypeDatabaseType.Ntext;
				
				newDataTypesList.Add(OrderInfoViewerDataTypeDef);
			}
			var OrderSectionDataTypeDef = umbracoVersion.GetDataTypeDefinition("uWebshop.OrderSection", new Guid("de47d313-9364-472b-8ee7-9b002cc204b9"));
			if (OrderSectionDataTypeDef == null)
			{				
				OrderSectionDataTypeDef = umbracoVersion.CreateDataTypeDefinition(-1, "uWebshop.OrderSection", new Guid("3c148cf9-de63-4bc6-860e-81fe9546fa61"));
				OrderSectionDataTypeDef.Name = "uWebshop OrderOverview";
				OrderSectionDataTypeDef.Key = new Guid("de47d313-9364-472b-8ee7-9b002cc204b9");
				OrderSectionDataTypeDef.DatabaseType = DataTypeDatabaseType.Ntext;
				
				newDataTypesList.Add(OrderSectionDataTypeDef);
			}
			var PaymentProviderAmountTypeDataTypeDef = umbracoVersion.GetDataTypeDefinition("uWebshop.PaymentProviderAmountType", new Guid("fb79c76e-8ccc-406d-a3ad-39c1f939a38d"));
			if (PaymentProviderAmountTypeDataTypeDef == null)
			{				
				PaymentProviderAmountTypeDataTypeDef = umbracoVersion.CreateDataTypeDefinition(-1, "uWebshop.PaymentProviderAmountType", new Guid("85c8f557-cf82-4450-856f-79492a02f55d"));
				PaymentProviderAmountTypeDataTypeDef.Name = "uWebshop Payment Provider Amount Type";
				PaymentProviderAmountTypeDataTypeDef.Key = new Guid("fb79c76e-8ccc-406d-a3ad-39c1f939a38d");
				PaymentProviderAmountTypeDataTypeDef.DatabaseType = DataTypeDatabaseType.Ntext;
				
				newDataTypesList.Add(PaymentProviderAmountTypeDataTypeDef);
			}
			var PaymentProviderTypeDataTypeDef = umbracoVersion.GetDataTypeDefinition("uWebshop.PaymentProviderType", new Guid("bbaf3c0e-fc22-4d33-b884-7b87d8dc3c8c"));
			if (PaymentProviderTypeDataTypeDef == null)
			{				
				PaymentProviderTypeDataTypeDef = umbracoVersion.CreateDataTypeDefinition(-1, "uWebshop.PaymentProviderType", new Guid("e2e9e8da-f7dc-43f4-806c-8a27fa738971"));
				PaymentProviderTypeDataTypeDef.Name = "uWebshop Payment Provider Type";
				PaymentProviderTypeDataTypeDef.Key = new Guid("bbaf3c0e-fc22-4d33-b884-7b87d8dc3c8c");
				PaymentProviderTypeDataTypeDef.DatabaseType = DataTypeDatabaseType.Ntext;
				
				newDataTypesList.Add(PaymentProviderTypeDataTypeDef);
			}
			var MultiContentPickerPaymentZonesDataTypeDef = umbracoVersion.GetDataTypeDefinition("uWebshop.MultiContentPickerPaymentZones", new Guid("406a4f2d-dd31-43be-9114-8077f43a6151"));
			if (MultiContentPickerPaymentZonesDataTypeDef == null)
			{				
				MultiContentPickerPaymentZonesDataTypeDef = umbracoVersion.CreateDataTypeDefinition(-1, "uWebshop.MultiContentPickerPaymentZones", new Guid("7e062c13-7c41-4ad9-b389-41d88aeef87c"));
				MultiContentPickerPaymentZonesDataTypeDef.Name = "uWebshop Payment Zone Picker";
				MultiContentPickerPaymentZonesDataTypeDef.Key = new Guid("406a4f2d-dd31-43be-9114-8077f43a6151");
				MultiContentPickerPaymentZonesDataTypeDef.DatabaseType = DataTypeDatabaseType.Ntext;
				
				newDataTypesList.Add(MultiContentPickerPaymentZonesDataTypeDef);
			}
			var PriceDataTypeDef = umbracoVersion.GetDataTypeDefinition("uWebshop.Price", new Guid("40e2736a-8ecd-41b3-bb27-7d11909f0a21"));
			if (PriceDataTypeDef == null)
			{				
				PriceDataTypeDef = umbracoVersion.CreateDataTypeDefinition(-1, "uWebshop.Price", new Guid("efe95454-4982-4bd8-a5dc-3fdfa24e5b5b"));
				PriceDataTypeDef.Name = "uWebshop Price Editor";
				PriceDataTypeDef.Key = new Guid("40e2736a-8ecd-41b3-bb27-7d11909f0a21");
				PriceDataTypeDef.DatabaseType = DataTypeDatabaseType.Ntext;
				
				newDataTypesList.Add(PriceDataTypeDef);
			}
			var MultiContentPickerProductsDataTypeDef = umbracoVersion.GetDataTypeDefinition("uWebshop.MultiContentPickerProducts", new Guid("23806a76-6a36-468c-8188-f25308a71cdb"));
			if (MultiContentPickerProductsDataTypeDef == null)
			{				
				MultiContentPickerProductsDataTypeDef = umbracoVersion.CreateDataTypeDefinition(-1, "uWebshop.MultiContentPickerProducts", new Guid("7e062c13-7c41-4ad9-b389-41d88aeef87c"));
				MultiContentPickerProductsDataTypeDef.Name = "uWebshop Product Picker";
				MultiContentPickerProductsDataTypeDef.Key = new Guid("23806a76-6a36-468c-8188-f25308a71cdb");
				MultiContentPickerProductsDataTypeDef.DatabaseType = DataTypeDatabaseType.Ntext;
				
				newDataTypesList.Add(MultiContentPickerProductsDataTypeDef);
			}
			var ProductOverviewDataTypeDef = umbracoVersion.GetDataTypeDefinition("uWebshop.ProductOverview", new Guid("3873106b-fbb8-4d55-8e91-a07680e796d7"));
			if (ProductOverviewDataTypeDef == null)
			{				
				ProductOverviewDataTypeDef = umbracoVersion.CreateDataTypeDefinition(-1, "uWebshop.ProductOverview", new Guid("3c148cf9-de63-4bc6-860e-81fe9546fa61"));
				ProductOverviewDataTypeDef.Name = "uWebshop ProductOverview";
				ProductOverviewDataTypeDef.Key = new Guid("3873106b-fbb8-4d55-8e91-a07680e796d7");
				ProductOverviewDataTypeDef.DatabaseType = DataTypeDatabaseType.Ntext;
				
				newDataTypesList.Add(ProductOverviewDataTypeDef);
			}
			var RangesDataTypeDef = umbracoVersion.GetDataTypeDefinition("uWebshop.Ranges", new Guid("09dfbeec-2681-42af-b07c-0ed56a575d48"));
			if (RangesDataTypeDef == null)
			{				
				RangesDataTypeDef = umbracoVersion.CreateDataTypeDefinition(-1, "uWebshop.Ranges", new Guid("d5065baa-bb03-437f-9b25-46aa363e1b1c"));
				RangesDataTypeDef.Name = "uWebshop Range Selector";
				RangesDataTypeDef.Key = new Guid("09dfbeec-2681-42af-b07c-0ed56a575d48");
				RangesDataTypeDef.DatabaseType = DataTypeDatabaseType.Ntext;
				
				newDataTypesList.Add(RangesDataTypeDef);
			}
			var ShippingProviderTypeDataTypeDef = umbracoVersion.GetDataTypeDefinition("uWebshop.ShippingProviderType", new Guid("c035ada7-5413-48de-8e62-b4b61e2e934f"));
			if (ShippingProviderTypeDataTypeDef == null)
			{				
				ShippingProviderTypeDataTypeDef = umbracoVersion.CreateDataTypeDefinition(-1, "uWebshop.ShippingProviderType", new Guid("2cdfc385-15dd-4165-84d9-b5e4b2aca970"));
				ShippingProviderTypeDataTypeDef.Name = "uWebshop Shipping Provider Type";
				ShippingProviderTypeDataTypeDef.Key = new Guid("c035ada7-5413-48de-8e62-b4b61e2e934f");
				ShippingProviderTypeDataTypeDef.DatabaseType = DataTypeDatabaseType.Ntext;
				
				newDataTypesList.Add(ShippingProviderTypeDataTypeDef);
			}
			var ShippingProviderRangeTypeDataTypeDef = umbracoVersion.GetDataTypeDefinition("uWebshop.ShippingProviderRangeType", new Guid("34c70cbf-ee1b-465e-941f-ddcd097f7912"));
			if (ShippingProviderRangeTypeDataTypeDef == null)
			{				
				ShippingProviderRangeTypeDataTypeDef = umbracoVersion.CreateDataTypeDefinition(-1, "uWebshop.ShippingProviderRangeType", new Guid("c0737154-5d14-4820-bcb1-98ed82363f7c"));
				ShippingProviderRangeTypeDataTypeDef.Name = "uWebshop Shipping Range Type";
				ShippingProviderRangeTypeDataTypeDef.Key = new Guid("34c70cbf-ee1b-465e-941f-ddcd097f7912");
				ShippingProviderRangeTypeDataTypeDef.DatabaseType = DataTypeDatabaseType.Ntext;
				
				newDataTypesList.Add(ShippingProviderRangeTypeDataTypeDef);
			}
			var MultiContentPickerShippingZonesDataTypeDef = umbracoVersion.GetDataTypeDefinition("uWebshop.MultiContentPickerShippingZones", new Guid("18ee35a7-7931-4d80-822c-ffe2bfb40f6e"));
			if (MultiContentPickerShippingZonesDataTypeDef == null)
			{				
				MultiContentPickerShippingZonesDataTypeDef = umbracoVersion.CreateDataTypeDefinition(-1, "uWebshop.MultiContentPickerShippingZones", new Guid("7e062c13-7c41-4ad9-b389-41d88aeef87c"));
				MultiContentPickerShippingZonesDataTypeDef.Name = "uWebshop Shipping Zone Picker";
				MultiContentPickerShippingZonesDataTypeDef.Key = new Guid("18ee35a7-7931-4d80-822c-ffe2bfb40f6e");
				MultiContentPickerShippingZonesDataTypeDef.DatabaseType = DataTypeDatabaseType.Ntext;
				
				newDataTypesList.Add(MultiContentPickerShippingZonesDataTypeDef);
			}
			var StockDataTypeDef = umbracoVersion.GetDataTypeDefinition("uWebshop.Stock", new Guid("5744ead8-977b-44c1-b362-fe8bebca7098"));
			if (StockDataTypeDef == null)
			{				
				StockDataTypeDef = umbracoVersion.CreateDataTypeDefinition(-1, "uWebshop.Stock", new Guid("8b5da1fc-4c8a-4e8f-b314-ae19a850dae8"));
				StockDataTypeDef.Name = "uWebshop Stock";
				StockDataTypeDef.Key = new Guid("5744ead8-977b-44c1-b362-fe8bebca7098");
				StockDataTypeDef.DatabaseType = DataTypeDatabaseType.Ntext;
				
				newDataTypesList.Add(StockDataTypeDef);
			}
			var StorePickerDataTypeDef = umbracoVersion.GetDataTypeDefinition("uWebshop.StorePicker", new Guid("1e8cdc0b-436e-46f5-bfec-57be45745771"));
			if (StorePickerDataTypeDef == null)
			{				
				StorePickerDataTypeDef = umbracoVersion.CreateDataTypeDefinition(-1, "uWebshop.StorePicker", new Guid("5fa345e3-9352-45d6-adaa-2da6cdc9aca3"));
				StorePickerDataTypeDef.Name = "uWebshop Store Picker";
				StorePickerDataTypeDef.Key = new Guid("1e8cdc0b-436e-46f5-bfec-57be45745771");
				StorePickerDataTypeDef.DatabaseType = DataTypeDatabaseType.Integer;
				
				newDataTypesList.Add(StorePickerDataTypeDef);
			}
			var StoreTemplatePickerDataTypeDef = umbracoVersion.GetDataTypeDefinition("uWebshop.StoreTemplatePicker", new Guid("a20c7c00-09f1-448d-9656-f5cb012107af"));
			if (StoreTemplatePickerDataTypeDef == null)
			{				
				StoreTemplatePickerDataTypeDef = umbracoVersion.CreateDataTypeDefinition(-1, "uWebshop.StoreTemplatePicker", new Guid("2ad05995-470e-47d9-956d-dd2ec892343d"));
				StoreTemplatePickerDataTypeDef.Name = "uWebshop Template Picker";
				StoreTemplatePickerDataTypeDef.Key = new Guid("a20c7c00-09f1-448d-9656-f5cb012107af");
				StoreTemplatePickerDataTypeDef.DatabaseType = DataTypeDatabaseType.Integer;
				
				newDataTypesList.Add(StoreTemplatePickerDataTypeDef);
			}
			var VatPickerDataTypeDef = umbracoVersion.GetDataTypeDefinition("uWebshop.VatPicker", new Guid("69d3f953-b565-4269-9d68-4b39e13c70e5"));
			if (VatPickerDataTypeDef == null)
			{				
				VatPickerDataTypeDef = umbracoVersion.CreateDataTypeDefinition(-1, "uWebshop.VatPicker", new Guid("a74ea9c9-8e18-4d2a-8cf6-73c6206c5da6"));
				VatPickerDataTypeDef.Name = "uWebshop Vat Picker";
				VatPickerDataTypeDef.Key = new Guid("69d3f953-b565-4269-9d68-4b39e13c70e5");
				VatPickerDataTypeDef.DatabaseType = DataTypeDatabaseType.Integer;
				
				newDataTypesList.Add(VatPickerDataTypeDef);
			}
			var ZonesDataTypeDef = umbracoVersion.GetDataTypeDefinition("uWebshop.Zones", new Guid("8bc628dd-fe95-4a73-bdde-a7f4b620c170"));
			if (ZonesDataTypeDef == null)
			{				
				ZonesDataTypeDef = umbracoVersion.CreateDataTypeDefinition(-1, "uWebshop.Zones", new Guid("4ee1243a-889b-4ffa-8f44-59cc1fc73be2"));
				ZonesDataTypeDef.Name = "uWebshop Zone Selector";
				ZonesDataTypeDef.Key = new Guid("8bc628dd-fe95-4a73-bdde-a7f4b620c170");
				ZonesDataTypeDef.DatabaseType = DataTypeDatabaseType.Ntext;
				
				newDataTypesList.Add(ZonesDataTypeDef);
			}
			if (newDataTypesList.Any()) dataTypeService.Save(newDataTypesList);

EnableDisableDataTypeDef = dataTypeService.GetDataTypeDefinitionById(new Guid("63c6fa9a-975f-4474-9155-62a229bafaef"));
if (EnableDisableDataTypeDef == null) throw new Exception("Could not create and/or load EnableDisable datatype");
MemberGroupsDataTypeDef = dataTypeService.GetDataTypeDefinitionById(new Guid("a99d5614-8b33-4a63-891a-a254c87af481"));
if (MemberGroupsDataTypeDef == null) throw new Exception("Could not create and/or load MemberGroups datatype");
MultiContentPickerCatalogDataTypeDef = dataTypeService.GetDataTypeDefinitionById(new Guid("ea745beb-271c-4542-a5be-5fba2be86f07"));
if (MultiContentPickerCatalogDataTypeDef == null) throw new Exception("Could not create and/or load MultiContentPickerCatalog datatype");
MultiContentPickerCategoriesDataTypeDef = dataTypeService.GetDataTypeDefinitionById(new Guid("c0d85cb4-6e4d-4f10-9107-abdee89b5d7d"));
if (MultiContentPickerCategoriesDataTypeDef == null) throw new Exception("Could not create and/or load MultiContentPickerCategories datatype");
CountriesDataTypeDef = dataTypeService.GetDataTypeDefinitionById(new Guid("a6d19ee8-ab93-42d6-a61a-8d4aaa759207"));
if (CountriesDataTypeDef == null) throw new Exception("Could not create and/or load Countries datatype");
CouponCodesDataTypeDef = dataTypeService.GetDataTypeDefinitionById(new Guid("0dbc0113-a084-44d1-8fef-f8ef0cd8453b"));
if (CouponCodesDataTypeDef == null) throw new Exception("Could not create and/or load CouponCodes datatype");
CurrenciesDataTypeDef = dataTypeService.GetDataTypeDefinitionById(new Guid("7e6cab81-528d-4b00-8321-72c36f131eea"));
if (CurrenciesDataTypeDef == null) throw new Exception("Could not create and/or load Currencies datatype");
DiscountOrderConditionDataTypeDef = dataTypeService.GetDataTypeDefinitionById(new Guid("e159f935-d7f4-4277-b5d0-adf74b003849"));
if (DiscountOrderConditionDataTypeDef == null) throw new Exception("Could not create and/or load DiscountOrderCondition datatype");
DiscountTypeDataTypeDef = dataTypeService.GetDataTypeDefinitionById(new Guid("2d89188e-33a7-4885-a6d0-1caef40320a7"));
if (DiscountTypeDataTypeDef == null) throw new Exception("Could not create and/or load DiscountType datatype");
TemplatePickerDataTypeDef = dataTypeService.GetDataTypeDefinitionById(new Guid("0a10d9dd-ebbc-48f5-be93-9fac239ac876"));
if (TemplatePickerDataTypeDef == null) throw new Exception("Could not create and/or load TemplatePicker datatype");
EmailDetailsDataTypeDef = dataTypeService.GetDataTypeDefinitionById(new Guid("9d87378c-5864-4ae2-bd83-1cd14b0c3290"));
if (EmailDetailsDataTypeDef == null) throw new Exception("Could not create and/or load EmailDetails datatype");
MultiContentPickerFilesDataTypeDef = dataTypeService.GetDataTypeDefinitionById(new Guid("83c13f11-95d8-4f10-b510-581983fe8c19"));
if (MultiContentPickerFilesDataTypeDef == null) throw new Exception("Could not create and/or load MultiContentPickerFiles datatype");
MultiNodePickerDataTypeDef = dataTypeService.GetDataTypeDefinitionById(new Guid("97600235-acf7-4ade-9ba9-6cad4743cb6d"));
if (MultiNodePickerDataTypeDef == null) throw new Exception("Could not create and/or load MultiNodePicker datatype");
MultiContentPickerImagesDataTypeDef = dataTypeService.GetDataTypeDefinitionById(new Guid("d61cbc2c-87d3-49d9-8e58-87996d22d97f"));
if (MultiContentPickerImagesDataTypeDef == null) throw new Exception("Could not create and/or load MultiContentPickerImages datatype");
OrderedCountDataTypeDef = dataTypeService.GetDataTypeDefinitionById(new Guid("2f6e0aa8-8291-4544-8aeb-db78cfb42b07"));
if (OrderedCountDataTypeDef == null) throw new Exception("Could not create and/or load OrderedCount datatype");
CulturesDataTypeDef = dataTypeService.GetDataTypeDefinitionById(new Guid("4235f880-64cc-4d78-8fc2-6e6e5ee72010"));
if (CulturesDataTypeDef == null) throw new Exception("Could not create and/or load Cultures datatype");
OrderStatusPickerDataTypeDef = dataTypeService.GetDataTypeDefinitionById(new Guid("8fa38a1b-7854-43ce-b03e-c7cc2ffd3d20"));
if (OrderStatusPickerDataTypeDef == null) throw new Exception("Could not create and/or load OrderStatusPicker datatype");
OrderInfoViewerDataTypeDef = dataTypeService.GetDataTypeDefinitionById(new Guid("6f455770-3677-4c6c-843d-2c76d7b33893"));
if (OrderInfoViewerDataTypeDef == null) throw new Exception("Could not create and/or load OrderInfoViewer datatype");
OrderSectionDataTypeDef = dataTypeService.GetDataTypeDefinitionById(new Guid("de47d313-9364-472b-8ee7-9b002cc204b9"));
if (OrderSectionDataTypeDef == null) throw new Exception("Could not create and/or load OrderSection datatype");
PaymentProviderAmountTypeDataTypeDef = dataTypeService.GetDataTypeDefinitionById(new Guid("fb79c76e-8ccc-406d-a3ad-39c1f939a38d"));
if (PaymentProviderAmountTypeDataTypeDef == null) throw new Exception("Could not create and/or load PaymentProviderAmountType datatype");
PaymentProviderTypeDataTypeDef = dataTypeService.GetDataTypeDefinitionById(new Guid("bbaf3c0e-fc22-4d33-b884-7b87d8dc3c8c"));
if (PaymentProviderTypeDataTypeDef == null) throw new Exception("Could not create and/or load PaymentProviderType datatype");
MultiContentPickerPaymentZonesDataTypeDef = dataTypeService.GetDataTypeDefinitionById(new Guid("406a4f2d-dd31-43be-9114-8077f43a6151"));
if (MultiContentPickerPaymentZonesDataTypeDef == null) throw new Exception("Could not create and/or load MultiContentPickerPaymentZones datatype");
PriceDataTypeDef = dataTypeService.GetDataTypeDefinitionById(new Guid("40e2736a-8ecd-41b3-bb27-7d11909f0a21"));
if (PriceDataTypeDef == null) throw new Exception("Could not create and/or load Price datatype");
MultiContentPickerProductsDataTypeDef = dataTypeService.GetDataTypeDefinitionById(new Guid("23806a76-6a36-468c-8188-f25308a71cdb"));
if (MultiContentPickerProductsDataTypeDef == null) throw new Exception("Could not create and/or load MultiContentPickerProducts datatype");
ProductOverviewDataTypeDef = dataTypeService.GetDataTypeDefinitionById(new Guid("3873106b-fbb8-4d55-8e91-a07680e796d7"));
if (ProductOverviewDataTypeDef == null) throw new Exception("Could not create and/or load ProductOverview datatype");
RangesDataTypeDef = dataTypeService.GetDataTypeDefinitionById(new Guid("09dfbeec-2681-42af-b07c-0ed56a575d48"));
if (RangesDataTypeDef == null) throw new Exception("Could not create and/or load Ranges datatype");
ShippingProviderTypeDataTypeDef = dataTypeService.GetDataTypeDefinitionById(new Guid("c035ada7-5413-48de-8e62-b4b61e2e934f"));
if (ShippingProviderTypeDataTypeDef == null) throw new Exception("Could not create and/or load ShippingProviderType datatype");
ShippingProviderRangeTypeDataTypeDef = dataTypeService.GetDataTypeDefinitionById(new Guid("34c70cbf-ee1b-465e-941f-ddcd097f7912"));
if (ShippingProviderRangeTypeDataTypeDef == null) throw new Exception("Could not create and/or load ShippingProviderRangeType datatype");
MultiContentPickerShippingZonesDataTypeDef = dataTypeService.GetDataTypeDefinitionById(new Guid("18ee35a7-7931-4d80-822c-ffe2bfb40f6e"));
if (MultiContentPickerShippingZonesDataTypeDef == null) throw new Exception("Could not create and/or load MultiContentPickerShippingZones datatype");
StockDataTypeDef = dataTypeService.GetDataTypeDefinitionById(new Guid("5744ead8-977b-44c1-b362-fe8bebca7098"));
if (StockDataTypeDef == null) throw new Exception("Could not create and/or load Stock datatype");
StorePickerDataTypeDef = dataTypeService.GetDataTypeDefinitionById(new Guid("1e8cdc0b-436e-46f5-bfec-57be45745771"));
if (StorePickerDataTypeDef == null) throw new Exception("Could not create and/or load StorePicker datatype");
StoreTemplatePickerDataTypeDef = dataTypeService.GetDataTypeDefinitionById(new Guid("a20c7c00-09f1-448d-9656-f5cb012107af"));
if (StoreTemplatePickerDataTypeDef == null) throw new Exception("Could not create and/or load StoreTemplatePicker datatype");
VatPickerDataTypeDef = dataTypeService.GetDataTypeDefinitionById(new Guid("69d3f953-b565-4269-9d68-4b39e13c70e5"));
if (VatPickerDataTypeDef == null) throw new Exception("Could not create and/or load VatPicker datatype");
ZonesDataTypeDef = dataTypeService.GetDataTypeDefinitionById(new Guid("8bc628dd-fe95-4a73-bdde-a7f4b620c170"));
if (ZonesDataTypeDef == null) throw new Exception("Could not create and/or load Zones datatype");

if (newDataTypesList.Contains(MultiContentPickerCatalogDataTypeDef))
	dataTypeService.SavePreValues(MultiContentPickerCatalogDataTypeDef.Id, new[] { "content","*","-1","False","1","1","0","False","1","0","//uwbsCatalog","200","0" });
if (newDataTypesList.Contains(MultiContentPickerCategoriesDataTypeDef))
	dataTypeService.SavePreValues(MultiContentPickerCategoriesDataTypeDef.Id, new[] { "content","/*[starts-with(name(),'uwbsCategory')]","-1","False","1","1","0","False","1","0","//uwbsCategoryRepository","200","0" });
if (newDataTypesList.Contains(MultiContentPickerFilesDataTypeDef))
	dataTypeService.SavePreValues(MultiContentPickerFilesDataTypeDef.Id, new[] { "media","*","-1","False","1","1","-1","False","0","0","/","200","0" });
if (newDataTypesList.Contains(MultiNodePickerDataTypeDef))
	dataTypeService.SavePreValues(MultiNodePickerDataTypeDef.Id, new[] { "content","*","-1","False","1","1","0","False","1","0","/*","200","0" });
if (newDataTypesList.Contains(MultiContentPickerImagesDataTypeDef))
	dataTypeService.SavePreValues(MultiContentPickerImagesDataTypeDef.Id, new[] { "media","*","-1","False","1","1","-1","False","0","0","/","200","0" });
if (newDataTypesList.Contains(OrderInfoViewerDataTypeDef))
	dataTypeService.SavePreValues(OrderInfoViewerDataTypeDef.Id, new[] { "/uWebshopBackend/uWebshopUmbracoOrderDetails.cshtml" });
if (newDataTypesList.Contains(OrderSectionDataTypeDef))
	dataTypeService.SavePreValues(OrderSectionDataTypeDef.Id, new[] { "/uWebshopBackend/uWebshopUmbracoOrderOverview.cshtml" });
if (newDataTypesList.Contains(MultiContentPickerPaymentZonesDataTypeDef))
	dataTypeService.SavePreValues(MultiContentPickerPaymentZonesDataTypeDef.Id, new[] { "content","/*[starts-with(name(),'uwbsPaymentProviderZone')]","-1","False","1","1","0","False","1","0","//uwbsPaymentProviderZoneSection","200","0" });
if (newDataTypesList.Contains(MultiContentPickerProductsDataTypeDef))
	dataTypeService.SavePreValues(MultiContentPickerProductsDataTypeDef.Id, new[] { "content","/*[starts-with(name(),'uwbsProduct')]","-1","False","1","1","0","False","1","0","//uwbsCatalog","200","0" });
if (newDataTypesList.Contains(ProductOverviewDataTypeDef))
	dataTypeService.SavePreValues(ProductOverviewDataTypeDef.Id, new[] { "/uWebshopBackend/uWebshopUmbracoProductOverview.cshtml" });
if (newDataTypesList.Contains(MultiContentPickerShippingZonesDataTypeDef))
	dataTypeService.SavePreValues(MultiContentPickerShippingZonesDataTypeDef.Id, new[] { "content","/*[starts-with(name(),'uwbsShippingProviderZone')]","-1","False","1","1","0","False","1","0","//uwbsShippingProviderZoneSection","200","0" });
if (newDataTypesList.Contains(VatPickerDataTypeDef))
	dataTypeService.SavePreValues(VatPickerDataTypeDef.Id, new[] { "0","6","15","19","21" });

if (newDataTypesList.Any()) dataTypeService.Save(newDataTypesList);


var uwbsCatalogContentType = contentTypeService.GetContentType("uwbsCatalog") ?? new ContentType(-1) {
Alias = "uwbsCatalog",
Name = "Catalog",
Description = "#CatalogDescription",
Thumbnail = "Folder.png",
Icon = umbracoVersionMajor > 6 ? "icon-uwebshop-stack" : "clipboard-list.png",
SortOrder = 1,
AllowedContentTypes = new List<ContentTypeSort>(),
AllowedTemplates = new List<ITemplate>(),
PropertyGroups = new PropertyGroupCollection(new List<PropertyGroup>()),};
contentTypeList.Add(uwbsCatalogContentType);



var uwbsCategoryContentType = contentTypeService.GetContentType("uwbsCategory") ?? new ContentType(-1) {
Alias = "uwbsCategory",
Name = "Category",
Description = "#CategoryDescription",
Thumbnail = "Folder.png",
Icon = umbracoVersionMajor > 6 ? "icon-uwebshop-stacklist" : "folder.png",
SortOrder = 1,
AllowedContentTypes = new List<ContentTypeSort>(),
AllowedTemplates = new List<ITemplate>(),
PropertyGroups = new PropertyGroupCollection(new List<PropertyGroup>()),};
contentTypeList.Add(uwbsCategoryContentType);

if (uwbsCategoryContentType.PropertyTypes.All(p => p.Alias != "title")){
GetOrAddPropertyGroup(uwbsCategoryContentType, "Global").PropertyTypes.Add(new PropertyType(StringDataTypeDef) { Alias = "title", Name = "#Title", Description = "#TitleDescription",Mandatory = true, });
}
if (uwbsCategoryContentType.PropertyTypes.All(p => p.Alias != "url")){
GetOrAddPropertyGroup(uwbsCategoryContentType, "Global").PropertyTypes.Add(new PropertyType(StringDataTypeDef) { Alias = "url", Name = "#Url", Description = "#UrlDescription",Mandatory = true, });
}
if (uwbsCategoryContentType.PropertyTypes.All(p => p.Alias != "metaDescription")){
GetOrAddPropertyGroup(uwbsCategoryContentType, "Global").PropertyTypes.Add(new PropertyType(TextboxMultipleDataTypeDef) { Alias = "metaDescription", Name = "#MetaDescription", Description = "#MetaDescriptionDescription",});
}
if (uwbsCategoryContentType.PropertyTypes.All(p => p.Alias != "metaTags")){
GetOrAddPropertyGroup(uwbsCategoryContentType, "Global").PropertyTypes.Add(new PropertyType(TagsDataTypeDef) { Alias = "metaTags", Name = "#Tags", Description = "#TagsDescription",});
}
if (uwbsCategoryContentType.PropertyTypes.All(p => p.Alias != "categories")){
GetOrAddPropertyGroup(uwbsCategoryContentType, "Global").PropertyTypes.Add(new PropertyType(MultiContentPickerCategoriesDataTypeDef) { Alias = "categories", Name = "#SubCategories", Description = "#SubCategoriesDescription",});
}
if (uwbsCategoryContentType.PropertyTypes.All(p => p.Alias != "description")){
GetOrAddPropertyGroup(uwbsCategoryContentType, "Details").PropertyTypes.Add(new PropertyType(RichTextDataTypeDef) { Alias = "description", Name = "#Description", Description = "#DescriptionDescription",});
}
if (uwbsCategoryContentType.PropertyTypes.All(p => p.Alias != "images")){
GetOrAddPropertyGroup(uwbsCategoryContentType, "Details").PropertyTypes.Add(new PropertyType(MultiContentPickerImagesDataTypeDef) { Alias = "images", Name = "#Images", Description = "#ImagesDescription",});
}


var uwbsCategoryRepositoryContentType = contentTypeService.GetContentType("uwbsCategoryRepository") ?? new ContentType(-1) {
Alias = "uwbsCategoryRepository",
Name = "Category Repository",
Description = "#CategoryRepositoryDescription",
Thumbnail = "Folder.png",
Icon = umbracoVersionMajor > 6 ? "icon-uwebshop-clipboard" : "folder-search-result.png",
SortOrder = 1,
AllowedContentTypes = new List<ContentTypeSort>(),
AllowedTemplates = new List<ITemplate>(),
PropertyGroups = new PropertyGroupCollection(new List<PropertyGroup>()),};
contentTypeList.Add(uwbsCategoryRepositoryContentType);



var uwbsOrderDateFolderContentType = contentTypeService.GetContentType("uwbsOrderDateFolder") ?? new ContentType(-1) {
Alias = "uwbsOrderDateFolder",
Name = "DateFolder",
Description = "#DateFolderDescription",
Thumbnail = "Folder.png",
Icon = umbracoVersionMajor > 6 ? "icon-uwebshop-calendar" : "calendar-month.png",
SortOrder = 1,
AllowedContentTypes = new List<ContentTypeSort>(),
AllowedTemplates = new List<ITemplate>(),
PropertyGroups = new PropertyGroupCollection(new List<PropertyGroup>()),};
contentTypeList.Add(uwbsOrderDateFolderContentType);



var uwbsDiscountOrderBasicContentType = contentTypeService.GetContentType("uwbsDiscountOrderBasic") ?? new ContentType(-1) {
Alias = "uwbsDiscountOrderBasic",
Name = "Basic Order Discount",
Description = "#DiscountOrderBasicDescription",
Thumbnail = "Folder.png",
Icon = umbracoVersionMajor > 6 ? "icon-uwebshop-cut" : "scissors.png",
SortOrder = 1,
AllowedContentTypes = new List<ContentTypeSort>(),
AllowedTemplates = new List<ITemplate>(),
PropertyGroups = new PropertyGroupCollection(new List<PropertyGroup>()),};
contentTypeList.Add(uwbsDiscountOrderBasicContentType);

if (uwbsDiscountOrderBasicContentType.PropertyTypes.All(p => p.Alias != "title")){
GetOrAddPropertyGroup(uwbsDiscountOrderBasicContentType, "Global").PropertyTypes.Add(new PropertyType(StringDataTypeDef) { Alias = "title", Name = "#Title", Description = "#TitleDescription",Mandatory = true, });
}
if (uwbsDiscountOrderBasicContentType.PropertyTypes.All(p => p.Alias != "description")){
GetOrAddPropertyGroup(uwbsDiscountOrderBasicContentType, "Global").PropertyTypes.Add(new PropertyType(RichTextDataTypeDef) { Alias = "description", Name = "#Description", Description = "#DescriptionDescription",});
}
if (uwbsDiscountOrderBasicContentType.PropertyTypes.All(p => p.Alias != "disable")){
GetOrAddPropertyGroup(uwbsDiscountOrderBasicContentType, "Global").PropertyTypes.Add(new PropertyType(TrueFalseDataTypeDef) { Alias = "disable", Name = "#Disable", Description = "#DisableDescription",});
}
if (uwbsDiscountOrderBasicContentType.PropertyTypes.All(p => p.Alias != "discountType")){
GetOrAddPropertyGroup(uwbsDiscountOrderBasicContentType, "Details").PropertyTypes.Add(new PropertyType(DiscountTypeDataTypeDef) { Alias = "discountType", Name = "#DiscountType", Description = "#DiscountTypeDescription",});
}
if (uwbsDiscountOrderBasicContentType.PropertyTypes.All(p => p.Alias != "discount")){
GetOrAddPropertyGroup(uwbsDiscountOrderBasicContentType, "Details").PropertyTypes.Add(new PropertyType(PriceDataTypeDef) { Alias = "discount", Name = "#Discount", Description = "#DiscountDescription",});
}


var uwbsDiscountOrderCountdownContentType = contentTypeService.GetContentType("uwbsDiscountOrderCountdown") ?? new ContentType(-1) {
Alias = "uwbsDiscountOrderCountdown",
Name = "Countdown Order Discount",
Description = "#DiscountOrderCountdownDescription",
Thumbnail = "Folder.png",
Icon = umbracoVersionMajor > 6 ? "icon-uwebshop-cut" : "scissors.png",
SortOrder = 1,
AllowedContentTypes = new List<ContentTypeSort>(),
AllowedTemplates = new List<ITemplate>(),
PropertyGroups = new PropertyGroupCollection(new List<PropertyGroup>()),};
contentTypeList.Add(uwbsDiscountOrderCountdownContentType);

if (uwbsDiscountOrderCountdownContentType.PropertyTypes.All(p => p.Alias != "title")){
GetOrAddPropertyGroup(uwbsDiscountOrderCountdownContentType, "Global").PropertyTypes.Add(new PropertyType(StringDataTypeDef) { Alias = "title", Name = "#Title", Description = "#TitleDescription",Mandatory = true, });
}
if (uwbsDiscountOrderCountdownContentType.PropertyTypes.All(p => p.Alias != "description")){
GetOrAddPropertyGroup(uwbsDiscountOrderCountdownContentType, "Global").PropertyTypes.Add(new PropertyType(RichTextDataTypeDef) { Alias = "description", Name = "#Description", Description = "#DescriptionDescription",});
}
if (uwbsDiscountOrderCountdownContentType.PropertyTypes.All(p => p.Alias != "disable")){
GetOrAddPropertyGroup(uwbsDiscountOrderCountdownContentType, "Global").PropertyTypes.Add(new PropertyType(TrueFalseDataTypeDef) { Alias = "disable", Name = "#Disable", Description = "#DisableDescription",});
}
if (uwbsDiscountOrderCountdownContentType.PropertyTypes.All(p => p.Alias != "discountType")){
GetOrAddPropertyGroup(uwbsDiscountOrderCountdownContentType, "Details").PropertyTypes.Add(new PropertyType(DiscountTypeDataTypeDef) { Alias = "discountType", Name = "#DiscountType", Description = "#DiscountTypeDescription",});
}
if (uwbsDiscountOrderCountdownContentType.PropertyTypes.All(p => p.Alias != "discount")){
GetOrAddPropertyGroup(uwbsDiscountOrderCountdownContentType, "Details").PropertyTypes.Add(new PropertyType(PriceDataTypeDef) { Alias = "discount", Name = "#Discount", Description = "#DiscountDescription",});
}
if (uwbsDiscountOrderCountdownContentType.PropertyTypes.All(p => p.Alias != "countdownEnabled")){
GetOrAddPropertyGroup(uwbsDiscountOrderCountdownContentType, "Details").PropertyTypes.Add(new PropertyType(TrueFalseDataTypeDef) { Alias = "countdownEnabled", Name = "#CountdownEnabled", Description = "#CountdownEnabledDescription",});
}
if (uwbsDiscountOrderCountdownContentType.PropertyTypes.All(p => p.Alias != "countdown")){
GetOrAddPropertyGroup(uwbsDiscountOrderCountdownContentType, "Details").PropertyTypes.Add(new PropertyType(StockDataTypeDef) { Alias = "countdown", Name = "#Countdown", Description = "#CountdownDescription",});
}


var uwbsDiscountOrderCouponContentType = contentTypeService.GetContentType("uwbsDiscountOrderCoupon") ?? new ContentType(-1) {
Alias = "uwbsDiscountOrderCoupon",
Name = "Coupon Order Discount",
Description = "#DiscountOrderCouponDescription",
Thumbnail = "Folder.png",
Icon = umbracoVersionMajor > 6 ? "icon-uwebshop-cut" : "scissors.png",
SortOrder = 1,
AllowedContentTypes = new List<ContentTypeSort>(),
AllowedTemplates = new List<ITemplate>(),
PropertyGroups = new PropertyGroupCollection(new List<PropertyGroup>()),};
contentTypeList.Add(uwbsDiscountOrderCouponContentType);

if (uwbsDiscountOrderCouponContentType.PropertyTypes.All(p => p.Alias != "title")){
GetOrAddPropertyGroup(uwbsDiscountOrderCouponContentType, "Global").PropertyTypes.Add(new PropertyType(StringDataTypeDef) { Alias = "title", Name = "#Title", Description = "#TitleDescription",Mandatory = true, });
}
if (uwbsDiscountOrderCouponContentType.PropertyTypes.All(p => p.Alias != "description")){
GetOrAddPropertyGroup(uwbsDiscountOrderCouponContentType, "Global").PropertyTypes.Add(new PropertyType(RichTextDataTypeDef) { Alias = "description", Name = "#Description", Description = "#DescriptionDescription",});
}
if (uwbsDiscountOrderCouponContentType.PropertyTypes.All(p => p.Alias != "disable")){
GetOrAddPropertyGroup(uwbsDiscountOrderCouponContentType, "Global").PropertyTypes.Add(new PropertyType(TrueFalseDataTypeDef) { Alias = "disable", Name = "#Disable", Description = "#DisableDescription",});
}
if (uwbsDiscountOrderCouponContentType.PropertyTypes.All(p => p.Alias != "discountType")){
GetOrAddPropertyGroup(uwbsDiscountOrderCouponContentType, "Details").PropertyTypes.Add(new PropertyType(DiscountTypeDataTypeDef) { Alias = "discountType", Name = "#DiscountType", Description = "#DiscountTypeDescription",});
}
if (uwbsDiscountOrderCouponContentType.PropertyTypes.All(p => p.Alias != "discount")){
GetOrAddPropertyGroup(uwbsDiscountOrderCouponContentType, "Details").PropertyTypes.Add(new PropertyType(PriceDataTypeDef) { Alias = "discount", Name = "#Discount", Description = "#DiscountDescription",});
}
if (uwbsDiscountOrderCouponContentType.PropertyTypes.All(p => p.Alias != "couponCodes")){
GetOrAddPropertyGroup(uwbsDiscountOrderCouponContentType, "Conditions").PropertyTypes.Add(new PropertyType(CouponCodesDataTypeDef) { Alias = "couponCodes", Name = "#CouponCodes", Description = "#CouponCodesDescription",});
}


var uwbsDiscountOrderMembergroupContentType = contentTypeService.GetContentType("uwbsDiscountOrderMembergroup") ?? new ContentType(-1) {
Alias = "uwbsDiscountOrderMembergroup",
Name = "Membergroup Order Discount",
Description = "#DiscountOrderMemberGroupDescription",
Thumbnail = "Folder.png",
Icon = umbracoVersionMajor > 6 ? "icon-uwebshop-cut" : "scissors.png",
SortOrder = 1,
AllowedContentTypes = new List<ContentTypeSort>(),
AllowedTemplates = new List<ITemplate>(),
PropertyGroups = new PropertyGroupCollection(new List<PropertyGroup>()),};
contentTypeList.Add(uwbsDiscountOrderMembergroupContentType);

if (uwbsDiscountOrderMembergroupContentType.PropertyTypes.All(p => p.Alias != "title")){
GetOrAddPropertyGroup(uwbsDiscountOrderMembergroupContentType, "Global").PropertyTypes.Add(new PropertyType(StringDataTypeDef) { Alias = "title", Name = "#Title", Description = "#TitleDescription",Mandatory = true, });
}
if (uwbsDiscountOrderMembergroupContentType.PropertyTypes.All(p => p.Alias != "description")){
GetOrAddPropertyGroup(uwbsDiscountOrderMembergroupContentType, "Global").PropertyTypes.Add(new PropertyType(RichTextDataTypeDef) { Alias = "description", Name = "#Description", Description = "#DescriptionDescription",});
}
if (uwbsDiscountOrderMembergroupContentType.PropertyTypes.All(p => p.Alias != "disable")){
GetOrAddPropertyGroup(uwbsDiscountOrderMembergroupContentType, "Global").PropertyTypes.Add(new PropertyType(TrueFalseDataTypeDef) { Alias = "disable", Name = "#Disable", Description = "#DisableDescription",});
}
if (uwbsDiscountOrderMembergroupContentType.PropertyTypes.All(p => p.Alias != "discountType")){
GetOrAddPropertyGroup(uwbsDiscountOrderMembergroupContentType, "Details").PropertyTypes.Add(new PropertyType(DiscountTypeDataTypeDef) { Alias = "discountType", Name = "#DiscountType", Description = "#DiscountTypeDescription",});
}
if (uwbsDiscountOrderMembergroupContentType.PropertyTypes.All(p => p.Alias != "discount")){
GetOrAddPropertyGroup(uwbsDiscountOrderMembergroupContentType, "Details").PropertyTypes.Add(new PropertyType(PriceDataTypeDef) { Alias = "discount", Name = "#Discount", Description = "#DiscountDescription",});
}
if (uwbsDiscountOrderMembergroupContentType.PropertyTypes.All(p => p.Alias != "memberGroups")){
GetOrAddPropertyGroup(uwbsDiscountOrderMembergroupContentType, "Conditions").PropertyTypes.Add(new PropertyType(MemberGroupsDataTypeDef) { Alias = "memberGroups", Name = "#MemberGroups", Description = "#MemberGroupsDescription",});
}


var uwbsDiscountOrderContentType = contentTypeService.GetContentType("uwbsDiscountOrder") ?? new ContentType(-1) {
Alias = "uwbsDiscountOrder",
Name = "Discount Order",
Description = "#DiscountOrderDescription",
Thumbnail = "Folder.png",
Icon = umbracoVersionMajor > 6 ? "icon-uwebshop-cut" : "scissors.png",
SortOrder = 1,
AllowedContentTypes = new List<ContentTypeSort>(),
AllowedTemplates = new List<ITemplate>(),
PropertyGroups = new PropertyGroupCollection(new List<PropertyGroup>()),};
contentTypeList.Add(uwbsDiscountOrderContentType);

if (uwbsDiscountOrderContentType.PropertyTypes.All(p => p.Alias != "title")){
GetOrAddPropertyGroup(uwbsDiscountOrderContentType, "Global").PropertyTypes.Add(new PropertyType(StringDataTypeDef) { Alias = "title", Name = "#Title", Description = "#TitleDescription",Mandatory = true, });
}
if (uwbsDiscountOrderContentType.PropertyTypes.All(p => p.Alias != "description")){
GetOrAddPropertyGroup(uwbsDiscountOrderContentType, "Global").PropertyTypes.Add(new PropertyType(RichTextDataTypeDef) { Alias = "description", Name = "#Description", Description = "#DescriptionDescription",});
}
if (uwbsDiscountOrderContentType.PropertyTypes.All(p => p.Alias != "disable")){
GetOrAddPropertyGroup(uwbsDiscountOrderContentType, "Global").PropertyTypes.Add(new PropertyType(TrueFalseDataTypeDef) { Alias = "disable", Name = "#Disable", Description = "#DisableDescription",});
}
if (uwbsDiscountOrderContentType.PropertyTypes.All(p => p.Alias != "shippingDiscountable")){
GetOrAddPropertyGroup(uwbsDiscountOrderContentType, "Details").PropertyTypes.Add(new PropertyType(TrueFalseDataTypeDef) { Alias = "shippingDiscountable", Name = "#ShippingDiscountable", Description = "#ShippingDiscountableDescription",});
}
if (uwbsDiscountOrderContentType.PropertyTypes.All(p => p.Alias != "discountType")){
GetOrAddPropertyGroup(uwbsDiscountOrderContentType, "Details").PropertyTypes.Add(new PropertyType(DiscountTypeDataTypeDef) { Alias = "discountType", Name = "#DiscountType", Description = "#DiscountTypeDescription",});
}
if (uwbsDiscountOrderContentType.PropertyTypes.All(p => p.Alias != "discount")){
GetOrAddPropertyGroup(uwbsDiscountOrderContentType, "Details").PropertyTypes.Add(new PropertyType(PriceDataTypeDef) { Alias = "discount", Name = "#Discount", Description = "#DiscountDescription",});
}
if (uwbsDiscountOrderContentType.PropertyTypes.All(p => p.Alias != "ranges")){
GetOrAddPropertyGroup(uwbsDiscountOrderContentType, "Details").PropertyTypes.Add(new PropertyType(RangesDataTypeDef) { Alias = "ranges", Name = "#Ranges", Description = "#RangesDescription",});
}
if (uwbsDiscountOrderContentType.PropertyTypes.All(p => p.Alias != "countdownEnabled")){
GetOrAddPropertyGroup(uwbsDiscountOrderContentType, "Details").PropertyTypes.Add(new PropertyType(TrueFalseDataTypeDef) { Alias = "countdownEnabled", Name = "#CountdownEnabled", Description = "#CountdownEnabledDescription",});
}
if (uwbsDiscountOrderContentType.PropertyTypes.All(p => p.Alias != "countdown")){
GetOrAddPropertyGroup(uwbsDiscountOrderContentType, "Details").PropertyTypes.Add(new PropertyType(StockDataTypeDef) { Alias = "countdown", Name = "#Countdown", Description = "#CountdownDescription",});
}
if (uwbsDiscountOrderContentType.PropertyTypes.All(p => p.Alias != "orderCondition")){
GetOrAddPropertyGroup(uwbsDiscountOrderContentType, "Conditions").PropertyTypes.Add(new PropertyType(DiscountOrderConditionDataTypeDef) { Alias = "orderCondition", Name = "#OrderCondition", Description = "#OrderConditionDescription",});
}
if (uwbsDiscountOrderContentType.PropertyTypes.All(p => p.Alias != "numberOfItemsCondition")){
GetOrAddPropertyGroup(uwbsDiscountOrderContentType, "Conditions").PropertyTypes.Add(new PropertyType(NumericDataTypeDef) { Alias = "numberOfItemsCondition", Name = "#NumberOfItemsCondition", Description = "#NumberOfItemsConditionDescription",});
}
if (uwbsDiscountOrderContentType.PropertyTypes.All(p => p.Alias != "minimumAmount")){
GetOrAddPropertyGroup(uwbsDiscountOrderContentType, "Conditions").PropertyTypes.Add(new PropertyType(PriceDataTypeDef) { Alias = "minimumAmount", Name = "#MinimumAmount", Description = "#MinimumAmountDescription",});
}
if (uwbsDiscountOrderContentType.PropertyTypes.All(p => p.Alias != "couponCodes")){
GetOrAddPropertyGroup(uwbsDiscountOrderContentType, "Conditions").PropertyTypes.Add(new PropertyType(CouponCodesDataTypeDef) { Alias = "couponCodes", Name = "#CouponCodes", Description = "#CouponCodesDescription",});
}
if (uwbsDiscountOrderContentType.PropertyTypes.All(p => p.Alias != "oncePerCustomer")){
GetOrAddPropertyGroup(uwbsDiscountOrderContentType, "Conditions").PropertyTypes.Add(new PropertyType(TrueFalseDataTypeDef) { Alias = "oncePerCustomer", Name = "#OncePerCustomer", Description = "#OncePerCustomerDescription",});
}
if (uwbsDiscountOrderContentType.PropertyTypes.All(p => p.Alias != "items")){
GetOrAddPropertyGroup(uwbsDiscountOrderContentType, "Filter").PropertyTypes.Add(new PropertyType(MultiNodePickerDataTypeDef) { Alias = "items", Name = "#Items", Description = "#ItemsDescription",});
}
if (uwbsDiscountOrderContentType.PropertyTypes.All(p => p.Alias != "affectedOrderlines")){
GetOrAddPropertyGroup(uwbsDiscountOrderContentType, "Filter").PropertyTypes.Add(new PropertyType(MultiContentPickerCatalogDataTypeDef) { Alias = "affectedOrderlines", Name = "#AffectedOrderlines", Description = "#AffectedOrderlinesDescription",});
}
if (uwbsDiscountOrderContentType.PropertyTypes.All(p => p.Alias != "affectedTags")){
GetOrAddPropertyGroup(uwbsDiscountOrderContentType, "Filter").PropertyTypes.Add(new PropertyType(TagsDataTypeDef) { Alias = "affectedTags", Name = "#AffectedTags", Description = "#AffectedTagsDescription",});
}
if (uwbsDiscountOrderContentType.PropertyTypes.All(p => p.Alias != "memberGroups")){
GetOrAddPropertyGroup(uwbsDiscountOrderContentType, "Conditions").PropertyTypes.Add(new PropertyType(MemberGroupsDataTypeDef) { Alias = "memberGroups", Name = "#MemberGroups", Description = "#MemberGroupsDescription",});
}


var uwbsDiscountOrderSectionContentType = contentTypeService.GetContentType("uwbsDiscountOrderSection") ?? new ContentType(-1) {
Alias = "uwbsDiscountOrderSection",
Name = "Discount Order Section",
Description = "#DiscountOrderSectionDescription",
Thumbnail = "Folder.png",
Icon = umbracoVersionMajor > 6 ? "icon-uwebshop-folder" : "inbox-document.png",
SortOrder = 1,
AllowedContentTypes = new List<ContentTypeSort>(),
AllowedTemplates = new List<ITemplate>(),
PropertyGroups = new PropertyGroupCollection(new List<PropertyGroup>()),};
contentTypeList.Add(uwbsDiscountOrderSectionContentType);



var uwbsDiscountProductContentType = contentTypeService.GetContentType("uwbsDiscountProduct") ?? new ContentType(-1) {
Alias = "uwbsDiscountProduct",
Name = "Discount Product",
Description = "#DiscountProductDescription",
Thumbnail = "Folder.png",
Icon = umbracoVersionMajor > 6 ? "icon-uwebshop-tag" : "price-tag--minus.png",
SortOrder = 1,
AllowedContentTypes = new List<ContentTypeSort>(),
AllowedTemplates = new List<ITemplate>(),
PropertyGroups = new PropertyGroupCollection(new List<PropertyGroup>()),};
contentTypeList.Add(uwbsDiscountProductContentType);

if (uwbsDiscountProductContentType.PropertyTypes.All(p => p.Alias != "title")){
GetOrAddPropertyGroup(uwbsDiscountProductContentType, "Global").PropertyTypes.Add(new PropertyType(StringDataTypeDef) { Alias = "title", Name = "#Title", Description = "#TitleDescription",Mandatory = true, });
}
if (uwbsDiscountProductContentType.PropertyTypes.All(p => p.Alias != "description")){
GetOrAddPropertyGroup(uwbsDiscountProductContentType, "Global").PropertyTypes.Add(new PropertyType(RichTextDataTypeDef) { Alias = "description", Name = "#Description", Description = "#DescriptionDescription",});
}
if (uwbsDiscountProductContentType.PropertyTypes.All(p => p.Alias != "disable")){
GetOrAddPropertyGroup(uwbsDiscountProductContentType, "Global").PropertyTypes.Add(new PropertyType(TrueFalseDataTypeDef) { Alias = "disable", Name = "#Disable", Description = "#DisableDescription",});
}
if (uwbsDiscountProductContentType.PropertyTypes.All(p => p.Alias != "products")){
GetOrAddPropertyGroup(uwbsDiscountProductContentType, "Details").PropertyTypes.Add(new PropertyType(MultiContentPickerCatalogDataTypeDef) { Alias = "products", Name = "#Products", Description = "#ProductsDescription",});
}
if (uwbsDiscountProductContentType.PropertyTypes.All(p => p.Alias != "excludeVariants")){
GetOrAddPropertyGroup(uwbsDiscountProductContentType, "Details").PropertyTypes.Add(new PropertyType(TrueFalseDataTypeDef) { Alias = "excludeVariants", Name = "#ExcludeVariants", Description = "#ExcludeVariantsDescription",});
}
if (uwbsDiscountProductContentType.PropertyTypes.All(p => p.Alias != "discountType")){
GetOrAddPropertyGroup(uwbsDiscountProductContentType, "Details").PropertyTypes.Add(new PropertyType(DiscountTypeDataTypeDef) { Alias = "discountType", Name = "#DiscountType", Description = "#DiscountTypeDescription",});
}
if (uwbsDiscountProductContentType.PropertyTypes.All(p => p.Alias != "discount")){
GetOrAddPropertyGroup(uwbsDiscountProductContentType, "Details").PropertyTypes.Add(new PropertyType(PriceDataTypeDef) { Alias = "discount", Name = "#Discount", Description = "#DiscountDescription",});
}
if (uwbsDiscountProductContentType.PropertyTypes.All(p => p.Alias != "ranges")){
GetOrAddPropertyGroup(uwbsDiscountProductContentType, "Details").PropertyTypes.Add(new PropertyType(RangesDataTypeDef) { Alias = "ranges", Name = "#Ranges", Description = "#RangesDescription",});
}
if (uwbsDiscountProductContentType.PropertyTypes.All(p => p.Alias != "countdownEnabled")){
GetOrAddPropertyGroup(uwbsDiscountProductContentType, "Details").PropertyTypes.Add(new PropertyType(TrueFalseDataTypeDef) { Alias = "countdownEnabled", Name = "#CountdownEnabled", Description = "#CountdownEnabledDescription",});
}
if (uwbsDiscountProductContentType.PropertyTypes.All(p => p.Alias != "countdown")){
GetOrAddPropertyGroup(uwbsDiscountProductContentType, "Details").PropertyTypes.Add(new PropertyType(StockDataTypeDef) { Alias = "countdown", Name = "#Countdown", Description = "#CountdownDescription",});
}
if (uwbsDiscountProductContentType.PropertyTypes.All(p => p.Alias != "memberGroups")){
GetOrAddPropertyGroup(uwbsDiscountProductContentType, "Conditions").PropertyTypes.Add(new PropertyType(MemberGroupsDataTypeDef) { Alias = "memberGroups", Name = "#MemberGroups", Description = "#MemberGroupsDescription",});
}


var uwbsDiscountProductSectionContentType = contentTypeService.GetContentType("uwbsDiscountProductSection") ?? new ContentType(-1) {
Alias = "uwbsDiscountProductSection",
Name = "Discount Product Section",
Description = "#DiscountProductSectionDescription",
Thumbnail = "Folder.png",
Icon = umbracoVersionMajor > 6 ? "icon-uwebshop-folder" : "inbox-document.png",
SortOrder = 1,
AllowedContentTypes = new List<ContentTypeSort>(),
AllowedTemplates = new List<ITemplate>(),
PropertyGroups = new PropertyGroupCollection(new List<PropertyGroup>()),};
contentTypeList.Add(uwbsDiscountProductSectionContentType);



var uwbsDiscountOrderRangedContentType = contentTypeService.GetContentType("uwbsDiscountOrderRanged") ?? new ContentType(-1) {
Alias = "uwbsDiscountOrderRanged",
Name = "Ranged Order Discount",
Description = "#DiscountOrderRangedDescription",
Thumbnail = "Folder.png",
Icon = umbracoVersionMajor > 6 ? "icon-uwebshop-cut" : "scissors.png",
SortOrder = 1,
AllowedContentTypes = new List<ContentTypeSort>(),
AllowedTemplates = new List<ITemplate>(),
PropertyGroups = new PropertyGroupCollection(new List<PropertyGroup>()),};
contentTypeList.Add(uwbsDiscountOrderRangedContentType);

if (uwbsDiscountOrderRangedContentType.PropertyTypes.All(p => p.Alias != "title")){
GetOrAddPropertyGroup(uwbsDiscountOrderRangedContentType, "Global").PropertyTypes.Add(new PropertyType(StringDataTypeDef) { Alias = "title", Name = "#Title", Description = "#TitleDescription",Mandatory = true, });
}
if (uwbsDiscountOrderRangedContentType.PropertyTypes.All(p => p.Alias != "description")){
GetOrAddPropertyGroup(uwbsDiscountOrderRangedContentType, "Global").PropertyTypes.Add(new PropertyType(RichTextDataTypeDef) { Alias = "description", Name = "#Description", Description = "#DescriptionDescription",});
}
if (uwbsDiscountOrderRangedContentType.PropertyTypes.All(p => p.Alias != "disable")){
GetOrAddPropertyGroup(uwbsDiscountOrderRangedContentType, "Global").PropertyTypes.Add(new PropertyType(TrueFalseDataTypeDef) { Alias = "disable", Name = "#Disable", Description = "#DisableDescription",});
}
if (uwbsDiscountOrderRangedContentType.PropertyTypes.All(p => p.Alias != "discountType")){
GetOrAddPropertyGroup(uwbsDiscountOrderRangedContentType, "Details").PropertyTypes.Add(new PropertyType(DiscountTypeDataTypeDef) { Alias = "discountType", Name = "#DiscountType", Description = "#DiscountTypeDescription",});
}
if (uwbsDiscountOrderRangedContentType.PropertyTypes.All(p => p.Alias != "discount")){
GetOrAddPropertyGroup(uwbsDiscountOrderRangedContentType, "Details").PropertyTypes.Add(new PropertyType(PriceDataTypeDef) { Alias = "discount", Name = "#Discount", Description = "#DiscountDescription",});
}
if (uwbsDiscountOrderRangedContentType.PropertyTypes.All(p => p.Alias != "ranges")){
GetOrAddPropertyGroup(uwbsDiscountOrderRangedContentType, "Details").PropertyTypes.Add(new PropertyType(RangesDataTypeDef) { Alias = "ranges", Name = "#Ranges", Description = "#RangesDescription",});
}


var uwbsDiscountRepositoryContentType = contentTypeService.GetContentType("uwbsDiscountRepository") ?? new ContentType(-1) {
Alias = "uwbsDiscountRepository",
Name = "Discount Repository",
Description = "#DiscountRepositoryDescription",
Thumbnail = "Folder.png",
Icon = umbracoVersionMajor > 6 ? "icon-uwebshop-bullhorn" : "megaphone.png",
SortOrder = 1,
AllowedContentTypes = new List<ContentTypeSort>(),
AllowedTemplates = new List<ITemplate>(),
PropertyGroups = new PropertyGroupCollection(new List<PropertyGroup>()),};
contentTypeList.Add(uwbsDiscountRepositoryContentType);



var uwbsEmailTemplateCustomerContentType = contentTypeService.GetContentType("uwbsEmailTemplateCustomer") ?? new ContentType(-1) {
Alias = "uwbsEmailTemplateCustomer",
Name = "Customer Email",
Description = "#CustomerEmailDescription",
Thumbnail = "Folder.png",
Icon = umbracoVersionMajor > 6 ? "icon-uwebshop-envelope" : "mail-open-document-text.png",
SortOrder = 1,
AllowedContentTypes = new List<ContentTypeSort>(),
AllowedTemplates = new List<ITemplate>(),
PropertyGroups = new PropertyGroupCollection(new List<PropertyGroup>()),};
contentTypeList.Add(uwbsEmailTemplateCustomerContentType);

if (uwbsEmailTemplateCustomerContentType.PropertyTypes.All(p => p.Alias != "title")){
GetOrAddPropertyGroup(uwbsEmailTemplateCustomerContentType, "Global").PropertyTypes.Add(new PropertyType(StringDataTypeDef) { Alias = "title", Name = "#Title", Description = "#TitleDescription",});
}
if (uwbsEmailTemplateCustomerContentType.PropertyTypes.All(p => p.Alias != "description")){
GetOrAddPropertyGroup(uwbsEmailTemplateCustomerContentType, "Global").PropertyTypes.Add(new PropertyType(RichTextDataTypeDef) { Alias = "description", Name = "#Description", Description = "#DescriptionDescription",});
}
if (uwbsEmailTemplateCustomerContentType.PropertyTypes.All(p => p.Alias != "emailtemplate")){
GetOrAddPropertyGroup(uwbsEmailTemplateCustomerContentType, "Global").PropertyTypes.Add(new PropertyType(TemplatePickerDataTypeDef) { Alias = "emailtemplate", Name = "#Template", Description = "#TemplateDescription",});
}
if (uwbsEmailTemplateCustomerContentType.PropertyTypes.All(p => p.Alias != "templatePreview")&& umbracoVersionMajor == 6){
GetOrAddPropertyGroup(uwbsEmailTemplateCustomerContentType, "Global").PropertyTypes.Add(new PropertyType(EmailDetailsDataTypeDef) { Alias = "templatePreview", Name = "#TemplatePreview", Description = "#TemplatePreviewDescription",});
}


var uwbsEmailTemplateCustomerSectionContentType = contentTypeService.GetContentType("uwbsEmailTemplateCustomerSection") ?? new ContentType(-1) {
Alias = "uwbsEmailTemplateCustomerSection",
Name = "Email Template Customer Section",
Description = "#EmailTemplateCustomerSectionDescription",
Thumbnail = "Folder.png",
Icon = umbracoVersionMajor > 6 ? "icon-uwebshop-folder" : "mail-air.png",
SortOrder = 1,
AllowedContentTypes = new List<ContentTypeSort>(),
AllowedTemplates = new List<ITemplate>(),
PropertyGroups = new PropertyGroupCollection(new List<PropertyGroup>()),};
contentTypeList.Add(uwbsEmailTemplateCustomerSectionContentType);



var uwbsEmailRepositoryContentType = contentTypeService.GetContentType("uwbsEmailRepository") ?? new ContentType(-1) {
Alias = "uwbsEmailRepository",
Name = "Email Repository",
Description = "#EmailRepositoryDescription",
Thumbnail = "Folder.png",
Icon = umbracoVersionMajor > 6 ? "icon-uwebshop-inbox" : "mails-stack.png",
SortOrder = 1,
AllowedContentTypes = new List<ContentTypeSort>(),
AllowedTemplates = new List<ITemplate>(),
PropertyGroups = new PropertyGroupCollection(new List<PropertyGroup>()),};
contentTypeList.Add(uwbsEmailRepositoryContentType);



var uwbsEmailTemplateStoreContentType = contentTypeService.GetContentType("uwbsEmailTemplateStore") ?? new ContentType(-1) {
Alias = "uwbsEmailTemplateStore",
Name = "Store Email",
Description = "#StoreEmailDescription",
Thumbnail = "Folder.png",
Icon = umbracoVersionMajor > 6 ? "icon-uwebshop-envelope" : "mail-open-document-text.png",
SortOrder = 1,
AllowedContentTypes = new List<ContentTypeSort>(),
AllowedTemplates = new List<ITemplate>(),
PropertyGroups = new PropertyGroupCollection(new List<PropertyGroup>()),};
contentTypeList.Add(uwbsEmailTemplateStoreContentType);

if (uwbsEmailTemplateStoreContentType.PropertyTypes.All(p => p.Alias != "title")){
GetOrAddPropertyGroup(uwbsEmailTemplateStoreContentType, "Global").PropertyTypes.Add(new PropertyType(StringDataTypeDef) { Alias = "title", Name = "#Title", Description = "#TitleDescription",});
}
if (uwbsEmailTemplateStoreContentType.PropertyTypes.All(p => p.Alias != "description")){
GetOrAddPropertyGroup(uwbsEmailTemplateStoreContentType, "Global").PropertyTypes.Add(new PropertyType(RichTextDataTypeDef) { Alias = "description", Name = "#Description", Description = "#DescriptionDescription",});
}
if (uwbsEmailTemplateStoreContentType.PropertyTypes.All(p => p.Alias != "emailtemplate")){
GetOrAddPropertyGroup(uwbsEmailTemplateStoreContentType, "Global").PropertyTypes.Add(new PropertyType(TemplatePickerDataTypeDef) { Alias = "emailtemplate", Name = "#Template", Description = "#TemplateDescription",});
}
if (uwbsEmailTemplateStoreContentType.PropertyTypes.All(p => p.Alias != "templatePreview")&& umbracoVersionMajor == 6){
GetOrAddPropertyGroup(uwbsEmailTemplateStoreContentType, "Global").PropertyTypes.Add(new PropertyType(EmailDetailsDataTypeDef) { Alias = "templatePreview", Name = "#TemplatePreview", Description = "#TemplatePreviewDescription",});
}


var uwbsEmailTemplateStoreSectionContentType = contentTypeService.GetContentType("uwbsEmailTemplateStoreSection") ?? new ContentType(-1) {
Alias = "uwbsEmailTemplateStoreSection",
Name = "Email Template Store Section",
Description = "#EmailTemplateStoreSectionDescription",
Thumbnail = "Folder.png",
Icon = umbracoVersionMajor > 6 ? "icon-uwebshop-folder" : "mail-air.png",
SortOrder = 1,
AllowedContentTypes = new List<ContentTypeSort>(),
AllowedTemplates = new List<ITemplate>(),
PropertyGroups = new PropertyGroupCollection(new List<PropertyGroup>()),};
contentTypeList.Add(uwbsEmailTemplateStoreSectionContentType);



var uwbsOrderContentType = contentTypeService.GetContentType("uwbsOrder") ?? new ContentType(-1) {
Alias = "uwbsOrder",
Name = "Order",
Description = "#OrderDescription",
Thumbnail = "Folder.png",
Icon = umbracoVersionMajor > 6 ? "icon-uwebshop-clipboard" : "clipboard-invoice.png",
SortOrder = 1,
AllowedContentTypes = new List<ContentTypeSort>(),
AllowedTemplates = new List<ITemplate>(),
PropertyGroups = new PropertyGroupCollection(new List<PropertyGroup>()),};
contentTypeList.Add(uwbsOrderContentType);

if (uwbsOrderContentType.PropertyTypes.All(p => p.Alias != "orderStatusPicker")&& umbracoVersionMajor == 6){
GetOrAddPropertyGroup(uwbsOrderContentType, "Global").PropertyTypes.Add(new PropertyType(OrderStatusPickerDataTypeDef) { Alias = "orderStatusPicker", Name = "#OrderStatusPicker", Description = "#OrderStatusPickerDescription",});
}
if (uwbsOrderContentType.PropertyTypes.All(p => p.Alias != "orderPaid")&& umbracoVersionMajor == 6){
GetOrAddPropertyGroup(uwbsOrderContentType, "Global").PropertyTypes.Add(new PropertyType(TrueFalseDataTypeDef) { Alias = "orderPaid", Name = "#OrderPaid", Description = "#OrderPaidDescription",});
}
if (uwbsOrderContentType.PropertyTypes.All(p => p.Alias != "orderDetails")){
GetOrAddPropertyGroup(uwbsOrderContentType, "Global").PropertyTypes.Add(new PropertyType(OrderInfoViewerDataTypeDef) { Alias = "orderDetails", Name = "#OrderDetails", Description = "#OrderDetailsDescription",});
}
if (uwbsOrderContentType.PropertyTypes.All(p => p.Alias != "orderGuid")&& umbracoVersionMajor == 6){
GetOrAddPropertyGroup(uwbsOrderContentType, "Global").PropertyTypes.Add(new PropertyType(LabelDataTypeDef) { Alias = "orderGuid", Name = "#OrderGuid", Description = "#OrderGuidDescription",});
}
if (uwbsOrderContentType.PropertyTypes.All(p => p.Alias != "customerEmail")){
GetOrAddPropertyGroup(uwbsOrderContentType, "Customer").PropertyTypes.Add(new PropertyType(StringDataTypeDef) { Alias = "customerEmail", Name = "#CustomerEmail", Description = "#CustomerEmailDescription",});
}
if (uwbsOrderContentType.PropertyTypes.All(p => p.Alias != "customerFirstName")){
GetOrAddPropertyGroup(uwbsOrderContentType, "Customer").PropertyTypes.Add(new PropertyType(StringDataTypeDef) { Alias = "customerFirstName", Name = "#CustomerFirstName", Description = "#CustomerFirstNameDescription",});
}
if (uwbsOrderContentType.PropertyTypes.All(p => p.Alias != "customerLastName")){
GetOrAddPropertyGroup(uwbsOrderContentType, "Customer").PropertyTypes.Add(new PropertyType(StringDataTypeDef) { Alias = "customerLastName", Name = "#CustomerLastName", Description = "#CustomerLastNameDescription",});
}


var uwbsOrderedProductContentType = contentTypeService.GetContentType("uwbsOrderedProduct") ?? new ContentType(-1) {
Alias = "uwbsOrderedProduct",
Name = "Ordered Product",
Description = "#OrderedProductDescription",
Thumbnail = "Folder.png",
Icon = umbracoVersionMajor > 6 ? "icon-uwebshop-box" : "box-label.png",
SortOrder = 1,
AllowedContentTypes = new List<ContentTypeSort>(),
AllowedTemplates = new List<ITemplate>(),
PropertyGroups = new PropertyGroupCollection(new List<PropertyGroup>()),};
contentTypeList.Add(uwbsOrderedProductContentType);

if (uwbsOrderedProductContentType.PropertyTypes.All(p => p.Alias != "productId")){
GetOrAddPropertyGroup(uwbsOrderedProductContentType, "Global").PropertyTypes.Add(new PropertyType(ContentPickerDataTypeDef) { Alias = "productId", Name = "#ProductId", Description = "#ProductIdDescription",});
}
if (uwbsOrderedProductContentType.PropertyTypes.All(p => p.Alias != "typeAlias")){
GetOrAddPropertyGroup(uwbsOrderedProductContentType, "Global").PropertyTypes.Add(new PropertyType(LabelDataTypeDef) { Alias = "typeAlias", Name = "#TypeAlias", Description = "#TypeAliasDescription",});
}
if (uwbsOrderedProductContentType.PropertyTypes.All(p => p.Alias != "title")){
GetOrAddPropertyGroup(uwbsOrderedProductContentType, "Global").PropertyTypes.Add(new PropertyType(StringDataTypeDef) { Alias = "title", Name = "#Title", Description = "#TitleDescription",});
}
if (uwbsOrderedProductContentType.PropertyTypes.All(p => p.Alias != "sku")){
GetOrAddPropertyGroup(uwbsOrderedProductContentType, "Global").PropertyTypes.Add(new PropertyType(StringDataTypeDef) { Alias = "sku", Name = "#SKU", Description = "#SKUDescription",});
}
if (uwbsOrderedProductContentType.PropertyTypes.All(p => p.Alias != "length")){
GetOrAddPropertyGroup(uwbsOrderedProductContentType, "Details").PropertyTypes.Add(new PropertyType(NumericDataTypeDef) { Alias = "length", Name = "#Length", Description = "#LengthDescription",});
}
if (uwbsOrderedProductContentType.PropertyTypes.All(p => p.Alias != "width")){
GetOrAddPropertyGroup(uwbsOrderedProductContentType, "Details").PropertyTypes.Add(new PropertyType(NumericDataTypeDef) { Alias = "width", Name = "#Width", Description = "WidthDescription",});
}
if (uwbsOrderedProductContentType.PropertyTypes.All(p => p.Alias != "height")){
GetOrAddPropertyGroup(uwbsOrderedProductContentType, "Details").PropertyTypes.Add(new PropertyType(NumericDataTypeDef) { Alias = "height", Name = "#Height", Description = "HeightDescription",});
}
if (uwbsOrderedProductContentType.PropertyTypes.All(p => p.Alias != "weight")){
GetOrAddPropertyGroup(uwbsOrderedProductContentType, "Details").PropertyTypes.Add(new PropertyType(NumericDataTypeDef) { Alias = "weight", Name = "#Weight", Description = "#WeightDescription",});
}
if (uwbsOrderedProductContentType.PropertyTypes.All(p => p.Alias != "ranges")){
GetOrAddPropertyGroup(uwbsOrderedProductContentType, "Price").PropertyTypes.Add(new PropertyType(RangesDataTypeDef) { Alias = "ranges", Name = "#Ranges", Description = "#RangesDescription",});
}
if (uwbsOrderedProductContentType.PropertyTypes.All(p => p.Alias != "price")){
GetOrAddPropertyGroup(uwbsOrderedProductContentType, "Price").PropertyTypes.Add(new PropertyType(PriceDataTypeDef) { Alias = "price", Name = "#Price", Description = "#PriceDescription",});
}
if (uwbsOrderedProductContentType.PropertyTypes.All(p => p.Alias != "vat")){
GetOrAddPropertyGroup(uwbsOrderedProductContentType, "Price").PropertyTypes.Add(new PropertyType(VatPickerDataTypeDef) { Alias = "vat", Name = "#VAT", Description = "#VatDescription",});
}
if (uwbsOrderedProductContentType.PropertyTypes.All(p => p.Alias != "itemCount")){
GetOrAddPropertyGroup(uwbsOrderedProductContentType, "Price").PropertyTypes.Add(new PropertyType(NumericDataTypeDef) { Alias = "itemCount", Name = "#ItemCount", Description = "#ItemCountDescription",});
}
if (uwbsOrderedProductContentType.PropertyTypes.All(p => p.Alias != "orderedProductDiscountPercentage")){
GetOrAddPropertyGroup(uwbsOrderedProductContentType, "Price").PropertyTypes.Add(new PropertyType(StringDataTypeDef) { Alias = "orderedProductDiscountPercentage", Name = "#OrderedProductDiscountPercentage", Description = "#OrderedProductDiscountPercentageDescription",});
}
if (uwbsOrderedProductContentType.PropertyTypes.All(p => p.Alias != "orderedProductDiscountAmount")){
GetOrAddPropertyGroup(uwbsOrderedProductContentType, "Price").PropertyTypes.Add(new PropertyType(StringDataTypeDef) { Alias = "orderedProductDiscountAmount", Name = "#OrderedProductDiscountAmount", Description = "#OrderedProductDiscountAmountDescription",});
}
if (uwbsOrderedProductContentType.PropertyTypes.All(p => p.Alias != "orderedProductDiscountExcludingVariants")){
GetOrAddPropertyGroup(uwbsOrderedProductContentType, "Price").PropertyTypes.Add(new PropertyType(TrueFalseDataTypeDef) { Alias = "orderedProductDiscountExcludingVariants", Name = "#OrderedProductDiscountExcludingVariants", Description = "#OrderedProductDiscountExcludingVariantsDescription",});
}


var uwbsOrderedProductVariantContentType = contentTypeService.GetContentType("uwbsOrderedProductVariant") ?? new ContentType(-1) {
Alias = "uwbsOrderedProductVariant",
Name = "Ordered Product Variant",
Description = "#OrderedProductVariantDescription",
Thumbnail = "Folder.png",
Icon = umbracoVersionMajor > 6 ? "icon-uwebshop-link" : "magnet-small.png",
SortOrder = 1,
AllowedContentTypes = new List<ContentTypeSort>(),
AllowedTemplates = new List<ITemplate>(),
PropertyGroups = new PropertyGroupCollection(new List<PropertyGroup>()),};
contentTypeList.Add(uwbsOrderedProductVariantContentType);

if (uwbsOrderedProductVariantContentType.PropertyTypes.All(p => p.Alias != "variantId")){
GetOrAddPropertyGroup(uwbsOrderedProductVariantContentType, "Global").PropertyTypes.Add(new PropertyType(ContentPickerDataTypeDef) { Alias = "variantId", Name = "#VariantId", Description = "#VariantIdDescription",});
}
if (uwbsOrderedProductVariantContentType.PropertyTypes.All(p => p.Alias != "typeAlias")){
GetOrAddPropertyGroup(uwbsOrderedProductVariantContentType, "Global").PropertyTypes.Add(new PropertyType(LabelDataTypeDef) { Alias = "typeAlias", Name = "#TypeAlias", Description = "#TypeAliasDescription",});
}
if (uwbsOrderedProductVariantContentType.PropertyTypes.All(p => p.Alias != "title")){
GetOrAddPropertyGroup(uwbsOrderedProductVariantContentType, "Global").PropertyTypes.Add(new PropertyType(StringDataTypeDef) { Alias = "title", Name = "#Title", Description = "#TitleDescription",});
}
if (uwbsOrderedProductVariantContentType.PropertyTypes.All(p => p.Alias != "sku")){
GetOrAddPropertyGroup(uwbsOrderedProductVariantContentType, "Global").PropertyTypes.Add(new PropertyType(StringDataTypeDef) { Alias = "sku", Name = "#SKU", Description = "#SKUDescription",});
}
if (uwbsOrderedProductVariantContentType.PropertyTypes.All(p => p.Alias != "group")){
GetOrAddPropertyGroup(uwbsOrderedProductVariantContentType, "Global").PropertyTypes.Add(new PropertyType(StringDataTypeDef) { Alias = "group", Name = "#Group", Description = "#GroupDescription",});
}
if (uwbsOrderedProductVariantContentType.PropertyTypes.All(p => p.Alias != "length")){
GetOrAddPropertyGroup(uwbsOrderedProductVariantContentType, "Details").PropertyTypes.Add(new PropertyType(NumericDataTypeDef) { Alias = "length", Name = "#Length", Description = "#LengthDescription",});
}
if (uwbsOrderedProductVariantContentType.PropertyTypes.All(p => p.Alias != "width")){
GetOrAddPropertyGroup(uwbsOrderedProductVariantContentType, "Details").PropertyTypes.Add(new PropertyType(NumericDataTypeDef) { Alias = "width", Name = "#Width", Description = "WidthDescription",});
}
if (uwbsOrderedProductVariantContentType.PropertyTypes.All(p => p.Alias != "height")){
GetOrAddPropertyGroup(uwbsOrderedProductVariantContentType, "Details").PropertyTypes.Add(new PropertyType(NumericDataTypeDef) { Alias = "height", Name = "#Height", Description = "HeightDescription",});
}
if (uwbsOrderedProductVariantContentType.PropertyTypes.All(p => p.Alias != "weight")){
GetOrAddPropertyGroup(uwbsOrderedProductVariantContentType, "Details").PropertyTypes.Add(new PropertyType(NumericDataTypeDef) { Alias = "weight", Name = "#Weight", Description = "#WeightDescription",});
}
if (uwbsOrderedProductVariantContentType.PropertyTypes.All(p => p.Alias != "price")){
GetOrAddPropertyGroup(uwbsOrderedProductVariantContentType, "Price").PropertyTypes.Add(new PropertyType(PriceDataTypeDef) { Alias = "price", Name = "#Price", Description = "#PriceDescription",});
}
if (uwbsOrderedProductVariantContentType.PropertyTypes.All(p => p.Alias != "ranges")){
GetOrAddPropertyGroup(uwbsOrderedProductVariantContentType, "Price").PropertyTypes.Add(new PropertyType(RangesDataTypeDef) { Alias = "ranges", Name = "#Ranges", Description = "#RangesDescription",});
}
if (uwbsOrderedProductVariantContentType.PropertyTypes.All(p => p.Alias != "discountPercentage")){
GetOrAddPropertyGroup(uwbsOrderedProductVariantContentType, "Price").PropertyTypes.Add(new PropertyType(StringDataTypeDef) { Alias = "discountPercentage", Name = "#OrderedProductVariantDiscountPercentage", Description = "#OrderedProductVariantDiscountPercentageDescription",});
}
if (uwbsOrderedProductVariantContentType.PropertyTypes.All(p => p.Alias != "discountAmount")){
GetOrAddPropertyGroup(uwbsOrderedProductVariantContentType, "Price").PropertyTypes.Add(new PropertyType(StringDataTypeDef) { Alias = "discountAmount", Name = "#OrderedProductVariantDiscountAmount", Description = "#OrderedProductVariantDiscountAmountDescription",});
}


var uwbsOrderRepositoryContentType = contentTypeService.GetContentType("uwbsOrderRepository") ?? new ContentType(-1) {
Alias = "uwbsOrderRepository",
Name = "Order Repository",
Description = "#OrderRepositoryDescription",
Thumbnail = "Folder.png",
Icon = umbracoVersionMajor > 6 ? "icon-uwebshop-archive" : "drawer.png",
SortOrder = 1,
AllowedContentTypes = new List<ContentTypeSort>(),
AllowedTemplates = new List<ITemplate>(),
PropertyGroups = new PropertyGroupCollection(new List<PropertyGroup>()),};
contentTypeList.Add(uwbsOrderRepositoryContentType);



var uwbsOrderSectionContentType = contentTypeService.GetContentType("uwbsOrderSection") ?? new ContentType(-1) {
Alias = "uwbsOrderSection",
Name = "Order Section",
Description = "#OrderSectionDescription",
Thumbnail = "Folder.png",
Icon = umbracoVersionMajor > 6 ? "icon-uwebshop-statistics" : "folder-open-table.png",
SortOrder = 1,
AllowedContentTypes = new List<ContentTypeSort>(),
AllowedTemplates = new List<ITemplate>(),
PropertyGroups = new PropertyGroupCollection(new List<PropertyGroup>()),};
contentTypeList.Add(uwbsOrderSectionContentType);

if (uwbsOrderSectionContentType.PropertyTypes.All(p => p.Alias != "orderSection")){
GetOrAddPropertyGroup(uwbsOrderSectionContentType, "Global").PropertyTypes.Add(new PropertyType(OrderSectionDataTypeDef) { Alias = "orderSection", Name = "#OrderSection", Description = "#OrderSectionDescription",});
}


var uwbsOrderStoreFolderContentType = contentTypeService.GetContentType("uwbsOrderStoreFolder") ?? new ContentType(-1) {
Alias = "uwbsOrderStoreFolder",
Name = "Order Store Folder",
Description = "#OrderStoreFolderDescription",
Thumbnail = "Folder.png",
Icon = umbracoVersionMajor > 6 ? "icon-uwebshop-store" : "store.png",
SortOrder = 1,
AllowedContentTypes = new List<ContentTypeSort>(),
AllowedTemplates = new List<ITemplate>(),
PropertyGroups = new PropertyGroupCollection(new List<PropertyGroup>()),};
contentTypeList.Add(uwbsOrderStoreFolderContentType);



var uwbsPaymentProviderContentType = contentTypeService.GetContentType("uwbsPaymentProvider") ?? new ContentType(-1) {
Alias = "uwbsPaymentProvider",
Name = "Payment Provider",
Description = "#PaymentProviderDescription",
Thumbnail = "Folder.png",
Icon = umbracoVersionMajor > 6 ? "icon-uwebshop-creditcard" : "credit-card.png",
SortOrder = 1,
AllowedContentTypes = new List<ContentTypeSort>(),
AllowedTemplates = new List<ITemplate>(),
PropertyGroups = new PropertyGroupCollection(new List<PropertyGroup>()),};
contentTypeList.Add(uwbsPaymentProviderContentType);

if (uwbsPaymentProviderContentType.PropertyTypes.All(p => p.Alias != "title")){
GetOrAddPropertyGroup(uwbsPaymentProviderContentType, "Global").PropertyTypes.Add(new PropertyType(StringDataTypeDef) { Alias = "title", Name = "#Title", Description = "#TitleDescription",});
}
if (uwbsPaymentProviderContentType.PropertyTypes.All(p => p.Alias != "description")){
GetOrAddPropertyGroup(uwbsPaymentProviderContentType, "Global").PropertyTypes.Add(new PropertyType(RichTextDataTypeDef) { Alias = "description", Name = "#Description", Description = "#DescriptionDescription",});
}
if (uwbsPaymentProviderContentType.PropertyTypes.All(p => p.Alias != "image")){
GetOrAddPropertyGroup(uwbsPaymentProviderContentType, "Global").PropertyTypes.Add(new PropertyType(MediaPickerDataTypeDef) { Alias = "image", Name = "#Image", Description = "#ImageDescription",});
}
if (uwbsPaymentProviderContentType.PropertyTypes.All(p => p.Alias != "type")){
GetOrAddPropertyGroup(uwbsPaymentProviderContentType, "Details").PropertyTypes.Add(new PropertyType(PaymentProviderTypeDataTypeDef) { Alias = "type", Name = "#PaymentProviderType", Description = "#PaymentProviderTypeDescription",});
}
if (uwbsPaymentProviderContentType.PropertyTypes.All(p => p.Alias != "zone")){
GetOrAddPropertyGroup(uwbsPaymentProviderContentType, "Details").PropertyTypes.Add(new PropertyType(MultiContentPickerPaymentZonesDataTypeDef) { Alias = "zone", Name = "#Zone", Description = "#ZoneDescription",});
}
if (uwbsPaymentProviderContentType.PropertyTypes.All(p => p.Alias != "successNode")){
GetOrAddPropertyGroup(uwbsPaymentProviderContentType, "Details").PropertyTypes.Add(new PropertyType(ContentPickerDataTypeDef) { Alias = "successNode", Name = "#SuccessNode", Description = "#SuccessNodeDescription",});
}
if (uwbsPaymentProviderContentType.PropertyTypes.All(p => p.Alias != "errorNode")){
GetOrAddPropertyGroup(uwbsPaymentProviderContentType, "Details").PropertyTypes.Add(new PropertyType(ContentPickerDataTypeDef) { Alias = "errorNode", Name = "#ErrorNode", Description = "#ErrorNodeDescription",});
}
if (uwbsPaymentProviderContentType.PropertyTypes.All(p => p.Alias != "testMode")){
GetOrAddPropertyGroup(uwbsPaymentProviderContentType, "Details").PropertyTypes.Add(new PropertyType(EnableDisableDataTypeDef) { Alias = "testMode", Name = "#TestMode", Description = "#TestModeDescription",});
}


var uwbsPaymentProviderMethodContentType = contentTypeService.GetContentType("uwbsPaymentProviderMethod") ?? new ContentType(-1) {
Alias = "uwbsPaymentProviderMethod",
Name = "Payment Provider Method",
Description = "#PaymentProviderMethodDescription",
Thumbnail = "Folder.png",
Icon = umbracoVersionMajor > 6 ? "icon-uwebshop-link" : "magnet-small.png",
SortOrder = 1,
AllowedContentTypes = new List<ContentTypeSort>(),
AllowedTemplates = new List<ITemplate>(),
PropertyGroups = new PropertyGroupCollection(new List<PropertyGroup>()),};
contentTypeList.Add(uwbsPaymentProviderMethodContentType);

if (uwbsPaymentProviderMethodContentType.PropertyTypes.All(p => p.Alias != "disable")){
GetOrAddPropertyGroup(uwbsPaymentProviderMethodContentType, "Global").PropertyTypes.Add(new PropertyType(TrueFalseDataTypeDef) { Alias = "disable", Name = "#Disable", Description = "#DisableDescription",});
}
if (uwbsPaymentProviderMethodContentType.PropertyTypes.All(p => p.Alias != "title")){
GetOrAddPropertyGroup(uwbsPaymentProviderMethodContentType, "Global").PropertyTypes.Add(new PropertyType(StringDataTypeDef) { Alias = "title", Name = "#Title", Description = "#TitleDescription",});
}
if (uwbsPaymentProviderMethodContentType.PropertyTypes.All(p => p.Alias != "description")){
GetOrAddPropertyGroup(uwbsPaymentProviderMethodContentType, "Global").PropertyTypes.Add(new PropertyType(RichTextDataTypeDef) { Alias = "description", Name = "#Description", Description = "#DescriptionDescription",});
}
if (uwbsPaymentProviderMethodContentType.PropertyTypes.All(p => p.Alias != "image")){
GetOrAddPropertyGroup(uwbsPaymentProviderMethodContentType, "Global").PropertyTypes.Add(new PropertyType(MediaPickerDataTypeDef) { Alias = "image", Name = "#Image", Description = "#ImageDescription",});
}
if (uwbsPaymentProviderMethodContentType.PropertyTypes.All(p => p.Alias != "price")){
GetOrAddPropertyGroup(uwbsPaymentProviderMethodContentType, "Price").PropertyTypes.Add(new PropertyType(PriceDataTypeDef) { Alias = "price", Name = "#Price", Description = "#PriceDescription",});
}
if (uwbsPaymentProviderMethodContentType.PropertyTypes.All(p => p.Alias != "vat")){
GetOrAddPropertyGroup(uwbsPaymentProviderMethodContentType, "Price").PropertyTypes.Add(new PropertyType(VatPickerDataTypeDef) { Alias = "vat", Name = "#VAT", Description = "#VatDescription",});
}
if (uwbsPaymentProviderMethodContentType.PropertyTypes.All(p => p.Alias != "amountType")){
GetOrAddPropertyGroup(uwbsPaymentProviderMethodContentType, "Price").PropertyTypes.Add(new PropertyType(PaymentProviderAmountTypeDataTypeDef) { Alias = "amountType", Name = "#AmountType", Description = "#AmountTypeDescription",});
}


var uwbsPaymentProviderRepositoryContentType = contentTypeService.GetContentType("uwbsPaymentProviderRepository") ?? new ContentType(-1) {
Alias = "uwbsPaymentProviderRepository",
Name = "Payment Provider Repository",
Description = "#PaymentProviderRepositoryDescription",
Thumbnail = "Folder.png",
Icon = umbracoVersionMajor > 6 ? "icon-uwebshop-piggybank" : "wallet.png",
SortOrder = 1,
AllowedContentTypes = new List<ContentTypeSort>(),
AllowedTemplates = new List<ITemplate>(),
PropertyGroups = new PropertyGroupCollection(new List<PropertyGroup>()),};
contentTypeList.Add(uwbsPaymentProviderRepositoryContentType);



var uwbsPaymentProviderSectionContentType = contentTypeService.GetContentType("uwbsPaymentProviderSection") ?? new ContentType(-1) {
Alias = "uwbsPaymentProviderSection",
Name = "Payment Provider Section",
Description = "#PaymentProviderSectionDescription",
Thumbnail = "Folder.png",
Icon = umbracoVersionMajor > 6 ? "icon-uwebshop-folder" : "bank.png",
SortOrder = 1,
AllowedContentTypes = new List<ContentTypeSort>(),
AllowedTemplates = new List<ITemplate>(),
PropertyGroups = new PropertyGroupCollection(new List<PropertyGroup>()),};
contentTypeList.Add(uwbsPaymentProviderSectionContentType);



var uwbsPaymentProviderZoneContentType = contentTypeService.GetContentType("uwbsPaymentProviderZone") ?? new ContentType(-1) {
Alias = "uwbsPaymentProviderZone",
Name = "Payment Provider Zone",
Description = "#PaymentProviderZoneDescription",
Thumbnail = "Folder.png",
Icon = umbracoVersionMajor > 6 ? "icon-uwebshop-archive" : "map-pin.png",
SortOrder = 1,
AllowedContentTypes = new List<ContentTypeSort>(),
AllowedTemplates = new List<ITemplate>(),
PropertyGroups = new PropertyGroupCollection(new List<PropertyGroup>()),};
contentTypeList.Add(uwbsPaymentProviderZoneContentType);

if (uwbsPaymentProviderZoneContentType.PropertyTypes.All(p => p.Alias != "zone")){
GetOrAddPropertyGroup(uwbsPaymentProviderZoneContentType, "Global").PropertyTypes.Add(new PropertyType(ZonesDataTypeDef) { Alias = "zone", Name = "#Zone", Description = "#ZoneDescription",});
}


var uwbsPaymentProviderZoneSectionContentType = contentTypeService.GetContentType("uwbsPaymentProviderZoneSection") ?? new ContentType(-1) {
Alias = "uwbsPaymentProviderZoneSection",
Name = "Payment Provider Zone Section",
Description = "#PaymentProviderZoneSectionDescription",
Thumbnail = "Folder.png",
Icon = umbracoVersionMajor > 6 ? "icon-uwebshop-globe" : "globe-green.png",
SortOrder = 1,
AllowedContentTypes = new List<ContentTypeSort>(),
AllowedTemplates = new List<ITemplate>(),
PropertyGroups = new PropertyGroupCollection(new List<PropertyGroup>()),};
contentTypeList.Add(uwbsPaymentProviderZoneSectionContentType);



var uwbsProductContentType = contentTypeService.GetContentType("uwbsProduct") ?? new ContentType(-1) {
Alias = "uwbsProduct",
Name = "Product",
Description = "#ProductDescription",
Thumbnail = "Folder.png",
Icon = umbracoVersionMajor > 6 ? "icon-uwebshop-box" : "box-label.png",
SortOrder = 1,
AllowedContentTypes = new List<ContentTypeSort>(),
AllowedTemplates = new List<ITemplate>(),
PropertyGroups = new PropertyGroupCollection(new List<PropertyGroup>()),};
contentTypeList.Add(uwbsProductContentType);

if (uwbsProductContentType.PropertyTypes.All(p => p.Alias != "title")){
GetOrAddPropertyGroup(uwbsProductContentType, "Global").PropertyTypes.Add(new PropertyType(StringDataTypeDef) { Alias = "title", Name = "#Title", Description = "#TitleDescription",Mandatory = true, });
}
if (uwbsProductContentType.PropertyTypes.All(p => p.Alias != "url")){
GetOrAddPropertyGroup(uwbsProductContentType, "Global").PropertyTypes.Add(new PropertyType(StringDataTypeDef) { Alias = "url", Name = "#Url", Description = "#UrlDescription",Mandatory = true, });
}
if (uwbsProductContentType.PropertyTypes.All(p => p.Alias != "sku")){
GetOrAddPropertyGroup(uwbsProductContentType, "Global").PropertyTypes.Add(new PropertyType(StringDataTypeDef) { Alias = "sku", Name = "#SKU", Description = "#SKUDescription",});
}
if (uwbsProductContentType.PropertyTypes.All(p => p.Alias != "metaDescription")){
GetOrAddPropertyGroup(uwbsProductContentType, "Global").PropertyTypes.Add(new PropertyType(TextboxMultipleDataTypeDef) { Alias = "metaDescription", Name = "#MetaDescription", Description = "#MetaDescriptionDescription",});
}
if (uwbsProductContentType.PropertyTypes.All(p => p.Alias != "metaTags")){
GetOrAddPropertyGroup(uwbsProductContentType, "Global").PropertyTypes.Add(new PropertyType(TagsDataTypeDef) { Alias = "metaTags", Name = "#Tags", Description = "#TagsDescription",});
}
if (uwbsProductContentType.PropertyTypes.All(p => p.Alias != "categories")){
GetOrAddPropertyGroup(uwbsProductContentType, "Global").PropertyTypes.Add(new PropertyType(MultiContentPickerCategoriesDataTypeDef) { Alias = "categories", Name = "#ProductCategories", Description = "#ProductCategoriesDescription",});
}
if (uwbsProductContentType.PropertyTypes.All(p => p.Alias != "disable")){
GetOrAddPropertyGroup(uwbsProductContentType, "Global").PropertyTypes.Add(new PropertyType(TrueFalseDataTypeDef) { Alias = "disable", Name = "#Disable", Description = "#DisableDescription",});
}
if (uwbsProductContentType.PropertyTypes.All(p => p.Alias != "description")){
GetOrAddPropertyGroup(uwbsProductContentType, "Details").PropertyTypes.Add(new PropertyType(RichTextDataTypeDef) { Alias = "description", Name = "#Description", Description = "#DescriptionDescription",});
}
if (uwbsProductContentType.PropertyTypes.All(p => p.Alias != "images")){
GetOrAddPropertyGroup(uwbsProductContentType, "Details").PropertyTypes.Add(new PropertyType(MultiContentPickerImagesDataTypeDef) { Alias = "images", Name = "#Images", Description = "#ImagesDescription",});
}
if (uwbsProductContentType.PropertyTypes.All(p => p.Alias != "files")){
GetOrAddPropertyGroup(uwbsProductContentType, "Details").PropertyTypes.Add(new PropertyType(MultiContentPickerFilesDataTypeDef) { Alias = "files", Name = "#Files", Description = "#FilesDescription",});
}
if (uwbsProductContentType.PropertyTypes.All(p => p.Alias != "length")){
GetOrAddPropertyGroup(uwbsProductContentType, "Details").PropertyTypes.Add(new PropertyType(NumericDataTypeDef) { Alias = "length", Name = "#Length", Description = "#LengthDescription",});
}
if (uwbsProductContentType.PropertyTypes.All(p => p.Alias != "width")){
GetOrAddPropertyGroup(uwbsProductContentType, "Details").PropertyTypes.Add(new PropertyType(NumericDataTypeDef) { Alias = "width", Name = "#Width", Description = "#WidthDescription",});
}
if (uwbsProductContentType.PropertyTypes.All(p => p.Alias != "height")){
GetOrAddPropertyGroup(uwbsProductContentType, "Details").PropertyTypes.Add(new PropertyType(NumericDataTypeDef) { Alias = "height", Name = "#Height", Description = "#HeightDescription",});
}
if (uwbsProductContentType.PropertyTypes.All(p => p.Alias != "weight")){
GetOrAddPropertyGroup(uwbsProductContentType, "Details").PropertyTypes.Add(new PropertyType(NumericDataTypeDef) { Alias = "weight", Name = "#Weight", Description = "#WeightDescription",});
}
if (uwbsProductContentType.PropertyTypes.All(p => p.Alias != "price")){
GetOrAddPropertyGroup(uwbsProductContentType, "Price").PropertyTypes.Add(new PropertyType(PriceDataTypeDef) { Alias = "price", Name = "#Price", Description = "#PriceDescription",});
}
if (uwbsProductContentType.PropertyTypes.All(p => p.Alias != "ranges")){
GetOrAddPropertyGroup(uwbsProductContentType, "Price").PropertyTypes.Add(new PropertyType(RangesDataTypeDef) { Alias = "ranges", Name = "#Ranges", Description = "#RangesDescription",});
}
if (uwbsProductContentType.PropertyTypes.All(p => p.Alias != "vat")){
GetOrAddPropertyGroup(uwbsProductContentType, "Price").PropertyTypes.Add(new PropertyType(VatPickerDataTypeDef) { Alias = "vat", Name = "#VAT", Description = "#VatDescription",});
}
if (uwbsProductContentType.PropertyTypes.All(p => p.Alias != "stock")){
GetOrAddPropertyGroup(uwbsProductContentType, "Price").PropertyTypes.Add(new PropertyType(StockDataTypeDef) { Alias = "stock", Name = "#Stock", Description = "#StockDescription",});
}
if (uwbsProductContentType.PropertyTypes.All(p => p.Alias != "ordered")){
GetOrAddPropertyGroup(uwbsProductContentType, "Price").PropertyTypes.Add(new PropertyType(OrderedCountDataTypeDef) { Alias = "ordered", Name = "#Ordered", Description = "#OrderedDescription",});
}
if (uwbsProductContentType.PropertyTypes.All(p => p.Alias != "stockStatus")){
GetOrAddPropertyGroup(uwbsProductContentType, "Price").PropertyTypes.Add(new PropertyType(EnableDisableDataTypeDef) { Alias = "stockStatus", Name = "#StockStatus", Description = "#StockStatusDescription",});
}
if (uwbsProductContentType.PropertyTypes.All(p => p.Alias != "backorderStatus")){
GetOrAddPropertyGroup(uwbsProductContentType, "Price").PropertyTypes.Add(new PropertyType(EnableDisableDataTypeDef) { Alias = "backorderStatus", Name = "#BackorderStatus", Description = "#BackorderStatusDescription",});
}
if (uwbsProductContentType.PropertyTypes.All(p => p.Alias != "useVariantStock")){
GetOrAddPropertyGroup(uwbsProductContentType, "Price").PropertyTypes.Add(new PropertyType(TrueFalseDataTypeDef) { Alias = "useVariantStock", Name = "#UseVariantStock", Description = "#UseVariantStockDescription",});
}


var uwbsProductRepositoryContentType = contentTypeService.GetContentType("uwbsProductRepository") ?? new ContentType(-1) {
Alias = "uwbsProductRepository",
Name = "Product Repository",
Description = "#ProductRepositoryDescription",
Thumbnail = "Folder.png",
Icon = umbracoVersionMajor > 6 ? "icon-uwebshop-barcode" : "box-search-result.png",
SortOrder = 1,
AllowedContentTypes = new List<ContentTypeSort>(),
AllowedTemplates = new List<ITemplate>(),
PropertyGroups = new PropertyGroupCollection(new List<PropertyGroup>()),};
contentTypeList.Add(uwbsProductRepositoryContentType);

if (uwbsProductRepositoryContentType.PropertyTypes.All(p => p.Alias != "productOverview")){
GetOrAddPropertyGroup(uwbsProductRepositoryContentType, "Global").PropertyTypes.Add(new PropertyType(ProductOverviewDataTypeDef) { Alias = "productOverview", Name = "#ProductOverview", Description = "#ProductOverviewDescription",});
}


var uwbsProductVariantContentType = contentTypeService.GetContentType("uwbsProductVariant") ?? new ContentType(-1) {
Alias = "uwbsProductVariant",
Name = "Product Variant",
Description = "#ProductVariantDescription",
Thumbnail = "Folder.png",
Icon = umbracoVersionMajor > 6 ? "icon-uwebshop-link" : "magnet-small.png",
SortOrder = 1,
AllowedContentTypes = new List<ContentTypeSort>(),
AllowedTemplates = new List<ITemplate>(),
PropertyGroups = new PropertyGroupCollection(new List<PropertyGroup>()),};
contentTypeList.Add(uwbsProductVariantContentType);

if (uwbsProductVariantContentType.PropertyTypes.All(p => p.Alias != "title")){
GetOrAddPropertyGroup(uwbsProductVariantContentType, "Global").PropertyTypes.Add(new PropertyType(StringDataTypeDef) { Alias = "title", Name = "#Title", Description = "#TitleDescription",Mandatory = true, });
}
if (uwbsProductVariantContentType.PropertyTypes.All(p => p.Alias != "sku")){
GetOrAddPropertyGroup(uwbsProductVariantContentType, "Global").PropertyTypes.Add(new PropertyType(StringDataTypeDef) { Alias = "sku", Name = "#SKU", Description = "#SKUDescription",});
}
if (uwbsProductVariantContentType.PropertyTypes.All(p => p.Alias != "disable")){
GetOrAddPropertyGroup(uwbsProductVariantContentType, "Global").PropertyTypes.Add(new PropertyType(TrueFalseDataTypeDef) { Alias = "disable", Name = "#Disable", Description = "#DisableDescription",});
}
if (uwbsProductVariantContentType.PropertyTypes.All(p => p.Alias != "description")){
GetOrAddPropertyGroup(uwbsProductVariantContentType, "Details").PropertyTypes.Add(new PropertyType(RichTextDataTypeDef) { Alias = "description", Name = "#Description", Description = "#DescriptionDescription",});
}
if (uwbsProductVariantContentType.PropertyTypes.All(p => p.Alias != "length")){
GetOrAddPropertyGroup(uwbsProductVariantContentType, "Details").PropertyTypes.Add(new PropertyType(NumericDataTypeDef) { Alias = "length", Name = "#Length", Description = "#LengthDescription",});
}
if (uwbsProductVariantContentType.PropertyTypes.All(p => p.Alias != "width")){
GetOrAddPropertyGroup(uwbsProductVariantContentType, "Details").PropertyTypes.Add(new PropertyType(NumericDataTypeDef) { Alias = "width", Name = "#Width", Description = "#WidthDescription",});
}
if (uwbsProductVariantContentType.PropertyTypes.All(p => p.Alias != "height")){
GetOrAddPropertyGroup(uwbsProductVariantContentType, "Details").PropertyTypes.Add(new PropertyType(NumericDataTypeDef) { Alias = "height", Name = "#Height", Description = "#HeightDescription",});
}
if (uwbsProductVariantContentType.PropertyTypes.All(p => p.Alias != "weight")){
GetOrAddPropertyGroup(uwbsProductVariantContentType, "Details").PropertyTypes.Add(new PropertyType(NumericDataTypeDef) { Alias = "weight", Name = "#Weight", Description = "#WeightDescription",});
}
if (uwbsProductVariantContentType.PropertyTypes.All(p => p.Alias != "price")){
GetOrAddPropertyGroup(uwbsProductVariantContentType, "Price").PropertyTypes.Add(new PropertyType(PriceDataTypeDef) { Alias = "price", Name = "#Price", Description = "#PriceDescription",});
}
if (uwbsProductVariantContentType.PropertyTypes.All(p => p.Alias != "ranges")){
GetOrAddPropertyGroup(uwbsProductVariantContentType, "Price").PropertyTypes.Add(new PropertyType(RangesDataTypeDef) { Alias = "ranges", Name = "#Ranges", Description = "#RangesDescription",});
}
if (uwbsProductVariantContentType.PropertyTypes.All(p => p.Alias != "stock")){
GetOrAddPropertyGroup(uwbsProductVariantContentType, "Price").PropertyTypes.Add(new PropertyType(StockDataTypeDef) { Alias = "stock", Name = "#Stock", Description = "#StockDescription",});
}
if (uwbsProductVariantContentType.PropertyTypes.All(p => p.Alias != "ordered")){
GetOrAddPropertyGroup(uwbsProductVariantContentType, "Price").PropertyTypes.Add(new PropertyType(OrderedCountDataTypeDef) { Alias = "ordered", Name = "#Ordered", Description = "#OrderedDescription",});
}
if (uwbsProductVariantContentType.PropertyTypes.All(p => p.Alias != "stockStatus")){
GetOrAddPropertyGroup(uwbsProductVariantContentType, "Price").PropertyTypes.Add(new PropertyType(EnableDisableDataTypeDef) { Alias = "stockStatus", Name = "#StockStatus", Description = "#StockStatusDescription",});
}
if (uwbsProductVariantContentType.PropertyTypes.All(p => p.Alias != "backorderStatus")){
GetOrAddPropertyGroup(uwbsProductVariantContentType, "Price").PropertyTypes.Add(new PropertyType(EnableDisableDataTypeDef) { Alias = "backorderStatus", Name = "#BackorderStatus", Description = "#BackorderStatusDescription",});
}


var uwbsProductVariantGroupContentType = contentTypeService.GetContentType("uwbsProductVariantGroup") ?? new ContentType(-1) {
Alias = "uwbsProductVariantGroup",
Name = "Product Variant Group",
Description = "#ProductVariantGroupDescription",
Thumbnail = "Folder.png",
Icon = umbracoVersionMajor > 6 ? "icon-uwebshop-folder" : "folder-open-table.png",
SortOrder = 1,
AllowedContentTypes = new List<ContentTypeSort>(),
AllowedTemplates = new List<ITemplate>(),
PropertyGroups = new PropertyGroupCollection(new List<PropertyGroup>()),};
contentTypeList.Add(uwbsProductVariantGroupContentType);

if (uwbsProductVariantGroupContentType.PropertyTypes.All(p => p.Alias != "title")){
GetOrAddPropertyGroup(uwbsProductVariantGroupContentType, "Global").PropertyTypes.Add(new PropertyType(StringDataTypeDef) { Alias = "title", Name = "#Title", Description = "#TitleDescription",Mandatory = true, });
}
if (uwbsProductVariantGroupContentType.PropertyTypes.All(p => p.Alias != "required")){
GetOrAddPropertyGroup(uwbsProductVariantGroupContentType, "Global").PropertyTypes.Add(new PropertyType(TrueFalseDataTypeDef) { Alias = "required", Name = "#RequiredVariantGroup", Description = "#RequiredVariantGroupDescription",});
}
if (uwbsProductVariantGroupContentType.PropertyTypes.All(p => p.Alias != "description")){
GetOrAddPropertyGroup(uwbsProductVariantGroupContentType, "Global").PropertyTypes.Add(new PropertyType(RichTextDataTypeDef) { Alias = "description", Name = "#Description", Description = "#DescriptionDescription",});
}


var uwbsSettingsContentType = contentTypeService.GetContentType("uwbsSettings") ?? new ContentType(-1) {
Alias = "uwbsSettings",
Name = "Settings",
Description = "#SettingsSectionDescription",
Thumbnail = "Folder.png",
Icon = umbracoVersionMajor > 6 ? "icon-uwebshop-briefcase" : "toolbox.png",
SortOrder = 1,
AllowedContentTypes = new List<ContentTypeSort>(),
AllowedTemplates = new List<ITemplate>(),
PropertyGroups = new PropertyGroupCollection(new List<PropertyGroup>()),};
contentTypeList.Add(uwbsSettingsContentType);

if (uwbsSettingsContentType.PropertyTypes.All(p => p.Alias != "includingVat")){
GetOrAddPropertyGroup(uwbsSettingsContentType, "Global").PropertyTypes.Add(new PropertyType(TrueFalseDataTypeDef) { Alias = "includingVat", Name = "#IncludingVat", Description = "#IncludingVatSettingDescription",});
}
if (uwbsSettingsContentType.PropertyTypes.All(p => p.Alias != "lowercaseUrls")){
GetOrAddPropertyGroup(uwbsSettingsContentType, "Global").PropertyTypes.Add(new PropertyType(TrueFalseDataTypeDef) { Alias = "lowercaseUrls", Name = "#LowercaseUrls", Description = "#LowercaseUrlsSettingDescription",});
}
if (uwbsSettingsContentType.PropertyTypes.All(p => p.Alias != "incompleteOrderLifetime")){
GetOrAddPropertyGroup(uwbsSettingsContentType, "Global").PropertyTypes.Add(new PropertyType(StringDataTypeDef) { Alias = "incompleteOrderLifetime", Name = "#IncompleteOrderLifetime", Description = "#OrderLifetimeSettingDescription",Mandatory = true, });
}


var uwbsShippingProviderContentType = contentTypeService.GetContentType("uwbsShippingProvider") ?? new ContentType(-1) {
Alias = "uwbsShippingProvider",
Name = "Shipping Provider",
Description = "#ShippingProviderDescription",
Thumbnail = "Folder.png",
Icon = umbracoVersionMajor > 6 ? "icon-uwebshop-truck" : "baggage-cart-box-label.png",
SortOrder = 1,
AllowedContentTypes = new List<ContentTypeSort>(),
AllowedTemplates = new List<ITemplate>(),
PropertyGroups = new PropertyGroupCollection(new List<PropertyGroup>()),};
contentTypeList.Add(uwbsShippingProviderContentType);

if (uwbsShippingProviderContentType.PropertyTypes.All(p => p.Alias != "title")){
GetOrAddPropertyGroup(uwbsShippingProviderContentType, "Global").PropertyTypes.Add(new PropertyType(StringDataTypeDef) { Alias = "title", Name = "#Title", Description = "#TitleDescription",});
}
if (uwbsShippingProviderContentType.PropertyTypes.All(p => p.Alias != "description")){
GetOrAddPropertyGroup(uwbsShippingProviderContentType, "Global").PropertyTypes.Add(new PropertyType(RichTextDataTypeDef) { Alias = "description", Name = "#Description", Description = "#DescriptionDescription",});
}
if (uwbsShippingProviderContentType.PropertyTypes.All(p => p.Alias != "image")){
GetOrAddPropertyGroup(uwbsShippingProviderContentType, "Global").PropertyTypes.Add(new PropertyType(MediaPickerDataTypeDef) { Alias = "image", Name = "#Image", Description = "#ImageDescription",});
}
if (uwbsShippingProviderContentType.PropertyTypes.All(p => p.Alias != "type")){
GetOrAddPropertyGroup(uwbsShippingProviderContentType, "Details").PropertyTypes.Add(new PropertyType(ShippingProviderTypeDataTypeDef) { Alias = "type", Name = "#ShippingProviderType", Description = "#ShippingProviderTypeDescription",});
}
if (uwbsShippingProviderContentType.PropertyTypes.All(p => p.Alias != "rangeType")){
GetOrAddPropertyGroup(uwbsShippingProviderContentType, "Details").PropertyTypes.Add(new PropertyType(ShippingProviderRangeTypeDataTypeDef) { Alias = "rangeType", Name = "#ShippingProviderRangeType", Description = "#ShippingProviderRangeTypeDescription",});
}
if (uwbsShippingProviderContentType.PropertyTypes.All(p => p.Alias != "rangeStart")){
GetOrAddPropertyGroup(uwbsShippingProviderContentType, "Details").PropertyTypes.Add(new PropertyType(NumericDataTypeDef) { Alias = "rangeStart", Name = "#RangeStart", Description = "#RangeStartDescription",});
}
if (uwbsShippingProviderContentType.PropertyTypes.All(p => p.Alias != "rangeEnd")){
GetOrAddPropertyGroup(uwbsShippingProviderContentType, "Details").PropertyTypes.Add(new PropertyType(NumericDataTypeDef) { Alias = "rangeEnd", Name = "#RangeEnd", Description = "#RangeEndDescription",});
}
if (uwbsShippingProviderContentType.PropertyTypes.All(p => p.Alias != "overrule")){
GetOrAddPropertyGroup(uwbsShippingProviderContentType, "Details").PropertyTypes.Add(new PropertyType(TrueFalseDataTypeDef) { Alias = "overrule", Name = "#Overrule", Description = "#OverruleDescription",});
}
if (uwbsShippingProviderContentType.PropertyTypes.All(p => p.Alias != "zone")){
GetOrAddPropertyGroup(uwbsShippingProviderContentType, "Details").PropertyTypes.Add(new PropertyType(MultiContentPickerShippingZonesDataTypeDef) { Alias = "zone", Name = "#Zone", Description = "#ZoneDescription",});
}
if (uwbsShippingProviderContentType.PropertyTypes.All(p => p.Alias != "testMode")){
GetOrAddPropertyGroup(uwbsShippingProviderContentType, "Details").PropertyTypes.Add(new PropertyType(EnableDisableDataTypeDef) { Alias = "testMode", Name = "#TestMode", Description = "#TestModeDescription",});
}


var uwbsShippingProviderMethodContentType = contentTypeService.GetContentType("uwbsShippingProviderMethod") ?? new ContentType(-1) {
Alias = "uwbsShippingProviderMethod",
Name = "Shipping Provider Method",
Description = "#ShippingProviderMethodDescription",
Thumbnail = "Folder.png",
Icon = umbracoVersionMajor > 6 ? "icon-uwebshop-link" : "magnet-small.png",
SortOrder = 1,
AllowedContentTypes = new List<ContentTypeSort>(),
AllowedTemplates = new List<ITemplate>(),
PropertyGroups = new PropertyGroupCollection(new List<PropertyGroup>()),};
contentTypeList.Add(uwbsShippingProviderMethodContentType);

if (uwbsShippingProviderMethodContentType.PropertyTypes.All(p => p.Alias != "disable")){
GetOrAddPropertyGroup(uwbsShippingProviderMethodContentType, "Global").PropertyTypes.Add(new PropertyType(TrueFalseDataTypeDef) { Alias = "disable", Name = "#Disable", Description = "#DisableDescription",});
}
if (uwbsShippingProviderMethodContentType.PropertyTypes.All(p => p.Alias != "title")){
GetOrAddPropertyGroup(uwbsShippingProviderMethodContentType, "Global").PropertyTypes.Add(new PropertyType(StringDataTypeDef) { Alias = "title", Name = "#Title", Description = "#TitleDescription",});
}
if (uwbsShippingProviderMethodContentType.PropertyTypes.All(p => p.Alias != "description")){
GetOrAddPropertyGroup(uwbsShippingProviderMethodContentType, "Global").PropertyTypes.Add(new PropertyType(RichTextDataTypeDef) { Alias = "description", Name = "#Description", Description = "#DescriptionDescription",});
}
if (uwbsShippingProviderMethodContentType.PropertyTypes.All(p => p.Alias != "image")){
GetOrAddPropertyGroup(uwbsShippingProviderMethodContentType, "Global").PropertyTypes.Add(new PropertyType(MediaPickerDataTypeDef) { Alias = "image", Name = "#Image", Description = "#ImageDescription",});
}
if (uwbsShippingProviderMethodContentType.PropertyTypes.All(p => p.Alias != "price")){
GetOrAddPropertyGroup(uwbsShippingProviderMethodContentType, "Price").PropertyTypes.Add(new PropertyType(PriceDataTypeDef) { Alias = "price", Name = "#Price", Description = "#PriceDescription",});
}
if (uwbsShippingProviderMethodContentType.PropertyTypes.All(p => p.Alias != "vat")){
GetOrAddPropertyGroup(uwbsShippingProviderMethodContentType, "Price").PropertyTypes.Add(new PropertyType(VatPickerDataTypeDef) { Alias = "vat", Name = "#VAT", Description = "#VatDescription",});
}


var uwbsShippingProviderRepositoryContentType = contentTypeService.GetContentType("uwbsShippingProviderRepository") ?? new ContentType(-1) {
Alias = "uwbsShippingProviderRepository",
Name = "Shipping Provider Repository",
Description = "#ShippingProviderRepositoryDescription",
Thumbnail = "Folder.png",
Icon = umbracoVersionMajor > 6 ? "icon-uwebshop-plane" : "truck-box-label.png",
SortOrder = 1,
AllowedContentTypes = new List<ContentTypeSort>(),
AllowedTemplates = new List<ITemplate>(),
PropertyGroups = new PropertyGroupCollection(new List<PropertyGroup>()),};
contentTypeList.Add(uwbsShippingProviderRepositoryContentType);



var uwbsShippingProviderSectionContentType = contentTypeService.GetContentType("uwbsShippingProviderSection") ?? new ContentType(-1) {
Alias = "uwbsShippingProviderSection",
Name = "Shipping Provider Section",
Description = "#ShippingProviderSectionDescription",
Thumbnail = "Folder.png",
Icon = umbracoVersionMajor > 6 ? "icon-uwebshop-folder" : "baggage-cart.png",
SortOrder = 1,
AllowedContentTypes = new List<ContentTypeSort>(),
AllowedTemplates = new List<ITemplate>(),
PropertyGroups = new PropertyGroupCollection(new List<PropertyGroup>()),};
contentTypeList.Add(uwbsShippingProviderSectionContentType);



var uwbsShippingProviderZoneContentType = contentTypeService.GetContentType("uwbsShippingProviderZone") ?? new ContentType(-1) {
Alias = "uwbsShippingProviderZone",
Name = "Shipping Provider Zone",
Description = "#ShippingProviderZoneDescription",
Thumbnail = "Folder.png",
Icon = umbracoVersionMajor > 6 ? "icon-uwebshop-archive" : "map-pin.png",
SortOrder = 1,
AllowedContentTypes = new List<ContentTypeSort>(),
AllowedTemplates = new List<ITemplate>(),
PropertyGroups = new PropertyGroupCollection(new List<PropertyGroup>()),};
contentTypeList.Add(uwbsShippingProviderZoneContentType);

if (uwbsShippingProviderZoneContentType.PropertyTypes.All(p => p.Alias != "zone")){
GetOrAddPropertyGroup(uwbsShippingProviderZoneContentType, "Global").PropertyTypes.Add(new PropertyType(ZonesDataTypeDef) { Alias = "zone", Name = "#Zone", Description = "#ZoneDescription",});
}


var uwbsShippingProviderZoneSectionContentType = contentTypeService.GetContentType("uwbsShippingProviderZoneSection") ?? new ContentType(-1) {
Alias = "uwbsShippingProviderZoneSection",
Name = "Shipping Provider Zone Section",
Description = "#ShippingProviderZoneSectionDescription",
Thumbnail = "Folder.png",
Icon = umbracoVersionMajor > 6 ? "icon-uwebshop-globe" : "globe-green.png",
SortOrder = 1,
AllowedContentTypes = new List<ContentTypeSort>(),
AllowedTemplates = new List<ITemplate>(),
PropertyGroups = new PropertyGroupCollection(new List<PropertyGroup>()),};
contentTypeList.Add(uwbsShippingProviderZoneSectionContentType);



var uwbsStoreContentType = contentTypeService.GetContentType("uwbsStore") ?? new ContentType(-1) {
Alias = "uwbsStore",
Name = "Store",
Description = "#StoreDescription",
Thumbnail = "Folder.png",
Icon = umbracoVersionMajor > 6 ? "icon-uwebshop-store" : "store.png",
SortOrder = 1,
AllowedContentTypes = new List<ContentTypeSort>(),
AllowedTemplates = new List<ITemplate>(),
PropertyGroups = new PropertyGroupCollection(new List<PropertyGroup>()),};
contentTypeList.Add(uwbsStoreContentType);

if (uwbsStoreContentType.PropertyTypes.All(p => p.Alias != "storeCulture")){
GetOrAddPropertyGroup(uwbsStoreContentType, "Global").PropertyTypes.Add(new PropertyType(CulturesDataTypeDef) { Alias = "storeCulture", Name = "#StoreCulture", Description = "#StoreCultureDescription",});
}
if (uwbsStoreContentType.PropertyTypes.All(p => p.Alias != "countryCode")){
GetOrAddPropertyGroup(uwbsStoreContentType, "Global").PropertyTypes.Add(new PropertyType(CountriesDataTypeDef) { Alias = "countryCode", Name = "#CountryCode", Description = "#CountryCodeDescription",});
}
if (uwbsStoreContentType.PropertyTypes.All(p => p.Alias != "currencies")){
GetOrAddPropertyGroup(uwbsStoreContentType, "Global").PropertyTypes.Add(new PropertyType(CurrenciesDataTypeDef) { Alias = "currencies", Name = "#Currencies", Description = "#CurrenciesDescription",});
}
if (uwbsStoreContentType.PropertyTypes.All(p => p.Alias != "globalVat")){
GetOrAddPropertyGroup(uwbsStoreContentType, "Global").PropertyTypes.Add(new PropertyType(VatPickerDataTypeDef) { Alias = "globalVat", Name = "#GlobalVat", Description = "#GlobalVatDescription",});
}
if (uwbsStoreContentType.PropertyTypes.All(p => p.Alias != "orderNumberPrefix")){
GetOrAddPropertyGroup(uwbsStoreContentType, "Global").PropertyTypes.Add(new PropertyType(StringDataTypeDef) { Alias = "orderNumberPrefix", Name = "#OrderNumberPrefix", Description = "#OrderNumberPrefixDescription",});
}
if (uwbsStoreContentType.PropertyTypes.All(p => p.Alias != "orderNumberTemplate")){
GetOrAddPropertyGroup(uwbsStoreContentType, "Global").PropertyTypes.Add(new PropertyType(StringDataTypeDef) { Alias = "orderNumberTemplate", Name = "#OrderNumberTemplate", Description = "#OrderNumberTemplateDescription",});
}
if (uwbsStoreContentType.PropertyTypes.All(p => p.Alias != "orderNumberStartNumber")){
GetOrAddPropertyGroup(uwbsStoreContentType, "Global").PropertyTypes.Add(new PropertyType(NumericDataTypeDef) { Alias = "orderNumberStartNumber", Name = "#OrderNumberStartNumber", Description = "#OrderNumberStartNumberDescription",});
}
if (uwbsStoreContentType.PropertyTypes.All(p => p.Alias != "enableStock")){
GetOrAddPropertyGroup(uwbsStoreContentType, "Global").PropertyTypes.Add(new PropertyType(EnableDisableDataTypeDef) { Alias = "enableStock", Name = "#EnableStock", Description = "#EnableStockDescription",});
}
if (uwbsStoreContentType.PropertyTypes.All(p => p.Alias != "defaultUseVariantStock")){
GetOrAddPropertyGroup(uwbsStoreContentType, "Global").PropertyTypes.Add(new PropertyType(EnableDisableDataTypeDef) { Alias = "defaultUseVariantStock", Name = "#UseVariantStock", Description = "#UseVariantStockDescription",});
}
if (uwbsStoreContentType.PropertyTypes.All(p => p.Alias != "defaultCountdownEnabled")){
GetOrAddPropertyGroup(uwbsStoreContentType, "Global").PropertyTypes.Add(new PropertyType(EnableDisableDataTypeDef) { Alias = "defaultCountdownEnabled", Name = "#CountdownEnabled", Description = "#CountdownEnabledDescription",});
}
if (uwbsStoreContentType.PropertyTypes.All(p => p.Alias != "storeStock")){
GetOrAddPropertyGroup(uwbsStoreContentType, "Global").PropertyTypes.Add(new PropertyType(TrueFalseDataTypeDef) { Alias = "storeStock", Name = "#StoreStock", Description = "#StoreStockDescription",});
}
if (uwbsStoreContentType.PropertyTypes.All(p => p.Alias != "useBackorders")){
GetOrAddPropertyGroup(uwbsStoreContentType, "Global").PropertyTypes.Add(new PropertyType(EnableDisableDataTypeDef) { Alias = "useBackorders", Name = "#UseBackorders", Description = "#UseBackordersDescription",});
}
if (uwbsStoreContentType.PropertyTypes.All(p => p.Alias != "enableTestmode")){
GetOrAddPropertyGroup(uwbsStoreContentType, "Global").PropertyTypes.Add(new PropertyType(EnableDisableDataTypeDef) { Alias = "enableTestmode", Name = "#EnableTestmode", Description = "#EnableTestmodeDescription",});
}
if (uwbsStoreContentType.PropertyTypes.All(p => p.Alias != "storeEmailFrom")){
GetOrAddPropertyGroup(uwbsStoreContentType, "Email").PropertyTypes.Add(new PropertyType(StringDataTypeDef) { Alias = "storeEmailFrom", Name = "#StoreEmailFrom", Description = "#StoreEmailFromDescription",});
}
if (uwbsStoreContentType.PropertyTypes.All(p => p.Alias != "storeEmailFromName")){
GetOrAddPropertyGroup(uwbsStoreContentType, "Email").PropertyTypes.Add(new PropertyType(StringDataTypeDef) { Alias = "storeEmailFromName", Name = "#StoreEmailFromName", Description = "#StoreEmailFromNameDescription",});
}
if (uwbsStoreContentType.PropertyTypes.All(p => p.Alias != "storeEmailTo")){
GetOrAddPropertyGroup(uwbsStoreContentType, "Email").PropertyTypes.Add(new PropertyType(StringDataTypeDef) { Alias = "storeEmailTo", Name = "#StoreEmailTo", Description = "#StoreEmailToDescription",});
}
if (uwbsStoreContentType.PropertyTypes.All(p => p.Alias != "accountEmailCreated")){
GetOrAddPropertyGroup(uwbsStoreContentType, "Email").PropertyTypes.Add(new PropertyType(ContentPickerDataTypeDef) { Alias = "accountEmailCreated", Name = "#AccountEmailCreated", Description = "#AccountEmailCreatedDescription",});
}
if (uwbsStoreContentType.PropertyTypes.All(p => p.Alias != "accountForgotPassword")){
GetOrAddPropertyGroup(uwbsStoreContentType, "Email").PropertyTypes.Add(new PropertyType(ContentPickerDataTypeDef) { Alias = "accountForgotPassword", Name = "#AccountForgotPassword", Description = "#AccountForgotPasswordDescription",});
}
if (uwbsStoreContentType.PropertyTypes.All(p => p.Alias != "confirmationEmailStore")){
GetOrAddPropertyGroup(uwbsStoreContentType, "Email").PropertyTypes.Add(new PropertyType(ContentPickerDataTypeDef) { Alias = "confirmationEmailStore", Name = "#ConfirmationEmailStore", Description = "#ConfirmationEmailStoreDescription",});
}
if (uwbsStoreContentType.PropertyTypes.All(p => p.Alias != "confirmationEmailCustomer")){
GetOrAddPropertyGroup(uwbsStoreContentType, "Email").PropertyTypes.Add(new PropertyType(ContentPickerDataTypeDef) { Alias = "confirmationEmailCustomer", Name = "#ConfirmationEmailCustomer", Description = "#ConfirmationEmailCustomerDescription",});
}
if (uwbsStoreContentType.PropertyTypes.All(p => p.Alias != "onlinePaymentEmailStore")){
GetOrAddPropertyGroup(uwbsStoreContentType, "Email").PropertyTypes.Add(new PropertyType(ContentPickerDataTypeDef) { Alias = "onlinePaymentEmailStore", Name = "#OnlinePaymentEmailStore", Description = "#OnlinePaymentEmailStoreDescription",});
}
if (uwbsStoreContentType.PropertyTypes.All(p => p.Alias != "onlinePaymentEmailCustomer")){
GetOrAddPropertyGroup(uwbsStoreContentType, "Email").PropertyTypes.Add(new PropertyType(ContentPickerDataTypeDef) { Alias = "onlinePaymentEmailCustomer", Name = "#OnlinePaymentEmailCustomer", Description = "#OnlinePaymentEmailCustomerDescription",});
}
if (uwbsStoreContentType.PropertyTypes.All(p => p.Alias != "offlinePaymentEmailStore")){
GetOrAddPropertyGroup(uwbsStoreContentType, "Email").PropertyTypes.Add(new PropertyType(ContentPickerDataTypeDef) { Alias = "offlinePaymentEmailStore", Name = "#OfflinePaymentEmailStore", Description = "#OfflinePaymentEmailStoreDescription",});
}
if (uwbsStoreContentType.PropertyTypes.All(p => p.Alias != "offlinePaymentEmailCustomer")){
GetOrAddPropertyGroup(uwbsStoreContentType, "Email").PropertyTypes.Add(new PropertyType(ContentPickerDataTypeDef) { Alias = "offlinePaymentEmailCustomer", Name = "#OfflinePaymentEmailCustomer", Description = "#OfflinePaymentEmailCustomerDescription",});
}
if (uwbsStoreContentType.PropertyTypes.All(p => p.Alias != "paymentFailedEmailStore")){
GetOrAddPropertyGroup(uwbsStoreContentType, "Email").PropertyTypes.Add(new PropertyType(ContentPickerDataTypeDef) { Alias = "paymentFailedEmailStore", Name = "#PaymentFailedEmailStore", Description = "#PaymentFailedEmailStoreDescription",});
}
if (uwbsStoreContentType.PropertyTypes.All(p => p.Alias != "paymentFailedEmailCustomer")){
GetOrAddPropertyGroup(uwbsStoreContentType, "Email").PropertyTypes.Add(new PropertyType(ContentPickerDataTypeDef) { Alias = "paymentFailedEmailCustomer", Name = "#PaymentFailedEmailCustomer", Description = "#PaymentFailedEmailCustomerDescription",});
}
if (uwbsStoreContentType.PropertyTypes.All(p => p.Alias != "dispatchedEmailStore")){
GetOrAddPropertyGroup(uwbsStoreContentType, "Email").PropertyTypes.Add(new PropertyType(ContentPickerDataTypeDef) { Alias = "dispatchedEmailStore", Name = "#DispatchedEmailStore", Description = "#DispatchedEmailStoreDescription",});
}
if (uwbsStoreContentType.PropertyTypes.All(p => p.Alias != "dispatchedEmailCustomer")){
GetOrAddPropertyGroup(uwbsStoreContentType, "Email").PropertyTypes.Add(new PropertyType(ContentPickerDataTypeDef) { Alias = "dispatchedEmailCustomer", Name = "#DispatchedEmailCustomer", Description = "#DispatchedEmailCustomerDescription",});
}
if (uwbsStoreContentType.PropertyTypes.All(p => p.Alias != "cancelEmailStore")){
GetOrAddPropertyGroup(uwbsStoreContentType, "Email").PropertyTypes.Add(new PropertyType(ContentPickerDataTypeDef) { Alias = "cancelEmailStore", Name = "#CancelEmailStore", Description = "#CancelEmailStoreDescription",});
}
if (uwbsStoreContentType.PropertyTypes.All(p => p.Alias != "cancelEmailCustomer")){
GetOrAddPropertyGroup(uwbsStoreContentType, "Email").PropertyTypes.Add(new PropertyType(ContentPickerDataTypeDef) { Alias = "cancelEmailCustomer", Name = "#CancelEmailCustomer", Description = "#CancelEmailCustomerDescription",});
}
if (uwbsStoreContentType.PropertyTypes.All(p => p.Alias != "closedEmailStore")){
GetOrAddPropertyGroup(uwbsStoreContentType, "Email").PropertyTypes.Add(new PropertyType(ContentPickerDataTypeDef) { Alias = "closedEmailStore", Name = "#ClosedEmailStore", Description = "#ClosedEmailStoreDescription",});
}
if (uwbsStoreContentType.PropertyTypes.All(p => p.Alias != "closedEmailCustomer")){
GetOrAddPropertyGroup(uwbsStoreContentType, "Email").PropertyTypes.Add(new PropertyType(ContentPickerDataTypeDef) { Alias = "closedEmailCustomer", Name = "#ClosedEmailCustomer", Description = "#ClosedEmailCustomerDescription",});
}
if (uwbsStoreContentType.PropertyTypes.All(p => p.Alias != "pendingEmailStore")){
GetOrAddPropertyGroup(uwbsStoreContentType, "Email").PropertyTypes.Add(new PropertyType(ContentPickerDataTypeDef) { Alias = "pendingEmailStore", Name = "#PendingEmailStore", Description = "#PendingEmailStoreDescription",});
}
if (uwbsStoreContentType.PropertyTypes.All(p => p.Alias != "pendingEmailCustomer")){
GetOrAddPropertyGroup(uwbsStoreContentType, "Email").PropertyTypes.Add(new PropertyType(ContentPickerDataTypeDef) { Alias = "pendingEmailCustomer", Name = "#PendingEmailCustomer", Description = "#PendingEmailCustomerDescription",});
}
if (uwbsStoreContentType.PropertyTypes.All(p => p.Alias != "temporaryOutOfStockEmailStore")){
GetOrAddPropertyGroup(uwbsStoreContentType, "Email").PropertyTypes.Add(new PropertyType(ContentPickerDataTypeDef) { Alias = "temporaryOutOfStockEmailStore", Name = "#TemporaryOutOfStockEmailStore", Description = "#TemporaryOutOfStockEmailStoreDescription",});
}
if (uwbsStoreContentType.PropertyTypes.All(p => p.Alias != "temporaryOutOfStockEmailCustomer")){
GetOrAddPropertyGroup(uwbsStoreContentType, "Email").PropertyTypes.Add(new PropertyType(ContentPickerDataTypeDef) { Alias = "temporaryOutOfStockEmailCustomer", Name = "#TemporaryOutOfStockEmailCustomer", Description = "#TemporaryOutOfStockEmailCustomerDescription",});
}
if (uwbsStoreContentType.PropertyTypes.All(p => p.Alias != "undeliverableEmailStore")){
GetOrAddPropertyGroup(uwbsStoreContentType, "Email").PropertyTypes.Add(new PropertyType(ContentPickerDataTypeDef) { Alias = "undeliverableEmailStore", Name = "#UndeliverableEmailStore", Description = "#UndeliverableEmailStoreDescription",});
}
if (uwbsStoreContentType.PropertyTypes.All(p => p.Alias != "undeliverableEmailCustomer")){
GetOrAddPropertyGroup(uwbsStoreContentType, "Email").PropertyTypes.Add(new PropertyType(ContentPickerDataTypeDef) { Alias = "undeliverableEmailCustomer", Name = "#UndeliverableEmailCustomer", Description = "#UndeliverableEmailCustomerDescription",});
}
if (uwbsStoreContentType.PropertyTypes.All(p => p.Alias != "returnEmailStore")){
GetOrAddPropertyGroup(uwbsStoreContentType, "Email").PropertyTypes.Add(new PropertyType(ContentPickerDataTypeDef) { Alias = "returnEmailStore", Name = "#ReturnEmailStore", Description = "#ReturnEmailStoreDescription",});
}
if (uwbsStoreContentType.PropertyTypes.All(p => p.Alias != "returnEmailCustomer")){
GetOrAddPropertyGroup(uwbsStoreContentType, "Email").PropertyTypes.Add(new PropertyType(ContentPickerDataTypeDef) { Alias = "returnEmailCustomer", Name = "#ReturnEmailCustomer", Description = "#ReturnEmailCustomerDescription",});
}


var uwbsStoreRepositoryContentType = contentTypeService.GetContentType("uwbsStoreRepository") ?? new ContentType(-1) {
Alias = "uwbsStoreRepository",
Name = "Store Repository",
Description = "#StoreRepositoryDescription",
Thumbnail = "Folder.png",
Icon = umbracoVersionMajor > 6 ? "icon-uwebshop-building" : "store-network.png",
SortOrder = 1,
AllowedContentTypes = new List<ContentTypeSort>(),
AllowedTemplates = new List<ITemplate>(),
PropertyGroups = new PropertyGroupCollection(new List<PropertyGroup>()),};
contentTypeList.Add(uwbsStoreRepositoryContentType);



var uWebshopContentType = contentTypeService.GetContentType("uWebshop") ?? new ContentType(-1) {
Alias = "uWebshop",
AllowedAsRoot = true,
Name = "uWebshop",
Description = "#uWebshopSectionDescription",
Thumbnail = "Folder.png",
Icon = umbracoVersionMajor > 6 ? "icon-uwebshop-uwebshoplogo" : "uwebshop16x16.png",
SortOrder = 1,
AllowedContentTypes = new List<ContentTypeSort>(),
AllowedTemplates = new List<ITemplate>(),
PropertyGroups = new PropertyGroupCollection(new List<PropertyGroup>()),};
contentTypeList.Add(uWebshopContentType);


contentTypeService.Save(contentTypeList);

uwbsCatalogContentType.SetLazyParentId(new Lazy<int>(() => uWebshopContentType.Id));
uwbsCategoryContentType.SetLazyParentId(new Lazy<int>(() => uwbsCatalogContentType.Id));
uwbsCategoryRepositoryContentType.SetLazyParentId(new Lazy<int>(() => uwbsCatalogContentType.Id));
uwbsOrderDateFolderContentType.SetLazyParentId(new Lazy<int>(() => uwbsOrderRepositoryContentType.Id));
uwbsDiscountOrderBasicContentType.SetLazyParentId(new Lazy<int>(() => uwbsDiscountOrderSectionContentType.Id));
uwbsDiscountOrderCountdownContentType.SetLazyParentId(new Lazy<int>(() => uwbsDiscountOrderSectionContentType.Id));
uwbsDiscountOrderCouponContentType.SetLazyParentId(new Lazy<int>(() => uwbsDiscountOrderSectionContentType.Id));
uwbsDiscountOrderMembergroupContentType.SetLazyParentId(new Lazy<int>(() => uwbsDiscountOrderSectionContentType.Id));
uwbsDiscountOrderContentType.SetLazyParentId(new Lazy<int>(() => uwbsDiscountOrderSectionContentType.Id));
uwbsDiscountOrderSectionContentType.SetLazyParentId(new Lazy<int>(() => uwbsDiscountRepositoryContentType.Id));
uwbsDiscountProductContentType.SetLazyParentId(new Lazy<int>(() => uwbsDiscountProductSectionContentType.Id));
uwbsDiscountProductSectionContentType.SetLazyParentId(new Lazy<int>(() => uwbsDiscountRepositoryContentType.Id));
uwbsDiscountOrderRangedContentType.SetLazyParentId(new Lazy<int>(() => uwbsDiscountOrderSectionContentType.Id));
uwbsDiscountRepositoryContentType.SetLazyParentId(new Lazy<int>(() => uWebshopContentType.Id));
uwbsEmailTemplateCustomerContentType.SetLazyParentId(new Lazy<int>(() => uwbsEmailTemplateCustomerSectionContentType.Id));
uwbsEmailTemplateCustomerSectionContentType.SetLazyParentId(new Lazy<int>(() => uwbsEmailRepositoryContentType.Id));
uwbsEmailRepositoryContentType.SetLazyParentId(new Lazy<int>(() => uWebshopContentType.Id));
uwbsEmailTemplateStoreContentType.SetLazyParentId(new Lazy<int>(() => uwbsEmailTemplateStoreSectionContentType.Id));
uwbsEmailTemplateStoreSectionContentType.SetLazyParentId(new Lazy<int>(() => uwbsEmailRepositoryContentType.Id));
uwbsOrderContentType.SetLazyParentId(new Lazy<int>(() => uwbsOrderRepositoryContentType.Id));
uwbsOrderedProductContentType.SetLazyParentId(new Lazy<int>(() => uwbsOrderRepositoryContentType.Id));
uwbsOrderedProductVariantContentType.SetLazyParentId(new Lazy<int>(() => uwbsOrderRepositoryContentType.Id));
uwbsOrderRepositoryContentType.SetLazyParentId(new Lazy<int>(() => uWebshopContentType.Id));
uwbsOrderSectionContentType.SetLazyParentId(new Lazy<int>(() => uwbsOrderRepositoryContentType.Id));
uwbsOrderStoreFolderContentType.SetLazyParentId(new Lazy<int>(() => uwbsOrderRepositoryContentType.Id));
uwbsPaymentProviderContentType.SetLazyParentId(new Lazy<int>(() => uwbsPaymentProviderSectionContentType.Id));
uwbsPaymentProviderMethodContentType.SetLazyParentId(new Lazy<int>(() => uwbsPaymentProviderSectionContentType.Id));
uwbsPaymentProviderRepositoryContentType.SetLazyParentId(new Lazy<int>(() => uWebshopContentType.Id));
uwbsPaymentProviderSectionContentType.SetLazyParentId(new Lazy<int>(() => uwbsPaymentProviderRepositoryContentType.Id));
uwbsPaymentProviderZoneContentType.SetLazyParentId(new Lazy<int>(() => uwbsPaymentProviderZoneSectionContentType.Id));
uwbsPaymentProviderZoneSectionContentType.SetLazyParentId(new Lazy<int>(() => uwbsPaymentProviderRepositoryContentType.Id));
uwbsProductContentType.SetLazyParentId(new Lazy<int>(() => uwbsCatalogContentType.Id));
uwbsProductRepositoryContentType.SetLazyParentId(new Lazy<int>(() => uwbsCatalogContentType.Id));
uwbsProductVariantContentType.SetLazyParentId(new Lazy<int>(() => uwbsCatalogContentType.Id));
uwbsProductVariantGroupContentType.SetLazyParentId(new Lazy<int>(() => uwbsCatalogContentType.Id));
uwbsSettingsContentType.SetLazyParentId(new Lazy<int>(() => uWebshopContentType.Id));
uwbsShippingProviderContentType.SetLazyParentId(new Lazy<int>(() => uwbsShippingProviderSectionContentType.Id));
uwbsShippingProviderMethodContentType.SetLazyParentId(new Lazy<int>(() => uwbsShippingProviderSectionContentType.Id));
uwbsShippingProviderRepositoryContentType.SetLazyParentId(new Lazy<int>(() => uWebshopContentType.Id));
uwbsShippingProviderSectionContentType.SetLazyParentId(new Lazy<int>(() => uwbsShippingProviderRepositoryContentType.Id));
uwbsShippingProviderZoneContentType.SetLazyParentId(new Lazy<int>(() => uwbsShippingProviderZoneSectionContentType.Id));
uwbsShippingProviderZoneSectionContentType.SetLazyParentId(new Lazy<int>(() => uwbsShippingProviderRepositoryContentType.Id));
uwbsStoreContentType.SetLazyParentId(new Lazy<int>(() => uwbsStoreRepositoryContentType.Id));
uwbsStoreRepositoryContentType.SetLazyParentId(new Lazy<int>(() => uWebshopContentType.Id));

contentTypeService.Save(contentTypeList);

uwbsCatalogContentType.AllowedContentTypes = new List<ContentTypeSort> { new ContentTypeSort { Alias = "uwbsCategoryRepository", Id = new Lazy<int>(() => uwbsCategoryRepositoryContentType.Id) },new ContentTypeSort { Alias = "uwbsProductRepository", Id = new Lazy<int>(() => uwbsProductRepositoryContentType.Id) },};
uwbsCategoryContentType.AllowedContentTypes = new List<ContentTypeSort> { new ContentTypeSort { Alias = "uwbsCategory", Id = new Lazy<int>(() => uwbsCategoryContentType.Id) },new ContentTypeSort { Alias = "uwbsProduct", Id = new Lazy<int>(() => uwbsProductContentType.Id) },};
uwbsCategoryRepositoryContentType.AllowedContentTypes = new List<ContentTypeSort> { new ContentTypeSort { Alias = "uwbsCategory", Id = new Lazy<int>(() => uwbsCategoryContentType.Id) },};
uwbsOrderDateFolderContentType.AllowedContentTypes = new List<ContentTypeSort> { new ContentTypeSort { Alias = "uwbsOrderDateFolder", Id = new Lazy<int>(() => uwbsOrderDateFolderContentType.Id) },new ContentTypeSort { Alias = "uwbsOrder", Id = new Lazy<int>(() => uwbsOrderContentType.Id) },};
uwbsDiscountOrderSectionContentType.AllowedContentTypes = new List<ContentTypeSort> { new ContentTypeSort { Alias = "uwbsDiscountOrder", Id = new Lazy<int>(() => uwbsDiscountOrderContentType.Id) },new ContentTypeSort { Alias = "uwbsDiscountOrderCoupon", Id = new Lazy<int>(() => uwbsDiscountOrderCouponContentType.Id) },new ContentTypeSort { Alias = "uwbsDiscountOrderBasic", Id = new Lazy<int>(() => uwbsDiscountOrderBasicContentType.Id) },new ContentTypeSort { Alias = "uwbsDiscountOrderRanged", Id = new Lazy<int>(() => uwbsDiscountOrderRangedContentType.Id) },new ContentTypeSort { Alias = "uwbsDiscountOrderMembergroup", Id = new Lazy<int>(() => uwbsDiscountOrderMembergroupContentType.Id) },new ContentTypeSort { Alias = "uwbsDiscountOrderCountdown", Id = new Lazy<int>(() => uwbsDiscountOrderCountdownContentType.Id) },};
uwbsDiscountProductSectionContentType.AllowedContentTypes = new List<ContentTypeSort> { new ContentTypeSort { Alias = "uwbsDiscountProduct", Id = new Lazy<int>(() => uwbsDiscountProductContentType.Id) },};
uwbsDiscountRepositoryContentType.AllowedContentTypes = new List<ContentTypeSort> { new ContentTypeSort { Alias = "uwbsDiscountOrderSection", Id = new Lazy<int>(() => uwbsDiscountOrderSectionContentType.Id) },new ContentTypeSort { Alias = "uwbsDiscountProductSection", Id = new Lazy<int>(() => uwbsDiscountProductSectionContentType.Id) },};
uwbsEmailTemplateCustomerSectionContentType.AllowedContentTypes = new List<ContentTypeSort> { new ContentTypeSort { Alias = "uwbsEmailTemplateCustomer", Id = new Lazy<int>(() => uwbsEmailTemplateCustomerContentType.Id) },};
uwbsEmailRepositoryContentType.AllowedContentTypes = new List<ContentTypeSort> { new ContentTypeSort { Alias = "uwbsEmailTemplateStoreSection", Id = new Lazy<int>(() => uwbsEmailTemplateStoreSectionContentType.Id) },new ContentTypeSort { Alias = "uwbsEmailTemplateCustomerSection", Id = new Lazy<int>(() => uwbsEmailTemplateCustomerSectionContentType.Id) },};
uwbsEmailTemplateStoreSectionContentType.AllowedContentTypes = new List<ContentTypeSort> { new ContentTypeSort { Alias = "uwbsEmailTemplateStore", Id = new Lazy<int>(() => uwbsEmailTemplateStoreContentType.Id) },};
uwbsOrderContentType.AllowedContentTypes = new List<ContentTypeSort> { new ContentTypeSort { Alias = "uwbsOrderedProduct", Id = new Lazy<int>(() => uwbsOrderedProductContentType.Id) },};
uwbsOrderedProductContentType.AllowedContentTypes = new List<ContentTypeSort> { new ContentTypeSort { Alias = "uwbsOrderedProductVariant", Id = new Lazy<int>(() => uwbsOrderedProductVariantContentType.Id) },};
uwbsOrderRepositoryContentType.AllowedContentTypes = new List<ContentTypeSort> { new ContentTypeSort { Alias = "uwbsOrderSection", Id = new Lazy<int>(() => uwbsOrderSectionContentType.Id) },};
uwbsOrderStoreFolderContentType.AllowedContentTypes = new List<ContentTypeSort> { new ContentTypeSort { Alias = "uwbsOrderDateFolder", Id = new Lazy<int>(() => uwbsOrderDateFolderContentType.Id) },};
uwbsPaymentProviderContentType.AllowedContentTypes = new List<ContentTypeSort> { new ContentTypeSort { Alias = "uwbsPaymentProviderMethod", Id = new Lazy<int>(() => uwbsPaymentProviderMethodContentType.Id) },};
uwbsPaymentProviderRepositoryContentType.AllowedContentTypes = new List<ContentTypeSort> { new ContentTypeSort { Alias = "uwbsPaymentProviderSection", Id = new Lazy<int>(() => uwbsPaymentProviderSectionContentType.Id) },new ContentTypeSort { Alias = "uwbsPaymentProviderZoneSection", Id = new Lazy<int>(() => uwbsPaymentProviderZoneSectionContentType.Id) },};
uwbsPaymentProviderSectionContentType.AllowedContentTypes = new List<ContentTypeSort> { new ContentTypeSort { Alias = "uwbsPaymentProvider", Id = new Lazy<int>(() => uwbsPaymentProviderContentType.Id) },};
uwbsPaymentProviderZoneSectionContentType.AllowedContentTypes = new List<ContentTypeSort> { new ContentTypeSort { Alias = "uwbsPaymentProviderZone", Id = new Lazy<int>(() => uwbsPaymentProviderZoneContentType.Id) },};
uwbsProductContentType.AllowedContentTypes = new List<ContentTypeSort> { new ContentTypeSort { Alias = "uwbsProductVariantGroup", Id = new Lazy<int>(() => uwbsProductVariantGroupContentType.Id) },};
uwbsProductRepositoryContentType.AllowedContentTypes = new List<ContentTypeSort> { new ContentTypeSort { Alias = "uwbsProduct", Id = new Lazy<int>(() => uwbsProductContentType.Id) },new ContentTypeSort { Alias = "uwbsProductVariant", Id = new Lazy<int>(() => uwbsProductVariantContentType.Id) },};
uwbsProductVariantGroupContentType.AllowedContentTypes = new List<ContentTypeSort> { new ContentTypeSort { Alias = "uwbsProductVariant", Id = new Lazy<int>(() => uwbsProductVariantContentType.Id) },};
uwbsSettingsContentType.AllowedContentTypes = new List<ContentTypeSort> { new ContentTypeSort { Alias = "uwbsSettings", Id = new Lazy<int>(() => uwbsSettingsContentType.Id) },};
uwbsShippingProviderContentType.AllowedContentTypes = new List<ContentTypeSort> { new ContentTypeSort { Alias = "uwbsShippingProviderMethod", Id = new Lazy<int>(() => uwbsShippingProviderMethodContentType.Id) },};
uwbsShippingProviderRepositoryContentType.AllowedContentTypes = new List<ContentTypeSort> { new ContentTypeSort { Alias = "uwbsShippingProviderSection", Id = new Lazy<int>(() => uwbsShippingProviderSectionContentType.Id) },new ContentTypeSort { Alias = "uwbsShippingProviderZoneSection", Id = new Lazy<int>(() => uwbsShippingProviderZoneSectionContentType.Id) },};
uwbsShippingProviderSectionContentType.AllowedContentTypes = new List<ContentTypeSort> { new ContentTypeSort { Alias = "uwbsShippingProvider", Id = new Lazy<int>(() => uwbsShippingProviderContentType.Id) },};
uwbsShippingProviderZoneSectionContentType.AllowedContentTypes = new List<ContentTypeSort> { new ContentTypeSort { Alias = "uwbsShippingProviderZone", Id = new Lazy<int>(() => uwbsShippingProviderZoneContentType.Id) },};
uwbsStoreRepositoryContentType.AllowedContentTypes = new List<ContentTypeSort> { new ContentTypeSort { Alias = "uwbsStore", Id = new Lazy<int>(() => uwbsStoreContentType.Id) },};
uWebshopContentType.AllowedContentTypes = new List<ContentTypeSort> { new ContentTypeSort { Alias = "uwbsCatalog", Id = new Lazy<int>(() => uwbsCatalogContentType.Id) },new ContentTypeSort { Alias = "uwbsSettings", Id = new Lazy<int>(() => uwbsSettingsContentType.Id) },new ContentTypeSort { Alias = "uwbsPaymentProviderRepository", Id = new Lazy<int>(() => uwbsPaymentProviderRepositoryContentType.Id) },new ContentTypeSort { Alias = "uwbsShippingProviderRepository", Id = new Lazy<int>(() => uwbsShippingProviderRepositoryContentType.Id) },new ContentTypeSort { Alias = "uwbsStoreRepository", Id = new Lazy<int>(() => uwbsStoreRepositoryContentType.Id) },new ContentTypeSort { Alias = "uwbsOrderRepository", Id = new Lazy<int>(() => uwbsOrderRepositoryContentType.Id) },new ContentTypeSort { Alias = "uwbsDiscountRepository", Id = new Lazy<int>(() => uwbsDiscountRepositoryContentType.Id) },new ContentTypeSort { Alias = "uwbsEmailRepository", Id = new Lazy<int>(() => uwbsEmailRepositoryContentType.Id) },};

contentTypeService.Save(contentTypeList);			ContentInstaller.InstallContent();
		}
	}
}
