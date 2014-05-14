namespace uWebshop.Domain.Interfaces
{
	/// <summary>
	/// 
	/// </summary>
	public interface IChosenPaymentProvider
	{
		/// <summary>
		/// Gets a value indicating whether [paid].
		/// </summary>
		/// <value>
		///   <c>true</c> if [paid]; otherwise, <c>false</c>.
		/// </value>
		bool Paid { get; }
		/// <summary>
		/// Gets the unique identifier.
		/// </summary>
		/// <value>
		/// The unique identifier.
		/// </value>
		int Id { get; }
		/// <summary>
		/// Gets the title.
		/// </summary>
		/// <value>
		/// The title.
		/// </value>
		string Title { get; }
		/// <summary>
		/// Gets the method unique identifier.
		/// </summary>
		/// <value>
		/// The method unique identifier.
		/// </value>
		string MethodId { get; }
		/// <summary>
		/// Gets the method title.
		/// </summary>
		/// <value>
		/// The method title.
		/// </value>
		string MethodTitle { get; }
		/// <summary>
		/// Gets the transaction unique identifier.
		/// </summary>
		/// <value>
		/// The transaction unique identifier.
		/// </value>
		string TransactionId { get; }
	}
}