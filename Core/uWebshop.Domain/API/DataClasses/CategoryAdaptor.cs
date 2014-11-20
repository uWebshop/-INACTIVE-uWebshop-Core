using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using uWebshop.Domain;
using uWebshop.Newtonsoft.Json;

namespace uWebshop.API
{
	[DataContract(Namespace = "")]
	internal class CategoryAdaptor : ICategory
	{
		private readonly ICategory _source;

		private CategoryAdaptor(ICategory source)
		{
			_source = source;
		}
		public static ICategory Create(ICategory source)
		{
			return source == null ? null : new CategoryAdaptor(source);
		}

        [JsonProperty]
        public string Title { get { return _source.Title; } }
        [JsonProperty]
        public string Description { get { return _source.Description; } }

        [JsonProperty]
        public int Id { get { return _source.Id; } }
		public string TypeAlias { get { return _source.TypeAlias; } }

		[IgnoreDataMember]
		public bool Disabled { get { return _source.Disabled; } }
		[IgnoreDataMember]
		public DateTime CreateDate { get { return _source.CreateDate; } }
        [JsonProperty]
		public DateTime UpdateDate { get { return _source.UpdateDate; } }
        [JsonProperty]
        public int SortOrder { get { return _source.SortOrder; } }

		[IgnoreDataMember]
		public ICategory ParentCategory { get { return Create(_source.ParentCategory); } }
        [JsonProperty]
        public string UrlName { get { return _source.UrlName; } }
		[IgnoreDataMember]
		public string ParentNodeTypeAlias { get { return _source.ParentNodeTypeAlias; } }
        [JsonProperty]
        public string MetaDescription { get { return _source.MetaDescription; } }
        [JsonProperty]
        public string[] Tags { get { return _source.Tags; } }
        [JsonProperty]
        public IEnumerable<Image> Images { get { return _source.Images; } }
		[IgnoreDataMember]
        [JsonProperty]
        public IEnumerable<ICategory> SubCategories { get { return _source.SubCategories.Select(Create); } }
        [JsonProperty]
        public bool HasCategories { get { return _source.HasCategories; } }
		[IgnoreDataMember]
		public IEnumerable<IProduct> Products { get { return _source.Products.Select(p => new ProductAdaptor(p)); } }
		[IgnoreDataMember]
		public IEnumerable<IProduct> ProductsRecursive { get { return _source.ProductsRecursive.Select(p => new ProductAdaptor(p)); } }
        [JsonProperty]
        public string Url { get { return _source.Url; } }
		public string GetProperty(string propertyAlias)
		{
			return _source.GetProperty(propertyAlias);
		}
		[IgnoreDataMember]
		public int Template { get { return _source.Template; } }

		public IEnumerable<ICategory> GetParentCategories()
		{
			return _source.GetParentCategories();
		}

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
	}
}