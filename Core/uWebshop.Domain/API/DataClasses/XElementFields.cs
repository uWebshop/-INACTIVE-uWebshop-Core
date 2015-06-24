using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Linq;
using uWebshop.Domain.Interfaces;

namespace uWebshop.API
{
	[DataContract(Namespace = "")]
	internal class XElementFields : IOrderFields
	{
		private readonly XElement _source;

		public XElementFields(XElement source)
		{
			_source = source;
		}

		[DataMember]
		public IEnumerable<Property> Properties
		{
			get
			{
				var enumElements = new List<Property>();
				if (_source != null && _source.Elements().Any())
				{
					enumElements.AddRange(
						_source.Elements().Select(field => new Property { Alias = field.Name.LocalName, Value = field.Value }));
				}

				return enumElements;
			}
			set { }
		}
		
		public T GetValue<T>(string fieldName)
		{
			return Helpers.GetValue<T>(fieldName, _source);
		}
	}
}