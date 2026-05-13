using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Model.Entity.OpenApi
{
    #region RGV实时采集项
    /// <summary>
    /// RGV实时采集项
    /// </summary>
    public class RGVRealCollectViewModel
    {
        /// <summary>
        /// RGV数据项数组
        /// </summary>
        public List<RGV> RGV { get; set; }
    }
    #endregion

    #region RGV数据项
    /// <summary>
    /// RGV数据项
    /// </summary>
    public class RGV
    {
        #region 名称
        /// <summary>
        /// 名称
        /// </summary>
        private string _name = string.Empty;
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (value is null)
                {
                    _name = string.Empty;
                }
                else
                {
                    _name = value;
                }
            }
        }
        #endregion
        #region 编码
        /// <summary>
        /// 编码
        /// </summary>
        private string _code = string.Empty;
        public string Code
        {
            get
            {
                return _code;
            }
            set
            {
                if (value is null)
                {
                    _code = string.Empty;
                }
                else
                {
                    _code = value;
                }
            }
        }
        #endregion
        #region 异常码
        /// <summary>
        /// 异常码
        /// </summary>
        private string _errCode = string.Empty;
        public string ErrCode
        {
            get
            {
                return _errCode;
            }
            set
            {
                if (value is null)
                {
                    _errCode = string.Empty;
                }
                else
                {
                    _errCode = value;
                }
            }
        }
        #endregion
        #region 异常信息
        /// <summary>
        /// 异常信息
        /// </summary>
        private string _errMsg = string.Empty;
        public string ErrMsg
        {
            get
            {
                return _errMsg;
            }
            set
            {
                if (value is null)
                {
                    _errMsg = string.Empty;
                }
                else
                {
                    _errMsg = value;
                }
            }
        }
        #endregion
        #region 任务号信息
        /// <summary>
        /// 任务号信息
        /// </summary>
        private string _taskNo = string.Empty;
        public string TaskNo
        {
            get
            {
                return _taskNo;
            }
            set
            {
                if (value is null)
                {
                    _taskNo = string.Empty;
                }
                else
                {
                    _taskNo = value;
                }
            }
        }
        #endregion
        #region 起始站点
        /// <summary>
        /// 起始站点
        /// </summary>
        private string _startStation = string.Empty;
        public string StartStation
        {
            get
            {
                return _startStation;
            }
            set
            {
                if (value is null)
                {
                    _startStation = string.Empty;
                }
                else
                {
                    _startStation = value;
                }
            }
        }
        #endregion
        #region 目标站点
        /// <summary>
        /// 目标站点
        /// </summary>
        private string _endStation = string.Empty;
        public string EndStation
        {
            get
            {
                return _endStation;
            }
            set
            {
                if (value is null)
                {
                    _endStation = string.Empty;
                }
                else
                {
                    _endStation = value;
                }
            }
        }
        #endregion
        #region 状态（待机或故障、新建任务、开始移动、移动中、取货、放货、完成）
        /// <summary>
        /// 状态（待机或故障、新建任务、开始移动、移动中、取货、放货、完成）
        /// </summary>
        private string _state = string.Empty;
        public string State
        {
            get
            {
                return _state;
            }
            set
            {
                if (value is null)
                {
                    _state = string.Empty;
                }
                else
                {
                    _state = value;
                }
            }
        }
        #endregion
        #region 托盘类型（空托盘|满托盘|空）
        /// <summary>
        /// 托盘类型（空托盘|满托盘|空）
        /// </summary>
        private string _trayType = string.Empty;
        public string TrayType
        {
            get
            {
                return _trayType;
            }
            set
            {
                if (value is null)
                {
                    _trayType = string.Empty;
                }
                else
                {
                    _trayType = value;
                }
            }
        }
        #endregion
        #region 货物信息
        /// <summary>
        /// 货物信息
        /// </summary>
        private string _goodsInfo = string.Empty;
        public string GoodsInfo
        {
            get
            {
                return _goodsInfo;
            }
            set
            {
                if (value is null)
                {
                    _goodsInfo = string.Empty;
                }
                else
                {
                    _goodsInfo = value;
                }
            }
        }
        #endregion
    }
    #endregion
}
