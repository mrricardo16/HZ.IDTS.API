using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Model.Entity.Simulate
{
    [SugarTable("tn_dts_agvtimetracking")]
    public class tn_dts_agvtimetracking
    {
        [SugarColumn(ColumnName = "cn_t_tracking_timestamp")]
        public DateTime cn_t_tracking_timestamp { get; set; }


        [SugarColumn(ColumnName = "cn_t_tracking_statejson")]
        public string cn_t_tracking_statejson { get; set; }
    }
}
