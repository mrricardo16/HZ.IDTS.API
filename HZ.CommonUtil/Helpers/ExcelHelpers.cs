using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace HZ.CommonUtil.Helpers
{
    public class ExcelHelpers
    {
        /// <summary>
        /// 将Excel文件流转成Table
        /// </summary>
        /// <param name="fs">文件流</param>
        /// <param name="validCol">有效列，判断数据是否有效，无值则放弃该列</param>
        /// <param name="dataStart">起始数据行</param>
        /// <returns></returns>
        public static DataTable ReadExcelDate(Stream fs,int validCol=0, int dataStart=0)
        {
            IWorkbook workbook = WorkbookFactory.Create(fs);
            ISheet sheet = null;
            int startRow = 0;
            sheet = workbook.GetSheetAt(0);
            DataTable data = new DataTable();
            if (sheet != null)
            {
                IRow firstRow = sheet.GetRow(dataStart);
                //一行最后一个cell的编号 即总的列数
                int cellCount = firstRow.LastCellNum;
                for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
                {
                    ICell cell = firstRow.GetCell(i);
                    if (cell != null)
                    {
                        string cellValue = cell.StringCellValue;
                        if (cellValue != null)
                        {
                            DataColumn column = new DataColumn(cellValue);
                            data.Columns.Add(column);
                        }
                    }
                }
                startRow = sheet.FirstRowNum + 1+ dataStart;
                try
                {
                    //最后一列的标号
                    int rowCount = sheet.LastRowNum;
                    for (int i = startRow; i <= rowCount; ++i)
                    {
                        IRow row = sheet.GetRow(i);
                        if (row == null) continue; //没有数据的行默认是null　　　　　　　

                        DataRow dataRow = data.NewRow();
                        for (int j = row.FirstCellNum; j < cellCount; ++j)
                        {
                            if (row.GetCell(j)!=null&&row.GetCell(j).ToString()!="") //同理，没有数据的单元格都默认是null
                            {
                                if (row.GetCell(j).CellType == CellType.Numeric)
                                {
                                    if (row.GetCell(j).DateCellValue.Year == 1900)
                                        dataRow[j] = row.GetCell(j).NumericCellValue;
                                    else
                                        dataRow[j] = row.GetCell(j).DateCellValue;
                                }
                                else
                                    dataRow[j] = row.GetCell(j).ToString();
                            }
                        }
                        data.Rows.Add(dataRow);
                    }
                }
                catch(Exception e)
                {
                    var s=e.Message.ToString();
                }
            }
            var tb = data.AsEnumerable().Where(x => !string.IsNullOrEmpty(x.Field<string>(validCol)));
            if (!tb.GetEnumerator().MoveNext())
            {
                data.Rows.Clear();
                return data;
            }
            return tb.CopyToDataTable();
        }
    }
}
