using System;

namespace uWebshop.Common.Interfaces
{
	/// <summary>
	/// 
	/// </summary>
	public interface IUwebshopEntity
	{
		/// <summary>
		/// Gets the unique identifier.
		/// </summary>
		/// <value>
		/// The unique identifier.
		/// </value>
		int Id { get; }

        /// <summary>
        /// Gets the unique identifier.
        /// </summary>
        /// <value>
        /// The unique identifier.
        /// </value>
        Guid Key { get; }

        /// <summary>
        /// Gets the name or alias for the type. (NodeTypeAlias/ContentTypeAlias in Umbraco)
        /// </summary>
        /// <value>
        /// The type alias.
        /// </value>
        string TypeAlias { get; }
	}
}