using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HZ.IDTSCore.Model.Entity
{
    [SugarTable("tn_wms_outside_code")]
    public class tn_wms_outside_code
    {

        public tn_wms_outside_code()
        {
        }

        /// <summary>
        /// 描述 :  
        /// 空值 : True
        /// 默认 : 
        /// <summary>
        
        [SugarColumn(IsPrimaryKey = true)]
        public string cn_s_stock_code { get; set; }

        /// <summary>
        /// 描述 :  
        /// 空值 : True
        /// 默认 : 
        /// <summary>
        
        public string cn_s_code_type { get; set; }

        /// <summary>
        /// 描述 :  
        /// 空值 : True
        /// 默认 : 
        /// <summary>
        
        public string cn_s_inside_code { get; set; }

        /// <summary>
        /// 描述 :  
        /// 空值 : True
        /// 默认 : 
        /// <summary>
        
        public string cn_s_outside_sys { get; set; }

        /// <summary>
        /// 描述 :  
        /// 空值 : True
        /// 默认 : 
        /// <summary>
        
        public string cn_s_outside_code_flag { get; set; }

        /// <summary>
        /// 描述 :  
        /// 空值 : True
        /// 默认 : 
        /// <summary>
        
        public string cn_s_outside_code { get; set; }


        /// <summary>
        /// 描述 :  
        /// 空值 : True
        /// 默认 : 
        /// <summary>
        
        public string cn_s_creator { get; set; }

        /// <summary>
        /// 描述 :  
        /// 空值 : True
        /// 默认 : 
        /// <summary>
        
        public string cn_s_creator_by { get; set; }

        /// <summary>
        /// 描述 :  
        /// 空值 : True
        /// 默认 : 
        /// <summary>
        
        public DateTime? cn_t_create { get; set; }

    }
}
