namespace uWebshop.Domain.Interfaces
{
	/// <summary>
	/// 
	/// </summary>
	public interface ICMSInstaller : IInstaller
	{
		/// <summary>
		/// Installs the store picker configuration node with unique identifier.
		/// </summary>
		/// <param name="nodeId">The node unique identifier.</param>
		/// <param name="feedbackSmall">The feedback small.</param>
		/// <param name="feedbackLarge">The feedback large.</param>
		/// <returns></returns>
		bool InstallStorePickerOnNodeWithId(int nodeId, out string feedbackSmall, out string feedbackLarge);

		/// <summary>
		/// Installs the sand box starterkit.
		/// </summary>
		/// <param name="starterkit">the name of the starterkit</param>
		/// <param name="storePresent">if set to <c>true</c> [store present].</param>
		/// <returns></returns>
		bool InstallStarterkit(string starterkit, out bool storePresent);
	}
}