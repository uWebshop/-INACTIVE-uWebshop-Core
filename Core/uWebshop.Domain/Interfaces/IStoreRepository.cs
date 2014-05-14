using System.Collections.Generic;

namespace uWebshop.Domain.Interfaces
{
	internal interface IStoreRepository
	{
		List<Store> GetAll();
		Store TryGetStoreFromCurrentNode();
		Store TryGetStoreFromCookie();
		Store GetById(int id, ILocalization localization);
	}
}