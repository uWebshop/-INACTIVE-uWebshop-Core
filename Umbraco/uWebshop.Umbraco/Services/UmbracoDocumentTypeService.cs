using System.Collections.Generic;
using System.Linq;
using uWebshop.Domain.Businesslogic;
using uWebshop.Domain.Interfaces;
using umbraco.cms.businesslogic.web;

namespace uWebshop.Umbraco.Services
{
	internal class UmbracoDocumentTypeService : ICMSDocumentTypeService
	{
		public IDocumentTypeInfo GetByAlias(string alias)
		{
			var documentType = DocumentType.GetByAlias(alias);
			return documentType != null ? new UmbracoDocument(documentType) : null;
		}
	}

	public class UmbracoDocument : IDocumentTypeInfo
	{
		private readonly DocumentType _documentType;

		public UmbracoDocument(DocumentType documentType)
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