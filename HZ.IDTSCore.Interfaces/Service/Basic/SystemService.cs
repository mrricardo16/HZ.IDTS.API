using HZ.CommonUtil.Helpers;
using HZ.CommonUtil.Model;
using HZ.DbHelper;
using HZ.IDTSCore.Common;
using HZ.IDTSCore.Common.Const;
using HZ.IDTSCore.Common.Helpers;
using HZ.IDTSCore.Interfaces.IService;
using HZ.IDTSCore.Model;
using HZ.IDTSCore.Model.Entity.Basic;
using HZ.IDTSCore.Model.Entity.Sys;
using HZ.IDTSCore.Model.InterfaceParams;
using HZ.IDTSCore.Model.View;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace HZ.IDTSCore.Interfaces.Service
{
    public class SystemService : BaseService<object>, ISystemService
    {
        public SystemService(SessionInfo session) : base(session)
        {

        }

        public ApiResult BackupRemoteDatabase(string remoteServer, string remoteUser, string remotePassword, string remoteDatabase, string dumpFilePath)
        {
            ApiResult result = new ApiResult();

            

            return result;
        }

        public ApiResult BackupRemoteDatabaseV2(string remoteServer, string remoteUser, string remotePassword, string remoteDatabase, string dumpFilePath)
        {
            ApiResult result = new ApiResult();
            string pgDumpPath = @"E:\Software\PostgreSQL\bin\pg_dump.exe";// pg_dump 在 Windows 服务器上的路径
            string fullFileName = "idts_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".dump";
            string outputPath = Path.Combine(dumpFilePath, fullFileName);
            // 构建 pg_dump 命令
            string pgDumpCommand = $"{pgDumpPath} -U {remoteUser} -d {remoteDatabase} -f \"{outputPath}\"";

            // 拼接完整的 Powershell 远程执行命令
            //string fullCommand = $"powershell -Command \"& {{Invoke-Command -ComputerName {remoteServer} -ScriptBlock {{cmd /c \\\"{pgDumpCommand}\\\"}} -Credential (New-Object System.Management.Automation.PSCredential(\\\"{remoteUser}\\\", (ConvertTo-SecureString \\\"{remotePassword}\\\" -AsPlainText -Force)))}}\"";
            //string fullCommand = $"powershell -Command \"Invoke-Command -ComputerName {remoteServer} -ScriptBlock {{cmd /c \\\"{pgDumpCommand}\\\"}} -Credential (New-Object System.Management.Automation.PSCredential(\\\"{remoteUser}\\\", (ConvertTo-SecureString \\\"{remotePassword}\\\" -AsPlainText -Force)))\"";
            //string fullCommand = $"powershell -Command \"Invoke-Command -ComputerName {remoteServer} -ScriptBlock {{& cmd /c '{pgDumpCommand}'}} -Credential (New-Object System.Management.Automation.PSCredential('{remoteUser}', (ConvertTo-SecureString '{remotePassword}' -AsPlainText -Force)))\"";
            string fullCommand = $"Invoke-Command -ComputerName {remoteServer} -ScriptBlock {{cmd /c \\\"{pgDumpCommand}\\\"}} -Credential (New-Object System.Management.Automation.PSCredential(\\\"{remoteUser}\\\", (ConvertTo-SecureString \\\"{remotePassword}\\\" -AsPlainText -Force)))";

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "powershell",
                    Arguments = fullCommand,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };
            StringBuilder errorBuilder = new StringBuilder();
            process.OutputDataReceived += (s, e) => Console.WriteLine(e.Data);
            process.ErrorDataReceived += (s, e) =>
            {
                Console.WriteLine(e.Data);
                errorBuilder.AppendLine(e.Data); // 收集错误信息
            };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();

            if (process.ExitCode == 0)
            {
                Console.WriteLine("备份完成！");
                result.IsSuccess = true;
            }
            else
            {
                Console.WriteLine("备份失败！");
                result.IsSuccess = false;
                result.Message = errorBuilder.ToString();
            }
            return result;
        }

        public void BackupRemoteDatabaseLinux(string remoteServer, string remoteUser, string remotePassword, string remoteDatabase, string dumpFilePath)
        {
            string pgDumpPath = "/usr/bin/pg_dump"; // PostgreSQL 服务器上的 pg_dump 路径

            // 构建 pg_dump 命令
            string pgDumpCommand = $"{pgDumpPath} -U {remoteUser} -d {remoteDatabase} -f \"{dumpFilePath}\"";

            // 拼接完整的 SSH 命令
            string fullCommand = $"ssh {remoteUser}@{remoteServer} \"{pgDumpCommand}\"";

            Process process = new Process();
            process.StartInfo.FileName = "bash";
            process.StartInfo.Arguments = $"-c \"{fullCommand}\"";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;

            process.OutputDataReceived += (sender, e) => Console.WriteLine(e.Data);
            process.ErrorDataReceived += (sender, e) => Console.WriteLine(e.Data);

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            process.WaitForExit();

            if (process.ExitCode == 0)
            {
                Console.WriteLine("备份完成！");
            }
            else
            {
                Console.WriteLine("备份失败！");
            }
        }

        public ApiResult BackupRemoteDatabaseV1(string remoteServer, string remoteUser, string remotePassword, string remoteDatabase, string dumpFilePath)
        {
            ApiResult result = new ApiResult();
            string fullFileName = "idts_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".dump";

            string outputPath = Path.Combine(dumpFilePath, fullFileName);

            string pgDumpPath = @"E:\Software\PostgreSQL\bin\pg_dump"; // pg_dump 可执行文件路径

            // 构建远程执行命令
            string sshCommand = $"ssh {remoteUser}@{remoteServer}";

            // 构建 pg_dump 命令参数
            string pgDumpCommand = $"pg_dump -U {remoteUser} -d {remoteDatabase} -f \"{outputPath}\"";

            // 拼接完整的远程执行命令
            string fullCommand = $"{sshCommand} \"{pgDumpPath} {pgDumpCommand}\"";

            Process process = new Process();
            process.StartInfo.FileName = "bash"; // 或者使用 ssh 客户端的路径
            process.StartInfo.Arguments = $"-c \"{fullCommand}\"";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;

            process.OutputDataReceived += (sender, e) => Console.WriteLine(e.Data);
            process.ErrorDataReceived += (sender, e) => Console.WriteLine(e.Data);

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (process.ExitCode == 0)
            {
                Console.WriteLine("备份完成！");
                result.IsSuccess = true;
                result.Message = error;
            }
            else
            {
                Console.WriteLine("备份失败！");
                result.IsSuccess = false;
                result.Message = error;
            }

            return result;
        }


        /// <summary>
        /// 数据库备份
        /// </summary>
        /// <param name="backupFilePath">备份的路径(绝对路径)</param>
        /// <returns></returns>
        public ApiResult DatabaseBackup(string backupFilePath)
        {
            ApiResult result = new ApiResult();
            string fullFileName = "idts_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".dump";
            // connectionString = Db.CurrentConnectionConfig.ConnectionString;
            //string outputPath = backupFilePath.Replace(@"\\", @"\") + fullFileName;
            //string pgDumpPath = Db.Queryable<tn_dts_setting>().Where(it => it.cn_s_setting_keycode == "PostgreSQLPgdumpDiretory").Select(it => it.cn_s_setting_keyvalue).First();
            string outputPath = Path.Combine(backupFilePath, fullFileName);
            //string pgDumpPath = @"E:\Software\PostgreSQL\pgsql\bin\pg_dump";
            string pgDumpPath = @"E:\Software\PostgreSQL\bin\pg_dump";
            string dbName = "hz_idts";
            string dbUser = "postgres";
            string dbPassword = "123456";
            string pgDumpArguments = $"-Fc -f \"{outputPath}\" -U {dbUser} {dbName}";

            using (Process process = new Process())
            {
                process.StartInfo.FileName = pgDumpPath;
                process.StartInfo.Arguments = pgDumpArguments;
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
                    //备份成功
                    result.IsSuccess = true;
                    result.StatusCode = 200;
                    result.Data = outputPath;
                    result.ErrCode = 0;
                    result.Message = "Backup completed successfully.指令=" + pgDumpArguments + "";
                }
                else
                {
                    // 备份失败
                    result.IsSuccess = false;
                    result.StatusCode = 500;
                    result.Data = outputPath;
                    result.ErrCode = -1;
                    result.Message = "Backup failed. Error: " + error + ";指令=" + pgDumpArguments + "";
                }
            }
            return result;
        }

        /// <summary>
        /// 还原数据库
        /// </summary>
        /// <param name="pathToPgRestore"></param>
        /// <param name="backupPath"></param>
        /// <param name="dbName"></param>
        /// <param name="dbUser"></param>
        /// <param name="dbPassword"></param>
        public void RestoreDatabase(string pathToPgRestore, string backupPath, string dbName, string dbUser, string dbPassword)
        {
            using (Process process = new Process())
            {
                process.StartInfo.FileName = pathToPgRestore;
                process.StartInfo.Arguments = $"-Fc -U {dbUser} -d {dbName} \"{backupPath}\"";
                process.StartInfo.Environment["PGPASSWORD"] = dbPassword;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.UseShellExecute = false;
                process.Start();

                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();

                process.WaitForExit();

                if (process.ExitCode != 0)
                    throw new Exception($"Restore failed: {error}");
            }
        }

        public List<sys_menu> GetAllMenus(string sourceTerminal)
        {
            UserSession user = GetSessionInfo();

            string mdg = IDTSCore.Common.Const.SysConst.MDGApi;// AppSettings.GetValue<string>("SysInterface:Mdg");

            var entity = new
            {
                parentCode = "-1",
                showType = sourceTerminal == "PC" ? 0 : 1,
                //appCode = "WMS"
                appCode = "DTS"
            };

            UserSession sessionUser = GetSessionInfo();
            ApiResult apiR = new ApiResult();
            string result = WebApiManager.HttpPost(mdg, "api/Power/GetMenu", JsonConvert.SerializeObject(entity), ref apiR, sessionUser.TokenId, sessionUser.OrgFlag);

            if (!apiR.IsSuccess)
                throw new Exception(apiR.Message);
            if (result == "")
                return new List<sys_menu>();

            apiR = JsonConvert.DeserializeObject<ApiResult>(result);
            if (!apiR.IsSuccess)
                return new List<sys_menu>();
            List<menu> menu = JsonConvert.DeserializeObject<List<menu>>(apiR.Data.ToString());

            //if (AppSettings.GetValue<bool>("AppSettings:MergeMDG"))
            //{
            //    entity = new
            //    {
            //        parentCode = "-1",
            //        showType = sourceTerminal == "PC" ? 0 : 1,
            //        appCode = "MDG"
            //    };
            //    result = WebApiManager.HttpPost(mdg, "api/Power/GetMenu", JsonConvert.SerializeObject(entity), ref apiR, sessionUser.TokenId);
            //    if (!apiR.IsSuccess)
            //        throw new Exception(apiR.Message);
            //    if (result != "")
            //    {
            //        apiR = JsonConvert.DeserializeObject<ApiResult>(result);
            //        if (!apiR.IsSuccess)
            //            return new List<sys_menu>();
            //        menu.AddRange(JsonConvert.DeserializeObject<List<menu>>(apiR.Data.ToString()));
            //    }
            //}

            List<sys_menu> sysMenu = menu.Select(x => new sys_menu()
            {
                Id = x.id,
                MenuId = x.id,
                MenuUrlCode = x.path,
                MenuComponentUrl = x.component,
                MenuName = x.name,
                MenuOrder = 1,
                MenuParentId = x.parentId,
                TitleName = x.meta.title,
                ShowMenu = true,
                Folder = true,
                Remark = ""
            }).ToList();

            List<string> mdgExist = new List<string>();
            menu.ForEach(x =>
             mdgExist.AddRange((x.children.Select(x => x.path).ToList())));

            menu.ForEach(x =>
            {
                sysMenu.AddRange(x.children.Select(y => new sys_menu()
                {
                    Id = y.id,
                    MenuId = y.id,// Guid.NewGuid().ToString(),
                    MenuUrlCode = y.path,
                    MenuComponentUrl = y.component,
                    MenuName = y.name,
                    MenuOrder = 1,
                    MenuParentId = y.parentId,
                    TitleName = y.meta.title,
                    ShowMenu = true,
                    Folder = false,
                    Remark = ""
                }));

                x.children.ForEach(x =>
                {
                    sysMenu.AddRange(
                            x.children.Select(y => new sys_menu()
                            {
                                Id = y.id,
                                MenuId = y.id,// Guid.NewGuid().ToString(),
                                MenuUrlCode = y.path,
                                MenuComponentUrl = y.component,
                                MenuName = y.name,
                                MenuOrder = 1,
                                MenuParentId = y.parentId,
                                TitleName = y.meta.title,
                                ShowMenu = true,
                                Folder = false,
                                Remark = ""
                            })
                        );

                });

                if (sourceTerminal == "PC")
                {
                    var m = Db.Queryable<sys_menu>().First(y => y.MenuUrlCode == x.path.TrimStart('/'));
                    if (m != null)
                    {
                        sysMenu.AddRange(Db.Queryable<sys_menu>().Where(x => x.ShowMenu == false && x.MenuParentId == m.MenuId
                        && !mdgExist.Contains(x.MenuUrlCode)).Select(y =>
                        new sys_menu()
                        {
                            Id = y.Id,
                            MenuId = y.MenuId,// Guid.NewGuid().ToString(),
                            MenuUrlCode = y.MenuUrlCode,
                            MenuComponentUrl = y.MenuComponentUrl,
                            MenuName = y.MenuName,
                            MenuOrder = 1,
                            MenuParentId = x.id,
                            TitleName = y.TitleName,
                            ShowMenu = y.ShowMenu,
                            Folder = y.Folder,
                            Remark = ""
                        }).ToList());
                    }

                    var collectMenu = Db.Queryable<tn_wms_menu_collect>().Where(x => x.cn_s_user_code == user.UserCode).ToList();
                    sysMenu.Where(x => collectMenu.Select(y => y.cn_s_menu_url).ToArray().Contains(x.MenuUrlCode)).ToList().ForEach(y => y.IsCollection = true);
                }
            });

            if (sourceTerminal != "PC")
            {
                return sysMenu.Where(e => e.MenuParentId != "-1").Select(
                    x => new sys_menu()
                    {
                        TitleName = x.TitleName,
                        MenuName = x.MenuName,
                        MenuUrlCode = x.MenuComponentUrl,
                    }
                    ).Where(x => x.MenuUrlCode != "").ToList();
            }

            string s = JsonConvert.SerializeObject(sysMenu);
            return sysMenu;
        }

        public List<string> GetButton(string pageName)
        {
            string mdg = IDTSCore.Common.Const.SysConst.MDGApi;// AppSettings.GetValue<string>("SysInterface:Mdg");
            UserSession sessionUser = GetSessionInfo();

            ApiResult apiR = new ApiResult();
            string result = WebApiManager.HttpGet(mdg, "api/Power/GetMenuButtonPower?cName=" + Uri.EscapeDataString(pageName ?? ""), ref apiR, "", sessionUser.TokenId);

            if (!apiR.IsSuccess)
                throw new Exception(apiR.Message);

            if (result == "")
                return new List<string>();

            apiR = JsonConvert.DeserializeObject<ApiResult>(result);
            if (!apiR.IsSuccess)
                return new List<string>();
            else
                return JsonConvert.DeserializeObject<BtnPower>(apiR.Data.ToString()).menuButtons;
        }
        public List<sys_menu> GetMenus(int type)
        {
            List<sys_menu> menus = new List<sys_menu>();
            UserSession user = GetSessionInfo();
            var entity = new
            {
                parentCode = "",
                showType = type
            };

            string mdg = IDTSCore.Common.Const.SysConst.MDGApi;// AppSettings.GetValue<string>("SysInterface:Mdg");
            ApiResult apiR = new ApiResult();
            string result = WebApiManager.HttpPost(mdg, "api/Power/GetMenu", JsonConvert.SerializeObject(entity), ref apiR, user.TokenId);
            if (!apiR.IsSuccess)
                throw new Exception(apiR.Message);

            try
            {
                if (result == "")
                    return new List<sys_menu>();

                apiR = JsonConvert.DeserializeObject<ApiResult>(result);
                if (!apiR.IsSuccess)
                    return new List<sys_menu>();

                List<menu> menu = JsonConvert.DeserializeObject<List<menu>>(apiR.Data.ToString());

                List<sys_menu> sysMenu = menu.Select(x => new sys_menu()
                {

                    MenuComponentUrl = x.component,
                    MenuName = x.meta.title,

                }).ToList();

                menu.ForEach(x =>
                {
                    sysMenu.AddRange(x.children.Select(y => new sys_menu()
                    {
                        MenuComponentUrl = y.component,
                        MenuName = y.meta.title,
                    }));


                });


                string s = JsonConvert.SerializeObject(sysMenu);
                return sysMenu;
            }
            catch (Exception ex)
            {
                throw new Exception(mdg);
            }
        }


        public List<sys_menu> GetPDAMenus()
        {
            UserSession user = GetSessionInfo();
            var entity = new
            {
                userName = user.UserCode,
                parentCode = "0",
                appCode = "WMS",
                showType = 1
            };

            string mdg = IDTSCore.Common.Const.SysConst.MDGApi;// AppSettings.GetValue<string>("SysInterface:Mdg");
            ApiResult res = new ApiResult();
            string result = WebApiManager.HttpPost(mdg, "api/Menu/GetMenuByUser", JsonConvert.SerializeObject(entity), ref res);

            if (!res.IsSuccess)
                throw new Exception(res.Message);

            try
            {
                tn_mdg_menu menu = JsonConvert.DeserializeObject<tn_mdg_menu>(result);

                List<sys_menu> menus = new List<sys_menu>();
                menu.UserMenuList.RemoveAll(x => x.name == "移动端");
                menu.UserMenuList.ForEach(x =>
                {

                    menus.Add(new sys_menu()
                    {
                        MenuUrlCode = x.url,
                        MenuName = x.name
                    });
                });
                return menus;
            }
            catch (Exception ex)
            {
                throw new Exception(mdg);
            }
        }
        public List<tn_wms_view_table_def> GetPageDefTree()
        {
            List<tn_wms_view_table_def> list = Db.Queryable<tn_wms_view_table_def>().ToList();

            List<tn_wms_view_table_def> result = new List<tn_wms_view_table_def>();

            result.AddRange(list.FindAll(x => string.IsNullOrEmpty(x.cn_s_parent_guid)));

            ListToTree(ref result, list);
            return result;
        }

        private void ListToTree(ref List<tn_wms_view_table_def> result, List<tn_wms_view_table_def> source)
        {
            foreach (var m in result)
            {
                var child = source.FindAll(x => x.cn_s_parent_guid == m.cn_s_guid);
                if (child.Any())
                {
                    m.child.AddRange(child);
                    ListToTree(ref child, source);
                }
            }
        }

        /// <summary>
        /// 获取用户配置的列
        /// </summary>
        /// <param name="userCode"></param>
        /// <returns></returns>
        public List<tn_wms_view_table_conf> GetTableColumns(string userCode)
        {
            bool multiOwner = WmsSysSet.GetBoolValue(GetCurrSession(), SysKeyword.SysSet_EnableMultiOwner);

            var v = Db.Queryable<tn_wms_view_table_conf, tn_wms_view_user_conf>((a, b) =>
            new object[] { JoinType.Left, a.cn_s_power_code == b.cn_s_power_code && a.cn_s_column_code == b.cn_s_column_code })
                    .Where((a, b) => b.cn_b_enabled == true && b.cn_s_user_code == userCode)
                    .WhereIF(!multiOwner, (a, b) => a.cn_s_column_code != "cn_s_owner")
                    .OrderBy(a => a.cn_n_order, OrderByType.Asc)
                    .Select((a, b) => new tn_wms_view_table_conf()
                    {
                        cn_b_is_built_in = a.cn_b_is_built_in,
                        cn_b_is_sort = a.cn_b_is_sort,
                        cn_b_is_valid = a.cn_b_is_valid,
                        cn_s_input_type = a.cn_s_input_type,
                        cn_s_link = a.cn_s_link,
                        cn_s_method_name = a.cn_s_method_name,
                        cn_s_column_code = a.cn_s_column_code,
                        cn_s_power_code = a.cn_s_power_code,
                        cn_s_power_name = a.cn_s_power_name,
                        cn_s_table = a.cn_s_table,
                        cn_s_column_name = b.cn_s_column_name,
                        cn_n_order = b.cn_n_order,
                        cn_n_width = b.cn_n_width,
                        cn_s_align = b.cn_s_align,
                        //cn_b_enabled = b.cn_b_enabled,
                        cn_s_dict_name = a.cn_s_dict_name,
                        cn_b_required = a.cn_b_required,
                        cn_b_active = a.cn_b_active,
                        cn_s_obj = a.cn_s_obj,
                        cn_n_max_length = a.cn_n_max_length
                    }).ToList();

            if (v.Count == 0)
            {
                List<tn_wms_view_user_conf> userCol = new List<tn_wms_view_user_conf>();
                List<tn_wms_view_table_conf> defCol = Db.Queryable<tn_wms_view_table_conf>().Where(x => x.cn_b_is_valid).OrderBy(x => x.cn_n_order).ToList();
                defCol.ForEach(x =>
                {
                    userCol.Add(new tn_wms_view_user_conf()
                    {
                        cn_s_guid = Guid.NewGuid().ToString(),
                        cn_b_enabled = true,
                        cn_n_order = x.cn_n_order,
                        cn_n_width = x.cn_n_width,
                        cn_s_align = x.cn_s_align,
                        cn_s_column_code = x.cn_s_column_code,
                        cn_s_column_name = x.cn_s_column_name,
                        cn_s_power_code = x.cn_s_power_code,
                        cn_s_power_name = x.cn_s_power_name,
                        cn_s_user_code = userCode,
                        cn_s_creator = userCode,
                        cn_t_create = DateTime.Now,
                    });
                });
                Db.Insertable<tn_wms_view_user_conf>(userCol).ExecuteCommand();
                if (multiOwner)
                    return defCol;
                else
                    return defCol.Where(x => x.cn_s_column_code != "cn_s_owner").ToList();
            }
            return v;
        }

        public List<tn_wms_view_table_conf> GetTableColumns(string userCode, string powerCode)
        {
            bool multiOwner = WmsSysSet.GetBoolValue(GetCurrSession(), SysKeyword.SysSet_EnableMultiOwner);
            var v = Db.Queryable<tn_wms_view_table_conf, tn_wms_view_user_conf>((a, b) =>
            new object[] { JoinType.Left, a.cn_s_power_code == b.cn_s_power_code && a.cn_s_column_code == b.cn_s_column_code })
                    .Where((a, b) => a.cn_s_power_code == powerCode && b.cn_s_user_code == userCode)
                    .WhereIF(!multiOwner, (a, b) => a.cn_s_column_code != "cn_s_owner")
                    .OrderBy((a, b) => b.cn_n_order, OrderByType.Asc)
                    .Select((a, b) => new tn_wms_view_table_conf()
                    {
                        cn_b_is_built_in = a.cn_b_is_built_in,
                        cn_b_is_sort = a.cn_b_is_sort,
                        cn_b_is_valid = a.cn_b_is_valid,
                        cn_s_input_type = a.cn_s_input_type,
                        cn_s_link = a.cn_s_link,
                        cn_s_method_name = a.cn_s_method_name,
                        cn_s_column_code = a.cn_s_column_code,
                        cn_s_column_name = b.cn_s_column_name,
                        cn_s_power_code = a.cn_s_power_code,
                        cn_s_power_name = a.cn_s_power_name,
                        cn_s_table = a.cn_s_table,
                        cn_n_order = b.cn_n_order,
                        cn_n_width = b.cn_n_width,
                        cn_s_align = b.cn_s_align,
                        cn_b_enabled = b.cn_b_enabled,
                        cn_s_dict_name = a.cn_s_dict_name,
                        cn_b_required = a.cn_b_required,
                        cn_s_obj = a.cn_s_obj,
                        cn_b_active = a.cn_b_active
                    }).ToList();

            if (v.Count == 0)
            {
                List<tn_wms_view_user_conf> userCol = new List<tn_wms_view_user_conf>();
                List<tn_wms_view_table_conf> defCol = Db.Queryable<tn_wms_view_table_conf>().Where(x => x.cn_s_power_code == powerCode
                && x.cn_b_is_valid).WhereIF(!multiOwner, x => x.cn_s_column_code != "cn_s_owner").OrderBy(x => x.cn_n_order).ToList();
                defCol.ForEach(x =>
                {
                    userCol.Add(new tn_wms_view_user_conf()
                    {
                        cn_s_guid = Guid.NewGuid().ToString(),
                        cn_b_enabled = true,
                        cn_n_order = x.cn_n_order,
                        cn_n_width = x.cn_n_width,
                        cn_s_align = x.cn_s_align,
                        cn_s_column_code = x.cn_s_column_code,
                        cn_s_column_name = x.cn_s_column_name,
                        cn_s_power_code = x.cn_s_power_code,
                        cn_s_power_name = x.cn_s_power_name,
                        cn_s_user_code = userCode,
                        cn_s_creator = userCode,
                        cn_t_create = DateTime.Now,
                    });
                });
                Db.Insertable<tn_wms_view_user_conf>(userCol).ExecuteCommand();
                return defCol;
            }
            return v;
        }

        public int SaveCusColumns(List<tn_wms_view_table_conf> columns)
        {
            UserSession user = GetSessionInfo();
            foreach (tn_wms_view_table_conf m in columns)
            {
                int result = Db.Updateable<tn_wms_view_user_conf>().SetColumns(x => new tn_wms_view_user_conf()
                {
                    cn_n_order = m.cn_n_order,
                    cn_b_enabled = m.cn_b_enabled,
                    cn_s_align = m.cn_s_align,
                    cn_s_column_name = m.cn_s_column_name,
                    cn_n_width = m.cn_n_width
                }).Where(x => x.cn_s_power_code == m.cn_s_power_code && x.cn_s_column_code == m.cn_s_column_code && x.cn_s_user_code == user.UserCode).ExecuteCommand();
            }
            return 1;
        }

        public List<tn_wms_view_table_conf> GetConfList(string powerCode)
        {
            bool multiOwner = WmsSysSet.GetBoolValue(GetCurrSession(), SysKeyword.SysSet_EnableMultiOwner);
            return Db.Queryable<tn_wms_view_table_conf>()
                .WhereIF(!string.IsNullOrEmpty(powerCode), x => x.cn_s_power_code == powerCode)
                .WhereIF(!multiOwner, x => x.cn_s_column_code != "cn_s_owner").OrderBy(x => x.cn_n_order).ToList();
        }

        public ApiResult SavePageBasicSet(tn_wms_view_table_conf model)
        {
            int affected = Db.Updateable<tn_wms_view_table_conf>().SetColumns(x => new tn_wms_view_table_conf()
            {
                //cn_b_enabled = model.cn_b_enabled,
                cn_s_input_type = model.cn_s_input_type,
                cn_b_is_built_in = model.cn_b_is_built_in,
                cn_b_is_sort = model.cn_b_is_sort,
                cn_b_is_valid = model.cn_b_is_valid,
                cn_b_required = model.cn_b_required,
                cn_n_order = model.cn_n_order,
                cn_n_width = model.cn_n_width,
                cn_s_align = model.cn_s_align,
                cn_s_column_name = model.cn_s_column_name,
                cn_s_power_name = model.cn_s_power_name,
                cn_n_max_length = model.cn_n_max_length
            }).Where(x => x.cn_s_power_code == model.cn_s_power_code && x.cn_s_column_code == model.cn_s_column_code).ExecuteCommand();

            if (model.cn_b_is_valid)
            {
                //启动
                List<tn_wms_view_user_conf> userConfs = new List<tn_wms_view_user_conf>();
                List<string> user = Db.Queryable<tn_wms_view_user_conf>().GroupBy(x => new { x.cn_s_user_code }).Select(x => x.cn_s_user_code).ToList();
                foreach (string m in user)
                {
                    int exist = Db.Queryable<tn_wms_view_user_conf>().Where(x => x.cn_s_user_code == m && x.cn_s_column_code == model.cn_s_column_code && x.cn_s_power_code == model.cn_s_power_code).Count();
                    if (exist == 0)
                    {
                        userConfs.Add(new tn_wms_view_user_conf()
                        {
                            cn_b_enabled = model.cn_b_is_valid,// model.cn_b_enabled,
                            cn_n_order = model.cn_n_order,
                            cn_n_width = model.cn_n_width,
                            cn_s_align = model.cn_s_align,
                            cn_s_column_code = model.cn_s_column_code,
                            cn_s_column_name = model.cn_s_column_name,
                            cn_s_guid = Guid.NewGuid().ToString(),
                            cn_s_power_code = model.cn_s_power_code,
                            cn_s_power_name = model.cn_s_power_name,
                            cn_s_user_code = m,
                        });
                    }
                }
                Db.Insertable<tn_wms_view_user_conf>(userConfs).ExecuteCommand();
            }
            else
            {
                //禁用
                Db.Deleteable<tn_wms_view_user_conf>().Where(x => x.cn_s_power_code == model.cn_s_power_code && x.cn_s_column_code == model.cn_s_column_code).ExecuteCommand();
            }

            affected = Db.Updateable<tn_wms_view_user_conf>().SetColumns(x => new tn_wms_view_user_conf()
            {
                cn_b_enabled = model.cn_b_is_valid,//.cn_b_enabled,
                cn_n_order = model.cn_n_order,
                cn_n_width = model.cn_n_width,
                cn_s_align = model.cn_s_align,
                cn_s_column_name = model.cn_s_column_name,
                cn_s_power_name = model.cn_s_power_name,
            }).Where(x => x.cn_s_power_code == model.cn_s_power_code && x.cn_s_column_code == model.cn_s_column_code).ExecuteCommand();
            return ApiResult.Success();
        }

        public ApiResult Add(tn_wms_view_table_conf model)
        {
            var v = Db.Queryable<tn_wms_view_table_conf>().First(x => x.cn_s_power_code == model.cn_s_power_code && x.cn_s_column_code == model.cn_s_column_code);
            if (v != null)
            {
                return SavePageBasicSet(model);
            }
            else
            {
                int affected = Db.Insertable<tn_wms_view_table_conf>(model).ExecuteCommand();
                if (model.cn_b_is_valid)
                {
                    //启动
                    List<tn_wms_view_user_conf> userConfs = new List<tn_wms_view_user_conf>();
                    List<string> user = Db.Queryable<tn_wms_view_user_conf>().GroupBy(x => new { x.cn_s_user_code }).Select(x => x.cn_s_user_code).ToList();
                    foreach (string m in user)
                    {
                        userConfs.Add(new tn_wms_view_user_conf()
                        {
                            cn_b_enabled = true,// model.cn_b_enabled,
                            cn_n_order = model.cn_n_order,
                            cn_n_width = model.cn_n_width,
                            cn_s_align = model.cn_s_align,
                            cn_s_column_code = model.cn_s_column_code,
                            cn_s_column_name = model.cn_s_column_name,
                            cn_s_guid = Guid.NewGuid().ToString(),
                            cn_s_power_code = model.cn_s_power_code,
                            cn_s_power_name = model.cn_s_power_name,
                            cn_s_user_code = m,
                        });
                    }
                    Db.Insertable<tn_wms_view_user_conf>(userConfs).ExecuteCommand();
                }
                return ApiResult.Success();
            }
        }

        public ApiResult IsCollection(string menuurlcode)
        {
            UserSession user = GetSessionInfo();
            tn_wms_menu_collect model = new tn_wms_menu_collect()
            {
                cn_s_menu_url = menuurlcode,
                cn_s_user_code = user.UserCode
            };
            var ss = Db.Deleteable<tn_wms_menu_collect>().Where(x => x.cn_s_menu_url == menuurlcode && x.cn_s_user_code == user.UserCode).ExecuteCommand();
            if (ss == 1)
            {
                return ApiResult.Success("取消收藏成功！");
            }
            else
            {
                Db.Insertable<tn_wms_menu_collect>(model).ExecuteCommand();
                return ApiResult.Success("收藏成功！");
            }

            //var ss = Db.Updateable<sys_menu>().SetColumns(x =>
            //     new sys_menu() { IsCollection = !x.IsCollection }).Where(x => x.MenuUrlCode == menuurlcode).ExecuteCommand();
            //if (ss == 1)
            //{
            //    return ApiResult.Success("收藏成功！");
            //}
            //else
            //{
            //    return ApiResult.Error("收藏失败！");
            //}
        }
        public ApiResult UpdateMenus(sys_menu mode)
        {
            var s = Db.Updateable<sys_menu>(mode).ExecuteCommand();
            if (s == 1)
            {
                return ApiResult.Success("success");
            }
            else
            {
                return ApiResult.Error();
            }
        }

        public ApiResult AddMenus(sys_menu mode)
        {
            mode.Id = Guid.NewGuid().ToString();
            var s = Db.Insertable<sys_menu>(mode).ExecuteCommand();
            if (s == 1)
            {
                return ApiResult.Success("success");
            }
            else
            {
                return ApiResult.Error();
            }
        }

        public ApiResult SaveNode(tn_wms_view_table_def model)
        {
            if (string.IsNullOrEmpty(model.cn_s_guid))
            {
                model.cn_s_guid = Guid.NewGuid().ToString();
                Db.Insertable<tn_wms_view_table_def>(model).ExecuteCommand();
            }
            else
            {
                Db.Updateable<tn_wms_view_table_def>().SetColumns(x => new tn_wms_view_table_def()
                {
                    cn_b_is_edit = model.cn_b_is_edit,
                    cn_s_org_code = model.cn_s_org_code,
                    cn_s_power_code = model.cn_s_power_code,
                    cn_s_power_name = model.cn_s_power_name,
                    cn_s_show_style = model.cn_s_show_style,
                    cn_t_modify = DateTime.Now
                }).Where(x => x.cn_s_guid == model.cn_s_guid).ExecuteCommand();
            }
            return ApiResult.Success();
        }

        public ApiResult DeleteNode(string guid)
        {
            Db.Deleteable<tn_wms_view_table_def>(x => x.cn_s_guid == guid).ExecuteCommand();
            return ApiResult.Success();
        }
    }
}
