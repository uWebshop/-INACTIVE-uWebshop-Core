using System;
using uWebshop.DataAccess;

namespace uWebshop.Domain
{
	/// <summary>
	/// 
	/// </summary>
	public class OrderSeries
	{
		public OrderSeries()
		{
			
		}
		public OrderSeries(OrderData orderData)
		{
			Id = orderData.SeriesId;
			CronInterval = orderData.SeriesCronInterval;
			Start = orderData.SeriesStart.Value;
			End = orderData.SeriesEnd;
			EndAfterInstances = orderData.SeriesEndAfterInstances;
		}

		/// <summary>
		/// Gets or sets the unique identifier.
		/// </summary>
		public int Id { get; set; }
		/// <summary>
		/// Gets or sets the cron interval.
		/// </summary>
		public string CronInterval { get; set; }
		/// <summary>
		/// Gets or sets the start datetime.
		/// </summary>
		public DateTime Start { get; set; }
		/// <summary>
		/// Gets or sets the end datetime.
		/// </summary>
		public DateTime? End { get; set; }
		/// <summary>
		/// Gets or sets the number of instances after which the series ends, 0 for no end.
		/// </summary>
		public int EndAfterInstances { get; set; }
	}
}