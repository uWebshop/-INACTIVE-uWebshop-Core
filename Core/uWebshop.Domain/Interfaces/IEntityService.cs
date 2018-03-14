using System.Collections.Generic;

namespace uWebshop.Domain.Interfaces
{
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IEntityService<T>
	{
		/// <summary>
		/// Gets the by unique identifier.
		/// </summary>
		/// <param name="id">The unique identifier.</param>
		/// <param name="localization">The localization.</param>
		/// <param name="includeDisabled">if set to <c>true</c> [include disabled].</param>
		/// <returns></returns>
		T GetById(int id, ILocalization localization, bool includeDisabled = false);

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <param name="localization">The localization.</param>
        /// <param name="includeDisabled">if set to <c>true</c> [include disabled].</param>
        /// <returns></returns>
        IEnumerable<T> GetAll(ILocalization localization, bool includeDisabled = false);

		/// <summary>
		/// Reloads the entity with unique identifier.
		/// </summary>
		/// <param name="id">The unique identifier.</param>
		void ReloadEntityWithId(int id);

		/// <summary>
		/// Unloads the entity with unique identifier.
		/// </summary>
		/// <param name="id">The unique identifier.</param>
		void UnloadEntityWithId(int id);

		/// <summary>
		/// Resets the cache.
		/// </summary>
		void FullResetCache();
	}
}