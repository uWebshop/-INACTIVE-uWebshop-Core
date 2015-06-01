using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using uWebshop.Domain.Helpers;

namespace uWebshop.Test.Domain.Helper_classes
{
	[TestFixture]
	public class CronHelperTests
	{
		[Test]
		public void ManualCheckOfCronHelper()
		{
			// functionality of NCronTab is assumed tested
			var a = CronHelper.GenerateDateTimeInstancesFromCrontabExpressionStartingNow("0 15 * jan,feb mon,tue");
			foreach (var dateTime in a)
			{
				Console.WriteLine(dateTime);
			}
		}
	}
}
