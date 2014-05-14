using System.Collections.Generic;

namespace uWebshop.Domain.Interfaces
{
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IEntityRepository<T>
	{
		/// <summary>
		/// Gets the by unique identifier.
		/// </summary>
		/// <param name="id">The unique identifier.</param>
		/// <param name="localization">The localization.</param>
		/// <returns></returns>
		T GetById(int id, ILocalization localization);

		/// <summary>
		/// Gets all.
		/// </summary>
		/// <param name="localization">The localization.</param>
		/// <returns></returns>
		List<T> GetAll(ILocalization localization);

		/// <summary>
		/// Reloads the data.
		/// </summary>
		/// <param name="entity">The entity.</param>
		/// <param name="localization">The localization.</param>
		void ReloadData(T entity, ILocalization localization);
	}
}