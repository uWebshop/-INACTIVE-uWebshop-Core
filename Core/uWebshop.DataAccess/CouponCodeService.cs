using System;
using System.Collections.Generic;
using System.Linq;
using umbraco;
using uWebshop.Common.Interfaces;
using uWebshop.DataAccess.Pocos;
using uWebshop.Domain.Interfaces;
using uWebshop.Domain.Model;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Persistence;
using Umbraco.Web;
using System.Web;
using System.Reflection;
using log4net;

namespace uWebshop.DataAccess
{
    public class CouponCodeService : ICouponCodeService
    {

        private static readonly ILog Log =
                LogManager.GetLogger(
                    MethodBase.GetCurrentMethod().DeclaringType
                );

        // todo: use Sql instead of string
        public IEnumerable<ICoupon> GetAll(string where = null)
        {

            string AllCouponCacheKey = "AllCouponCacheKey";

            List<Coupon> coupons;

            if (HttpContext.Current.Items.Contains(AllCouponCacheKey))
            {
                coupons = ((List<Coupon>)HttpContext.Current.Items[AllCouponCacheKey]);
            }
            else
            {
                using (var db = ApplicationContext.Current.DatabaseContext.Database)
                {
                    var couponsList = db.Fetch<uWebshopCoupon>("SELECT * FROM uWebshopCoupons");

                    coupons = couponsList.Select(coupon => new Coupon(coupon.Id, coupon.DiscountId, coupon.CouponCode, coupon.NumberAvailable)).ToList();

                    HttpContext.Current.Items[AllCouponCacheKey] = coupons;
                }


            }
            
            return coupons;
        }

        public IEnumerable<ICoupon> GetAllForDiscount(int discountId)
        {
            return GetAll().Where(x => x.DiscountId == discountId);
        }

        public ICoupon Get(int discountId, string couponCode)
        {
            var sql = Sql.Builder.Select("*")
                .From("uWebshopCoupons")
                .Where("DiscountId = @0", discountId)
                .Where("CouponCode = @0", couponCode);

            using (var db = ApplicationContext.Current.DatabaseContext.Database)
            {

                var coupon = db.FirstOrDefault<uWebshopCoupon>(sql);

                return new Coupon(coupon.Id, coupon.DiscountId, coupon.CouponCode, coupon.NumberAvailable);
            }

        }

        public IEnumerable<ICoupon> GetAllWithCouponcode(string couponCode)
        {
			var sql = Sql.Builder.Where("CouponCode = @0", couponCode);

            using (var db = ApplicationContext.Current.DatabaseContext.Database)
            {
                var coupons = db.Fetch<uWebshopCoupon>(sql);

                return coupons.Select(coupon => new Coupon(coupon.Id, coupon.DiscountId, coupon.CouponCode, coupon.NumberAvailable));
            }

		}

        public IEnumerable<ICoupon> GetAllWithCouponcodes(IEnumerable<string> couponCodes)
        {
            var codes = couponCodes.ToList();
            return GetAll().Where(c => codes.Contains(c.CouponCode));
        }

        public void Save(ICoupon coupon)
        {

            using (var db = ApplicationContext.Current.DatabaseContext.Database)
            {
                var saveCoupon = new uWebshopCoupon
                {
                    Id = coupon.Id,
                    DiscountId = coupon.DiscountId,
                    CouponCode = coupon.CouponCode,
                    NumberAvailable = coupon.NumberAvailable,
                    uniqueID = coupon.uniqueID
                };

                db.Update(saveCoupon);
            }


        }

        public void Save(int discountId, IEnumerable<ICoupon> coupons)
        {
            var couponDiscounts = GetAllForDiscount(discountId).Select(coupon => new uWebshopCoupon
            {
                Id = coupon.Id,
                DiscountId = coupon.DiscountId,
                CouponCode = coupon.CouponCode,
                NumberAvailable = coupon.NumberAvailable,
                uniqueID = coupon.uniqueID
            });

            using (var db = ApplicationContext.Current.DatabaseContext.Database)
            {
                foreach (var coupon in couponDiscounts)
                {
                    db.Execute("DELETE FROM uWebshopCoupons WHERE Id = @0", coupon.Id);
                }

                if (coupons.Any())
                {

                    var newCoupon = coupons.Select(coupon => new uWebshopCoupon
                    {
                        DiscountId = coupon.DiscountId,
                        CouponCode = coupon.CouponCode,
                        NumberAvailable = coupon.NumberAvailable,
                        uniqueID = coupon.uniqueID
                    });

                    foreach (var c in newCoupon)
                    {
                        db.Insert("uWebshopCoupons", "Id", c);
                    }
                }
            }

        }

        public void DecreaseCountByOneFor(IEnumerable<ICoupon> coupons)
        {

            foreach (var coupon in coupons.Where(c => c != null).Cast<Coupon>()) // in theory the cast might be wrong
            {
                coupon.NumberAvailable--;
                
                Save(coupon);
            }
        }
    }
}