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
		public static string ConnectionString = ApplicationContext.Current.DatabaseContext.ConnectionString;
		public IEnumerable<ICoupon> GetAll()
		{
			var coupons = new List<Coupon>();

			var sqlHelper = DataLayerHelper.CreateSqlHelper(ConnectionString);

			using (var reader = sqlHelper.ExecuteReader("SELECT * FROM uWebshopCoupons " + null))
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
			var sqlHelper = DataLayerHelper.CreateSqlHelper(ConnectionString);
			var coupons = new List<Coupon>();

			using (var reader = sqlHelper.ExecuteReader("SELECT * FROM uWebshopCoupons WHERE DiscountId = @discountId", sqlHelper.CreateParameter("@discountId", discountId)))
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
			var sqlHelper = DataLayerHelper.CreateSqlHelper(ConnectionString);

			using (var reader = sqlHelper.ExecuteReader("SELECT * FROM uWebshopCoupons WHERE DiscountId = @discountId AND CouponCode = @couponCode", sqlHelper.CreateParameter("@discountId", discountId), sqlHelper.CreateParameter("@couponCode", couponCode)))
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
			var sqlHelper = DataLayerHelper.CreateSqlHelper(ConnectionString);
			var coupons = new List<Coupon>();

			using (var reader = sqlHelper.ExecuteReader("SELECT * FROM uWebshopCoupons WHERE CouponCode = @couponCode", sqlHelper.CreateParameter("@couponCode", couponCode)))
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
			var sqlHelper = DataLayerHelper.CreateSqlHelper(ConnectionString);

			sqlHelper.ExecuteNonQuery("update [uWebshopCoupons] set NumberAvailable = @NumberAvailable WHERE DiscountId = @DiscountId and CouponCode = @CouponCode", sqlHelper.CreateParameter("@DiscountId", coupon.DiscountId), sqlHelper.CreateParameter("@CouponCode", coupon.CouponCode), sqlHelper.CreateParameter("@NumberAvailable", coupon.NumberAvailable));
		}

		public void Save(int discountId, IEnumerable<ICoupon> coupons)
		{
			var sqlHelper = DataLayerHelper.CreateSqlHelper(ConnectionString);

			sqlHelper.ExecuteNonQuery("delete from [uWebshopCoupons] WHERE DiscountId = @discountId", sqlHelper.CreateParameter("@discountId", discountId));

			if (coupons.Any())
			{
				if (sqlHelper.ConnectionString.Contains("|DataDirectory|") || DataLayerHelper.IsEmbeddedDatabase(ConnectionString) || ConnectionString.ToLower().Contains("mysql"))
				{
					foreach (var coupon in coupons)
					{
						sqlHelper.ExecuteNonQuery(@"INSERT into uWebshopCoupons(DiscountId, CouponCode, NumberAvailable) values(@discountId, @couponcode, @numberavailable)", sqlHelper.CreateParameter("@discountId", coupon.DiscountId), sqlHelper.CreateParameter("@couponcode", coupon.CouponCode), sqlHelper.CreateParameter("@numberavailable", coupon.NumberAvailable));
					}
				}
				else
				{
					sqlHelper.ExecuteNonQuery("insert into [uWebshopCoupons] (DiscountId, CouponCode, NumberAvailable) VALUES " + string.Join(", ", coupons.Select(c => "(" + c.DiscountId + ", '" + c.CouponCode + "', " + c.NumberAvailable + ")").ToArray()));
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
			var sqlHelper = DataLayerHelper.CreateSqlHelper(ConnectionString);

			try
			{
				sqlHelper.ExecuteNonQuery(@"CREATE TABLE 
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