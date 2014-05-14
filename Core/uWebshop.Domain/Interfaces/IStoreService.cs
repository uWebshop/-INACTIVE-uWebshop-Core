using System.Collections;
using System;
using System.Collections.Generic;

namespace uWebshop.Domain.Interfaces
{
	/// <summary>
	/// 
	/// </summary>
	interface IStoreService
	{
		/// <summary>
		/// Gets the current store.
		/// </summary>
		/// <returns></returns>
		Store GetCurrentStore();

		// hmm

		/// <summary>
		/// Currents the store alias.
		/// </summary>
		/// <returns></returns>
		string CurrentStoreAlias();

		/// <summary>
		/// Gets the current localization.
		/// </summary>
		/// <returns></returns>
		ILocalization GetCurrentLocalization();

		/// <summary>
		/// Gets all stores.
		/// </summary>
		/// <returns></returns>
		IEnumerable<Store> GetAllStores();

		/// <summary>
		/// Gets the by unique identifier.
		/// </summary>
		/// <param name="id">The unique identifier.</param>
		/// <param name="localization">The localization.</param>
		/// <returns></returns>
		Store GetById(int id, ILocalization localization);

		/// <summary>
		/// Gets the by alias.
		/// </summary>
		/// <param name="alias">The alias.</param>
		/// <returns></returns>
		Store GetByAlias(string alias);

		/// <summary>
		/// Loads the store URL.
		/// </summary>
		/// <param name="store">The store.</param>
		void LoadStoreUrl(Store store);

		/// <summary>
		/// Renames the store.
		/// </summary>
		/// <param name="oldStoreAlias">The old store alias.</param>
		/// <param name="newStoreAlias">The new store alias.</param>
		void RenameStore(string oldStoreAlias, string newStoreAlias);

		/// <summary>
		/// Gets the nice URL.
		/// </summary>
		/// <param name="id">The unique identifier.</param>
		/// <param name="categoryId">The category unique identifier.</param>
		/// <param name="localization">The localization.</param>
		/// <returns></returns>
		string GetNiceUrl(int id, int categoryId, ILocalization localization);

		/// <summary>
		/// Gets the actual store, no fallback if none can be determined
		/// </summary>
		/// <returns></returns>
		Store GetCurrentStoreNoFallback();

		void RegisterStoreChangedEvent(Action<Store> e);
		void TriggerStoreChangedEvent(Store store);
		void InvalidateCache(int storeId = 0);

		/// <summary>
		/// Gets all localizations.
		/// </summary>
		/// <returns></returns>
		IEnumerable<ILocalization> GetAllLocalizations();
	}
}