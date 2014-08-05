using System;
using System.Collections.Generic;
using uWebshop.API;
using uWebshop.Domain;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;
using IProduct = uWebshop.Common.Interfaces.IProduct;

namespace uWebshop.Test.Repositories
{
	internal class TestProductService : IProductService
	{
		// todo: move this functionality to a fake repository
		public const int ProductId1 = 1234;
		private static Lazy<Product> _productFactory = new Lazy<Product>(() => new Product { Id = ProductId1, OriginalPriceInCents = 995, PriceInCents = 995, Disabled = false, StockStatus = true, Categories = new ICategory[0], Localization = StoreHelper.CurrentLocalization });
		public static Product Product1
		{
			get { return _productFactory.Value; }
		}

		public Product GetById(int id, ILocalization localization, bool includeDisabled = false)
		{
			if (id == ProductId1) return Product1;
			return null;
		}

		public int GetStockForProduct(int productId)
		{
			return 1; // hmm
		}

		public ProductInfo CreateProductInfoByProductId(int productId, OrderInfo order, ILocalization localization, int itemCount)
		{
			if (productId == ProductId1) return DefaultFactoriesAndSharedFunctionality.CreateProductInfo(995, 1);
			return null;
		}

		public List<Product> GetAllEnabledAndWithCategory(ILocalization localization)
		{
			throw new System.NotImplementedException();
		}

		public void ReloadWithVATSetting()
		{
		}

		public IProduct Localize(IProduct product, ILocalization localization)
		{
			return GetById(product.Id, localization);
		}

		public void FullResetCache()
		{
		}

		public IEnumerable<Product> GetAll(ILocalization localization, bool includeDisabled = false)
		{
			throw new System.NotImplementedException();
		}

		public void ReloadEntityWithId(int id)
		{
		}

		public void UnloadEntityWithId(int id)
		{
		}
	}
}