using System;
using System.Collections.Generic;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Umbraco.Repositories
{
	interface ICache<T>
	{
		T GetById(int id);
		T GetOrAdd(int id, Func<int, T> factory);
		IEnumerable<T> GetAll();
		ICacheRebuilder GetRebuilder();
	}
}