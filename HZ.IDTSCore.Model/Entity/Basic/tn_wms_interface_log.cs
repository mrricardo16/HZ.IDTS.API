using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HZ.IDTSCore.Model.Entity.Basic
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable("tn_wms_interface_log")]
    public class tn_wms_interface_log
    {
        public tn_wms_interface_log()
        {
        }

        /// <summary>
        /// guid
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public string cn_s_guid { get; set; }

        /// <summary>
        /// 接口编码
        /// </summary>
        public string cn_s_infcode { get; set; }


        /// <summary>
        /// 接口地址
        /// </summary>
        public string cn_s_url { get; set; }

        /// <summary>
        /// 访问类型
        /// </summary>
        public string cn_s_mode { get; set; }


        /// <summary>
        /// 数据类型
        /// </summary>
        public string cn_s_data_type { get; set; }


        /// <summary>
        /// 请求数据
        /// </summary>
        public string cn_s_data { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public string cn_s_state { get; set; }


        /// <summary>
        /// 返回结果
        /// </summary>
        public string cn_s_response { get; set; }

        /// <summary>
        /// 异常原因
        /// </summary>
        public string cn_s_err_msg { get; set; }

        /// <summary>
        /// 失败重试次数
        /// </summary>
        public int cn_n_retry_coun { get; set; }


        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? cn_t_create { get; set; }

        /// <summary>
        /// header
        /// </summary>
        public string cn_s_header { get; set; }


        /// <summary>
        /// 是否挂起
        /// </summary>
        public bool cn_b_suspend { get; set; }


        /// <summary>
        /// 接口名称
        /// </summary>
        public string cn_s_infname { get; set; }

        /// <summary>
        /// 服务者  系统、手工
        /// </summary>
        public string cn_s_server { get; set; }

        /// <summary>
        /// 请求头反射
        /// </summary>
        //public string cn_s_header_reflect { get; set; }
    }
}