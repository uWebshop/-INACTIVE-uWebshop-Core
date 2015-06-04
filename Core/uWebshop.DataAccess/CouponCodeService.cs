using System;
using System.Collections.Generic;
using System.Linq;
using umbraco;
using umbraco.BusinessLogic;
using umbraco.DataLayer;
using uWebshop.Common.Interfaces;
using uWebshop.Domain.Interfaces;
using uWebshop.Domain.Model;
using Umbraco.Core;
using Umbraco.Core.Logging;

namespace uWebshop.DataAccess
{
	internal class CouponCodeService : ICouponCodeService
	{
		public IEnumerable<ICoupon> GetAll()
		{
			var coupons = new List<Coupon>();
			using (var reader = uWebshopOrders.SQLHelper.ExecuteReader("SELECT * FROM uWebshopCoupons " + null))
			{
				while (reader.Read())
				{
					coupons.Add(new Coupon(reader));
				}
			}
			return coupons;
		}

		public IEnumerable<ICoupon> GetAllForDiscount(int discountId)
		{
			var coupons = new List<Coupon>();
			using (var reader = uWebshopOrders.SQLHelper.ExecuteReader("SELECT * FROM uWebshopCoupons WHERE DiscountId = @discountId", uWebshopOrders.SQLHelper.CreateParameter("@discountId", discountId)))
			{
				while (reader.Read())
				{
					coupons.Add(new Coupon(reader));
				}
			}
			return coupons;
		}

		public ICoupon Get(int discountId, string couponCode)
		{
			using (var reader = uWebshopOrders.SQLHelper.ExecuteReader("SELECT * FROM uWebshopCoupons WHERE DiscountId = @discountId AND CouponCode = @couponCode", uWebshopOrders.SQLHelper.CreateParameter("@discountId", discountId), uWebshopOrders.SQLHelper.CreateParameter("@couponCode", couponCode)))
			{
				while (reader.Read())
				{
					var orderInfo = reader.GetString("orderInfo");
					if (!string.IsNullOrEmpty(orderInfo))
					{
						return new Coupon(reader);
					}
				}
				return null;
			}
		}
		
		public IEnumerable<ICoupon> GetAllWithCouponcode(string couponCode)
		{
			var coupons = new List<Coupon>();
			using (var reader = uWebshopOrders.SQLHelper.ExecuteReader("SELECT * FROM uWebshopCoupons WHERE CouponCode = @couponCode", uWebshopOrders.SQLHelper.CreateParameter("@couponCode", couponCode)))
			{
				while (reader.Read())
				{
					coupons.Add(new Coupon(reader));
				}
			}
			return coupons;
		}

		public IEnumerable<ICoupon> GetAllWithCouponcodes(IEnumerable<string> couponCodes)
		{
			var codes = couponCodes.ToList();
			return GetAll().Where(c => codes.Contains(c.CouponCode));
		}

		public void Save(ICoupon coupon)
		{
			uWebshopOrders.SQLHelper.ExecuteNonQuery("update [uWebshopCoupons] set NumberAvailable = @NumberAvailable WHERE DiscountId = @DiscountId and CouponCode = @CouponCode", uWebshopOrders.SQLHelper.CreateParameter("@DiscountId", coupon.DiscountId), uWebshopOrders.SQLHelper.CreateParameter("@CouponCode", coupon.CouponCode), uWebshopOrders.SQLHelper.CreateParameter("@NumberAvailable", coupon.NumberAvailable));
		}

		public void Save(int discountId, IEnumerable<ICoupon> coupons)
		{
			uWebshopOrders.SQLHelper.ExecuteNonQuery("delete from [uWebshopCoupons] WHERE DiscountId = @discountId", uWebshopOrders.SQLHelper.CreateParameter("@discountId", discountId));

			if (coupons.Any())
			{
				var connectionString = uWebshopOrders.ConnectionString;
				if (uWebshopOrders.SQLHelper.ConnectionString.Contains("|DataDirectory|") || DataLayerHelper.IsEmbeddedDatabase(connectionString) || connectionString.ToLower().Contains("mysql"))
				{
					foreach (var coupon in coupons)
					{
						uWebshopOrders.SQLHelper.ExecuteNonQuery(@"INSERT into uWebshopCoupons(DiscountId, CouponCode, NumberAvailable) values(@discountId, @couponcode, @numberavailable)", uWebshopOrders.SQLHelper.CreateParameter("@discountId", coupon.DiscountId), uWebshopOrders.SQLHelper.CreateParameter("@couponcode", coupon.CouponCode), uWebshopOrders.SQLHelper.CreateParameter("@numberavailable", coupon.NumberAvailable));
					}
				}
				else
				{
					uWebshopOrders.SQLHelper.ExecuteNonQuery("insert into [uWebshopCoupons] (DiscountId, CouponCode, NumberAvailable) VALUES " + string.Join(", ", coupons.Select(c => "(" + c.DiscountId + ", '" + c.CouponCode + "', " + c.NumberAvailable + ")").ToArray()));
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

		public void InstallCouponsTable()
		{
			try
			{
				uWebshopOrders.SQLHelper.ExecuteNonQuery(@"CREATE TABLE 
					[uWebshopCoupons](
					[DiscountId] [int] NOT NULL,
					[CouponCode] nvarchar (500) NOT NULL, 
					[NumberAvailable] [int] NOT NULL)");
			}
			catch (Exception ex)
			{
				LogHelper.Debug(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "InstallCouponsTable Catch: Already Exists?");
			}
		}
	}
}