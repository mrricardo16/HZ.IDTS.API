using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Model.Logic
{
    public class UpdateInfoEntity
    {
        /// <summary>
        /// 是否更新
        /// </summary>
        public string serverFlag { get; set; }
        /// <summary>
        /// 获取PDA的下载地址
        /// </summary>
        public string updateUrl { get; set; }
        /// <summary>
        /// 待更新apk名称
        /// </summary>
        public string serverVersionName { get; set; }
        public string upGradeInfo { get; set; }
    }
}
