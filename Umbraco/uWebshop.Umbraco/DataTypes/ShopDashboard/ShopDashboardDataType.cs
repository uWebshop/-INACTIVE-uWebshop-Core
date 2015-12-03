using System;
using uWebshop.Domain.ContentTypes;

namespace uWebshop.Umbraco.DataTypes.ShopDashBoard
{
	public class ShopDashboardDataType
	{
		public static readonly Guid DefId = new Guid("53da7362-3348-4312-9271-38a9381081b4");
        public static readonly Guid Key = new Guid("df1c2d1f-77c2-40fb-a6f8-7fd8612101c5");
		public static readonly string Name = "uWebshop ShopDashboard";
		public static readonly DatabaseType DatabaseType = DatabaseType.Nvarchar;
	}
}