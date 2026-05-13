using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HZ.IDTSCore.Model
{

    [SugarTable("tn_wms_lotcode")]
    public class tn_wms_lotcode : ICloneable
    {
        public tn_wms_lotcode()
        {
        }
        /// <summary>
        /// 描述 : 批次条码
        /// 空值 : True
        /// 默认 : 
        /// <summary>
        
        [SugarColumn(IsPrimaryKey = true)]
        public string cn_s_lot_barcode { get; set; }

        /// <summary>
        /// 描述 : 物料编码
        /// 空值 : True
        /// 默认 : 
        /// <summary>
        
        public string cn_s_item_code { get; set; }


        /// <summary>
        /// 描述 : 物料名称
        /// 空值 : True
        /// 默认 : 
        /// <summary>
        
        public string cn_s_item_name { get; set; }

        /// <summary>
        /// 描述 : 规格型号
        /// 空值 : True
        /// 默认 : 
        /// <summary>
        
        public string cn_s_model { get; set; }

        /// <summary>
        /// 描述 : 单位
        /// 空值 : True
        /// 默认 : 
        /// <summary>
        
        public string cn_s_unit { get; set; }

        /// <summary>
        /// 描述 : 生产日期
        /// 空值 : True
        /// 默认 : 
        /// <summary>
        
        public DateTime? cn_t_production { get; set; }

        /// <summary>
        /// 描述 : 入库日期
        /// 空值 : True
        /// 默认 : 
        /// <summary>
        
        public DateTime? cn_t_in_storage_date { get; set; }

        /// <summary>
        /// 描述 : 生产批次
        /// 空值 : True
        /// 默认 : 
        /// <summary>
        
        public string cn_s_production_batch { get; set; }

        /// <summary>
        /// 描述 : 其他批号
        /// 空值 : True
        /// 默认 : 
        /// <summary>
        
        public string cn_s_other_lotcode { get; set; }

        /// <summary>
        /// 描述 : 是否已打印
        /// 空值 : True
        /// 默认 : 
        /// <summary>
        
        public bool cn_b_printed { get; set; }


        /// <summary>
        /// 描述 :  创建人
        /// 空值 : True
        /// 默认 : 
        /// <summary>
        
        public string cn_s_creator { get; set; }

        /// <summary>
        /// 描述 :  创建人名称
        /// 空值 : True
        /// 默认 : 
        /// <summary>
        
        public string cn_s_creator_by { get; set; }

        /// <summary>
        /// 描述 :  创建时间
        /// 空值 : True
        /// 默认 : 
        /// <summary>
        
        public DateTime? cn_t_create { get; set; }

        /// <summary>
        /// 描述 :  修改人
        /// 空值 : True
        /// 默认 : 
        /// <summary>
        
        public string cn_s_modify { get; set; }

        /// <summary>
        /// 描述 :  修改人名称
        /// 空值 : True
        /// 默认 : 
        /// <summary>
        
        public string cn_s_modify_by { get; set; }

        /// <summary>
        /// 描述 :  修改时间
        /// 空值 : True
        /// 默认 : 
        /// <summary>
        
        public DateTime? cn_t_modify { get; set; }

        /// <summary>
        /// 描述 :  货主
        /// 空值 : True
        /// 默认 : 
        /// <summary>
        
        public string cn_s_owner { get; set; }

        /// <summary>
        /// 描述 :  码值类型
        /// 空值 : True
        /// 默认 : 
        /// <summary>
        
        public string cn_s_code_type { get; set; }
        /// <summary>
        /// 描述 :  来源业务
        /// 空值 : True
        /// 默认 : 
        /// <summary>
        
        public string cn_s_from_buss { get; set; }

        /// <summary>
        /// 描述 :  来源业务单号
        /// 空值 : True
        /// 默认 : 
        /// <summary>
        
        public string cn_s_from_no { get; set; }

        
        public int cn_n_printed_count { get; set; }

        
        public string cn_s_arrival_batch { get; set; }

        
        public decimal cn_f_qty { get; set; }



        [SugarColumn(IsIgnore = true)]
        public int count { get; set; }

        [SugarColumn(IsIgnore = true)]
        public string production { get; set; }

        //[SugarColumn(IsIgnore = true)]
        //public string productiontype { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
