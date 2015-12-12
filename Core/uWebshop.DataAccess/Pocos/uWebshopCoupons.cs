using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace uWebshop.DataAccess.Pocos
{
    [TableName("uWebshopCoupons")]
	[ExplicitColumns]
    public class uWebshopCoupon
    {
		[Column("id")]
		[PrimaryKeyColumn(AutoIncrement = true)]
		[NullSetting(NullSetting = NullSettings.NotNull)]
		public int Id { get; set; }

		[Column("DiscountId")]
        [NullSetting(NullSetting = NullSettings.NotNull)]
        public int DiscountId { get; set; }

        [Column("CouponCode")]
        [Length(500)]
        [NullSetting(NullSetting = NullSettings.NotNull)]
        public string CouponCode { get; set; }

        [Column("NumberAvailable")]
        [NullSetting(NullSetting = NullSettings.NotNull)]
        public int NumberAvailable { get; set; }

    }

}
