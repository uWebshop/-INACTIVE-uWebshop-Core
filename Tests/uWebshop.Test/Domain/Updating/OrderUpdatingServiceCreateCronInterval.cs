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
		public void Setup()
		{
			
		}

		[Test]
		public void agds()
		{

			var cron = OrderUpdatingService.CreateCronInterval(DateTime.Now, "weekly", "", 1, "mon,tue");

			Console.WriteLine(cron);
		}

		[TestCase("00 15 * * mon", "weekly", "", 1, "mon")]
		[TestCase("00 15 * * tue,wed", "weekly", "", 1, "tue,wed")]
		[TestCase("w2|00 15 * * thu", "weekly", "", 2, "thu")]
		[TestCase("00 15 1-7 * thu", "monthly", "", 1, "")]
		[TestCase("00 15 1-7 * thu", "monthly", "", 1, "fri")]
		[TestCase("00 15 1-7 */3 thu", "monthly", "", 3, "")]
		public void TestCron(string expected, string repeatNature, string repeatTimes, int interval, string days)
		{
			var actual = OrderUpdatingService.CreateCronInterval(_startDate, repeatNature, repeatTimes, interval, days);
			Console.WriteLine(actual.Item2);
			Assert.AreEqual(expected, actual.Item1);
		}
	}
}
