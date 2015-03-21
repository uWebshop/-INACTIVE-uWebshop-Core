using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;
using uWebshop.Domain;
using uWebshop.Domain.Interfaces;
using uWebshop.Newtonsoft.Json;

namespace uWebshop.API
{
	[DataContract(Name = "OrderedProductVariant", Namespace = "")]
	internal class OrderedProductVariantInfoAdapter : IOrderedProductVariant
	{
		private readonly ProductVariantInfo _source;

		public OrderedProductVariantInfoAdapter(ProductVariantInfo source, ProductInfo product)
		{
			_source = source;
			_source.Product = product;
		}

		[DataMember]
		public string TypeAlias { get { return _source.TypeAlias; } set { } }
		[IgnoreDataMember]
		public IDiscountedRangedPrice Price { get { return _source.Price; } }
		[DataMember(Name = "Price")]
		[JsonProperty(PropertyName = "Price")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public FlatDiscountedRangedPrice PriceFlat { get { return new FlatDiscountedRangedPrice(Price); } set { } }
		[DataMember]
		public bool Required { get { return _source.Required; } set { } }
		[DataMember]
		public string SKU { get { return _source.SKU; } set { } }
		[DataMember]
		public double Length { get { return _source.Length; } set { } }
		[DataMember]
		public double Width { get { return _source.Width; } set { } }
		[DataMember]
		public double Height { get { return _source.Height; } set { } }
		[DataMember]
		public string Group { get { return _source.Group; } set { } }
		[DataMember]
		public double Weight { get { return _source.Weight; } set { } }
		[IgnoreDataMember]
		public IProductVariant Variant { get { return new VariantAdaptor(_source.Variant); } }
		[DataMember]
		public IEnumerable<Range> Ranges { get { return _source.Ranges; } set { } }
		[DataMember]
		public decimal Vat { get { return _source.Id; } set { } }
		[DataMember]
		public string Title { get { return _source.Title; } set { } }
		[DataMember]
		public int OriginalId { get { return _source.OriginalId; } set { } }
	}
}