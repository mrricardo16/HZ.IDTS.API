using HZ.DbHelper;
using HZ.IDTSCore.Interfaces.IService.Common;
using HZ.IDTSCore.Model;
using HZ.IDTSCore.Model.Entity.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Interfaces.Service.Common
{
    public class Barcode : DbContext,IBarcode
    {
        public Barcode(SessionInfo session):base(session)
        { 
        }
        public BarcodeItem GetItem(string key, string type)
        {
            BarcodeItem cache = RedisServer.BarCode.Get<BarcodeItem>(key);
            if (cache == null)
            {
                BarcodeItem returnObj = null;
                switch (type)
                {
                    case "物料码":
                        var item = Db.Queryable<tn_wms_item>().First(x => x.cn_s_item_code == key);
                        if (item == null)
                            return null;
                        returnObj = Convert(item);
                        break;
                    case "批次号":
                        var lotcode = Db.Queryable<tn_wms_lotcode>().First(x => x.cn_s_lot_barcode == key);
                        if (lotcode == null)
                            return null;
                        returnObj = Convert(lotcode);
                        break;
                }

                if (returnObj != null)
                    RedisServer.BarCode.Set(key, returnObj);
                return returnObj;
            }
            else
                return cache;
        }

        private BarcodeItem Convert(tn_wms_item item)
        {
            return new BarcodeItem()
            {
                cn_s_item_code = item.cn_s_item_code,
                cn_f_qty = 1,
                cn_s_item_name = item.cn_s_item_name,
                cn_s_model = item.cn_s_model,
                cn_s_unit = item.cn_s_unit
            };
        }


        private BarcodeItem Convert(tn_wms_lotcode lot)
        {
            return new BarcodeItem()
            {
                cn_s_item_code = lot.cn_s_item_code,
                cn_f_qty = 1,
                cn_s_item_name = lot.cn_s_item_name,
                cn_s_model = lot.cn_s_model,
                cn_s_unit = lot.cn_s_unit,
                 cn_s_other_lotcode= lot.cn_s_other_lotcode,
                  cn_s_production_batch= lot.cn_s_production_batch,
                   cn_t_in_storage_date= lot.cn_t_in_storage_date,
                    cn_t_production= lot.cn_t_production
            };
        }
    }
}
