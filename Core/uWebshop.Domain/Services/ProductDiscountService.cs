using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Domain.Services
{
	internal class ProductDiscountService : MultiStoreEntityService<DiscountProduct>, IProductDiscountService
	{
		public ProductDiscountService(IProductDiscountRepository repository, IStoreService storeService) : base(repository, storeService)
		{
		}

		private IEnumerable<DiscountProduct> GetAllForProductAndCurrentUser(int productId, ILocalization localization)
		{
			return GetAll(localization).Where(discount => discount.IsActive && discount.Products.Any(x => x.Id == productId) && (!discount.MemberGroups.Any() || UwebshopRequest.Current.User != null && discount.MemberGroups.Intersect(Roles.GetRolesForUser(UwebshopRequest.Current.User.UserName)).Any()));
		}
		
		public DiscountProduct GetDiscountByProductId(int productId, ILocalization localization, OrderInfo order = null)
		{
			return GetAllForProductAndCurrentUser(productId, localization)
				// todo: below can give some unexpected behaviour since GetDiscountAmountInCents doens't take ranges into account
									   .OrderByDescending(discount => discount.GetDiscountAmountInCents(productId, order)).FirstOrDefault();
		}

		public int GetAdjustedPriceForProductWithId(int productId, ILocalization localization, int currentProductPrice, int orderTotalItemCount = 0)
		{
			var discountProducts = GetAllForProductAndCurrentUser(productId, localization).ToList();
			if (!discountProducts.Any()) return currentProductPrice;
			return discountProducts.Select(discount => discount.GetAdjustedPrice(currentProductPrice, orderTotalItemCount)).Min();
		}

		protected override void AfterEntitiesLoadedFromRepository(List<DiscountProduct> entities, string storeAlias)
		{
		}
	}
}