using System;
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
	[DataContract(Namespace = "")]
	internal class ProductAdaptor : IProduct
	{
		private readonly IProduct _source;

		public ProductAdaptor(IProduct source)
		{
			_source = source;
		}

		[IgnoreDataMember]
		public IDiscountedRangedPrice Price { get { return _source.Price; } }
		[JsonProperty(PropertyName = "Price")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public FlatDiscountedRangedPrice PriceFlat { get { return new FlatDiscountedRangedPrice(Price); } }

		[JsonProperty]
		public bool HasCategories { get { return _source.HasCategories; } }
		[JsonProperty]
		public IEnumerable<ICategory> Categories { get { return _source.Categories.Select(CategoryAdaptor.Create); } }
		public bool PricesIncludingVat { get { return _source.PricesIncludingVat; } }
		[JsonProperty]
		public string SKU { get { return _source.SKU; } }
		[JsonProperty]
		public string[] Tags { get { return _source.Tags; } }
		[JsonProperty]
		public IEnumerable<Image> Images { get { return _source.Images; } }
		[JsonProperty]
		public IEnumerable<File> Files { get { return _source.Files; } }
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
		[JsonProperty]
		public int TotalItemsOrdered { get { return _source.TotalItemsOrdered; } }
		[JsonProperty]
		public int OrderCount { get { return _source.OrderCount; } }
		[JsonProperty]
		public bool StockStatus { get { return _source.StockStatus; } }
		[JsonProperty]
		public bool BackorderStatus { get { return _source.BackorderStatus; } }
		[JsonProperty]
		public bool UseVariantStock { get { return _source.UseVariantStock; } }
		public IEnumerable<IProductVariant> GetAllVariants()
		{
			return _source.GetAllVariants();
		}

		[IgnoreDataMember]
		public IProductDiscount Discount { get { return _source.Discount; } }
		[JsonProperty]
		public IEnumerable<IProductVariantGroup> VariantGroups { get { return _source.VariantGroups.Select(g => new VariantGroupAdaptor(g)); } }
		[JsonProperty]
		public bool Orderable { get { return _source.Orderable; } }
		[JsonProperty]
		public bool IsDiscounted { get { return _source.IsDiscounted; } }
		[JsonProperty]
		public string Url { get { return _source.Url; } }
		[JsonProperty]
		public string Title { get { return _source.Title; } }
		[JsonProperty]
		public string MetaDescription { get { return _source.MetaDescription; } }
		[JsonProperty]
		public string Description { get { return _source.Description; } }
		[JsonProperty]
		public int Template { get { return _source.Template; } }
		[JsonProperty]
		public string UrlName { get { return _source.UrlName; } }
		public string NiceUrl()
		{
			return _source.NiceUrl();
		}

		public string NiceUrl(string storeAlias)
		{
			return _source.NiceUrl(storeAlias);
		}

		public string NiceUrl(bool getCanonicalUrl, string storeAlias = null, string currencyCode = null)
		{
			return _source.NiceUrl(getCanonicalUrl, storeAlias, currencyCode);
		}

		public string GetProperty(string propertyAlias)
		{
			return _source.GetProperty(propertyAlias);
		}
        public T GetPropertyValue<T>(string propertyAlias)
        {
            return _source.GetPropertyValue<T>(propertyAlias);
        }

        public int Id { get { return _source.Id; } }
        public Guid Key { get { return _source.Key; } }
        public string TypeAlias { get { return _source.TypeAlias; } }

		[IgnoreDataMember]
		public bool Disabled { get { return _source.Disabled; } }
		[IgnoreDataMember]
		public DateTime CreateDate { get { return _source.CreateDate; } }
		[IgnoreDataMember]
		public DateTime UpdateDate { get { return _source.UpdateDate; } }
		[JsonProperty]
		public int SortOrder { get { return _source.SortOrder; } }
	}
}