using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;
using uWebshop.Domain;
using uWebshop.Domain.Interfaces;
using uWebshop.Newtonsoft.Json;

namespace uWebshop.API
{
	[DataContract(Namespace = "")]
	internal class VariantAdaptor : IProductVariant
	{
		private readonly IProductVariant _source;

		public VariantAdaptor(IProductVariant source)
		{
			_source = source;
		}

		[IgnoreDataMember]
		public IDiscountedRangedPrice Price { get { return _source.Price; } }

		public IDiscountedRangedPrice PriceIncludingProduct(IProduct product)
		{
			return _source.PriceIncludingProduct(product);
		}

		[JsonProperty(PropertyName = "Price")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public FlatDiscountedRangedPrice PriceFlat { get { return new FlatDiscountedRangedPrice(Price); } }

		[JsonProperty]
		public string SKU { get { return _source.SKU; } }
		[JsonProperty]
		public string Group { get { return _source.SKU; } }
		[JsonProperty]
		public bool Required { get { return _source.Required; } }
		[JsonProperty]
		public double Length { get { return _source.Length; } }
		[JsonProperty]
		public double Width { get { return _source.Width; } }
		[JsonProperty]
		public double Height { get { return _source.Height; } }
		[JsonProperty]
		public double Weight { get { return _source.Weight; } }
		[JsonProperty]
		public IEnumerable<Range> Ranges { get { return _source.Ranges; } }
		[JsonProperty]
		public decimal Vat { get { return _source.Id; } }
		[JsonProperty]
		public int Stock { get { return _source.Stock; } }
        public string Color()
        {
            return _source.Color();
        }
        [JsonProperty]
		public int OrderCount { get { return _source.OrderCount; } }
		[JsonProperty]
		public bool StockStatus { get { return _source.StockStatus; } }
		[JsonProperty]
		public bool BackorderStatus { get { return _source.BackorderStatus; } }
		public string GetProperty(string propertyAlias)
		{
			return _source.GetProperty(propertyAlias);
		}
        public T GetPropertyValue<T>(string propertyAlias)
        {
            return _source.GetPropertyValue<T>(propertyAlias);
        }

        [JsonProperty]
		public bool Orderable { get { return _source.Orderable; } }
		[IgnoreDataMember]
		public IProductDiscount Discount { get { return _source.Discount; } }
		[JsonProperty]
		public bool IsDiscounted { get { return _source.IsDiscounted; } }
		[JsonProperty]
		public string Title { get { return _source.Title; } }
		[JsonProperty]
		public string Description { get { return _source.Description; } }

		[JsonProperty]
		public int Id { get { return _source.Id; } }
        [JsonProperty]
        public Guid Key { get { return _source.Key; } }
        [JsonProperty]
		public string TypeAlias { get { return _source.TypeAlias; } }

		[IgnoreDataMember]
		public bool Disabled { get { return _source.Disabled; } }
		[IgnoreDataMember]
		public DateTime CreateDate { get { return _source.CreateDate; } }
		 [JsonProperty]
		public DateTime UpdateDate { get { return _source.UpdateDate; } }
		 [JsonProperty]
		 public int SortOrder { get { return _source.SortOrder; } }
	}
}