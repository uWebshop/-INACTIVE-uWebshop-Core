using Umbraco.Core;
using Umbraco.Core.Models;
using uWebshop.Umbraco.Interfaces;

namespace uWebshop.Umbraco.Services
{
	internal class CMSChangeContentService : ICMSChangeContentService
	{
		public ICMSContent GetById(int nodeId)
		{
			return new CMSContent(nodeId);
		}

		private class CMSContent : ICMSContent
		{
			public CMSContent(int id)
			{
				_content = ApplicationContext.Current.Services.ContentService.GetById(id);
			}

			private readonly IContent _content;

			public void SetValue(string propertyAlias, string value)
			{
				_content.SetValue(propertyAlias, value);
			}

			public void SaveAndPublish()
			{
				ApplicationContext.Current.Services.ContentService.SaveAndPublish(_content);
			}

			public string ContentTypeAlias
			{
				get { return _content.ContentType.Alias; }
			}

			public bool HasProperty(string propertyAlias)
			{
				return _content.HasProperty(propertyAlias);
			}
		}
	}
}