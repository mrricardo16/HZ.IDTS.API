using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Model.Entity.OpenApi
{
    #region 堆垛车实时采集项
    /// <summary>
    /// 堆垛车实时采集项
    /// </summary>
    public class StackerRealCollectViewModel
    {
        /// <summary>
        /// 堆垛车数据项数组
        /// </summary>
        public List<Stacker>  Stacker { get; set; }
    }
    #endregion

    #region 堆垛车数据项
    /// <summary>
    /// 堆垛车数据项
    /// </summary>
    public class Stacker
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
        #region 任务类型（入库/出库）
        /// <summary>
        /// 任务类型（入库/出库）
        /// </summary>
        private string _taskType = string.Empty;
        public string TaskType
        {
            get
            {
                return _taskType;
            }
            set
            {
                if (value is null)
                {
                    _taskType = string.Empty;
                }
                else
                {
                    _taskType = value;
                }
            }
        }
        #endregion
        #region 任务号（唯一）
        /// <summary>
        /// 任务号（唯一）
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
        #region 起始站点（入库起点在线体，出库起点在立库）
        /// <summary>
        /// 起始站点（入库起点在线体，出库起点在立库）
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
        #region 目标站点（入库终点在立库，出库终点在线体）
        /// <summary>
        /// 目标站点（入库终点在立库，出库终点在线体）
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
        #region 行走（单位：mm）
        /// <summary>
        /// 行走（单位：mm）
        /// </summary>
        private double? _xposition = 0;
        public double? Xposition
        {
            get
            {
                return _xposition;
            }
            set
            {
                if (value is null)
                {
                    _xposition = 0;
                }
                else
                {
                    _xposition = value;
                }
            }
        }
        #endregion
        #region 举升（单位：mm）
        /// <summary>
        /// 举升（单位：mm）
        /// </summary>
        private double? _yposition = 0;
        public double? Yposition
        {
            get
            {
                return _yposition;
            }
            set
            {
                if (value is null)
                {
                    _yposition = 0;
                }
                else
                {
                    _yposition = value;
                }
            }
        }
        #endregion
        #region 货叉（单位：mm）
        /// <summary>
        /// 货叉（单位：mm）
        /// </summary>
        private double? _zposition = 0;
        public double? Zposition
        {
            get
            {
                return _zposition;
            }
            set
            {
                if (value is null)
                {
                    _zposition = 0;
                }
                else
                {
                    _zposition = value;
                }
            }
        }
        #endregion
        #region 托盘类型（空托盘/满托盘）
        /// <summary>
        /// 托盘类型（空托盘/满托盘）
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
        #region 状态（待机或故障、新建任务、开始移动、移动中、开始取货、取货中、开始放货、放货中、完成）
        /// <summary>
        /// 状态（待机或故障、新建任务、开始移动、移动中、开始取货、取货中、开始放货、放货中、完成）
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
