using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using HZ.CommonUtil.Model;

namespace HZ.CommonUtil.Helpers
{
   public class ProgressRewardsHelp
    {
        public static List<ProgressRewards> progressRewardsList = new List<ProgressRewards>();
        public void SetData(ProgressRewards progressRewards)
        {
          var model=  progressRewardsList.Where(x=>x.usid== progressRewards.usid && x.type == progressRewards.type).FirstOrDefault();
            if (model==null)
            {
                progressRewards.percent = (int)(Convert.ToDouble(progressRewards.countOk) / Convert.ToDouble(progressRewards.count) * 100);
                progressRewardsList.Add(progressRewards);
            }
            else
            {
                model.countOk = progressRewards.countOk;
                //model.percent =int.Parse( (model.countOk / model.count).ToString("fo") )* 100;
                double percent = Convert.ToDouble(model.countOk) / Convert.ToDouble(model.count)*100;
                model.percent =(int) percent;

            }
        }
        /// <summary>
        /// 移除list已经缓存完成的数据
        /// </summary>
        /// <param name="progressRewards"></param>
        public void RemoveData(ProgressRewards progressRewards)
        {
            var model = progressRewardsList.Where(x => x.usid == progressRewards.usid && x.type == progressRewards.type).FirstOrDefault();
            if (model != null)
            {
                progressRewardsList.Remove(progressRewards);
            }
        }
    }
}
