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
        [NullSetting(NullSetting = NullSettings.NotNull)]
        public int Id { get; set; }

        [Column("uniqueID")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public Guid UniqueId { get; set; }

        [Column("customerEmail")]
        [Length(500)]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string CustomerEmail { get; set; }

        [Column("customerFirstName")]
        [Length(500)]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string CustomerFirstName { get; set; }

        [Column("customerLastName")]
        [Length(500)]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string CustomerLastName { get; set; }

        [Column("orderNumber")]
        [Length(100)]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string OrderNumber { get; set; }

        [Column("storeOrderReferenceID")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public int StoreOrderReferenceId { get; set; }

        /// <summary>
        /// this value is later altered to use NVARCHAR(MAX)
        /// db.Execute("ALTER TABLE uWebshopOrderData ALTER COLUMN orderInfo NVARCHAR(MAX)");
        /// </summary>
        [Column("orderInfo")]
        [SpecialDbType(SpecialDbTypes.NTEXT)]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string OrderInfo { get; set; }

        [Column("orderStatus")]
        [Length(100)]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string OrderStatus { get; set; }

        [Column("transactionID")]
        [Length(100)]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string TransactionId { get; set; }

        [Column("storeAlias")]
        [Length(500)]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string StoreAlias { get; set; }

        [Column("customerID")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public int CustomerId { get; set; }

        [Column("customerUsername")]
        [Length(500)]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string CustomerUsername { get; set; }

        [Column("createDate")]
        public DateTime CreateDate { get; set; }

        [Column("updateDate")]
        public DateTime? UpdateDate { get; set; }

        [Column("deliveryDate")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public DateTime? DeliveryDate { get; set; }

        [Column("seriesID")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public int SeriesId { get; set; }

        [Column("SeriesCronInterval")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string SeriesCronInterval { get; set; }
        
        [Column("SeriesStart")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public DateTime? SeriesStart { get; set; }

        [Column("SeriesEnd")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public DateTime? SeriesEnd { get; set; }

        [Column("SeriesEndAfterInstances")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public int SeriesEndAfterInstances { get; set; }
    }

    

}
