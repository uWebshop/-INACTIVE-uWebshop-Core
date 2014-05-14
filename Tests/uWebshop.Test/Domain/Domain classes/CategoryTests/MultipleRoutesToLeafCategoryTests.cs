using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using uWebshop.Domain;

namespace uWebshop.Test.Domain.Domain_classes.CategoryTests
{
	[TestFixture]
	public class MultipleRoutesToLeafCategoryTests
	{
		[Test]
		public void UrlRenderingForSecondaryRoute()
		{
			var a = new Category();
			//a.NiceUrl()
		}
	}
}