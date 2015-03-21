using System.Collections.Generic;

namespace uWebshop.Domain.Interfaces
{
	/// <summary>
	/// 
	/// </summary>
	public interface IProductVariantGroup : IUwebshopUmbracoEntity
	{
		/// <summary>
		/// Product Variants in this group
		/// </summary>
		/// <value>
		/// The product variants.
		/// </value>
		IEnumerable<IProductVariant> Variants { get; }

		/// <summary>
		/// Gets or sets the title.
		/// </summary>
		/// <value>
		/// The title.
		/// </value>
		string Title { get; }

		/// <summary>
		/// Gets or sets the description.
		/// </summary>
		/// <value>
		/// The title.
		/// </value>
		string Description { get; }

		/// <summary>
		/// Gets a value indicating whether the variant group is required.
		/// </summary>
		/// <value>
		/// <c>true</c> if the variant group is required; otherwise, <c>false</c>.
		/// </value>
		bool Required { get; }

		/// <summary>
		/// Gets the unique identifier.
		/// </summary>
		/// <value>
		/// The unique identifier.
		/// </value>
		int Id { get; }
	}
}