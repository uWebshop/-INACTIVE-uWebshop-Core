using System;
using uWebshop.Domain.BaseClasses;

namespace uWebshop.Domain
{
	/// <summary>
	/// Class representing a file in Umbraco
	/// </summary>
	public class File : MediaBase
	{
		/// <summary>
		/// Gets the name of the file
		/// </summary>
		/// <value>
		/// The name of the file.
		/// </value>
		public string FileName { get; set; }

		/// <summary>
		/// Gets the multilanguage name of the file
		/// </summary>
		/// <value>
		/// The name of the multilanguage file.
		/// </value>
		public string MultilanguageFileName { get; set; }
	}
}