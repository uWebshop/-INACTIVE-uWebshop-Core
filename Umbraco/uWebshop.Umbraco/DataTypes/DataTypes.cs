using System;
using System.Collections.Generic;
using uWebshop.Domain;
using uWebshop.Domain.ContentTypes;
using uWebshop.Domain.Core;
using uWebshop.Domain.Interfaces;
using uWebshop.Umbraco.DataTypes.CountySelector;
using uWebshop.Umbraco.DataTypes.CouponCodeEditor;
using uWebshop.Umbraco.DataTypes.Currencies;
using uWebshop.Umbraco.DataTypes.DiscountOrderCondition;
using uWebshop.Umbraco.DataTypes.DiscountType;
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
using uWebshop.Umbraco.DataTypes.ShippingProviderType;
using uWebshop.Umbraco.DataTypes.ShippingRangeType;
using uWebshop.Umbraco.DataTypes.StockUpdate;
using uWebshop.Umbraco.DataTypes.StorePicker;
using uWebshop.Umbraco.DataTypes.StoreTemplatePicker;
using uWebshop.Umbraco.DataTypes.ZoneSelector;

namespace uWebshop.Umbraco.DataTypes
{
    internal class Addon : SimpleAddon
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

    internal class DataTypes : IDataTypeDefinitions
    {
        private readonly List<UwebshopDataTypeDefinition> dataTypes = new List<UwebshopDataTypeDefinition>();

        private void Define(DataType type, string alias)
        {
            Define(type, alias, null, null);
        }

        private void Define(DataType type, Guid? key, string name, DatabaseType dbType = DatabaseType.Ntext,
            List<string> preValues = null)
        {
            Define(type, "uWebshop." + type, key, name, dbType, preValues);
        }

        private void Define(DataType type, string alias, Guid? key, string name,
            DatabaseType dbType = DatabaseType.Ntext, List<string> preValues = null)
        {
            dataTypes.Add(new UwebshopDataTypeDefinition
            {
                DataType = type,
                Alias = alias,
                KeyGuid = key.HasValue ? key.ToString() : null,
                Name = name,
                Type = dbType,
                PreValues = preValues
            });
        }


        public List<UwebshopDataTypeDefinition> LoadDataTypeDefinitions()
        {
            Define(DataType.TrueFalse, "Umbraco.TrueFalse");
            Define(DataType.String, "Umbraco.Textbox");
            Define(DataType.Numeric, "Umbraco.Integer");
            Define(DataType.Tags, "Umbraco.Tags");
            Define(DataType.Label, "Umbraco.NoEdit");
            Define(DataType.RichText, "Umbraco.TinyMCEv3");
            Define(DataType.TextboxMultiple, "Umbraco.TextboxMultiple");
            Define(DataType.ContentPicker, "Umbraco.ContentPickerAlias");
            Define(DataType.MediaPicker, "Umbraco.MediaPicker");

            Define(DataType.Cultures, LanguagePickerDataType.Key, LanguagePickerDataType.Name,
                LanguagePickerDataType.DatabaseType);
            Define(DataType.Countries, CountrySelectorDataType.Key, CountrySelectorDataType.Name,
                CountrySelectorDataType.DatabaseType);
            Define(DataType.DiscountOrderCondition, DiscountOrderConditionDataType.Key,
                DiscountOrderConditionDataType.Name, DiscountOrderConditionDataType.DatabaseType);
            Define(DataType.DiscountType, DiscountTypeDataType.Key, DiscountTypeDataType.Name,
                DiscountTypeDataType.DatabaseType);
            Define(DataType.EnableDisable, EnableDisableDataType.Key, EnableDisableDataType.Name,
                EnableDisableDataType.DatabaseType);
            Define(DataType.MemberGroups, MemberGroupPickerDataType.Key, MemberGroupPickerDataType.Name,
                MemberGroupPickerDataType.DatabaseType);
            Define(DataType.CouponCodes, CouponCodeDataType.Key, CouponCodeDataType.Name,
                CouponCodeDataType.DatabaseType);
            Define(DataType.OrderedCount, OrderCountDataType.Key, OrderCountDataType.Name,
                OrderCountDataType.DatabaseType);
            Define(DataType.OrderStatusPicker, OrderStatusPickerDataType.Key, OrderStatusPickerDataType.Name,
                OrderStatusPickerDataType.DatabaseType);
            //Define(DataType.OrderSection, OrderStatusSectionDataType.DefId, OrderStatusSectionDataType.Key, OrderStatusSectionDataType.Name, OrderStatusSectionDataType.DatabaseType);
            Define(DataType.PaymentProviderType, PaymentProviderTypeDataType.Key, PaymentProviderTypeDataType.Name,
                PaymentProviderTypeDataType.DatabaseType);
            Define(DataType.PaymentProviderAmountType, PaymentProviderAmountTypeDataType.Key,
                PaymentProviderAmountTypeDataType.Name, PaymentProviderAmountTypeDataType.DatabaseType);
            Define(DataType.Price, PriceDataType.Key, PriceDataType.Name, PriceDataType.DatabaseType);
            Define(DataType.Ranges, RangesDataType.Key, RangesDataType.Name, RangesDataType.DatabaseType);
            Define(DataType.ShippingProviderType, ShippingProviderTypeDataType.Key, ShippingProviderTypeDataType.Name,
                ShippingProviderTypeDataType.DatabaseType);
            Define(DataType.ShippingProviderRangeType, ShippingRangeTypeDataType.Key, ShippingRangeTypeDataType.Name,
                ShippingRangeTypeDataType.DatabaseType);
            Define(DataType.Stock, StockUpdateDataType.Key, StockUpdateDataType.Name, StockUpdateDataType.DatabaseType);
            Define(DataType.StorePicker, StorePickerDataType.Key, StorePickerDataType.Name,
                StorePickerDataType.DatabaseType);
            Define(DataType.StoreTemplatePicker, StoreTemplatePickerDataType.Key, StoreTemplatePickerDataType.Name,
                StoreTemplatePickerDataType.DatabaseType);
            Define(DataType.TemplatePicker, EmailTemplateSelectorDataType.Key, EmailTemplateSelectorDataType.Name,
                EmailTemplateSelectorDataType.DatabaseType);
            Define(DataType.Zones, ZoneSelectorDataType.Key, ZoneSelectorDataType.Name,
                ZoneSelectorDataType.DatabaseType);
            Define(DataType.Currencies, CurrenciesDataType.Key, CurrenciesDataType.Name, CurrenciesDataType.DatabaseType);

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


            Define(DataType.MultiContentPickerCatalog, new Guid("ea745beb-271c-4542-a5be-5fba2be86f07"),
                "uWebshop Catalog Picker", DatabaseType.Ntext, GetPrevaluesForMultiNodeTreePicker(catalogXPath));

            Define(DataType.MultiContentPickerCatalog, new Guid("ea745beb-271c-4542-a5be-5fba2be86f07"),
                "uWebshop Catalog Picker", DatabaseType.Ntext, GetPrevaluesForMultiNodeTreePicker(catalogXPath));
            Define(DataType.MultiContentPickerCategories, new Guid("c0d85cb4-6e4d-4f10-9107-abdee89b5d7d"),
                "uWebshop Category Picker", DatabaseType.Ntext,
                GetPrevaluesForMultiNodeTreePicker(catagoryRepositoryXPath, categoryNodeAlias));
            Define(DataType.MultiContentPickerImages, new Guid("d61cbc2c-87d3-49d9-8e58-87996d22d97f"),
                "uWebshop Image Picker", DatabaseType.Ntext, GetPrevaluesForImagesFiles());
            Define(DataType.MultiContentPickerFiles, new Guid("83c13f11-95d8-4f10-b510-581983fe8c19"),
                "uWebshop File Picker", DatabaseType.Ntext, GetPrevaluesForImagesFiles());
            Define(DataType.MultiContentPickerProducts, new Guid("23806a76-6a36-468c-8188-f25308a71cdb"),
                "uWebshop Product Picker", DatabaseType.Ntext,
                GetPrevaluesForMultiNodeTreePicker(catalogXPath, productNodeAlias));
            Define(DataType.MultiContentPickerPaymentZones, new Guid("406a4f2d-dd31-43be-9114-8077f43a6151"),
                "uWebshop Payment Zone Picker", DatabaseType.Ntext,
                GetPrevaluesForMultiNodeTreePicker(paymentproviderzoneXpath, paymentZoneNodeAlias));
            Define(DataType.MultiContentPickerShippingZones, new Guid("18ee35a7-7931-4d80-822c-ffe2bfb40f6e"),
                "uWebshop Shipping Zone Picker", DatabaseType.Ntext,
                GetPrevaluesForMultiNodeTreePicker(shippingproviderzoneXpath, shippingZoneNodeAlias));



            // uWebshop OrderDetails
            // uWebshop OrderOverview
            // EmailDetails
            // ProductOverview

            Define(DataType.OrderInfoViewer, new Guid("6f455770-3677-4c6c-843d-2c76d7b33893"), "uWebshop OrderDetails",
                DatabaseType.Ntext, new List<string> {"/uWebshopBackend/uWebshopUmbracoOrderDetails.cshtml"});
            Define(DataType.OrderSection, new Guid("de47d313-9364-472b-8ee7-9b002cc204b9"), "uWebshop OrderOverview",
                DatabaseType.Ntext, new List<string> {"/uWebshopBackend/uWebshopUmbracoOrderOverview.cshtml"});
            Define(DataType.EmailDetails, new Guid("9d87378c-5864-4ae2-bd83-1cd14b0c3290"), "uWebshop EmailDetails");
            Define(DataType.ProductOverview, new Guid("3873106b-fbb8-4d55-8e91-a07680e796d7"),
                "uWebshop ProductOverview", DatabaseType.Ntext,
                new List<string> {"/uWebshopBackend/uWebshopUmbracoProductOverview.cshtml"});


            Define(DataType.MultiNodePicker, new Guid("97600235-acf7-4ade-9ba9-6cad4743cb6d"), "uWebshop Global Picker",
                DatabaseType.Ntext, GetPrevaluesForMultiNodeTreePicker("/*"));
            //Define(DataType.MultiNodePicker, new Guid(DataTypeGuids.MultiNodeTreePickerId));

            Define(DataType.VatPicker, new Guid("69d3f953-b565-4269-9d68-4b39e13c70e5"), "uWebshop Vat Picker",
                DatabaseType.Integer, new List<string> {"0", "6", "15", "19", "21"});

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
            return new List<string>
            {
                "content",
                string.IsNullOrWhiteSpace(nodeTypeAliasFilter)
                    ? "*"
                    : "/*[starts-with(name(),'" + nodeTypeAliasFilter + "')]",
                "-1",
                "False",
                "1",
                "1",
                "0",
                "False",
                "1",
                "0",
                startXPath,
                "200",
                "0",
            };
        }

        private List<string> GetPrevaluesForImagesFiles()
        {
            return new List<string> {"media", "*", "-1", "False", "1", "1", "-1", "False", "0", "0", "/", "200", "0",};
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
}