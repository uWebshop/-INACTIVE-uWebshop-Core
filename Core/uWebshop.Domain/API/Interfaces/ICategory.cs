using System;
using System.Collections.Generic;
using uWebshop.Common.Interfaces.Shared;
using uWebshop.Domain;
using uWebshop.Domain.Interfaces;

namespace uWebshop.API
{
	/// <summary>
	/// 
	/// </summary>
	public interface ICategory : IUwebshopSortableEntity, ICategoryInternalExternalShared
	{
		/// <summary>
		/// Gets the parent category.
		/// </summary>
		/// <value>
		/// The parent category.
		/// </value>
		ICategory ParentCategory { get; }

		/// <summary>
		/// Gets a list of products which belong to the category
		/// </summary>
		/// <value>
		/// The products.
		/// </value>
		IEnumerable<IProduct> Products { get; }

		/// <summary>
		/// Gets the title.
		/// </summary>
		/// <value>
		/// The title.
		/// </value>
		string Title { get; }

		/// <summary>
		/// Gets the meta description.
		/// </summary>
		/// <value>
		/// The meta description.
		/// </value>
		string MetaDescription { get; }

		/// <summary>
		/// Gets the tags.
		/// </summary>
		/// <value>
		/// The tags.
		/// </value>
		string[] Tags { get; }

		/// <summary>
		/// Gets the description.
		/// </summary>
		/// <value>
		/// The description.
		/// </value>
		string Description { get; }

		/// <summary>
		/// Gets the images.
		/// </summary>
		/// <value>
		/// The images.
		/// </value>
		IEnumerable<Image> Images { get; }

		/// <summary>
		/// Gets the sub categories.
		/// </summary>
		/// <value>
		/// The sub categories.
		/// </value>
		IEnumerable<ICategory> SubCategories { get; }

		/// <summary>
		/// Gets a value indicating whether [has categories].
		/// </summary>
		/// <value>
		///   <c>true</c> if [has categories]; otherwise, <c>false</c>.
		/// </value>
		bool HasCategories { get; }

		/// <summary>
		/// Gets a list of products which belong to the category
		/// </summary>
		/// <value>
		/// The products recursive.
		/// </value>
		IEnumerable<IProduct> ProductsRecursive { get; }

		// todo: for a future major revision (API break)
		///// <summary>
		///// Gets the parent categories.
		///// </summary>
		///// <value>
		///// The parent categories.
		///// </value>
		//IEnumerable<ICategory> ParentCategories { get; }

		/// <summary>
		/// Gets the URL.
		/// </summary>
		/// <value>
		/// The URL.
		/// </value>
		string Url { get; }

		/// <summary>
		/// Gets the property.
		/// </summary>
		/// <param name="propertyAlias">The property alias.</param>
		/// <returns></returns>
		string GetProperty(string propertyAlias);
		
		/// <summary>
		/// Gets the template of the product
		/// </summary>
		/// <value>
		/// The template.
		/// </value>
		int Template { get; }

		/// <summary>
		/// Gets the parent categories.
		/// </summary>
		IEnumerable<ICategory> GetParentCategories();

		/// <summary>
		/// The url for this product
		/// </summary>
		/// <returns></returns>
		string NiceUrl();

		/// <summary>
		/// The url for this product based on the current store
		/// </summary>
		/// <param name="storeAlias">The store alias.</param>
		/// <returns></returns>
		string NiceUrl(string storeAlias);

		/// <summary>
		/// The url for this product based on the current store
		/// </summary>
		/// <param name="getCanonicalUrl">if set to <c>true</c> get the canonical URL.</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <param name="currencyCode">The currency code.</param>
		/// <returns></returns>
		string NiceUrl(bool getCanonicalUrl, string storeAlias = null, string currencyCode = null);
	}
}