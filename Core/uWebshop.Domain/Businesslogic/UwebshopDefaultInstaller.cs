using System;
using uWebshop.DataAccess;
using uWebshop.Domain.Interfaces;
using uWebshop.Domain.Upgrading;

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
			try
			{
				Log.Instance.LogDebug("uWebshop Installer InstallOrderTable Start: " + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"));
				uWebshopOrders.InstallOrderTable();
				Log.Instance.LogDebug("uWebshop Installer InstallOrderTable End: " + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"));
			}
			catch
			{
				Log.Instance.LogError("uWebshopOrders.InstallOrderTable()");
			}
			try
			{
				Log.Instance.LogDebug("uWebshop Installer InstallOrderSeriesTable Start: " + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"));
				uWebshopOrders.InstallOrderSeriesTable();
				Log.Instance.LogDebug("uWebshop Installer InstallOrderSeriesTable End: " + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"));
			}
			catch
			{
				Log.Instance.LogError("uWebshopOrders.InstallOrderSeriesTable()");
			}
			try
			{
				Log.Instance.LogDebug("uWebshop Installer AddStoreOrderReferenceIdToExistingOrders Start: " + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"));
				new OrderTableUpdater().AddStoreOrderReferenceIdToExistingOrders();
				Log.Instance.LogDebug("uWebshop Installer AddStoreOrderReferenceIdToExistingOrders End: " + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"));
			}
			catch
			{
				Log.Instance.LogError("AddStoreOrderReferenceIdToExistingOrders");
			}

			try
			{
				Log.Instance.LogDebug("uWebshop Installer InstallStockTable Start: " + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"));
				UWebshopStock.InstallStockTable();
				Log.Instance.LogDebug("uWebshop Installer InstallStockTable End: " + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"));
			}
			catch
			{
				Log.Instance.LogError("uWebshopOrders.InstallStockTable()");
			}

			try
			{
				Log.Instance.LogDebug("uWebshop Installer InstallCouponsTable Start: " + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"));
				IO.Container.Resolve<ICouponCodeService>().InstallCouponsTable();
				Log.Instance.LogDebug("uWebshop Installer InstallCouponsTable End: " + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"));
			}
			catch
			{
				Log.Instance.LogError("uWebshopOrders.InstallCouponsTable()");
			}

			Log.Instance.LogDebug("uWebshop Installer InstalluWebshopDocumentTypes Start: " + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"));

			_cmsInstaller.Install();

			Log.Instance.LogDebug("uWebshop Installer InstalluWebshopDocumentTypes End: " + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"));
		}
	}
}