using System;
using System.Collections.Generic;
using System.Linq;
using uWebshop.DataAccess.Pocos;
using uWebshop.Umbraco.Interfaces;
using Umbraco.Core;
using Umbraco.Core.Configuration;
using Umbraco.Core.Models;
using Umbraco.Core.Persistence;
using Umbraco.Web;

namespace uWebshop.Umbraco.Services
{
	internal partial class CMSInstaller
	{
		partial void InstallGenerated(IUmbracoVersion umbracoVersion, bool createMissingProperties = false)
		{
				#region check for uWebshop Database tables

		        //Get the Umbraco Database context
		        var db = UmbracoContext.Current.Application.DatabaseContext.Database;

				if(db != null){
					//Check if the DB table does NOT exist
					if (!db.TableExist("uWebshopOrders"))
					{
						//Create DB table - and set overwrite to false
						db.CreateTable<uWebshopOrderData>(false);
						// Alter the OrderInfo (XML) column to NVarChar(MAX)
						try {
							db.Execute("ALTER TABLE uWebshopOrders ALTER COLUMN orderInfo NVARCHAR(MAX)");
						}
						catch {
							// this fails on SQL-CE and is fine because it will stay NTEXT
						}
					}
					//Check if the DB table does NOT exist
					if (!db.TableExist("uWebshopOrderSeries"))
					{
						//Create DB table - and set overwrite to false
						db.CreateTable<uWebshopOrderSeries>(false);
					}

					//Check if the DB table does NOT exist
					if (!db.TableExist("uWebshopCoupon"))
					{
						//Create DB table - and set overwrite to false
						db.CreateTable<uWebshopCoupon>(false);
					}

					//Check if the DB table does NOT exist
					if (!db.TableExist("uWebshopStock"))
					{
						//Create DB table - and set overwrite to false
						db.CreateTable<uWebshopStock>(false);
					}
				}

		        #endregion
			var contentTypeService = ApplicationContext.Current.Services.ContentTypeService;
var dataTypeService = ApplicationContext.Current.Services.DataTypeService;
var contentTypeList = new List<IContentType>();
var newDataTypesList = new List<IDataTypeDefinition>();
var umbracoVersionMajor = UmbracoVersion.Current.Major;
var TrueFalseDataTypeDef = umbracoVersion.GetDataTypeDefinition("Umbraco.TrueFalse");
if (TrueFalseDataTypeDef == null) throw new Exception("Could not load default umbraco TrueFalse datatype");
var StringDataTypeDef = umbracoVersion.GetDataTypeDefinition("Umbraco.Textbox");
if (StringDataTypeDef == null) throw new Exception("Could not load default umbraco String datatype");
var NumericDataTypeDef = umbracoVersion.GetDataTypeDefinition("Umbraco.Integer");
if (NumericDataTypeDef == null) throw new Exception("Could not load default umbraco Numeric datatype");
var TagsDataTypeDef = umbracoVersion.GetDataTypeDefinition("Umbraco.Tags");
if (TagsDataTypeDef == null) throw new Exception("Could not load default umbraco Tags datatype");
var LabelDataTypeDef = umbracoVersion.GetDataTypeDefinition("Umbraco.NoEdit");
if (LabelDataTypeDef == null) throw new Exception("Could not load default umbraco Label datatype");
var RichTextDataTypeDef = umbracoVersion.GetDataTypeDefinition("Umbraco.TinyMCEv3");
if (RichTextDataTypeDef == null) throw new Exception("Could not load default umbraco RichText datatype");
var TextboxMultipleDataTypeDef = umbracoVersion.GetDataTypeDefinition("Umbraco.TextboxMultiple");
if (TextboxMultipleDataTypeDef == null) throw new Exception("Could not load default umbraco TextboxMultiple datatype");
var ContentPickerDataTypeDef = umbracoVersion.GetDataTypeDefinition("Umbraco.ContentPickerAlias");
if (ContentPickerDataTypeDef == null) throw new Exception("Could not load default umbraco ContentPicker datatype");
var MediaPickerDataTypeDef = umbracoVersion.GetDataTypeDefinition("Umbraco.MediaPicker");
if (MediaPickerDataTypeDef == null) throw new Exception("Could not load default umbraco MediaPicker datatype");
var EnableDisableDataTypeDef = umbracoVersion.GetDataTypeDefinition("uWebshop.EnableDisable");
			if (EnableDisableDataTypeDef == null)
			{
			    try
			    {				
				    EnableDisableDataTypeDef = umbracoVersion.CreateDataTypeDefinition(-1, "uWebshop.EnableDisable");
				    EnableDisableDataTypeDef.Name = "Enable/Disable";
				    EnableDisableDataTypeDef.Key = new Guid("63c6fa9a-975f-4474-9155-62a229bafaef");
				    EnableDisableDataTypeDef.DatabaseType = DataTypeDatabaseType.Ntext;
				
				    dataTypeService.Save(EnableDisableDataTypeDef);
                }catch(Exception){}
			}
			var MultiContentPickerCatalogDataTypeDef = umbracoVersion.GetDataTypeDefinition("uWebshop.MultiContentPickerCatalog");
			if (MultiContentPickerCatalogDataTypeDef == null)
			{
			    try
			    {				
				    MultiContentPickerCatalogDataTypeDef = umbracoVersion.CreateDataTypeDefinition(-1, "uWebshop.MultiContentPickerCatalog");
				    MultiContentPickerCatalogDataTypeDef.Name = "uWebshop Catalog Picker";
				    MultiContentPickerCatalogDataTypeDef.Key = new Guid("ea745beb-271c-4542-a5be-5fba2be86f07");
				    MultiContentPickerCatalogDataTypeDef.DatabaseType = DataTypeDatabaseType.Ntext;
				
				    dataTypeService.Save(MultiContentPickerCatalogDataTypeDef);
                }catch(Exception){}
			}
			var MultiContentPickerCategoriesDataTypeDef = umbracoVersion.GetDataTypeDefinition("uWebshop.MultiContentPickerCategories");
			if (MultiContentPickerCategoriesDataTypeDef == null)
			{
			    try
			    {				
				    MultiContentPickerCategoriesDataTypeDef = umbracoVersion.CreateDataTypeDefinition(-1, "uWebshop.MultiContentPickerCategories");
				    MultiContentPickerCategoriesDataTypeDef.Name = "uWebshop Category Picker";
				    MultiContentPickerCategoriesDataTypeDef.Key = new Guid("c0d85cb4-6e4d-4f10-9107-abdee89b5d7d");
				    MultiContentPickerCategoriesDataTypeDef.DatabaseType = DataTypeDatabaseType.Ntext;
				
				    dataTypeService.Save(MultiContentPickerCategoriesDataTypeDef);
                }catch(Exception){}
			}
			var CountriesDataTypeDef = umbracoVersion.GetDataTypeDefinition("uWebshop.Countries");
			if (CountriesDataTypeDef == null)
			{
			    try
			    {				
				    CountriesDataTypeDef = umbracoVersion.CreateDataTypeDefinition(-1, "uWebshop.Countries");
				    CountriesDataTypeDef.Name = "uWebshop Country Selector";
				    CountriesDataTypeDef.Key = new Guid("a6d19ee8-ab93-42d6-a61a-8d4aaa759207");
				    CountriesDataTypeDef.DatabaseType = DataTypeDatabaseType.Ntext;
				
				    dataTypeService.Save(CountriesDataTypeDef);
                }catch(Exception){}
			}
			var CouponCodesDataTypeDef = umbracoVersion.GetDataTypeDefinition("uWebshop.CouponCodes");
			if (CouponCodesDataTypeDef == null)
			{
			    try
			    {				
				    CouponCodesDataTypeDef = umbracoVersion.CreateDataTypeDefinition(-1, "uWebshop.CouponCodes");
				    CouponCodesDataTypeDef.Name = "uWebshop Couponcode Editor";
				    CouponCodesDataTypeDef.Key = new Guid("0dbc0113-a084-44d1-8fef-f8ef0cd8453b");
				    CouponCodesDataTypeDef.DatabaseType = DataTypeDatabaseType.Ntext;
				
				    dataTypeService.Save(CouponCodesDataTypeDef);
                }catch(Exception){}
			}
			var CurrenciesDataTypeDef = umbracoVersion.GetDataTypeDefinition("uWebshop.Currencies");
			if (CurrenciesDataTypeDef == null)
			{
			    try
			    {				
				    CurrenciesDataTypeDef = umbracoVersion.CreateDataTypeDefinition(-1, "uWebshop.Currencies");
				    CurrenciesDataTypeDef.Name = "uWebshop Currencies";
				    CurrenciesDataTypeDef.Key = new Guid("7e6cab81-528d-4b00-8321-72c36f131eea");
				    CurrenciesDataTypeDef.DatabaseType = DataTypeDatabaseType.Ntext;
				
				    dataTypeService.Save(CurrenciesDataTypeDef);
                }catch(Exception){}
			}
			var DiscountOrderConditionDataTypeDef = umbracoVersion.GetDataTypeDefinition("uWebshop.DiscountOrderCondition");
			if (DiscountOrderConditionDataTypeDef == null)
			{
			    try
			    {				
				    DiscountOrderConditionDataTypeDef = umbracoVersion.CreateDataTypeDefinition(-1, "uWebshop.DiscountOrderCondition");
				    DiscountOrderConditionDataTypeDef.Name = "uWebshop Discount Condition Picker";
				    DiscountOrderConditionDataTypeDef.Key = new Guid("e159f935-d7f4-4277-b5d0-adf74b003849");
				    DiscountOrderConditionDataTypeDef.DatabaseType = DataTypeDatabaseType.Ntext;
				
				    dataTypeService.Save(DiscountOrderConditionDataTypeDef);
                }catch(Exception){}
			}
			var DiscountTypeDataTypeDef = umbracoVersion.GetDataTypeDefinition("uWebshop.DiscountType");
			if (DiscountTypeDataTypeDef == null)
			{
			    try
			    {				
				    DiscountTypeDataTypeDef = umbracoVersion.CreateDataTypeDefinition(-1, "uWebshop.DiscountType");
				    DiscountTypeDataTypeDef.Name = "uWebshop Discount Picker";
				    DiscountTypeDataTypeDef.Key = new Guid("2d89188e-33a7-4885-a6d0-1caef40320a7");
				    DiscountTypeDataTypeDef.DatabaseType = DataTypeDatabaseType.Ntext;
				
				    dataTypeService.Save(DiscountTypeDataTypeDef);
                }catch(Exception){}
			}
			var TemplatePickerDataTypeDef = umbracoVersion.GetDataTypeDefinition("uWebshop.TemplatePicker");
			if (TemplatePickerDataTypeDef == null)
			{
			    try
			    {				
				    TemplatePickerDataTypeDef = umbracoVersion.CreateDataTypeDefinition(-1, "uWebshop.TemplatePicker");
				    TemplatePickerDataTypeDef.Name = "uWebshop Email Template Picker";
				    TemplatePickerDataTypeDef.Key = new Guid("0a10d9dd-ebbc-48f5-be93-9fac239ac876");
				    TemplatePickerDataTypeDef.DatabaseType = DataTypeDatabaseType.Nvarchar;
				
				    dataTypeService.Save(TemplatePickerDataTypeDef);
                }catch(Exception){}
			}
			var MultiContentPickerFilesDataTypeDef = umbracoVersion.GetDataTypeDefinition("uWebshop.MultiContentPickerFiles");
			if (MultiContentPickerFilesDataTypeDef == null)
			{
			    try
			    {				
				    MultiContentPickerFilesDataTypeDef = umbracoVersion.CreateDataTypeDefinition(-1, "uWebshop.MultiContentPickerFiles");
				    MultiContentPickerFilesDataTypeDef.Name = "uWebshop File Picker";
				    MultiContentPickerFilesDataTypeDef.Key = new Guid("83c13f11-95d8-4f10-b510-581983fe8c19");
				    MultiContentPickerFilesDataTypeDef.DatabaseType = DataTypeDatabaseType.Ntext;
				
				    dataTypeService.Save(MultiContentPickerFilesDataTypeDef);
                }catch(Exception){}
			}
			var MultiNodePickerDataTypeDef = umbracoVersion.GetDataTypeDefinition("uWebshop.MultiNodePicker");
			if (MultiNodePickerDataTypeDef == null)
			{
			    try
			    {				
				    MultiNodePickerDataTypeDef = umbracoVersion.CreateDataTypeDefinition(-1, "uWebshop.MultiNodePicker");
				    MultiNodePickerDataTypeDef.Name = "uWebshop Global Picker";
				    MultiNodePickerDataTypeDef.Key = new Guid("97600235-acf7-4ade-9ba9-6cad4743cb6d");
				    MultiNodePickerDataTypeDef.DatabaseType = DataTypeDatabaseType.Ntext;
				
				    dataTypeService.Save(MultiNodePickerDataTypeDef);
                }catch(Exception){}
			}
			var MultiContentPickerImagesDataTypeDef = umbracoVersion.GetDataTypeDefinition("uWebshop.MultiContentPickerImages");
			if (MultiContentPickerImagesDataTypeDef == null)
			{
			    try
			    {				
				    MultiContentPickerImagesDataTypeDef = umbracoVersion.CreateDataTypeDefinition(-1, "uWebshop.MultiContentPickerImages");
				    MultiContentPickerImagesDataTypeDef.Name = "uWebshop Image Picker";
				    MultiContentPickerImagesDataTypeDef.Key = new Guid("d61cbc2c-87d3-49d9-8e58-87996d22d97f");
				    MultiContentPickerImagesDataTypeDef.DatabaseType = DataTypeDatabaseType.Ntext;
				
				    dataTypeService.Save(MultiContentPickerImagesDataTypeDef);
                }catch(Exception){}
			}
			var OrderedCountDataTypeDef = umbracoVersion.GetDataTypeDefinition("uWebshop.OrderedCount");
			if (OrderedCountDataTypeDef == null)
			{
			    try
			    {				
				    OrderedCountDataTypeDef = umbracoVersion.CreateDataTypeDefinition(-1, "uWebshop.OrderedCount");
				    OrderedCountDataTypeDef.Name = "uWebshop Item Ordered Count";
				    OrderedCountDataTypeDef.Key = new Guid("2f6e0aa8-8291-4544-8aeb-db78cfb42b07");
				    OrderedCountDataTypeDef.DatabaseType = DataTypeDatabaseType.Integer;
				
				    dataTypeService.Save(OrderedCountDataTypeDef);
                }catch(Exception){}
			}
			var CulturesDataTypeDef = umbracoVersion.GetDataTypeDefinition("uWebshop.Cultures");
			if (CulturesDataTypeDef == null)
			{
			    try
			    {				
				    CulturesDataTypeDef = umbracoVersion.CreateDataTypeDefinition(-1, "uWebshop.Cultures");
				    CulturesDataTypeDef.Name = "uWebshop Language Picker";
				    CulturesDataTypeDef.Key = new Guid("4235f880-64cc-4d78-8fc2-6e6e5ee72010");
				    CulturesDataTypeDef.DatabaseType = DataTypeDatabaseType.Ntext;
				
				    dataTypeService.Save(CulturesDataTypeDef);
                }catch(Exception){}
			}
			var MemberGroupsDataTypeDef = umbracoVersion.GetDataTypeDefinition("uWebshop.MemberGroups");
			if (MemberGroupsDataTypeDef == null)
			{
			    try
			    {				
				    MemberGroupsDataTypeDef = umbracoVersion.CreateDataTypeDefinition(-1, "uWebshop.MemberGroups");
				    MemberGroupsDataTypeDef.Name = "uWebshop MemberGroup Picker";
				    MemberGroupsDataTypeDef.Key = new Guid("a99d5614-8b33-4a63-891a-a254c87af481");
				    MemberGroupsDataTypeDef.DatabaseType = DataTypeDatabaseType.Ntext;
				
				    dataTypeService.Save(MemberGroupsDataTypeDef);
                }catch(Exception){}
			}
			var OrderStatusPickerDataTypeDef = umbracoVersion.GetDataTypeDefinition("uWebshop.OrderStatusPicker");
			if (OrderStatusPickerDataTypeDef == null)
			{
			    try
			    {				
				    OrderStatusPickerDataTypeDef = umbracoVersion.CreateDataTypeDefinition(-1, "uWebshop.OrderStatusPicker");
				    OrderStatusPickerDataTypeDef.Name = "uWebshop Order Status Picker";
				    OrderStatusPickerDataTypeDef.Key = new Guid("8fa38a1b-7854-43ce-b03e-c7cc2ffd3d20");
				    OrderStatusPickerDataTypeDef.DatabaseType = DataTypeDatabaseType.Ntext;
				
				    dataTypeService.Save(OrderStatusPickerDataTypeDef);
                }catch(Exception){}
			}
			var OrderInfoViewerDataTypeDef = umbracoVersion.GetDataTypeDefinition("uWebshop.OrderInfoViewer");
			if (OrderInfoViewerDataTypeDef == null)
			{
			    try
			    {				
				    OrderInfoViewerDataTypeDef = umbracoVersion.CreateDataTypeDefinition(-1, "uWebshop.OrderInfoViewer");
				    OrderInfoViewerDataTypeDef.Name = "uWebshop OrderDetails";
				    OrderInfoViewerDataTypeDef.Key = new Guid("6f455770-3677-4c6c-843d-2c76d7b33893");
				    OrderInfoViewerDataTypeDef.DatabaseType = DataTypeDatabaseType.Ntext;
				
				    dataTypeService.Save(OrderInfoViewerDataTypeDef);
                }catch(Exception){}
			}
			var OrderSectionDataTypeDef = umbracoVersion.GetDataTypeDefinition("uWebshop.OrderSection");
			if (OrderSectionDataTypeDef == null)
			{
			    try
			    {				
				    OrderSectionDataTypeDef = umbracoVersion.CreateDataTypeDefinition(-1, "uWebshop.OrderSection");
				    OrderSectionDataTypeDef.Name = "uWebshop OrderOverview";
				    OrderSectionDataTypeDef.Key = new Guid("de47d313-9364-472b-8ee7-9b002cc204b9");
				    OrderSectionDataTypeDef.DatabaseType = DataTypeDatabaseType.Ntext;
				
				    dataTypeService.Save(OrderSectionDataTypeDef);
                }catch(Exception){}
			}
			var PaymentProviderAmountTypeDataTypeDef = umbracoVersion.GetDataTypeDefinition("uWebshop.PaymentProviderAmountType");
			if (PaymentProviderAmountTypeDataTypeDef == null)
			{
			    try
			    {				
				    PaymentProviderAmountTypeDataTypeDef = umbracoVersion.CreateDataTypeDefinition(-1, "uWebshop.PaymentProviderAmountType");
				    PaymentProviderAmountTypeDataTypeDef.Name = "uWebshop Payment Provider Amount Type";
				    PaymentProviderAmountTypeDataTypeDef.Key = new Guid("fb79c76e-8ccc-406d-a3ad-39c1f939a38d");
				    PaymentProviderAmountTypeDataTypeDef.DatabaseType = DataTypeDatabaseType.Ntext;
				
				    dataTypeService.Save(PaymentProviderAmountTypeDataTypeDef);
                }catch(Exception){}
			}
			var PaymentProviderTypeDataTypeDef = umbracoVersion.GetDataTypeDefinition("uWebshop.PaymentProviderType");
			if (PaymentProviderTypeDataTypeDef == null)
			{
			    try
			    {				
				    PaymentProviderTypeDataTypeDef = umbracoVersion.CreateDataTypeDefinition(-1, "uWebshop.PaymentProviderType");
				    PaymentProviderTypeDataTypeDef.Name = "uWebshop Payment Provider Type";
				    PaymentProviderTypeDataTypeDef.Key = new Guid("bbaf3c0e-fc22-4d33-b884-7b87d8dc3c8c");
				    PaymentProviderTypeDataTypeDef.DatabaseType = DataTypeDatabaseType.Ntext;
				
				    dataTypeService.Save(PaymentProviderTypeDataTypeDef);
                }catch(Exception){}
			}
			var MultiContentPickerPaymentZonesDataTypeDef = umbracoVersion.GetDataTypeDefinition("uWebshop.MultiContentPickerPaymentZones");
			if (MultiContentPickerPaymentZonesDataTypeDef == null)
			{
			    try
			    {				
				    MultiContentPickerPaymentZonesDataTypeDef = umbracoVersion.CreateDataTypeDefinition(-1, "uWebshop.MultiContentPickerPaymentZones");
				    MultiContentPickerPaymentZonesDataTypeDef.Name = "uWebshop Payment Zone Picker";
				    MultiContentPickerPaymentZonesDataTypeDef.Key = new Guid("406a4f2d-dd31-43be-9114-8077f43a6151");
				    MultiContentPickerPaymentZonesDataTypeDef.DatabaseType = DataTypeDatabaseType.Ntext;
				
				    dataTypeService.Save(MultiContentPickerPaymentZonesDataTypeDef);
                }catch(Exception){}
			}
			var PriceDataTypeDef = umbracoVersion.GetDataTypeDefinition("uWebshop.Price");
			if (PriceDataTypeDef == null)
			{
			    try
			    {				
				    PriceDataTypeDef = umbracoVersion.CreateDataTypeDefinition(-1, "uWebshop.Price");
				    PriceDataTypeDef.Name = "uWebshop Price Editor";
				    PriceDataTypeDef.Key = new Guid("40e2736a-8ecd-41b3-bb27-7d11909f0a21");
				    PriceDataTypeDef.DatabaseType = DataTypeDatabaseType.Ntext;
				
				    dataTypeService.Save(PriceDataTypeDef);
                }catch(Exception){}
			}
			var MultiContentPickerProductsDataTypeDef = umbracoVersion.GetDataTypeDefinition("uWebshop.MultiContentPickerProducts");
			if (MultiContentPickerProductsDataTypeDef == null)
			{
			    try
			    {				
				    MultiContentPickerProductsDataTypeDef = umbracoVersion.CreateDataTypeDefinition(-1, "uWebshop.MultiContentPickerProducts");
				    MultiContentPickerProductsDataTypeDef.Name = "uWebshop Product Picker";
				    MultiContentPickerProductsDataTypeDef.Key = new Guid("23806a76-6a36-468c-8188-f25308a71cdb");
				    MultiContentPickerProductsDataTypeDef.DatabaseType = DataTypeDatabaseType.Ntext;
				
				    dataTypeService.Save(MultiContentPickerProductsDataTypeDef);
                }catch(Exception){}
			}
			var ProductOverviewDataTypeDef = umbracoVersion.GetDataTypeDefinition("uWebshop.ProductOverview");
			if (ProductOverviewDataTypeDef == null)
			{
			    try
			    {				
				    ProductOverviewDataTypeDef = umbracoVersion.CreateDataTypeDefinition(-1, "uWebshop.ProductOverview");
				    ProductOverviewDataTypeDef.Name = "uWebshop ProductOverview";
				    ProductOverviewDataTypeDef.Key = new Guid("3873106b-fbb8-4d55-8e91-a07680e796d7");
				    ProductOverviewDataTypeDef.DatabaseType = DataTypeDatabaseType.Ntext;
				
				    dataTypeService.Save(ProductOverviewDataTypeDef);
                }catch(Exception){}
			}
			var RangesDataTypeDef = umbracoVersion.GetDataTypeDefinition("uWebshop.Ranges");
			if (RangesDataTypeDef == null)
			{
			    try
			    {				
				    RangesDataTypeDef = umbracoVersion.CreateDataTypeDefinition(-1, "uWebshop.Ranges");
				    RangesDataTypeDef.Name = "uWebshop Range Selector";
				    RangesDataTypeDef.Key = new Guid("09dfbeec-2681-42af-b07c-0ed56a575d48");
				    RangesDataTypeDef.DatabaseType = DataTypeDatabaseType.Ntext;
				
				    dataTypeService.Save(RangesDataTypeDef);
                }catch(Exception){}
			}
			var ShippingProviderTypeDataTypeDef = umbracoVersion.GetDataTypeDefinition("uWebshop.ShippingProviderType");
			if (ShippingProviderTypeDataTypeDef == null)
			{
			    try
			    {				
				    ShippingProviderTypeDataTypeDef = umbracoVersion.CreateDataTypeDefinition(-1, "uWebshop.ShippingProviderType");
				    ShippingProviderTypeDataTypeDef.Name = "uWebshop Shipping Provider Type";
				    ShippingProviderTypeDataTypeDef.Key = new Guid("c035ada7-5413-48de-8e62-b4b61e2e934f");
				    ShippingProviderTypeDataTypeDef.DatabaseType = DataTypeDatabaseType.Ntext;
				
				    dataTypeService.Save(ShippingProviderTypeDataTypeDef);
                }catch(Exception){}
			}
			var ShippingProviderRangeTypeDataTypeDef = umbracoVersion.GetDataTypeDefinition("uWebshop.ShippingProviderRangeType");
			if (ShippingProviderRangeTypeDataTypeDef == null)
			{
			    try
			    {				
				    ShippingProviderRangeTypeDataTypeDef = umbracoVersion.CreateDataTypeDefinition(-1, "uWebshop.ShippingProviderRangeType");
				    ShippingProviderRangeTypeDataTypeDef.Name = "uWebshop Shipping Range Type";
				    ShippingProviderRangeTypeDataTypeDef.Key = new Guid("34c70cbf-ee1b-465e-941f-ddcd097f7912");
				    ShippingProviderRangeTypeDataTypeDef.DatabaseType = DataTypeDatabaseType.Ntext;
				
				    dataTypeService.Save(ShippingProviderRangeTypeDataTypeDef);
                }catch(Exception){}
			}
			var MultiContentPickerShippingZonesDataTypeDef = umbracoVersion.GetDataTypeDefinition("uWebshop.MultiContentPickerShippingZones");
			if (MultiContentPickerShippingZonesDataTypeDef == null)
			{
			    try
			    {				
				    MultiContentPickerShippingZonesDataTypeDef = umbracoVersion.CreateDataTypeDefinition(-1, "uWebshop.MultiContentPickerShippingZones");
				    MultiContentPickerShippingZonesDataTypeDef.Name = "uWebshop Shipping Zone Picker";
				    MultiContentPickerShippingZonesDataTypeDef.Key = new Guid("18ee35a7-7931-4d80-822c-ffe2bfb40f6e");
				    MultiContentPickerShippingZonesDataTypeDef.DatabaseType = DataTypeDatabaseType.Ntext;
				
				    dataTypeService.Save(MultiContentPickerShippingZonesDataTypeDef);
                }catch(Exception){}
			}
			var ShopDashboardDataTypeDef = umbracoVersion.GetDataTypeDefinition("uWebshop.ShopDashboard");
			if (ShopDashboardDataTypeDef == null)
			{
			    try
			    {				
				    ShopDashboardDataTypeDef = umbracoVersion.CreateDataTypeDefinition(-1, "uWebshop.ShopDashboard");
				    ShopDashboardDataTypeDef.Name = "uWebshop ShopDashboard";
				    ShopDashboardDataTypeDef.Key = new Guid("df1c2d1f-77c2-40fb-a6f8-7fd8612101c5");
				    ShopDashboardDataTypeDef.DatabaseType = DataTypeDatabaseType.Nvarchar;
				
				    dataTypeService.Save(ShopDashboardDataTypeDef);
                }catch(Exception){}
			}
			var StockDataTypeDef = umbracoVersion.GetDataTypeDefinition("uWebshop.Stock");
			if (StockDataTypeDef == null)
			{
			    try
			    {				
				    StockDataTypeDef = umbracoVersion.CreateDataTypeDefinition(-1, "uWebshop.Stock");
				    StockDataTypeDef.Name = "uWebshop Stock";
				    StockDataTypeDef.Key = new Guid("5744ead8-977b-44c1-b362-fe8bebca7098");
				    StockDataTypeDef.DatabaseType = DataTypeDatabaseType.Ntext;
				
				    dataTypeService.Save(StockDataTypeDef);
                }catch(Exception){}
			}
			var StorePickerDataTypeDef = umbracoVersion.GetDataTypeDefinition("uWebshop.StorePicker");
			if (StorePickerDataTypeDef == null)
			{
			    try
			    {				
				    StorePickerDataTypeDef = umbracoVersion.CreateDataTypeDefinition(-1, "uWebshop.StorePicker");
				    StorePickerDataTypeDef.Name = "uWebshop Store Picker";
				    StorePickerDataTypeDef.Key = new Guid("1e8cdc0b-436e-46f5-bfec-57be45745771");
				    StorePickerDataTypeDef.DatabaseType = DataTypeDatabaseType.Integer;
				
				    dataTypeService.Save(StorePickerDataTypeDef);
                }catch(Exception){}
			}
			var StoreTemplatePickerDataTypeDef = umbracoVersion.GetDataTypeDefinition("uWebshop.StoreTemplatePicker");
			if (StoreTemplatePickerDataTypeDef == null)
			{
			    try
			    {				
				    StoreTemplatePickerDataTypeDef = umbracoVersion.CreateDataTypeDefinition(-1, "uWebshop.StoreTemplatePicker");
				    StoreTemplatePickerDataTypeDef.Name = "uWebshop Template Picker";
				    StoreTemplatePickerDataTypeDef.Key = new Guid("a20c7c00-09f1-448d-9656-f5cb012107af");
				    StoreTemplatePickerDataTypeDef.DatabaseType = DataTypeDatabaseType.Integer;
				
				    dataTypeService.Save(StoreTemplatePickerDataTypeDef);
                }catch(Exception){}
			}
			var VatPickerDataTypeDef = umbracoVersion.GetDataTypeDefinition("uWebshop.VatPicker");
			if (VatPickerDataTypeDef == null)
			{
			    try
			    {				
				    VatPickerDataTypeDef = umbracoVersion.CreateDataTypeDefinition(-1, "uWebshop.VatPicker");
				    VatPickerDataTypeDef.Name = "uWebshop Vat Picker";
				    VatPickerDataTypeDef.Key = new Guid("69d3f953-b565-4269-9d68-4b39e13c70e5");
				    VatPickerDataTypeDef.DatabaseType = DataTypeDatabaseType.Integer;
				
				    dataTypeService.Save(VatPickerDataTypeDef);
                }catch(Exception){}
			}
			var ZonesDataTypeDef = umbracoVersion.GetDataTypeDefinition("uWebshop.Zones");
			if (ZonesDataTypeDef == null)
			{
			    try
			    {				
				    ZonesDataTypeDef = umbracoVersion.CreateDataTypeDefinition(-1, "uWebshop.Zones");
				    ZonesDataTypeDef.Name = "uWebshop Zone Selector";
				    ZonesDataTypeDef.Key = new Guid("8bc628dd-fe95-4a73-bdde-a7f4b620c170");
				    ZonesDataTypeDef.DatabaseType = DataTypeDatabaseType.Ntext;
				
				    dataTypeService.Save(ZonesDataTypeDef);
                }catch(Exception){}
			}
			if (newDataTypesList.Any()) dataTypeService.Save(newDataTypesList);

EnableDisableDataTypeDef = dataTypeService.GetDataTypeDefinitionById(new Guid("63c6fa9a-975f-4474-9155-62a229bafaef"));
if (EnableDisableDataTypeDef == null) throw new Exception("Could not create and/or load EnableDisable datatype");
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
MemberGroupsDataTypeDef = dataTypeService.GetDataTypeDefinitionById(new Guid("a99d5614-8b33-4a63-891a-a254c87af481"));
if (MemberGroupsDataTypeDef == null) throw new Exception("Could not create and/or load MemberGroups datatype");
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
ShopDashboardDataTypeDef = dataTypeService.GetDataTypeDefinitionById(new Guid("df1c2d1f-77c2-40fb-a6f8-7fd8612101c5"));
if (ShopDashboardDataTypeDef == null) throw new Exception("Could not create and/or load ShopDashboard datatype");
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

	var MultiContentPickerCatalogDict = new Dictionary<string, PreValue>();MultiContentPickerCatalogDict.Add("treesource", new PreValue("content"));MultiContentPickerCatalogDict.Add("startNode", new PreValue("//uwbsCatalog"));MultiContentPickerCatalogDict.Add("filter", new PreValue("*"));	dataTypeService.SavePreValues(MultiContentPickerCatalogDataTypeDef.Id,MultiContentPickerCatalogDict);
	var MultiContentPickerCategoriesDict = new Dictionary<string, PreValue>();MultiContentPickerCategoriesDict.Add("treesource", new PreValue("content"));MultiContentPickerCategoriesDict.Add("startNode", new PreValue("//uwbsCategoryRepository"));MultiContentPickerCategoriesDict.Add("filter", new PreValue("/*[starts-with(name(),'uwbsCategory')]"));	dataTypeService.SavePreValues(MultiContentPickerCategoriesDataTypeDef.Id,MultiContentPickerCategoriesDict);
	var MultiContentPickerFilesDict = new Dictionary<string, PreValue>();MultiContentPickerFilesDict.Add("treesource", new PreValue("media"));	dataTypeService.SavePreValues(MultiContentPickerFilesDataTypeDef.Id,MultiContentPickerFilesDict);
	var MultiNodePickerDict = new Dictionary<string, PreValue>();MultiNodePickerDict.Add("treesource", new PreValue("content"));MultiNodePickerDict.Add("startNode", new PreValue("/*"));MultiNodePickerDict.Add("filter", new PreValue("*"));	dataTypeService.SavePreValues(MultiNodePickerDataTypeDef.Id,MultiNodePickerDict);
	var MultiContentPickerImagesDict = new Dictionary<string, PreValue>();MultiContentPickerImagesDict.Add("treesource", new PreValue("media"));	dataTypeService.SavePreValues(MultiContentPickerImagesDataTypeDef.Id,MultiContentPickerImagesDict);
	var MultiContentPickerPaymentZonesDict = new Dictionary<string, PreValue>();MultiContentPickerPaymentZonesDict.Add("treesource", new PreValue("content"));MultiContentPickerPaymentZonesDict.Add("startNode", new PreValue("//uwbsPaymentProviderZoneSection"));MultiContentPickerPaymentZonesDict.Add("filter", new PreValue("/*[starts-with(name(),'uwbsPaymentProviderZone')]"));	dataTypeService.SavePreValues(MultiContentPickerPaymentZonesDataTypeDef.Id,MultiContentPickerPaymentZonesDict);
	var MultiContentPickerProductsDict = new Dictionary<string, PreValue>();MultiContentPickerProductsDict.Add("treesource", new PreValue("content"));MultiContentPickerProductsDict.Add("startNode", new PreValue("//uwbsCatalog"));MultiContentPickerProductsDict.Add("filter", new PreValue("/*[starts-with(name(),'uwbsProduct')]"));	dataTypeService.SavePreValues(MultiContentPickerProductsDataTypeDef.Id,MultiContentPickerProductsDict);
	var MultiContentPickerShippingZonesDict = new Dictionary<string, PreValue>();MultiContentPickerShippingZonesDict.Add("treesource", new PreValue("content"));MultiContentPickerShippingZonesDict.Add("startNode", new PreValue("//uwbsShippingProviderZoneSection"));MultiContentPickerShippingZonesDict.Add("filter", new PreValue("/*[starts-with(name(),'uwbsShippingProviderZone')]"));	dataTypeService.SavePreValues(MultiContentPickerShippingZonesDataTypeDef.Id,MultiContentPickerShippingZonesDict);
	var VatPickerDict = new Dictionary<string, PreValue>();VatPickerDict.Add("value", new PreValue("0, 6, 19, 21"));	dataTypeService.SavePreValues(VatPickerDataTypeDef.Id,VatPickerDict);

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

if (uwbsCategoryContentType.PropertyTypes.All(p => p.Alias != "title") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsCategoryContentType, "Global").PropertyTypes.Add(new PropertyType(StringDataTypeDef) { Alias = "title", Name = "#Title", Description = "#TitleDescription",Mandatory = true, });
}
if (uwbsCategoryContentType.PropertyTypes.All(p => p.Alias != "url") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsCategoryContentType, "Global").PropertyTypes.Add(new PropertyType(StringDataTypeDef) { Alias = "url", Name = "#Url", Description = "#UrlDescription",Mandatory = true, });
}
if (uwbsCategoryContentType.PropertyTypes.All(p => p.Alias != "metaDescription") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsCategoryContentType, "Global").PropertyTypes.Add(new PropertyType(TextboxMultipleDataTypeDef) { Alias = "metaDescription", Name = "#MetaDescription", Description = "#MetaDescriptionDescription",});
}
if (uwbsCategoryContentType.PropertyTypes.All(p => p.Alias != "metaTags") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsCategoryContentType, "Global").PropertyTypes.Add(new PropertyType(TagsDataTypeDef) { Alias = "metaTags", Name = "#Tags", Description = "#TagsDescription",});
}
if (uwbsCategoryContentType.PropertyTypes.All(p => p.Alias != "categories") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsCategoryContentType, "Global").PropertyTypes.Add(new PropertyType(MultiContentPickerCategoriesDataTypeDef) { Alias = "categories", Name = "#SubCategories", Description = "#SubCategoriesDescription",});
}
if (uwbsCategoryContentType.PropertyTypes.All(p => p.Alias != "description") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsCategoryContentType, "Details").PropertyTypes.Add(new PropertyType(RichTextDataTypeDef) { Alias = "description", Name = "#Description", Description = "#DescriptionDescription",});
}
if (uwbsCategoryContentType.PropertyTypes.All(p => p.Alias != "images") && createMissingProperties == true){
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

if (uwbsDiscountOrderBasicContentType.PropertyTypes.All(p => p.Alias != "title") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsDiscountOrderBasicContentType, "Global").PropertyTypes.Add(new PropertyType(StringDataTypeDef) { Alias = "title", Name = "#Title", Description = "#TitleDescription",Mandatory = true, });
}
if (uwbsDiscountOrderBasicContentType.PropertyTypes.All(p => p.Alias != "description") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsDiscountOrderBasicContentType, "Global").PropertyTypes.Add(new PropertyType(RichTextDataTypeDef) { Alias = "description", Name = "#Description", Description = "#DescriptionDescription",});
}
if (uwbsDiscountOrderBasicContentType.PropertyTypes.All(p => p.Alias != "disable") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsDiscountOrderBasicContentType, "Global").PropertyTypes.Add(new PropertyType(TrueFalseDataTypeDef) { Alias = "disable", Name = "#Disable", Description = "#DisableDescription",});
}
if (uwbsDiscountOrderBasicContentType.PropertyTypes.All(p => p.Alias != "discountType") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsDiscountOrderBasicContentType, "Details").PropertyTypes.Add(new PropertyType(DiscountTypeDataTypeDef) { Alias = "discountType", Name = "#DiscountType", Description = "#DiscountTypeDescription",});
}
if (uwbsDiscountOrderBasicContentType.PropertyTypes.All(p => p.Alias != "discount") && createMissingProperties == true){
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

if (uwbsDiscountOrderCountdownContentType.PropertyTypes.All(p => p.Alias != "title") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsDiscountOrderCountdownContentType, "Global").PropertyTypes.Add(new PropertyType(StringDataTypeDef) { Alias = "title", Name = "#Title", Description = "#TitleDescription",Mandatory = true, });
}
if (uwbsDiscountOrderCountdownContentType.PropertyTypes.All(p => p.Alias != "description") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsDiscountOrderCountdownContentType, "Global").PropertyTypes.Add(new PropertyType(RichTextDataTypeDef) { Alias = "description", Name = "#Description", Description = "#DescriptionDescription",});
}
if (uwbsDiscountOrderCountdownContentType.PropertyTypes.All(p => p.Alias != "disable") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsDiscountOrderCountdownContentType, "Global").PropertyTypes.Add(new PropertyType(TrueFalseDataTypeDef) { Alias = "disable", Name = "#Disable", Description = "#DisableDescription",});
}
if (uwbsDiscountOrderCountdownContentType.PropertyTypes.All(p => p.Alias != "discountType") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsDiscountOrderCountdownContentType, "Details").PropertyTypes.Add(new PropertyType(DiscountTypeDataTypeDef) { Alias = "discountType", Name = "#DiscountType", Description = "#DiscountTypeDescription",});
}
if (uwbsDiscountOrderCountdownContentType.PropertyTypes.All(p => p.Alias != "discount") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsDiscountOrderCountdownContentType, "Details").PropertyTypes.Add(new PropertyType(PriceDataTypeDef) { Alias = "discount", Name = "#Discount", Description = "#DiscountDescription",});
}
if (uwbsDiscountOrderCountdownContentType.PropertyTypes.All(p => p.Alias != "countdownEnabled") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsDiscountOrderCountdownContentType, "Details").PropertyTypes.Add(new PropertyType(TrueFalseDataTypeDef) { Alias = "countdownEnabled", Name = "#CountdownEnabled", Description = "#CountdownEnabledDescription",});
}
if (uwbsDiscountOrderCountdownContentType.PropertyTypes.All(p => p.Alias != "countdown") && createMissingProperties == true){
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

if (uwbsDiscountOrderCouponContentType.PropertyTypes.All(p => p.Alias != "title") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsDiscountOrderCouponContentType, "Global").PropertyTypes.Add(new PropertyType(StringDataTypeDef) { Alias = "title", Name = "#Title", Description = "#TitleDescription",Mandatory = true, });
}
if (uwbsDiscountOrderCouponContentType.PropertyTypes.All(p => p.Alias != "description") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsDiscountOrderCouponContentType, "Global").PropertyTypes.Add(new PropertyType(RichTextDataTypeDef) { Alias = "description", Name = "#Description", Description = "#DescriptionDescription",});
}
if (uwbsDiscountOrderCouponContentType.PropertyTypes.All(p => p.Alias != "disable") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsDiscountOrderCouponContentType, "Global").PropertyTypes.Add(new PropertyType(TrueFalseDataTypeDef) { Alias = "disable", Name = "#Disable", Description = "#DisableDescription",});
}
if (uwbsDiscountOrderCouponContentType.PropertyTypes.All(p => p.Alias != "discountType") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsDiscountOrderCouponContentType, "Details").PropertyTypes.Add(new PropertyType(DiscountTypeDataTypeDef) { Alias = "discountType", Name = "#DiscountType", Description = "#DiscountTypeDescription",});
}
if (uwbsDiscountOrderCouponContentType.PropertyTypes.All(p => p.Alias != "discount") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsDiscountOrderCouponContentType, "Details").PropertyTypes.Add(new PropertyType(PriceDataTypeDef) { Alias = "discount", Name = "#Discount", Description = "#DiscountDescription",});
}
if (uwbsDiscountOrderCouponContentType.PropertyTypes.All(p => p.Alias != "couponCodes") && createMissingProperties == true){
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

if (uwbsDiscountOrderMembergroupContentType.PropertyTypes.All(p => p.Alias != "title") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsDiscountOrderMembergroupContentType, "Global").PropertyTypes.Add(new PropertyType(StringDataTypeDef) { Alias = "title", Name = "#Title", Description = "#TitleDescription",Mandatory = true, });
}
if (uwbsDiscountOrderMembergroupContentType.PropertyTypes.All(p => p.Alias != "description") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsDiscountOrderMembergroupContentType, "Global").PropertyTypes.Add(new PropertyType(RichTextDataTypeDef) { Alias = "description", Name = "#Description", Description = "#DescriptionDescription",});
}
if (uwbsDiscountOrderMembergroupContentType.PropertyTypes.All(p => p.Alias != "disable") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsDiscountOrderMembergroupContentType, "Global").PropertyTypes.Add(new PropertyType(TrueFalseDataTypeDef) { Alias = "disable", Name = "#Disable", Description = "#DisableDescription",});
}
if (uwbsDiscountOrderMembergroupContentType.PropertyTypes.All(p => p.Alias != "discountType") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsDiscountOrderMembergroupContentType, "Details").PropertyTypes.Add(new PropertyType(DiscountTypeDataTypeDef) { Alias = "discountType", Name = "#DiscountType", Description = "#DiscountTypeDescription",});
}
if (uwbsDiscountOrderMembergroupContentType.PropertyTypes.All(p => p.Alias != "discount") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsDiscountOrderMembergroupContentType, "Details").PropertyTypes.Add(new PropertyType(PriceDataTypeDef) { Alias = "discount", Name = "#Discount", Description = "#DiscountDescription",});
}
if (uwbsDiscountOrderMembergroupContentType.PropertyTypes.All(p => p.Alias != "memberGroups") && createMissingProperties == true){
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

if (uwbsDiscountOrderContentType.PropertyTypes.All(p => p.Alias != "title") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsDiscountOrderContentType, "Global").PropertyTypes.Add(new PropertyType(StringDataTypeDef) { Alias = "title", Name = "#Title", Description = "#TitleDescription",Mandatory = true, });
}
if (uwbsDiscountOrderContentType.PropertyTypes.All(p => p.Alias != "description") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsDiscountOrderContentType, "Global").PropertyTypes.Add(new PropertyType(RichTextDataTypeDef) { Alias = "description", Name = "#Description", Description = "#DescriptionDescription",});
}
if (uwbsDiscountOrderContentType.PropertyTypes.All(p => p.Alias != "disable") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsDiscountOrderContentType, "Global").PropertyTypes.Add(new PropertyType(TrueFalseDataTypeDef) { Alias = "disable", Name = "#Disable", Description = "#DisableDescription",});
}
if (uwbsDiscountOrderContentType.PropertyTypes.All(p => p.Alias != "shippingDiscountable") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsDiscountOrderContentType, "Details").PropertyTypes.Add(new PropertyType(TrueFalseDataTypeDef) { Alias = "shippingDiscountable", Name = "#ShippingDiscountable", Description = "#ShippingDiscountableDescription",});
}
if (uwbsDiscountOrderContentType.PropertyTypes.All(p => p.Alias != "discountType") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsDiscountOrderContentType, "Details").PropertyTypes.Add(new PropertyType(DiscountTypeDataTypeDef) { Alias = "discountType", Name = "#DiscountType", Description = "#DiscountTypeDescription",});
}
if (uwbsDiscountOrderContentType.PropertyTypes.All(p => p.Alias != "discount") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsDiscountOrderContentType, "Details").PropertyTypes.Add(new PropertyType(PriceDataTypeDef) { Alias = "discount", Name = "#Discount", Description = "#DiscountDescription",});
}
if (uwbsDiscountOrderContentType.PropertyTypes.All(p => p.Alias != "ranges") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsDiscountOrderContentType, "Details").PropertyTypes.Add(new PropertyType(RangesDataTypeDef) { Alias = "ranges", Name = "#Ranges", Description = "#RangesDescription",});
}
if (uwbsDiscountOrderContentType.PropertyTypes.All(p => p.Alias != "countdownEnabled") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsDiscountOrderContentType, "Details").PropertyTypes.Add(new PropertyType(TrueFalseDataTypeDef) { Alias = "countdownEnabled", Name = "#CountdownEnabled", Description = "#CountdownEnabledDescription",});
}
if (uwbsDiscountOrderContentType.PropertyTypes.All(p => p.Alias != "countdown") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsDiscountOrderContentType, "Details").PropertyTypes.Add(new PropertyType(StockDataTypeDef) { Alias = "countdown", Name = "#Countdown", Description = "#CountdownDescription",});
}
if (uwbsDiscountOrderContentType.PropertyTypes.All(p => p.Alias != "orderCondition") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsDiscountOrderContentType, "Conditions").PropertyTypes.Add(new PropertyType(DiscountOrderConditionDataTypeDef) { Alias = "orderCondition", Name = "#OrderCondition", Description = "#OrderConditionDescription",});
}
if (uwbsDiscountOrderContentType.PropertyTypes.All(p => p.Alias != "numberOfItemsCondition") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsDiscountOrderContentType, "Conditions").PropertyTypes.Add(new PropertyType(NumericDataTypeDef) { Alias = "numberOfItemsCondition", Name = "#NumberOfItemsCondition", Description = "#NumberOfItemsConditionDescription",});
}
if (uwbsDiscountOrderContentType.PropertyTypes.All(p => p.Alias != "minimumAmount") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsDiscountOrderContentType, "Conditions").PropertyTypes.Add(new PropertyType(PriceDataTypeDef) { Alias = "minimumAmount", Name = "#MinimumAmount", Description = "#MinimumAmountDescription",});
}
if (uwbsDiscountOrderContentType.PropertyTypes.All(p => p.Alias != "couponCodes") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsDiscountOrderContentType, "Conditions").PropertyTypes.Add(new PropertyType(CouponCodesDataTypeDef) { Alias = "couponCodes", Name = "#CouponCodes", Description = "#CouponCodesDescription",});
}
if (uwbsDiscountOrderContentType.PropertyTypes.All(p => p.Alias != "oncePerCustomer") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsDiscountOrderContentType, "Conditions").PropertyTypes.Add(new PropertyType(TrueFalseDataTypeDef) { Alias = "oncePerCustomer", Name = "#OncePerCustomer", Description = "#OncePerCustomerDescription",});
}
if (uwbsDiscountOrderContentType.PropertyTypes.All(p => p.Alias != "items") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsDiscountOrderContentType, "Filter").PropertyTypes.Add(new PropertyType(MultiNodePickerDataTypeDef) { Alias = "items", Name = "#Items", Description = "#ItemsDescription",});
}
if (uwbsDiscountOrderContentType.PropertyTypes.All(p => p.Alias != "affectedOrderlines") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsDiscountOrderContentType, "Filter").PropertyTypes.Add(new PropertyType(MultiContentPickerCatalogDataTypeDef) { Alias = "affectedOrderlines", Name = "#AffectedOrderlines", Description = "#AffectedOrderlinesDescription",});
}
if (uwbsDiscountOrderContentType.PropertyTypes.All(p => p.Alias != "affectedTags") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsDiscountOrderContentType, "Filter").PropertyTypes.Add(new PropertyType(TagsDataTypeDef) { Alias = "affectedTags", Name = "#AffectedTags", Description = "#AffectedTagsDescription",});
}
if (uwbsDiscountOrderContentType.PropertyTypes.All(p => p.Alias != "memberGroups") && createMissingProperties == true){
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

if (uwbsDiscountProductContentType.PropertyTypes.All(p => p.Alias != "title") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsDiscountProductContentType, "Global").PropertyTypes.Add(new PropertyType(StringDataTypeDef) { Alias = "title", Name = "#Title", Description = "#TitleDescription",Mandatory = true, });
}
if (uwbsDiscountProductContentType.PropertyTypes.All(p => p.Alias != "description") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsDiscountProductContentType, "Global").PropertyTypes.Add(new PropertyType(RichTextDataTypeDef) { Alias = "description", Name = "#Description", Description = "#DescriptionDescription",});
}
if (uwbsDiscountProductContentType.PropertyTypes.All(p => p.Alias != "disable") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsDiscountProductContentType, "Global").PropertyTypes.Add(new PropertyType(TrueFalseDataTypeDef) { Alias = "disable", Name = "#Disable", Description = "#DisableDescription",});
}
if (uwbsDiscountProductContentType.PropertyTypes.All(p => p.Alias != "products") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsDiscountProductContentType, "Details").PropertyTypes.Add(new PropertyType(MultiContentPickerCatalogDataTypeDef) { Alias = "products", Name = "#Products", Description = "#ProductsDescription",});
}
if (uwbsDiscountProductContentType.PropertyTypes.All(p => p.Alias != "excludeVariants") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsDiscountProductContentType, "Details").PropertyTypes.Add(new PropertyType(TrueFalseDataTypeDef) { Alias = "excludeVariants", Name = "#ExcludeVariants", Description = "#ExcludeVariantsDescription",});
}
if (uwbsDiscountProductContentType.PropertyTypes.All(p => p.Alias != "discountType") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsDiscountProductContentType, "Details").PropertyTypes.Add(new PropertyType(DiscountTypeDataTypeDef) { Alias = "discountType", Name = "#DiscountType", Description = "#DiscountTypeDescription",});
}
if (uwbsDiscountProductContentType.PropertyTypes.All(p => p.Alias != "discount") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsDiscountProductContentType, "Details").PropertyTypes.Add(new PropertyType(PriceDataTypeDef) { Alias = "discount", Name = "#Discount", Description = "#DiscountDescription",});
}
if (uwbsDiscountProductContentType.PropertyTypes.All(p => p.Alias != "ranges") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsDiscountProductContentType, "Details").PropertyTypes.Add(new PropertyType(RangesDataTypeDef) { Alias = "ranges", Name = "#Ranges", Description = "#RangesDescription",});
}
if (uwbsDiscountProductContentType.PropertyTypes.All(p => p.Alias != "countdownEnabled") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsDiscountProductContentType, "Details").PropertyTypes.Add(new PropertyType(TrueFalseDataTypeDef) { Alias = "countdownEnabled", Name = "#CountdownEnabled", Description = "#CountdownEnabledDescription",});
}
if (uwbsDiscountProductContentType.PropertyTypes.All(p => p.Alias != "countdown") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsDiscountProductContentType, "Details").PropertyTypes.Add(new PropertyType(StockDataTypeDef) { Alias = "countdown", Name = "#Countdown", Description = "#CountdownDescription",});
}
if (uwbsDiscountProductContentType.PropertyTypes.All(p => p.Alias != "memberGroups") && createMissingProperties == true){
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

if (uwbsDiscountOrderRangedContentType.PropertyTypes.All(p => p.Alias != "title") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsDiscountOrderRangedContentType, "Global").PropertyTypes.Add(new PropertyType(StringDataTypeDef) { Alias = "title", Name = "#Title", Description = "#TitleDescription",Mandatory = true, });
}
if (uwbsDiscountOrderRangedContentType.PropertyTypes.All(p => p.Alias != "description") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsDiscountOrderRangedContentType, "Global").PropertyTypes.Add(new PropertyType(RichTextDataTypeDef) { Alias = "description", Name = "#Description", Description = "#DescriptionDescription",});
}
if (uwbsDiscountOrderRangedContentType.PropertyTypes.All(p => p.Alias != "disable") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsDiscountOrderRangedContentType, "Global").PropertyTypes.Add(new PropertyType(TrueFalseDataTypeDef) { Alias = "disable", Name = "#Disable", Description = "#DisableDescription",});
}
if (uwbsDiscountOrderRangedContentType.PropertyTypes.All(p => p.Alias != "discountType") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsDiscountOrderRangedContentType, "Details").PropertyTypes.Add(new PropertyType(DiscountTypeDataTypeDef) { Alias = "discountType", Name = "#DiscountType", Description = "#DiscountTypeDescription",});
}
if (uwbsDiscountOrderRangedContentType.PropertyTypes.All(p => p.Alias != "discount") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsDiscountOrderRangedContentType, "Details").PropertyTypes.Add(new PropertyType(PriceDataTypeDef) { Alias = "discount", Name = "#Discount", Description = "#DiscountDescription",});
}
if (uwbsDiscountOrderRangedContentType.PropertyTypes.All(p => p.Alias != "ranges") && createMissingProperties == true){
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

if (uwbsEmailTemplateCustomerContentType.PropertyTypes.All(p => p.Alias != "title") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsEmailTemplateCustomerContentType, "Global").PropertyTypes.Add(new PropertyType(StringDataTypeDef) { Alias = "title", Name = "#Title", Description = "#TitleDescription",});
}
if (uwbsEmailTemplateCustomerContentType.PropertyTypes.All(p => p.Alias != "description") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsEmailTemplateCustomerContentType, "Global").PropertyTypes.Add(new PropertyType(RichTextDataTypeDef) { Alias = "description", Name = "#Description", Description = "#DescriptionDescription",});
}
if (uwbsEmailTemplateCustomerContentType.PropertyTypes.All(p => p.Alias != "emailtemplate") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsEmailTemplateCustomerContentType, "Global").PropertyTypes.Add(new PropertyType(TemplatePickerDataTypeDef) { Alias = "emailtemplate", Name = "#Template", Description = "#TemplateDescription",});
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

if (uwbsEmailTemplateStoreContentType.PropertyTypes.All(p => p.Alias != "title") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsEmailTemplateStoreContentType, "Global").PropertyTypes.Add(new PropertyType(StringDataTypeDef) { Alias = "title", Name = "#Title", Description = "#TitleDescription",});
}
if (uwbsEmailTemplateStoreContentType.PropertyTypes.All(p => p.Alias != "description") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsEmailTemplateStoreContentType, "Global").PropertyTypes.Add(new PropertyType(RichTextDataTypeDef) { Alias = "description", Name = "#Description", Description = "#DescriptionDescription",});
}
if (uwbsEmailTemplateStoreContentType.PropertyTypes.All(p => p.Alias != "emailtemplate") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsEmailTemplateStoreContentType, "Global").PropertyTypes.Add(new PropertyType(TemplatePickerDataTypeDef) { Alias = "emailtemplate", Name = "#Template", Description = "#TemplateDescription",});
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

if (uwbsOrderContentType.PropertyTypes.All(p => p.Alias != "orderStatusPicker")&& umbracoVersionMajor == 6 && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsOrderContentType, "Global").PropertyTypes.Add(new PropertyType(OrderStatusPickerDataTypeDef) { Alias = "orderStatusPicker", Name = "#OrderStatusPicker", Description = "#OrderStatusPickerDescription",});
}
if (uwbsOrderContentType.PropertyTypes.All(p => p.Alias != "orderPaid")&& umbracoVersionMajor == 6 && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsOrderContentType, "Global").PropertyTypes.Add(new PropertyType(TrueFalseDataTypeDef) { Alias = "orderPaid", Name = "#OrderPaid", Description = "#OrderPaidDescription",});
}
if (uwbsOrderContentType.PropertyTypes.All(p => p.Alias != "orderDetails") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsOrderContentType, "Global").PropertyTypes.Add(new PropertyType(OrderInfoViewerDataTypeDef) { Alias = "orderDetails", Name = "#OrderDetails", Description = "#OrderDetailsDescription",});
}
if (uwbsOrderContentType.PropertyTypes.All(p => p.Alias != "orderGuid") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsOrderContentType, "Global").PropertyTypes.Add(new PropertyType(LabelDataTypeDef) { Alias = "orderGuid", Name = "#OrderGuid", Description = "#OrderGuidDescription",});
}
if (uwbsOrderContentType.PropertyTypes.All(p => p.Alias != "customerEmail") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsOrderContentType, "Customer").PropertyTypes.Add(new PropertyType(StringDataTypeDef) { Alias = "customerEmail", Name = "#CustomerEmail", Description = "#CustomerEmailDescription",});
}
if (uwbsOrderContentType.PropertyTypes.All(p => p.Alias != "customerFirstName") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsOrderContentType, "Customer").PropertyTypes.Add(new PropertyType(StringDataTypeDef) { Alias = "customerFirstName", Name = "#CustomerFirstName", Description = "#CustomerFirstNameDescription",});
}
if (uwbsOrderContentType.PropertyTypes.All(p => p.Alias != "customerLastName") && createMissingProperties == true){
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

if (uwbsOrderedProductContentType.PropertyTypes.All(p => p.Alias != "productId") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsOrderedProductContentType, "Global").PropertyTypes.Add(new PropertyType(ContentPickerDataTypeDef) { Alias = "productId", Name = "#ProductId", Description = "#ProductIdDescription",});
}
if (uwbsOrderedProductContentType.PropertyTypes.All(p => p.Alias != "typeAlias") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsOrderedProductContentType, "Global").PropertyTypes.Add(new PropertyType(LabelDataTypeDef) { Alias = "typeAlias", Name = "#TypeAlias", Description = "#TypeAliasDescription",});
}
if (uwbsOrderedProductContentType.PropertyTypes.All(p => p.Alias != "title") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsOrderedProductContentType, "Global").PropertyTypes.Add(new PropertyType(StringDataTypeDef) { Alias = "title", Name = "#Title", Description = "#TitleDescription",});
}
if (uwbsOrderedProductContentType.PropertyTypes.All(p => p.Alias != "sku") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsOrderedProductContentType, "Global").PropertyTypes.Add(new PropertyType(StringDataTypeDef) { Alias = "sku", Name = "#SKU", Description = "#SKUDescription",});
}
if (uwbsOrderedProductContentType.PropertyTypes.All(p => p.Alias != "length") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsOrderedProductContentType, "Details").PropertyTypes.Add(new PropertyType(NumericDataTypeDef) { Alias = "length", Name = "#Length", Description = "#LengthDescription",});
}
if (uwbsOrderedProductContentType.PropertyTypes.All(p => p.Alias != "width") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsOrderedProductContentType, "Details").PropertyTypes.Add(new PropertyType(NumericDataTypeDef) { Alias = "width", Name = "#Width", Description = "WidthDescription",});
}
if (uwbsOrderedProductContentType.PropertyTypes.All(p => p.Alias != "height") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsOrderedProductContentType, "Details").PropertyTypes.Add(new PropertyType(NumericDataTypeDef) { Alias = "height", Name = "#Height", Description = "HeightDescription",});
}
if (uwbsOrderedProductContentType.PropertyTypes.All(p => p.Alias != "weight") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsOrderedProductContentType, "Details").PropertyTypes.Add(new PropertyType(NumericDataTypeDef) { Alias = "weight", Name = "#Weight", Description = "#WeightDescription",});
}
if (uwbsOrderedProductContentType.PropertyTypes.All(p => p.Alias != "ranges") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsOrderedProductContentType, "Price").PropertyTypes.Add(new PropertyType(RangesDataTypeDef) { Alias = "ranges", Name = "#Ranges", Description = "#RangesDescription",});
}
if (uwbsOrderedProductContentType.PropertyTypes.All(p => p.Alias != "price") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsOrderedProductContentType, "Price").PropertyTypes.Add(new PropertyType(PriceDataTypeDef) { Alias = "price", Name = "#Price", Description = "#PriceDescription",});
}
if (uwbsOrderedProductContentType.PropertyTypes.All(p => p.Alias != "vat") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsOrderedProductContentType, "Price").PropertyTypes.Add(new PropertyType(VatPickerDataTypeDef) { Alias = "vat", Name = "#VAT", Description = "#VatDescription",});
}
if (uwbsOrderedProductContentType.PropertyTypes.All(p => p.Alias != "itemCount") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsOrderedProductContentType, "Price").PropertyTypes.Add(new PropertyType(NumericDataTypeDef) { Alias = "itemCount", Name = "#ItemCount", Description = "#ItemCountDescription",});
}
if (uwbsOrderedProductContentType.PropertyTypes.All(p => p.Alias != "orderedProductDiscountPercentage") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsOrderedProductContentType, "Price").PropertyTypes.Add(new PropertyType(StringDataTypeDef) { Alias = "orderedProductDiscountPercentage", Name = "#OrderedProductDiscountPercentage", Description = "#OrderedProductDiscountPercentageDescription",});
}
if (uwbsOrderedProductContentType.PropertyTypes.All(p => p.Alias != "orderedProductDiscountAmount") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsOrderedProductContentType, "Price").PropertyTypes.Add(new PropertyType(StringDataTypeDef) { Alias = "orderedProductDiscountAmount", Name = "#OrderedProductDiscountAmount", Description = "#OrderedProductDiscountAmountDescription",});
}
if (uwbsOrderedProductContentType.PropertyTypes.All(p => p.Alias != "orderedProductDiscountExcludingVariants") && createMissingProperties == true){
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

if (uwbsOrderedProductVariantContentType.PropertyTypes.All(p => p.Alias != "variantId") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsOrderedProductVariantContentType, "Global").PropertyTypes.Add(new PropertyType(ContentPickerDataTypeDef) { Alias = "variantId", Name = "#VariantId", Description = "#VariantIdDescription",});
}
if (uwbsOrderedProductVariantContentType.PropertyTypes.All(p => p.Alias != "typeAlias") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsOrderedProductVariantContentType, "Global").PropertyTypes.Add(new PropertyType(LabelDataTypeDef) { Alias = "typeAlias", Name = "#TypeAlias", Description = "#TypeAliasDescription",});
}
if (uwbsOrderedProductVariantContentType.PropertyTypes.All(p => p.Alias != "title") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsOrderedProductVariantContentType, "Global").PropertyTypes.Add(new PropertyType(StringDataTypeDef) { Alias = "title", Name = "#Title", Description = "#TitleDescription",});
}
if (uwbsOrderedProductVariantContentType.PropertyTypes.All(p => p.Alias != "sku") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsOrderedProductVariantContentType, "Global").PropertyTypes.Add(new PropertyType(StringDataTypeDef) { Alias = "sku", Name = "#SKU", Description = "#SKUDescription",});
}
if (uwbsOrderedProductVariantContentType.PropertyTypes.All(p => p.Alias != "group") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsOrderedProductVariantContentType, "Global").PropertyTypes.Add(new PropertyType(StringDataTypeDef) { Alias = "group", Name = "#Group", Description = "#GroupDescription",});
}
if (uwbsOrderedProductVariantContentType.PropertyTypes.All(p => p.Alias != "length") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsOrderedProductVariantContentType, "Details").PropertyTypes.Add(new PropertyType(NumericDataTypeDef) { Alias = "length", Name = "#Length", Description = "#LengthDescription",});
}
if (uwbsOrderedProductVariantContentType.PropertyTypes.All(p => p.Alias != "width") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsOrderedProductVariantContentType, "Details").PropertyTypes.Add(new PropertyType(NumericDataTypeDef) { Alias = "width", Name = "#Width", Description = "WidthDescription",});
}
if (uwbsOrderedProductVariantContentType.PropertyTypes.All(p => p.Alias != "height") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsOrderedProductVariantContentType, "Details").PropertyTypes.Add(new PropertyType(NumericDataTypeDef) { Alias = "height", Name = "#Height", Description = "HeightDescription",});
}
if (uwbsOrderedProductVariantContentType.PropertyTypes.All(p => p.Alias != "weight") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsOrderedProductVariantContentType, "Details").PropertyTypes.Add(new PropertyType(NumericDataTypeDef) { Alias = "weight", Name = "#Weight", Description = "#WeightDescription",});
}
if (uwbsOrderedProductVariantContentType.PropertyTypes.All(p => p.Alias != "price") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsOrderedProductVariantContentType, "Price").PropertyTypes.Add(new PropertyType(PriceDataTypeDef) { Alias = "price", Name = "#Price", Description = "#PriceDescription",});
}
if (uwbsOrderedProductVariantContentType.PropertyTypes.All(p => p.Alias != "ranges") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsOrderedProductVariantContentType, "Price").PropertyTypes.Add(new PropertyType(RangesDataTypeDef) { Alias = "ranges", Name = "#Ranges", Description = "#RangesDescription",});
}
if (uwbsOrderedProductVariantContentType.PropertyTypes.All(p => p.Alias != "discountPercentage") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsOrderedProductVariantContentType, "Price").PropertyTypes.Add(new PropertyType(StringDataTypeDef) { Alias = "discountPercentage", Name = "#OrderedProductVariantDiscountPercentage", Description = "#OrderedProductVariantDiscountPercentageDescription",});
}
if (uwbsOrderedProductVariantContentType.PropertyTypes.All(p => p.Alias != "discountAmount") && createMissingProperties == true){
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

if (uwbsOrderRepositoryContentType.PropertyTypes.All(p => p.Alias != "orderSection") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsOrderRepositoryContentType, "Global").PropertyTypes.Add(new PropertyType(OrderSectionDataTypeDef) { Alias = "orderSection", Name = "#OrderSection", Description = "#OrderSectionDescription",});
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

if (uwbsPaymentProviderContentType.PropertyTypes.All(p => p.Alias != "title") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsPaymentProviderContentType, "Global").PropertyTypes.Add(new PropertyType(StringDataTypeDef) { Alias = "title", Name = "#Title", Description = "#TitleDescription",});
}
if (uwbsPaymentProviderContentType.PropertyTypes.All(p => p.Alias != "description") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsPaymentProviderContentType, "Global").PropertyTypes.Add(new PropertyType(RichTextDataTypeDef) { Alias = "description", Name = "#Description", Description = "#DescriptionDescription",});
}
if (uwbsPaymentProviderContentType.PropertyTypes.All(p => p.Alias != "image") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsPaymentProviderContentType, "Global").PropertyTypes.Add(new PropertyType(MediaPickerDataTypeDef) { Alias = "image", Name = "#Image", Description = "#ImageDescription",});
}
if (uwbsPaymentProviderContentType.PropertyTypes.All(p => p.Alias != "type") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsPaymentProviderContentType, "Details").PropertyTypes.Add(new PropertyType(PaymentProviderTypeDataTypeDef) { Alias = "type", Name = "#PaymentProviderType", Description = "#PaymentProviderTypeDescription",});
}
if (uwbsPaymentProviderContentType.PropertyTypes.All(p => p.Alias != "zone") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsPaymentProviderContentType, "Details").PropertyTypes.Add(new PropertyType(MultiContentPickerPaymentZonesDataTypeDef) { Alias = "zone", Name = "#Zone", Description = "#ZoneDescription",});
}
if (uwbsPaymentProviderContentType.PropertyTypes.All(p => p.Alias != "successNode") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsPaymentProviderContentType, "Details").PropertyTypes.Add(new PropertyType(ContentPickerDataTypeDef) { Alias = "successNode", Name = "#SuccessNode", Description = "#SuccessNodeDescription",});
}
if (uwbsPaymentProviderContentType.PropertyTypes.All(p => p.Alias != "errorNode") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsPaymentProviderContentType, "Details").PropertyTypes.Add(new PropertyType(ContentPickerDataTypeDef) { Alias = "errorNode", Name = "#ErrorNode", Description = "#ErrorNodeDescription",});
}
if (uwbsPaymentProviderContentType.PropertyTypes.All(p => p.Alias != "cancelNode") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsPaymentProviderContentType, "Details").PropertyTypes.Add(new PropertyType(ContentPickerDataTypeDef) { Alias = "cancelNode", Name = "#CancelNode", Description = "#CancelNodeDescription",});
}
if (uwbsPaymentProviderContentType.PropertyTypes.All(p => p.Alias != "testMode") && createMissingProperties == true){
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

if (uwbsPaymentProviderMethodContentType.PropertyTypes.All(p => p.Alias != "disable") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsPaymentProviderMethodContentType, "Global").PropertyTypes.Add(new PropertyType(TrueFalseDataTypeDef) { Alias = "disable", Name = "#Disable", Description = "#DisableDescription",});
}
if (uwbsPaymentProviderMethodContentType.PropertyTypes.All(p => p.Alias != "title") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsPaymentProviderMethodContentType, "Global").PropertyTypes.Add(new PropertyType(StringDataTypeDef) { Alias = "title", Name = "#Title", Description = "#TitleDescription",});
}
if (uwbsPaymentProviderMethodContentType.PropertyTypes.All(p => p.Alias != "description") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsPaymentProviderMethodContentType, "Global").PropertyTypes.Add(new PropertyType(RichTextDataTypeDef) { Alias = "description", Name = "#Description", Description = "#DescriptionDescription",});
}
if (uwbsPaymentProviderMethodContentType.PropertyTypes.All(p => p.Alias != "image") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsPaymentProviderMethodContentType, "Global").PropertyTypes.Add(new PropertyType(MediaPickerDataTypeDef) { Alias = "image", Name = "#Image", Description = "#ImageDescription",});
}
if (uwbsPaymentProviderMethodContentType.PropertyTypes.All(p => p.Alias != "price") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsPaymentProviderMethodContentType, "Price").PropertyTypes.Add(new PropertyType(PriceDataTypeDef) { Alias = "price", Name = "#Price", Description = "#PriceDescription",});
}
if (uwbsPaymentProviderMethodContentType.PropertyTypes.All(p => p.Alias != "vat") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsPaymentProviderMethodContentType, "Price").PropertyTypes.Add(new PropertyType(VatPickerDataTypeDef) { Alias = "vat", Name = "#VAT", Description = "#VatDescription",});
}
if (uwbsPaymentProviderMethodContentType.PropertyTypes.All(p => p.Alias != "amountType") && createMissingProperties == true){
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

if (uwbsPaymentProviderZoneContentType.PropertyTypes.All(p => p.Alias != "zone") && createMissingProperties == true){
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

if (uwbsProductContentType.PropertyTypes.All(p => p.Alias != "title") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsProductContentType, "Global").PropertyTypes.Add(new PropertyType(StringDataTypeDef) { Alias = "title", Name = "#Title", Description = "#TitleDescription",Mandatory = true, });
}
if (uwbsProductContentType.PropertyTypes.All(p => p.Alias != "url") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsProductContentType, "Global").PropertyTypes.Add(new PropertyType(StringDataTypeDef) { Alias = "url", Name = "#Url", Description = "#UrlDescription",Mandatory = true, });
}
if (uwbsProductContentType.PropertyTypes.All(p => p.Alias != "sku") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsProductContentType, "Global").PropertyTypes.Add(new PropertyType(StringDataTypeDef) { Alias = "sku", Name = "#SKU", Description = "#SKUDescription",});
}
if (uwbsProductContentType.PropertyTypes.All(p => p.Alias != "metaDescription") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsProductContentType, "Global").PropertyTypes.Add(new PropertyType(TextboxMultipleDataTypeDef) { Alias = "metaDescription", Name = "#MetaDescription", Description = "#MetaDescriptionDescription",});
}
if (uwbsProductContentType.PropertyTypes.All(p => p.Alias != "metaTags") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsProductContentType, "Global").PropertyTypes.Add(new PropertyType(TagsDataTypeDef) { Alias = "metaTags", Name = "#Tags", Description = "#TagsDescription",});
}
if (uwbsProductContentType.PropertyTypes.All(p => p.Alias != "categories") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsProductContentType, "Global").PropertyTypes.Add(new PropertyType(MultiContentPickerCategoriesDataTypeDef) { Alias = "categories", Name = "#ProductCategories", Description = "#ProductCategoriesDescription",});
}
if (uwbsProductContentType.PropertyTypes.All(p => p.Alias != "disable") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsProductContentType, "Global").PropertyTypes.Add(new PropertyType(TrueFalseDataTypeDef) { Alias = "disable", Name = "#Disable", Description = "#DisableDescription",});
}
if (uwbsProductContentType.PropertyTypes.All(p => p.Alias != "description") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsProductContentType, "Details").PropertyTypes.Add(new PropertyType(RichTextDataTypeDef) { Alias = "description", Name = "#Description", Description = "#DescriptionDescription",});
}
if (uwbsProductContentType.PropertyTypes.All(p => p.Alias != "images") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsProductContentType, "Details").PropertyTypes.Add(new PropertyType(MultiContentPickerImagesDataTypeDef) { Alias = "images", Name = "#Images", Description = "#ImagesDescription",});
}
if (uwbsProductContentType.PropertyTypes.All(p => p.Alias != "files") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsProductContentType, "Details").PropertyTypes.Add(new PropertyType(MultiContentPickerFilesDataTypeDef) { Alias = "files", Name = "#Files", Description = "#FilesDescription",});
}
if (uwbsProductContentType.PropertyTypes.All(p => p.Alias != "length") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsProductContentType, "Details").PropertyTypes.Add(new PropertyType(NumericDataTypeDef) { Alias = "length", Name = "#Length", Description = "#LengthDescription",});
}
if (uwbsProductContentType.PropertyTypes.All(p => p.Alias != "width") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsProductContentType, "Details").PropertyTypes.Add(new PropertyType(NumericDataTypeDef) { Alias = "width", Name = "#Width", Description = "#WidthDescription",});
}
if (uwbsProductContentType.PropertyTypes.All(p => p.Alias != "height") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsProductContentType, "Details").PropertyTypes.Add(new PropertyType(NumericDataTypeDef) { Alias = "height", Name = "#Height", Description = "#HeightDescription",});
}
if (uwbsProductContentType.PropertyTypes.All(p => p.Alias != "weight") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsProductContentType, "Details").PropertyTypes.Add(new PropertyType(NumericDataTypeDef) { Alias = "weight", Name = "#Weight", Description = "#WeightDescription",});
}
if (uwbsProductContentType.PropertyTypes.All(p => p.Alias != "price") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsProductContentType, "Price").PropertyTypes.Add(new PropertyType(PriceDataTypeDef) { Alias = "price", Name = "#Price", Description = "#PriceDescription",});
}
if (uwbsProductContentType.PropertyTypes.All(p => p.Alias != "ranges") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsProductContentType, "Price").PropertyTypes.Add(new PropertyType(RangesDataTypeDef) { Alias = "ranges", Name = "#Ranges", Description = "#RangesDescription",});
}
if (uwbsProductContentType.PropertyTypes.All(p => p.Alias != "vat") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsProductContentType, "Price").PropertyTypes.Add(new PropertyType(VatPickerDataTypeDef) { Alias = "vat", Name = "#VAT", Description = "#VatDescription",});
}
if (uwbsProductContentType.PropertyTypes.All(p => p.Alias != "stock") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsProductContentType, "Price").PropertyTypes.Add(new PropertyType(StockDataTypeDef) { Alias = "stock", Name = "#Stock", Description = "#StockDescription",});
}
if (uwbsProductContentType.PropertyTypes.All(p => p.Alias != "ordered") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsProductContentType, "Price").PropertyTypes.Add(new PropertyType(OrderedCountDataTypeDef) { Alias = "ordered", Name = "#Ordered", Description = "#OrderedDescription",});
}
if (uwbsProductContentType.PropertyTypes.All(p => p.Alias != "stockStatus") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsProductContentType, "Price").PropertyTypes.Add(new PropertyType(EnableDisableDataTypeDef) { Alias = "stockStatus", Name = "#StockStatus", Description = "#StockStatusDescription",});
}
if (uwbsProductContentType.PropertyTypes.All(p => p.Alias != "backorderStatus") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsProductContentType, "Price").PropertyTypes.Add(new PropertyType(EnableDisableDataTypeDef) { Alias = "backorderStatus", Name = "#BackorderStatus", Description = "#BackorderStatusDescription",});
}
if (uwbsProductContentType.PropertyTypes.All(p => p.Alias != "useVariantStock") && createMissingProperties == true){
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

if (uwbsProductRepositoryContentType.PropertyTypes.All(p => p.Alias != "productOverview") && createMissingProperties == true){
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

if (uwbsProductVariantContentType.PropertyTypes.All(p => p.Alias != "title") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsProductVariantContentType, "Global").PropertyTypes.Add(new PropertyType(StringDataTypeDef) { Alias = "title", Name = "#Title", Description = "#TitleDescription",Mandatory = true, });
}
if (uwbsProductVariantContentType.PropertyTypes.All(p => p.Alias != "sku") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsProductVariantContentType, "Global").PropertyTypes.Add(new PropertyType(StringDataTypeDef) { Alias = "sku", Name = "#SKU", Description = "#SKUDescription",});
}
if (uwbsProductVariantContentType.PropertyTypes.All(p => p.Alias != "disable") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsProductVariantContentType, "Global").PropertyTypes.Add(new PropertyType(TrueFalseDataTypeDef) { Alias = "disable", Name = "#Disable", Description = "#DisableDescription",});
}
if (uwbsProductVariantContentType.PropertyTypes.All(p => p.Alias != "description") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsProductVariantContentType, "Details").PropertyTypes.Add(new PropertyType(RichTextDataTypeDef) { Alias = "description", Name = "#Description", Description = "#DescriptionDescription",});
}
if (uwbsProductVariantContentType.PropertyTypes.All(p => p.Alias != "length") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsProductVariantContentType, "Details").PropertyTypes.Add(new PropertyType(NumericDataTypeDef) { Alias = "length", Name = "#Length", Description = "#LengthDescription",});
}
if (uwbsProductVariantContentType.PropertyTypes.All(p => p.Alias != "width") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsProductVariantContentType, "Details").PropertyTypes.Add(new PropertyType(NumericDataTypeDef) { Alias = "width", Name = "#Width", Description = "#WidthDescription",});
}
if (uwbsProductVariantContentType.PropertyTypes.All(p => p.Alias != "height") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsProductVariantContentType, "Details").PropertyTypes.Add(new PropertyType(NumericDataTypeDef) { Alias = "height", Name = "#Height", Description = "#HeightDescription",});
}
if (uwbsProductVariantContentType.PropertyTypes.All(p => p.Alias != "weight") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsProductVariantContentType, "Details").PropertyTypes.Add(new PropertyType(NumericDataTypeDef) { Alias = "weight", Name = "#Weight", Description = "#WeightDescription",});
}
if (uwbsProductVariantContentType.PropertyTypes.All(p => p.Alias != "price") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsProductVariantContentType, "Price").PropertyTypes.Add(new PropertyType(PriceDataTypeDef) { Alias = "price", Name = "#Price", Description = "#PriceDescription",});
}
if (uwbsProductVariantContentType.PropertyTypes.All(p => p.Alias != "ranges") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsProductVariantContentType, "Price").PropertyTypes.Add(new PropertyType(RangesDataTypeDef) { Alias = "ranges", Name = "#Ranges", Description = "#RangesDescription",});
}
if (uwbsProductVariantContentType.PropertyTypes.All(p => p.Alias != "stock") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsProductVariantContentType, "Price").PropertyTypes.Add(new PropertyType(StockDataTypeDef) { Alias = "stock", Name = "#Stock", Description = "#StockDescription",});
}
if (uwbsProductVariantContentType.PropertyTypes.All(p => p.Alias != "ordered") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsProductVariantContentType, "Price").PropertyTypes.Add(new PropertyType(OrderedCountDataTypeDef) { Alias = "ordered", Name = "#Ordered", Description = "#OrderedDescription",});
}
if (uwbsProductVariantContentType.PropertyTypes.All(p => p.Alias != "stockStatus") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsProductVariantContentType, "Price").PropertyTypes.Add(new PropertyType(EnableDisableDataTypeDef) { Alias = "stockStatus", Name = "#StockStatus", Description = "#StockStatusDescription",});
}
if (uwbsProductVariantContentType.PropertyTypes.All(p => p.Alias != "backorderStatus") && createMissingProperties == true){
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

if (uwbsProductVariantGroupContentType.PropertyTypes.All(p => p.Alias != "title") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsProductVariantGroupContentType, "Global").PropertyTypes.Add(new PropertyType(StringDataTypeDef) { Alias = "title", Name = "#Title", Description = "#TitleDescription",Mandatory = true, });
}
if (uwbsProductVariantGroupContentType.PropertyTypes.All(p => p.Alias != "required") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsProductVariantGroupContentType, "Global").PropertyTypes.Add(new PropertyType(TrueFalseDataTypeDef) { Alias = "required", Name = "#RequiredVariantGroup", Description = "#RequiredVariantGroupDescription",});
}
if (uwbsProductVariantGroupContentType.PropertyTypes.All(p => p.Alias != "description") && createMissingProperties == true){
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

if (uwbsSettingsContentType.PropertyTypes.All(p => p.Alias != "includingVat") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsSettingsContentType, "Global").PropertyTypes.Add(new PropertyType(TrueFalseDataTypeDef) { Alias = "includingVat", Name = "#IncludingVat", Description = "#IncludingVatSettingDescription",});
}
if (uwbsSettingsContentType.PropertyTypes.All(p => p.Alias != "lowercaseUrls") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsSettingsContentType, "Global").PropertyTypes.Add(new PropertyType(TrueFalseDataTypeDef) { Alias = "lowercaseUrls", Name = "#LowercaseUrls", Description = "#LowercaseUrlsSettingDescription",});
}
if (uwbsSettingsContentType.PropertyTypes.All(p => p.Alias != "incompleteOrderLifetime") && createMissingProperties == true){
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

if (uwbsShippingProviderContentType.PropertyTypes.All(p => p.Alias != "title") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsShippingProviderContentType, "Global").PropertyTypes.Add(new PropertyType(StringDataTypeDef) { Alias = "title", Name = "#Title", Description = "#TitleDescription",});
}
if (uwbsShippingProviderContentType.PropertyTypes.All(p => p.Alias != "description") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsShippingProviderContentType, "Global").PropertyTypes.Add(new PropertyType(RichTextDataTypeDef) { Alias = "description", Name = "#Description", Description = "#DescriptionDescription",});
}
if (uwbsShippingProviderContentType.PropertyTypes.All(p => p.Alias != "image") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsShippingProviderContentType, "Global").PropertyTypes.Add(new PropertyType(MediaPickerDataTypeDef) { Alias = "image", Name = "#Image", Description = "#ImageDescription",});
}
if (uwbsShippingProviderContentType.PropertyTypes.All(p => p.Alias != "type") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsShippingProviderContentType, "Details").PropertyTypes.Add(new PropertyType(ShippingProviderTypeDataTypeDef) { Alias = "type", Name = "#ShippingProviderType", Description = "#ShippingProviderTypeDescription",});
}
if (uwbsShippingProviderContentType.PropertyTypes.All(p => p.Alias != "rangeType") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsShippingProviderContentType, "Details").PropertyTypes.Add(new PropertyType(ShippingProviderRangeTypeDataTypeDef) { Alias = "rangeType", Name = "#ShippingProviderRangeType", Description = "#ShippingProviderRangeTypeDescription",});
}
if (uwbsShippingProviderContentType.PropertyTypes.All(p => p.Alias != "rangeStart") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsShippingProviderContentType, "Details").PropertyTypes.Add(new PropertyType(NumericDataTypeDef) { Alias = "rangeStart", Name = "#RangeStart", Description = "#RangeStartDescription",});
}
if (uwbsShippingProviderContentType.PropertyTypes.All(p => p.Alias != "rangeEnd") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsShippingProviderContentType, "Details").PropertyTypes.Add(new PropertyType(NumericDataTypeDef) { Alias = "rangeEnd", Name = "#RangeEnd", Description = "#RangeEndDescription",});
}
if (uwbsShippingProviderContentType.PropertyTypes.All(p => p.Alias != "overrule") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsShippingProviderContentType, "Details").PropertyTypes.Add(new PropertyType(TrueFalseDataTypeDef) { Alias = "overrule", Name = "#Overrule", Description = "#OverruleDescription",});
}
if (uwbsShippingProviderContentType.PropertyTypes.All(p => p.Alias != "zone") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsShippingProviderContentType, "Details").PropertyTypes.Add(new PropertyType(MultiContentPickerShippingZonesDataTypeDef) { Alias = "zone", Name = "#Zone", Description = "#ZoneDescription",});
}
if (uwbsShippingProviderContentType.PropertyTypes.All(p => p.Alias != "testMode") && createMissingProperties == true){
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

if (uwbsShippingProviderMethodContentType.PropertyTypes.All(p => p.Alias != "disable") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsShippingProviderMethodContentType, "Global").PropertyTypes.Add(new PropertyType(TrueFalseDataTypeDef) { Alias = "disable", Name = "#Disable", Description = "#DisableDescription",});
}
if (uwbsShippingProviderMethodContentType.PropertyTypes.All(p => p.Alias != "title") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsShippingProviderMethodContentType, "Global").PropertyTypes.Add(new PropertyType(StringDataTypeDef) { Alias = "title", Name = "#Title", Description = "#TitleDescription",});
}
if (uwbsShippingProviderMethodContentType.PropertyTypes.All(p => p.Alias != "description") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsShippingProviderMethodContentType, "Global").PropertyTypes.Add(new PropertyType(RichTextDataTypeDef) { Alias = "description", Name = "#Description", Description = "#DescriptionDescription",});
}
if (uwbsShippingProviderMethodContentType.PropertyTypes.All(p => p.Alias != "image") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsShippingProviderMethodContentType, "Global").PropertyTypes.Add(new PropertyType(MediaPickerDataTypeDef) { Alias = "image", Name = "#Image", Description = "#ImageDescription",});
}
if (uwbsShippingProviderMethodContentType.PropertyTypes.All(p => p.Alias != "price") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsShippingProviderMethodContentType, "Price").PropertyTypes.Add(new PropertyType(PriceDataTypeDef) { Alias = "price", Name = "#Price", Description = "#PriceDescription",});
}
if (uwbsShippingProviderMethodContentType.PropertyTypes.All(p => p.Alias != "vat") && createMissingProperties == true){
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

if (uwbsShippingProviderZoneContentType.PropertyTypes.All(p => p.Alias != "zone") && createMissingProperties == true){
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

if (uwbsStoreContentType.PropertyTypes.All(p => p.Alias != "storeCulture") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsStoreContentType, "Global").PropertyTypes.Add(new PropertyType(CulturesDataTypeDef) { Alias = "storeCulture", Name = "#StoreCulture", Description = "#StoreCultureDescription",});
}
if (uwbsStoreContentType.PropertyTypes.All(p => p.Alias != "countryCode") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsStoreContentType, "Global").PropertyTypes.Add(new PropertyType(CountriesDataTypeDef) { Alias = "countryCode", Name = "#CountryCode", Description = "#CountryCodeDescription",});
}
if (uwbsStoreContentType.PropertyTypes.All(p => p.Alias != "currencies") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsStoreContentType, "Global").PropertyTypes.Add(new PropertyType(CurrenciesDataTypeDef) { Alias = "currencies", Name = "#Currencies", Description = "#CurrenciesDescription",});
}
if (uwbsStoreContentType.PropertyTypes.All(p => p.Alias != "globalVat") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsStoreContentType, "Global").PropertyTypes.Add(new PropertyType(VatPickerDataTypeDef) { Alias = "globalVat", Name = "#GlobalVat", Description = "#GlobalVatDescription",});
}
if (uwbsStoreContentType.PropertyTypes.All(p => p.Alias != "orderNumberPrefix") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsStoreContentType, "Global").PropertyTypes.Add(new PropertyType(StringDataTypeDef) { Alias = "orderNumberPrefix", Name = "#OrderNumberPrefix", Description = "#OrderNumberPrefixDescription",});
}
if (uwbsStoreContentType.PropertyTypes.All(p => p.Alias != "orderNumberTemplate") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsStoreContentType, "Global").PropertyTypes.Add(new PropertyType(StringDataTypeDef) { Alias = "orderNumberTemplate", Name = "#OrderNumberTemplate", Description = "#OrderNumberTemplateDescription",});
}
if (uwbsStoreContentType.PropertyTypes.All(p => p.Alias != "orderNumberStartNumber") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsStoreContentType, "Global").PropertyTypes.Add(new PropertyType(NumericDataTypeDef) { Alias = "orderNumberStartNumber", Name = "#OrderNumberStartNumber", Description = "#OrderNumberStartNumberDescription",});
}
if (uwbsStoreContentType.PropertyTypes.All(p => p.Alias != "enableStock") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsStoreContentType, "Global").PropertyTypes.Add(new PropertyType(EnableDisableDataTypeDef) { Alias = "enableStock", Name = "#EnableStock", Description = "#EnableStockDescription",});
}
if (uwbsStoreContentType.PropertyTypes.All(p => p.Alias != "defaultUseVariantStock") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsStoreContentType, "Global").PropertyTypes.Add(new PropertyType(EnableDisableDataTypeDef) { Alias = "defaultUseVariantStock", Name = "#UseVariantStock", Description = "#UseVariantStockDescription",});
}
if (uwbsStoreContentType.PropertyTypes.All(p => p.Alias != "defaultCountdownEnabled") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsStoreContentType, "Global").PropertyTypes.Add(new PropertyType(EnableDisableDataTypeDef) { Alias = "defaultCountdownEnabled", Name = "#CountdownEnabled", Description = "#CountdownEnabledDescription",});
}
if (uwbsStoreContentType.PropertyTypes.All(p => p.Alias != "storeStock") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsStoreContentType, "Global").PropertyTypes.Add(new PropertyType(TrueFalseDataTypeDef) { Alias = "storeStock", Name = "#StoreStock", Description = "#StoreStockDescription",});
}
if (uwbsStoreContentType.PropertyTypes.All(p => p.Alias != "useBackorders") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsStoreContentType, "Global").PropertyTypes.Add(new PropertyType(EnableDisableDataTypeDef) { Alias = "useBackorders", Name = "#UseBackorders", Description = "#UseBackordersDescription",});
}
if (uwbsStoreContentType.PropertyTypes.All(p => p.Alias != "enableTestmode") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsStoreContentType, "Global").PropertyTypes.Add(new PropertyType(EnableDisableDataTypeDef) { Alias = "enableTestmode", Name = "#EnableTestmode", Description = "#EnableTestmodeDescription",});
}
if (uwbsStoreContentType.PropertyTypes.All(p => p.Alias != "storeEmailFrom") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsStoreContentType, "Email").PropertyTypes.Add(new PropertyType(StringDataTypeDef) { Alias = "storeEmailFrom", Name = "#StoreEmailFrom", Description = "#StoreEmailFromDescription",});
}
if (uwbsStoreContentType.PropertyTypes.All(p => p.Alias != "storeEmailFromName") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsStoreContentType, "Email").PropertyTypes.Add(new PropertyType(StringDataTypeDef) { Alias = "storeEmailFromName", Name = "#StoreEmailFromName", Description = "#StoreEmailFromNameDescription",});
}
if (uwbsStoreContentType.PropertyTypes.All(p => p.Alias != "storeEmailTo") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsStoreContentType, "Email").PropertyTypes.Add(new PropertyType(StringDataTypeDef) { Alias = "storeEmailTo", Name = "#StoreEmailTo", Description = "#StoreEmailToDescription",});
}
if (uwbsStoreContentType.PropertyTypes.All(p => p.Alias != "accountEmailCreated") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsStoreContentType, "Email").PropertyTypes.Add(new PropertyType(ContentPickerDataTypeDef) { Alias = "accountEmailCreated", Name = "#AccountEmailCreated", Description = "#AccountEmailCreatedDescription",});
}
if (uwbsStoreContentType.PropertyTypes.All(p => p.Alias != "accountForgotPassword") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsStoreContentType, "Email").PropertyTypes.Add(new PropertyType(ContentPickerDataTypeDef) { Alias = "accountForgotPassword", Name = "#AccountForgotPassword", Description = "#AccountForgotPasswordDescription",});
}
if (uwbsStoreContentType.PropertyTypes.All(p => p.Alias != "AccountChangePasswordNode") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsStoreContentType, "Email").PropertyTypes.Add(new PropertyType(ContentPickerDataTypeDef) { Alias = "AccountChangePasswordNode", Name = "#AccountChangePasswordNode", Description = "#AccountChangePasswordNodeDescription",});
}
if (uwbsStoreContentType.PropertyTypes.All(p => p.Alias != "confirmationEmailStore") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsStoreContentType, "Email").PropertyTypes.Add(new PropertyType(ContentPickerDataTypeDef) { Alias = "confirmationEmailStore", Name = "#ConfirmationEmailStore", Description = "#ConfirmationEmailStoreDescription",});
}
if (uwbsStoreContentType.PropertyTypes.All(p => p.Alias != "confirmationEmailCustomer") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsStoreContentType, "Email").PropertyTypes.Add(new PropertyType(ContentPickerDataTypeDef) { Alias = "confirmationEmailCustomer", Name = "#ConfirmationEmailCustomer", Description = "#ConfirmationEmailCustomerDescription",});
}
if (uwbsStoreContentType.PropertyTypes.All(p => p.Alias != "onlinePaymentEmailStore") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsStoreContentType, "Email").PropertyTypes.Add(new PropertyType(ContentPickerDataTypeDef) { Alias = "onlinePaymentEmailStore", Name = "#OnlinePaymentEmailStore", Description = "#OnlinePaymentEmailStoreDescription",});
}
if (uwbsStoreContentType.PropertyTypes.All(p => p.Alias != "onlinePaymentEmailCustomer") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsStoreContentType, "Email").PropertyTypes.Add(new PropertyType(ContentPickerDataTypeDef) { Alias = "onlinePaymentEmailCustomer", Name = "#OnlinePaymentEmailCustomer", Description = "#OnlinePaymentEmailCustomerDescription",});
}
if (uwbsStoreContentType.PropertyTypes.All(p => p.Alias != "offlinePaymentEmailStore") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsStoreContentType, "Email").PropertyTypes.Add(new PropertyType(ContentPickerDataTypeDef) { Alias = "offlinePaymentEmailStore", Name = "#OfflinePaymentEmailStore", Description = "#OfflinePaymentEmailStoreDescription",});
}
if (uwbsStoreContentType.PropertyTypes.All(p => p.Alias != "offlinePaymentEmailCustomer") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsStoreContentType, "Email").PropertyTypes.Add(new PropertyType(ContentPickerDataTypeDef) { Alias = "offlinePaymentEmailCustomer", Name = "#OfflinePaymentEmailCustomer", Description = "#OfflinePaymentEmailCustomerDescription",});
}
if (uwbsStoreContentType.PropertyTypes.All(p => p.Alias != "paymentFailedEmailStore") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsStoreContentType, "Email").PropertyTypes.Add(new PropertyType(ContentPickerDataTypeDef) { Alias = "paymentFailedEmailStore", Name = "#PaymentFailedEmailStore", Description = "#PaymentFailedEmailStoreDescription",});
}
if (uwbsStoreContentType.PropertyTypes.All(p => p.Alias != "paymentFailedEmailCustomer") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsStoreContentType, "Email").PropertyTypes.Add(new PropertyType(ContentPickerDataTypeDef) { Alias = "paymentFailedEmailCustomer", Name = "#PaymentFailedEmailCustomer", Description = "#PaymentFailedEmailCustomerDescription",});
}
if (uwbsStoreContentType.PropertyTypes.All(p => p.Alias != "dispatchedEmailStore") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsStoreContentType, "Email").PropertyTypes.Add(new PropertyType(ContentPickerDataTypeDef) { Alias = "dispatchedEmailStore", Name = "#DispatchedEmailStore", Description = "#DispatchedEmailStoreDescription",});
}
if (uwbsStoreContentType.PropertyTypes.All(p => p.Alias != "dispatchedEmailCustomer") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsStoreContentType, "Email").PropertyTypes.Add(new PropertyType(ContentPickerDataTypeDef) { Alias = "dispatchedEmailCustomer", Name = "#DispatchedEmailCustomer", Description = "#DispatchedEmailCustomerDescription",});
}
if (uwbsStoreContentType.PropertyTypes.All(p => p.Alias != "cancelEmailStore") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsStoreContentType, "Email").PropertyTypes.Add(new PropertyType(ContentPickerDataTypeDef) { Alias = "cancelEmailStore", Name = "#CancelEmailStore", Description = "#CancelEmailStoreDescription",});
}
if (uwbsStoreContentType.PropertyTypes.All(p => p.Alias != "cancelEmailCustomer") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsStoreContentType, "Email").PropertyTypes.Add(new PropertyType(ContentPickerDataTypeDef) { Alias = "cancelEmailCustomer", Name = "#CancelEmailCustomer", Description = "#CancelEmailCustomerDescription",});
}
if (uwbsStoreContentType.PropertyTypes.All(p => p.Alias != "closedEmailStore") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsStoreContentType, "Email").PropertyTypes.Add(new PropertyType(ContentPickerDataTypeDef) { Alias = "closedEmailStore", Name = "#ClosedEmailStore", Description = "#ClosedEmailStoreDescription",});
}
if (uwbsStoreContentType.PropertyTypes.All(p => p.Alias != "closedEmailCustomer") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsStoreContentType, "Email").PropertyTypes.Add(new PropertyType(ContentPickerDataTypeDef) { Alias = "closedEmailCustomer", Name = "#ClosedEmailCustomer", Description = "#ClosedEmailCustomerDescription",});
}
if (uwbsStoreContentType.PropertyTypes.All(p => p.Alias != "pendingEmailStore") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsStoreContentType, "Email").PropertyTypes.Add(new PropertyType(ContentPickerDataTypeDef) { Alias = "pendingEmailStore", Name = "#PendingEmailStore", Description = "#PendingEmailStoreDescription",});
}
if (uwbsStoreContentType.PropertyTypes.All(p => p.Alias != "pendingEmailCustomer") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsStoreContentType, "Email").PropertyTypes.Add(new PropertyType(ContentPickerDataTypeDef) { Alias = "pendingEmailCustomer", Name = "#PendingEmailCustomer", Description = "#PendingEmailCustomerDescription",});
}
if (uwbsStoreContentType.PropertyTypes.All(p => p.Alias != "temporaryOutOfStockEmailStore") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsStoreContentType, "Email").PropertyTypes.Add(new PropertyType(ContentPickerDataTypeDef) { Alias = "temporaryOutOfStockEmailStore", Name = "#TemporaryOutOfStockEmailStore", Description = "#TemporaryOutOfStockEmailStoreDescription",});
}
if (uwbsStoreContentType.PropertyTypes.All(p => p.Alias != "temporaryOutOfStockEmailCustomer") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsStoreContentType, "Email").PropertyTypes.Add(new PropertyType(ContentPickerDataTypeDef) { Alias = "temporaryOutOfStockEmailCustomer", Name = "#TemporaryOutOfStockEmailCustomer", Description = "#TemporaryOutOfStockEmailCustomerDescription",});
}
if (uwbsStoreContentType.PropertyTypes.All(p => p.Alias != "undeliverableEmailStore") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsStoreContentType, "Email").PropertyTypes.Add(new PropertyType(ContentPickerDataTypeDef) { Alias = "undeliverableEmailStore", Name = "#UndeliverableEmailStore", Description = "#UndeliverableEmailStoreDescription",});
}
if (uwbsStoreContentType.PropertyTypes.All(p => p.Alias != "undeliverableEmailCustomer") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsStoreContentType, "Email").PropertyTypes.Add(new PropertyType(ContentPickerDataTypeDef) { Alias = "undeliverableEmailCustomer", Name = "#UndeliverableEmailCustomer", Description = "#UndeliverableEmailCustomerDescription",});
}
if (uwbsStoreContentType.PropertyTypes.All(p => p.Alias != "returnEmailStore") && createMissingProperties == true){
GetOrAddPropertyGroup(uwbsStoreContentType, "Email").PropertyTypes.Add(new PropertyType(ContentPickerDataTypeDef) { Alias = "returnEmailStore", Name = "#ReturnEmailStore", Description = "#ReturnEmailStoreDescription",});
}
if (uwbsStoreContentType.PropertyTypes.All(p => p.Alias != "returnEmailCustomer") && createMissingProperties == true){
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

if (uWebshopContentType.PropertyTypes.All(p => p.Alias != "shopDashboard") && createMissingProperties == true){
GetOrAddPropertyGroup(uWebshopContentType, "Global").PropertyTypes.Add(new PropertyType(ShopDashboardDataTypeDef) { Alias = "shopDashboard", Name = "#ShopDashboard", Description = "#ShopDashboardDescription",});
}

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

contentTypeService.Save(contentTypeList);			
			ContentInstaller.InstallContent();
		}
	}
}
