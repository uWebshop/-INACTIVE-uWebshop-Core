using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Domain.Services
{
    internal abstract class EntityService<T> where T : class, IUwebshopRepositoryEntity
    {
        protected readonly ConcurrentDictionary<string, List<T>> _cache 
                     = new ConcurrentDictionary<string, List<T>>();

        private readonly IEntityRepository<T> _repository;

        protected EntityService(IEntityRepository<T> repository)
        {
            _repository = repository;
        }

        public T GetById(int id, ILocalization localization, bool includeDisabled = false)
        {
            var entity = GetAll(localization).FirstOrDefault(p => p.Id == id);

            if (entity != null) return entity;

            entity = _repository.GetById(id, localization);

            List<T> cache;
            if (entity != null && _cache.TryGetValue(GetCacheKey(localization), out cache))
            {
                // We lock on the list itself, enabling operations on multiple localizations concurrently
                lock (cache) cache.Add(entity);
            }

            return includeDisabled || (entity != null && !entity.Disabled) ? entity : null;
        }

        public IEnumerable<T> GetAll(ILocalization localization, bool includeDisabled = false)
        {
            if (localization == null) throw new Exception("Trying to load localized content without localization");

            var entities = _cache.GetOrAdd(GetCacheKey(localization), alias => _repository.GetAll(localization).Cast<T>().ToList());

            if (includeDisabled) return entities;

            // Again we lock on the list itself, 
            // enabling operations on multiple localizations concurrently
            else lock (entities) return entities.Where(e => e != null && !e.Disabled).ToList();
        }

        protected string GetCacheKey(ILocalization localization)
        {
            return localization.StoreAlias + localization.CurrencyCode;
        }

        public void ReloadEntityWithId(int id)
        {
        }

        public virtual void UnloadEntityWithId(int id)
        {
            // todo?   
        }

        public void FullResetCache()
        {
            _cache.Clear();
        }
    }
}