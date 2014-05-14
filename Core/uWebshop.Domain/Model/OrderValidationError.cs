using System;

namespace uWebshop.Domain
{
#pragma warning disable 1591
	[Serializable]
	public class OrderValidationError
	{
		public int Id { get; set; }
		public string Alias { get; set; }
		public string Key { get; set; }
		public string Value { get; set; }
		public string Name { get; set; }
	}
}