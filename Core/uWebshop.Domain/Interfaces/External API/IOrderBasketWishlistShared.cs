using System;
using System.Collections.Generic;

namespace uWebshop.Domain.Interfaces
{
	/// <summary>
	/// 
	/// </summary>
	public interface IOrderBasketWishlistShared
	{
		// essential stuff
		/// <summary>
		/// Gets the unique unique identifier.
		/// </summary>
		/// <value>
		/// The unique unique identifier.
		/// </value>
		Guid UniqueId { get; }

		/// <summary>
		/// Gets the name given to this order.
		/// </summary>
		/// <value>
		/// The name.
		/// </value>
		string Name { get; }

		/// <summary>
		/// Gets the order amount.
		/// </summary>
		/// <value>
		/// The order amount.
		/// </value>
		IVatPrice OrderAmount { get; } // or TotalOrderAmount oid

		// relations
		/// <summary>
		/// Gets the order lines.
		/// </summary>
		/// <value>
		/// The order lines.
		/// </value>
		IEnumerable<IOrderLine> OrderLines { get; }

		/// <summary>
		/// Gets the customer.
		/// </summary>
		/// <value>
		/// The customer.
		/// </value>
		ICustomer Customer { get; }

		/// <summary>
		/// Get the extra orderfields
		/// </summary>
		IOrderFields OrderFields { get; }

		/// <summary>
		/// Get the series for this order
		/// </summary>
		IOrderSeries OrderSeries { get; }

		/// <summary>
		/// Gets the store.
		/// </summary>
		/// <value>
		/// The store.
		/// </value>
		IStore Store { get; }

		// helpers

		/// <summary>
		/// Gets the grand total.
		/// </summary>
		/// <value>
		/// The grand total.
		/// </value>
		IPrice GrandTotal { get; }

		/// <summary>
		/// Gets the sub total.
		/// </summary>
		/// <value>
		/// The sub total.
		/// </value>
		IPrice SubTotal { get; }

		/// <summary>
		/// Gets the average order vat percentage.
		/// </summary>
		/// <value>
		/// The average order vat percentage.
		/// </value>
		decimal AverageOrderVatPercentage { get; }

		/// <summary>
		/// Gets the order line item quantity.
		/// </summary>
		/// <value>
		/// The total item quantity
		/// </value>
		int Quantity { get; }

		/// <summary>
		/// Gets the order line total.
		/// </summary>
		/// <value>
		/// The order line total.
		/// </value>
		IVatPrice OrderLineTotal { get; }
	}
}