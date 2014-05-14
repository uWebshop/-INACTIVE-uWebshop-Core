using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Domain.Services
{
	internal abstract class MultiStoreEntityService<T1> : IEntityService<T1> where T1 : class, IUwebshopRepositoryEntity
	{
		// todo: rewrite the two remaining usages of this class and remove it

		protected readonly ConcurrentDictionary<string, List<T1>> _cache = new ConcurrentDictionary<string, List<T1>>();
		private readonly IEntityRepository<T1> _repository;
		protected readonly IStoreService _storeService;

		protected MultiStoreEntityService(IEntityRepository<T1> repository, IStoreService storeService)
		{
			_repository = repository;
			_storeService = storeService;
		}

		public T1 GetById(int id, ILocalization localization, bool includeDisabled = false)
		{
			var entity = GetAll(localization).FirstOrDefault(p => p.Id == id);
			if (entity != null) return entity;
			entity = _repository.GetById(id, localization);
			List<T1> cache;
			if (entity != null && _cache.TryGetValue(GetCacheKey(localization), out cache))
			{
				cache.Add(entity);
			}
			return includeDisabled || (entity != null && !entity.Disabled) ? entity : null;
		}

		public IEnumerable<T1> GetAll(ILocalization localization, bool includeDisabled = false)
		{
			if (localization == null) throw new Exception("Trying to load localized content without localization");
			var entities = _cache.GetOrAdd(GetCacheKey(localization), alias => _repository.GetAll(localization).Cast<T1>().ToList());
			return includeDisabled ? entities : entities.Where(e => e != null && !e.Disabled).ToList();
		}

		public virtual void ReloadEntityWithId(int id)
		{
			// store loopje zou nog een laagje hoger kunnen eventueel
			// alternatief zou je kunnen loopen over de Keys in de dictionary
			//foreach (var localization in _storeService.GetAllLocalizations())
			//{
			//	List<T1> cache;
			//	if (_cache.TryGetValue(GetCacheKey(localization), out cache)) // this fixes strange IndexReader closed exceptions
			//	{
			//		var entity = GetById(id, localization);
			//		_repository.ReloadData(entity, localization);
			//		EntityReloaded(entity);
			//	}
			//}
		}

		protected string GetCacheKey(ILocalization localization)
		{
			return localization.StoreAlias + localization.CurrencyCode;
		}

		public virtual void UnloadEntityWithId(int id)
		{
			// store loopje zou nog een laagje hoger kunnen eventueel
			foreach (var store in _storeService.GetAllStores())
			{
				List<T1> cache;
				if (_cache.TryGetValue(store.Alias, out cache))
				{
					cache.RemoveAll(entity => entity.Id == id);
				}
			}
		}

		protected virtual void EntityReloaded(T1 entity)
		{
		}

		public virtual void FullResetCache()
		{
			_cache.Clear();
		}

		protected abstract void AfterEntitiesLoadedFromRepository(List<T1> entities, string storeAlias);
	}
}