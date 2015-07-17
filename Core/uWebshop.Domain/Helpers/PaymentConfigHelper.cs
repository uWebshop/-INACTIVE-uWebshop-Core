using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Domain
{
	/// <summary>
	/// 
	/// </summary>
	public class PaymentConfigHelper
	{
		/// <summary>
		/// Gets the provider.
		/// </summary>
		/// <value>
		/// The provider.
		/// </value>
		public PaymentProvider Provider { get; private set; }

		/// <summary>
		/// Gets the settings.
		/// </summary>
		/// <value>
		/// The settings.
		/// </value>
		public IConfigSettings Settings { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="PaymentConfigHelper"/> class.
		/// </summary>
		/// <param name="paymentProvider">The payment provider.</param>
		public PaymentConfigHelper(PaymentProvider paymentProvider)
		{
			Provider = paymentProvider;
			Settings = new ConfigSettings(Provider);
		}

		/// <summary>
		/// Get the payment config file
		/// </summary>
		/// <returns></returns>
		public static string GetPaymentProviderConfig()
		{
			var path = "~/App_Plugins/uWebshop/config/PaymentProviders.config";
			var configFile = HttpContext.Current.Request.MapPath(path);

			if (configFile != null)
			{
				var configFileExists = System.IO.File.Exists(configFile);

				if (configFileExists)
				{
					return path;
				}
			}

			// backup for location of pre uWebshop 2.4 configpath: 
			path = "~/config/uWebshop/PaymentProviders.config";
			configFile = HttpContext.Current.Request.MapPath(path);

			if (configFile != null)
			{
				var configFileExists = System.IO.File.Exists(configFile);

				if (configFileExists)
				{
					return path;
				}
			}
			
			Log.Instance.LogError("GetPaymentProviderConfig: Could not find PaymentProviders.config file!");
			return null;
		}

		public static XDocument GetPaymentProviderConfigXml()
		{
			var path = "~/App_Plugins/uWebshop/config/PaymentProviders.config";
			var configFile = HttpContext.Current.Request.MapPath(path);

			if (configFile != null)
			{
				var configFileExists = System.IO.File.Exists(configFile);

				if (configFileExists)
				{
				   return XDocument.Load(configFile);
				}
			}

			// backup for location of pre uWebshop 2.4 configpath: 
			path = "~/config/uWebshop/PaymentProviders.config";
			configFile = HttpContext.Current.Request.MapPath(path);

			if (configFile != null)
			{
				var configFileExists = System.IO.File.Exists(configFile);

				if (configFileExists)
				{
					return XDocument.Load(configFile);
				}
			}

			Log.Instance.LogError("GetPaymentProviderConfig: Could not find PaymentProviders.config file!");
			return null;
		}

		/// <summary>
		/// 
		/// </summary>
		private class ConfigSettings : IConfigSettings
		{
			private readonly PaymentProvider _provider;
			private readonly IDictionary<string, string> _dictionary = new Dictionary<string, string>();

			internal ConfigSettings(PaymentProvider provider)
			{
				_provider = provider;

				LoadProviderSettings();
			}

			public XElement LoadProviderSettingsXML()
			{
				var configFile = GetPaymentProviderConfig();

				if (string.IsNullOrEmpty(configFile))
				{
					Log.Instance.LogError("LoadProviderSettingsXML: configFile == null");
					return null;
				}

				var configFilePath = HttpContext.Current.Server.MapPath(configFile);

				if (configFilePath != null)
				{
					var doc = XDocument.Load(configFilePath);

					if (doc.Descendants("provider").Any())
					{
						var providerNodes = doc.Descendants("provider").ToList();
						
						var matchedTitleProvider = providerNodes.FirstOrDefault(x =>
						{
							var value = x.Attribute("title").Value;
							return !string.IsNullOrEmpty(value) && value.ToLowerInvariant() == _provider.Name.ToLowerInvariant();
						});

						if (matchedTitleProvider == null)
						{
							matchedTitleProvider = providerNodes.FirstOrDefault(x =>
							{
								var value = x.Attribute("name").Value;
								return !string.IsNullOrEmpty(value) && value.ToLowerInvariant() == _provider.Name.ToLowerInvariant();
							});
						}

						if (matchedTitleProvider != null && matchedTitleProvider.DescendantNodes().Any())
						{
							return matchedTitleProvider;
						}
					}

					Log.Instance.LogError(string.Format("LoadProviderSettingsXML: Could not find provider with title: {0} in PaymentProviders.config", _provider.Name));
				}
				
				
				Log.Instance.LogError(string.Format("LoadProviderSettingsXML: Could not find PaymentProviders.config at location: {0}", configFile));
				

				return null;
			}

			private void LoadProviderSettings()
			{
				var providerNode = LoadProviderSettingsXML();

				if (providerNode == null)
				{
					return;
				}

				// add config settings by name
				foreach (var node in providerNode.Descendants().Where(x => x.Name.ToString().ToLowerInvariant() != "add"))
				{
					if (!_dictionary.ContainsKey(node.Name.ToString().ToLowerInvariant()))
					{
						_dictionary.Add(node.Name.ToString().ToLowerInvariant(), node.Value);
					}
				}
				// add config like it would be in web.config with <add key="" value="" settings.
				foreach (var node in providerNode.Descendants().Where(x => x.Name.ToString().ToLowerInvariant() == "add"))
				{
					var key = node.Attribute("key");

					if (key != null && !string.IsNullOrEmpty(key.Value))
					{
						var value = node.Attribute("value");

						if (value != null && !string.IsNullOrEmpty(value.Value))
						{
							if (!_dictionary.ContainsKey(value.Value.ToLowerInvariant()))
							{
								_dictionary.Add(key.Value.ToLowerInvariant(), value.Value);
							}
						}
					}
				}
			}

			/// <summary>
			/// Determines whether the settings contain given key.
			/// </summary>
			/// <param name="key">The key.</param>
			/// <returns></returns>
			public bool ContainsKey(string key)
			{
				return _dictionary.ContainsKey(key);
			}
			
			/// <summary>
			/// Gets the setting with the specified key.
			/// </summary>
			/// <value>
			/// The setting>.
			/// </value>
			/// <param name="key">The key.</param>
			/// <returns></returns>
			public string this[string key]
			{
				get
				{
					var keyLowercase = key.ToLowerInvariant();
					if (_provider.TestMode)
					{
						var testKeylowercase = "test" + keyLowercase;
						if (_dictionary.ContainsKey(testKeylowercase))
						{
							return _dictionary[testKeylowercase];
						}

						var testKey = "test" + key;
						if (_dictionary.ContainsKey(testKey))
						{
							return _dictionary[testKey];
						}
					}

					if (_dictionary.ContainsKey(keyLowercase))
					{
						return _dictionary[keyLowercase];
					}

					if (_dictionary.ContainsKey(key))
					{
						return _dictionary[key];
					}

					return string.Empty;
				}
			}

			/// <summary>
			/// Gets the number of loaded settings.
			/// </summary>
			public int Count
			{
				get { return _dictionary.Count; }
			}
		}
	}
}