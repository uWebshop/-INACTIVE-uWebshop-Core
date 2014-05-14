using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;
using uWebshop.Domain;
using uWebshop.Domain.Businesslogic;
using uWebshop.Domain.Interfaces;
using uWebshop.Newtonsoft.Json;

namespace uWebshop.API
{
	[KnownType(typeof(OrderedProductInfoAdaptor))]
	[DataContract(Name="OrderLine", Namespace= "")]
	internal class OrderLineBasketAdaptor : IOrderLine
	{
		private readonly OrderLine _line;
		private readonly OrderInfo _order;
		
		public OrderLineBasketAdaptor(OrderLine line, OrderInfo order)
		{
			_line = line;
			_order = order;
			Id = line.OrderLineId;
		}

		[DataMember]
		public int Quantity { get { return _line.ProductInfo.Quantity; } set { } }
		[DataMember]
		public IOrderedProduct Product { get { return new OrderedProductInfoAdaptor(_line.ProductInfo); } set { } }

	    [IgnoreDataMember]
	    public IDiscountedRangedPrice Amount
	    {
	        get { return _line.Amount; }
	    }

        [DataMember(Name = "Amount")]
        [JsonProperty(PropertyName = "Amount")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public FlatDiscountedPrice AmountFlat { get { return new FlatDiscountedPrice(Amount); } set { } }

		[DataMember]
		public int Id { get; set; }

		public T GetValue<T>(string fieldName)
		{
			return Helpers.GetValue<T>(fieldName, _line.CustomData);
		}
	}
}