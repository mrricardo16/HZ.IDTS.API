using HZ.CommonUtil.Model;
using HZ.DbHelper;
using HZ.IDTSCore.Interfaces.IService.Sys;
using HZ.IDTSCore.Model.Entity.Equipment;
using HZ.IDTSCore.Model.Entity.Sys;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace HZ.IDTSCore.Interfaces.Service.Sys
{
    public class BackupsService : BaseService<tn_dts_dbops>, IBackupsService
    {
        public BackupsService(SessionInfo session) : base(session)
        {

        }

        #region 备份接口
        /// <summary>
        /// 备份接口
        /// </summary>
        /// <param name="backupFilePath">备份的路径(绝对路径)</param>
        /// <param name="category">备份类别</param>
        /// <returns></returns>
        public ReturnMessage BackUp(string backupFilePath, string category)
        {
            ReturnMessage returnMessage = new ReturnMessage();
            string guid = Guid.NewGuid().ToString();
            DateTime nowTime = DateTime.Now;
            string fullFileName = guid + "_" + "idts_" + nowTime.ToString("yyyyMMddHHmmssfff") + ".dump";
            //string outputPath = backupFilePath.Replace(@"\\", @"\") + fullFileName;
            string pgDumpPath = Db.Queryable<tn_dts_setting>().Where(it => it.cn_s_setting_keycode == "PostgreSQLPgdumpDiretory").Select(it => it.cn_s_setting_keyvalue).First();
            string outputPath = Path.Combine(backupFilePath, fullFileName);
            //string pgDumpPath = @"E:\Software\PostgreSQL\pgsql\bin\pg_dump";
            //string pgDumpPath = @"E:\Software\PostgreSQL\12\bin\pg_dump";
            if(!Directory.Exists(backupFilePath))
            {
                returnMessage.IsSuccess = false;
                returnMessage.Message = "传入的备份路径不存在，请重试！";
                return returnMessage;
            }
            string pgDumpFileName = pgDumpPath;
            if (!File.Exists(pgDumpFileName))
            {
                pgDumpFileName = pgDumpPath + ".exe";
            }
            if (!File.Exists(pgDumpFileName))
            {
                returnMessage.IsSuccess = false;
                returnMessage.Message = "系统设置中的PostgreSQL的pg_dump路径找不到pg_dump文件，请重试！";
                return returnMessage;
            }
            Dictionary<string, string> connectionItems = ParseConnectionString(Db.CurrentConnectionConfig.ConnectionString);
            string dbHost = GetConnectionValue(connectionItems, "HOST");
            string dbPort = GetConnectionValue(connectionItems, "PORT");
            string dbName = GetConnectionValue(connectionItems, "DATABASE");
            string dbUser = GetConnectionValue(connectionItems, "USER ID");
            string dbPassword = GetConnectionValue(connectionItems, "PASSWORD");
            if (string.IsNullOrEmpty(dbHost) || string.IsNullOrEmpty(dbPort) || string.IsNullOrEmpty(dbName) || string.IsNullOrEmpty(dbUser) || string.IsNullOrEmpty(dbPassword))
            {
                returnMessage.IsSuccess = false;
                returnMessage.Message = "当前数据库连接串中备份配置项有空项，请检查HOST、PORT、DATABASE、USER ID、PASSWORD这五项！";
                return returnMessage;
            }
            string pgDumpArguments = $"-Fc -h {dbHost} -p {dbPort} -f \"{outputPath}\" -U {dbUser} {dbName}";
            using (Process process = new Process())
            {
                process.StartInfo.FileName = pgDumpFileName;
                process.StartInfo.ArgumentList.Add("-Fc");
                process.StartInfo.ArgumentList.Add("-h");
                process.StartInfo.ArgumentList.Add(dbHost);
                process.StartInfo.ArgumentList.Add("-p");
                process.StartInfo.ArgumentList.Add(dbPort);
                process.StartInfo.ArgumentList.Add("-f");
                process.StartInfo.ArgumentList.Add(outputPath);
                process.StartInfo.ArgumentList.Add("-U");
                process.StartInfo.ArgumentList.Add(dbUser);
                process.StartInfo.ArgumentList.Add(dbName);
                process.StartInfo.Environment["PGPASSWORD"] = dbPassword;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.UseShellExecute = false;
                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                // 等待备份进程完成
                process.WaitForExit();
                if (process.ExitCode == 0)
                {
                    FileInfo dumpfile = new FileInfo(outputPath);
                    tn_dts_dbops dbops = new tn_dts_dbops()
                    {
                        cn_guid = guid,
                        cn_s_dbops_category = category,
                        cn_s_dbops_type = "备份",
                        cn_s_dbops_filename = fullFileName.Substring(fullFileName.IndexOf('_') + 1).Split('.')[0],
                        cn_s_dbops_filesize = dumpfile.Length / 1024f / 1024f,
                        cn_s_dbops_fullpath = outputPath,
                        cn_s_dbops_result = "备份成功",
                        cn_t_create = nowTime
                    };
                    if(category == "人工")
                    {
                        UserSession user = GetSessionInfo();
                        dbops.cn_s_creator = user.UserCode;
                        dbops.cn_s_creator_by = user.UserName;
                    }
                    else if(category == "自动")
                    {
                        dbops.cn_s_creator = "系统自动";
                        dbops.cn_s_creator_by = "系统自动";
                    }
                    int res = Db.Insertable(dbops).ExecuteCommand();
                    if (res <= 0)
                    {
                        returnMessage.IsSuccess = false;
                        returnMessage.Message = "新增tn_dts_dbops表失败！";
                        return returnMessage;
                    }
                    returnMessage.IsSuccess = true;
                    returnMessage.Message = "备份成功！";
                    return returnMessage;
                }
                else
                {
                    tn_dts_dbops dbops = new tn_dts_dbops()
                    {
                        cn_guid = guid,
                        cn_s_dbops_category = category,
                        cn_s_dbops_type = "备份",
                        cn_s_dbops_filename = null,
                        cn_s_dbops_filesize = 0,
                        cn_s_dbops_fullpath = null,
                        cn_s_dbops_result = "备份失败。" + error + ";指令=" + pgDumpArguments + "",
                        cn_t_create = nowTime
                    };
                    if (category == "人工")
                    {
                        UserSession user = GetSessionInfo();
                        dbops.cn_s_creator = user.UserCode;
                        dbops.cn_s_creator_by = user.UserName;
                    }
                    else if (category == "自动")
                    {
                        dbops.cn_s_creator = "系统自动";
                        dbops.cn_s_creator_by = "系统自动";
                    }
                    int res = Db.Insertable(dbops).ExecuteCommand();
                    if (res <= 0)
                    {
                        returnMessage.IsSuccess = false;
                        returnMessage.Message = "新增tn_dts_dbops表失败！";
                        return returnMessage;
                    }
                    System.IO.File.Delete(outputPath);
                    returnMessage.IsSuccess = false;
                    returnMessage.Message = "备份失败，" + error;
                    return returnMessage;
                }
            }

        }
        #endregion

        private Dictionary<string, string> ParseConnectionString(string connectionString)
        {
            Dictionary<string, string> connectionItems = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (string.IsNullOrEmpty(connectionString))
            {
                return connectionItems;
            }

            string[] items = connectionString.Split(';');
            foreach (string item in items)
            {
                if (string.IsNullOrWhiteSpace(item))
                {
                    continue;
                }

                int index = item.IndexOf('=');
                if (index <= 0)
                {
                    continue;
                }

                string key = item.Substring(0, index).Trim();
                string value = item.Substring(index + 1).Trim();
                connectionItems[key] = value;
            }

            return connectionItems;
        }

        private string GetConnectionValue(Dictionary<string, string> connectionItems, string key)
        {
            if (connectionItems.TryGetValue(key, out string value))
            {
                return value;
            }
            return string.Empty;
        }

        #region 保存接口
        /// <summary>
        /// 保存接口
        /// </summary>
        /// <param name="saveBackups"></param>
        /// <returns></returns>
        public ReturnMessage Save(SaveBackups saveBackups)
        {
            ReturnMessage returnMessage = new ReturnMessage();
            UserSession user = GetSessionInfo();
            tn_dts_setting isAutomatic = Db.Queryable<tn_dts_setting>().Where(it => it.cn_s_setting_keycode == "IsAutomatic").First();
            string saveIsAutomatic;
            if(saveBackups.IsAutomatic.ToString() == "True")
            {
                saveIsAutomatic = "true";
            }
            else
            {
                saveIsAutomatic = "false";
            }
            if (isAutomatic.cn_s_setting_keyvalue!= saveIsAutomatic)
            {
                isAutomatic.cn_s_setting_keyvalue = saveIsAutomatic;
                isAutomatic.cn_s_modify = user.UserCode;
                isAutomatic.cn_s_modifyBy = user.UserName;
                isAutomatic.cn_t_modify = DateTime.Now;
                int resAutomatic = Db.Updateable(isAutomatic).Where(it => it.cn_s_setting_keycode == "IsAutomatic").ExecuteCommand();
                if (resAutomatic <= 0)
                {
                    returnMessage.IsSuccess = false;
                    returnMessage.Message = "更新设备表中“是否开启自动备份”项修改失败！";
                    return returnMessage;
                }
            }
            tn_dts_setting backupsSpan = Db.Queryable<tn_dts_setting>().Where(it => it.cn_s_setting_keycode == "BackupsSpan").First();
            if (backupsSpan.cn_s_setting_keyvalue != saveBackups.Span.ToString())
            {
                backupsSpan.cn_s_setting_keyvalue = saveBackups.Span.ToString();
                backupsSpan.cn_s_modify = user.UserCode;
                backupsSpan.cn_s_modifyBy = user.UserName;
                backupsSpan.cn_t_modify = DateTime.Now;
                int resBackupsSpan = Db.Updateable(backupsSpan).Where(it => it.cn_s_setting_keycode == "BackupsSpan").ExecuteCommand();
                if (resBackupsSpan <= 0)
                {
                    returnMessage.IsSuccess = false;
                    returnMessage.Message = "更新设备表中“备份周期”项修改失败！";
                    return returnMessage;
                }
            }
            tn_dts_setting backupsTime = Db.Queryable<tn_dts_setting>().Where(it => it.cn_s_setting_keycode == "BackupsTime").First();
            if (backupsTime.cn_s_setting_keyvalue != saveBackups.HourDayWeek)
            {
                backupsTime.cn_s_setting_keyvalue = saveBackups.HourDayWeek;
                backupsTime.cn_s_modify = user.UserCode;
                backupsTime.cn_s_modifyBy = user.UserName;
                backupsTime.cn_t_modify = DateTime.Now;
                int resBackupsTime = Db.Updateable(backupsTime).Where(it => it.cn_s_setting_keycode == "BackupsTime").ExecuteCommand();
                if (resBackupsTime <= 0)
                {
                    returnMessage.IsSuccess = false;
                    returnMessage.Message = "更新设备表中“备份时间”项修改失败！";
                    return returnMessage;
                }
            }
            tn_dts_setting backupsDiretory = Db.Queryable<tn_dts_setting>().Where(it => it.cn_s_setting_keycode == "BackupsDiretory").First();
            if (backupsDiretory.cn_s_setting_keyvalue != saveBackups.BackupsDiretory)
            {
                backupsDiretory.cn_s_setting_keyvalue = saveBackups.BackupsDiretory;
                backupsDiretory.cn_s_modify = user.UserCode;
                backupsDiretory.cn_s_modifyBy = user.UserName;
                backupsDiretory.cn_t_modify = DateTime.Now;
                int resBackupsDiretory = Db.Updateable(backupsDiretory).Where(it => it.cn_s_setting_keycode == "BackupsDiretory").ExecuteCommand();
                if (resBackupsDiretory <= 0)
                {
                    returnMessage.IsSuccess = false;
                    returnMessage.Message = "更新设备表中“备份目录”项修改失败！";
                    return returnMessage;
                }
            }
            returnMessage.IsSuccess = true;
            returnMessage.Message = "保存成功！";
            return returnMessage;
        }
        #endregion

        #region 分页获取所有备份记录（不含查询）接口
        /// <summary>
        /// 分页获取所有备份记录（不含查询）接口
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        public PagedInfo<tn_dts_dbops> GetListPages(PageParm parm)
        {
            try
            {
                return Db.Queryable<tn_dts_dbops>()
               .OrderBy(string.IsNullOrEmpty(parm.OrderBy) ? " cn_t_modify desc" : parm.OrderBy)
               .ToPage(parm.PageIndex, parm.PageSize);
            }
            catch { return new PagedInfo<tn_dts_dbops>(); };
        }
        #endregion

        #region 获取最近的自动备份配置接口
        /// <summary>
        /// 获取最近的自动备份配置接口
        /// </summary>
        /// <returns></returns>
        public SaveBackups GetLatestSaveBackups()
        {
            SaveBackups saveBackups = new SaveBackups();
            try
            {
                string isAutomatic = Db.Queryable<tn_dts_setting>().Where(it => it.cn_s_setting_keycode == "IsAutomatic").Select(it => it.cn_s_setting_keyvalue).First();
                if (!string.IsNullOrEmpty(isAutomatic))
                {
                    saveBackups.IsAutomatic = bool.Parse(isAutomatic);
                }
                else
                {
                    saveBackups.IsAutomatic = null;
                }
                string backupsSpan = Db.Queryable<tn_dts_setting>().Where(it => it.cn_s_setting_keycode == "BackupsSpan").Select(it => it.cn_s_setting_keyvalue).First();
                if (!string.IsNullOrEmpty(backupsSpan))
                {
                    if (backupsSpan == "Day")
                    {
                        saveBackups.Span = Span.Day;
                    }
                    else if (backupsSpan == "Week")
                    {
                        saveBackups.Span = Span.Week;
                    }
                    else if (backupsSpan == "Month")
                    {
                        saveBackups.Span = Span.Month;
                    }
                    else
                    {
                        saveBackups.Span = null;
                    }
                }
                else
                {
                    saveBackups.Span = null;
                }
                string backupsTime = Db.Queryable<tn_dts_setting>().Where(it => it.cn_s_setting_keycode == "BackupsTime").Select(it => it.cn_s_setting_keyvalue).First();
                saveBackups.HourDayWeek = backupsTime;
                string backupsDiretory = Db.Queryable<tn_dts_setting>().Where(it => it.cn_s_setting_keycode == "BackupsDiretory").Select(it => it.cn_s_setting_keyvalue).First();
                saveBackups.BackupsDiretory = backupsDiretory;
            }
            catch { }
            return saveBackups;
        }
        #endregion
    }
}
