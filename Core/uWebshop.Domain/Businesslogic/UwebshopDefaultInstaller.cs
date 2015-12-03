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

		public void Install(bool createMissingProperties = false)
		{
			Log.Instance.LogDebug("uWebshop Installer InstalluWebshopDocumentTypes Start: " + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"));

			_cmsInstaller.Install(createMissingProperties);

			Log.Instance.LogDebug("uWebshop Installer InstalluWebshopDocumentTypes End: " + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"));
		}
	}
}