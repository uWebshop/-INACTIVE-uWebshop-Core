using System;
using System.Runtime.Serialization;
using uWebshop.Common;

namespace uWebshop.Domain
{
	/// <summary>
	///     Payment Provider and Payment Transaction details of the order
	/// </summary>
	[DataContract(Namespace = "")]
	[Serializable]
	public class PaymentInfo
	{
		/// <summary>
		///     Payment Provider Id
		/// </summary>
		[DataMember]
		public int Id { get; set; }

        /// <summary>
        ///     Payment Provider Key
        /// </summary>
        [DataMember]
        public Guid Key { get; set; }

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
        ///     Payment Provider Method Key
        /// </summary>
        [DataMember]
        public Guid MethodKey { get; set; }

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
		///     Parameters to sent over to the Payment Provider
		/// </summary>
		[DataMember]
		public string Parameters { get; set; }

		/// <summary>
		///     Unique Id for the transaction of the payment
		/// </summary>
		[DataMember]
		public string TransactionId { get; set; }

		/// <summary>
		///     They way this transaction needs to be send over to the payment provider
		///     Options are: QueryString, Form, Custom, ServerPost
		/// </summary>
		[DataMember]
		public PaymentTransactionMethod TransactionMethod { get; set; }

		/// <summary>
		///     Type of the payment provider
		///     OnlinePayment, OfflinePaymentInStore, OfflinePaymentAtCustomer
		/// </summary>
		[DataMember]
		public PaymentProviderType PaymentType { get; set; }

		/// <summary>
		///     Possible error messages returned from the payment provider
		/// </summary>
		[DataMember]
		public string ErrorMessage { get; set; }
	}
}