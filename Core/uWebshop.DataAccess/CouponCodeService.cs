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

namespace uWebshop.DataAccess
{
    internal class CouponCodeService : ICouponCodeService
    {
        internal static UmbracoDatabase Database
        {
            get { return UmbracoContext.Current.Application.DatabaseContext.Database; }
        }

        // todo: use Sql instead of string
        public IEnumerable<ICoupon> GetAll(string where = null)
        {
            var coupons = Database.Query<uWebshopCoupon>("SELECT * FROM uWebshopCoupons " + where);

            return coupons.Select(coupon => new Coupon(coupon.DiscountId, coupon.CouponCode, coupon.NumberAvailable));
        }

        public IEnumerable<ICoupon> GetAllForDiscount(int discountId)
        {
            return GetAll("WHERE DiscountId = " + discountId);
        }

        public ICoupon Get(int discountId, string couponCode)
        {
            var sql = Sql.Builder.Select("*")
                .From("uWebshopCoupons")
                .Where("DiscountId = @0", discountId)
                .Where("CouponCode @0", couponCode);

            var coupon =
                Database.FirstOrDefault<uWebshopCoupon>(sql);

            return new Coupon(coupon.DiscountId, coupon.CouponCode, coupon.NumberAvailable);
        }

        public IEnumerable<ICoupon> GetAllWithCouponcode(string couponCode)
        {
            return GetAll("WHERE CouponCode = " + couponCode);
        }

        public IEnumerable<ICoupon> GetAllWithCouponcodes(IEnumerable<string> couponCodes)
        {
            var codes = couponCodes.ToList();
            return GetAll().Where(c => codes.Contains(c.CouponCode));
        }

        public void Save(ICoupon coupon)
        {
            var saveCoupon = new uWebshopCoupon
            {
                DiscountId = coupon.DiscountId,
                CouponCode = coupon.CouponCode,
                NumberAvailable = coupon.NumberAvailable
            };

            Database.Update(saveCoupon);
        }

        public void Save(int discountId, IEnumerable<ICoupon> coupons)
        {
            var couponDiscounts = GetAllForDiscount(discountId);

            foreach (var coupon in couponDiscounts)
            {
                Database.Delete(coupon);
            }

            if (coupons.Any())
            {
                foreach (var newCoupon in coupons.Select(coupon => new uWebshopCoupon
                {
                    DiscountId = coupon.DiscountId,
                    CouponCode = coupon.CouponCode,
                    NumberAvailable = coupon.NumberAvailable
                }))
                {
                    Database.Insert(newCoupon);
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