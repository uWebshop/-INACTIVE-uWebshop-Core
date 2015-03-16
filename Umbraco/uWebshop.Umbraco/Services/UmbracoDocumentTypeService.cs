using System.Collections.Generic;
using System.Linq;
using uWebshop.Domain.Businesslogic;
using uWebshop.Domain.Interfaces;
using umbraco.cms.businesslogic.web;
using Umbraco.Core;
using Umbraco.Core.Models;

namespace uWebshop.Umbraco.Services
{
	internal class UmbracoDocumentTypeService : ICMSDocumentTypeService
	{
		public IDocumentTypeInfo GetByAlias(string alias)
		{
			var ctService = ApplicationContext.Current.Services.ContentTypeService;

			var documentType = ctService.GetContentType(alias);
			return documentType != null ? new UmbracoDocument(documentType) : null;
		}
	}

	public class UmbracoDocument : IDocumentTypeInfo
	{
		private readonly IContentType _contentType;

		public UmbracoDocument(IContentType contentType)
		{
			_contentType = contentType;
		}

		public IEnumerable<IDocumentProperty> Properties
		{
			get
			{
				if (_contentType != null && _contentType.PropertyTypes != null)
					return _contentType.PropertyTypes.Select(property => new DocumentProperty {Alias = property.Alias, Name = property.Name, ValidationRegularExpression = property.ValidationRegExp, Mandatory = property.Mandatory,});

				return Enumerable.Empty<IDocumentProperty>();
			}
		}
	}
}