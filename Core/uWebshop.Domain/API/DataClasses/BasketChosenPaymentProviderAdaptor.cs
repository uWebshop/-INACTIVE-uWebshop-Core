using System.Runtime.Serialization;
using uWebshop.Domain.Interfaces;

namespace uWebshop.API
{
	[DataContract(Namespace = "")]
	internal class BasketChosenPaymentProviderAdaptor : IChosenPaymentProvider
	{
		[DataMember]
		public bool Paid { get { return false; } set { } }
		[DataMember]
		public int Id { get; set; }
		[DataMember]
		public string Title { get; set; }
		[DataMember]
		public string MethodId { get; set; }
		[DataMember]
		public string MethodTitle { get; set; }
		[DataMember]
		public string TransactionId { get; set; }
	}
}