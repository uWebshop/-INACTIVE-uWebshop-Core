using System;
using System.Globalization;
using System.Xml.Serialization;

namespace SuperSimpleWebshop.Domain
{
    [Serializable]
    public class StaticOrderLine
    {
        public PricingInfo PricingInfo;
        public int ProductId;
        public string ProductTitle;
        public int ItemCount;

        #region total price without tax
        public decimal TotalPriceWithoutTax;

        [XmlElement(ElementName = "OrderlineAmountWithoutTax")]
        public string TotalPriceWithoutTaxAsString
        {
            get
            {
                return TotalPriceWithoutTax.ToString("C");
            }
            set
            {
                TotalPriceWithoutTax = decimal.Parse(value, NumberStyles.Currency);
            }
        }
        #endregion

        #region total tax
        public decimal TotalTax;

        [XmlElement(ElementName = "OrderlineTax")]
        public string TotalTaxAsString
        {
            get
            {
                return TotalTax.ToString("C");
            }
            set
            {
                TotalTax = decimal.Parse(value, NumberStyles.Currency);
            }
        }
        #endregion

        #region total price with tax
        public decimal TotalPriceWithTax;

        [XmlElement(ElementName = "OrderlineAmountWithTax")]
        public string TotalPriceWithTaxString
        {
            get
            {
                return TotalPriceWithTax.ToString("C");
            }
            set
            {
                TotalPriceWithTax = decimal.Parse(value, NumberStyles.Currency);
            }
        }
        #endregion

        #region constructors
        public StaticOrderLine()
        {
            PricingInfo = new PricingInfo();
        }
        #endregion
    }
}
