using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Linq;
using uWebshop.Domain.Interfaces;

namespace uWebshop.API
{
	[DataContract(Namespace = "")]
	internal class OrderFields : XElementFields, IOrderFields
	{
		public OrderFields(XElement source) : base(source)
		{
		}
	}
}