using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using uWebshop.Domain;
using uWebshop.Domain.Interfaces;
using uWebshop.Domain.Model;

namespace uWebshop.Test.Repositories
{
	// todo: zou deze class overbodig zijn geworden?
	public class TestStoreService : IStoreService
	{
		public Store GetCurrentStore()
		{
			return TestStoreRepository.Stores.First();
		}

		public string CurrentStoreAlias()
		{
			return GetCurrentStore().Alias;
		}

		public ILocalization GetCurrentLocalization()
		{
			var currentStore = GetCurrentStore();
			return Localization.CreateLocalization(currentStore, new RegionInfo(currentStore.DefaultCurrencyCultureInfo.LCID).ISOCurrencySymbol);
		}

		public IEnumerable<Store> GetAllStores()
		{
			return new List<Store> {GetCurrentStore()};
		}

		public Store GetById(int id, ILocalization localization)
		{
			throw new System.NotImplementedException();
		}

		public Store GetByAlias(string alias)
		{
			return GetCurrentStore();
		}

		public void LoadStoreUrl(Store store)
		{
			//store.StoreURL = "http://my.uwebshop.com/";
		}

		public void RenameStore(string oldStoreAlias, string newStoreAlias)
		{
			GetByAlias(oldStoreAlias).Alias = newStoreAlias;
		}

		public string GetNiceUrl(int id, int categoryId, ILocalization localization)
		{
			throw new System.NotImplementedException();
		}

		public Store GetCurrentStoreNoFallback()
		{
			return GetCurrentStore();
		}

		public void RegisterStoreChangedEvent(Action<Store> e)
		{
		}

		public void TriggerStoreChangedEvent(Store store)
		{
		}

		public void InvalidateCache(int storeId = 0)
		{
			
		}

		public IEnumerable<ILocalization> GetAllLocalizations()
		{
			throw new System.NotImplementedException();
		}
	}
}