using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using SuperSimpleWebshop.Domain.Repositories;

namespace SuperSimpleWebshop.Domain
{
    [Serializable]
    public class PricingVariantInfo
    {
        public int PricingVariantId;




        public string Title
        {
            get
            {
                return PricingVariant.Title;
            }
            set { }
        }

        public string Group
        {
            get
            {
                return PricingVariant.PricingVariantGroup;
            }
            set { }
        }

        private PricingVariant _PricingVariant;

        public PricingVariant PricingVariant
        {
            get
            {
                if (_PricingVariant == null)
                {
                    _PricingVariant = new PricingVariant(PricingVariantId);
                }

                return _PricingVariant;
            }
        }

        [XmlIgnore]
        private VariantSale _VariantSale;
        [XmlIgnore]
        public VariantSale VariantSale
        {
            get
            {
                if (_VariantSale == null)
                {
                    _VariantSale = new CatalogRepository().GetVariantSale(PricingVariantId);
                }

                return _VariantSale;
            }
        }

        //[XmlIgnore]
        //private List<CustomVariantSale> _CustomVariantSales;
        //[XmlIgnore]
        //public List<CustomVariantSale> CustomVariantSales
        //{
        //    get
        //    {
        //        if (_CustomVariantSales == null)
        //        {
        //            _CustomVariantSales = SaleHelper.GetCustomSalesForPricingVariant(PricingVariant);
        //        }
        //        return _CustomVariantSales;
        //    }
        //}

        /// <summary>
        /// Returns if the pricingvariant is discounted
        /// </summary>
        //[XmlIgnore]
        //public bool IsDiscounted
        //{
        //    get
        //    {
        //        if (VariantSale != null || (CustomVariantSales != null && CustomVariantSales.Count > 0))
        //            return true;
        //        else
        //            return false;
        //    }
        //}

        //[XmlIgnore]
        //public decimal DiscountedAmount
        //{
        //    get
        //    {
        //        if (IsDiscounted)
        //        {
        //            if (VariantSale != null)
        //            {
        //                if (VariantSale.DiscountPercentage > 0)
        //                {
        //                    return PricingVariant.AddedValue - ((decimal)VariantSale.DiscountPercentage / 100 * PricingVariant.AddedValue);
        //                }

        //                if (VariantSale.DiscountAmount > 0)
        //                {
        //                    decimal priceWithoutTax = PricingVariant.AddedValue - VariantSale.DiscountAmount;

        //                    return priceWithoutTax;
        //                }
        //            }

        //            //if (CustomVariantSales != null && CustomVariantSales.Count > 0)
        //            //{
        //            //    foreach (var sale in CustomVariantSales)
        //            //    {
        //            //        if (sale.DiscountPercentage > 0)
        //            //        {
        //            //            return PricingVariant.AddedValue - ((decimal)sale.DiscountPercentage / 100 * PricingVariant.AddedValue);
        //            //        }

        //            //        if (sale.DiscountAmount > 0)
        //            //        {
        //            //            return PricingVariant.AddedValue - sale.DiscountAmount;
        //            //        }
        //            //    }
        //            //}
        //        }
        //        else
        //        {
        //            return PricingVariant.AddedValue;
        //        }

        //        return 0;
        //    }
        //}

        [XmlIgnore]
        public decimal AddedValue
        {
            get
            {
                //if (IsDiscounted)
                //{
                //    return DiscountedAmount;
                //}
                //else
                //{
                    return PricingVariant.AddedValue;
                //}
            }
        }

        [XmlElement(ElementName = "AddedValue")]
        public string AddedValueAsString
        {
            get
            {
                return AddedValue.ToString("C");
            }
            set { }
        }

        [XmlElement(ElementName = "AddedValueWithoutCurrencySign")]
        public string AddedValueAsStringWithoutCurrencySign
        {
            get
            {
                return AddedValue.ToString();
            }
            set { }
        }

        [XmlElement(ElementName = "AddedValueWithoutCurrencySignDotSeparator")]
        public string AddedValueAsStringWithoutCurrencySignDotSeparator
        {
            get
            {
                return decimal.Round(AddedValue,2).ToString().Replace(",", ".");
            }
            set { }
        }


    
        [XmlElement(ElementName = "AddedWeight")]
        public string AddedWeightAsString
        {
            get
            {
                return PricingVariant.AddedWeight.ToString();
            }
            set { }
        }
    }
}
