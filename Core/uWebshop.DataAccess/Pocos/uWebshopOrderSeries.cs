using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace uWebshop.DataAccess.Pocos
{
    [TableName("uWebshopOrderSeries")]
    [PrimaryKey("id", autoIncrement = true)]
    [ExplicitColumns]
    public class uWebshopOrderSeries
    {
        [Column("id")]
        [PrimaryKeyColumn(AutoIncrement = true)]
        public int id { get; set; }

        [Column("cronInterval")]
        public string CronInterval { get; set; }

        [Column("start")]
        public DateTime Start { get; set; }

        [Column("end")]
        public DateTime End { get; set; }

        [Column("endAfterInstances")]
        public int EndAfterInstances { get; set; }
    }
}
