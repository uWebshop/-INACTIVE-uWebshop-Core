using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uWebshop.Common;

namespace uWebshop.Domain
{
	/// <summary>
	/// 
	/// </summary>
	public static class OrderInfoExtensions
	{
		/// <summary>
		/// Determines whether the order is not confirmed.
		/// </summary>
		/// <param name="order">The order.</param>
		public static bool IsNotConfirmed(this OrderInfo order)
		{
			return order.Status == OrderStatus.Incomplete || order.Status == OrderStatus.Scheduled; // todo: what about whishlist?
		}

		/// <summary>
		/// Determines whether the order is not confirmed.
		/// </summary>
		/// <param name="order">The order.</param>
		public static bool IsBasket(this OrderInfo order)
		{
			return order.Status == OrderStatus.Incomplete; // not scheduled!
		}

		/// <summary>
		/// Determines whether the order is confirmed.
		/// </summary>
		/// <param name="order">The order.</param>
		public static bool IsConfirmed(this OrderInfo order)
		{
			return order.Status != OrderStatus.Incomplete && order.Status != OrderStatus.Scheduled; // todo: what about whishlist?
		}
	}
}
