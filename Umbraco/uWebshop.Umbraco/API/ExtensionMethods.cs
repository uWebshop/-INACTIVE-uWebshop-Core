using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using uWebshop.Common.Interfaces;
using uWebshop.Common.Interfaces.Shared;
using uWebshop.Domain;
using uWebshop.Domain.BaseClasses;
using uWebshop.Domain.Interfaces;

namespace uWebshop.API
{
	public static class ExtensionMethods
	{
		public static string NodeTypeAlias(this IUwebshopEntity e)
		{
			if (e == null)
			{
				throw new ArgumentNullException("e");
			}
			if (!string.IsNullOrEmpty(e.TypeAlias))
			{
				return e.TypeAlias;
			}
			// should never come beyond this line
			var p = e as uWebshopEntity;
			if (p != null && p.NodeTypeAlias != null)
			{
				return p.NodeTypeAlias;
			}

			var entity = IO.Container.Resolve<ICMSEntityRepository>().GetByGlobalId(e.Id);

			if (entity != null && entity.NodeTypeAlias != null)
			{
				return entity.NodeTypeAlias;
			}
			return string.Empty;
		}

		internal static string NodeTypeAlias(this IProductInternalExternalShared product)
		{
			if (product == null)
			{
				return Product.NodeAlias;
			}
			if (!string.IsNullOrEmpty(product.TypeAlias))
			{
				return product.TypeAlias;
			}
			// should never come beyond this line
			var p = product as Product;
			if (p != null && p.NodeTypeAlias != null)
			{
				return p.NodeTypeAlias;
			}

			var entity = IO.Container.Resolve<ICMSEntityRepository>().GetByGlobalId(product.Id);

			if (entity != null && entity.NodeTypeAlias != null)
			{
				return entity.NodeTypeAlias;
			}
			return Product.NodeAlias;
		}

		internal static string Path(this IProduct product)
		{
			var p = product as Product;
			if (p != null)
			{
				return p.Path;
			}
			return IO.Container.Resolve<ICMSEntityRepository>().GetByGlobalId(product.Id).Path;
		}
	}
}
