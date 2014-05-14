using System.Collections.Generic;
using System.Linq;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Test.Repositories
{
	public class TestCMSEntityRepository : ICMSEntityRepository
	{
		public List<UwbsNode> Entities = new List<UwbsNode>();

		public void Add(UwbsNode entity)
		{
			Entities.Add(entity);
		}

		public UwbsNode GetByGlobalId(int globalId)
		{
			return Entities.FirstOrDefault(entity => entity.Id == globalId);
		}

		public IEnumerable<UwbsNode> GetAll()
		{
			return Entities;
		}

		public IEnumerable<T> GetObjectsByAlias<T>(string nodeTypeAlias, ILocalization localization = null, int startNodeId = 0)
		{
			throw new System.NotImplementedException();
		}

		public IEnumerable<T> GetObjectsByAliasUncached<T>(string nodeTypeAlias, ILocalization localization = null, int startNodeId = 0)
		{
			throw new System.NotImplementedException();
		}

		public UwbsNode GetNodeWithStorePicker(int storeId)
		{
			return null;
		}

		public IEnumerable<UwbsNode> GetNodesWithStorePicker(int storeId)
		{
			return Enumerable.Empty<UwbsNode>();
		}
	}
}
