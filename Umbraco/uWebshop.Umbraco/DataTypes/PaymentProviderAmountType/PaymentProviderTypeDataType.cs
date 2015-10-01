using System;
using uWebshop.Domain.ContentTypes;
using umbraco.cms.businesslogic.datatype;
using umbraco.interfaces;
using uWebshop.Umbraco.DataTypes.CountySelector;

namespace uWebshop.Umbraco.DataTypes.PaymentProviderAmountType
{
	public class PaymentProviderAmountTypeDataType
	{
		public static readonly Guid DefId = new Guid("85c8f557-cf82-4450-856f-79492a02f55d");
		public static readonly Guid Key = new Guid("fb79c76e-8ccc-406d-a3ad-39c1f939a38d");
		public static readonly string Name = "uWebshop Payment Provider Amount Type";
		public static readonly DatabaseType DatabaseType = DatabaseType.Ntext;	
	}
}