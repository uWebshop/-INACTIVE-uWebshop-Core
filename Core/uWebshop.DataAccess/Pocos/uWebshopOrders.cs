using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace uWebshop.DataAccess.Pocos
{
    [TableName("uWebshopOrders")]
    [PrimaryKey("Id", autoIncrement = true)]
    [ExplicitColumns]
    public class uWebshopOrderData
    {
        [Column("id")]
        [PrimaryKeyColumn(AutoIncrement = true)]
        public int Id { get; set; }

        [Column("uniqueID")]
        public Guid UniqueId { get; set; }

        [Column("customerEmail")]
        public string CustomerEmail { get; set; }

        [Column("customerFirstName")]
        public string CustomerFirstName { get; set; }

        [Column("customerLastName")]
        public string CustomerLastName { get; set; }

        [Column("orderNumber")]
        public string OrderNumber { get; set; }

        [Column("storeOrderReferenceID")]
        public int StoreOrderReferenceId { get; set; }

        [Column("orderInfo")]
        public string OrderInfo { get; set; }

        [Column("orderStatus")]
        public string OrderStatus { get; set; }

        [Column("transactionID")]
        public string TransactionId { get; set; }

        [Column("storeAlias")]
        public string StoreAlias { get; set; }

        [Column("customerID")]
        public int CustomerId { get; set; }

        [Column("customerUsername")]
        public string CustomerUsername { get; set; }

        [Column("createDate")]
        public DateTime CreateDate { get; set; }

        [Column("updateDate")]
        public DateTime? UpdateDate { get; set; }

        [Column("deliveryDate")]
        public DateTime? DeliveryDate { get; set; }

        [Column("seriesID")]
        public int SeriesId { get; set; }

        [Column("SeriesCronInterval")]
        public string SeriesCronInterval { get; set; }
        
        [Column("SeriesStart")]
        public DateTime? SeriesStart { get; set; }

        [Column("SeriesEnd")]
        public DateTime? SeriesEnd { get; set; }

        [Column("SeriesEndAfterInstances")]
        public int SeriesEndAfterInstances { get; set; }
    }

    

}
