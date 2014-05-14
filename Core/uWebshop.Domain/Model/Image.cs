using System;
using uWebshop.Domain.BaseClasses;

namespace uWebshop.Domain
{
	/// <summary>
	/// Class representing an image in Umbraco
	/// </summary>
	public class Image : MediaBase
	{
		/// <summary>
		/// Gets the width of the image
		/// </summary>
		/// <value>
		/// The width.
		/// </value>
		public int Width { get; set; }

		/// <summary>
		/// Gets the height of the image
		/// </summary>
		/// <value>
		/// The height.
		/// </value>
		public int Height { get; set; }
	}
}