using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace uWebshop.Test.Common
{
	[TestFixture]
	public class DateTimeMultiCultureParseTests
	{
		[Test]
		public void _20150804RegressionTestsForParsing()
		{
			//04-08-2015 10.45 uur
			//25 aug. 12.00 uur
			//25-08-2015  8.30 uur
			Assert.AreEqual(new DateTime(2015, 8, 25, 12, 0, 0), uWebshop.Common.Helpers.DateTimeMultiCultureParse("25 aug 2015 12:00 uur"));
			Assert.AreEqual(new DateTime(2015, 8, 4, 10, 45, 0), uWebshop.Common.Helpers.DateTimeMultiCultureParse("04-08-2015 10:45 uur"));
		}
	}
}
