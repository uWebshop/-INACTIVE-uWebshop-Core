using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace uWebshop.Domain.Interfaces
{
	/// <summary>
	/// 
	/// </summary>
	public interface ICustomerType
	{
		/// <summary>
		/// Gets the alias.
		/// </summary>
		/// <value>
		/// The alias.
		/// </value>
		string Alias { get; }
	}

    public interface ICustomerGroup
    {
        /// <summary>
        /// Gets the alias.
        /// </summary>
        /// <value>
        /// The alias.
        /// </value>
        string Alias { get; }
    }

}