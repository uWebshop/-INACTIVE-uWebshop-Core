using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using uWebshop.Domain;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;
using uWebshop.Test.Stubs;

namespace uWebshop.Test.Repositories
{
	public class TestStoreRepository : IStoreRepository
	{
		private static List<Store> _stores; // = new List<Store> {DefaultFactoriesAndSharedFunctionality.CreateDefaultStore()};

		public static List<Store> Stores
		{
			get { return _stores ?? (_stores = new List<Store> { StubStore.CreateDefaultStore() }); }
		}

		public List<Store> GetAll()
		{
			return Stores;
		}

		public Store TryGetStoreFromBackendCurrentOrder()
		{
			return null;
		}

		public Store TryGetStoreFromCurrentNode()
		{
			return null;
		}

		public Store TryGetStoreFromCookie()
		{
			return null;
		}

		public Store GetById(int id, ILocalization localization)
		{
			return Stores.FirstOrDefault();
		}
	}
}