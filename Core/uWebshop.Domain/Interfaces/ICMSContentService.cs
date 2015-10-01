using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace uWebshop.Domain.Interfaces
{
	/// <summary>
	/// 
	/// </summary>
	internal interface ICMSContentService
	{
		/// <summary>
		/// Gets the by unique identifier.
		/// </summary>
		/// <param name="id">The unique identifier.</param>
		/// <returns></returns>
		IUwebshopReadonlyContent GetReadonlyById(int id);

		IEnumerable<IUwebshopReadonlyContent> GetAllRootNodes();
		
		/// <summary>
		/// Gets the by unique identifier.
		/// </summary>
		/// <param name="id">The unique identifier.</param>
		/// <returns></returns>
		IUwebshopContent GetById(int id);

		/// <summary>
		/// Gets the file by unique identifier.
		/// </summary>
		/// <param name="id">The unique identifier.</param>
		/// <returns></returns>
		File GetFileById(int id);

		/// <summary>
		/// Gets the image by unique identifier.
		/// </summary>
		/// <param name="id">The unique identifier.</param>
		/// <returns></returns>
		Image GetImageById(int id);
	}
}