using Umbraco.Core.Persistence;

namespace uWebshop.DataAccess.Pocos
{

    [TableName("uWebshopCoupons")]
    [ExplicitColumns]
    public class uWebshopCoupon
    {
        [Column("DiscountId")]
        public int DiscountId { get; set; }

        [Column("CouponCode")]
        public string CouponCode { get; set; }

        [Column("NumberAvailable")]
        public int NumberAvailable { get; set; }

    }

}
