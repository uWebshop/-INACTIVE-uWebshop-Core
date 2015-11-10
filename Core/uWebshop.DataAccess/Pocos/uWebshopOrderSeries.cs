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
        [NullSetting(NullSetting = NullSettings.NotNull)]
        public int id { get; set; }

        [Column("cronInterval")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string CronInterval { get; set; }

        [Column("start")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public DateTime Start { get; set; }

        [Column("end")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public DateTime End { get; set; }

        [Column("endAfterInstances")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public int EndAfterInstances { get; set; }
    }
}
