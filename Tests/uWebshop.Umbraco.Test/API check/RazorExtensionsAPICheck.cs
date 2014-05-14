using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using uWebshop.Domain;

namespace uWebshop.Umbraco.Test.API_check
{
	internal class RazorExtensionsAPICheck
	{
		private void Check()
		{
#pragma warning disable 612,618
			var category = RazorExtensions.Catalog.GetCategory();
			CategoryAPICheck.Check(category);

			Product p1 = RazorExtensions.Catalog.GetProduct();
			p1 = RazorExtensions.Catalog.GetProduct(1234);
#pragma warning restore 612,618
		}
	}
}