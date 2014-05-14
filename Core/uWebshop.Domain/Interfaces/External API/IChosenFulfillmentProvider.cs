namespace uWebshop.Domain.Interfaces
{
	/// <summary>
	/// 
	/// </summary>
	public interface IChosenFulfillmentProvider
	{
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
		/// Gets a value indicating whether [fulfilled].
		/// </summary>
		/// <value>
		///   <c>true</c> if [fulfilled]; otherwise, <c>false</c>.
		/// </value>
		bool Fulfilled { get; }
		/// <summary>
		/// Gets the status.
		/// </summary>
		/// <value>
		/// The status.
		/// </value>
		FulfillmentStatus Status { get; }
		/// <summary>
		/// Gets the tracking token.
		/// </summary>
		/// <value>
		/// The tracking token.
		/// </value>
		string TrackingToken { get; }
	}
}