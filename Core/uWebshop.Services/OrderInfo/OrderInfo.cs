using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Serialization;
using SuperSimpleWebshop.Common;
using SuperSimpleWebshop.Domain.Helpers;
using SuperSimpleWebshop.Domain.Repositories;
using umbraco.BusinessLogic;

namespace SuperSimpleWebshop.Domain
{
    [Serializable]
    public class OrderInfo
    {
        public int OrderId;

        #region country & language
        public string ShopCountry { get; set; }
        public string ShopLanguage { get; set; }
        public string Alias { get; set; }

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
                if (!string.IsNullOrEmpty(Alias))
                {
                    ShopAlias shopAlias = new ShopAlias(ShopAliasHelper.GetAllShopAlias().Where(x => x.Alias == Alias).FirstOrDefault().Id);
                    return shopAlias;
                }

                return null;
            }
        }
        #endregion

        /// <summary>
        /// Gets the unique ordernumber for the order
        /// </summary>
        public string OrderNumber;

        public string UniqueOrderId;

        /// <summary>
        /// Gets the status of the order
        /// </summary>
        public OrderStatus Status;

        /// <summary>
        /// Gets a list with the orderlines of the order
        /// </summary>
        public List<OrderLine> OrderLines;

        public List<CouponInfo> Coupons;

        public CustomerInfo CustomerInfo;

        public ShippingInfo ShippingInfo;

        public PaymentInfo PaymentInfo;

        /// <summary>
        /// Are shippingcosts still up to date?
        /// TRUE = shipping costs needs to be updated
        /// FALSE = shipping costs are correct!
        /// 
        /// </summary>
        public bool UpdateShippingCosts;

        public OrderInfo()
        {
            OrderLines = new List<OrderLine>();
            Coupons = new List<CouponInfo>();

            CustomerInfo = new CustomerInfo();
            ShippingInfo = new ShippingInfo();
            PaymentInfo = new PaymentInfo();

            OrderValidationErrors = new List<OrderValidationError>();
        }

        private string _CustomerVATNumber;

        /// <summary>
        /// Gets the total amount of the order, excluding tax, including shippingcosts, includingn paymentcosts
        /// </summary>
        public decimal TotalAmountWithoutTax
        {
            get
            {
                decimal orderTotalWithoutTax = 0;

                foreach (var orderline in OrderLines)
                {
                    orderTotalWithoutTax += orderline.TotalPriceWithoutTax;
                }

                var oldTotalAmount = orderTotalWithoutTax;

                #region subtract coupon discounts from total amount
                orderTotalWithoutTax -= DiscountAmount;

                foreach (var coupon in Coupons)
                {
                    switch (coupon.CouponDiscount.CouponType)
                    {
                        case CouponType.Percentage:
                            orderTotalWithoutTax = oldTotalAmount - (coupon.CouponDiscount.DiscountPercentage / 100m * oldTotalAmount);
                            break;
                        case CouponType.Amount:
                            orderTotalWithoutTax = oldTotalAmount - coupon.CouponDiscount.DiscountAmount;
                            break;
                        case CouponType.Free_shipping:
                            orderTotalWithoutTax -= ShippingProviderCostsWithoutTax;
                            break;
                    }
                }
                #endregion

                orderTotalWithoutTax += ShippingProviderCostsWithoutTax;

                orderTotalWithoutTax += PaymentProviderCostsWithoutTax;

                return orderTotalWithoutTax;
            }
        }

        [XmlElement(ElementName = "TotalAmountWithoutTax")]
        public string TotalAmountWithoutTaxAsString
        {
            get
            {
                return TotalAmountWithoutTax.ToString("C");
            }
            set { }
        }

        [XmlElement(ElementName = "TotalAmountWithoutTaxWithoutCurrencySign")]
        public string TotalAmountWithoutTaxAsStringWithoutCurrencySign
        {
            get
            {
                return TotalAmountWithoutTax.ToString("N");
            }
            set { }
        }

        [XmlElement(ElementName = "TotalAmountWithoutTaxWithoutCurrencySignDotSeparator")]
        public string TotalAmountWithoutTaxAsStringWithoutCurrencySignDotSeparator
        {
            get
            {
                return decimal.Round(TotalAmountWithoutTax, 2).ToString().Replace(",", ".");
            }
            set { }
        }



        /// <summary>
        /// Gets the total tax of the order, excluding shippingcosts
        /// </summary>
        public decimal TotalTax
        {
            get
            {
                decimal orderTotalTax = 0;

                foreach (var orderline in OrderLines)
                {
                    orderTotalTax += orderline.TotalTax;
                }

                orderTotalTax += ShippingProviderTaxAmount;
                orderTotalTax += PaymentProviderTaxAmount;

                return orderTotalTax;
            }
        }

        [XmlElement(ElementName = "TotalTax")]
        public string TotalTaxAsString
        {
            get
            {
                return TotalTax.ToString("C");
            }
            set { }
        }

        [XmlElement(ElementName = "TotalTaxWithoutCurrencySign")]
        public string TotalTaxAsStringWithoutCurrencySign
        {
            get
            {
                return TotalTax.ToString("N");
            }
            set { }
        }

        [XmlElement(ElementName = "TotalTaxWithoutCurrencySignDotSeparator")]
        public string TotalTaxAsStringWithoutCurrencySignDotSeparator
        {
            get
            {
                return decimal.Round(TotalTax, 2).ToString().Replace(",", ".");
            }
            set { }
        }

        /// <summary>
        /// Gets the total amount of the order, including tax, including shippingcosts
        /// </summary>
        public decimal TotalAmountWithTax
        {
            get
            {
                decimal orderTotalWithTax = 0;

                foreach (var orderline in OrderLines)
                {
                    orderTotalWithTax += orderline.TotalPriceWithTax;
                }

                var oldTotalAmount = orderTotalWithTax;

                #region subtract coupon discounts from total amount
                orderTotalWithTax -= DiscountAmount;

                foreach (var coupon in Coupons)
                {
                    switch (coupon.CouponDiscount.CouponType)
                    {
                        case CouponType.Percentage:
                            orderTotalWithTax = oldTotalAmount - (coupon.CouponDiscount.DiscountPercentage / 100m * oldTotalAmount);
                            break;
                        case CouponType.Amount:
                            orderTotalWithTax = oldTotalAmount - coupon.CouponDiscount.DiscountAmount;
                            break;
                        case CouponType.Free_shipping:
                            orderTotalWithTax -= ShippingProviderCostsWithTax;
                            break;
                    }
                }
                #endregion

                orderTotalWithTax += ShippingProviderCostsWithTax;
                orderTotalWithTax += PaymentProviderCostsWithTax;

                #region load RegionalTax
                if (ShopAlias != null)
                {
                    var regions = ShippingHelper.GetAllRegionsForShopAlias(ShopAlias.Alias);
                    if (regions != null)
                    {
                        var region = regions.Where(x => x.Code == CustomerInfo.RegionCode).FirstOrDefault();
                        if (region != null)
                        {
                            decimal tax = 0;
                            if (decimal.TryParse(region.Tax, out tax))
                            {
                                var regionTax = orderTotalWithTax / 100 * tax;
                                orderTotalWithTax += regionTax;
                            }
                        }
                        else
                        {
                            //Log.Add(LogTypes.Debug, 0, "Customer does not have a (valid) region");
                        }
                    }
                }
                #endregion

                return orderTotalWithTax;
            }
        }

        [XmlElement(ElementName = "TotalAmountWithTax")]
        public string TotalAmountWithTaxAsString
        {
            get
            {
                return TotalAmountWithTax.ToString("C");
            }
            set { }
        }

        [XmlElement(ElementName = "TotalAmountWithTaxWithoutCurrencySign")]
        public string TotalAmountWithTaxAsStringWithoutCurrencySign
        {
            get
            {
                return TotalAmountWithTax.ToString("N");
            }
            set { }
        }

        [XmlElement(ElementName = "TotalAmountWithTaxWithoutCurrencySignDotSeparator")]
        public string TotalAmountWithTaxAsStringWithoutCurrencySignDotSeparator
        {
            get
            {
                //Log.Add(LogTypes.Debug, 0, "TotalAmountWithTax: " + TotalAmountWithTax.ToString().Replace(",", "."));
                ////string newvalue = Regex.Replace(TotalAmountWithTax.ToString(), "[^0-9]", string.Empty);

                ////Log.Add(LogTypes.Debug, 0, "newvalue: " + newvalue);

                ////Log.Add(LogTypes.Debug, 0, "return Convert.ToString(int.Parse(newvalue) / 100)" + Convert.ToString(int.Parse(newvalue) / 100));

                return decimal.Round(TotalAmountWithTax, 2).ToString().Replace(",", ".");
            }
            set { }
        }

        /// <summary>
        /// Gets the regionaltax for this order
        /// </summary>
        [XmlIgnore]
        public decimal RegionalTax
        {
            get
            {
                decimal regionTax = 0;
                decimal orderTotalWithTax = 0;

                foreach (var orderline in OrderLines)
                {
                    orderTotalWithTax += orderline.TotalPriceWithTax;
                }

                var oldTotalAmount = orderTotalWithTax;

                #region subtract coupon discounts from total amount
                orderTotalWithTax -= DiscountAmount;

                foreach (var coupon in Coupons)
                {
                    switch (coupon.CouponDiscount.CouponType)
                    {
                        case CouponType.Percentage:
                            orderTotalWithTax = oldTotalAmount - (coupon.CouponDiscount.DiscountPercentage / 100m * oldTotalAmount);
                            break;
                        case CouponType.Amount:
                            orderTotalWithTax = oldTotalAmount - coupon.CouponDiscount.DiscountAmount;
                            break;
                        case CouponType.Free_shipping:
                            orderTotalWithTax -= ShippingProviderCostsWithTax;
                            break;
                    }
                }
                #endregion


                orderTotalWithTax += ShippingProviderCostsWithTax;
                orderTotalWithTax += PaymentProviderCostsWithTax;

                #region load RegionalTax
                if (ShopAlias != null)
                {
                    var regions = ShippingHelper.GetAllRegionsForShopAlias(ShopAlias.Alias);
                    if (regions != null)
                    {
                        var region = regions.Where(x => x.Code == CustomerInfo.RegionCode).FirstOrDefault();
                        if (region != null)
                        {
                            decimal tax = 0;
                            if (decimal.TryParse(region.Tax, out tax))
                            {
                                regionTax = orderTotalWithTax / 100 * tax;
                            }
                        }
                        else
                        {
                            //Log.Add(LogTypes.Debug, 0, "Customer does not have a (valid) region");
                        }
                    }
                }
                #endregion


                return regionTax;
            }
        }

        [XmlElement(ElementName = "RegionalTax")]
        public string RegionalTaxAsString
        {
            get
            {
                return RegionalTax.ToString("C");
            }
            set { }
        }

        [XmlElement(ElementName = "RegionalTaxWithoutCurrencySign")]
        public string RegionalTaxAsStringWithoutCurrencySign
        {
            get
            {
                return RegionalTax.ToString("N");
            }
            set { }
        }

        [XmlElement(ElementName = "RegionalTaxWithoutCurrencySignDotSeparator")]
        public string RegionalTaxAsStringWithoutCurrencySignDotSeparator
        {
            get
            {
                return decimal.Round(RegionalTax, 2).ToString().Replace(",", ".");
            }
            set { }
        }

        /// <summary>
        /// Gets the shipping costs of the order without Tax
        /// </summary>
        public decimal ShippingProviderAmount;
       
        public decimal ShippingProviderTaxPercentage;

        public decimal ShippingProviderTaxAmount
        {
            get
            {
                return ShippingProviderCostsWithoutTax * (ShippingProviderTaxPercentage / (decimal)100.0);
            }
        }

        public decimal ShippingProviderCostsWithoutTax
        {
            get
            {
                var pricesIncludingVAT = new Settings().GetMultiLanguageSetting(Settings.PRICES_INCLUDING_VAT);

                if (pricesIncludingVAT != null && pricesIncludingVAT.ToString() != string.Empty && pricesIncludingVAT.ToString() == "1")
                {
                    return (ShippingProviderAmount / (100 + ShippingProviderTaxPercentage)) * 100;
                }
                else
                {
                    return ShippingProviderAmount;
                }
            }
        }

        public decimal ShippingProviderCostsWithTax
        {
            get
            {
                var pricesIncludingVAT = new Settings().GetMultiLanguageSetting(Settings.PRICES_INCLUDING_VAT);

                if (pricesIncludingVAT != null && pricesIncludingVAT.ToString() != string.Empty && pricesIncludingVAT.ToString() == "1")
                {
                    return ShippingProviderAmount;
                }
                else
                {
                    return (ShippingProviderAmount * (100 + ShippingProviderTaxPercentage)) / 100;
                }
            }
        }


        [XmlElement(ElementName = "ShippingProviderTax")]
        public string ShippingProviderTaxAsString
        {
            get
            {
                return (ShippingProviderCostsWithoutTax * (ShippingProviderTaxPercentage / (decimal)100.0)).ToString("C");
            }
            set { }
        }

        [XmlElement(ElementName = "ShippingProviderTaxWithoutCurrencySign")]
        public string ShippingProviderTaxAsStringWithoutCurrencySign
        {
            get
            {
                return (ShippingProviderCostsWithoutTax * (ShippingProviderTaxPercentage / (decimal)100.0)).ToString("N");
            }
            set { }
        }


        [XmlElement(ElementName = "ShippingProviderTaxWithoutCurrencySignDotSeparator")]
        public string ShippingProviderTaxAsStringWithoutCurrencySignDotSeparator
        {
            get
            {
                return (ShippingProviderCostsWithoutTax * (ShippingProviderTaxPercentage / (decimal)100.0)).ToString().Replace(",", ".");
            }
            set { }
        }

        [XmlElement(ElementName = "ShippingCosts")]
        public string ShippingProviderCostsAsString
        {
            get
            {
                return ShippingProviderCostsWithTax.ToString("C");
            }
            set
            {
            }
        }


        [XmlElement(ElementName = "ShippingProviderCostsWithTax")]
        public string ShippingProviderCostsWithTaxAsString
        {
            get
            {
                return ShippingProviderCostsWithTax.ToString("C");
            }
            set
            {
            }
        }


        [XmlElement(ElementName = "ShippingProviderCostsWithTaxWithoutCurrencySign")]
        public string ShippingProviderCostsWithTaxAsStringWithoutCurrencySign
        {
            get
            {
                return ShippingProviderCostsWithTax.ToString("N");
            }
            set { }
        }

        [XmlElement(ElementName = "ShippingProviderCostsWithTaxWithoutCurrencySignDotSeparator")]
        public string ShippingProviderCostsWithTaxAsStringWithoutCurrencySignDotSeparator
        {
            get
            {
                return decimal.Round(ShippingProviderCostsWithTax, 2).ToString().Replace(",", ".");
            }
            set { }
        }

        [XmlElement(ElementName = "ShippingProviderCostsWithoutTax")]
        public string ShippingProviderCostsWithoutTaxAsString
        {
            get
            {
                return ShippingProviderCostsWithoutTax.ToString("C");
            }
            set { }
        }


        [XmlElement(ElementName = "ShippingProviderCostsWithoutTaxWithoutCurrencySign")]
        public string ShippingProviderCostsWithoutTaxAsStringWithoutCurrencySign
        {
            get
            {
                return ShippingProviderCostsWithoutTax.ToString("N");
            }
            set { }
        }

        [XmlElement(ElementName = "ShippingProviderCostsWithoutTaxWithoutCurrencySignDotSeparator")]
        public string ShippingProviderCostsWithoutTaxAsStringWithoutCurrencySignDotSeparator
        {
            get
            {
                return decimal.Round(ShippingProviderCostsWithoutTax, 2).ToString().Replace(",", ".");
            }
            set { }
        }


        /// <summary>
        /// Gets the payment provider costs of the order
        /// </summary>      
        public decimal PaymentProviderAmount;
      
        public decimal PaymentProviderTaxPercentage;

        public decimal PaymentProviderTaxAmount
        {
            get
            {
                return PaymentProviderCostsWithoutTax * (PaymentProviderTaxPercentage / (decimal)100.0);
            }
        }
 
        public decimal PaymentProviderCostsWithoutTax
        {
            get
            {
                var pricesIncludingVAT = new Settings().GetMultiLanguageSetting(Settings.PRICES_INCLUDING_VAT);

                if (pricesIncludingVAT != null && pricesIncludingVAT.ToString() != string.Empty && pricesIncludingVAT.ToString() == "1")
                {
                    return (PaymentProviderAmount / (100 + PaymentProviderTaxPercentage)) * 100;
                }
                else
                {
                    return PaymentProviderAmount;
                }
            }
        }

        public decimal PaymentProviderCostsWithTax
        {
            get
            {
                var pricesIncludingVAT = new Settings().GetMultiLanguageSetting(Settings.PRICES_INCLUDING_VAT);

                if (pricesIncludingVAT != null && pricesIncludingVAT.ToString() != string.Empty && pricesIncludingVAT.ToString() == "1")
                {
                    return PaymentProviderAmount;
                }
                else
                {
                    return (PaymentProviderAmount * (100 + PaymentProviderTaxPercentage)) / 100;
                }
            }
        }

        [XmlElement(ElementName = "PaymentProviderTax")]
        public string PaymentProviderTaxAsString
        {
            get
            {
                return (PaymentProviderCostsWithoutTax * (PaymentProviderTaxPercentage / (decimal)100.0)).ToString("C");
            }
            set { }
        }

        [XmlElement(ElementName = "PaymentProviderTaxWithoutCurrencySign")]
        public string PaymentProviderTaxAsStringWithoutCurrencySign
        {
            get
            {
                return (PaymentProviderCostsWithoutTax * (PaymentProviderTaxPercentage / (decimal)100.0)).ToString("N");
            }
            set { }
        }


        [XmlElement(ElementName = "PaymentProviderTaxWithoutCurrencySignDotSeparator")]
        public string PaymentProviderTaxAsStringWithoutCurrencySignDotSeparator
        {
            get
            {
                return (PaymentProviderCostsWithoutTax * (PaymentProviderTaxPercentage / (decimal)100.0)).ToString().Replace(",", ".");
            }
            set { }
        }

        [XmlElement(ElementName = "PaymentCosts")]
        public string PaymentProviderCostsAsString
        {
            get
            {
                return PaymentProviderCostsWithTax.ToString("C");
            }
            set
            {
            }
        }

        [XmlElement(ElementName = "PaymentProviderCostsWithTax")]
        public string PaymentProviderCostsWithTaxAsString
        {
            get
            {
                return PaymentProviderCostsWithTax.ToString("C");
            }
            set
            {
            }
        }


        [XmlElement(ElementName = "PaymentProviderCostsWithTaxWithoutCurrencySign")]
        public string SPaymentProviderCostsWithTaxAsStringWithoutCurrencySign
        {
            get
            {
                return PaymentProviderCostsWithTax.ToString("N");
            }
            set { }
        }

        [XmlElement(ElementName = "PaymentProviderCostsWithTaxWithoutCurrencySignDotSeparator")]
        public string PaymentProviderCostsWithTaxAsStringWithoutCurrencySignDotSeparator
        {
            get
            {
                return decimal.Round(PaymentProviderCostsWithTax, 2).ToString().Replace(",", ".");
            }
            set { }
        }

        [XmlElement(ElementName = "PaymentProviderCostsWithoutTax")]
        public string PaymentProviderCostsWithoutTaxAsString
        {
            get
            {
                return PaymentProviderCostsWithoutTax.ToString("C");
            }
            set { }
        }


        [XmlElement(ElementName = "PaymentProviderCostsWithoutTaxWithoutCurrencySign")]
        public string PaymentProviderCostsWithoutTaxAsStringWithoutCurrencySign
        {
            get
            {
                return PaymentProviderCostsWithoutTax.ToString("N");
            }
            set { }
        }

        [XmlElement(ElementName = "PaymentProviderCostsWithoutTaxWithoutCurrencySignDotSeparator")]
        public string PaymentProviderCostsWithoutTaxAsStringWithoutCurrencySignDotSeparator
        {
            get
            {
                return decimal.Round(PaymentProviderCostsWithoutTax, 2).ToString().Replace(",", ".");
            }
            set { }
        }

        
        /// <summary>
        /// Gets the total amount of the order, including tax, including shippingcosts, including paymentcosts
        /// </summary>
        [XmlIgnore]
        public decimal TotalAmount
        {
            get
            {
                decimal totalAmount = 0;

                foreach (var orderLine in OrderLines)
                {
                    totalAmount += orderLine.TotalPriceWithTax;
                }

                var oldTotalAmount = totalAmount;

                #region subtract coupon discounts from total amount
                totalAmount -= DiscountAmount;

                foreach (var coupon in Coupons)
                {
                    switch (coupon.CouponDiscount.CouponType)
                    {
                        case CouponType.Percentage:
                            totalAmount = oldTotalAmount - (coupon.CouponDiscount.DiscountPercentage / 100m * oldTotalAmount);
                            break;
                        case CouponType.Amount:
                            totalAmount = oldTotalAmount - coupon.CouponDiscount.DiscountAmount;
                            break;
                        case CouponType.Free_shipping:
                            totalAmount -= ShippingProviderCostsWithTax;
                            break;
                    }
                }
                #endregion

                totalAmount += ShippingProviderCostsWithTax;

                totalAmount += PaymentProviderCostsWithTax;


                return totalAmount;
            }
            set { }
        }

        [XmlElement(ElementName = "TotalAmount")]
        public string TotalAmountAsString
        {
            get
            {
                return TotalAmount.ToString("C");
            }
            set { }
        }

        [XmlElement(ElementName = "TotalAmountWithoutCurrencySign")]
        public string TotalAmountAsStringWithoutCurrencySign
        {
            get
            {

                return TotalAmount.ToString("N");
            }
            set { }
        }

        [XmlElement(ElementName = "TotalAmountWithoutCurrencySignDotSeparator")]
        public string TotalAmountAsStringWithoutCurrencySignDotSeparator
        {
            get
            {
                return decimal.Round(TotalAmount, 2).ToString().Replace(",", ".");
            }
            set { }
        }

        /// <summary>
        /// Gets the subtotal of the order, including tax excluding shipping and coupons
        /// </summary>
        public decimal SubTotalAmountWithTax
        {
            get
            {
                decimal orderSubTotalWithTax = 0;

                foreach (var orderline in OrderLines)
                {
                    orderSubTotalWithTax += orderline.TotalPriceWithTax;
                }

                var oldTotalAmount = orderSubTotalWithTax;

                return orderSubTotalWithTax;
            }
        }

        [XmlElement(ElementName = "SubTotalAmountWithTax")]
        public string SubTotalAmountWithTaxAsString
        {
            get
            {
                return SubTotalAmountWithTax.ToString("C");
            }
            set { }
        }

        [XmlElement(ElementName = "SubTotalAmountWithTaxWithoutCurrencySign")]
        public string SubTotalAmountWithTaxAsStringWithoutCurrencySign
        {
            get
            {
                return SubTotalAmountWithTax.ToString("N");
            }
            set { }
        }

        [XmlElement(ElementName = "SubTotalAmountWithTaxWithoutCurrencySignDotSeparator")]
        public string SubTotalAmountWithTaxAsStringWithoutCurrencySignDotSeparator
        {
            get
            {
                return SubTotalAmountWithTax.ToString("N").Replace(",", ".");
            }
            set { }
        }




        /// <summary>
        /// Gets the subtotal of the order, excluding tax, shipping and coupons
        /// </summary>
        public decimal SubTotalAmountWithoutTax
        {
            get
            {
                decimal orderSubTotalWithoutTax = 0;

                foreach (var orderline in OrderLines)
                {
                    orderSubTotalWithoutTax += orderline.TotalPriceWithoutTax;
                }

                var oldTotalAmount = orderSubTotalWithoutTax;

                return orderSubTotalWithoutTax;
            }
        }

        [XmlElement(ElementName = "SubTotalAmountWithoutTax")]
        public string SubTotalAmountWithoutTaxAsString
        {
            get
            {
                return SubTotalAmountWithoutTax.ToString("C");
            }
            set { }
        }

        [XmlElement(ElementName = "SubTotalAmountWithoutTaxWithoutCurrencySign")]
        public string SubTotalAmountWithoutTaxAsStringWithoutCurrencySign
        {
            get
            {
                return SubTotalAmountWithoutTax.ToString("N");
            }
            set { }
        }

        [XmlElement(ElementName = "SubTotalAmountWithoutTaxWithoutCurrencySignDotSeparator")]
        public string SubTotalAmountWithoutTaxAsStringWithoutCurrencySignDotSeparator
        {
            get
            {
                return SubTotalAmountWithoutTax.ToString("N").Replace(",", ".");
            }
            set { }
        }

        [XmlIgnore]
        private OrderDiscount _OrderDiscount;

        [XmlIgnore]
        public OrderDiscount OrderDiscount
        {
            get
            {
                if (_OrderDiscount == null)
                {
                    _OrderDiscount = new CatalogRepository().GetOrderDiscount();
                }

                return _OrderDiscount;
            }
        }

        public string IsDiscounted
        {
            get
            {
                return (OrderDiscount != null).ToString();
            }
            set { }
        }

        [XmlIgnore]
        public decimal DiscountAmount
        {
            get
            {
                decimal orderTotalWithTax = 0;

                foreach (var orderline in OrderLines)
                {
                    orderTotalWithTax += orderline.TotalPriceWithTax;
                }

                if (OrderDiscount != null)
                {
                    if (OrderDiscount.DiscountAmount != 0m)
                    {
                        return OrderDiscount.DiscountAmount;
                    }
                    else
                    {
                        return (OrderDiscount.DiscountPercentage / 100m * orderTotalWithTax);
                    }
                }

                return 0m;
            }
            set { }
        }

        [XmlElement(ElementName = "DiscountAmount")]
        public string DiscountAmountAsString
        {
            get
            {
                return DiscountAmount.ToString("C");
            }
            set { }
        }

        [XmlElement(ElementName = "DiscountAmountWithoutCurrencySign")]
        public string DiscountAmountAsStringWithoutCurrencySign
        {
            get
            {
                return DiscountAmount.ToString("N");
            }
            set { }
        }

        [XmlElement(ElementName = "DiscountAmountWithoutCurrencySignDotSeparator")]
        public string DiscountAmountAsStringWithoutCurrencySignDotSeparator
        {
            get
            {
                return DiscountAmount.ToString("N").Replace(",", "."); ;
            }
            set { }
        }

        public List<OrderValidationError> OrderValidationErrors { get; set; }
    }
}
