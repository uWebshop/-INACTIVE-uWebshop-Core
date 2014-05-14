using System;
using System.Collections.Generic;
using System.Linq;
using Examine;
using uWebshop.Domain;
using uWebshop.Domain.BaseClasses;
using uWebshop.Domain.Interfaces;
using umbraco.NodeFactory;

namespace uWebshop.Umbraco.Repositories
{
	internal abstract class UmbracoMultiStoreEntityRepository<T1, T2> : IEntityRepository<T1>
		where T2 : uWebshopEntity, T1
		where T1 : class
	{
		public T1 GetById(int id, ILocalization localization)
		{
			return CreateEntityFromNode(new Node(id), localization);
		}

		public List<T1> GetAll(ILocalization localization)
		{
			//Log.Instance.LogDebug("Reloading all entities of type "+ TypeAlias + " for store "+storeAlias);

			var examineResults = UmbracoStaticCachedEntityRepository.GetExamineResultsForNodeTypeAlias(TypeAlias);
			if (examineResults != null && examineResults.Any()) return examineResults.Select(e => CreateEntityFromExamineData(e, localization)).ToList();
			return UmbracoStaticCachedEntityRepository.GetNodeIdsFromXMLStoreForNodeTypeAlias(TypeAlias).Select(id => CreateEntityFromNode(new Node(id), localization)).Where(e => e != null).ToList();
		}

		private T1 CreateEntityFromExamineData(SearchResult examineNode, ILocalization localization)
		{
			if (examineNode == null) throw new Exception("Trying to load data from null examine SearchResult");

			if (!examineNode.Fields.ContainsKey("id") || string.IsNullOrEmpty(examineNode.Fields["id"]))
				throw new Exception("Trying to load data from null examine SearchResult without id field. Actual fields: {" + string.Join(", ", examineNode.Fields.Select(a => a.Key + " = " + a.Value)) + "}");

			var entity = Activator.CreateInstance<T2>();
			var fields = new DictionaryPropertyProvider(examineNode);
			entity.NodeTypeAlias = TypeAlias;
			entity.LoadFieldsFromExamine(fields);
			LoadDataFromPropertiesDictionary(entity, fields, localization);
			return entity;
		}

		internal T1 CreateEntityFromNode(Node node, ILocalization localization)
		{
			if (node == null) throw new Exception("Trying to load data from null node");
			if (node.NodeTypeAlias == null || (node.NodeTypeAlias != TypeAlias && !node.NodeTypeAlias.StartsWith(TypeAlias))) return null;

			var entity = Activator.CreateInstance<T2>();
			LoadDataFromNode(entity, node, localization);
			if (UmbracoStaticCachedEntityRepository.CheckNodeTypeAliasForImproperOverlap(entity.NodeTypeAlias, TypeAlias))
			{
				return entity;
			}
			return null;
		}

		internal void LoadDataFromNode(T2 entity, Node node, ILocalization localization)
		{
			Helpers.LoadUwebshopEntityPropertiesFromNode(entity, node);
			LoadDataFromPropertiesDictionary(entity, new UmbracoNodePropertyProvider(node), localization);
		}

		public void ReloadData(T1 entity, ILocalization localization)
		{
			var e = entity as T2; // todo: this will break
			LoadDataFromNode(e, new Node(e.Id), localization);
		}

		public abstract void LoadDataFromPropertiesDictionary(T2 entity, IPropertyProvider fields, ILocalization localization);
		public abstract string TypeAlias { get; }
	}
}