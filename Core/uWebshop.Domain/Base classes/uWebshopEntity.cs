using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Web;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Domain.BaseClasses
{
	/// <summary>
	///     Class based on the Umbraco Node-class to inherit from
	/// </summary>
	[DataContract(Name = "Node", Namespace = "", IsReference = true)]
	public class uWebshopEntity : IUwebshopUmbracoEntity
	{
		private DateTime? _createDate;

		private int _nodeId;

		/// <summary>
		/// The _node name
		/// </summary>
		protected string _nodeName;

		private string _nodeTypeAlias;
		private int? _parentId;
		private string _path;
		private int? _sortOrder;
		private DateTime? _updateDate;
		private string _urlName;

		/// <summary>
		/// Initializes a new instance of the <see cref="uWebshopEntity"/> class.
		/// </summary>
		public uWebshopEntity()
		{
		}

		/// <summary>
		///     Initializes a new instance of the uWebshop.Domain.NodeBase class
		/// </summary>
		/// <param name="id">NodeId of the node</param>
		public uWebshopEntity(int id)
		{
			_nodeId = id;
		}

		/// <summary>
		/// Gets the node.
		/// </summary>
		/// <value> The node.
		/// </value>
		/// <exception cref="System.ArgumentException">Request for Node but no nodeId</exception>
		[IgnoreDataMember]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[Obsolete("Use GetProperty if you need custom properties")]
		public IUwebshopReadonlyContent Node
		{
			get
			{
				if (_nodeId == 0) throw new ArgumentException("Request for Node but no nodeId");
				return IO.Container.Resolve<ICMSContentService>().GetReadonlyById(Id);
			}
		}

		/// <summary>
		///     Gets the id of the node
		/// </summary>
		[DataMember(Name = "id")]
		public int Id
		{
			get
			{
				return _nodeId; // todo: not sure if this is (always) correct!
			}
			set { _nodeId = value; }
		}

		public string TypeAlias
		{
			get { return NodeTypeAlias; }
			internal set { NodeTypeAlias = value; }
		}
		
		/// <summary>
		/// Gets a value indicating whether the entity is disabled.
		/// </summary>
		/// <value>
		///   <c>true</c> if disabled; otherwise, <c>false</c>.
		/// </value>
		public virtual bool Disabled { get; set; }
		//{
		//	get { return false; }
		//	set { }
		//}

		/// <summary>
		///     Gets the type alias of the node
		/// </summary>
		[DataMember(Name = "nodeTypeAlias")]
		public string NodeTypeAlias
		{
			get { return _nodeTypeAlias ?? (_nodeTypeAlias = Node.NodeTypeAlias); } // StoreHelper.GetMultiStoreItem(Id, "nodeTypeAlias")); }
			set { _nodeTypeAlias = value; }
		}

		/// <summary>
		///     Gets the name of the node
		/// </summary>
		[DataMember(Name = "nodeName")]
		public string Name
		{
			get { return HttpContext.Current != null ? HttpContext.Current.Server.HtmlEncode(_nodeName ?? (_nodeName = Node.Name)) : _nodeName; }
			set { _nodeName = value; }
		}

		/// <summary>
		///     Gets the id of the parent of the node
		/// </summary>
		[DataMember(Name = "parentID")]
		public int ParentId
		{
			get
			{
				if (_parentId.HasValue) return _parentId.GetValueOrDefault();

				var parent = IO.Container.Resolve<ICMSContentService>().GetReadonlyById(Id).Parent;
				if (parent != null)
					_parentId = parent.Id;
				return _parentId.GetValueOrDefault();

				//Log.Instance.LogDebug(DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt") + " using Fallback to find ParentID, Examine failed!");
				//SearchResult examineNode = StoreHelper.GetNodeFromExamine(Id, "NodeBase.....ParentId....");
				//if (examineNode != null && examineNode.Fields.ContainsKey("parentID"))
				//{
				//	string value = examineNode.Fields["parentID"];
				//	int id;
				//	if (Int32.TryParse(value, out id))
				//		return id;
				//}

				////Log.Instance.LogDebug("ParentId 1: " + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"));
				//var parent = Node.Parent;
				////Log.Instance.LogDebug("ParentId 2: " + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"));
				//bool parentNotNull = parent != null;
				////Log.Instance.LogDebug("ParentId 3: " + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"));

				//if (parentNotNull)
				//{
				//	int result = parent.Id;
				//	_parentId = result;
				//	//Log.Instance.LogDebug("ParentId 4: " + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"));
				//	return result;
				//}

				//var n = Node;
				//if (n.Parent != null)
				//{
				//	_parentId = n.Parent.Id;
				//	return n.Parent.Id;
				//}

				//return -1;
			}
			set { _parentId = value; }
		}

		/// <summary>
		/// Gets or sets the path.
		/// </summary>
		/// <value>
		/// The path.
		/// </value>
		[DataMember(Name = "path")]
		public string Path
		{
			get { return _path ?? (_path = Node.Path); } //StoreHelper.GetMultiStoreItem(Id, "path"));
			set { _path = value; }
		}

		/// <summary>
		///     Gets a System.DateTime object that is set to the date and time when the node is created
		/// </summary>
		[DataMember(Name = "createDate")]
		public DateTime CreateDate
		{
			get { return _createDate ?? (_createDate = Node.CreateDate).GetValueOrDefault(); }
			set { _createDate = value; }
		}

		/// <summary>
		///     Gets a System.DateTime object that is set to the date and time when the node is updated
		/// </summary>
		[DataMember(Name = "updateDate")]
		public DateTime UpdateDate
		{
			get { return _updateDate ?? (_updateDate = Node.UpdateDate).GetValueOrDefault(); }
			set { _updateDate = value; }
		}

		/// <summary>
		///     SortOrder for the node
		/// </summary>
		[DataMember]
		public int SortOrder
		{
			get
			{
				if (_sortOrder.HasValue) return _sortOrder.GetValueOrDefault();
				//int sortOrder;
				return //int.TryParse(StoreHelper.GetMultiStoreItem(Id, "sortOrder"), out sortOrder) ? sortOrder : 
					Node.SortOrder; // Node fallback
			}
			set { _sortOrder = value; }
		}

		/// <summary>
		/// Gets or sets the name of the URL.
		/// </summary>
		/// <value>
		/// The name of the URL.
		/// </value>
		[DataMember]
		public string UrlName
		{
			get { return _urlName ?? Node.UrlName; }
			set { _urlName = value; }
		}

		///// <summary>
		///// Loads the data from examine.
		///// </summary>
		///// <param name="storeAlias">The store alias.</param>
		///// <param name="examineNode">The examine node.</param>
		///// <returns></returns>
		//protected virtual bool LoadDataFromExamine(string storeAlias, SearchResult examineNode = null)
		//{
		//	//Log.Instance.LogDebug( DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt") + "LoadDataFromExamine::NodeBase start"); 
		//	bool logAtEnd = false;
		//	if (examineNode == null)
		//	{
		//		logAtEnd = true;
		//		examineNode = StoreHelper.GetNodeFromExamine(_nodeId);
		//	}
		//	if (examineNode == null)
		//	{
		//		return false;
		//	}

		//	LoadFieldsFromExamine(new DictionaryPropertyProvider(examineNode));

		//	//if (logAtEnd)
		//	//    Log.Instance.LogDebug(DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt") + "END LoadDataFromExamine::NodeBase typealias:" + _nodeTypeAlias);
		//	return true;
		//}

		internal virtual void LoadFieldsFromExamine(IPropertyProvider fields)
		{
			string stringValue = null;
			if (_nodeId == 0 && fields.UpdateValueIfPropertyPresent("id", ref stringValue))
			{
				int.TryParse(stringValue, out _nodeId);
			}

			if (fields.UpdateValueIfPropertyPresent("parentID", ref stringValue))
			{
				int intVal;
				if (int.TryParse(stringValue, out intVal))
					_parentId = intVal;
			}

			if (fields.ContainsKey("sortOrder"))
			{
				string value = fields.GetStringValue("sortOrder");
				int sortOrder;
				if (Int32.TryParse(value, out sortOrder))
					_sortOrder = sortOrder;
			}
			//if (_nodeId == 0)
			//	_nodeId = GetIntValue("id", examineNode).GetValueOrDefault(); can't use multistore here!!!! -> vanwege Store (will cause endless loop)
			//_parentId = GetIntValue("parentID", examineNode);  can't use multistore here!!!! -> vanwege Store
			//_sortOrder = GetIntValue("sortOrder", examineNode); can't use multistore here!!!! -> vanwege Store

			if (fields.ContainsKey("nodeTypeAlias"))
			{
				_nodeTypeAlias = fields.GetStringValue("nodeTypeAlias");
			}
			if (fields.ContainsKey("nodeName"))
			{
				_nodeName = fields.GetStringValue("nodeName");
				if (_nodeName == string.Empty) throw new Exception("Node with empty name");
			}
			if (fields.ContainsKey("path"))
			{
				_path = fields.GetStringValue("path");
			}
			if (fields.ContainsKey("createDate"))
			{
				string value = fields.GetStringValue("createDate");
				DateTime date;
				if (DateTime.TryParse(value, out date)) // todo: use the right culture!
					_createDate = date;
			}
			if (fields.ContainsKey("updateDate"))
			{
				string value = fields.GetStringValue("updateDate");
				DateTime date;
				if (DateTime.TryParse(value, out date)) // todo: use the right culture!
					_updateDate = date;
			}
			if (fields.ContainsKey("urlName"))
			{
				_urlName = fields.GetStringValue("urlName");
			}
		}
	}
}