using System;
using uWebshop.Domain.ContentTypes;
using umbraco.cms.businesslogic.datatype;
using umbraco.interfaces;
using uWebshop.Umbraco.DataTypes.CountySelector;

namespace uWebshop.Umbraco.DataTypes.CouponCodeEditor
{
	public class CouponCodeDataType
	{
		public static readonly Guid DefId = new Guid("4db2a111-36e1-4f96-a021-fd98be96aeaf");
		public static readonly Guid Key = new Guid("0dbc0113-a084-44d1-8fef-f8ef0cd8453b");
		public static readonly string Name = "uWebshop Couponcode Editor";
		public static readonly DatabaseType DatabaseType = DatabaseType.Ntext;
	}
}