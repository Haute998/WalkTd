using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public class ExcelWay
    {
        /// <summary>
        /// 字段名称
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// 字段ID
        /// </summary>
        public string FieldId { get; set; }
    }
    public class ExportWay
    {

        public static DataTable ExcelDataTable(string StrSql)
        {
            return  DAL.SqlHelper.ReturnDataTableDefault(StrSql);
        }
        public static MemoryStream GetExcel(DataTable dt,string[] ExcelList)
        {
            //创建Excel文件的对象
            NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
            //添加一个sheet
            ISheet sheet1 = book.CreateSheet("Excel");

            //给excel添加第一行的头部标题
            NPOI.SS.UserModel.IRow Rows = sheet1.CreateRow(0);
            for (int i = 0; i < ExcelList.Length; i++)
            {
                Rows.CreateCell(i).SetCellValue(ExcelList[i]);
            }

            //将数据逐步写入excel各个行
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                NPOI.SS.UserModel.IRow rowtemp = sheet1.CreateRow(i + 1);
                for (int j = 0; j < ExcelList.Length; j++)
                {
                    rowtemp.CreateCell(j).SetCellValue(dt.Rows[i][j].ToString());
                }
            }
            //列宽自适应，只对英文和数字有效
            for (int i = 0; i <= dt.Rows.Count; i++)
            {
                sheet1.AutoSizeColumn(i);
            }
            //获取当前列的宽度，然后对比本列的长度，取最大值
            for (int columnNum = 0; columnNum <= dt.Rows.Count; columnNum++)
            {
                int columnWidth = sheet1.GetColumnWidth(columnNum) / 256;
                for (int rowNum = 1; rowNum <= sheet1.LastRowNum; rowNum++)
                {
                    IRow currentRow;
                    //当前行未被使用过
                    if (sheet1.GetRow(rowNum) == null)
                    {
                        currentRow = sheet1.CreateRow(rowNum);
                    }
                    else
                    {
                        currentRow = sheet1.GetRow(rowNum);
                    }

                    if (currentRow.GetCell(columnNum) != null)
                    {
                        ICell currentCell = currentRow.GetCell(columnNum);
                        int length = Encoding.Default.GetBytes(currentCell.ToString()).Length;
                        if (columnWidth < length)
                        {
                            columnWidth = length;
                        }
                    }
                }
                sheet1.SetColumnWidth(columnNum, columnWidth * 280);
            }
            // 写入到客户端 
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            book.Write(ms);
            ms.Seek(0, SeekOrigin.Begin).ToString();
            return ms;
        }
    }
}
