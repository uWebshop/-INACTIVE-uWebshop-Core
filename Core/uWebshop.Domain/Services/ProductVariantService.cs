using System.Collections.Generic;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Domain.Services
{
	internal class ProductVariantService : EntityService<ProductVariant>, IProductVariantService
	{
		public ProductVariantService(IProductVariantRepository productVariantRepository) : base(productVariantRepository)
		{
		}
	}

    internal class ProductVariantGroupService : EntityService<ProductVariantGroup>, IProductVariantGroupService
    {
        public ProductVariantGroupService(IProductVariantGroupRepository productVariantGroupRepository)
            : base(productVariantGroupRepository)
        {
        }
    }
}