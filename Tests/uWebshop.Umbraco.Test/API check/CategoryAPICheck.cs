using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using uWebshop.Domain;
using uWebshop.Domain.Interfaces;
using uWebshop.RazorExtensions;

namespace uWebshop.Umbraco.Test.API_check
{
	internal class CategoryAPICheck
	{
		internal static void Check(Category category)
		{
			category.NiceUrl();
			var aaa1 = category.Url;
			var aaa2 = category.UrlName;
			var aaa3 = category.Images;
			var aaa4 = category.Tags;

#pragma warning disable 612,618
			Check(category.Categories.First()); // important, razors are out there that use this
#pragma warning restore 612,618
		}
	}
}