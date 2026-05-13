using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HZ.IDTSCore.Model.Entity
{
    [SugarTable("tn_wms_outside_sys")]
    public class tn_wms_outside_sys
    {
        /// <summary>
        /// 外部系统编码
        /// </summary>
        
        [SqlSugar.SugarColumn(IsPrimaryKey =true)]
        public string cn_s_sys_code { get; set; }
        /// <summary>
        /// 外部系统名称
        /// </summary>
        
        public string cn_s_sys_name { get; set; }
        /// <summary>
        /// 系统分类
        /// </summary>
        
        public string cn_s_sys_class { get; set; }
        /// <summary>
        /// 通讯方式
        /// </summary>
        
        public string cn_s_connection_mode { get; set; }
        /// <summary>
        /// IP
        /// </summary>
        
        public string cn_s_ip { get; set; }
        /// <summary>
        /// port
        /// </summary>
        
        public int cn_n_port { get; set; }
        /// <summary>
        /// ext1
        /// </summary>
        
        public string cn_s_ext1 { get; set; }
        /// <summary>
        /// ext2
        /// </summary>
        
        public string cn_s_ext2 { get; set; }
        /// <summary>
        /// ext3
        /// </summary>
        
        public string cn_s_ext3 { get; set; }
    }
}
