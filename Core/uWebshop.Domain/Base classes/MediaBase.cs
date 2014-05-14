using System;
using System.Runtime.Serialization;

namespace uWebshop.Domain.BaseClasses
{
	/// <summary>
	/// Class based on the Umbraco media class to inherit from
	/// </summary>
	[DataContract(Name = "Media", Namespace = "")]
	public class MediaBase
	{
		/// <summary>
		///     Gets the id of the media item
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		///     Gets the id of the parent of the media item
		/// </summary>
		public int ParentId { get; set; }

		/// <summary>
		///     Gets a System.DateTime object that is set to the date and time when the media item is created
		/// </summary>
		public DateTime CreateDateTime { get; set; }

		/// <summary>
		///     Gets a value that indicates whether the item is in the trash
		/// </summary>
		public bool IsTrashed { get; set; }

		/// <summary>
		///     Gets the file extension of the file
		/// </summary>
		public string FileExtension { get; set; }

		/// <summary>
		///     Gets the filesize of the file in bytes
		/// </summary>
		public long FileSize { get; set; }

		/// <summary>
		///     Gets the relative path to the file from the root of the umbraco installation path
		/// </summary>
		public string RelativePathToFile { get; set; }
	}
}