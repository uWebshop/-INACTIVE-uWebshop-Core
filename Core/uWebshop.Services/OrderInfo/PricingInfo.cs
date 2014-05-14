using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using SuperSimpleWebshop.Domain.Repositories;
using umbraco.BusinessLogic;

namespace SuperSimpleWebshop.Domain
{
    public class PricingInfo
    {
        public int PricingId;

        [XmlIgnore]
        private Pricing _Pricing;

        [XmlIgnore]
        public Pricing Pricing
        {
            get
            {
                if (_Pricing == null)
                {
                    _Pricing = new Pricing(PricingId);
                }

                return _Pricing;
            }
        }

        [XmlIgnore]
        private PricingDiscount _PricingDiscount;

        [XmlIgnore]
        public PricingDiscount PricingDiscount
        {
            get
            {
                if (_PricingDiscount == null)
                {
                    _PricingDiscount = new CatalogRepository().GetPricingDiscount(PricingId);
                }

                return _PricingDiscount;
            }
        }

        /// <summary>
        /// Returns if the pricing is discounted
        /// </summary>
        [XmlIgnore]
        public bool IsDiscounted
        {
            get
            {
                if (PricingDiscount != null)
                    return true;
                else
                    return false;
            }
        }

        public string Title
        {
            get
            {
                return Pricing.Title;
            }
            set { }
        }

        public string Number
        {
            get
            {
                return Pricing.Number;
            }
            set { }
        }


        public double Weight
        {
            get
            {
                return Pricing.Weight;
            }
            set { }
        }

        public int Stock
        {
            get
            {
                return Pricing.Stock;
            }
            set { }
        }

        #region price without tax
        public decimal PriceWithoutTax
        {
            get
            {
                decimal priceWithoutTax = 0;

                priceWithoutTax = Pricing.PriceWithoutTax;

                if (ItemCount.HasValue)
                {
                    if (Pricing.Ranges.Count > 0)
                    {
                        var range = Pricing.Ranges.Where(x => x.From <= ItemCount.Value && x.To >= ItemCount.Value).FirstOrDefault();

                        if (range != null)
                        {
                            priceWithoutTax = range.Amount;
                        }
                    }
                }

                foreach (var variant in PricingVariants)
                {
                    priceWithoutTax += variant.AddedValue;
                }

                return priceWithoutTax;
            }
        }

        [XmlElement(ElementName = "PriceWithoutTax")]
        public string PriceWithoutTaxAsString
        {
            get
            {
                return PriceWithoutTax.ToString("C");
            }
            set { }
        }

        [XmlElement(ElementName = "PriceWithoutTaxWithoutCurrencySign")]
        public string PriceWithoutTaxAsStringWithoutCurrencySign
        {
            get
            {
                return PriceWithoutTax.ToString();
            }
            set { }
        }

        [XmlElement(ElementName = "PriceWithoutTaxWithoutCurrencySignDotSeparator")]
        public string PriceWithoutTaxAsStringWithoutCurrencySignDotSeparator
        {
            get
            {
                return decimal.Round(PriceWithoutTax,2).ToString().Replace(",", ".");
            }
            set { }
        }
        #endregion

        #region tax
        public decimal Tax
        {
            get
            {
                decimal tax = CurrentPriceWithoutTax * (Pricing.TaxPercentage / (decimal)100.0);

                return tax;
            }
        }

        [XmlElement(ElementName = "Tax")]
        public string TaxAsString
        {
            get
            {
                return Tax.ToString("C");
            }
            set { }
        }

        [XmlElement(ElementName = "TaxWithoutCurrencySign")]
        public string TaxAsStringWithoutCurrencySign
        {
            get
            {
                return Tax.ToString();
            }
            set { }
        }

        [XmlElement(ElementName = "TaxWithoutCurrencySignDotSeparator")]
        public string TaxWithoutCurrencySignDotSeparator
        {
            get
            {
                return decimal.Round(Tax,2).ToString().Replace(",", ".");
            }
            set { }
        }
        #endregion

        #region price with tax
        public decimal PriceWithTax
        {
            get
            {
                return PriceWithoutTax + Tax;
            }
        }

        [XmlElement(ElementName = "PriceWithTax")]
        public string PriceWithTaxAsString
        {
            get
            {
                return PriceWithTax.ToString("C");
            }
            set { }
        }

        [XmlElement(ElementName = "PriceWithTaxWithoutCurrencySign")]
        public string PriceWithTaxAsStringWithoutCurrencySign
        {
            get
            {
                return PriceWithTax.ToString();
            }
            set { }
        }

        [XmlElement(ElementName = "PriceWithTaxWithoutCurrencySignDotSeparator")]
        public string PriceWithTaxWithoutCurrencySignDotSeparator
        {
            get
            {
                return decimal.Round(PriceWithTax,2).ToString().Replace(",", ".");
            }
            set { }
        }
        #endregion

        public decimal TaxPercentage
        {
            get
            {
                return Pricing.TaxPercentage;
            }
        }

        #region discounted price & tax
        [XmlIgnore]
        public decimal DiscountedPriceWithoutTax
        {
            get
            {
                return DiscountedPriceWithTax / (100 + TaxPercentage) * 100;
            }
        }
        [XmlIgnore]
        public decimal DiscountedTax
        {
            get
            {
                return DiscountedPriceWithTax - DiscountedPriceWithoutTax;
            }
        }

        [XmlIgnore]
        public decimal DiscountedPriceWithTax
        {
            get
            {
                if (IsDiscounted)
                {
                    if (PricingDiscount != null)
                    {
                        if (PricingDiscount.DiscountPercentage > 0)
                        {
                            decimal priceWithTax = Pricing.PriceWithTax - ((decimal)PricingDiscount.DiscountPercentage / 100 * Pricing.PriceWithTax);

                            foreach (var variant in PricingVariants)
                            {
                                priceWithTax += variant.AddedValue;
                            }

                            return priceWithTax;

                        }

                        if (PricingDiscount.DiscountAmount > 0)
                        {
                            decimal priceWithTax = Pricing.PriceWithTax - PricingDiscount.DiscountAmount;

                            foreach (var variant in PricingVariants)
                            {
                                priceWithTax += variant.AddedValue;
                            }

                            return priceWithTax;
                        }
                    }
                }
                else
                {
                    return PriceWithTax;
                }

                return 0;
            }
        }
        #endregion

        #region current price
        [XmlIgnore]
        public decimal CurrentPriceWithTax
        {
            get
            {
                if (IsDiscounted)
                {
                    return DiscountedPriceWithTax;
                }
                else
                {
                    return PriceWithTax;
                }
            }
        }

        [XmlElement(ElementName = "CurrentPriceWithTax")]
        public string CurrentPriceWithTaxAsString
        {
            get
            {
                return CurrentPriceWithTax.ToString("C");
            }
            set { }
        }

        [XmlElement(ElementName = "CurrentPriceWithTaxWithoutCurrencySign")]
        public string CurrentPriceWithTaxAsStringWithoutCurrencySign
        {
            get
            {
                return CurrentPriceWithTax.ToString();
            }
            set { }
        }

        [XmlElement(ElementName = "CurrentPriceWithTaxWithoutCurrencySignDotSeparator")]
        public string CurrentPriceWithTaxWithoutCurrencySignDotSeparator
        {
            get
            {
                return decimal.Round(CurrentPriceWithTax,2).ToString().Replace(",", ".");
            }
            set { }
        }

        [XmlIgnore]
        public decimal CurrentPriceWithoutTax
        {
            get
            {
                //Log.Add(LogTypes.Debug, 0, "discounted " + IsDiscounted + " " + DiscountedPriceWithoutTax + " " + PriceWithoutTax);

                if (IsDiscounted)
                {
                    return DiscountedPriceWithoutTax;
                }
                else
                {
                    return PriceWithoutTax;
                }
            }
        }

        [XmlElement(ElementName = "CurrentPriceWithoutTax")]
        public string CurrentPriceWithoutTaxAsString
        {
            get
            {
                return CurrentPriceWithoutTax.ToString("C");
            }
            set { }
        }

        [XmlElement(ElementName = "CurrentPriceWithoutTaxWithoutCurrencySign")]
        public string CurrentPriceWithoutTaxAsStringWithoutCurrencySign
        {
            get
            {
                return CurrentPriceWithoutTax.ToString();
            }
            set { }
        }

        [XmlElement(ElementName = "CurrentPriceWithoutTaxWithoutCurrencySignDotSeparator")]
        public string CurrentPriceWithoutTaxWithoutCurrencySignDotSeparator
        {
            get
            {
                return decimal.Round(CurrentPriceWithoutTax,2).ToString().Replace(",", ".");
            }
            set { }
        }
        #endregion

        public List<PricingVariantInfo> PricingVariants;

        public PricingInfo()
        {
            PricingVariants = new List<PricingVariantInfo>();
        }

        public int? ItemCount { get; set; }
    }
}
