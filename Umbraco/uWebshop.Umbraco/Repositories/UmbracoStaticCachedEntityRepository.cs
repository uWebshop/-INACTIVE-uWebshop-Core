using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Xml.XPath;
using Examine;
using Examine.LuceneEngine.SearchCriteria;
using Examine.SearchCriteria;
using Lucene.Net.Documents;
using uWebshop.Common;
using uWebshop.Domain;
using uWebshop.Domain.Interfaces;
using uWebshop.Domain.Helpers;
using uWebshop.Umbraco.Businesslogic;
using umbraco;
using umbraco.NodeFactory;
using Umbraco.Web;

namespace uWebshop.Umbraco.Repositories
{
	internal class UmbracoStaticCachedEntityRepository : ICMSEntityRepository
	{
		// todo: urgent, remove static or caching will bug on uWebshop reboot
		// todo: kunnen we het static gedeelte non-static maken? (link met ApplicationCacheManager moet dan gemaakt worden op een of andere manier)
		private static List<UwbsNode> _allUwbsNodes; // => per request cache gives 20ms overhead over static cache
		private static readonly ConcurrentDictionary<CacheKey, object> _getObjectsByAliasCache = new ConcurrentDictionary<CacheKey, object>();

		internal static void ResetStaticCache()
		{
			_getObjectsByAliasCache.Clear();
		}

		internal static void ResetEntityCache()
		{
			_allUwbsNodes = null;
		}

		public UwbsNode GetByGlobalId(int globalId)
		{
			var uwbsNode = GetAll().FirstOrDefault(node => node.Id == globalId);
			uwbsNode = uwbsNode ?? LoadUwbsNodeFromNode(globalId); // fallback = slow! (nodefactory)
			return uwbsNode.Path != null ? uwbsNode : null;
		}

		private static UwbsNode LoadUwbsNodeFromNode(int id)
		{
            var umbracoHelper = new UmbracoHelper(UmbracoContext.Current);
		    var node = umbracoHelper.TypedContent(id);
			var n = new UwbsNode();
			n.Path = node.Path;
			n.NodeTypeAlias = node.DocumentTypeAlias;
			if (node.Name != null && node.Parent != null)
				n.ParentId = node.Parent.Id;
			n.Id = node.Id;
			n.UrlName = node.UrlName ?? "";
			n.SortOrder = node.SortOrder;
			n.Level = node.Level;
			return n;
		}

		private static UwbsNode LoadUwbsNodeFromLuceneDocument(Document examineNode)
		{
			var node = new UwbsNode();
			if (examineNode.GetField("id") != null)
			{
				string value = examineNode.GetField("id").StringValue();
				int id;
				if (int.TryParse(value, out id))
					node.Id = id;
			}
			if (examineNode.GetField("parentID") != null)
			{
				string value = examineNode.GetField("parentID").StringValue();
				int id;
				if (int.TryParse(value, out id))
					node.ParentId = id;
			}
			if (examineNode.GetField("nodeTypeAlias") != null)
			{
				node.NodeTypeAlias = examineNode.GetField("nodeTypeAlias").StringValue();
			}
			if (examineNode.GetField("path") != null)
			{
				node.Path = examineNode.GetField("path").StringValue();
			}
			if (examineNode.GetField("sortOrder") != null)
			{
				string value = examineNode.GetField("sortOrder").StringValue();
				int id;
				if (int.TryParse(value, out id))
					node.SortOrder = id;
			}
			if (examineNode.GetField("level") != null)
			{
				string value = examineNode.GetField("level").StringValue();
				int id;
				if (int.TryParse(value, out id))
					node.Level = id;
			}
			if (examineNode.GetField("urlName") != null)
			{
				node.UrlName = examineNode.GetField("urlName").StringValue();
			}
			return node;
		}

		public IEnumerable<UwbsNode> GetAll()
		{
			if (_allUwbsNodes == null)
			{
				Log.Instance.LogDebug("Reloading basic info of all umbraco nodes");

				var examineResults = InternalHelpers.GetSearchResults("nodeTypeAlias:uwbs*");
				if (examineResults == null) return Enumerable.Empty<UwbsNode>();
				_allUwbsNodes = examineResults.Select(LoadUwbsNodeFromLuceneDocument).ToList();
			}
			return _allUwbsNodes;
		}

		public static IEnumerable<SearchResult> GetExamineResultsForNodeTypeAlias(string nodeTypeAlias)
		{
			try
			{
				var examineprovider = ExamineManager.Instance.SearchProviderCollection[UwebshopConfiguration.Current.ExamineSearcher];
				if (examineprovider != null)
				{
					var searcher = examineprovider.CreateSearchCriteria();
					if (searcher != null)
					{
						IBooleanOperation query = searcher.Field("__NodeTypeAlias", nodeTypeAlias.ToLower().MultipleCharacterWildcard());

						//Log.Instance.LogDebug(DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt") + " EXAMINE GetObjectsByAlias<T> " + typeof(T).Name);
						var compiledQuery = query.Compile();
						var searchResults = examineprovider.Search(compiledQuery);
						return searchResults.Where(examineNode => examineNode.Fields["__NodeId"] != null).Where(examineNode => CheckNodeTypeAliasForImproperOverlap(examineNode.Fields["__NodeTypeAlias"], nodeTypeAlias));
					}
				}
			}
			catch (Exception ex)
			{
				Log.Instance.LogError(ex, "Error while searching Examine for type with alias " + nodeTypeAlias);
			}

			return null;
		}

		public IEnumerable<T> GetObjectsByAlias<T>(string nodeTypeAlias, ILocalization localization = null, int startNodeId = 0)
		{
			var cacheKey = CreateCacheKey(nodeTypeAlias, localization);

			return (IEnumerable<T>) _getObjectsByAliasCache.GetOrAdd(cacheKey, key => QueryEntitiesByAlias<T>(key, startNodeId));
		}

		public IEnumerable<T> GetObjectsByAliasUncached<T>(string nodeTypeAlias, ILocalization localization = null, int startNodeId = 0)
		{
			var cacheKey = CreateCacheKey(nodeTypeAlias, localization);

			return QueryEntitiesByAlias<T>(cacheKey, startNodeId);
		}

		private static CacheKey CreateCacheKey(string nodeTypeAlias, ILocalization localization)
		{
			if (nodeTypeAlias == Order.NodeAlias) throw new Exception("Orders are not stored in Umbraco nodes");

			localization = localization ?? StoreHelper.CurrentLocalization;
			var localizationAlias = localization.StoreAlias + localization.CurrencyCode;
			if (localization == null)
			{
				Log.Instance.LogWarning("Used a generic fallback to load entities of type " + nodeTypeAlias + " this is most likely due to an error and might effect your shop");
				localizationAlias = Constants.NonMultiStoreAlias;
			}
			return new CacheKey(nodeTypeAlias, localizationAlias);
		}

		private IEnumerable<T> QueryEntitiesByAlias<T>(CacheKey cacheKey, int startNodeId = 0)
		{
			var nodeTypeAlias = cacheKey.Alias;
			var fallbackConstructorInfo = typeof (T).GetConstructor(new[] {typeof (int)});
			if (fallbackConstructorInfo == null)
			{
				throw new Exception("No constructor for type " + nodeTypeAlias);
			}

			var constructorInfo = typeof (T).GetConstructor(new[] {typeof (SearchResult)});

			try
			{
				var examineResults = GetExamineResultsForNodeTypeAlias(nodeTypeAlias);
				if (examineResults != null && examineResults.Any())
					return examineResults.Select(examineNode => constructorInfo != null ? (T) constructorInfo.Invoke(new object[] {examineNode}) : (T) fallbackConstructorInfo.Invoke(new object[] {Int32.Parse(examineNode.Fields["__NodeId"])})).ToList();
			}
			finally
			{
			}

			// if no examine index found, fallback to umbraco.config
			Log.Instance.LogWarning(DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt") + " warning! Examine index failed when loading type " + typeof (T).Name);

			return GetNodeIdsFromXMLStoreForNodeTypeAlias(nodeTypeAlias, startNodeId).Select(id => (T) fallbackConstructorInfo.Invoke(new object[] {id}));
		}

		public UwbsNode GetNodeWithStorePicker(int storeId)
		{
			return GetNodesWithStorePicker(storeId).OrderBy(n => n.SortOrder).FirstOrDefault();
		}

		public IEnumerable<UwbsNode> GetNodesWithStorePicker(int storeId)
		{
			var nodes = InternalHelpers.GetSearchResults("uwbsStorePicker:[0* TO 9*]").Where(x => x.GetField("uwbsStorePicker") != null 
				&& x.GetField("uwbsStorePicker").StringValue() == storeId.ToString()).Select(LoadUwbsNodeFromLuceneDocument);
			if (nodes.Any()) return nodes;

			var nodesFromXml = new List<UwbsNode>();
			// xmlstore/node factory fallback
			var it = library.GetXmlNodeByXPath("//uwbsStorePicker[text() = '" + storeId + "']/parent::*");
			while (it.MoveNext())
			{
				var intval = it.Current.GetAttribute("id", it.Current.NamespaceURI);
				nodesFromXml.Add(LoadUwbsNodeFromNode(int.Parse(intval))); // possible todo: make a constructor that can work with the XML itself (to work without UmbracoContext)
			}

			return nodesFromXml;
		}
		
		public static IEnumerable<int> GetNodeIdsFromXMLStoreForNodeTypeAlias(string nodeTypeAlias, int startNodeId = 0)
		{
            var umbracoHelper = new UmbracoHelper(UmbracoContext.Current);

			var start = startNodeId > 0 ? string.Format("//{0}[@id = {1}]", umbracoHelper.TypedContent(startNodeId).DocumentTypeAlias, startNodeId) : string.Empty;
			XPathNodeIterator it = library.GetXmlNodeByXPath(string.Format(start + "//*[starts-with(name(),'{0}')]", nodeTypeAlias));
			var objects = new List<int>();
			while (it.MoveNext())
			{
				if (it.Current == null) continue;
				if (CheckNodeTypeAliasForImproperOverlap(it.Current.Name, nodeTypeAlias))
				{
					var intval = it.Current.GetAttribute("id", it.Current.NamespaceURI);
					if (!string.IsNullOrWhiteSpace(intval))
						objects.Add(Convert.ToInt32(intval));
				}
			}
			return objects;
		}

		internal static bool CheckNodeTypeAliasForImproperOverlap(string foundAlias, string requestedNodeTypeAlias)
		{
			foundAlias = foundAlias.ToLowerInvariant();
			requestedNodeTypeAlias = requestedNodeTypeAlias.ToLowerInvariant();
			return CheckRepositories(foundAlias, requestedNodeTypeAlias) && CheckProduct(foundAlias, requestedNodeTypeAlias) && CheckPaymentProvider(foundAlias, requestedNodeTypeAlias) && CheckShippingProvider(foundAlias, requestedNodeTypeAlias) && CheckOrderDiscount(foundAlias, requestedNodeTypeAlias) && CheckProductDiscount(foundAlias, requestedNodeTypeAlias) && CheckEmailCustomer(foundAlias, requestedNodeTypeAlias) && CheckEmailStore(foundAlias, requestedNodeTypeAlias) && CheckStore(foundAlias, requestedNodeTypeAlias);
		}

		internal static bool CheckRepositories(string foundAlias, string requestedNodeTypeAlias)
		{
			return foundAlias != requestedNodeTypeAlias + "repository";
		}

		internal static bool CheckProduct(string foundAlias, string requestedNodeTypeAlias)
		{
			return requestedNodeTypeAlias != Product.NodeAlias.ToLowerInvariant() || !foundAlias.StartsWith(ProductVariant.NodeAlias.ToLowerInvariant());
		}

		internal static bool CheckStore(string foundAlias, string requestedNodeTypeAlias)
		{
			return requestedNodeTypeAlias != Store.NodeAlias.ToLowerInvariant() || !foundAlias.StartsWith(Store.StoreRepositoryNodeAlias.ToLowerInvariant());
		}

		internal static bool CheckOrderDiscount(string foundAlias, string requestedNodeTypeAlias)
		{
			return requestedNodeTypeAlias != DiscountOrder.NodeAlias.ToLowerInvariant() || !foundAlias.StartsWith(DiscountOrder.SectionNodeAlias.ToLowerInvariant());
		}

		internal static bool CheckProductDiscount(string foundAlias, string requestedNodeTypeAlias)
		{
			return requestedNodeTypeAlias != DiscountProduct.NodeAlias.ToLowerInvariant() || !foundAlias.StartsWith(DiscountProduct.SectionNodeAlias.ToLowerInvariant());
		}

		internal static bool CheckEmailCustomer(string foundAlias, string requestedNodeTypeAlias)
		{
			return requestedNodeTypeAlias != Email.EmailTemplateCustomerNodeAlias.ToLowerInvariant() || !foundAlias.StartsWith(Email.EmailTemplateCustomerSectionNodeAlias.ToLowerInvariant());
		}

		internal static bool CheckEmailStore(string foundAlias, string requestedNodeTypeAlias)
		{
			return requestedNodeTypeAlias != Email.EmailTemplateStoreNodeAlias.ToLowerInvariant() || !foundAlias.StartsWith(Email.EmailTemplateStoreSectionNodeAlias.ToLowerInvariant());
		}

		internal static bool CheckPaymentProvider(string foundAlias, string requestedNodeTypeAlias)
		{
			return requestedNodeTypeAlias != PaymentProvider.NodeAlias.ToLowerInvariant() || (!foundAlias.StartsWith(PaymentProviderMethod.NodeAlias.ToLowerInvariant()) && !foundAlias.StartsWith(PaymentProvider.PaymentProviderSectionNodeAlias.ToLowerInvariant()) && !foundAlias.StartsWith(Zone.PaymentZoneNodeAlias.ToLowerInvariant()) && !foundAlias.StartsWith(PaymentProvider.PaymentProviderZoneSectionNodeAlias.ToLowerInvariant()));
		}

		internal static bool CheckShippingProvider(string foundAlias, string requestedNodeTypeAlias)
		{
			return requestedNodeTypeAlias != ShippingProvider.NodeAlias.ToLowerInvariant() || (!foundAlias.StartsWith(ShippingProviderMethod.NodeAlias.ToLowerInvariant()) && !foundAlias.StartsWith(ShippingProvider.ShippingProviderSectionNodeAlias.ToLowerInvariant()) && !foundAlias.StartsWith(Zone.ShippingZoneNodeAlias.ToLowerInvariant()) && !foundAlias.StartsWith(ShippingProvider.ShippingProviderZoneSectionNodeAlias.ToLowerInvariant()));
		}

		private class CacheKey
		{
			public readonly string Alias;
			public readonly string CurrentStore;

			public CacheKey(string alias, string currentStore)
			{
				Alias = alias;
				CurrentStore = currentStore;
			}

			public override bool Equals(object obj)
			{
				if (obj.GetType() != this.GetType()) return false;
				var other = (CacheKey) obj;
				return Alias == other.Alias && CurrentStore == other.CurrentStore;
			}

			public override int GetHashCode()
			{
				return Alias.GetHashCode() + CurrentStore.GetHashCode();
			}
		}
	}
}