using System;
using System.Web;
using uWebshop.Common.Interfaces;

namespace uWebshop.Domain.Services
{
	class LoadBalancedApplicationCacheService : IApplicationCacheService
	{
		private readonly IApplicationCacheTokenService _appCacheTokenService;
		private Guid _localCacheToken;

		public LoadBalancedApplicationCacheService(IApplicationCacheTokenService appCacheTokenService)
		{
			_appCacheTokenService = appCacheTokenService;
		}

		public void CheckCacheStateAndRebuildIfNeccessary()
		{
			Guid token;
			try
			{
				token = _appCacheTokenService.GetToken();
			}
			catch (Exception exception)
			{
				Log.Instance.LogError("Error in load balanced caching, did you install the azure binaries and configured the azure cache settings? Exception: " + exception.Message);
				return;
			}

			if (token == Guid.Empty)
			{	// first ever time
				SetNewToken();
				return;
			}

			if (_localCacheToken == Guid.Empty)
			{	// local first time
				_localCacheToken = token;
				Log.Instance.LogDebug("LOADBALANCE Local first time, loaded token " + _localCacheToken + " at " + HttpContext.Current.Request["LOCAL_ADDR"]);
				return;
			}

			// compare global with local
			if (!token.Equals(_localCacheToken))
			{	// if different trigger cache rebuild
				Log.Instance.LogDebug("LOADBALANCE Token changed from " + _localCacheToken + " to " + token + " at " + HttpContext.Current.Request["LOCAL_ADDR"]);
				_localCacheToken = token;

				IO.Container.Resolve<IApplicationCacheManagingService>().RebuildTriggeredByRemoteServer(); // circular dependency
			}
		}

		private void SetNewToken()
		{
			_localCacheToken = Guid.NewGuid();
			if (Log.Instance != null)
			{
				var machine = "unknown";
				if (HttpContext.Current != null && HttpContext.Current.Request["LOCAL_ADDR"] != null)
				{
					machine = HttpContext.Current.Request["LOCAL_ADDR"];
				}
				Log.Instance.LogDebug("LOADBALANCE Generated new token " + _localCacheToken + " at " + machine);
			}
			_appCacheTokenService.SetToken(_localCacheToken);
		}

		public void TriggerRemoteRebuild()
		{
			Log.Instance.LogDebug("Triggering remote servers to rebuild");
			SetNewToken();
		}
	}
}
