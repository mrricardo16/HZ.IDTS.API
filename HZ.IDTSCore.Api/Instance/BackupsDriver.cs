using HZ.IDTSCore.Model.Entity.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HZ.IDTSCore.Api.Instance
{
    public class BackupsDriver
    {
        private static readonly BackupsDriver instance = new BackupsDriver();

        private BackupsDriver()
        {

        }

        public static BackupsDriver Instance
        {
            get
            {
                return instance;
            }
        }

        /// <summary>
        /// 自动备份的最新配置信息
        /// </summary>
        public SaveBackups DBackups = new SaveBackups();

        /// <summary>
        /// 上一次自动备份配置信息
        /// </summary>
        public SaveBackups LastDBackups { get; set; }

        /// <summary>
        /// 自动备份配置修改时间
        /// </summary>
        public DateTime LastBackups { get; set; }

        /// <summary>
        /// 是否刚启动API
        /// </summary>
        public bool IsFirst { get; set; }

        /// <summary>
        /// 是否执行自动备份
        /// </summary>
        public bool BackupsFlag { get; set; }

        /// <summary>
        /// 复原BackupsFlag时间
        /// </summary>
        public int RestoreFlagTime { get; set; }

        #region 获取指定日期所在周的周一和周日在该年（前一年/后一年）的DayOfYear
        /// <summary>
        /// 获取指定日期所在周的周一和周日在该年（前一年/后一年）的DayOfYear
        /// </summary>
        /// <param name="dateTime">指定日期</param>
        /// <returns></returns>
        public WeekDayOfYearRange GetWeekDayOfYearRange(DateTime dateTime)
        {
            DayOfWeek dayOfWeek = dateTime.DayOfWeek;
            int dayOfYear = dateTime.DayOfYear;
            WeekDayOfYearRange weekDayOfYearRange = new WeekDayOfYearRange();
            int currentYearDays = 0;
            int lastYearDays = 0;
            int year = dateTime.Year;
            int lastYear = year - 1;
            if (year % 400 == 0 || (year % 4 == 0 && year % 100 != 0))
            {
                currentYearDays = 366;
            }
            else
            {
                currentYearDays = 365;
            }
            if (lastYear % 400 == 0 || (lastYear % 4 == 0 && lastYear % 100 != 0))
            {
                lastYearDays = 366;
            }
            else
            {
                lastYearDays = 355;
            }
            if (dayOfYear > 7 && dayOfYear < currentYearDays - 6)
            {
                switch (dayOfWeek)
                {
                    case DayOfWeek.Monday:
                        weekDayOfYearRange.ThisWeekMonday = dayOfYear;
                        weekDayOfYearRange.ThisWeekSunday = dayOfYear + 6;
                        weekDayOfYearRange.Status = 0;
                        weekDayOfYearRange.Year = year;
                        break;
                    case DayOfWeek.Tuesday:
                        weekDayOfYearRange.ThisWeekMonday = dayOfYear - 1;
                        weekDayOfYearRange.ThisWeekSunday = dayOfYear + 5;
                        weekDayOfYearRange.Status = 0;
                        weekDayOfYearRange.Year = year;
                        break;
                    case DayOfWeek.Wednesday:
                        weekDayOfYearRange.ThisWeekMonday = dayOfYear - 2;
                        weekDayOfYearRange.ThisWeekSunday = dayOfYear + 4;
                        weekDayOfYearRange.Status = 0;
                        weekDayOfYearRange.Year = year;
                        break;
                    case DayOfWeek.Thursday:
                        weekDayOfYearRange.ThisWeekMonday = dayOfYear - 3;
                        weekDayOfYearRange.ThisWeekSunday = dayOfYear + 3;
                        weekDayOfYearRange.Status = 0;
                        weekDayOfYearRange.Year = year;
                        break;
                    case DayOfWeek.Friday:
                        weekDayOfYearRange.ThisWeekMonday = dayOfYear - 4;
                        weekDayOfYearRange.ThisWeekSunday = dayOfYear + 2;
                        weekDayOfYearRange.Status = 0;
                        weekDayOfYearRange.Year = year;
                        break;
                    case DayOfWeek.Saturday:
                        weekDayOfYearRange.ThisWeekMonday = dayOfYear - 5;
                        weekDayOfYearRange.ThisWeekSunday = dayOfYear + 1;
                        weekDayOfYearRange.Status = 0;
                        weekDayOfYearRange.Year = year;
                        break;
                    case DayOfWeek.Sunday:
                        weekDayOfYearRange.ThisWeekMonday = dayOfYear - 6;
                        weekDayOfYearRange.ThisWeekSunday = dayOfYear;
                        weekDayOfYearRange.Status = 0;
                        weekDayOfYearRange.Year = year;
                        break;
                }
            }
            else if (dayOfYear <= 7)
            {
                //
                DayOfWeek yearFirstDay = dateTime.AddDays(-dayOfYear + 1).DayOfWeek;
                switch (yearFirstDay)
                {
                    case DayOfWeek.Monday:
                        weekDayOfYearRange.ThisWeekMonday = 1;
                        weekDayOfYearRange.ThisWeekSunday = 7;
                        weekDayOfYearRange.Status = 1;
                        weekDayOfYearRange.Year = year;
                        break;
                    case DayOfWeek.Tuesday:
                        if (dayOfYear <= 6)
                        {
                            weekDayOfYearRange.ThisWeekMonday = lastYearDays;
                            weekDayOfYearRange.ThisWeekSunday = 6;
                            weekDayOfYearRange.Status = 1;          
                        }
                        else
                        {
                            weekDayOfYearRange.ThisWeekMonday = 7;
                            weekDayOfYearRange.ThisWeekSunday = 7 + 6;
                            weekDayOfYearRange.Status = 0;
                        }
                        weekDayOfYearRange.Year = year;
                        break;
                    case DayOfWeek.Wednesday:
                        if (dayOfYear <= 5)
                        {
                            weekDayOfYearRange.ThisWeekMonday = lastYearDays - 1;
                            weekDayOfYearRange.ThisWeekSunday = 5;
                            weekDayOfYearRange.Status = 1;
                        }
                        else
                        {
                            weekDayOfYearRange.ThisWeekMonday = 6;
                            weekDayOfYearRange.ThisWeekSunday = 6 + 6;
                            weekDayOfYearRange.Status = 0;
                        }
                        weekDayOfYearRange.Year = year;
                        break;
                    case DayOfWeek.Thursday:
                        if (dayOfYear <= 4)
                        {
                            weekDayOfYearRange.ThisWeekMonday = lastYearDays - 2;
                            weekDayOfYearRange.ThisWeekSunday = 4;
                            weekDayOfYearRange.Status = 1;                           
                        }
                        else
                        {
                            weekDayOfYearRange.ThisWeekMonday = 5;
                            weekDayOfYearRange.ThisWeekSunday = 5 + 6;
                            weekDayOfYearRange.Status = 0;
                        }
                        weekDayOfYearRange.Year = year;
                        break;
                    case DayOfWeek.Friday:
                        if (dayOfYear <= 3)
                        {
                            weekDayOfYearRange.ThisWeekMonday = lastYearDays - 3;
                            weekDayOfYearRange.ThisWeekSunday = 3;
                            weekDayOfYearRange.Status = 1;                            
                        }
                        else
                        {
                            weekDayOfYearRange.ThisWeekMonday = 4;
                            weekDayOfYearRange.ThisWeekSunday = 4 + 6;
                            weekDayOfYearRange.Status = 0;
                        }
                        weekDayOfYearRange.Year = year;
                        break;
                    case DayOfWeek.Saturday:
                        if (dayOfYear <= 2)
                        {
                            weekDayOfYearRange.ThisWeekMonday = lastYearDays - 4;
                            weekDayOfYearRange.ThisWeekSunday = 2;
                            weekDayOfYearRange.Status = 1;
                        }
                        else
                        {
                            weekDayOfYearRange.ThisWeekMonday = 3;
                            weekDayOfYearRange.ThisWeekSunday = 3 + 6;
                            weekDayOfYearRange.Status = 0;
                        }
                        weekDayOfYearRange.Year = year;
                        break;
                    case DayOfWeek.Sunday:
                        if (dayOfYear <= 1)
                        {
                            weekDayOfYearRange.ThisWeekMonday = lastYearDays - 5;
                            weekDayOfYearRange.ThisWeekSunday = 1;
                            weekDayOfYearRange.Status = 1;
                        }
                        else
                        {
                            weekDayOfYearRange.ThisWeekMonday = 2;
                            weekDayOfYearRange.ThisWeekSunday = 2 + 6;
                            weekDayOfYearRange.Status = 0;
                        }
                        weekDayOfYearRange.Year = year;
                        break;
                }
            }
            else//dayOfYear >= currentYearDays - 6
            {
                DayOfWeek yearFinalDay = dateTime.AddDays(-dayOfYear + 1).AddDays(currentYearDays - 1).DayOfWeek;
                switch (yearFinalDay)
                {
                    case DayOfWeek.Monday:
                        if (dayOfYear >= currentYearDays)
                        {
                            weekDayOfYearRange.ThisWeekMonday = currentYearDays;
                            weekDayOfYearRange.ThisWeekSunday = 6;
                            weekDayOfYearRange.Status = 2;
                            weekDayOfYearRange.LastWeek = yearFinalDay;
                            weekDayOfYearRange.AllYearDays = currentYearDays;                           
                        }
                        else
                        {
                            weekDayOfYearRange.ThisWeekMonday = currentYearDays - 1 - 6;
                            weekDayOfYearRange.ThisWeekSunday = currentYearDays - 1;
                            weekDayOfYearRange.Status = 0;
                        }
                        weekDayOfYearRange.Year = year;
                        break;
                    case DayOfWeek.Tuesday:
                        if (dayOfYear >= currentYearDays - 1)
                        {
                            weekDayOfYearRange.ThisWeekMonday = currentYearDays - 1;
                            weekDayOfYearRange.ThisWeekSunday = 5;
                            weekDayOfYearRange.Status = 2;
                            weekDayOfYearRange.LastWeek = yearFinalDay;
                            weekDayOfYearRange.AllYearDays = currentYearDays;
                        }
                        else
                        {
                            weekDayOfYearRange.ThisWeekMonday = currentYearDays - 2 - 6;
                            weekDayOfYearRange.ThisWeekSunday = currentYearDays - 2;
                            weekDayOfYearRange.Status = 0;
                        }
                        weekDayOfYearRange.Year = year;
                        break;
                    case DayOfWeek.Wednesday:
                        if (dayOfYear >= currentYearDays - 2)
                        {
                            weekDayOfYearRange.ThisWeekMonday = currentYearDays - 2;
                            weekDayOfYearRange.ThisWeekSunday = 4;
                            weekDayOfYearRange.Status = 2;
                            weekDayOfYearRange.LastWeek = yearFinalDay;
                            weekDayOfYearRange.AllYearDays = currentYearDays;                           
                        }
                        else
                        {
                            weekDayOfYearRange.ThisWeekMonday = currentYearDays - 3 - 6;
                            weekDayOfYearRange.ThisWeekSunday = currentYearDays - 3;
                            weekDayOfYearRange.Status = 0;
                        }
                        weekDayOfYearRange.Year = year;
                        break;
                    case DayOfWeek.Thursday:
                        if(dayOfYear >= currentYearDays - 3)
                        {
                            weekDayOfYearRange.ThisWeekMonday = currentYearDays - 3;
                            weekDayOfYearRange.ThisWeekSunday = 3;
                            weekDayOfYearRange.Status = 2;
                            weekDayOfYearRange.LastWeek = yearFinalDay;
                            weekDayOfYearRange.AllYearDays = currentYearDays;
                        }
                        else
                        {
                            weekDayOfYearRange.ThisWeekMonday = currentYearDays - 4 - 6;
                            weekDayOfYearRange.ThisWeekSunday = currentYearDays - 4;
                            weekDayOfYearRange.Status = 0;
                        }
                        weekDayOfYearRange.Year = year;
                        break;
                    case DayOfWeek.Friday:
                        if(dayOfYear >= currentYearDays - 4)
                        {
                            weekDayOfYearRange.ThisWeekMonday = currentYearDays - 4;
                            weekDayOfYearRange.ThisWeekSunday = 2;
                            weekDayOfYearRange.Status = 2;
                            weekDayOfYearRange.LastWeek = yearFinalDay;
                            weekDayOfYearRange.AllYearDays = currentYearDays;
                        }
                        else
                        {
                            weekDayOfYearRange.ThisWeekMonday = currentYearDays - 5 - 6;
                            weekDayOfYearRange.ThisWeekSunday = currentYearDays - 5;
                            weekDayOfYearRange.Status = 0;
                        }
                        weekDayOfYearRange.Year = year;
                        break;
                    case DayOfWeek.Saturday:
                        if(dayOfYear >= currentYearDays - 5)
                        {
                            weekDayOfYearRange.ThisWeekMonday = currentYearDays - 5;
                            weekDayOfYearRange.ThisWeekSunday = 1;
                            weekDayOfYearRange.Status = 2;
                            weekDayOfYearRange.LastWeek = yearFinalDay;
                            weekDayOfYearRange.AllYearDays = currentYearDays;
                        }
                        else
                        {
                            weekDayOfYearRange.ThisWeekMonday = currentYearDays - 6 - 6;
                            weekDayOfYearRange.ThisWeekSunday = currentYearDays - 6;
                            weekDayOfYearRange.Status = 0;
                        }
                        weekDayOfYearRange.Year = year;
                        break;
                    case DayOfWeek.Sunday:
                        weekDayOfYearRange.ThisWeekMonday = currentYearDays - 6;
                        weekDayOfYearRange.ThisWeekSunday = currentYearDays;
                        weekDayOfYearRange.Status = 2;
                        weekDayOfYearRange.LastWeek = yearFinalDay;
                        weekDayOfYearRange.AllYearDays = currentYearDays;
                        weekDayOfYearRange.Year = year;
                        break;
                }
            }
            return weekDayOfYearRange;
        }
        #endregion

    }
}
