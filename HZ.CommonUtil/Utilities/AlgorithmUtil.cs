using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.CommonUtil.Utilities
{
    public static class AlgorithmUtil
    {
        private static Dictionary<string, float> task = new Dictionary<string, float>();

        /// <summary>
        /// 下次重试延迟时间
        /// </summary>
        /// <param name="key">关键编码</param>
        /// <param name="success">本次是否成功</param>
        /// <returns></returns>
        public static float NextTryDelay(string key, bool success, float defaultDelay)
        {
            if (success)
            {
                task.Remove(key);
                return defaultDelay;
            }
            else
            {
                if (task.ContainsKey(key))
                {
                    task[key] = DelayLevel(task[key]);
                    return task[key];
                }
                else
                {
                    task.Add(key, DelayLevel(defaultDelay));
                    return task[key];
                }
            }
        }

        private static float DelayLevel(float levelDelay)
        {
            if (levelDelay >= 60)//不会大于10分钟
                return levelDelay;
            return levelDelay * 2;
        }
    }
}
