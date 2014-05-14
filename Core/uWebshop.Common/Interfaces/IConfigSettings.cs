using System.Xml.Linq;

namespace uWebshop.Domain.Interfaces
{
	/// <summary>
	/// 
	/// </summary>
	public interface IConfigSettings
	{
		/// <summary>
		/// Determines whether the settings contain given key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns></returns>
		bool ContainsKey(string key);

		/// <summary>
		/// Gets the setting as XMLdocument
		/// </summary>
		/// <returns></returns>
		XElement LoadProviderSettingsXML();

		/// <summary>
		/// Gets the setting with the specified key.
		/// </summary>
		/// <value>
		/// The setting>.
		/// </value>
		/// <param name="key">The key.</param>
		/// <returns></returns>
		string this[string key] { get; }

		/// <summary>
		/// Gets the number of loaded settings.
		/// </summary>
		int Count { get; }
	}
}