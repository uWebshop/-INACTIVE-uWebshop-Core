using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uWebshop.Domain.Helpers
{
	/// <summary>
	/// 
	/// </summary>
	public class CronHelper
	{
		/// <summary>
		/// Generates the date time instances from a crontab expression starting now, until the end time or one year in advance if not specified.
		/// </summary>
		/// <param name="crontabExpression">The crontab expression.</param>
		/// <param name="endTime">The end time.</param>
		public static IEnumerable<DateTime> GenerateDateTimeInstancesFromCrontabExpressionStartingNow(string crontabExpression, DateTime? endTime = null)
		{
			return CrontabSchedule.CrontabSchedule.Parse(crontabExpression).GetNextOccurrences(DateTime.Now, endTime ?? DateTime.Now.AddYears(1));
		}
	}
}
