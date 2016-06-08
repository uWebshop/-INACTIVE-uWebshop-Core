namespace uWebshop.Domain.Interfaces
{
	/// <summary>
	/// 
	/// </summary>
	public interface IBillingProviderMethod
	{
		/// <summary>
		/// Gets the unique identifier.
		/// </summary>
		/// <value>
		/// The unique identifier.
		/// </value>
		string Id { get; }
		/// <summary>
		/// Gets the sort order.
		/// </summary>
		/// <value>
		/// The sort order.
		/// </value>
		int SortOrder { get; }
		/// <summary>
		/// Gets the title.
		/// </summary>
		/// <value>
		/// The title.
		/// </value>
		string Title { get; }
		/// <summary>
		/// Gets the description.
		/// </summary>
		/// <value>
		/// The title.
		/// </value>
		string Description { get; }

		/// <summary>
		/// Disabled?.
		/// </summary>
		/// <value>
		/// The title.
		/// </value>
		bool Disabled { get; }
		/// <summary>
		/// Gets the name.
		/// </summary>
		/// <value>
		/// The title.
		/// </value>
		string Name { get; }
        /// <summary>
        /// Gets the amount.
        /// </summary>
        /// <value>
        /// The amount.
        /// </value>
        IVatPrice Amount { get; }

        /// <summary>
        /// Gets the percentage.
        /// </summary>
        /// <value>
        /// The percentage.
        /// </value>
        decimal Percentage { get; }

        /// <summary>
		/// Gets the image.
		/// </summary>
		/// <value>
		/// The image.
		/// </value>
		Image Image { get; }
	}
}