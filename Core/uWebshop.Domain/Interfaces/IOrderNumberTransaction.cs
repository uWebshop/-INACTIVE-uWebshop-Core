using System;

namespace uWebshop.Domain.Interfaces
{
	interface IOrderNumberTransaction : IDisposable
	{
		void Generate();
		void Persist();
		void Rollback();
	}
}