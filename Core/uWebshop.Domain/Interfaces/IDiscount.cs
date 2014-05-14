using System.Security.Cryptography.X509Certificates;
using uWebshop.Common;

namespace uWebshop.Domain.Interfaces
{
	/// <summary>
	/// 
	/// </summary>
	public interface IDiscount
	{
		//public int Id { get { return _discount.Id; } }
		//public bool Disabled { get { return _discount.Disabled; } }
		//public int SortOrder { get { return _discount.SortOrder; } }
		//public int Counter { get { return _discount.Counter; } }

        string Title { get; }

        string Description { get; }

		/// <summary>
		/// Gets a value indicating whether the discount is once per customer.
		/// </summary>
		/// <value>
		///   <c>true</c> if the discount is once per customer; otherwise, <c>false</c>.
		/// </value>
		bool OncePerCustomer { get; }

		///// <summary>
		///// Gets the minimum order amount in cents.
		///// </summary>
		///// <value>
		///// The minimum order amount in cents.
		///// </value>
		//int MinimumOrderAmountInCents { get; }

		/// <summary>
		/// Gets a value indicating whether the counter is enabled.
		/// </summary>
		/// <value>
		///   <c>true</c> if the counter is enabled; otherwise, <c>false</c>.
		/// </value>
		bool CounterEnabled { get; }

		/// <summary>
		/// Gets the type.
		/// </summary>
		/// <value>
		/// The type.
		/// </value>
		DiscountType Type { get; }
	}
}