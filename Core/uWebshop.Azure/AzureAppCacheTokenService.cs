using System;
using uWebshop.Common.Interfaces;

namespace uWebshop.Azure
{
	class AzureAppCacheTokenService : IApplicationCacheTokenService
	{
		private const string CacheKey = "uWebshop5b4ce6f8-c3f3-438d-9f6c-925f05c79baf";

		public Guid GetToken()
		{
			var mycache = new DataCache();

			var cacheObject = mycache.Get(CacheKey);

			if (cacheObject == null || string.IsNullOrEmpty(cacheObject.ToString())) return Guid.Empty;
			if (cacheObject is Guid) return (Guid) cacheObject;

			Guid result;
			if (Guid.TryParse(cacheObject.ToString(), out result))
				return result;

			return Guid.Empty;
		}

		public void SetToken(Guid token)
		{
			var mycache = new DataCache();
			mycache.Put(CacheKey, token);
		}
	}
}