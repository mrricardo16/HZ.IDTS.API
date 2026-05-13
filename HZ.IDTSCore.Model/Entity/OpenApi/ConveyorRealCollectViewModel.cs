using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Model.Entity.OpenApi
{
    public class ConveyorRealCollectViewModel
    {
        /// <summary>
        /// 输送线数据项数组
        /// </summary>
        public List<Conveyor> Conveyor { get; set; }
    }

    #region 输送线数据项
    /// <summary>
    /// 输送线数据项
    /// </summary>
    public class Conveyor
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
        #region 线体编号
        /// <summary>
        /// 线体编号
        /// </summary>
        private string _lineNo = string.Empty;
        public string LineNo
        {
            get
            {
                return _lineNo;
            }
            set
            {
                if (value is null)
                {
                    _lineNo = string.Empty;
                }
                else
                {
                    _lineNo = value;
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
        #region 光电信号（true或false）
        /// <summary>
        /// 光电信号（true或false）
        /// </summary>
        private bool? _signal = false;
        public bool? Signal
        {
            get
            {
                return _signal;
            }
            set
            {
                if (value is null)
                {
                    _signal = false;
                }
                else
                {
                    _signal = value;
                }
            }
        }
        //public bool? Signal { get; set; }
        #endregion

        /// <summary>
        /// 货物列表
        /// </summary>
        public List<Goods> GoodsList { get; set; }

        #region 线体类型
        /// <summary>
        /// 线体类型(0表示线体；1表示提升机）
        /// </summary>
        private int? _equitype = 0;
        public int? Equitype
        {
            get
            {
                return _equitype;
            }
            set
            {
                if (value is null)
                {
                    _equitype = 0;
                }
                else
                {
                    _equitype = value;
                }
            }
        }
        //public int? Equitype { get; set; }
        #endregion

        #region 线体层数
        /// <summary>
        /// 线体层数
        /// </summary>
        private int? _equilayer = 0;
        public int? Equilayer
        {
            get
            {
                return _equilayer;
            }
            set
            {
                if (value is null)
                {
                    _equilayer = 0;
                }
                else
                {
                    _equilayer = value;
                }
            }
        }
        //public int? Equilayer { get; set; }
        #endregion

        #region 提升高度（单位：mm）
        /// <summary>
        /// 提升高度（单位：mm）
        /// </summary>
        private double? _equiheight = 0;
        public double? Equiheight
        {
            get
            {
                return _equiheight;
            }
            set
            {
                if (value is null)
                {
                    _equiheight = 0;
                }
                else
                {
                    _equiheight = value;
                }
            }
        }
        //public double? Equiheight { get; set; }
        #endregion
    }
    #endregion

    #region 货物数据项
    /// <summary>
    /// 货物数据项
    /// </summary>
    public class Goods
    {
        #region 任务号
        /// <summary>
        /// 任务号
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
        #region 入库、出库
        /// <summary>
        /// 入库、出库
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
        #region 货物起点
        /// <summary>
        /// 货物起点
        /// </summary>
        private string _goodsFromPos = string.Empty;
        public string GoodsFromPos
        {
            get
            {
                return _goodsFromPos;
            }
            set
            {
                if (value is null)
                {
                    _goodsFromPos = string.Empty;
                }
                else
                {
                    _goodsFromPos = value;
                }
            }
        }
        #endregion
        #region 货物终点
        /// <summary>
        /// 货物终点
        /// </summary>
        private string _goodsToPos = string.Empty;
        public string GoodsToPos
        {
            get
            {
                return _goodsToPos;
            }
            set
            {
                if (value is null)
                {
                    _goodsToPos = string.Empty;
                }
                else
                {
                    _goodsToPos = value;
                }
            }
        }
        #endregion
        #region 外形检测标记：NO：未开始检测；OK：检测通过；NG：检测不通过
        /// <summary>
        /// 外形检测标记：NO：未开始检测；OK：检测通过；NG：检测不通过
        /// </summary>
        private string _shapeCheck = string.Empty;
        public string ShapeCheck
        {
            get
            {
                return _shapeCheck;
            }
            set
            {
                if (value is null)
                {
                    _shapeCheck = string.Empty;
                }
                else
                {
                    _shapeCheck = value;
                }
            }
        }
        #endregion
        #region 外形检测不通过的信息，如超宽，超高
        /// <summary>
        /// 外形检测不通过的信息，如超宽，超高
        /// </summary>
        private string _shapeInfo = string.Empty;
        public string ShapeInfo
        {
            get
            {
                return _shapeInfo;
            }
            set
            {
                if (value is null)
                {
                    _shapeInfo = string.Empty;
                }
                else
                {
                    _shapeInfo = value;
                }
            }
        }
        #endregion
        #region 托盘扫码信息，若有则传（若是托盘，则为托盘码；若是料箱，则为料箱码）
        /// <summary>
        /// 托盘扫码信息，若有则传（若是托盘，则为托盘码；若是料箱，则为料箱码）
        /// </summary>
        private string _scancode = string.Empty;
        public string Scancode
        {
            get
            {
                return _scancode;
            }
            set
            {
                if (value is null)
                {
                    _scancode = string.Empty;
                }
                else
                {
                    _scancode = value;
                }
            }
        }
        #endregion
    }
    #endregion
}
