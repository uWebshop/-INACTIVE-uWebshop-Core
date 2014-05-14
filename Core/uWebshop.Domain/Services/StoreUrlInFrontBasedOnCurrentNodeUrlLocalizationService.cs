using System;
using System.Threading;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;
using System.Linq;

namespace uWebshop.Domain.Services
{
	internal class StoreUrlInFrontBasedOnCurrentNodeUrlLocalizationService : IUrlLocalizationService
	{
		public string LocalizeCatalogUrl(string catalogUrl, ILocalization localization)
		{
			if (catalogUrl == null) throw new ArgumentNullException("catalogUrl");
			if (localization == null) throw new ArgumentNullException("localization");

			var store = (Store)localization.Store;
			//if (store.CanonicalStoreURL == null) throw new Exception("store.CanonicalStoreURL == null");
			//if (store.StoreURL == null) throw new Exception("store.StoreURL == null");

			if (store.CanonicalStoreURL == null || store.StoreURL == null)
			{
				Log.Instance.LogWarning("No store url known for store " + store.Alias + " waiting 300ms for possible cache rebuild");
				Thread.Sleep(300);
			}

			var storeURL = localization.Store.StoreURL ?? string.Empty;
			return storeURL.TrimEnd('/') + "/" + catalogUrl.TrimStart('/');
		}
	}
}