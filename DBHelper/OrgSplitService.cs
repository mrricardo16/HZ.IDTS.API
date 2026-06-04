using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace HZ.DbHelper
{
    public class OrgSplitService : ISplitTableService
    {
        public List<SplitTableInfo> GetAllTables(ISqlSugarClient db, EntityInfo EntityInfo, List<DbTableInfo> tableInfos)
        {
            List<SplitTableInfo> result = new List<SplitTableInfo>();
            foreach (var item in tableInfos)
            {
                var tableName = EntityInfo.DbTableName.Replace("_{orgcode}", "");
                if (EntityInfo.DbTableName.Contains("_{orgcode}") && item.Name.Contains(tableName))// MySpliteTest_202204  这种格式的表
                {
                    SplitTableInfo tableInfo = new SplitTableInfo();
                    //tableInfo.TableName = item.Name;
                    //var value = Regex.Match(item.Name, @"\d{6}$").Value;
                    //value = value.Insert(4, "-");
                    //tableInfo.Date = Convert.ToDateTime(value + "-01");
                    tableInfo.TableName = item.Name + "_001";
                    result.Add(tableInfo);
                }
            }
            return result;
        }

        public object GetFieldValue(ISqlSugarClient db, EntityInfo entityInfo, SplitType splitType, object entityValue)
        {
            var splitColumn = entityInfo.Columns.FirstOrDefault(it => it.PropertyInfo.GetCustomAttribute<SplitFieldAttribute>() != null);
            if (splitColumn == null)
            {
                return db.GetDate();
            }
            else
            {
                var value = splitColumn.PropertyInfo.GetValue(entityValue, null);
                return value;
            }
        }

        public string GetTableName(ISqlSugarClient db, EntityInfo EntityInfo)
        {
            return EntityInfo.DbTableName + "_001";//.Replace("{yyyyMM}", DateTime.Now.ToString("yyyyMM"));
        }

        public string GetTableName(ISqlSugarClient db, EntityInfo EntityInfo, SplitType type)
        {
            return EntityInfo.DbTableName + "_001";//.Replace("{yyyyMM}", DateTime.Now.ToString("yyyyMM"));
        }

        public string GetTableName(ISqlSugarClient db, EntityInfo entityInfo, SplitType splitType, object fieldValue)
        {
            return entityInfo.DbTableName + "_001";//.Replace("{yyyyMM}", Convert.ToDateTime(fieldValue).ToString("yyyyMM"));
        }
    }
}
