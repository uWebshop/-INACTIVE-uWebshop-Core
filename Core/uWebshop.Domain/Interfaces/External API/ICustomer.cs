namespace uWebshop.Domain.Interfaces
{
	/// <summary>
	/// 
	/// </summary>
	public interface ICustomer : IAddress
	{
		/// <summary>
		/// Gets shipping address information for this order.
		/// </summary>
		/// <value>
		/// The shipping.
		/// </value>
		IAddress Shipping { get; }
		/// <summary>
		/// Gets a value indicating whether [accepts marketing].
		/// </summary>
		/// <value>
		///   <c>true</c> if [accepts marketing]; otherwise, <c>false</c>.
		/// </value>
		bool AcceptsMarketing { get; }
		/// <summary>
		/// Gets the name of the user.
		/// </summary>
		/// <value>
		/// The name of the user.
		/// </value>
		string UserName { get; }
		/// <summary>
		/// Gets the user unique identifier.
		/// </summary>
		/// <value>
		/// The user unique identifier.
		/// </value>
		string UserId { get; }
		
		/// <summary>
		/// Gets the total spending. Note that this can be an expensive call if many orders exist!
		/// </summary>
		/// <value>
		/// The total spending.
		/// </value>
		int TotalSpendingInCents { get; }

		/// <summary>
		/// Check if the customers shipping address == billing address
		/// </summary>
		/// <value>
		/// The total spending.
		/// </value>
		bool CustomerIsShipping { get; }
	}
}