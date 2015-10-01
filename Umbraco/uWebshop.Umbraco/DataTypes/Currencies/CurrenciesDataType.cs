using System;
using uWebshop.Domain.ContentTypes;
using umbraco.cms.businesslogic.datatype;
using umbraco.interfaces;

namespace uWebshop.Umbraco.DataTypes.Currencies
{
	public class CurrenciesDataType
	{
		public static readonly Guid DefId = new Guid("9dbebe55-72b2-4573-b16c-9bfe3c4d24f6");
		public static readonly Guid Key = new Guid("7e6cab81-528d-4b00-8321-72c36f131eea");
		public static readonly string Name = "uWebshop Currencies";
		public static readonly DatabaseType DatabaseType = DatabaseType.Ntext;

	}
}