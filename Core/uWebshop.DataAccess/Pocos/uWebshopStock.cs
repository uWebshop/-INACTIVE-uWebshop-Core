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
        public int id { get; set; }

        [Column("Stock")]
        public int Stock { get; set; }

        [Column("NodeId")]
        public int NodeId { get; set; }

        [Column("OrderCount")]
        public int OrderCount { get; set; }

        [Column("StoreAlias")]
        public string StoreAlias { get; set; }

        [Column("createDate")]
        public DateTime CreatDateTime { get; set; }

        [Column("updateDate")]
        public DateTime UpdateDateTime { get; set; }

    }

}
