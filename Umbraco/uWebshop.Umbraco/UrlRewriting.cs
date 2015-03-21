using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Web;
using uWebshop.Umbraco;
using umbraco;
using uWebshop.ActionHandlers;
using uWebshop.Domain;
using uWebshop.Domain.Interfaces;
using uWebshop.Umbraco.Businesslogic;
using Umbraco.Core.Configuration;
using Umbraco.Core.Logging;
using Log = uWebshop.Domain.Log;

//[assembly: PreApplicationStartMethod(typeof (UwebshopUmbracoStartup), "Startup")]

namespace uWebshop.ActionHandlers
{
	public class UwebshopUmbracoStartup
	{
		public static void Startup()
		{
			try
			{
				// this doesn't seem to work because the modules come at the end of the modules list (after umbraco...)
				//DynamicModuleUtility.RegisterModule(typeof(UrlRewriting));
				//DynamicModuleUtility.RegisterModule(typeof(aaaaaaaUwebshop));
			}
			catch (Exception ex)
			{
				LogHelper.Error(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "Error while initializing uWebshop, most likely due to wrong umbraco.config, please republish the site", ex);

				throw;
			}
		}
	}

	public class UrlRewriting : IHttpModule
	{
		static UrlRewriting()
		{
			try
			{
				Domain.Core.Initialize.ContinueInitialization();
				Log.Instance.LogDebug("uWebshop initialized");
			}
			catch (Exception)
			{
				//umbraco.BusinessLogic.Log.Add(LogTypes.Error, 0, "Error while initializing uWebshop, most likely due to wrong umbraco.config, please republish the site");
				//throw;
			}
			try
			{
				NoRewriting = (InternalHelpers.MvcRenderMode && (UmbracoVersion.Current.Major > 6 || UmbracoVersion.Current.Major == 6 && UmbracoVersion.Current.Minor >= 1));
			}
			catch (Exception)
			{
			}
		}

		private static bool NoRewriting;

		public void Init(HttpApplication app)
		{
			Domain.Core.Initialize.ContinueInitialization();

			if (!NoRewriting)
			{
				app.BeginRequest += RewriteUrlsOnAppBeginRequest;
			}
		}

		public void Dispose()
		{
		}

		private void RewriteUrlsOnAppBeginRequest(object source, EventArgs e)
		{
			if (!UmbracoAddon.VersionSpecificTypesConfiguredInIOCContainer) // if the v4 or v6 DLL is not present, exceptions will be thrown by the RewritingService
				return;

			var context = ((HttpApplication) source).Context;
			var contextAbsPath = context.Request.Url.AbsolutePath;

			try
			{
				var rewritingService = IO.Container.Resolve<IUrlRewritingService>();
				rewritingService.RedirectPermanentOldCatalogUrls();
				rewritingService.Rewrite();
			}
			catch (Exception ex)
			{
				Log.Instance.LogDebug("Exception while urlRewriting: " + ex + ", url: " + contextAbsPath);
				//	if (umbraco.GlobalSettings.DebugMode)
				//		throw;
			}
		}

		private static Lazy<IUrlRewritingService> _urlRewritingService = new Lazy<IUrlRewritingService>(() => IO.Container.Resolve<IUrlRewritingService>());
	}
}