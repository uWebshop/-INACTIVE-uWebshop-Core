using System.Collections.Generic;
using System.Web;
using System.Xml;

namespace uWebshop.Domain
{
	/// <summary>
	/// 
	/// </summary>
	public class ShippingConfigHelper
	{
		/// <summary>
		/// Gets the provider.
		/// </summary>
		/// <value>
		/// The provider.
		/// </value>
		public ShippingProvider Provider { get; private set; }

		/// <summary>
		/// Gets the settings.
		/// </summary>
		/// <value>
		/// The settings.
		/// </value>
		public Dictionary<string, string> Settings { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="ShippingConfigHelper"/> class.
		/// </summary>
		/// <param name="shippingProvider">The shipping provider.</param>
		public ShippingConfigHelper(ShippingProvider shippingProvider)
		{
			Provider = shippingProvider;

			LoadProviderSettings();
		}
		
		private void LoadProviderSettings()
		{
			var path = "~/App_Plugins/uWebshop/config/ShippingProviders.config";
			var configFile = HttpContext.Current.Request.MapPath(path);

			if (!System.IO.File.Exists(configFile))
			{
				path = "~/config/uWebshop/ShippingProviders.config";
				configFile = HttpContext.Current.Request.MapPath(path);
			}

			if (!System.IO.File.Exists(configFile))
			{
				Log.Instance.LogError("Could not find PaymentProviders.config file!");
				return;
			}

			var doc = new XmlDocument();
			doc.Load(HttpContext.Current.Server.MapPath(path));

			XmlNode providerNode = doc.SelectSingleNode(string.Format("//provider[translate(@title, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz')='{0}']", Provider.Node.Name.ToLower()));

			if (providerNode == null)
			{
				Log.Instance.LogError(string.Format("Could not find provider with title {0} in ShippingProviders.config", Provider.Node.Name));

				return;
			}

			Settings = new Dictionary<string, string>();

			if (providerNode.ChildNodes.Count == 0) return;
			foreach (XmlNode node in providerNode.ChildNodes)
			{
				Settings.Add(node.Name, node.InnerText);
			}
		}
	}
}