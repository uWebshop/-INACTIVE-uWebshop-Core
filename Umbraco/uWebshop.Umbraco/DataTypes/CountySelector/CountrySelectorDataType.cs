using System;
using uWebshop.Domain.ContentTypes;
using umbraco.cms.businesslogic.datatype;
using umbraco.interfaces;

namespace uWebshop.Umbraco.DataTypes.CountySelector
{
	public class CountrySelectorDataType
	{
		public static readonly Guid DefId = new Guid("cdf9f220-ea26-4f37-9505-4b65dfdd59e8");
		public static readonly Guid Key = new Guid("a6d19ee8-ab93-42d6-a61a-8d4aaa759207");
		public static readonly string Name = "uWebshop Country Selector";
		public static readonly DatabaseType DatabaseType = DatabaseType.Ntext;
	}
}