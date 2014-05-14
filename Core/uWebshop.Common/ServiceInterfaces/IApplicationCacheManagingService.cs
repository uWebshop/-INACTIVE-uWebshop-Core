namespace uWebshop.Common.Interfaces
{
	public interface IApplicationCacheManagingService
	{
		void Initialize();
		void ReloadEntityWithGlobalId(int id, string typeName = null);
		void UnloadEntityWithGlobalId(int id, string typeName = null);
		void RebuildTriggeredByRemoteServer();
	}
}