using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace uWebshop.DataAccess.Pocos
{
    [TableName("uWebshopCoupons")]
    [PrimaryKey("Id", autoIncrement = true)]
    public class uWebshopCoupon
    {
        public int Id { get; set; }

        [NullSetting(NullSetting = NullSettings.NotNull)]
        public int DiscountId { get; set; }

        [Length(500)]
        [NullSetting(NullSetting = NullSettings.NotNull)]
        public string CouponCode { get; set; }

        [NullSetting(NullSetting = NullSettings.NotNull)]
        public int NumberAvailable { get; set; }

        [NullSetting(NullSetting = NullSettings.Null)]
        public Guid uniqueID { get; set; }
    }

}
