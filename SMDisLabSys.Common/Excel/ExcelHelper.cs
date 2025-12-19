using SMDisLabSys.Common;
using NPOI.HSSF.UserModel;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FATP.BackEnd.Common.ExcelHelper
{
    public class ExcelHelper
    {
        /// <summary>
        /// Excel导入成DataTble
        /// </summary>
        /// <param name="file">导入路径(包含文件名与扩展名)</param>
        /// <returns></returns>
        public static DataTable ExcelToTable(string file)
        {
            if (File.Exists(file))
            {

            }
            DataTable dt = new DataTable();
            IWorkbook workbook;
            string fileExt = Path.GetExtension(file).ToLower();
            using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                if (fileExt == ".xlsx") { workbook = new XSSFWorkbook(fs); } else if (fileExt == ".xls") { workbook = new HSSFWorkbook(fs); } else { workbook = null; }
                if (workbook == null) { return null; }
                ISheet sheet = workbook.GetSheetAt(0);

                //表头  
                IRow header = sheet.GetRow(sheet.FirstRowNum);
                List<int> columns = new List<int>();
                for (int i = 0; i < header.LastCellNum; i++)
                {
                    object obj = GetValueType(header.GetCell(i));
                    if (obj == null || obj.ToString() == string.Empty)
                    {
                        dt.Columns.Add(new DataColumn("Columns" + i.ToString()));
                    }
                    else
                        dt.Columns.Add(new DataColumn(obj.ToString()));
                    columns.Add(i);
                }
                //数据  
                for (int i = sheet.FirstRowNum + 1; i <= sheet.LastRowNum; i++)
                {
                    DataRow dr = dt.NewRow();
                    bool hasValue = false;
                    foreach (int j in columns)
                    {
                        dr[j] = GetValueType(sheet.GetRow(i).GetCell(j));
                        if (dr[j] != null && dr[j].ToString() != string.Empty)
                        {
                            hasValue = true;
                        }
                    }
                    if (hasValue)
                    {
                        dt.Rows.Add(dr);
                    }
                }
            }
            return dt;
        }
        /// <summary>
        /// Excel 读成DataTble
        /// </summary>
        /// <param name="file">导入路径(包含文件名与扩展名)</param>
        /// <returns></returns>
        public static Dictionary<string, DataTable> ExcelToTables(string file)
        {
            Dictionary<string, DataTable> DtDic = new Dictionary<string, DataTable>();

            if (!File.Exists(file))
            {
                return DtDic;
            }

            try
            {
                IWorkbook workbook;
                string fileExt = Path.GetExtension(file).ToLower();
                using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read))
                {
                    if (fileExt == ".xlsx") { workbook = new XSSFWorkbook(fs); } else if (fileExt == ".xls") { workbook = new HSSFWorkbook(fs); } else { workbook = null; }
                    if (workbook == null) { return null; }
                    int sheetIndex = 0;


                    //while (true)
                    {
                        //if (sheetIndex + 1 > workbook.NumberOfSheets)
                        //{
                        //    break;
                        //}
                        ISheet sheet = workbook.GetSheetAt(sheetIndex);
                        try
                        {
                            DataTable dt = new DataTable();
                            //表头  
                            IRow header = sheet.GetRow(sheet.FirstRowNum);
                            List<int> columns = new List<int>();
                            for (int i = 0; i < header.LastCellNum; i++)
                            {
                                object obj = GetValueType(header.GetCell(i));
                                if (obj == null || obj.ToString() == string.Empty)
                                {
                                    dt.Columns.Add(new DataColumn("Columns" + i.ToString()));
                                }
                                else
                                    dt.Columns.Add(new DataColumn(obj.ToString()));
                                columns.Add(i);
                            }
                            //数据  
                            for (int i = sheet.FirstRowNum + 1; i <= sheet.LastRowNum; i++)
                            {
                                DataRow dr = dt.NewRow();
                                bool hasValue = false;
                                foreach (int j in columns)
                                {
                                    dr[j] = GetValueType(sheet.GetRow(i).GetCell(j));
                                    if (dr[j] != null && dr[j].ToString() != string.Empty)
                                    {
                                        hasValue = true;
                                    }
                                }
                                if (hasValue)
                                {
                                    dt.Rows.Add(dr);
                                }
                            }
                            DtDic.Add(sheet.SheetName, dt);
                            sheetIndex++;

                        }
                        catch (Exception exc)
                        {
                            LogMgr.Instance.Warn($"{file}读取出错 sheet={sheet.SheetName} sheet lastRowNum={sheet.LastRowNum}");
                            sheetIndex++;
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                LogMgr.Instance.Warn($"{file}读取出错");
            }
            return DtDic;
        }
        /// <summary>
        /// 获取单元格类型
        /// </summary>
        /// <param name="cell">目标单元格</param>
        /// <returns></returns>
        private static object GetValueType(ICell cell)
        {
            if (cell == null)
                return null;
            switch (cell.CellType)
            {
                case CellType.Blank:
                    return null;
                case CellType.Boolean:
                    return cell.BooleanCellValue;
                case CellType.Numeric:
                    return cell.NumericCellValue;
                case CellType.String:
                    return cell.StringCellValue;
                case CellType.Error:
                    return cell.ErrorCellValue;
                case CellType.Formula:
                default:
                    return "=" + cell.CellFormula;
            }
        }
    }
}
