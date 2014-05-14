using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Serialization;
using SuperSimpleWebshop.Common;
using SuperSimpleWebshop.Domain.Helpers;

namespace SuperSimpleWebshop.Domain
{
    [Serializable]
    [XmlRoot(ElementName = "OrderInfo")]
    public class StaticOrderInfo
    {
        public int OrderId;

        #region country & language
        public string ShopCountry { get; set; }
        public string ShopLanguage { get; set; }

        [XmlIgnore]
        public CultureInfo CultureInfo
        {
            get
            {
                if (!string.IsNullOrEmpty(ShopCountry) && !string.IsNullOrEmpty(ShopLanguage))
                {
                    return new CultureInfo(string.Format("{0}-{1}", ShopLanguage, ShopCountry));
                }

                return new CultureInfo("en-US");
            }
        }

        [XmlIgnore]
        public ShopAlias ShopAlias
        {
            get
            {
                if (OrderId != 0)
                {
                    Order order = new Order(OrderId);
                    return ShopAliasHelper.GetAllShopAlias().Where(x => x.Alias == order.OrderInfo.ShopAlias.Alias).FirstOrDefault();
                }

                return null;
            }
        }
        #endregion

        /// <summary>
        /// Gets the unique ordernumber for the order
        /// </summary>
        public string OrderNumber;

        /// <summary>
        /// Gets the status of the order
        /// </summary>
        public OrderStatus Status;

        /// <summary>
        /// Gets a list with the orderlines of the order
        /// </summary>
        public List<StaticOrderLine> OrderLines;

        public List<CouponInfo> Coupons;

        public StaticCustomerInfo CustomerInfo;

        public StaticShippingInfo ShippingInfo;

        public StaticPaymentInfo PaymentInfo;

        [XmlIgnore]
        public decimal TotalAmountWithoutTax;

        /// <summary>
        /// Gets the total amount of the order, excluding tax
        /// </summary>
        [XmlElement(ElementName = "TotalAmountWithoutTax")]
        public string TotalAmountWithoutTaxAsString
        {
            get
            {
                return TotalAmountWithoutTax.ToString("C");
            }
            set
            {
                TotalAmountWithoutTax = decimal.Parse(value, NumberStyles.Currency);
            }
        }

        /// <summary>
        /// Gets the total tax of the order, excluding shippingcosts
        /// </summary>
        [XmlIgnore]
        public decimal TotalTax;

        [XmlElement(ElementName = "TotalTax")]
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

        /// <summary>
        /// Gets the total amount of the order, including tax, excluding shippingcosts
        /// </summary>
        [XmlIgnore]
        public decimal TotalAmountWithTax;

        [XmlElement(ElementName = "TotalAmountWithTax")]
        public string TotalAmountWithTaxAsString
        {
            get
            {
                return TotalAmountWithTax.ToString("C");
            }
            set
            {
                TotalAmountWithTax = decimal.Parse(value, NumberStyles.Currency);
            }
        }

        /// <summary>
        /// Gets the shipping costs of the order
        /// </summary>
        [XmlIgnore]
        public decimal ShippingCosts;

        [XmlElement(ElementName = "ShippingCosts")]
        public string ShippingCostsAsString
        {
            get
            {
                return ShippingCosts.ToString("C");
            }
            set
            {
                ShippingCosts = decimal.Parse(value, NumberStyles.Currency);
            }
        }

        /// <summary>
        /// Gets the total amount of the order, including tax, including shippingcosts
        /// </summary>
        [XmlIgnore]
        public decimal TotalAmount;

        [XmlElement(ElementName = "TotalAmount")]
        public string TotalAmountAsString
        {
            get
            {
                return TotalAmount.ToString("C");
            }
            set
            {
                TotalAmount = decimal.Parse(value, NumberStyles.Currency);
            }
        }
    }
}
