using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using uWebshop.Domain;
using uWebshop.Domain.Interfaces;
using uWebshop.Newtonsoft.Json;

namespace uWebshop.API
{
	[KnownType(typeof(OrderedProductVariantInfoAdapter))]
	[DataContract(Name = "OrderedProduct", Namespace= "")]
	internal class OrderedProductInfoAdaptor : IOrderedProduct
	{
		private readonly ProductInfo _source;

		public OrderedProductInfoAdaptor(ProductInfo source)
		{
			_source = source;
		}

		[IgnoreDataMember]
		public IDiscountedRangedPrice Price { get { return _source.Price; } }
		[DataMember(Name = "Price")]
		[JsonProperty(PropertyName = "Price")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public FlatDiscountedRangedPrice PriceFlat { get { return new FlatDiscountedRangedPrice(Price); } set { } }

		[DataMember]
		public string SKU { get { return _source.SKU; } set { } }
		[DataMember]
		public double Length { get { return _source.Length; } set { } }
		[DataMember]
		public double Width { get { return _source.Width; } set { } }
		[DataMember]
		public double Height { get { return _source.Height; } set { } }
		[DataMember]
		public double Weight { get { return _source.Weight; } set { } }
		[IgnoreDataMember]
		public IProduct CatalogProduct {
			get
			{
				if (_source.CatalogProduct == null) return null;
				return new ProductAdaptor(_source.CatalogProduct);
			}
		}
		[DataMember]
		public IEnumerable<Range> Ranges { get { return _source.Ranges; } set { } }
		[DataMember]
		public decimal Vat { get { return _source.Vat; } set { } }
		[DataMember]
		public decimal DiscountPercentage { get { return _source.DiscountPercentage; } set { } }
		[DataMember]
		public bool DiscountExcludingVariants { get { return _source.DiscountExcludingVariants; } set { } }
		[DataMember]
		public int Quantity { get { return _source.ItemCount.GetValueOrDefault(1); } set { } }
		[DataMember]
		public bool IsDiscounted { get { return _source.IsDiscounted; } set { } }
		[DataMember]
		public int OriginalId { get { return _source.OriginalId; } set { } }
		[DataMember]
		public IEnumerable<IOrderedProductVariant> Variants { get { return _source.ProductVariants.Select(v => new OrderedProductVariantInfoAdapter(v, _source)); } set { } }
		[DataMember]
		public string TypeAlias { get { return _source.TypeAlias; } set { } }
		[DataMember]
		public string Title { get { return _source.Title; } set { } }
	}
}