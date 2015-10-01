using System;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Domain.Businesslogic
{
	internal class UwebshopDefaultInstaller : IInstaller
	{
		private readonly ICMSInstaller _cmsInstaller;

		public UwebshopDefaultInstaller(ICMSInstaller cmsInstaller)
		{
			_cmsInstaller = cmsInstaller;
		}

		public void Install()
		{
			Log.Instance.LogDebug("uWebshop Installer InstalluWebshopDocumentTypes Start: " + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"));

			_cmsInstaller.Install();

			Log.Instance.LogDebug("uWebshop Installer InstalluWebshopDocumentTypes End: " + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"));
		}
	}
}