using System.Collections.Generic;
using uWebshop.Common;

namespace uWebshop.Domain.Interfaces
{
	/// <summary>
	/// 
	/// </summary>
	public interface IFulfillmentProvider
	{
		/// <summary>
		/// Gets the unique identifier.
		/// </summary>
		/// <value>
		/// The unique identifier.
		/// </value>
		int Id { get; }
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
		/// Gets the methods.
		/// </summary>
		/// <value>
		/// The methods.
		/// </value>
		IEnumerable<IFulfillmentProviderMethod> Methods { get; }

        /// <summary>
        /// Gets the Zones.
        /// </summary>
        /// <value>
        /// The methods.
        /// </value>
        List<Zone> Zones { get; }
        
		/// <summary>
		/// Gets the type of the fulfillment provider.
		/// </summary>
		/// <value>
		/// The type of the fulfillment provider.
		/// </value>
		ShippingProviderType Type { get; }
	}
}