using System.Collections.Generic;
using uWebshop.Domain.Helpers;

namespace uWebshop.Domain.Interfaces
{
	/// <summary>
	/// 
	/// </summary>
	interface ICMSEntityRepository
	{
		/// <summary>
		/// Gets the by global unique identifier.
		/// </summary>
		/// <param name="globalId">The global unique identifier.</param>
		/// <returns></returns>
		UwbsNode GetByGlobalId(int globalId);

		/// <summary>
		/// Gets all.
		/// </summary>
		/// <returns></returns>
		IEnumerable<UwbsNode> GetAll();

		/// <summary>
		/// Gets the objects by alias.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="nodeTypeAlias">The node type alias.</param>
		/// <param name="localization">The localization.</param>
		/// <param name="startNodeId">The start node unique identifier.</param>
		/// <returns></returns>
		IEnumerable<T> GetObjectsByAlias<T>(string nodeTypeAlias, ILocalization localization = null, int startNodeId = 0);

		/// <summary>
		/// Gets the objects by alias uncached.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="nodeTypeAlias">The node type alias.</param>
		/// <param name="localization">The localization.</param>
		/// <param name="startNodeId">The start node unique identifier.</param>
		/// <returns></returns>
		IEnumerable<T> GetObjectsByAliasUncached<T>(string nodeTypeAlias, ILocalization localization = null, int startNodeId = 0);

		/// <summary>
		/// Gets the node with store picker.
		/// </summary>
		/// <param name="storeId">The store unique identifier.</param>
		/// <returns></returns>
		UwbsNode GetNodeWithStorePicker(int storeId);

		IEnumerable<UwbsNode> GetNodesWithStorePicker(int storeId);
	}

	//public interface ICMSEntityService
}