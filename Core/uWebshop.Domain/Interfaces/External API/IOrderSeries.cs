using System;
using System.Collections.Generic;

namespace uWebshop.Domain.Interfaces
{
	/// <summary>
	/// 
	/// </summary>
	public interface IOrderSeries
	{
		int Id { get; set; }
		/// <summary>
		/// the cron interval.
		/// </summary>
		string CronInterval { get; set; }
		/// <summary>
		/// the start datetime.
		/// </summary>
		DateTime Start { get; set; }
		/// <summary>
		///the end datetime.
		/// </summary>
		DateTime? End { get; set; }
		/// <summary>
		/// the number of instances after which the series ends, 0 for no end.
		/// </summary>
		int EndAfterInstances { get; set; }
	}
}