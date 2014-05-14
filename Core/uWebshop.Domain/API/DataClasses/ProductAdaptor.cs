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

		public bool HasCategories { get { return _source.HasCategories; } }
		public IEnumerable<ICategory> Categories { get { return _source.Categories.Select(CategoryAdaptor.Create); } }
		public bool PricesIncludingVat { get { return _source.PricesIncludingVat; } }
		public string SKU { get { return _source.SKU; } }
		public string[] Tags { get { return _source.Tags; } }
		public IEnumerable<Image> Images { get { return _source.Images; } }
		public IEnumerable<File> Files { get { return _source.Files; } }
		public double Length { get { return _source.Length; } }
		public double Width { get { return _source.Width; } }
		public double Height { get { return _source.Height; } }
		public double Weight { get { return _source.Weight; } }
		public IEnumerable<Range> Ranges { get { return _source.Ranges; } }
		public decimal Vat { get { return _source.Id; } }
		public int Stock { get { return _source.Stock; } }
		public int TotalItemsOrdered { get { return _source.TotalItemsOrdered; } }
		public int OrderCount { get { return _source.OrderCount; } }
		public bool StockStatus { get { return _source.StockStatus; } }
		public bool BackorderStatus { get { return _source.BackorderStatus; } }
		public bool UseVariantStock { get { return _source.UseVariantStock; } }
		public IEnumerable<IProductVariant> GetAllVariants()
		{
			return _source.GetAllVariants();
		}

		[IgnoreDataMember]
		public IProductDiscount Discount { get { return _source.Discount; } }
		public IEnumerable<IProductVariantGroup> VariantGroups { get { return _source.VariantGroups.Select(g => new VariantGroupAdaptor(g)); } }
		public bool Orderable { get { return _source.Orderable; } }
		public bool IsDiscounted { get { return _source.IsDiscounted; } }
		public string Url { get { return _source.Url; } }
		public string Title { get { return _source.Title; } }
		public string MetaDescription { get { return _source.MetaDescription; } }
		public string Description { get { return _source.Description; } }
		public int Template { get { return _source.Template; } }
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