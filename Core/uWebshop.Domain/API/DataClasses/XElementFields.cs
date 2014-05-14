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
		
		public T GetValue<T>(string fieldName)
		{
			return Helpers.GetValue<T>(fieldName, _source);
		}
	}
}