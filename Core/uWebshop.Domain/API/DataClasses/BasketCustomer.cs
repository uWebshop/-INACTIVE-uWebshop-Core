using System.Linq;
using System.Runtime.Serialization;
using uWebshop.Domain;
using uWebshop.Domain.Interfaces;

namespace uWebshop.API
{
	[KnownType(typeof(XElementAddress))]
	[DataContract(Namespace = "")]
	internal class BasketCustomer : XElementAddress, ICustomer
	{
		public BasketCustomer(CustomerInfo info) : base(info.CustomerInformation, "customer")
		{
			Shipping = new XElementAddress(info.ShippingInformation, "shipping", info.CustomerIsShipping);
			UserId = info.CustomerId.ToString();
			UserName = info.LoginName;
			CustomerIsShipping = info.CustomerIsShipping;
			AcceptsMarketing = info.AcceptsMarketing;
		}
		
		[DataMember]
		public IAddress Shipping { get; set; }
		[DataMember]
		public bool AcceptsMarketing { get; set; }
		[DataMember]
		public string UserName { get; set; }
		[DataMember]
		public string UserId { get; set; }
		[DataMember]
		public bool CustomerIsShipping { get; set; }
		
		private int? _totalSpending;
		[DataMember]
		public int TotalSpendingInCents
		{
			get
			{
				if (_totalSpending == null)
				{
					var orders = Orders.GetOrdersForCustomer(UserName);
					//if (!string.IsNullOrEmpty(storeAlias))
					//{
					//	orders = orders.Where(x => x.Store.Alias.ToUpperInvariant() == storeAlias.ToUpperInvariant());
					//}
					_totalSpending = (int)orders.Sum(x => x.ChargedOrderAmount.ValueInCents / x.Localization.Currency.Ratio);
				}
				return _totalSpending.GetValueOrDefault();
			}
			set { _totalSpending = value; }
		}
	}
}