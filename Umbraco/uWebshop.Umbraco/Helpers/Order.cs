using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Umbraco
{
    public class OrderData
    {
        public Guid uniqueId { get; set; }
        public string email { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string orderNumber { get; set; }
        public int refrenceId { get; set; }
        public string transactionId { get; set; }
        public string orderStatus { get; set; }
        public string storeAlias { get; set; }
        public int customerId { get; set; }
        public DateTime createDate { get; set; }
        public DateTime updateDate { get; set; }
        public DateTime confirmDate { get; set; }
        public decimal chargedAmount { get; set; }
        public string chargedAmountCurrencyFormat { get; set; }
        public int quantity { get; set; }
        public string currency { get; set; }
        public int orderlines { get; set; }
        public decimal chargedShippingAmount { get; set; }
        public bool vatCharged { get; set; }
        public string culture { get; set; }
        //public IOrder order { get; set; }
        public string orderNodeId { get; set; }
        public OrderCustomer customer { get; set; }
        public OrderCustomer shipping { get; set; }
        public List<OrderOrderLineProduct> products = new List<OrderOrderLineProduct>();
        public OrderPaymentInfo paymentInfo { get; set; }
        public OrderPaymentInfo shippingInfo { get; set; }
        public OrderDiscountInfo discountInfo { get; set; }
        public string refund { get; set; }
    }

    public class OrderCustomer
    {
        public int id { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string fullName()
        {
            return this.firstName + " " + lastName;
        }
        public string name { get; set; }
        public string socialId { get; set; }
        public string address1 { get; set; }
        public string zipCode { get; set; }
        public string city { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string country { get; set; }
        public bool isShipping { get; set; }
        public DateTime created { get; set; }
        public string username { get; set; }
        public int orderCount { get; set; }
        public List<OrderData> orders = new List<OrderData>();
    }
    public class OrderRawCustomer
    {
        public int orderCount { get; set; }
        public string customerEmail { get; set; }
        public string customerFirstName { get; set; }
        public string customerLastName { get; set; }
        public DateTime firstOrderDate { get; set; }
        public string orderInfo { get; set; }
    }
    public class OrderOrderLineProduct
    {
        public int productId { get; set; }
        public int quantity { get; set; }
        public string title { get; set; }
        public string sku { get; set; }
        public string price { get; set; }
        public List<OrderOrderLineVariant> variants = new List<OrderOrderLineVariant>();
    }

    public class OrderOrderLineVariant
    {
        public int variantId { get; set; }
        public string title { get; set; }
        public string sku { get; set; }
        public string price { get; set; }
    }

    public class OrderPaymentInfo
    {
        public string title { get; set; }
        public int id { get; set; }
    }

    public class OrderDiscountInfo
    {
        public string CouponCode { get; set; }
        public int id { get; set; }
    }

    public class OrderDataInfo
    {
        public List<OrderData> list = new List<OrderData>();
        public string grandTotal { get; set; }
        public int totalQty { get; set; }
        public int totalOrders { get; set; }
        public string averageQtyInBasket { get; set; }
        public string averageAmountInBasket { get; set; }
        public string totalShippingCost { get; set; }
    }
}
