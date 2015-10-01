using System;
using uWebshop.Domain.ContentTypes;
using umbraco.cms.businesslogic.datatype;
using umbraco.interfaces;
using uWebshop.Umbraco.DataTypes.CountySelector;

namespace uWebshop.Umbraco.DataTypes.OrderCountViewer
{
    public class OrderCountDataType
    {
        public static readonly Guid DefId = new Guid("1779c16d-1b5d-4312-974b-782590caca3e");
        public static readonly Guid Key = new Guid("2f6e0aa8-8291-4544-8aeb-db78cfb42b07");
        public static readonly string Name = "uWebshop Item Ordered Count";
        public static readonly DatabaseType DatabaseType = DatabaseType.Integer;
    }

}