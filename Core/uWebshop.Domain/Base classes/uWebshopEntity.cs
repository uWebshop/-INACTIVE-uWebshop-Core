using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Web;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Domain.BaseClasses
{
    /// <summary>
    /// Class based on the Umbraco Node-class to inherit from
    /// </summary>
    [DataContract(Name = "Node", Namespace = "", IsReference = true)]
    public class uWebshopEntity : IUwebshopUmbracoEntity
    {
        private DateTime? _createDate;

        private int _nodeId;
        private Guid _key;

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
        /// Initializes a new instance of the uWebshop.Domain.NodeBase class
        /// </summary>
        /// <param name="id">NodeId of the node</param>
        public uWebshopEntity(int id)
        {
            _nodeId = id;
        }

        /// <summary>
        /// Gets the node.
        /// </summary>
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
        /// Gets the id of the node
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

        /// <summary>
        /// Gets the uniqueId of the node
        /// </summary>
        [DataMember(Name = "key")]
        public Guid Key
        {
            get
            {
                return _key == null || _key == Guid.Empty ? _nodeId != 0 ? Node.Key : Guid.Empty : _key;
            }
            set { _key = value; }
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

        /// <summary>
        /// Gets the type alias of the node
        /// </summary>
        [DataMember(Name = "nodeTypeAlias")]
        public string NodeTypeAlias
        {
            get { return _nodeTypeAlias ?? (_nodeTypeAlias = Node.NodeTypeAlias); } // StoreHelper.GetMultiStoreItem(Id, "nodeTypeAlias")); }
            set { _nodeTypeAlias = value; }
        }

        /// <summary>
        /// Gets the name of the node
        /// </summary>
        [DataMember(Name = "nodeName")]
        public string Name
        {
            get { return HttpContext.Current != null ? HttpContext.Current.Server.HtmlEncode(_nodeName ?? (_nodeName = Node.Name)) : _nodeName; }
            set { _nodeName = value; }
        }

        /// <summary>
        /// Gets the id of the parent of the node
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
            get { return _path ?? (_path = Node.Path); } //StoreHelper.GetMultiStoreItem(Id, "path")); => todo: preferred, but we can't use multistore at this level
            set { _path = value; }
        }

        /// <summary>
        /// Gets a System.DateTime object that is set to the date and time when the node is created
        /// </summary>
        [DataMember(Name = "createDate")]
        public DateTime CreateDate
        {
            get { return _createDate ?? (_createDate = Node.CreateDate).GetValueOrDefault(); }
            set { _createDate = value; }
        }

        /// <summary>
        /// Gets a System.DateTime object that is set to the date and time when the node is updated
        /// </summary>
        [DataMember(Name = "updateDate")]
        public DateTime UpdateDate
        {
            get { return _updateDate ?? (_updateDate = Node.UpdateDate).GetValueOrDefault(); }
            set { _updateDate = value; }
        }

        /// <summary>
        /// SortOrder for the node
        /// </summary>
        [DataMember]
        public int SortOrder
        {
            get
            {
                if (_sortOrder.HasValue) return _sortOrder.GetValueOrDefault();
                //int sortOrder;
                return //int.TryParse(StoreHelper.GetMultiStoreItem(Id, "sortOrder"), out sortOrder) ? sortOrder :  => todo: preferred, but we can't use multistore at this level
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

        internal virtual void LoadFieldsFromExamine(IPropertyProvider fields)
        {
            // NB: we can't use multi store properties here, since Store itself is loaded using this method
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
                var value = fields.GetStringValue("sortOrder");
                int sortOrder;
                if (Int32.TryParse(value, out sortOrder))
                    _sortOrder = sortOrder;
            }

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