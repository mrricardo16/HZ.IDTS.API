
/********************************************************************************

** auth： zh

** date： 2019/4/8

** desc： 在global中的定时线程均实现该接口

** Ver.:  V1.0.0

*********************************************************************************/
using System.Timers;

namespace HZ.IDTSCore.Interfaces
{
    /// <summary>
    /// 定时执行线程
    /// </summary>
    public interface IExecuteThreadTimed
    {
        /// <summary>
        /// 执行事件
        /// </summary>
        /// <param name="source"></param>
        /// <param name="elapsedEventArgs"></param>
        void Run(object source, ElapsedEventArgs elapsedEventArgs);
    }
}