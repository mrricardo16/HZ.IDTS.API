using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace HZ.IDTSCore.Common.Const
{
    public class SysKeyword
    {
        public const string OrderType_OutOrder = "出库订单号";
        public const string OrderType_OutAdviceOrder = "出库通知单";
        public const string OrderType_OutPlan = "出库计划单";
        public const string OrderType_InOrder = "入库订单";
        public const string OrderType_InArrival = "到货单";
        public const string OrderType_InInventory = "入库单";
        public const string OrderType_OutInventory = "出库单";
        public const string OrderType_Sorting = "分拣单";
        public const string OrderType_PickupOrder = "拣货单";
        public const string OrderType_CheckOrder = "盘点单";
        public const string OrderType_Cpfr = "补货单";

        #region 业务类型

        public const string OpType_PurchaseInFail = "报废入库";

        public const string OpType_PurchaseOutFail = "报废出库";

        public const string OpType_PurchaseInInventory = "采购入库";

        public const string OpType_FastOutInventory = "快速出库单";
        public const string OpType_CheckOutInventory = "盘亏出库";
        public const string OpType_CheckInInventory = "盘盈入库";
        public const string OpType_InitInInventory = "初始化入库";

        public const string OpType_InitOutInventory = "初始化出库";

        public const string OpType_TallyInInventory = "理货入库";

        public const string OpType_TallyOutInventory = "理货出库";

        public const string OpType_CancelOutInventory = "取消入库";


        public const string OpType_OutProduce = "生产领料";

        public const string OpType_OutMaunal = "人工请领";

        public const string OpType_WeiWai = "委外领料";

        /// <summary>
        /// 领配料请领
        /// </summary>
        public const string OpType_Out_MixedPicking = "领配料请领"; 


        public const string OpType_OutPurchaseReturn = "采购退货";

        /// <summary>
        /// 销售出库
        /// </summary>
        public const string OpType_OutSale = "销售出库";

        /// <summary>
        /// 生产领料
        /// </summary>
        public const string OpType_Out_Produce = "生产领料";

        /// <summary>
        /// 非生产领料
        /// </summary>
        public const string OpType_OutNotProduce = "非生产领料";

        /// <summary>
        /// 领配料
        /// </summary>
        public const string OpType_Out_Picking = "领配料";

        /// <summary>
        /// 派工领料
        /// </summary>
        public const string OpType_Out_PgPicking = "派工领料";

        /// <summary>
        /// 充放电领料
        /// </summary>
        public const string OpType_Out_ChangePicking = "充放电领料";

        /// <summary>
        /// 采购退货
        /// </summary>
        public const string OpType_Out_PurchaseReturn = "采购退货";

        /// <summary>
        /// 销售出库
        /// </summary>
        public const string OpType_Out_Sale = "销售出库";

        /// <summary>
        /// 调拨出库 - 一步法
        /// </summary>
        public const string OpType_Out_Move = "调拨出库";

        /// <summary>
        /// 2期-领料出库 
        /// </summary>
        public const string OpType_Picking_Out = "领料出库";

        /// <summary>
        /// 2期-领料退库
        /// </summary>
        public const string OpType_H_Picking_Out_Return = "领料退库";

        /// <summary>
        /// 2期-成品入库
        /// </summary>
        public const string OpType_H_In_Produce = "成品入库2";

        /// <summary>
        /// 2期-成品返工
        /// </summary>
        public const string OpType_H_In_Produce_Return = "成品返工";

        /// <summary>
        /// 2期-盘盈入库
        /// </summary>
        public const string OpType_H_Check_More = "盘盈入库";

        /// <summary>
        /// 2期-盘亏出库
        /// </summary>
        public const string OpType_H_Check_Less = "盘亏出库";

        /// <summary>
        /// 2期-非生产领用
        /// </summary>
        public const string OpType_H_UN_PRODUCE_PICK = "非生产领用";

        /// <summary>
        /// 2期-两步法-UB调拨出库
        /// </summary>
        public const string OpType_H_Out_Move = "UB调拨出库";

        /// <summary>
        /// 2期-两步法-UB调拨入库
        /// </summary>
        public const string OpType_H_In_Move = "UB调拨入库";

        /// <summary>
        /// 2期-客供料入库
        /// </summary>
        public const string OpType_H_In_Customer = "客供料入库";

        public const string OpType_PurchaseInArrival = "采购到货";

        public const string OpType_PurchaseInArrival_TB = "TB采购到货";

        public const string OpType_In_Produce = "成品入库";

        public const string OpType_In_ProduceCreate = "生产入库";
        
        public const string OpType_In_WeiWai = "委外入库";

        public const string OpType_In_ProduceLineReturn = "产线退库";

        public const string OpType_In_CarLineReturn = "车间退库";

        public const string OpType_PurchaseInReturnInventory = "采购收货冲销";


        #endregion

        #region 业务分类

        public const string OpClass_OtherInInventory = "初始化入库";
        #endregion

        #region 单据状态

        #region 波次

        public const string WaveState_WaitPick = "待分拣";
        public const string WaveState_Pickuped = "已分拣";

        #endregion

        #region 出库订单
        public const string OutOrderState_New = "新建"; //DUMPTRANSACTION 库名 WITH NO_LOG
        public const string OutOrderState_Submit = "已提交";
        public const string OutOrderState_Review = "已审核";
        public const string OutOrderState_Reject = "已驳回";
        public const string OutOrderState_Complated = "已完成";
        #endregion

        #region 出库通知单
        public const string OutAdviceState_New = "新建"; //DUMPTRANSACTION 库名 WITH NO_LOG
        public const string OutAdviceState_Submit = "已提交";
        public const string OutAdviceState_Review = "已审核";
        public const string OutAdviceState_Reject = "已驳回";
        public const string OutAdviceState_Complated = "已完成";
        public const string OutAdviceState_Pickuped = "已分拣";
        public const string OutAdviceState_LackItem = "缺料";
        public const string OutAdviceState_WriteOffed = "已冲销";
        #endregion

        #region 出库单
        public const string OutInventoryState_New = "新建";
        public const string OutInventoryState_Submit = "已提交";
        public const string OutInventoryState_Review = "已审核";
        public const string OutInventoryState_Reject = "已驳回";
        public const string OutInventoryState_Complated = "已完成";
        public const string OutInventoryState_Handle = "已处理";
        #endregion

        #region 出库计划
        public const string OutPlanState_New = "新建";
        public const string OutPlanState_Submit = "已提交";
        public const string OutPlanState_Review = "已审核";
        public const string OutPlanState_Reject = "已驳回";
        public const string OutPlanState_Complated = "已完成";
        #endregion

        #region 入库单
        public const string State_New = "新建"; //DUMPTRANSACTION 库名 WITH NO_LOG
        public const string State_Submit = "已提交";
        public const string State_WaitCpfr = "待补货";
        public const string State_Review = "已审核";
        public const string State_Reject = "已驳回";
        public const string State_Complated = "已完成";
        public const string State_Handle = "已处理";
        #endregion

        #region 入库订单
        public const string InOrderState_New = "新建";
        public const string InOrderState_Submit = "已提交";
        public const string InOrderState_Review = "已审核";
        public const string InOrderState_Reject = "已驳回";
        public const string InOrderState_Complated = "已完成";
        #endregion

        #region 到货单
        public const string InArrivalState_New = "新建";
        public const string InArrivalState_Submit = "已提交";
        public const string InArrivalState_Review = "已审核";
        public const string InArrivalState_Reject = "已驳回";
        public const string InArrivalState_Complated = "已完成";
        public const string InArrivalState_Delete = "已冻结";
        public const string InArrivalState_Cancel = "已取消";
        #endregion

        #region 盘点单
        public const string CheckState_New = "新建";
        public const string CheckState_Submit = "已提交";
        public const string CheckState_Checked = "已盘点";
        public const string CheckState_Complated = "已完成";
        public const string CheckState_Cancel = "已取消";
        #endregion

        #region 盘点库存调整方式

        public const string CheckMode_Sure = "盈亏确认";

        #endregion

        #region 调拨单
        public const string TransferState_New = "新建";
        public const string TransferState_Submit = "已提交";
        public const string TransferState_Review = "已审核";
        public const string TransferState_Reject = "已驳回";
        public const string TransferState_Complated = "已完成";
        #endregion

        #region 上架单
        public const string UpLocationState_New = "新建";
        public const string UpLocationState_Complate = "已完成";
        public const string UpLocationState_Submit = "已提交";
        #endregion
        #endregion

        #region 分拣单
        public const string SortingState_New = "新建";
        public const string SortingState_Wait= "待分拣";
        public const string SortingState_Executeing = "分拣中";
        public const string SortingState_SortEnd = "已分拣";
        public const string SortingState_LackItem = "缺料";
        public const string SortingState_Complate = "已完成";
        #endregion

        #region 分拣指引状态

        public const string SortingGuideState_Wait = "待拣";
        public const string SortingGuideState_Picking = "拣货中";
        public const string SortingGuideState_Complete = "已拣";

        #endregion

        #region 编码规则

        public const string Rule_LotcodeNo = "批次号";
        public const string Rule_SortingNo = "分拣单号";
        public const string Rule_WaveNo = "波次号";
        public const string Rule_ArrivalNo = "到货单号";
        public const string Rule_InInventoryNo = "入库单号";
        public const string Rule_CpfrNo = "补货单号";
        public const string Rule_OutInventoryNo = "出库单号";
        public const string Rule_OutAdviceNo = "出库通知单";
        public const string Rule_UpLocationNo = "上架单号";

        public const string Rule_PickUpNo = "拣货单号";
        public const string Rule_TaskNo = "任务号";
        public const string Rule_TrayNum = "托盘";
        public const string Rule_CheckNo = "盘点单号";
        public const string Rule_TransferNo = "调拨单号";
        public const string Rule_OutOrderNo = "出库订单号";

        public const string Rule_QuestionSubmitNo = "反馈单号";


        public const string Rule_ArrivalLot = "到货批次";

        #endregion

        #region 业务类型
        //public const string OpType_ = "生产越库-入"; 
        #endregion


        public const string VirtualTray = "虚拟托盘";
        public const string VirtualInLot = "虚拟批次";

        #region 保养类型
        public const string MaintainType_Timing = "定期保养";
        public const string MaintainType_Already = "已保养";
        #endregion

        /// <summary>
        /// 虚拟库
        /// </summary>
        //public const string StockTypeVirtual = "虚拟库";

        #region 出库生效节点
        public const string OutNode_Pickuped = "拣货完成";
        public const string OutNode_Checked = "复验完成";
        public const string OutNode_Packinged = "包装完成";
        public const string OutNode_OQCed = "OQC完成";
        #endregion

        #region 库区结构

        /// <summary>
        /// 两头单向穿梭车
        /// </summary>
        public const string StockStructure_TwoPortSingleWay = "两头单向穿梭车";
        public const string StockStructure_FlatStorage = "平库";
        public const string StockStructure_SolidStorage = "立库";
        public const string StockStructure_BlackBox = "黑盒";

        //立库、人工库、地堆、流利式货架、单头单向穿梭车、两头单向穿梭车、单头四向穿梭车、

        #endregion

        #region 仓库类型

        public const string StockType_Actual = "实体仓";

        public const string StockType_Virtual = "虚拟仓";

        #endregion

        #region 补货类型

        public const string CpfrType_Gen = "常规补货";
        public const string CpfrType_Black = "黑仓补货";
        #endregion

        #region 货位使用状态
        public const string LocationUserState_Empty = "空";
        public const string LocationUserState_Full = "满";


        public const string LocationState_ReInLock = "预入库锁定";
        public const string LocationState_ReOutLock = "预出库锁定";
        public const string LocationState_Normal = "正常";
        public const string LocationState_Breakdown = "故障";
        #endregion

        #region 上架类型
        public const string UpLocationType_InUp = "入库上架";
        public const string UpLocationType_PickUpSurplusUp = "拣货尾盘上架";

        #endregion

        #region 系统策略
        /// <summary>
        /// 默认入库货位
        /// </summary>
        public const string SysSet_DefaultInStorageBit = "DefaultInStorageBit";
        /// <summary>
        /// 组盘自动获取推荐上架货位
        /// </summary>
        public const string SysSet_AutoGainRecommendUpBit = "AutoGainRecommendUpBit";
        /// <summary>
        /// 默认上架库区
        /// </summary>
        public const string SysSet_DefaultUpArea = "DefaultUpArea";
        /// <summary>
        /// 拣货区
        /// </summary>
        public const string SysSet_DefaultPickUpArea = "DefaultPickUpArea";
        /// <summary>
        /// 默认拣货货位
        /// </summary>
        public const string SysSet_DefaultPickUpBit = "DefaultPickUpBit";
        /// <summary>
        /// 下架即拣货
        /// </summary>
        public const string SysSet_DownBitPickupComplate = "DownBitPickupComplate";
        /// <summary>
        /// 拣货完成自动出库
        /// </summary>
        public const string SysSet_PickupComplateAutoOut = "PickupComplateAutoOut";

        /// <summary>
        /// 是否启用多货主
        /// </summary>
        public const string SysSet_EnableMultiOwner = "EnableMultiOwner";

        /// <summary>
        /// 仓库是否只允许一个正在执行的分拣单
        /// </summary>
        public const string SysSet_OnlySingleSorting = "OnlySingleSorting";

        /// <summary>
        /// 审核金额限制
        /// </summary>
        public const string SysSet_ReCheckMiddleMoney = "ReCheckMiddleMoney";

        /// <summary>
        /// 虚拟库默认出库
        /// </summary>
        public const string SysSet_VirtualStockDefaultOut = "VirtualStockDefaultOut";

        /// <summary>
        /// 收货区
        /// </summary>
        public const string SysSet_ReceivingArea = "ReceivingArea";
        /// <summary>
        /// 是否启用流程审批
        /// </summary>
        public const string SysSet_EnableFlowReCheck = "EnableFlowReCheck";

        /// <summary>
        /// 是否启用仓库物料
        /// </summary>
        public const string SysSet_EnableStockItem = "EnableStockItem";
        /// <summary>
        /// 自动创建拣货单(一单一拣)
        /// </summary>
        public const string SysSet_AutoCreateSorting = "AutoCreateSorting";
        /// <summary>
        /// 自动执行分拣单
        /// </summary>
        public const string SysSet_AutoExecuteSorting = "AutoExecuteSorting";
        /// <summary>
        /// 是否允许批次混放
        /// </summary>
        public const string SysSet_IsMixedLotNo = "IsControlMixedLotNo";
        /// <summary>
        /// 是否允许物料状态混放
        /// </summary>
        public const string SysSet_IsMixedItemState = "IsControlMixedItemState";

        /// <summary>
        /// 根据存放库区自动补货
        /// </summary>
        public const string SysSet_AutoCpfrByItemArea = "AutoCpfrByItemArea";

        /// <summary>
        /// 月结存时间
        /// </summary>
        public const string SysSet_MonthBalanceDay = "MonthBalanceDay";

        /// <summary>
        /// 盘点库存调整方式
        /// </summary>
        public const string SysSet_CheckQtyResetType = "CheckQtyResetType";

        /// <summary>
        /// 允许超入
        /// </summary>
        public const string SysSet_AbleOverIn = "AbleOverIn";
        /// <summary>
        /// 允许超发
        /// </summary>
        public const string SysSet_AllowOverOut = "AllowOverOut";

        #endregion

        #region Queue队列项目
        public static string QueueItem_ComplatedBuss = SysConst.ProjectCode+"_ComplatedBuss";
        public static string QueueItem_TakeeffectOrder = SysConst.ProjectCode + "_TakeeffectOrder";
        public static string QueueItem_InterfaceReq = SysConst.ProjectCode + "_InterfaceReq";
        public static string QueueItem_CmdTask = SysConst.ProjectCode + "_CmdTask";
        #endregion

        public enum OrderType
        {
            [Description("入库单")]
            InInventory,
            [Description("到货单")]
            Arrival,
            [Description("入库订单")]
            InOrder,
            [Description("检验单")]
            Inspect,
            [Description("出库订单")]
            OutOrder,
            [Description("发货通知单")]
            OutAdvice,
            [Description("出库单")]
            OutInventory,
            [Description("盘点单")]
            Check
        }

        #region 任务回报状态

        public enum ExecuteStateEnum
        {
            [Description("待执行")]
            UnExecute = 0,

            [Description("待下发")]
            UnSend = 10,

            /// <summary>
            /// 执行中
            /// </summary>
            [Description("执行中")]
            DoExecute = 1,

            [Description("完成")]
            Complete = 2,

            [Description("开始取货")]
            PickStart = 3,

            [Description("取货完成")]
            PickComplete = 4,

            [Description("开始卸货")]
            UnloadingStart = 5,

            [Description("卸货完成")]
            UnloadingComplete = 6,

            [Description("取消")]
            Cancel = 7,

            [Description("失败")]
            Error = 11,

            [Description("空取")]
            Empty = 12
        }   // EnumHelper.GetEnumDescription(executeState);

        #endregion

        #region 任务正常回报状态流程

        public enum ExecuteCompleteStateEnum
        {
            /// <summary>
            /// 执行中
            /// </summary>
            [Description("执行中")]
            DoExecute = 1,

            /// <summary>
            /// 取货完成
            /// </summary>
            [Description("取货完成")]
            PickComplete = 2,

            /// <summary>
            /// 卸货完成
            /// </summary>
            [Description("卸货完成")]
            UnloadingComplete = 4,

            /// <summary>
            /// 完成
            /// </summary>
            [Description("完成")]
            Complete = 8

        }

        #endregion
        public enum TaskEnum
        {
            未执行 = 0,
            开始 = 1,
            开始取货 = 3,
            取货完成 = 4,
            开始卸货 = 5,
            卸货完成 = 6,
            完成 = 2,
            取消 = 7
        }

        #region 任务类型

        /// <summary>
        /// 入库
        /// </summary>
        public const string TaskType_In = "入库";

        /// <summary>
        /// 出库
        /// </summary>
        public const string TaskType_Out = "出库";

        /// <summary>
        /// 空托入库
        /// </summary>
        public const string TaskType_EmptyIn = "空托入库";

        /// <summary>
        /// 空托出库
        /// </summary>
        public const string TaskType_EmptyOut = "空托出库";

        #endregion

        #region 库区 出入库锁定状态

        public enum AreaLockStateEnum
        {
            /// <summary>
            /// 执行中
            /// </summary>
            [Description("正常")]
            Normal = 0,

            /// <summary>
            /// 取货完成
            /// </summary>
            [Description("入库锁")]
            LockIn = 1,

            /// <summary>
            /// 卸货完成
            /// </summary>
            [Description("出库锁")]
            LockOut = 2

        }

        #endregion

        #region 出入库策略
        /// <summary>
        /// 补料策略，找空托
        /// </summary>
        public const string SysAI_Replenish_FindEmpty = "Replenish_FindEmpty";
        /// <summary>
        /// 补料策略，找不满
        /// </summary>
        public const string SysAI_Replenish_FindNotFull = "Replenish_FindNotFull";

        /// <summary>
        /// 排从大到小
        /// </summary>
        public const string SysAI_In_RowMaxTrendMin = "In_RowMaxTrendMin";
        public const string SysAI_In_RowMinTrendMax = "In_RowMinTrendMax";


        public const string SysAI_In_FloorMaxTrendMin = "In_FloorMaxTrendMin";
        public const string SysAI_In_FloorMinTrendMax = "In_FloorMinTrendMax";


        public const string SysAI_In_ColMaxTrendMin = "In_ColMaxTrendMin";
        public const string SysAI_In_ColMinTrendMax = "In_ColMinTrendMax";


        public const string SysAI_Out_InLot_FIFO = "Out_InLot_FIFO";
        public const string SysAI_Out_CtlLot_FIFO = "Out_CtlLot_FIFO";

        public const string SysAI_Out_RowMaxTrendMin = "Out_RowMaxTrendMin";
        public const string SysAI_Out_RowMinTrendMax = "Out_RowMinTrendMax";


        public const string SysAI_Out_FloorMaxTrendMin = "Out_FloorMaxTrendMin";
        public const string SysAI_Out_FloorMinTrendMax = "Out_FloorMinTrendMax";


        public const string SysAI_Out_ColMaxTrendMin = "Out_ColMaxTrendMin";
        public const string SysAI_Out_ColMinTrendMax = "Out_ColMinTrendMax";
        #endregion

        #region 贵阳料箱亮灯系统

        public const string OP_TYPE_IN = "入库";

        public const string OP_TYPE_BU_ZHUANG_IN = "部装后";

        public const string OP_TYPE_OUT = "出库";

        public const string OP_TYPE_IN_SPECIAL = "推研后入库";

        public const string OP_TYPE_OUT_SPECIAL = "推研前出库";


        public const string OP_SPECIAL_STATE_HANDLE = "已处理";


        #endregion

        #region 库区类型 -自动库/人工库

        public static string AREA_MODE_AUTO = "自动";

        public static string AREA_MODE_MANUAL = "人工";

        #endregion

        #region 货位标识

        public static string LOCATION_FLAG_TEMP_STORAGE = "暂存位";

        public static string LOCATION_FLAG_WORK_BIT = "作业位";

        public static string LOCATION_FLAG_MOVE_BIT = "移动货位";


        //public static string LOCATION_FLAG_MOVE_BIT = "移动货位";
        //{ "货位标识",new List<string> {"暂存货位","拣货货位","移动货位","备货货位","虚拟货位","作业位" }

        #endregion

        #region WCS对接-接口地址

        /// <summary>
        /// WCS接口地址-商品信息
        /// </summary>
        public const string WCS_ITEM_BUTT = "wcs.item.butt";

        /// <summary>
        /// WCS接口地址-入库移库信息
        /// </summary>
        public const string WCS_RECEIPT_BUTT = "wcs.receipt.butt";

        /// <summary>
        /// WCS接口地址-出库信息
        /// </summary>
        public const string WCS_SHIPMENT_BUTT = "wcs.shipment.butt";

        /// <summary>
        /// WCS接口地址-订单取消信息
        /// </summary>
        public const string WCS_CANCEL_BUTT = "wcs.cancel.butt";

        /// <summary>
        /// WCS接口地址-盘点信息
        /// </summary>
        public const string WCS_CYCLECOUNT_BUTT = "wcs.cycleCount.butt";

        /// <summary>
        /// WCS接口地址-库存对账信息
        /// </summary>
        public const string WCS_INVENTORIES_BUTT = "wcs.inventories.butt";

        /// <summary>
        /// WCS接口地址-实时库存查询
        /// </summary>
        public const string WCS_INVENTORIES_PAGE_BUTT = "wcs.inventories.page.butt";

        #endregion

        #region 长虹电源

        /// <summary>
        /// 寄售标识
        /// </summary>
        public const string SCCH_ITEM_TYPE_KB = "KB";

        #endregion

        #region 自动下发接口 - 状态

        /// <summary>
        /// 待执行
        /// </summary>
        public static string Interface_Log_State_Wait = "待执行";

        /// <summary>
        /// 已执行
        /// </summary>
        public static string Interface_Log_State_Success = "已执行";

        /// <summary>
        /// 已失败
        /// </summary>
        public static string Interface_Log_State_Error = "已失败";

        /// <summary>
        /// 已取消
        /// </summary>
        public static string Interface_Log_State_Cancel = "已取消";

        #endregion

        #region 接口安全认证方式
        public const string AuthType_Basic = "Basic";
        public const string AuthType_Jwt = "Jwt";
        #endregion

        public static List<string> xt_purchase_arrival_key_list = new List<string>();

        /// <summary>
        /// 销售出库 - 免费订单
        /// </summary>
        public const string Out_Sale_Free_Erp_No = "-";

        public const string STATE_INTER_LOG_COMPLETE = "已完成";

        public const string STATE_INTER_LOG_FAIL = "已失败";

        public const string STATE_INTER_LOG_WAIT = "待执行";

        /// <summary>
        /// 工厂编码 - 四川长虹电源
        /// </summary>
        public const string FACTORY_CODE_C = "Z201";

        /// <summary>
        /// 工厂编码 - H
        /// </summary>
        public const string FACTORY_CODE_H = "Z202";


        public const string VOUCHER_TYPE_IN = "入库单";

        public const string VOUCHER_TYPE_OUT = "出库单";

        public const string VOUCHER_TYPE_OUT_ORDER = "出库订单";

        public const string VOUCHER_TYPE_OUT_ADVICE = "出库通知单";
    }
}
