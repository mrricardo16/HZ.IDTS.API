/// <summary>
/// 功能描述    ：组织单据更新和新增记录，添加到队列中    
/// 创 建 者    ：dbs
/// 创建日期    ：2021/2/23 15:34:24 
/// 最后修改者  ：dbs
/// 最后修改日期：2021/2/23 15:34:24 
/// </summary>
using HZ.CommonUtil.Model;
using HZ.DbHelper;
using HZ.IDTSCore.Model;
using HZ.IDTSCore.Model.Entity.Redis;
using System;
using System.IO;

namespace HZ.IDTSCore.Interfaces.Service.Sys
{
    public static class RedisUpdateDetailLog
    {   
        public static bool AddLog(string activeType,string updateId,string opNo,string orderType,object oldObj,object newObj,string userCode)
        {
            try
            {
                redis_update_detail_log mud = new redis_update_detail_log()
                {
                    cn_s_new_obj = newObj,
                    cn_s_old_obj = oldObj,
                    cn_s_op_no = opNo,
                    cn_s_order_type = orderType,
                    cn_s_update_id = updateId,
                     cn_t_execute=DateTime.Now,
                      cn_s_active_type= activeType, 
                       cn_s_executor=userCode
                };
                
                long sequencsL = RedisServer.Sequence.LPush("editLog", mud); ;// RedisServer.Sequence.LPush("editLog", mud);
                if (sequencsL > 0)
                    return true;
                throw new Exception("redis插入失败！");
            }
            catch(Exception ex){
                throw ex;
            }
        }

        public static bool GetLog()
        {
            try
            {
                redis_update_detail_log mud = RedisServer.Sequence.BRPop<redis_update_detail_log>(10,"editLog");
                //redis_update_detail_log mud = RedisServer.Sequence.BRPopLPush<redis_update_detail_log>( "editLog","executedLog",10);
                ApiResult result = BussObjectCompare.MongoUpdateDetailExec(mud);
                if (result.IsSuccess)
                {
                    DbContext db = new DbContext(new SessionInfo() { });
                    int i= db.Db.Insertable<tn_wms_bill_exec>(result.Data).ExecuteCommand();
                }
                else
                {
                    if (mud != null)
                    {
                        long sequencsL = RedisServer.Sequence.LPush("editLog", mud);
                    }
                        //RedisServer.Sequence.LPush("editLog", mud);
                }

            }
            catch (IOException ex)
            {
            }
            catch (Exception ex)
            { 
            }
            return true;
        }
    }
}
