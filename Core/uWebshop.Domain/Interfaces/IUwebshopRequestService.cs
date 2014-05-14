namespace uWebshop.Domain.Interfaces
{
	/// <summary>
	/// 
	/// </summary>
	internal interface IUwebshopRequestService
	{
		/// <summary>
		/// Gets the current request.
		/// </summary>
		/// <value>
		/// The current.
		/// </value>
		UwebshopRequest Current { get; }
	}
}