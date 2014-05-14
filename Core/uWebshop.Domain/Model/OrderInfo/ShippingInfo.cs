using System;
using System.Runtime.Serialization;
using uWebshop.Common;

namespace uWebshop.Domain
{
	/// <summary>
	///     Shipping Provider and Shipping Transaction details of the order
	/// </summary>
	[DataContract(Namespace = "")]
	[Serializable]
	public class ShippingInfo
	{
		/// <summary>
		///     Payment Provider Id
		/// </summary>
		[DataMember]
		public int Id { get; set; }

		/// <summary>
		///     Payment Provider Title
		/// </summary>
		[DataMember]
		public string Title { get; set; }

		/// <summary>
		///     Payment Provider Method Id
		/// </summary>
		[DataMember]
		public string MethodId { get; set; }

		/// <summary>
		///     Payment Provider Method Title
		/// </summary>
		[DataMember]
		public string MethodTitle { get; set; }

		/// <summary>
		///     Url of the Payment Provider
		/// </summary>
		[DataMember]
		public string Url { get; set; }

		/// <summary>
		///     Parameters to sent over to the Shipping Provider
		/// </summary>
		[DataMember]
		public string Parameters { get; set; }

		/// <summary>
		///     Unique Id for the transaction of the shipping
		/// </summary>
		[DataMember]
		public string TransactionId { get; set; }

		/// <summary>
		///     They way this transaction needs to be send over to the Shipping provider
		///     Options are: QueryString, Form, Custom, ServerPost
		/// </summary>
		[DataMember]
		public ShippingTransactionMethod TransactionMethod { get; set; }

		/// <summary>
		///     Type of the Shipping provider
		///     Uknownn, Shipping, Pickup
		/// </summary>
		[DataMember]
		public ShippingProviderType ShippingType { get; set; }

		/// <summary>
		///     Possible error messages returned from the Shipping provider
		/// </summary>
		[DataMember]
		public string ErrorMessage { get; set; }
	}
}