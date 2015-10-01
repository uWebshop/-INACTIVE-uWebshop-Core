using System;
using uWebshop.Common.Interfaces;
using Umbraco.Core.Logging;
using umbraco;

namespace uWebshop.Umbraco.Services
{
	internal class UmbracoLoggingService : ILoggingService
	{
		private static bool? _uWebshopDebugMessages;

		private static bool UWebshopDebugMessages
		{
			get { return _uWebshopDebugMessages ?? (_uWebshopDebugMessages = System.Web.Configuration.WebConfigurationManager.AppSettings["uWebshopDebugMessages"] == "true" || GlobalSettings.DebugMode).GetValueOrDefault(); }
		}

		private static bool? _uWebshopYSODOnError;

		private static bool UWebshopYSODOnError
		{
			get { return _uWebshopYSODOnError ?? (_uWebshopYSODOnError = System.Web.Configuration.WebConfigurationManager.AppSettings["uWebshopYSODOnError"] == "true").GetValueOrDefault(); }
		}

		public void LogError(Exception exception, string message = null)
		{
			LogHelper.Error<UmbracoLoggingService>(message, exception);
			if (UWebshopYSODOnError)
			{
				throw exception;
			}
		}

		public void LogError(string message)
		{
			LogHelper.Error<UmbracoLoggingService>(message, null);
		}

		public void LogWarning(string message)
		{
			LogHelper.Warn<UmbracoLoggingService>(message);
		}

		public void LogDebug(string message)
		{
			if (UWebshopDebugMessages)
				LogHelper.Info<UmbracoLoggingService>(message);
		}
	}
}