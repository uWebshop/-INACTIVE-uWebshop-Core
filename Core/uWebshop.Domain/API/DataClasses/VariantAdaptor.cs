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

		public string SKU { get { return _source.SKU; } }
		public string Group { get { return _source.SKU; } }
		public bool Required { get { return _source.Required; } }
		public double Length { get { return _source.Length; } }
		public double Width { get { return _source.Width; } }
		public double Height { get { return _source.Height; } }
		public double Weight { get { return _source.Weight; } }
		public IEnumerable<Range> Ranges { get { return _source.Ranges; } }
		public decimal Vat { get { return _source.Id; } }
		public int Stock { get { return _source.Stock; } }
		public int OrderCount { get { return _source.OrderCount; } }
		public bool StockStatus { get { return _source.StockStatus; } }
		public bool BackorderStatus { get { return _source.BackorderStatus; } }
		public string GetProperty(string propertyAlias)
		{
			return _source.GetProperty(propertyAlias);
		}

		public bool Orderable { get { return _source.Orderable; } }
		[IgnoreDataMember]
		public IProductDiscount Discount { get { return _source.Discount; } }
		public bool IsDiscounted { get { return _source.IsDiscounted; } }
		public string Title { get { return _source.Title; } }
		public string Description { get { return _source.Description; } }

		public int Id { get { return _source.Id; } }
		public string TypeAlias { get { return _source.TypeAlias; } }

		[IgnoreDataMember]
		public bool Disabled { get { return _source.Disabled; } }
		[IgnoreDataMember]
		public DateTime CreateDate { get { return _source.CreateDate; } }
		[IgnoreDataMember]
		public DateTime UpdateDate { get { return _source.UpdateDate; } }
		public int SortOrder { get { return _source.SortOrder; } }
	}
}