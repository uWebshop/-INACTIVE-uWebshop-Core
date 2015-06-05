using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using uWebshop.Domain.Services;

namespace uWebshop.Test.Domain.Updating
{
	[TestFixture]
	public class OrderUpdatingServiceCreateCronInterval
	{
		private readonly DateTime _startDate = new DateTime(2015, 6, 4, 15, 0, 0);

		[TestCase("00 15 * * mon", "weekly", "", 1, "mon")]
		[TestCase("00 15 * * tue,wed", "weekly", "", 1, "tue,wed")]
		[TestCase("w2|00 15 * * thu", "weekly", "", 2, "thu")]
		[TestCase("00 15 1-7 * thu", "monthly", "", 1, "")]
		[TestCase("00 15 1-7 * thu", "monthly", "", 1, "fri")]
		[TestCase("00 15 1-7 */3 thu", "monthly", "", 3, "")]
		[TestCase("00 15 * * mon|12:30", "weekly", "12:30", 1, "mon")]
		[TestCase("00 15 * * mon|15:30,16:00,17:00", "weekly", "15:30,16:00,17:00", 1, "mon")]
		public void TestCron(string expected, string repeatNature, string repeatTimes, int interval, string days)
		{
			var actual = OrderUpdatingService.CreateCronInterval(_startDate, repeatNature, repeatTimes, interval, days);
			Console.WriteLine(actual.Item2);
			Assert.AreEqual(expected, actual.Item1);
		}
	}
}
