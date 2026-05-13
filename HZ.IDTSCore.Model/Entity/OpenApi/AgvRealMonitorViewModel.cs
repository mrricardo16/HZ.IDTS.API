using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Model.Entity.OpenApi
{
    /// <summary>
    /// AGV状态接口
    /// </summary>
    public class AgvRealMonitorViewModel
    {
        public List<AgvModel> Agv = new List<AgvModel>();
    }

    public class AgvModel
    {
        #region AGV编号
        /// <summary>
        /// AGV编号
        /// </summary>
        private string _carCode = string.Empty;
        public string carCode
        {
            get
            {
                return _carCode;
            }
            set
            {
                if (value is null)
                {
                    _carCode = string.Empty;
                }
                else
                {
                    _carCode = value;
                }
            }
        }
        #endregion

        #region 设备名称
        /// <summary>
        /// 设备名称
        /// </summary>
        private string _carName = string.Empty;
        public string carName
        {
            get
            {
                return _carName;
            }
            set
            {
                if (value is null)
                {
                    _carName = string.Empty;
                }
                else
                {
                    _carName = value;
                }
            }
        }
        #endregion

        #region 设备的运行状态，需要提供定义（执行状态：运行中、取货中、卸货中、故障、充电、空闲）
        /// <summary>
        /// 设备的运行状态，需要提供定义（执行状态：运行中、取货中、卸货中、故障、充电、空闲）
        /// </summary>
        private string _carState = string.Empty;
        public string carState
        {
            get
            {
                return _carState;
            }
            set
            {
                if (value is null)
                {
                    _carState = string.Empty;
                }
                else
                {
                    _carState = value;
                }
            }
        }
        #endregion

        #region 在线状态(在线、离线)
        /// <summary>
        /// 在线状态(在线、离线)
        /// </summary>
        private string _onlineState = string.Empty;
        public string onlineState
        {
            get
            {
                return _onlineState;
            }
            set
            {
                if (value is null)
                {
                    _onlineState = string.Empty;
                }
                else
                {
                    _onlineState = value;
                }
            }
        }
        #endregion

        #region 异常信息
        /// <summary>
        /// 异常信息
        /// </summary>
        private string _errMsg = string.Empty;
        public string errMsg
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

        #region 设备的当前角度
        /// <summary>
        /// 设备的当前角度
        /// </summary>
        private string _angle = string.Empty;
        public string angle
        {
            get
            {
                return _angle;
            }
            set
            {
                if (value is null)
                {
                    _angle = string.Empty;
                }
                else
                {
                    _angle = value;
                }
            }
        }
        #endregion

        #region 设备位置x坐标
        /// <summary>
        /// 设备位置x坐标
        /// </summary>
        private string _x = string.Empty;
        public string x
        {
            get
            {
                return _x;
            }
            set
            {
                if (value is null)
                {
                    _x = string.Empty;
                }
                else
                {
                    _x = value;
                }
            }
        }
        #endregion

        #region 设备位置y坐标
        /// <summary>
        /// 设备位置y坐标
        /// </summary>
        private string _y = string.Empty;
        public string y
        {
            get
            {
                return _y;
            }
            set
            {
                if (value is null)
                {
                    _y = string.Empty;
                }
                else
                {
                    _y = value;
                }
            }
        }
        #endregion

        #region 速度  mm/s
        /// <summary>
        /// 速度  mm/s
        /// </summary>
        private string _speed = string.Empty;
        public string speed
        {
            get
            {
                return _speed;
            }
            set
            {
                if (value is null)
                {
                    _speed = string.Empty;
                }
                else
                {
                    _speed = value;
                }
            }
        }
        #endregion

        #region 举升高度 mm
        /// <summary>
        /// 举升高度 mm
        /// </summary>
        private string _liftHeight = string.Empty;
        public string liftHeight
        {
            get
            {
                return _liftHeight;
            }
            set
            {
                if (value is null)
                {
                    _liftHeight = string.Empty;
                }
                else
                {
                    _liftHeight = value;
                }
            }
        }
        #endregion

        #region 载货状态  0:放下 1:举升
        /// <summary>
        /// 载货状态  0:放下 1:举升
        /// </summary>
        private string _loadStatus = string.Empty;
        public string loadStatus
        {
            get
            {
                return _loadStatus;
            }
            set
            {
                if (value is null)
                {
                    _loadStatus = string.Empty;
                }
                else
                {
                    _loadStatus = value;
                }
            }
        }
        #endregion


        #region 电量
        /// <summary>
        /// 电量
        /// </summary>
        private string _power = string.Empty;
        public string power
        {
            get
            {
                return _power;
            }
            set
            {
                if (value is null)
                {
                    _power = string.Empty;
                }
                else
                {
                    _power = value;
                }
            }
        }
        #endregion

        #region 是否有货物
        /// <summary>
        /// 是否有货物
        /// </summary>
        private int _hasGoods = 0;
        public int hasGoods
        {
            get
            {
                return _hasGoods;
            }
            set
            {
                _hasGoods = value;
            }
        }
        #endregion

        /// <summary>
        /// 货物信息
        /// </summary>
        public GoodsInfoViewModel goodsInfo = new GoodsInfoViewModel();

        #region 托盘/料架标记（0表示托盘，1表示料架，若是0看hasGoods和goodsInfo;若是1看rackInfo）
        /// <summary>
        /// 托盘/料架标记（0表示托盘，1表示料架，若是0看hasGoods和goodsInfo;若是1看rackInfo）
        /// </summary>
        private int _cargoType = 0;
        public int cargoType
        {
            get
            {
                return _cargoType;
            }
            set
            {
                _cargoType = value;
            }
        }
        #endregion

        /// <summary>
        /// 货物信息
        /// </summary>
        public RackInfoViewModel rackInfo = new RackInfoViewModel();
    }

    public class GoodsInfoViewModel
    {
        #region 任务号
        /// <summary>
        /// 任务号
        /// </summary>
        private string _taskNo = string.Empty;
        public string taskNo
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

        #region 起点
        /// <summary>
        /// 起点
        /// </summary>
        private string _startBit = string.Empty;
        public string startBit
        {
            get
            {
                return _startBit;
            }
            set
            {
                if (value is null)
                {
                    _startBit = string.Empty;
                }
                else
                {
                    _startBit = value;
                }
            }
        }
        #endregion

        #region 终点
        /// <summary>
        ///  终点
        /// </summary>
        private string _endBit = string.Empty;
        public string endBit
        {
            get
            {
                return _endBit;
            }
            set
            {
                if (value is null)
                {
                    _endBit = string.Empty;
                }
                else
                {
                    _endBit = value;
                }
            }
        }
        #endregion

        #region 搬运的货位内容描述
        /// <summary>
        /// 搬运的货位内容描述
        /// </summary>
        private string _content = string.Empty;
        public string content
        {
            get
            {
                return _content;
            }
            set
            {
                if (value is null)
                {
                    _content = string.Empty;
                }
                else
                {
                    _content = value;
                }
            }
        }
        #endregion

        #region 创建时间
        /// <summary>
        /// 创建时间
        /// </summary>
        private string _createTime = string.Empty;
        public string createTime
        {
            get
            {
                return _createTime;
            }
            set
            {
                if (value is null)
                {
                    _createTime = string.Empty;
                }
                else
                {
                    _createTime = value;
                }
            }
        }
        #endregion
    }

    public class RackInfoViewModel
    {
        #region 备注信息
        /// <summary>
        /// 备注信息
        /// </summary>
        private string _remarks = string.Empty;
        public string remarks
        {
            get
            {
                return _remarks;
            }
            set
            {
                if (value is null)
                {
                    _remarks = string.Empty;
                }
                else
                {
                    _remarks = value;
                }
            }
        }
        #endregion

        #region 料架编号
        /// <summary>
        /// 料架编号
        /// </summary>
        private string _rackCode = string.Empty;
        public string rackCode
        {
            get
            {
                return _rackCode;
            }
            set
            {
                if (value is null)
                {
                    _rackCode = string.Empty;
                }
                else
                {
                    _rackCode = value;
                }
            }
        }
        #endregion

        #region 料架角度 0|90|180|360
        /// <summary>
        /// 料架角度 0|90|180|360
        /// </summary>
        private string _rackAngle = string.Empty;
        public string rackAngle
        {
            get
            {
                return _rackAngle;
            }
            set
            {
                if (value is null)
                {
                    _rackAngle = string.Empty;
                }
                else
                {
                    _rackAngle = value;
                }
            }
        }
        #endregion

        #region 拓展字段1
        /// <summary>
        /// 拓展字段1
        /// </summary>
        private string _ext1 = string.Empty;
        public string ext1
        {
            get
            {
                return _ext1;
            }
            set
            {
                if (value is null)
                {
                    _ext1 = string.Empty;
                }
                else
                {
                    _ext1 = value;
                }
            }
        }
        #endregion

        #region 拓展字段2
        /// <summary>
        /// 拓展字段2
        /// </summary>
        private string _ext2 = string.Empty;
        public string ext2
        {
            get
            {
                return _ext2;
            }
            set
            {
                if (value is null)
                {
                    _ext2 = string.Empty;
                }
                else
                {
                    _ext2 = value;
                }
            }
        }
        #endregion

        /// <summary>
        /// 料箱信息
        /// </summary>
        public List<BoxInfoViewModel> boxInfo = new List<BoxInfoViewModel>();
    }

    public class BoxInfoViewModel
    {
        #region 料箱编号
        /// <summary>
        /// 料箱编号
        /// </summary>
        private string _boxCode = string.Empty;
        public string boxCode
        {
            get
            {
                return _boxCode;
            }
            set
            {
                if (value is null)
                {
                    _boxCode = string.Empty;
                }
                else
                {
                    _boxCode = value;
                }
            }
        }
        #endregion

        #region 料箱位置-排-列-层
        /// <summary>
        /// 料箱位置-排-列-层
        /// </summary>
        private string _boxLocation = string.Empty;
        public string boxLocation
        {
            get
            {
                return _boxLocation;
            }
            set
            {
                if (value is null)
                {
                    _boxLocation = string.Empty;
                }
                else
                {
                    _boxLocation = value;
                }
            }
        }
        #endregion

        #region 储位状态（空、空箱、满箱）
        /// <summary>
        /// 储位状态（空、空箱、满箱）
        /// </summary>
        private string _storageState = string.Empty;
        public string storageState
        {
            get
            {
                return _storageState;
            }
            set
            {
                if (value is null)
                {
                    _storageState = string.Empty;
                }
                else
                {
                    _storageState = value;
                }
            }
        }
        #endregion

        public List<ItemRowRack> itemRow = new List<ItemRowRack>();
    }

    /// <summary>
    /// 料架物料信息项
    /// </summary>
    public class ItemRowRack
    {
        #region 物料编码
        /// <summary>
        /// 物料编码
        /// </summary>
        private string _itemCode = string.Empty;
        public string itemCode
        {
            get
            {
                return _itemCode;
            }
            set
            {
                if (value is null)
                {
                    _itemCode = string.Empty;
                }
                else
                {
                    _itemCode = value;
                }
            }
        }
        #endregion

        #region 物料名称
        /// <summary>
        /// 物料名称
        /// </summary>
        private string _itemName = string.Empty;
        public string itemName
        {
            get
            {
                return _itemName;
            }
            set
            {
                if (value is null)
                {
                    _itemName = string.Empty;
                }
                else
                {
                    _itemName = value;
                }
            }
        }
        #endregion

        #region 数量
        /// <summary>
        /// 数量
        /// </summary>
        private string _qty = string.Empty;
        public string Qty
        {
            get
            {
                return _qty;
            }
            set
            {
                if (value is null)
                {
                    _qty = string.Empty;
                }
                else
                {
                    _qty = value;
                }
            }
        }
        #endregion

        #region 批次
        /// <summary>
        /// 批次
        /// </summary>
        private string _bacthNo = string.Empty;
        public string bacthNo
        {
            get
            {
                return _bacthNo;
            }
            set
            {
                if (value is null)
                {
                    _bacthNo = string.Empty;
                }
                else
                {
                    _bacthNo = value;
                }
            }
        }
        #endregion

        #region 拓展字段1
        /// <summary>
        /// 拓展字段1
        /// </summary>
        private string _ext1 = string.Empty;
        public string ext1
        {
            get
            {
                return _ext1;
            }
            set
            {
                if (value is null)
                {
                    _ext1 = string.Empty;
                }
                else
                {
                    _ext1 = value;
                }
            }
        }
        #endregion

        #region 拓展字段2
        /// <summary>
        /// 拓展字段2
        /// </summary>
        private string _ext2 = string.Empty;
        public string ext2
        {
            get
            {
                return _ext2;
            }
            set
            {
                if (value is null)
                {
                    _ext2 = string.Empty;
                }
                else
                {
                    _ext2 = value;
                }
            }
        }
        #endregion

        /// <summary>
        /// (货位、托盘、料箱）物料拓展信息项
        /// </summary>
        public class ItemRowPlus
        {
            #region 备注信息
            /// <summary>
            /// 备注信息
            /// </summary>
            private string _remarks = string.Empty;
            public string remarks
            {
                get
                {
                    return _remarks;
                }
                set
                {
                    if (value is null)
                    {
                        _remarks = string.Empty;
                    }
                    else
                    {
                        _remarks = value;
                    }
                }
            }
            #endregion

            #region 物料编码
            /// <summary>
            /// 物料编码
            /// </summary>
            private string _itemCode = string.Empty;
            public string itemCode
            {
                get
                {
                    return _itemCode;
                }
                set
                {
                    if (value is null)
                    {
                        _itemCode = string.Empty;
                    }
                    else
                    {
                        _itemCode = value;
                    }
                }
            }
            #endregion

            #region 物料名称
            /// <summary>
            /// 物料名称
            /// </summary>
            private string _itemName = string.Empty;
            public string itemName
            {
                get
                {
                    return _itemName;
                }
                set
                {
                    if (value is null)
                    {
                        _itemName = string.Empty;
                    }
                    else
                    {
                        _itemName = value;
                    }
                }
            }
            #endregion

            #region 托盘码
            /// <summary>
            /// 托盘码
            /// </summary>
            private string _trayCode = string.Empty;
            public string trayCode
            {
                get
                {
                    return _trayCode;
                }
                set
                {
                    if (value is null)
                    {
                        _trayCode = string.Empty;
                    }
                    else
                    {
                        _trayCode = value;
                    }
                }
            }
            #endregion

            #region 数量
            /// <summary>
            /// 数量
            /// </summary>
            private double? _qty = 0;
            public double? qty
            {
                get
                {
                    return _qty;
                }
                set
                {
                    if (value is null)
                    {
                        _qty = 0;
                    }
                    else
                    {
                        _qty = value;
                    }
                }
            }
            #endregion

            #region 批次号
            /// <summary>
            /// 批次号
            /// </summary>
            private string _bacthNo = string.Empty;
            public string bacthNo
            {
                get
                {
                    return _bacthNo;
                }
                set
                {
                    if (value is null)
                    {
                        _bacthNo = string.Empty;
                    }
                    else
                    {
                        _bacthNo = value;
                    }
                }
            }
            #endregion

            #region 拓展字段1
            /// <summary>
            /// 拓展字段1
            /// </summary>
            private string _ext1 = string.Empty;
            public string ext1
            {
                get
                {
                    return _ext1;
                }
                set
                {
                    if (value is null)
                    {
                        _ext1 = string.Empty;
                    }
                    else
                    {
                        _ext1 = value;
                    }
                }
            }
            #endregion

            #region 拓展字段2
            /// <summary>
            /// 拓展字段2
            /// </summary>
            private string _ext2 = string.Empty;
            public string ext2
            {
                get
                {
                    return _ext2;
                }
                set
                {
                    if (value is null)
                    {
                        _ext2 = string.Empty;
                    }
                    else
                    {
                        _ext2 = value;
                    }
                }
            }
            #endregion
        }
    }

}
