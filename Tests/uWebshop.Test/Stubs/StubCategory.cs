using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using uWebshop.Common.Interfaces;
using uWebshop.Domain;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Test.Stubs
{
	internal class StubCategory : ICategory, API.ICategory
	{
		public int Id { get; set; }
		public string TypeAlias { get { return NodeTypeAlias; } set { NodeTypeAlias = value; } }
		public bool Disabled { get; set; }
		public string NodeTypeAlias { get; set; }
		public string Name { get; set; }
		public int ParentId { get; set; }
		public string Path { get; set; }
		public DateTime CreateDate { get; set; }
		public DateTime UpdateDate { get; set; }
		public int SortOrder { get; set; }
		public ICategory ParentCategory { get; set; }
		IEnumerable<API.IProduct> API.ICategory.Products { get { return Products.Cast<API.IProduct>(); } }
		public string UrlName { get; set; }
		public string ParentNodeTypeAlias { get; set; }
		public string Title { get; set; }
		public string MetaDescription { get; set; }
		public string[] Tags { get; set; }
		public string Description { get; set; }
		public IEnumerable<Image> Images { get; set; }
		IEnumerable<API.ICategory> API.ICategory.SubCategories { get { return SubCategories.Cast<API.ICategory>(); } }
		IEnumerable<ICategory> ICategory.ParentCategories { get { return ParentCategories; } }
		public IEnumerable<ICategory> SubCategories { get; set; }
		IEnumerable<API.IProduct> API.ICategory.ProductsRecursive { get { return ProductsRecursive.Cast<API.IProduct>(); } }
		public string Url { get; set; }
		public bool HasCategories { get; set; }
		public ICategory Parent { get; private set; }
		API.ICategory API.ICategory.ParentCategory { get { return Parent as API.ICategory; } }
		public IEnumerable<IProduct> Products { get; set; }
		public IEnumerable<IProduct> ProductsRecursive { get; set; }
		public string URL { get; set; }
		public int Template { get; set; }
		IEnumerable<API.ICategory> API.ICategory.GetParentCategories()
		{
			return GetParentCategories().Cast<API.ICategory>();
		}

		public string NiceUrl()
		{
			throw new NotImplementedException();
		}

		public string NiceUrl(string storeAlias)
		{
			throw new NotImplementedException();
		}

		public string NiceUrl(bool getCanonicalUrl, string storeAlias = null, string currencyCode = null)
		{
			throw new NotImplementedException();
		}

		public List<ICategory> ParentCategories { get; set; } 
		public IEnumerable<ICategory> GetParentCategories()
		{
			return (IEnumerable<ICategory>)ParentCategories ?? new[] {ParentCategory};
		}

		public string GetProperty(string propertyAlias)
		{
			throw new NotImplementedException();
		}
	}
}