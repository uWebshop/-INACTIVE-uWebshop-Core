using System;
using System.Collections.Generic;
using System.Linq;
using uWebshop.Domain;
using uWebshop.Domain.Interfaces;
using uWebshop.Umbraco.Businesslogic;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Web;
using File = uWebshop.Domain.File;
using Log = uWebshop.Domain.Log;

namespace uWebshop.Umbraco.Services
{
	internal class CMSContentService : ICMSContentService
	{
		public IUwebshopReadonlyContent GetReadonlyById(int id)
		{
			return new NodeBasedContent(id);
		}

		public IEnumerable<IUwebshopReadonlyContent> GetAllRootNodes()
		{
            // todo: test
            var contentService = new UmbracoHelper(UmbracoContext.Current);
		    return contentService.TypedContentAtRoot().Select(n => new NodeBasedContent(n.Id));
		}
        
		public IUwebshopContent GetById(int id)
		{
			return new DocumentBasedContent(id);
		}

		public File GetFileById(int id)
		{
			return InternalHelpers.LoadFileWithId(id);
		}

		public Image GetImageById(int id)
		{
			return InternalHelpers.LoadImageWithId(id);
		}
		
		private class DocumentBasedContent : IUwebshopContent
		{
			private readonly IContent _document;

			public DocumentBasedContent(int id)
				: this(ApplicationContext.Current.Services.ContentService.GetById(id))
			{
			}

			public DocumentBasedContent(IContent document)
			{
				_document = document;
			}

			public string Path
			{
				get { return _document.Path; }
			}

			public DateTime CreateDate
			{
				get { return _document.CreateDate; }
			}

			public DateTime UpdateDate
			{
				get { return _document.UpdateDate; }
			}

			public int SortOrder
			{
				get { return _document.SortOrder; }
			}

			public string UrlName
			{
				get { return _document.Name; }
			}

			IUwebshopReadonlyContent IUwebshopReadonlyContent.Parent
			{
				get { return new NodeBasedContent(_document.ParentId); }
			}

			public IUwebshopContent Parent
			{
				get { return new DocumentBasedContent(_document.ParentId); }
			}

			public int Id
			{
				get { return _document.Id; }
			}
            public Guid Key
            {
                get { return _document.Key; }
            }

            public string NodeTypeAlias
			{
				get { return _document.ContentType.Alias; }
			}

			public string Name
			{
				get { return _document.Name; }
			}

			public int template
			{
				get
				{
					return _document.Template != null ? _document.Template.Id : 0;
				}
			}

			public ICMSProperty GetProperty(string propertyAlias)
			{
				return new DocProperty(_document.Properties.FirstOrDefault(x => x.Alias == propertyAlias));
			}

			public ICMSProperty GetMultiStoreItem(string propertyAlias)
			{
				return new DocProperty(_document.GetMultiStoreItem(propertyAlias));
			}

			public string Url
			{
				get { throw new NotImplementedException(); }
			}

			public List<IUwebshopReadonlyContent> ChildrenAsList
			{
				get { throw new NotImplementedException(); }
			}

			public T1 GetProperty<T1>(string propertyAlias)
			{
				return _document.GetValue<T1>(propertyAlias);
			}

			public void SetProperty(string propertyAlias, object value)
			{
				_document.SetValue(propertyAlias, value);
			}

			public void Publish(int userId = 0)
			{
				var contentService = ApplicationContext.Current.Services.ContentService;
				contentService.Publish(_document);
			}

			public ICMSProperty getProperty(string propertyAlias)
			{
				return new DocProperty(_document.Properties.FirstOrDefault(x => x.Alias == propertyAlias));
			}
		}

		private class DocProperty : ICMSProperty
		{
			private readonly global::Umbraco.Core.Models.Property _property;

			public DocProperty(global::Umbraco.Core.Models.Property property)
			{
				this._property = property;
			}

			public string Value
			{
				get
				{
					if (_property != null && _property.Value != null)
						return _property.Value.ToString();
					return string.Empty;
				}
			}
		}

		private class NodeProperty : ICMSProperty
		{
			private readonly IPublishedProperty _property;

			public NodeProperty(IPublishedProperty property)
			{
				this._property = property;
			}

			public string Value
			{
				get
				{
					return _property != null ? _property.Value.ToString() : string.Empty;
				}
			}
		}

		private class NodeBasedContent : IUwebshopReadonlyContent
		{
			private readonly IPublishedContent _node;

			public NodeBasedContent(int id)
			{
				var umbHelper = new UmbracoHelper(UmbracoContext.Current);
				var node = umbHelper.TypedContent(id);

				if (node != null)
				{
					_node = node;
				}
				else
				{
					Log.Instance.LogError("NodeBasedContent - IPublishedContent with Id: " + id + " was null or is not correctly in the cache (republish)");
				}
			}

			private NodeBasedContent(IPublishedContent node)
			{
				if (node != null)
				{
					_node = node;
				}
				else
				{
					Log.Instance.LogError("NodeBasedContent - IPublishedContent: node was null");
				}
			}

			public string Path
			{
				get { return _node.Path; }
			}

			public DateTime CreateDate
			{
				get { return _node.CreateDate; }
			}

			public DateTime UpdateDate
			{
				get { return _node.UpdateDate; }
			}

			public int SortOrder
			{
				get { return _node.SortOrder; }
			}

			public string UrlName
			{
				get { if (_node != null) return _node.UrlName;
					return string.Empty;
				}
			}

			public IUwebshopReadonlyContent Parent
			{
				get
				{
					if (_node.Parent == null) return null;
					return new NodeBasedContent(_node.Parent.Id);
				}
			}

			public int Id
			{
				get { return _node.Id; }
			}
            public Guid Key
            {
                get { return _node.GetKey(); }
            }

            public string NodeTypeAlias
			{
				get { return _node.DocumentTypeAlias; }
			}

			public string Name
			{
				get { return _node.Name; }
			}

			public int template
			{
				get { return _node.TemplateId; }
			}

			public ICMSProperty GetProperty(string propertyAlias)
			{
				return new NodeProperty(_node.Properties.FirstOrDefault(x => x.PropertyTypeAlias == propertyAlias));
			}

			public ICMSProperty GetMultiStoreItem(string propertyAlias)
			{
				return new NodeProperty(_node.GetMultiStoreItem(propertyAlias));
				
			}

			public string Url
			{
				get { return _node.Url; }
			}

			public List<IUwebshopReadonlyContent> ChildrenAsList
			{
				get { return _node.Children.Select(n => (IUwebshopReadonlyContent) new NodeBasedContent(n)).ToList(); }
			}
		}
	}
}
