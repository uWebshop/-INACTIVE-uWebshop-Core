using System.Collections.Generic;

namespace uWebshop.Domain.Interfaces
{
	/// <summary>
	/// 
	/// </summary>
	public interface IDocumentTypeInfo
	{
		/// <summary>
		/// Gets the properties.
		/// </summary>
		/// <value>
		/// The properties.
		/// </value>
		IEnumerable<IDocumentProperty> Properties { get; }
	}
}