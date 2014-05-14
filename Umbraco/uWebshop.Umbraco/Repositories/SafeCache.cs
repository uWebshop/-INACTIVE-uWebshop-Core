using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using uWebshop.Common.Interfaces;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Umbraco.Repositories
{
	class SafeCache<T> : ICache<T> where T : class, IUwebshopEntity
	{
		private readonly Func<IEnumerable<T>> _factory;
		private Lazy<ConcurrentDictionary<int, T>> _all;
		
		public SafeCache(Func<IEnumerable<T>> factory)
		{
			_factory = factory;
			_all = new Lazy<ConcurrentDictionary<int, T>>(() => CreateDictionary(factory));
		}

		private static ConcurrentDictionary<int, T> CreateDictionary(Func<IEnumerable<T>> factory)
		{
			return new ConcurrentDictionary<int, T>(factory().Select(i => new KeyValuePair<int, T>(i.Id, i)));// this is a long duration operation
		}
		public T GetById(int id)
		{
			T res;
			_all.Value.TryGetValue(id, out res);
			return res;
		}

		public T GetOrAdd(int id, Func<int, T> factory)
		{
			T entity;
			if (_all.Value.TryGetValue(id, out entity))
			{
				return entity;
			}
			entity = factory(id);
			if (entity != null)
			{
				_all.Value.TryAdd(id, entity);
			}
			return entity;
		}

		public IEnumerable<T> GetAll()
		{
			if (_all == null) throw new Exception("_all");
			if (_all.Value == null) throw new Exception("_all.Value");
			return _all.Value.Values;
		}

		public ICacheRebuilder GetRebuilder()
		{
			return new Rebuilder(_factory, newCache =>
				{
					if (newCache == null) throw new Exception("Building new cache with null values");
					_all = new Lazy<ConcurrentDictionary<int, T>>(() => newCache);
				});
		}

		class Rebuilder : ICacheRebuilder
		{
			private readonly Func<IEnumerable<T>> _factory;
			private readonly Action<ConcurrentDictionary<int, T>> _setNewCache;
			private ConcurrentDictionary<int, T> _newCache;

			public Rebuilder(Func<IEnumerable<T>> factory, Action<ConcurrentDictionary<int, T>> setNewCache)
			{
				_factory = factory;
				_setNewCache = setNewCache;
			}

			public void Lock()
			{
				//Monitor.Enter(_factory); // todo!! why disabled??
			}

			public void Rebuild()
			{
				_newCache = CreateDictionary(_factory);
			}

			public void SwitchCache()
			{
				if (_newCache == null) throw new Exception("SwitchCache before rebuild");
				_setNewCache(_newCache);
			}

			public void Unlock()
			{
				//Monitor.Exit(_factory);
			}
		}
	}
}