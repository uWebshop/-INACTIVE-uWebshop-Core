using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SuperSimpleWebshop.Domain
{
    [Serializable]
    public class OrderLine
    {
        public PricingInfo PricingInfo;
        public int ProductId;
        public string ProductTitle;
        public int ItemCount;

        #region total weight
        public double ItemWeight
        {
            get
            {
                return PricingInfo.Weight * ItemCount;
            }
        }

        [XmlElement(ElementName = "OrderlineWeight")]
        public string ItemWeightAsString
        {
            get
            {
                return ItemWeight.ToString();
            }
            set { }
        }
        #endregion

        #region total price without tax
        public decimal TotalPriceWithoutTax
        {
            get
            {
                return PricingInfo.CurrentPriceWithoutTax * ItemCount;
            }
        }

        [XmlElement(ElementName = "OrderlineAmountWithoutTax")]
        public string TotalPriceWithoutTaxAsString
        {
            get
            {
                return TotalPriceWithoutTax.ToString("C");
            }
            set { }
        }

        [XmlElement(ElementName = "OrderlineAmountWithoutTaxWithoutCurrencySign")]
        public string TotalPriceWithoutTaxAsStringWithoutCurrencySign
        {
            get
            {
                return TotalPriceWithoutTax.ToString("N");
            }
            set { }
        }

        [XmlElement(ElementName = "OrderlineAmountWithoutTaxWithoutCurrencySignDotSeparator")]
        public string TotalPriceWithoutTaxAsStringWithoutCurrencySignDotSeparator
        {
            get
            {
                return decimal.Round(TotalPriceWithoutTax, 2).ToString().Replace(",", ".");
            }
            set { }
        }
        #endregion

        #region total tax
        public decimal TotalTax
        {
            get
            {
                return PricingInfo.Tax * ItemCount;
            }
        }

        [XmlElement(ElementName = "OrderlineTax")]
        public string TotalTaxAsString
        {
            get
            {
                return TotalTax.ToString("C");
            }
            set { }
        }

        [XmlElement(ElementName = "OrderlineTaxWithoutCurrencySign")]
        public string TotalTaxAsStringWithoutCurrencySign
        {
            get
            {
                return TotalTax.ToString("N");
            }
            set { }
        }

        [XmlElement(ElementName = "OrderlineTaxWithoutCurrencySignDotSeparator")]
        public string TotalTaxAsStringWithoutCurrencySignDotSeparator
        {
            get
            {
                return decimal.Round(TotalTax, 2).ToString().Replace(",", ".");
            }
            set { }
        }
        #endregion

        #region total price with tax
        public decimal TotalPriceWithTax
        {
            get
            {
                return PricingInfo.CurrentPriceWithTax * ItemCount;
            }
        }

        [XmlElement(ElementName = "OrderlineAmountWithTax")]
        public string TotalPriceWithTaxAsString
        {
            get
            {
                return TotalPriceWithTax.ToString("C");
            }
            set { }
        }

        [XmlElement(ElementName = "OrderlineAmountWithTaxWithoutCurrencySign")]
        public string TotalPriceWithTaxAsStringWithoutCurrencySign
        {
            get
            {
                return TotalPriceWithTax.ToString("N");
            }
            set { }
        }

        [XmlElement(ElementName = "OrderlineAmountWithTaxWithoutCurrencySignDotSeparator")]
        public string TotalPriceWithTaxAsStringWithoutCurrencySignDotSeparator
        {
            get
            {
                return decimal.Round(TotalPriceWithTax, 2).ToString().Replace(",", ".");
            }
            set { }
        }
        #endregion

        public string Text { get; set; }
        public int ImageId { get; set; }

        #region constructors
        public OrderLine()
        {
            PricingInfo = new PricingInfo();
        }

        /// <summary>
        /// Initializes a new instance of the SuperSimpleWebshop.Domain.OrderLine class
        /// </summary>
        /// <param name="id">NodeId of the pricing</param>
        /// <param name="itemCount">Amount of items</param>
        public OrderLine(int id, int itemCount)
        {
            PricingInfo = new PricingInfo();
            PricingInfo.PricingId = id;

            ProductId = PricingInfo.Pricing.Product.Id;
            ProductTitle = PricingInfo.Pricing.Product.Title;

            ItemCount = itemCount;

            PricingInfo.ItemCount = itemCount;
        }

        /// <summary>
        /// Initializes a new instance of the SuperSimpleWebshop.Domain.OrderLine class
        /// </summary>
        /// <param name="id">NodeId of the pricing</param>
        /// <param name="variants">Pricing variants of the pricing</param>
        /// <param name="itemCount">Amount of items</param>
        public OrderLine(int id, List<int> variants, int itemCount)
        {
            PricingInfo = new PricingInfo();
            PricingInfo.PricingId = id;

            #region fill retrieve pricing variants
            PricingInfo.PricingVariants = new List<PricingVariantInfo>();

            if (variants != null && variants.Count > 0)
            {
                foreach (var variantId in variants)
                {
                    PricingInfo.PricingVariants.Add(
                        new PricingVariantInfo
                        {
                            PricingVariantId = variantId
                        }
                    );
                }
            }
            #endregion

            ProductId = PricingInfo.Pricing.Product.Id;
            ProductTitle = PricingInfo.Pricing.Product.Title;

            ItemCount = itemCount;

            PricingInfo.ItemCount = itemCount;
        }
        #endregion
    }
}
