using Examine;
using uWebshop.Domain.BaseClasses;

namespace uWebshop.Domain
{
	/// <summary>
	/// 
	/// </summary>
	public class UwbsImage : uWebshopEntity
	{
		/// <summary>
		/// The umbraco file
		/// </summary>
		public string UmbracoFile;

		/// <summary>
		/// Initializes a new instance of the <see cref="UwbsImage"/> class.
		/// </summary>
		public UwbsImage()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UwbsImage"/> class.
		/// </summary>
		/// <param name="examineNode">The examine node.</param>
		public UwbsImage(SearchResult examineNode)
		{
			if (examineNode.Fields.ContainsKey("umbracoFile"))
			{
				UmbracoFile = examineNode.Fields["umbracoFile"];
			}
		}
	}
}