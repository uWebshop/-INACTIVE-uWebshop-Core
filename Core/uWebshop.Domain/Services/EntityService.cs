using System.Collections.Generic;
using System.Linq;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Domain.Services
{
	internal abstract class EntityService<T> where T : class, IUwebshopRepositoryEntity
	{
		private readonly IEntityRepository<T> _repository;

		protected EntityService(IEntityRepository<T> repository)
		{
			_repository = repository;
		}

		public T GetById(int id, ILocalization localization, bool includeDisabled = false)
		{
			var entity = _repository.GetById(id, localization);
			if (includeDisabled || entity == null || !entity.Disabled) return entity;
			return null;
		}

		public IEnumerable<T> GetAll(ILocalization localization, bool includeDisabled = false)
		{
			return _repository.GetAll(localization).Where(e => includeDisabled || !e.Disabled);
		}

		public void ReloadEntityWithId(int id)
		{
		}

		public void UnloadEntityWithId(int id)
		{
			// maybe todo
		}

		public void FullResetCache()
		{
		}
	}
}