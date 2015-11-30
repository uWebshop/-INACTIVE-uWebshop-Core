using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using uWebshop.API;
using uWebshop.Domain.BaseClasses;
using uWebshop.Domain.ContentTypes;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;
using uWebshop.Domain.Services;

namespace uWebshop.Domain
{
	/// <summary>
	///     Class representing a category in webshop, containing a group of products
	/// </summary>
	[DataContract(Namespace = "", IsReference = true)]
	[ContentType(ParentContentType = typeof(Catalog), Name = "Category", Description = "#CategoryDescription", Alias = "uwbsCategory", IconClass = IconClass.stacklist, Icon = ContentIcon.Folder, Thumbnail = ContentThumbnail.Folder, AllowedChildTypes = new[] { typeof(Category), typeof(Product) })]
	public class Category : MultiStoreUwebshopContent, ICategory, Common.Interfaces.ICategory
	{
		/// <summary>
		/// The node alias
		/// </summary>
		public static string NodeAlias;

		internal ILocalization Localization;

		internal Func<List<IProduct>> ProductsFactory;
		private List<Category> _categories;

		internal List<int> _categoryIds;
		private string _localizedUrl;
		private API.ICategory _parentCategory;

		private string _parentNodeTypeAlias;
		private List<IProduct> _products;
		private List<IProduct> _productsRecursively;

		/// <summary>
		/// Gets or sets the parent node type alias.
		/// </summary>
		/// <value>
		/// The parent node type alias.
		/// </value>
		public string ParentNodeTypeAlias
		{
			// todo: some issues with loops when trying to load this or ParentCategory in the service/repo
			get
			{
			    var getById = IO.Container.Resolve<ICMSEntityRepository>().GetByGlobalId(ParentId);
			    if (getById != null)
			    {
			        var parentNodeTypeAlias = IO.Container.Resolve<ICMSEntityRepository>().GetByGlobalId(ParentId).NodeTypeAlias;
			        if (parentNodeTypeAlias != null)
			            return _parentNodeTypeAlias ?? (_parentNodeTypeAlias = parentNodeTypeAlias);
			    }

			    return null;
			}
		    set { }
		}

		/// <summary>
		/// Gets or sets the parent category.
		/// </summary>
		/// <value>
		/// The parent category.
		/// </value>
		public API.ICategory ParentCategory
		{
			get { return _parentCategory ?? (_parentCategory = IO.Container.Resolve<ICategoryService>().GetById(ParentId, Localization)); }
			set { }
		}
		Common.Interfaces.ICategory Common.Interfaces.ICategory.Parent { get { return (Common.Interfaces.ICategory)ParentCategory; } }

		/// <summary>
		/// Gets or sets a value indicating whether [has categories].
		/// </summary>
		/// <value>
		///   <c>true</c> if [has categories]; otherwise, <c>false</c>.
		/// </value>
		[DataMember]
		public bool HasCategories { get; set; }

		public override bool Disabled { get; set; }

		/// <summary>
		///     Gets a list of products which belong to the category
		/// </summary>
		[DataMember]
		public IEnumerable<IProduct> Products
		{
			get { return (_products ?? (_products = ProductsFactory())); }
			set { }
		}

		IEnumerable<Common.Interfaces.ICategory> Common.Interfaces.ICategory.ParentCategories { get { return ParentCategories; } }
		IEnumerable<Common.Interfaces.ICategory> Common.Interfaces.ICategory.SubCategories { get { return SubCategories.Cast<Common.Interfaces.ICategory>(); } }
		IEnumerable<Common.Interfaces.IProduct> Common.Interfaces.ICategory.Products { get { return Products.Cast<Common.Interfaces.IProduct>(); } }
		
		/// <summary>
		///     Gets a list of products which belong to the category
		/// </summary>
		[DataMember]
		public IEnumerable<API.IProduct> ProductsRecursive
		{
			get { return (_productsRecursively ?? (_productsRecursively = StoreHelper.GetProductsRecursive(this))).Cast<API.IProduct>(); }
			set { }
		}

		/// <summary>
		/// Gets the parent categories.
		/// </summary>
		/// <value>
		/// The parent categories.
		/// </value>
		public IEnumerable<Category> ParentCategories
		{
			get
			{
				var list = IO.Container.Resolve<ICategoryService>().GetAll(Localization).Cast<Category>().Where(c => c.SubCategories.Contains(this)).ToList();
				list.Add(ParentCategory as Category);
				return list.Where(c => c != null);
			}
		}

		private IEnumerable<ICategory> ParentCategories1
		{
			get { return IO.Container.Resolve<ICategoryService>().GetAll(Localization).Where(c => (c)._categories != null && (c)._categories.Contains(this)); }
		}

		/// <summary>
		/// Gets a list of (sub)categories of the current category
		/// </summary>
		/// <value>
		/// The categories.n
		/// </value>
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[Obsolete("Use SubCategories")]
		public IEnumerable<Category> Categories
		{
			get { return SubCategories.Cast<Category>(); }
			set { }
		}

		/// <summary>
		/// Gets the sub categories.
		/// </summary>
		/// <value>
		/// The sub categories.
		/// </value>
		[DataMember]
		[ContentPropertyType(Alias = "categories", DataType = DataType.MultiContentPickerCategories, Tab = ContentTypeTab.Global, Name = "#SubCategories", Description = "#SubCategoriesDescription", Mandatory = false, SortOrder = 6)]
		public IEnumerable<API.ICategory> SubCategories
		{
			get
			{
				if (_categories == null)
				{
					var recursiveParentsIncludingSelf = GetParentCategoriesRecursiveToPreventLoops();
					recursiveParentsIncludingSelf.Add(this);
					// todo: remove loops
					_categories = IO.Container.Resolve<ICategoryService>().GetAll(Localization).Where(x => x.ParentId == Id).OrderBy(c => c.SortOrder).Cast<Category>().ToList();
					_categories.AddRange(_categoryIds.Select(id => IO.Container.Resolve<ICategoryService>().GetById(id, Localization)).Where(x => x != null));
					_categories = _categories.Where(c => !recursiveParentsIncludingSelf.Contains(c)).ToList();
				}
				return _categories;
			}
			set { }
		}

		/// <summary>
		/// Gets the tags of the category
		/// </summary>
		/// <value>
		/// The tags.
		/// </value>
		[DataMember]
		[ContentPropertyType(Alias = "metaTags", DataType = DataType.Tags, Tab = ContentTypeTab.Global, Name = "#Tags", Description = "#TagsDescription", Mandatory = false, SortOrder = 4)]
		public string[] Tags { get; set; }

		/// <summary>
		/// Gets the image of the category
		/// </summary>
		/// <value>
		/// The images.
		/// </value>
		[DataMember]
		[ContentPropertyType(Alias = "images", DataType = DataType.MultiContentPickerImages, Tab = ContentTypeTab.Details, Name = "#Images", Description = "#ImagesDescription", Mandatory = false, SortOrder = 7)]
		public IEnumerable<Image> Images
		{
			get { return _images ?? (_images = ImageIds.Select(ImageFactory).Where(i => i != null).ToArray()); }
			set { }
		}

		internal static Func<int, Image> ImageFactory; 
		private Image[] _images;
		internal int[] ImageIds;

		/// <summary>
		/// Gets or sets the localized URL.
		/// </summary>
		/// <value>
		/// The localized URL.
		/// </value>
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[Obsolete("use UrlName")]
		public string LocalizedUrl
		{
			get { return UrlName; }
			set { }
		}

		/// <summary>
		///     Returns the localized url (ID-URL)
		/// </summary>
		[DataMember(Name = "urlName")]
		public new string UrlName
		{
			get
			{
				if (_localizedUrl != null) return _localizedUrl;

				string urlFormat = DomainHelper.BuildUrlFromTemplate(UwebshopConfiguration.Current.CategoryUrl, this) ?? URL;

				_localizedUrl = IO.Container.Resolve<ICMSApplication>().ApplyUrlFormatRules(urlFormat);

				return _localizedUrl;
			}
			set { _localizedUrl = value; }
		}

		/// <summary>
		///     The url for this category based on the current store
		/// </summary>
		public string Url
		{
			get { return NiceUrl(); }
			set { }
		}

		public IEnumerable<ICategory> GetParentCategories()
		{
			return ParentCategories;
		}

		/// <summary>
		/// Determines whether the specified node type alias is alias.
		/// </summary>
		/// <param name="nodeTypeAlias">The node type alias.</param>
		/// <returns></returns>
		public static bool IsAlias(string nodeTypeAlias)
		{
			return nodeTypeAlias != null && !nodeTypeAlias.StartsWith(Catalog.CategoryRepositoryNodeAlias) && nodeTypeAlias.StartsWith(NodeAlias);
		}

		private List<ICategory> GetParentCategoriesRecursiveToPreventLoops()
		{
			var parents = ParentCategories1.ToList();
			var prevparents = new List<ICategory>();
			while (parents.Count > prevparents.Count)
			{
				prevparents = parents;
				parents = parents.Concat(parents.SelectMany(c => (c as Category).ParentCategories1)).Distinct().ToList();
			}
			return parents;
		}

		/// <summary>
		/// The url for this category bsaed on a storeAlias
		/// </summary>
		/// <param name="storeAlias">The store alias.</param>
		/// <param name="currencyCode">The currency code.</param>
		/// <returns></returns>
		public string NiceUrl(string storeAlias = null, string currencyCode = null)
		{
			return NiceUrl(false, storeAlias, currencyCode);
		}

		public string NiceUrl()
		{
			return NiceUrl(null, null);
		}

		public string NiceUrl(string storeAlias)
		{
			return NiceUrl(storeAlias, null);
		}

		public string NiceUrl(bool getCanonicalUrl, string storeAlias = null, string currencyCode = null)
		{
			var localization = storeAlias == null && currencyCode == null ? Localization : StoreHelper.GetLocalizationOrCurrent(storeAlias, currencyCode);
			var urlService = IO.Container.Resolve<IUrlService>();
			return getCanonicalUrl ? urlService.CategoryCanonicalUrl(this, localization) : urlService.CategoryUrlUsingCurrentPath(this, localization);
		}

		/// <summary>
		/// The full localized URL (ID-URL) of the category
		/// </summary>
		/// <param name="storeAlias">The store alias.</param>
		/// <returns></returns>
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Use NiceUrl(string storeAlias)")]
		public string FullLocalizedUrl(string storeAlias = null)
		{
			return NiceUrl(storeAlias);
		}

		internal void ClearCachedValues()
		{
// temporary hack
			_categories = null;
			_parentNodeTypeAlias = null;
			_products = null;
			_productsRecursively = null;
			_parentCategory = null;
		}
	}
}