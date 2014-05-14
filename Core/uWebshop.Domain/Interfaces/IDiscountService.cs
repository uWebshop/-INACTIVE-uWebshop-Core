using System.Collections.Generic;

namespace uWebshop.Domain.Interfaces
{
	/// <summary>
	/// 
	/// </summary>
	public interface IDiscountService
	{
		/// <summary>
		/// Gets the order discount by unique identifier.
		/// </summary>
		/// <param name="id">The unique identifier.</param>
		/// <param name="localization">The localization.</param>
		/// <returns></returns>
		IOrderDiscount GetOrderDiscountById(int id, ILocalization localization);

		/// <summary>
		/// Gets the product discount by unique identifier.
		/// </summary>
		/// <param name="id">The unique identifier.</param>
		/// <param name="localization">The localization.</param>
		/// <returns></returns>
		DiscountProduct GetProductDiscountById(int id, ILocalization localization);

		/// <summary>
		/// Gets the discount by unique identifier.
		/// </summary>
		/// <param name="id">The unique identifier.</param>
		/// <param name="localization">The localization.</param>
		/// <returns></returns>
		IDiscount GetById(int id, ILocalization localization);
	}
}