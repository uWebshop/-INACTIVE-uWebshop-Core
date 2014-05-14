namespace uWebshop.Domain.Services
{
	/// <summary>
	/// 
	/// </summary>
	public interface IApplicationCacheService
	{
		/// <summary>
		/// Checks the cache state and rebuild local cache if neccessary.
		/// </summary>
		void CheckCacheStateAndRebuildIfNeccessary();
		/// <summary>
		/// Triggers remote rebuild.
		/// </summary>
		void TriggerRemoteRebuild();
	}
}