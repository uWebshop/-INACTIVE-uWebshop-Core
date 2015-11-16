using System;
using System.Runtime.Serialization;
using uWebshop.DataAccess;
using uWebshop.DataAccess.Pocos;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Domain
{
	/// <summary>
	/// 
	/// </summary>
	[DataContract(Namespace = "")]
	[Serializable]
	[KnownType(typeof(OrderSeries))]
	public class OrderSeries : IOrderSeries
	{
		public OrderSeries()
		{
			
		}
		/// <summary>
		/// uWebshop Orderseries (to repat orders)
		/// </summary>
		/// <param name="orderData"></param>
		public OrderSeries(uWebshopOrderData orderData)
		{
			Id = orderData.SeriesId;
			CronInterval = orderData.SeriesCronInterval;
			if (orderData.SeriesStart != null) Start = orderData.SeriesStart.Value;
			End = orderData.SeriesEnd;
			EndAfterInstances = orderData.SeriesEndAfterInstances;
		}

		/// <summary>
		/// Gets or sets the unique identifier.
		/// </summary>
		[DataMember]
		public int Id { get; set; }
		/// <summary>
		/// Gets or sets the cron interval.
		/// </summary>
		[DataMember]
		public string CronInterval { get; set; }
		/// <summary>
		/// Gets or sets the start datetime.
		/// </summary>
		[DataMember]
		public DateTime Start { get; set; }
		/// <summary>
		/// Gets or sets the end datetime.
		/// </summary>
		[DataMember]
		public DateTime? End { get; set; }
		/// <summary>
		/// Gets or sets the number of instances after which the series ends, 0 for no end.
		/// </summary>
		[DataMember]
		public int EndAfterInstances { get; set; }
	}
}