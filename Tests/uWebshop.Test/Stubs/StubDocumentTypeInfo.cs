using System.Collections.Generic;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Test.Stubs
{
	public class StubDocumentTypeInfo : IDocumentTypeInfo
	{
		public IEnumerable<IDocumentProperty> Properties
		{
			get { return new List<IDocumentProperty>(); }
		}
	}
}