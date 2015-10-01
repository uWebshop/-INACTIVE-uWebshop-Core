using System;
using uWebshop.Domain.ContentTypes;
using umbraco.cms.businesslogic.datatype;
using umbraco.interfaces;
using uWebshop.Umbraco.DataTypes.CountySelector;

namespace uWebshop.Umbraco.DataTypes.OrderInfoViewer
{
    public class OrderInfoViewerDataType
    {
        public static readonly Guid DefId = new Guid("ec7573c7-0b80-481d-b013-0f791e2756bb");
        public static readonly Guid Key = new Guid("e500f0c0-7fc2-49ed-89de-fe1091398a99");
        public static readonly string Name = "uWebshop OrderInfo Viewer";
        public static readonly DatabaseType DatabaseType = DatabaseType.Ntext;

    }
}