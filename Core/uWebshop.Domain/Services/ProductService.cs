using System.Collections.Generic;
using System.Linq;
using uWebshop.Common.Interfaces;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Domain.Services
{
	internal class ProductService : EntityService<Product>, IProductService
	{
		private readonly IStockService _stockService;
		private readonly IStoreService _storeService;

		public ProductService(IProductRepository productRepository, IStoreService storeService, IStockService stockService) : base(productRepository)
		{
			_storeService = storeService;
			_stockService = stockService;
		}

		public int GetStockForProduct(int productId)
		{
			return _stockService.GetStockForUwebshopEntityWithId(productId);
		}

		public ProductInfo CreateProductInfoByProductId(int productId, OrderInfo order, ILocalization localization, int itemCount)
		{
			return new ProductInfo(GetById(productId, localization), order, itemCount);
		}

		public List<Product> GetAllEnabledAndWithCategory(ILocalization localization)
		{
			return GetAll(localization, true).Where(x => !x.Disabled && x.Categories.Any()).ToList();
		}

		public void ReloadWithVATSetting()
		{
			// todo: move functionality to repository
			var includingVAT = IO.Container.Resolve<ISettingsService>().IncludingVat;
			foreach (var localization in _storeService.GetAllLocalizations())
			{
				GetAll(localization, true).ToList().ForEach(product => product.PricesIncludingVat = includingVAT);
			}
		}

		public IProduct Localize(IProduct product, ILocalization localization)
		{
			if (product == null) return null;
			return GetById(product.Id, localization);
		}
	}
}