using System.Collections.Generic;
using System.Linq;
using uWebshop.Domain.Businesslogic;
using uWebshop.Domain.Interfaces;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Services;

namespace uWebshop.Umbraco.Services
{
	internal class UmbracoDocumentTypeService : ICMSDocumentTypeService
	{
		public static IContentTypeService ContentTypeService = ApplicationContext.Current.Services.ContentTypeService;
		public IDocumentTypeInfo GetByAlias(string alias)
		{
			var documentType = ContentTypeService.GetContentType(alias);
			return documentType != null ? new UmbracoDocument(documentType) : null;
		}
	}

	public class UmbracoDocument : IDocumentTypeInfo
	{
		private readonly IContentType _documentType;

		public UmbracoDocument(IContentType documentType)
		{
			_documentType = documentType;
		}

		public IEnumerable<IDocumentProperty> Properties
		{
			get
			{
				if (_documentType != null && _documentType.PropertyTypes != null)
					return _documentType.PropertyTypes.Select(property => new DocumentProperty {Alias = property.Alias, Name = property.Name, ValidationRegularExpression = property.ValidationRegExp, Mandatory = property.Mandatory,});

				return Enumerable.Empty<IDocumentProperty>();
			}
		}
	}
}