using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Model.Entity.OpenApi
{
    /// <summary>
    /// 货位同步Model
    /// </summary>
    public class LocationRealMonitorViewModel
    {
        //public List<StockViewModel> stock = new List<StockViewModel>();
        public List<StockViewModel> stock { get; set; }
    }

    public class StockViewModel
    {
        #region 仓库编号
        /// <summary>
        /// 仓库编号
        /// </summary>
        private string _stockCode = string.Empty;
        public string stockCode
        {
            get
            {
                return _stockCode;
            }
            set
            {
                if (value is null)
                {
                    _stockCode = string.Empty;
                }
                else
                {
                    _stockCode = value;
                }
            }
        }
        #endregion

        #region 库区编号
        /// <summary>
        /// 库区编号
        /// </summary>
        private string _areaCode = string.Empty;
        public string areaCode
        {
            get
            {
                return _areaCode;
            }
            set
            {
                if (value is null)
                {
                    _areaCode = string.Empty;
                }
                else
                {
                    _areaCode = value;
                }
            }
        }
        #endregion

        #region 货位编码
        /// <summary>
        /// 货位编码
        /// </summary>
        private string _locationCode = string.Empty;
        public string locationCode
        {
            get
            {
                return _locationCode;
            }
            set
            {
                if (value is null)
                {
                    _locationCode = string.Empty;
                }
                else
                {
                    _locationCode = value;
                }
            }
        }
        #endregion

        #region 货位类型：立库、地堆
        /// <summary>
        /// 货位类型：立库、地堆
        /// </summary>
        private string _locationType = string.Empty;
        public string locationType
        {
            get
            {
                return _locationType;
            }
            set
            {
                if (value is null)
                {
                    _locationType = string.Empty;
                }
                else
                {
                    _locationType = value;
                }
            }
        }
        #endregion

        #region 库位状态(正常、报废、预入库锁定、预出库锁定)
        /// <summary>
        /// 库位状态(正常、报废、预入库锁定、预出库锁定)
        /// </summary>
        private string _state = string.Empty;
        public string state
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

        #region 储位状态(空、满、空托盘、不满)
        /// <summary>
        /// 储位状态(空、满、空托盘、不满)
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

        /// <summary>
        /// 货位详情列表
        /// </summary>
        //public List<ItemRowViewModel> itemRow = new List<ItemRowViewModel>();
        private List<ItemRowViewModel> _itemRow = new List<ItemRowViewModel>();
        public List<ItemRowViewModel> itemRow
        {
            get
            {
                return _itemRow;
            }
            set
            {
                if (value is null)
                {
                    _itemRow = new List<ItemRowViewModel>();
                }
                else
                {
                    _itemRow = value;
                }
            }
        }
        //public List<ItemRowViewModel> itemRow { get; set; }

        /// <summary>
        /// 货物信息
        /// </summary>
        private RackInfoViewModel _rackInfo = new RackInfoViewModel();
        public RackInfoViewModel rackInfo
        {
            get
            {
                return _rackInfo;
            }
            set
            {
                if (value is null)
                {
                    _rackInfo = new RackInfoViewModel();
                }
                else
                {
                    _rackInfo = value;
                }
            }
        }
        //public RackInfoViewModel rackInfo { get; set; }
    }

    public class ItemRowViewModel
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

    /// <summary>
    /// 判断是否缓存数据类
    /// </summary>
    public class DetermineSynchronizeModel
    {
        /// <summary>
        /// 货位编码唯一标识
        /// </summary>
        public string goodscommandGuid { get; set; }
        /// <summary>
        /// 货位信息
        /// </summary>
        public StockViewModel stockView { get; set; }
    }

    ///// <summary>
    ///// 仓库信息（不含仓库信息）
    ///// </summary>
    //public class StockNoDetail
    //{
    //    /// <summary>
    //    /// 货位设备唯一标识
    //    /// </summary>
    //    public string Goodsequipmentguid { get; set; }
    //    /// <summary>
    //    /// 货位唯一标识
    //    /// </summary>
    //    public string Stockguid { get; set; }
    //    /// <summary>
    //    /// 仓库编号
    //    /// </summary>
    //    public string StockCode { get; set; }
    //    /// <summary>
    //    /// 库区编号
    //    /// </summary>
    //    public string AreaCode { get; set; }
    //    /// <summary>
    //    /// 货位编码
    //    /// </summary>
    //    public string LocationCode { get; set; }
    //    /// <summary>
    //    /// 货位类型：立库、地堆
    //    /// </summary>
    //    public string LocationType { get; set; }
    //    /// <summary>
    //    /// 库位状态（正常、报废、预入库锁定、预出库锁定）
    //    /// </summary>
    //    public string State { get; set; }
    //    /// <summary>
    //    /// 储位状态（空、满、空托盘、不满）
    //    /// </summary>
    //    public string StorageState { get; set; }
    //}
}
