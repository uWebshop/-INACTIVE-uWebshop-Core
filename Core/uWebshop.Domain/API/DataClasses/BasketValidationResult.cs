using System.Runtime.Serialization;
using uWebshop.Domain;
using uWebshop.Domain.Interfaces;

namespace uWebshop.API
{
	[DataContract]
	internal class BasketValidationResult : IValidationResult
	{
		public BasketValidationResult(OrderValidationError err)
		{
			Id = err.Id; 
			Alias = err.Alias;
			Key = err.Key; 
			Value = err.Value;
			Name = err.Name;
		}
		[DataMember]
		public int Id { get; set; }
		[DataMember]
		public string Alias { get; set; }
		[DataMember]
		public string Key { get; set; }
		[DataMember]
		public string Value { get; set; }
		[DataMember]
		public string Name { get; set; }
	}
}