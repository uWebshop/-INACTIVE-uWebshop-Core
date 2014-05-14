using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Domain.Services
{
	class StoreFromUrlDeterminationService : IStoreFromUrlDeterminationService
	{
		private readonly IStoreUrlService _storeUrlService;

		public StoreFromUrlDeterminationService(IStoreUrlService storeUrlService)
		{
			_storeUrlService = storeUrlService;
		}

		public StoreUrlDeterminationResult DetermineStoreAndUrlParts(string domain, string url)
		{
			var urlIncludingDomain = (domain.TrimEnd('/') + "/" + url.TrimStart('/'));
			var urlIncludingDomainHttpStripped = urlIncludingDomain.Replace("http://", "").Replace("https://", "");
			var storeUrlsWithDomain = _storeUrlService.GetStoreUrlsWithDomain();
			var match = storeUrlsWithDomain.Where(t => urlIncludingDomainHttpStripped.StartsWith(t.Url.Replace("http://", "").Replace("https://", "").TrimEnd('/')))
				.OrderByDescending(t => t.Url.Length).FirstOrDefault();

			if (match != null)
			{
				var matchurlHttpStripped = match.Url.Replace("http://", "").Replace("https://", "");
				var one = urlIncludingDomain.IndexOf(matchurlHttpStripped);
				var two = matchurlHttpStripped.Length - 1;

				var value = urlIncludingDomain.Substring(one + two);
				return new StoreUrlDeterminationResult(match.Store, match.Url, value);
			}
			
			var storeUrlsWithoutDomain = _storeUrlService.GetStoreUrlsWithoutDomain();
			
			if (url != "/")
			{
				url = url.TrimStart('/');
			}

			var matches = storeUrlsWithoutDomain.Where(store => store.Url == "/" || url.ToLowerInvariant().StartsWith(store.Url.Trim('/').ToLowerInvariant())).ToList();

			match = matches.Skip(1).Any() ? matches.FirstOrDefault(m => m.Url != "/") : matches.FirstOrDefault();
			if (match != null)
			{
				var catalogUrl = match.Url == "/" ? url : url.Substring(url.IndexOf(match.Url.TrimStart('/')) + match.Url.Length - 1);

				return new StoreUrlDeterminationResult(match.Store, match.Url, catalogUrl);
			}
			return null;
		}
	}
}