using System.Collections.Generic;

namespace HZ.IDTSCore.Common
{
    public static class SysDic
    {
        public static List<string> GetDic(string dicName)
        {
            return dic[dicName];
        }

        private static Dictionary<string, List<string>> dic = new Dictionary<string, List<string>>()
        {

            { "IQC单状态", new List<string> { "送检",  "已检"}},
            { "类型", new List<string> { "分拣墙", "分拣车" }},
            { "分拣墙状态", new List<string> { "启用", "禁用" }},
            { "入库订单状态", new List<string> { "新建", "已提交", "已审核", "已完成", "已驳回", "已取消" }},//入库订单状态
            { "到货单状态", new List<string> { "新建", "已提交", "已审核", "已完成", "已驳回", "已取消" }},//到货单状态
            { "入库单状态", new List<string> { "新建", "已提交", "已审核", "已完成", "已驳回", "已取消" }},//入库单状态
            { "出库订单状态", new List<string> { "新建", "已提交", "已审核", "已完成", "已驳回", "作业中"}},//出库订单状态
            { "出库通知单状态", new List<string> { "新建", "已提交", "已审核", "已完成", "已驳回", "已取消" , "缺料", "待分拣", "已分拣", "分拣中"}},//出库通知单状态
            { "出库单状态", new List<string> { "新建", "已提交", "已审核", "已完成", "已驳回", "已取消" }},//出库单状态
            { "分拣单状态", new List<string> { "新建", "已提交", "分拣中", "已分拣", "已取消" }},//分拣单状态
            { "拣货单状态", new List<string> { "新建", "已提交", "分拣中", "已分拣", "已取消" }},//拣货单状态
            { "出库计划状态", new List<string> { "新建", "已提交", "已审核", "已取消" }},//出库计划状态
            { "补货单状态", new List<string> { "新建",  "待补货", "补货中", "已完成", "已取消","已驳回"}},
            
            { "调拨类型", new List<string> { "普通调拨", "调拨出库", "调拨入库"}},
            { "调拨方式", new List<string> { "一步法", "二步法"}},
            { "补货类型", new List<string> { "常规补货", "黑仓补货"}},

            { "盘点类型", new List<string> { "物料盘点","货位盘点" }},
            { "盘点方式", new List<string> { "明盘", "暗盘"}},      //"静态盘点", "动态盘点"
            { "库存调整方式", new List<string> { "盈亏确认", "直接生效"}},

            #region 货位管理→通道
            { "通道类型", new List<string> { "储物通道", "行车通道" }},//通道类型
            { "通道模式", new List<string> { "队列", "栈" }},//通道模式
            { "通道方向", new List<string> { "从大到小", "从小到大" }},//通道方向
            { "通道状态", new List<string> { "正常", "故障" }},//通道状态

	        #endregion
            
            { "队列方向", new List<string> { "列从小到大", "列从大到小" }},
            { "作业方式", new List<string> { "自动", "人工" }},

            #region 任务查询
            { "任务类型", new List<string> { "叫料", "出料" }},//任务类型
            { "任务状态", new List<string> { "待执行", "执行中", "待推送", "完成" }},//任务状态
            { "任务名称", new List<string> { "出库", "入库" ,"周转","移库"}},//任务名称
            { "任务执行状态", new List<string> { "待执行","待下发", "执行中", "取货完成","卸货完成", "完成" }},//任务执行状态
            #endregion

            { "展示形式",new List<string> { "表单", "列表", "筛选条件" }},//展示形式
            { "对齐方式",new List<string> { "left", "center", "right" }},//对齐方式
            { "编辑类型",new List<string> { "string", "bool", "number", "itemdialog", "select","date","cusselect", "double", "label" }},//编辑类型

            { "管控级别",new List<string> { "到货位", "到仓库" }},//管控级别
            #region 通道记录查询
            { "数据状态", new List<string> { "正常", "异常"}},//数据状态
            #endregion

            
            { "控制属性",new List<string> { "按重量", "按体积" }},//控制属性
            
            { "货位标识",new List<string> {"暂存货位","拣货货位","移动货位","备货货位","虚拟货位","作业位" }},
            { "库区类型",new List<string> { "库区", "作业区" }},//控制属性
            { "库区结构",new List<string> { "立库","平库","地堆","流利式货架","单头单向穿梭车","两头单向穿梭车","单头四向穿梭车","黑盒"}},


            #region 业务分类
            //{ "出库通知单",new List<string> { "销售发货通知单", "生产领料通知单","调拨出库通知单","其他出库通知单","外借发货通知单"}},

            //{ "入库订单",new List<string> { "采购入库订单", "生产入库订单", "调拨入库订单", "其他入库订单"}},
            //{ "到货单",new List<string> { "采购入库到货单", "生产入库到货单", "调拨入库到货单", "其他入库到货单"}},

            {"单据类型",new List<string> { "入库订单", "到货单", "入库单", "出库订单", "出库通知单", "出库单"}},
            #endregion

            { "出库节点",new List<string> { "手工确认","拣货完成","OQC"}},
            { "系统分类",new List<string> { "亮灯系统","AGV系统","业务系统"}},
            { "系统通讯方式",new List<string> { "网口通讯","TCP/IP","中间库"}},
            { "外部系统",new List<string> { "亮灯系统","AGV系统"} },
            { "处理状态",new List<string>{ "已处理","待处理"} },

             { "工作台状态",new List<string> { "工作", "停止" }},//工作台状态
              
            { "码制",new List<string> { "物料码", "批次码","唯一码" }},
            { "批次管控方式",new List<string> { "入库批次","生产批次", "到货批次","生产日期"}},

            { "提醒类型",new List<string> { "保管到期", "油封到期"}},

            { "日志状态",new List<string> { "待执行", "已执行"}},
        };

        private static Dictionary<string, string> factory = new Dictionary<string, string>() {

            { "Z201", "00"},
            { "Z202", "01"},


            //陈赛男测试用
            { "00", "00"},
            { "01", "01"},
            { "02", "02"},
        };

        public static string GetFactoryOrg(string key)
        {
            return factory[key];
        }

        public static string GetFactoryCode(string code)
        {
            foreach(var dict in factory)
            {
                if (dict.Value.Equals(code))
                    return dict.Key;
            }
            return string.Empty;
        }

        //public static string GetOrgFactory(string key)
        //{

        //}

        //Z201 长虹电源工厂  00
        //Z202 H工厂     01
    }
}