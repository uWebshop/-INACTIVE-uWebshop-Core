using System;
using uWebshop.Common.Interfaces;

namespace uWebshop.Domain.Services
{
	class SqlAppCacheTokenService : IApplicationCacheTokenService
	{
		public Guid GetToken()
		{
			throw new NotImplementedException();
		}

		public void SetToken(Guid token)
		{
			throw new NotImplementedException();
		}
	}
}