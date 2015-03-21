using System;
using uWebshop.Common;

namespace uWebshop.Domain.Interfaces
{
	/// <summary>
	/// 
	/// </summary>
	public interface IOrder : IOrderBasketShared
	{
		/// <summary>
		/// Gets the localization.
		/// </summary>
		/// <value>
		/// The localization.
		/// </value>
		ILocalization Localization { get; }

		/// <summary>
		/// Gets the order reference.
		/// </summary>
		/// <value>
		/// The order reference.
		/// </value>
		string OrderReference { get; } // OrderNumber
		/// <summary>
		/// Gets the store order reference unique identifier.
		/// </summary>
		/// <value>
		/// The store order reference unique identifier.
		/// </value>
		int StoreOrderReferenceId { get; } // maybe leave out

		/// <summary>
		/// Gets the status.
		/// </summary>
		/// <value>
		/// The status.
		/// </value>
		OrderStatus Status { get; }
		/// <summary>
		/// Gets the confirm date.
		/// </summary>
		/// <value>
		/// The confirm date.
		/// </value>
		DateTime ConfirmDate { get; }
		/// <summary>
		/// Gets the paid date.
		/// </summary>
		/// <value>
		/// The paid date.
		/// </value>
		DateTime PaidDate { get; }
		/// <summary>
		/// Gets the shipped date.
		/// </summary>
		/// <value>
		/// The shipped date.
		/// </value>
		DateTime ShippedDate { get; }

		// helpers
		/// <summary>
		/// Gets a value indicating whether [is paid].
		/// </summary>
		/// <value>
		///   <c>true</c> if [is paid]; otherwise, <c>false</c>.
		/// </value>
		bool IsPaid { get; }
		/// <summary>
		/// Gets a value indicating whether [is fulfilled].
		/// </summary>
		/// <value>
		///   <c>true</c> if [is fulfilled]; otherwise, <c>false</c>.
		/// </value>
		bool IsFulfilled { get; }

		int ContentId { get; }
	}
}