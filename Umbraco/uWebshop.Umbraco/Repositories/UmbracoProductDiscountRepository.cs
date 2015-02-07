using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using uWebshop.API;
using uWebshop.Domain;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Umbraco.Repositories
{
	internal class UmbracoProductDiscountRepository : UmbracoMultiStoreEntityRepository<DiscountProduct, DiscountProduct>, IProductDiscountRepository
	{
		private readonly IDiscountProductAliassesService _aliasses;
		private readonly IStoreService _storeService;

		public UmbracoProductDiscountRepository(IDiscountProductAliassesService aliasses, IStoreService storeService)
		{
			_aliasses = aliasses;
			_storeService = storeService;
		}

		public override void LoadDataFromPropertiesDictionary(DiscountProduct discount, IPropertyProvider fields, ILocalization localization)
		{
			UmbracoOrderDiscountRepository.LoadBaseProperties(discount, fields, localization, _storeService);

			discount.Items = StoreHelper.ReadMultiStoreItemFromPropertiesDictionary(_aliasses.products, localization, fields).Split(',').Select(id => Common.Helpers.ParseInt(id)).Distinct().Where(id => id > 0).ToList();

			var categories = discount.Items.Select(id => DomainHelper.GetCategoryById(id)).Where(x => x != null).ToList();
			var products = discount.Items.Select(id => DomainHelper.GetProductById(id)).Where(x => x != null).ToList();

			var discountProducts = new List<IProduct>();

			foreach (var category in categories)
			{
				foreach (var catProduct in category.Products)
				{
					if (discountProducts.All(x => x.Id != catProduct.Id))
					{
						discountProducts.Add(catProduct);
					}
				}
			}

			foreach (var product in products.Where(product => discountProducts.All(x => product != null && x.Id != product.Id)))
			{
				discountProducts.Add(product);
			}
			
			discount.Products = discountProducts;

			discount.ProductVariants = discount.Items.Select(id => DomainHelper.GetProductVariantById(id)).Where(variant => variant != null);

			var excludeVariants = StoreHelper.ReadMultiStoreItemFromPropertiesDictionary(_aliasses.excludeVariants, localization, fields);
			discount.ExcludeVariants = excludeVariants == "enable" || excludeVariants == "1" || excludeVariants == "true";
		}

		public override string TypeAlias
		{
			get { return DiscountProduct.NodeAlias; }
		}
	}
}