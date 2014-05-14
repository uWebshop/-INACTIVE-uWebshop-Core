using System.Runtime.Serialization;

namespace uWebshop.API
{
	[DataContract(Namespace = "")]
	public class Localization
	{
		[DataMember]
		public string StoreAlias;
		[DataMember]
		public string CurrencyCode;
		[DataMember]
		public string CurrencySymbol;
		[DataMember]
		public decimal Ratio;
	}
}