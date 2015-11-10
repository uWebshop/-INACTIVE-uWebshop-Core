using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace uWebshop.DataAccess.Pocos
{
    [TableName("uWebshopStock")]
    [PrimaryKey("id", autoIncrement = true)]
    [ExplicitColumns]
    public class uWebshopStock
    {
        [Column("id")]
        [PrimaryKeyColumn(AutoIncrement = true)]
        [NullSetting(NullSetting = NullSettings.NotNull)]
        public int id { get; set; }

        [Column("Stock")]
        [NullSetting(NullSetting = NullSettings.NotNull)]
        public int Stock { get; set; }

        [Column("NodeId")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public int NodeId { get; set; }

        [Column("OrderCount")]
        [NullSetting(NullSetting = NullSettings.NotNull)]
        public int OrderCount { get; set; }

        [Column("StoreAlias")]
        [Length(500)]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string StoreAlias { get; set; }

        [Column("createDate")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public DateTime CreatDateTime { get; set; }

        [Column("updateDate")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public DateTime UpdateDateTime { get; set; }

    }

}
