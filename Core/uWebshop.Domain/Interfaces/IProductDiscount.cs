namespace uWebshop.Domain.Interfaces
{
	/// <summary>
	/// 
	/// </summary>
	public interface IProductDiscount : IDiscount, IUwebshopSortableEntity
	{
		/// <summary>
		/// Gets or sets a value indicating whether [exclude variants].
		/// </summary>
		/// <value>
		///   <c>true</c> if [exclude variants]; otherwise, <c>false</c>.
		/// </value>
		bool ExcludeVariants { get; set; }

		/// <summary>
		/// Gets the discount value.
		/// </summary>
		/// <value>
		/// The discount value.
		/// </value>
		int DiscountValue { get; }

		int RangedDiscountValue(int orderTotalItemCount);
	}
}