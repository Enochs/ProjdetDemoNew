using Microsoft.VisualBasic.FileIO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb; 
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Pro.Utils
{
    public class ImportExcelHelper
    {
        /// <summary>
        /// 导入Excel
        /// </summary>
        public static List<T> GetListFromExcel<T>(Stream fileStream, string type, int sheetIndex = 0, int headerRowIndex = 0, int? startRowIndex = null, int? endRowIndex = null, bool isLazadaBill = false)
        {
            try
            {
                if (sheetIndex < 0) sheetIndex = 0;
                if (headerRowIndex < 0) headerRowIndex = 0;

                IWorkbook workbook = null;
                if (type == "XSSF")
                {
                    workbook = new XSSFWorkbook(fileStream);
                }
                else if (type == "HSSF")
                {
                    workbook = new HSSFWorkbook(fileStream);
                }

                if (workbook == null || workbook.NumberOfSheets == 0)
                    throw new Exception(string.Format(Msg.Msg_FileNoDataWithPath, sheetIndex));

                ISheet sheet = workbook.GetSheetAt(sheetIndex);// as HSSFSheet;
                if (sheet == null) throw new Exception(string.Format(Msg.Msg_FileNoDataWithPath, sheetIndex));

                IRow headerRow = sheet.GetRow(headerRowIndex);// as HSSFRow;
                if (headerRow == null || headerRow.PhysicalNumberOfCells == 0) throw new Exception(string.Format(Msg.Msg_FileNoDataWithPath, sheetIndex));

                //验证标题列
                var propertyMap = headerRow.Cells.Where(a=>a.CellType != CellType.Blank).ToDictionary(k => getHeadName(k.StringCellValue, isLazadaBill).ToUpper(), v => v.ColumnIndex);
                PropertyInfo[] properties = (Activator.CreateInstance<T>()).GetType().GetProperties();
                foreach (PropertyInfo property in properties) //每个属性都有对应列
                {
                    if (headerRow.Cells.FindIndex(c => property.Name.Equals(getHeadName(c.StringCellValue, isLazadaBill), StringComparison.CurrentCultureIgnoreCase)) < 0)
                        throw new Exception(string.Format(Msg.Msg_MissDataColumn, property.Name));
                }

                if (!startRowIndex.HasValue || startRowIndex.Value < 0) startRowIndex = sheet.FirstRowNum + headerRowIndex + 1;
                if (!endRowIndex.HasValue || endRowIndex.Value < 0) endRowIndex = sheet.LastRowNum;

                List<T> lists = new List<T>();
                for (int i = startRowIndex.Value; i <= endRowIndex; i++)
                {
                    IRow row = sheet.GetRow(i);// as HSSFRow;
                    //if (row == null || row.Cells == null || row.Cells.TrueForAll(p => string.IsNullOrWhiteSpace(p.ToString()))) break;
                    if (row == null || row.Cells == null || row.Cells.TrueForAll(p => string.IsNullOrWhiteSpace(p.ToString()))) continue;

                    T entity = Activator.CreateInstance<T>(); 
                    foreach (PropertyInfo property in properties)
                    {
                        try
                        {
                            //int cellIndex = headerRow.Cells.FindIndex(c => property.Name.Equals(getHeadName(c.StringCellValue, isLazadaBill), StringComparison.CurrentCultureIgnoreCase));
                            var propertyName = property.Name.ToUpper();
                            var cellIndex = propertyMap.ContainsKey(propertyName) ? propertyMap[propertyName] : -1;
                            if (cellIndex == -1) continue;

                            if (cellIndex >= 0 && row.GetCell(cellIndex) != null)
                            {
                                object value = getTypeValue(property.PropertyType, row.GetCell(cellIndex));
                                //特殊字符处理
                                //value = Regex.Replace(value.ToString(), @"(-{2,})", "—");
                                //value = Regex.Replace(value.ToString(), @"(')", "’");
                                //value = Regex.Replace(value.ToString(), @"(&)", "＆");
                                //value = Regex.Replace(value.ToString(), @"(/)", "／");
                                property.SetValue(entity, value, null);
                            }
                        }
                        catch (Exception e)
                        {
                            string error = string.Format("[列：{0}  行：{1}] ", property.Name, i + 1) + e.ToMessage();
                            throw new Exception(error);
                        }
                    } 
                    lists.Add(entity);
                }
                return lists;
            }
            catch (Exception e)
            {
                throw new Exception(e.ToMessage());
            }
        }
        /// <summary>
        /// 导入Excel
        /// </summary>
        public static List<T> GetListFromExcel2<T>(Stream fileStream, string type, int sheetIndex = 0, int headerRowIndex = 0, int? startRowIndex = null, int? endRowIndex = null)
        {
            try
            {
                if (sheetIndex < 0) sheetIndex = 0;
                if (headerRowIndex < 0) headerRowIndex = 0;

                IWorkbook workbook = null;
                if (type == "XSSF")
                {
                    workbook = new XSSFWorkbook(fileStream);
                }
                else if (type == "HSSF")
                {
                    workbook = new HSSFWorkbook(fileStream);
                }

                if (workbook == null || workbook.NumberOfSheets == 0)
                    throw new Exception(string.Format(Msg.Msg_FileNoDataWithPath, sheetIndex));

                ISheet sheet = workbook.GetSheetAt(sheetIndex);// as HSSFSheet;
                if (sheet == null) throw new Exception(string.Format(Msg.Msg_FileNoDataWithPath, sheetIndex));

                IRow headerRow = sheet.GetRow(headerRowIndex);// as HSSFRow;
                if (headerRow == null || headerRow.PhysicalNumberOfCells == 0) throw new Exception(string.Format(Msg.Msg_FileNoDataWithPath, sheetIndex));

                ////验证标题列
                PropertyInfo[] properties = (Activator.CreateInstance<T>()).GetType().GetProperties();
                //foreach (PropertyInfo property in properties) //每个属性都有对应列
                //{
                //    if (headerRow.Cells.FindIndex(c => property.Name.Equals(getHeadName(c.StringCellValue), StringComparison.CurrentCultureIgnoreCase)) < 0)
                //        throw new Exception(string.Format(Msg.Msg_MissDataColumn, property.Name));
                //}

                if (!startRowIndex.HasValue || startRowIndex.Value < 0) startRowIndex = sheet.FirstRowNum + headerRowIndex + 1;
                if (!endRowIndex.HasValue || endRowIndex.Value < 0) endRowIndex = sheet.LastRowNum;
                List<T> lists = new List<T>();
                for (int i = startRowIndex.Value; i <= endRowIndex; i++)
                {
                    IRow row = sheet.GetRow(i);// as HSSFRow;
                    if (row == null || row.Cells == null || row.Cells.TrueForAll(p => string.IsNullOrWhiteSpace(p.ToString()))) break;

                    T entity = Activator.CreateInstance<T>();
                    foreach (PropertyInfo property in properties)
                    {
                        try
                        {
                            int cellIndex = headerRow.Cells.FindIndex(c => property.Name.Equals(getHeadName(c.StringCellValue), StringComparison.CurrentCultureIgnoreCase));
                            if (cellIndex >= 0 && row.GetCell(cellIndex) != null)
                            {
                                object value = getTypeValue(property.PropertyType, row.GetCell(cellIndex));
                                property.SetValue(entity, value, null); ;
                            }
                        }
                        catch (Exception e)
                        {
                            string error = string.Format("[列：{0}  行：{1}] ", property.Name, i + 1) + e.ToMessage();
                            throw new Exception(error);
                        }
                    }
                    lists.Add(entity);
                }
                return lists;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static List<T> GetListFromExcel3<T>(Stream fileStream, string type, int sheetIndex = 0, int headerRowIndex = 0, int? startRowIndex = null, int? endRowIndex = null)
        {
            try
            {
                if (sheetIndex < 0) sheetIndex = 0;
                if (headerRowIndex < 0) headerRowIndex = 0;

                IWorkbook workbook = null;
                if (type == "XSSF")
                {
                    workbook = new XSSFWorkbook(fileStream);
                }
                else if (type == "HSSF")
                {
                    workbook = new HSSFWorkbook(fileStream);
                }

                if (workbook == null || workbook.NumberOfSheets == 0)
                    throw new Exception(string.Format(Msg.Msg_FileNoDataWithPath, sheetIndex));

                ISheet sheet = workbook.GetSheetAt(sheetIndex);// as HSSFSheet;
                if (sheet == null) throw new Exception(string.Format(Msg.Msg_FileNoDataWithPath, sheetIndex));

                IRow headerRow = sheet.GetRow(headerRowIndex);// as HSSFRow;
                if (headerRow == null || headerRow.PhysicalNumberOfCells == 0) throw new Exception(string.Format(Msg.Msg_FileNoDataWithPath, sheetIndex));

                ////验证标题列
                PropertyInfo[] properties = (Activator.CreateInstance<T>()).GetType().GetProperties();
                //foreach (PropertyInfo property in properties) //每个属性都有对应列
                //{
                //    if (headerRow.Cells.FindIndex(c => property.Name.Equals(getHeadName(c.StringCellValue), StringComparison.CurrentCultureIgnoreCase)) < 0)
                //        throw new Exception(string.Format(Msg.Msg_MissDataColumn, property.Name));
                //}

                if (!startRowIndex.HasValue || startRowIndex.Value < 0) startRowIndex = sheet.FirstRowNum + headerRowIndex + 1;
                if (!endRowIndex.HasValue || endRowIndex.Value < 0) endRowIndex = sheet.LastRowNum;
                List<T> lists = new List<T>();
                for (int i = startRowIndex.Value; i <= endRowIndex; i++)
                {
                    IRow row = sheet.GetRow(i);// as HSSFRow;
                    if (row == null || row.Cells == null || row.Cells.TrueForAll(p => string.IsNullOrWhiteSpace(p.ToString()))) break;

                    T entity = Activator.CreateInstance<T>();
                    foreach (PropertyInfo property in properties)
                    {
                        try
                        {
                            int cellIndex = headerRow.Cells.FindIndex(c => property.Name.Equals(getHeadName(c.StringCellValue), StringComparison.CurrentCultureIgnoreCase));
                            if (cellIndex >= 0 && row.GetCell(cellIndex) != null)
                            {
                                object value = getTypeValue(property.PropertyType, row.GetCell(cellIndex));
                                property.SetValue(entity, value, null);
                            }
                        }
                        catch (Exception e)
                        {
                            string error = string.Format("[列：{0}  行：{1}] ", property.Name, i + 1) + e.ToMessage();
                            throw new Exception(error);
                        }
                    }
                    lists.Add(entity);
                }
                return lists;
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        /// <summary>
        /// 导入Excel（特殊字符处理）
        /// </summary>
        public static List<T> GetListFromExcel5<T>(Stream fileStream, string type, int sheetIndex = 0, int headerRowIndex = 0, int? startRowIndex = null, int? endRowIndex = null, bool isLazadaBill = false)
        {
            try
            {
                if (sheetIndex < 0) sheetIndex = 0;
                if (headerRowIndex < 0) headerRowIndex = 0;

                IWorkbook workbook = null;
                if (type == "XSSF")
                {
                    workbook = new XSSFWorkbook(fileStream);
                }
                else if (type == "HSSF")
                {
                    workbook = new HSSFWorkbook(fileStream);
                }

                if (workbook == null || workbook.NumberOfSheets == 0)
                    throw new Exception(string.Format(Msg.Msg_FileNoDataWithPath, sheetIndex));

                ISheet sheet = workbook.GetSheetAt(sheetIndex);// as HSSFSheet;
                if (sheet == null) throw new Exception(string.Format(Msg.Msg_FileNoDataWithPath, sheetIndex));

                IRow headerRow = sheet.GetRow(headerRowIndex);// as HSSFRow;
                if (headerRow == null || headerRow.PhysicalNumberOfCells == 0) throw new Exception(string.Format(Msg.Msg_FileNoDataWithPath, sheetIndex));

                //验证标题列
                PropertyInfo[] properties = (Activator.CreateInstance<T>()).GetType().GetProperties();
                foreach (PropertyInfo property in properties) //每个属性都有对应列
                {
                    if (headerRow.Cells.FindIndex(c => property.Name.Equals(getHeadNameIgnore(c.StringCellValue), StringComparison.CurrentCultureIgnoreCase)) < 0)
                        throw new Exception(string.Format(Msg.Msg_MissDataColumn, property.Name));
                }

                if (!startRowIndex.HasValue || startRowIndex.Value < 0) startRowIndex = sheet.FirstRowNum + headerRowIndex + 1;
                if (!endRowIndex.HasValue || endRowIndex.Value < 0) endRowIndex = sheet.LastRowNum;
                List<T> lists = new List<T>();
                for (int i = startRowIndex.Value; i <= endRowIndex; i++)
                {
                    IRow row = sheet.GetRow(i);// as HSSFRow;
                    //if (row == null || row.Cells == null || row.Cells.TrueForAll(p => string.IsNullOrWhiteSpace(p.ToString()))) break;
                    if (row == null || row.Cells == null || row.Cells.TrueForAll(p => string.IsNullOrWhiteSpace(p.ToString()))) continue;

                    T entity = Activator.CreateInstance<T>();
                    foreach (PropertyInfo property in properties)
                    {
                        try
                        {
                            int cellIndex = headerRow.Cells.FindIndex(c => property.Name.Equals(getHeadName(c.StringCellValue, isLazadaBill), StringComparison.CurrentCultureIgnoreCase));
                            if (cellIndex >= 0 && row.GetCell(cellIndex) != null)
                            {
                                object value = getTypeValue(property.PropertyType, row.GetCell(cellIndex));
                                //特殊字符处理
                                //value = Regex.Replace(value.ToString(), @"(-{2,})", "—");
                                //value = Regex.Replace(value.ToString(), @"(')", "’");
                                //value = Regex.Replace(value.ToString(), @"(&)", "＆");
                                //value = Regex.Replace(value.ToString(), @"(/)", "／");
                                property.SetValue(entity, value, null);
                            }
                        }
                        catch (Exception e)
                        {
                            string error = string.Format("[列：{0}  行：{1}] ", property.Name, i + 1) + e.ToMessage();
                            throw new Exception(error);
                        }
                    }
                    lists.Add(entity);
                }
                return lists;
            }
            catch (Exception e)
            {
                throw new Exception(e.ToMessage());
            }
        }
        /// <summary>
        /// 多个sheet读取数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileStream"></param>
        /// <param name="type"></param>
        /// <param name="sheetIndex"></param>
        /// <param name="headerRowIndex"></param>
        /// <param name="startRowIndex"></param>
        /// <param name="endRowIndex"></param>
        /// <returns></returns>
        public static List<T> GetListFromExcelMoreSheet<T>(Stream fileStream, string type, int sheetStart = 0, int sheetEnd = 0, int headerRowIndex = 0,
            int? startRowIndex = null, int? endRowIndex = null)
        {
            try
            {
                if (sheetStart < 0) sheetStart = 0;
                if (headerRowIndex < 0) headerRowIndex = 0;

                IWorkbook workbook = null;
                if (type == "XSSF")
                {
                    workbook = new XSSFWorkbook(fileStream);
                }
                else if (type == "HSSF")
                {
                    workbook = new HSSFWorkbook(fileStream);
                }

                if (workbook == null || workbook.NumberOfSheets == 0)
                    throw new Exception(string.Format(Msg.Msg_FileNoDataWithPath, sheetStart));
                List<T> lists = new List<T>();

                sheetEnd = (0 < sheetEnd && sheetEnd < workbook.NumberOfSheets) ? sheetEnd : workbook.NumberOfSheets;
                for (int ii = sheetStart; ii < sheetEnd; ii++)
                {

                    ISheet sheet = workbook.GetSheetAt(ii);// as HSSFSheet;
                    if (sheet == null) throw new Exception(string.Format(Msg.Msg_FileNoDataWithPath, ii));

                    IRow headerRow = sheet.GetRow(headerRowIndex);// as HSSFRow;
                    if (headerRow == null || headerRow.PhysicalNumberOfCells == 0) throw new Exception(string.Format(Msg.Msg_FileNoDataWithPath, ii));

                    ////验证标题列
                    PropertyInfo[] properties = (Activator.CreateInstance<T>()).GetType().GetProperties();
                    if (!startRowIndex.HasValue || startRowIndex.Value < 0) startRowIndex = sheet.FirstRowNum + headerRowIndex + 1;
                    if (!endRowIndex.HasValue || endRowIndex.Value < 0) endRowIndex = sheet.LastRowNum;
                    //无可说，因为名称太长了信息不完整只能这么补
                    string sheetName = sheet.SheetName.ToLower().Trim() == "windmill daily chemical supplie" ? "windmill daily chemical supplies" : sheet.SheetName.Trim();
                    for (int i = startRowIndex.Value; i <= sheet.LastRowNum; i++)
                    {
                        IRow row = sheet.GetRow(i);// as HSSFRow;
                        if (row == null || row.Cells == null || row.Cells.TrueForAll(p => string.IsNullOrWhiteSpace(p.ToString()))) break;

                        T entity = Activator.CreateInstance<T>();
                        foreach (PropertyInfo property in properties)
                        {
                            try
                            {
                                if (property.Name == "SheetName")
                                {
                                    object value = sheetName;
                                    property.SetValue(entity, value, null);
                                    continue;
                                }
                                int cellIndex = headerRow.Cells.FindIndex(c => property.Name.Equals(getHeadName(c.StringCellValue),
                                    StringComparison.CurrentCultureIgnoreCase));
                                if (cellIndex >= 0 && row.GetCell(cellIndex) != null)
                                {
                                    object value = getTypeValue(property.PropertyType, row.GetCell(cellIndex));
                                    property.SetValue(entity, value, null);
                                }

                            }
                            catch (Exception e)
                            {
                                string error = string.Format("[列：{0}  行：{1}] ", property.Name, i + 1) + e.ToMessage();
                                throw new Exception(error);
                            }
                        }
                        lists.Add(entity);
                    }
                }
                return lists;
            }
            catch (Exception e)
            {
                throw e;
            }
        }



        /// <summary>
        /// 导入Excel
        /// </summary>
        /// <param name="filePath">Excel所在路径</param>
        /// <param name="isLazadaBill">是否lazada账单导入</param>
        public static List<T> GetListFromExcel<T>(string filePath, int sheetIndex = 0, int headerRowIndex = 0, int? startRowIndex = null, int? endRowIndex = null, bool isLazadaBill = false)
        {
            if (!System.IO.File.Exists(filePath)) throw new Exception(string.Format(Msg.Msg_FileNoExistWithPath, filePath));
            using (FileStream fileStream = System.IO.File.OpenRead(filePath))
            {
                if (filePath.Contains(".csv"))
                {
                    return GetListFromCSVFile<T>(filePath);
                }
                else if (filePath.Contains(".xlsx"))
                {
                    return GetListFromExcel<T>(fileStream, "XSSF", sheetIndex, headerRowIndex, startRowIndex, endRowIndex, isLazadaBill);
                }
                else
                {
                    return GetListFromExcel<T>(fileStream, "HSSF", sheetIndex, headerRowIndex, startRowIndex, endRowIndex, isLazadaBill);
                }
            }
        }


        /// <summary>
        /// 导入Excel
        /// </summary>
        /// <param name="filePath">Excel所在路径</param>
        public static List<T> GetListFromExcelWithNote<T>(string filePath, int sheetIndex = 0, int headerRowIndex = 0, int? startRowIndex = null, int? endRowIndex = null)
        {
            try
            {
                if (!System.IO.File.Exists(filePath)) throw new Exception(string.Format(Msg.Msg_FileNoExistWithPath, filePath));
                using (FileStream fileStream = System.IO.File.OpenRead(filePath))
                {
                    if (filePath.Contains(".csv"))
                    {
                        return GetListFromCSVFile<T>(filePath);
                    }
                    else if (filePath.Contains(".xlsx"))
                    {
                        return GetListFromExcel5<T>(fileStream, "XSSF", sheetIndex, headerRowIndex, startRowIndex, endRowIndex);
                    }
                    else
                    {
                        return GetListFromExcel5<T>(fileStream, "HSSF", sheetIndex, headerRowIndex, startRowIndex, endRowIndex);
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 导入Excel多个sheetName导入
        /// </summary>
        /// <param name="filePath">Excel所在路径</param>
        public static List<T> GetListFromExcelMoreSheet<T>(string filePath, int sheetStart = 0, int sheetEnd = 0, int headerRowIndex = 0,
            int? startRowIndex = null, int? endRowIndex = null)
        {
            try
            {
                if (!System.IO.File.Exists(filePath)) throw new Exception(string.Format(Msg.Msg_FileNoExistWithPath, filePath));
                using (FileStream fileStream = System.IO.File.OpenRead(filePath))
                {
                    if (filePath.Contains(".xlsx"))
                    {
                        return GetListFromExcelMoreSheet<T>(fileStream, "XSSF", sheetStart, sheetEnd, headerRowIndex, startRowIndex, endRowIndex);
                    }
                    else
                    {
                        return GetListFromExcelMoreSheet<T>(fileStream, "HSSF", sheetStart, sheetEnd, headerRowIndex, startRowIndex, endRowIndex);
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }



        public static List<T> GetListFromExcel2<T>(string filePath, int sheetIndex = 0, int headerRowIndex = 0, int? startRowIndex = null, int? endRowIndex = null)
        {
            try
            {
                if (!System.IO.File.Exists(filePath)) throw new Exception(string.Format(Msg.Msg_FileNoExistWithPath, filePath));
                using (FileStream fileStream = System.IO.File.OpenRead(filePath))
                {
                    if (filePath.Contains(".xlsx"))
                    {
                        return GetListFromExcel2<T>(fileStream, "XSSF", sheetIndex, headerRowIndex, startRowIndex, endRowIndex);
                    }
                    else
                    {
                        return GetListFromExcel2<T>(fileStream, "HSSF", sheetIndex, headerRowIndex, startRowIndex, endRowIndex);
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static List<T> GetListFromExcelByMappingCols<T>(string filePath, Dictionary<string, string> dictColMapping, int sheetIndex = 0, int headerRowIndex = 0, int? startRowIndex = null, int? endRowIndex = null)
        {
            IWorkbook workBook = getWorkBook(filePath);
            ISheet sheet = workBook.GetSheetAt(sheetIndex);
            IRow headerRow = sheet.GetRow(headerRowIndex);
            if (headerRow == null || headerRow.PhysicalNumberOfCells == 0)
            {
                throw new Exception(string.Format(Msg.Msg_FileNoDataWithPath, sheetIndex));
            }

            //验证标题列
            PropertyInfo[] properties = (Activator.CreateInstance<T>()).GetType().GetProperties();

            if (!startRowIndex.HasValue || startRowIndex.Value < 0) startRowIndex = sheet.FirstRowNum + headerRowIndex + 1;
            if (!endRowIndex.HasValue || endRowIndex.Value < 0) endRowIndex = sheet.LastRowNum;

            List<T> lists = new List<T>();
            for (int i = startRowIndex.Value; i <= endRowIndex; i++)
            {
                IRow row = sheet.GetRow(i);
                if (row == null || row.Cells == null || row.Cells.TrueForAll(p => string.IsNullOrWhiteSpace(p.ToString()))) break;

                T entity = Activator.CreateInstance<T>();
                foreach (PropertyInfo property in properties) //
                {
                    try
                    {
                        if (!dictColMapping.ContainsKey(property.Name))
                        {
                            continue;
                        }

                        int cellIndex = headerRow.Cells.FindIndex(c => dictColMapping[property.Name].Equals(c.StringCellValue));
                        if (cellIndex >= 0 && row.GetCell(cellIndex) != null)
                        {
                            object value = getTypeValue(property.PropertyType, row.GetCell(cellIndex));
                            property.SetValue(entity, value, null);
                        }
                    }
                    catch (Exception e)
                    {
                        string error = string.Format("[列：{0}  行：{1}] ", dictColMapping[property.Name], i + 1) + e.ToMessage();
                        throw new Exception(error);
                    }
                }
                lists.Add(entity);
            }
            return lists;
        }

        private static object getTypeValue(Type type, ICell value)
        {
            try
            {
                object result = null;
                string typeName = type.Name.ToLower();
                if (typeName == "nullable`1")
                {
                    if (!string.IsNullOrWhiteSpace(value.ToString()))
                    {
                        return getTypeValue(type.GetGenericArguments()[0], value);
                    }
                }
                else
                {
                    switch (typeName)
                    {
                        case "short":
                        case "int16":
                            try { result = (short)(value.NumericCellValue); }
                            catch (Exception) { result = Convert.ToInt16(value.ToString()); }
                            break;
                        case "int":
                        case "int32":
                            try { result = (int)(value.NumericCellValue); }
                            catch (Exception) { result = Convert.ToInt32(value.ToString()); }
                            break;
                        case "long":
                        case "int64":
                            try { result = (long)(value.NumericCellValue); }
                            catch (Exception) { result = Convert.ToInt64(value.ToString()); }
                            break;
                        case "decimal":
                            try { result = (decimal)(value.NumericCellValue); }
                            catch (Exception) { result = Convert.ToDecimal(value.ToString()); }
                            break;
                        case "float":
                        case "single":
                            try { result = (float)(value.NumericCellValue); }
                            catch (Exception) { result = Convert.ToSingle(value.ToString()); }
                            break;
                        case "datetime":
                            try { result = value.DateCellValue; }
                            catch (Exception)
                            {
                                try
                                {
                                    result = Convert.ToDateTime(value.ToString());
                                }
                                catch (Exception)
                                {
                                    //在这里对自定义，不规范的时间格式进行处理 [CSX 2018/1/22] 
                                    DateTime output;
                                    bool isTrue = false;
                                    isTrue = DateTime.TryParseExact(value.ToString(), "MM/dd/yy", null, DateTimeStyles.None, out output);
                                    if (isTrue)
                                    {
                                        result = output;
                                    }

                                }
                            }
                            break;
                        case "bool":
                        case "boolean":
                            try { result = value.BooleanCellValue; }
                            catch (Exception) { result = Convert.ToBoolean(value.ToString()); }
                            break;
                        default:
                            try { result = value.StringCellValue; }
                            catch (Exception) { result = value.ToString(); }
                            break;
                    }
                }
                return result;
            }
            catch (Exception)
            {
                throw new Exception(string.Format(Msg.Msg_ImportDateFormatError, value));
            }
        } 
        /// <summary>
        /// 转化为列头名，把空格等特殊字符替代为下划线
        /// </summary>
        private static string getHeadName(string cellValue, bool isRemoveSpace = false)
        {
            if (string.IsNullOrWhiteSpace(cellValue)) return null;
            //lazada导入单独处理一下表头 2018-7-16 11:58:12 pwk
            if (isRemoveSpace)
            {
                switch (cellValue)
                {
                    case "Statement":
                        return "PlatformStatement";
                    case "Order No.":
                        return "PlatformOrderID";
                    case "Order Item No.":
                        return "OrderItemID";
                    case "Reference":
                        return "ReferenceOrderBy";
                    case "Comment":
                        return "CommentRemark";
                    default:
                        return cellValue.Trim().Replace(" ", "");
                }
            }
            return cellValue.Trim().Replace(" & ", " ").Replace(" # ", " ").Replace(" #", " ").Trim()
                .Replace(' ', '_').Replace('/', '_').Replace('-', '_').Replace('&', '_').Replace('#', '_').Replace('(', '_').Replace(')', '_').Replace('（', '_').Replace('）', '_');//中文括号
        }
        /// <summary>
        /// 转化为列头名，把空格等特殊字符替代为下划线
        /// </summary>
        private static string getHeadNameIgnore(string cellValue, bool isRemoveSpace = true)
        {
            if (string.IsNullOrWhiteSpace(cellValue)) return null;
            //lazada导入单独处理一下表头 2018-7-16 11:58:12 pwk
            if (isRemoveSpace)
            {
                switch (cellValue)
                {
                    //Daraz 导入特处理
                    case "Shipping Provider (first mile)":
                        return "Shipping_Provider_First_Mile";
                    case "Tracking Code (first mile)":
                        return "Tracking_Code_First_Mile";
                    case "Tracking URL (first mile)":
                        return "Tracking_Url_First_Mile";
                    case "Cancel / Return Initiator":
                        return "Cancel_Return_Initiator";
                    default:
                        return cellValue.Trim().Replace(" & ", " ").Replace(" # ", " ").Replace(" #", " ").Trim()
                .Replace(' ', '_').Replace('/', '_').Replace('-', '_').Replace('&', '_').Replace('#', '_').Replace('(', '_').Replace(')', '_').Replace('（', '_').Replace('）', '_');//中文括号
                }
            }
            return cellValue.Trim().Replace(" & ", " ").Replace(" # ", " ").Replace(" #", " ").Trim()
                .Replace(' ', '_').Replace('/', '_').Replace('-', '_').Replace('&', '_').Replace('#', '_').Replace('(', '_').Replace(')', '_').Replace('（', '_').Replace('）', '_');//中文括号
        }

        /// <summary>
        /// 由Excel导入DataTable
        /// </summary>
        /// <param name="excelFilePath">Excel文件路径，为物理路径。</param>
        /// <param name="sheetName">Excel工作表索引</param>
        /// <param name="headerRowIndex">Excel表头行索引</param>
        /// <returns>DataTable</returns>
        public static DataTable GetDataTableFromExcel(string excelFilePath, int sheetIndex = 0, int headerRowIndex = 0)
        {
            using (FileStream stream = System.IO.File.OpenRead(excelFilePath))
            {
                return getDataTableFromExcel(stream, sheetIndex, headerRowIndex);
            }
        }
        /// <summary>
        /// 由Excel导入DataTable
        /// </summary>
        /// <param name="excelFileStream">Excel文件流</param>
        /// <param name="sheetName">Excel工作表索引</param>
        /// <param name="headerRowIndex">Excel表头行索引</param>
        /// <returns>DataTable</returns>
        private static DataTable getDataTableFromExcel(Stream excelFileStream, int sheetIndex = 0, int headerRowIndex = 0)
        {
            HSSFWorkbook workbook = new HSSFWorkbook(excelFileStream);
            HSSFSheet sheet = workbook.GetSheetAt(sheetIndex) as HSSFSheet;

            DataTable table = new DataTable();

            HSSFRow headerRow = sheet.GetRow(headerRowIndex) as HSSFRow;
            int cellCount = headerRow.LastCellNum;

            for (int i = headerRow.FirstCellNum; i < cellCount; i++)
            {
                if (headerRow.GetCell(i) == null || headerRow.GetCell(i).StringCellValue.Trim() == "")
                {
                    // 如果遇到第一个空列，则不再继续向后读取
                    cellCount = i + 1;
                    break;
                }
                DataColumn column = new DataColumn(headerRow.GetCell(i).StringCellValue);
                table.Columns.Add(column);
            }
            for (int i = (sheet.FirstRowNum + headerRowIndex + 1); i <= sheet.LastRowNum; i++)
            {
                HSSFRow row = sheet.GetRow(i) as HSSFRow;
                if (row == null || row.Cells == null || row.Cells.TrueForAll(p => string.IsNullOrWhiteSpace(p.ToString()))) break;

                DataRow dataRow = table.NewRow();
                for (int j = row.FirstCellNum; j < cellCount; j++)
                {
                    dataRow[j] = row.GetCell(j);
                }
                table.Rows.Add(dataRow);
            }
            excelFileStream.Close();
            workbook = null;
            sheet = null;
            return table;
        }

        public static DataTable ReadFileByImportFile(string Path)
        {
            FileStream fs = System.IO.File.OpenRead(Path);
            //根据路径通过已存在的excel来创建HSSFWorkbook，即整个excel文档
            HSSFWorkbook workbook = new HSSFWorkbook(fs);

            //获取excel的第一个sheet
            HSSFSheet sheet = (HSSFSheet)workbook.GetSheetAt(0);

            DataTable table = new DataTable();

            int startRowIndex = 0, endRowIndex = 0;
            //获取所有行集合
            IEnumerator rowsCount = sheet.GetRowEnumerator();
            int cellCount = 0;
            int rowCount = 0;
            while (rowsCount.MoveNext())
            {
                rowCount++;
                int temp = ((HSSFRow)rowsCount.Current).LastCellNum;
                if (temp > cellCount)
                    cellCount = temp;
            }
            //未指定结束行索引 默认最大行索引
            if (endRowIndex == 0)
                endRowIndex = rowCount;
            IEnumerator rows = sheet.GetRowEnumerator();
            //如指定起始行索引 "指针"往后移
            int count = 1;
            while (count <= startRowIndex)
            {
                rows.MoveNext();
                count++;
            }
            count -= 1;
            bool isOK = false;
            int j = -1;
            while (rows.MoveNext())
            {
                if (count <= endRowIndex)
                {
                    HSSFRow row = (HSSFRow)rows.Current;
                    //确定列数 根据表头指定 表单数据可为空 （主单和子单列数不一致）
                    if (!isOK && !(startRowIndex == 0 && endRowIndex == 0))
                    {
                        cellCount = row.LastCellNum;
                        isOK = true;
                    }
                    DataRow dataRow = table.NewRow();
                    for (int i = 0; i < cellCount; i++)
                    {

                        HSSFCell cell = (HSSFCell)row.GetCell(i);
                        if (cell != null)
                        {
                            if (count <= 0)
                            {
                                DataColumn column = new DataColumn(GetStringByType(cell).
                                Replace("amazon-order-id", "order-id").
                                Replace("amazon-order-item-id", "order-item-id").
                                Replace("quantity-shipped", "quantity-purchased").
                                Replace("-", ""));
                                table.Columns.Add(column);
                            }
                            if (j >= 0)
                            {
                                dataRow[i] = GetStringByType(cell);
                            }

                        }
                    }
                    if (j >= 0)
                        table.Rows.Add(dataRow);
                    j++;
                    count++;
                }
            }
            ////获取sheet的首行
            //HSSFRow headerRow = (HSSFRow)sheet.GetRow(0);

            ////一行最后一个方格的编号 即总的列数
            //int cellCount = headerRow.LastCellNum;

            //for (int i = headerRow.FirstCellNum; i < cellCount; i++)
            //{
            //    DataColumn column = new DataColumn(headerRow.GetCell(i).StringCellValue.
            //        Replace("amazon-order-id", "order-id").Replace("amazon-order-item-id", "order-item-id").Replace("quantity-shipped", "quantity-purchased").Replace("-", ""));
            //    table.Columns.Add(column);
            //}
            ////最后一列的标号  即总的行数
            //int rowCount = sheet.LastRowNum;
            //for (int i = (sheet.FirstRowNum + 1); i < rowCount; i++)
            //{
            //    HSSFRow row = (HSSFRow)sheet.GetRow(i);
            //    DataRow dataRow = table.NewRow();

            //    for (int j = row.FirstCellNum; j < cellCount; j++)
            //    {
            //        if (row.GetCell(j) != null)
            //            dataRow[j] = row.GetCell(j).ToString();
            //    }
            //    table.Rows.Add(dataRow);
            //}
            workbook = null;
            sheet = null;
            fs.Close();
            return table;
        }

        /// <summary>
        /// 根据单元格类型来取值
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        public static string GetStringByType(HSSFCell cell)
        {
            string type = "";
            switch (cell.CellType)
            {
                case CellType.Blank:
                    type = "";
                    break;
                case CellType.Numeric:
                    type = cell.ToString();
                    break;
                case CellType.String:
                    type = cell.StringCellValue;
                    break;
                default:
                    type = "";
                    break;
            }
            return type;
        }

        private static IWorkbook getWorkBook(string filePath)
        {
            using (FileStream fileStream = System.IO.File.OpenRead(filePath))
            {
                IWorkbook workbook = null;
                if (filePath.Contains(".xlsx"))
                {
                    workbook = new XSSFWorkbook(fileStream);
                }
                else
                {
                    workbook = new HSSFWorkbook(fileStream);
                }

                return workbook;
            }
        }

        /// <summary>
        /// 获取表头
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="sheetIndex"></param>
        /// <param name="headerRowIndex"></param>
        /// <returns></returns>
        public static List<string> GetHeaderList(string filePath, int sheetIndex = 0, int headerRowIndex = 0)
        {
            if (sheetIndex < 0) sheetIndex = 0;
            if (headerRowIndex < 0) headerRowIndex = 0;

            IWorkbook workbook = getWorkBook(filePath);
            if (workbook == null || workbook.NumberOfSheets == 0)
            {
                throw new Exception(string.Format(Msg.Msg_FileNoDataWithPath, sheetIndex));
            }

            ISheet sheet = workbook.GetSheetAt(sheetIndex);
            {
                if (sheet == null) throw new Exception(string.Format
                    (Msg.Msg_FileNoDataWithPath, sheetIndex));
            }

            IRow headerRow = sheet.GetRow(headerRowIndex);// as HSSFRow;
            if (headerRow == null || headerRow.PhysicalNumberOfCells == 0)
            {
                throw new Exception(string.Format(Msg.Msg_FileNoDataWithPath, sheetIndex));
            }

            return headerRow.Cells.Select(zw => zw.StringCellValue).ToList();
        }

        public static DataTable GetDataTableByExcel(string excelFilePath, int sheetIndex = 0, int headerRowIndex = 0)
        {
            using (FileStream stream = File.OpenRead(excelFilePath))
            {
                if (Path.GetExtension(excelFilePath).ToLower() == ".xlsx")
                    return getDaTableBy07Excel(stream, sheetIndex, headerRowIndex);
                else
                    return getDaTableBy03Excel(stream, sheetIndex, headerRowIndex);
            }
        }
        private static DataTable getDaTableBy07Excel(Stream excelFileStream, int sheetIndex = 0, int headerRowIndex = 0)
        {
            DataTable table = new DataTable();
            XSSFWorkbook workbook = new XSSFWorkbook(excelFileStream);
            XSSFSheet sheet = workbook.GetSheetAt(sheetIndex) as XSSFSheet;

            XSSFRow headerRow = sheet.GetRow(headerRowIndex) as XSSFRow;    //表头行的索引
            int cellCount = headerRow.LastCellNum;
            for (int i = headerRow.FirstCellNum; i < cellCount; i++)
            {
                DataColumn column = new DataColumn(headerRow.GetCell(i).StringCellValue);
                table.Columns.Add(column);
            }
            for (int i = 0; i <= sheet.LastRowNum; i++)
            {
                XSSFRow row = sheet.GetRow(i) as XSSFRow;
                if (row == null || row.Cells == null || row.Cells.TrueForAll(p => string.IsNullOrWhiteSpace(p.ToString()))) break;
                DataRow dataRow = table.NewRow();
                for (int j = row.FirstCellNum; j < cellCount; j++)
                {
                    dataRow[j] = row.GetCell(j) != null ? row.GetCell(j) + "" : "";
                }
                table.Rows.Add(dataRow);
            }
            excelFileStream.Close();
            workbook = null;
            sheet = null;
            return table;
        }

        private static DataTable getDaTableBy03Excel(Stream excelFileStream, int sheetIndex = 0, int headerRowIndex = 0)
        {
            HSSFWorkbook workbook = new HSSFWorkbook(excelFileStream);
            HSSFSheet sheet = workbook.GetSheetAt(sheetIndex) as HSSFSheet;
            DataTable table = new DataTable();

            HSSFRow headerRow = sheet.GetRow(headerRowIndex) as HSSFRow;    //表头行的索引
            int cellCount = headerRow.LastCellNum;
            for (int i = headerRow.FirstCellNum; i < cellCount; i++)
            {
                DataColumn column = new DataColumn(headerRow.GetCell(i).StringCellValue);
                table.Columns.Add(column);
            }
            for (int i = 0; i <= sheet.LastRowNum; i++)
            {
                HSSFRow row = sheet.GetRow(i) as HSSFRow;
                if (row == null || row.Cells == null || row.Cells.TrueForAll(p => string.IsNullOrWhiteSpace(p.ToString()))) break;
                DataRow dataRow = table.NewRow();
                for (int j = row.FirstCellNum; j < cellCount; j++)
                {
                    dataRow[j] = row.GetCell(j) != null ? row.GetCell(j) + "" : "";
                }
                table.Rows.Add(dataRow);
            }
            excelFileStream.Close();
            workbook = null;
            sheet = null;
            return table;
        }

        /// <summary>  
        /// 将excel导入到DataSet  
        /// pwk 2017-2-6 14:49:13
        /// </summary>  
        /// <param name="filePath">excel路径</param>  
        /// <param name="isColumnName">第一行是否是列名</param>  
        /// <returns>返回datatable</returns>  
        public static DataSet ExcelToDataSet(string filePath, bool isColumnName)
        {
            DataSet ds = null;//要返回的数据集
            FileStream fs = null;
            IWorkbook workbook = null;
            int startRow = 0;
            try
            {
                using (fs = File.OpenRead(filePath))
                {
                    // 2007版本  
                    if (filePath.IndexOf(".xlsx") > 0)
                        workbook = new XSSFWorkbook(fs);
                    // 2003版本  
                    else if (filePath.IndexOf(".xls") > 0)
                        workbook = new HSSFWorkbook(fs);

                    if (workbook == null) return ds;
                    ds = new DataSet();
                    var sheetsCount = workbook.NumberOfSheets;
                    for (int s = 0; s < sheetsCount; s++)
                    {
                        var sheet = workbook.GetSheetAt(s);
                        var dataTable = new DataTable();
                        if (sheet != null)
                        {
                            int rowCount = sheet.LastRowNum; //总行数  
                            if (rowCount > 0)
                            {
                                IRow firstRow = sheet.GetRow(0); //第一行  
                                int cellCount = firstRow.LastCellNum; //列数  

                                //构建datatable的列  
                                DataColumn column = null;
                                ICell cell = null;
                                if (isColumnName)
                                {
                                    startRow = 1; //如果第一行是列名，则从第二行开始读取  
                                    for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
                                    {
                                        cell = firstRow.GetCell(i);
                                        if (cell != null)
                                        {
                                            if (cell.StringCellValue != null)
                                            {
                                                column = new DataColumn(cell.StringCellValue);
                                                dataTable.Columns.Add(column);
                                            }

                                        }
                                    }
                                }
                                else
                                {
                                    for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
                                    {
                                        column = new DataColumn("column" + (i + 1));
                                        dataTable.Columns.Add(column);
                                    }
                                }

                                //填充行  
                                for (int i = startRow; i <= rowCount; ++i)
                                {
                                    var row = sheet.GetRow(i);
                                    if (row == null) continue;

                                    var dataRow = dataTable.NewRow();
                                    for (int j = row.FirstCellNum; j < cellCount; ++j)
                                    {
                                        cell = row.GetCell(j);
                                        if (cell == null)
                                        {
                                            dataRow[j] = "";
                                        }
                                        else
                                        {
                                            //CellType(Unknown = -1,Numeric = 0,String = 1,Formula = 2,Blank = 3,Boolean = 4,Error = 5,)  
                                            switch (cell.CellType)
                                            {
                                                case CellType.Blank:
                                                    dataRow[j] = "";
                                                    break;
                                                case CellType.Numeric:
                                                    short format = cell.CellStyle.DataFormat;
                                                    //对时间格式（2015.12.5、2015/12/5、2015-12-5等）的处理  
                                                    if (format == 14 || format == 31 || format == 57 || format == 58)
                                                    {
                                                        dataRow[j] = cell.DateCellValue;
                                                    }
                                                    else
                                                    {
                                                        dataRow[j] = cell.NumericCellValue;
                                                    }
                                                    break;
                                                case CellType.String:
                                                    dataRow[j] = cell.StringCellValue;
                                                    break;
                                                case CellType.Formula:
                                                    dataRow[j] = cell.NumericCellValue;
                                                    break;
                                            }
                                        }
                                    }
                                    dataTable.Rows.Add(dataRow);
                                }
                            }
                        }
                        ds.Tables.Add(dataTable);
                    }
                }
                return ds;
            }
            catch (Exception)
            {
                if (fs != null)
                {
                    fs.Close();
                }
                return null;
            }
        }

        /// <summary>
        /// 将CSV文件的数据读取到DataTable中
        /// </summary>
        /// <param name="fileName">CSV文件路径</param>
        /// <returns>返回读取了CSV数据的DataTable</returns>
        public static DataTable OpenCSV(string filePath)
        {
            DataTable dt = new DataTable();
            FileStream fs = new FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            StreamReader sr = new StreamReader(fs, Encoding.Default);
            //记录每次读取的一行记录
            string strLine = "";
            //记录每行记录中的各字段内容
            string[] aryLine = null;
            string[] tableHead = null;
            //标示列数
            int columnCount = 0;
            //标示是否是读取的第一行
            bool IsFirst = true;
            //逐行读取CSV中的数据
            while (!string.IsNullOrEmpty((strLine = sr.ReadLine())))
            {
                //数据不能为null
                if (strLine.IndexOf(",,,,,,,,,,,,,,,") < 0)
                {
                    if (IsFirst == true)
                    {
                        tableHead = strLine.Split(',');
                        IsFirst = false;
                        columnCount = tableHead.Length;
                        //创建列
                        for (int i = 0; i < columnCount; i++)
                        {
                            tableHead[i] = tableHead[i].Replace('"', ' ').Trim();
                            DataColumn dc = new DataColumn(tableHead[i]);
                            dt.Columns.Add(dc);
                        }
                    }
                    else
                    {
                        aryLine = strLine.Split(',');
                        DataRow dr = dt.NewRow();
                        for (int j = 0; j < columnCount; j++)
                        {
                            dr[j] = aryLine[j].Replace('"', ' ').Replace('&', '|').Replace("'", "''").Trim();
                        }
                        dt.Rows.Add(dr);
                    }
                }
            }
            //if (aryLine != null && aryLine.Length > 0)
            //{
            //    dt.DefaultView.Sort = tableHead[2] + " " + "DESC";
            //}
            sr.Close();
            fs.Close();
            return dt;
        }

        #region 社保导入

        //南昌社保
        private static DataTable getDaTableBy03ExcelNC(Stream excelFileStream, int sheetIndex = 0, int headerRowIndex = 0)
        {
            HSSFWorkbook workbook = new HSSFWorkbook(excelFileStream);
            HSSFSheet sheet = workbook.GetSheetAt(sheetIndex) as HSSFSheet;
            DataTable table = new DataTable();

            HSSFRow headerRow = sheet.GetRow(headerRowIndex) as HSSFRow;    //表头行的索引
            int cellCount = headerRow.LastCellNum;
            for (int i = headerRow.FirstCellNum; i < cellCount; i++)
            {
                DataColumn column = new DataColumn(headerRow.GetCell(i).StringCellValue);
                table.Columns.Add(column);
            }
            for (int i = 0; i <= sheet.LastRowNum; i++)
            {
                HSSFRow row = sheet.GetRow(i) as HSSFRow;
                if (row == null || row.Cells == null || row.Cells.TrueForAll(p => string.IsNullOrWhiteSpace(p.ToString()))) break;
                DataRow dataRow = table.NewRow();
                var reg = @"/^[1-9]\d{7}((0\d)|(1[0-2]))(([0|1|2]\d)|3[0-1])\d{3}$|^[1-9]\d{5}[1-9]\d{3}((0\d)|(1[0-2]))(([0|1|2]\d)|3[0-1])\d{3}([0-9]|[Xx])$/";
                for (int j = row.FirstCellNum; j < cellCount; j++)
                {
                    if (((((HSSFCell)row.GetCell(4)).StringCellValue).IndexOf(" ") >= 0 || !Regex.IsMatch(((HSSFCell)row.GetCell(4)).StringCellValue, reg)) && (j == 8 || j == 9))
                    {
                    }
                    else
                    {
                        dataRow[j] = row.GetCell(j) != null ? GetStringByType1((HSSFCell)row.GetCell(j)) + "" : "";
                    }
                }
                table.Rows.Add(dataRow);
            }
            excelFileStream.Close();
            workbook = null;
            sheet = null;
            return table;
        }

        //深圳、重庆社保
        private static DataTable getDaTableBy03Excel1(Stream excelFileStream, int sheetIndex = 0, int headerRowIndex = 0)
        {
            HSSFWorkbook workbook = new HSSFWorkbook(excelFileStream);
            HSSFSheet sheet = workbook.GetSheetAt(sheetIndex) as HSSFSheet;
            DataTable table = new DataTable();

            HSSFRow headerRow = sheet.GetRow(headerRowIndex) as HSSFRow;    //表头行的索引
            int cellCount = headerRow.LastCellNum;
            for (int i = headerRow.FirstCellNum; i < cellCount; i++)
            {
                DataColumn column = new DataColumn(headerRow.GetCell(i).StringCellValue);
                table.Columns.Add(column);
            }
            for (int i = 0; i <= sheet.LastRowNum; i++)
            {
                HSSFRow row = sheet.GetRow(i) as HSSFRow;
                if (row == null || row.Cells == null || row.Cells.TrueForAll(p => string.IsNullOrWhiteSpace(p.ToString()))) break;
                DataRow dataRow = table.NewRow();
                for (int j = row.FirstCellNum; j < cellCount; j++)
                {
                    dataRow[j] = row.GetCell(j) != null ? GetStringByType1((HSSFCell)row.GetCell(j)) + "" : "";
                }
                table.Rows.Add(dataRow);
            }
            excelFileStream.Close();
            workbook = null;
            sheet = null;
            return table;
        }

        //深圳、重庆社保
        private static DataTable getDaTableBy07Excel1(Stream excelFileStream, int sheetIndex = 0, int headerRowIndex = 0)
        {
            DataTable table = new DataTable();
            XSSFWorkbook workbook = new XSSFWorkbook(excelFileStream);
            XSSFSheet sheet = workbook.GetSheetAt(sheetIndex) as XSSFSheet;

            XSSFRow headerRow = sheet.GetRow(headerRowIndex) as XSSFRow;    //表头行的索引
            int cellCount = headerRow.LastCellNum;
            for (int i = headerRow.FirstCellNum; i < cellCount; i++)
            {
                DataColumn column = new DataColumn(headerRow.GetCell(i).StringCellValue);
                table.Columns.Add(column);
            }
            for (int i = 0; i <= sheet.LastRowNum; i++)
            {
                XSSFRow row = sheet.GetRow(i) as XSSFRow;
                if (row == null || row.Cells == null || row.Cells.TrueForAll(p => string.IsNullOrWhiteSpace(p.ToString()))) break;
                DataRow dataRow = table.NewRow();
                for (int j = row.FirstCellNum; j < cellCount; j++)
                {
                    dataRow[j] = row.GetCell(j) != null ? GetStringByType2((XSSFCell)row.GetCell(j)) + "" : "";
                }
                table.Rows.Add(dataRow);
            }
            excelFileStream.Close();
            workbook = null;
            sheet = null;
            return table;
        }

        //社保根据索引导入
        public static DataTable GetDataTableByExcel1(string excelFilePath, int sheetIndex = 0, int headerRowIndex = 0)
        {
            using (FileStream stream = File.OpenRead(excelFilePath))
            {
                if (Path.GetExtension(excelFilePath).ToLower() == ".xlsx")
                    return getDaTableBy07Excel1(stream, sheetIndex, headerRowIndex);
                else
                    return getDaTableBy03Excel1(stream, sheetIndex, headerRowIndex);
            }
        }

        //南昌社保根据索引导入
        public static DataTable GetDataTableByExcelNC(string excelFilePath, int sheetIndex = 0, int headerRowIndex = 0)
        {
            using (FileStream stream = File.OpenRead(excelFilePath))
            {
                if (Path.GetExtension(excelFilePath).ToLower() == ".xlsx")
                    return getDaTableBy07ExcelNC(stream, sheetIndex, headerRowIndex);
                else
                    return getDaTableBy03ExcelNC(stream, sheetIndex, headerRowIndex);
            }
        }

        //南昌社保
        private static DataTable getDaTableBy07ExcelNC(Stream excelFileStream, int sheetIndex = 0, int headerRowIndex = 0)
        {
            DataTable table = new DataTable();
            XSSFWorkbook workbook = new XSSFWorkbook(excelFileStream);
            XSSFSheet sheet = workbook.GetSheetAt(sheetIndex) as XSSFSheet;

            XSSFRow headerRow = sheet.GetRow(headerRowIndex) as XSSFRow;    //表头行的索引
            int cellCount = headerRow.LastCellNum;
            for (int i = headerRow.FirstCellNum; i < cellCount; i++)
            {
                DataColumn column = new DataColumn(headerRow.GetCell(i).StringCellValue);
                table.Columns.Add(column);
            }
            for (int i = 0; i <= sheet.LastRowNum; i++)
            {
                XSSFRow row = sheet.GetRow(i) as XSSFRow;
                if (row == null || row.Cells == null || row.Cells.TrueForAll(p => string.IsNullOrWhiteSpace(p.ToString()))) break;
                DataRow dataRow = table.NewRow();
                var reg = @"/^[1-9]\d{7}((0\d)|(1[0-2]))(([0|1|2]\d)|3[0-1])\d{3}$|^[1-9]\d{5}[1-9]\d{3}((0\d)|(1[0-2]))(([0|1|2]\d)|3[0-1])\d{3}([0-9]|[Xx])$/";
                for (int j = row.FirstCellNum; j < cellCount; j++)
                {
                    if ((((HSSFCell)row.GetCell(4)).StringCellValue).IndexOf(" ") >= 0 || !Regex.IsMatch(((HSSFCell)row.GetCell(4)).StringCellValue, reg))
                    {
                        if (j == 8 || j == 9)
                        { }
                    }
                    else
                    {
                        dataRow[j] = row.GetCell(j) != null ? GetStringByType2((XSSFCell)row.GetCell(j)) + "" : "";
                    }
                }
                table.Rows.Add(dataRow);
            }
            excelFileStream.Close();
            workbook = null;
            sheet = null;
            return table;
        }

        /// <summary>
        /// 社保信息根据单元格类型来取值
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        public static string GetStringByType1(HSSFCell cell)
        {
            string type = "";
            switch (cell.CellType)
            {
                case CellType.Blank:
                    type = "";
                    break;
                case CellType.Numeric:
                    type = cell.ToString();
                    break;
                case CellType.String:
                    type = cell.StringCellValue;
                    break;
                case CellType.Formula:
                    if (cell.CellFormula.Contains("IF") && cell.CellFormula.Contains("城镇"))
                        type = cell.NumericCellValue.ToString();
                    else if (cell.CellFormula.Contains("IF") && cell.CellFormula.Contains("农村"))
                        type = cell.NumericCellValue.ToString();
                    else if (cell.CellFormula.Contains("IF"))
                        type = cell.StringCellValue;
                    else
                        type = cell.NumericCellValue.ToString();
                    break;
                default:
                    type = "";
                    break;
            }
            return type;
        }

        /// <summary>
        /// 社保信息根据单元格类型来取值
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        public static string GetStringByType2(XSSFCell cell)
        {
            string type = "";
            switch (cell.CellType)
            {
                case CellType.Blank:
                    type = "";
                    break;
                case CellType.Numeric:
                    type = cell.ToString();
                    break;
                case CellType.String:
                    type = cell.StringCellValue;
                    break;
                case CellType.Formula:
                    if (cell.CellFormula.Contains("IF"))
                        type = cell.StringCellValue;
                    else
                        type = cell.NumericCellValue.ToString();
                    break;
                default:
                    type = "";
                    break;
            }
            return type;
        }

        //社保
        public static List<T> GetListFromExcel4<T>(string filePath, int sheetIndex = 0, int headerRowIndex = 0, int? startRowIndex = null, int? endRowIndex = null)
        {
            try
            {
                if (!System.IO.File.Exists(filePath)) throw new Exception(string.Format(Msg.Msg_FileNoExistWithPath, filePath));
                using (FileStream fileStream = System.IO.File.OpenRead(filePath))
                {
                    if (filePath.Contains(".xlsx"))
                    {
                        return GetListFromExcel4<T>(fileStream, "XSSF", sheetIndex, headerRowIndex, startRowIndex, endRowIndex);
                    }
                    else
                    {
                        return GetListFromExcel4<T>(fileStream, "HSSF", sheetIndex, headerRowIndex, startRowIndex, endRowIndex);
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 导入Excel
        /// </summary>
        public static List<T> GetListFromExcel4<T>(Stream fileStream, string type, int sheetIndex = 0, int headerRowIndex = 0, int? startRowIndex = null, int? endRowIndex = null)
        {
            try
            {
                if (sheetIndex < 0) sheetIndex = 0;
                if (headerRowIndex < 0) headerRowIndex = 0;

                IWorkbook workbook = null;
                if (type == "XSSF")
                {
                    workbook = new XSSFWorkbook(fileStream);
                }
                else if (type == "HSSF")
                {
                    workbook = new HSSFWorkbook(fileStream);
                }

                if (workbook == null || workbook.NumberOfSheets == 0)
                    throw new Exception(string.Format(Msg.Msg_FileNoDataWithPath, sheetIndex));

                ISheet sheet = workbook.GetSheetAt(sheetIndex);// as HSSFSheet;
                if (sheet == null) throw new Exception(string.Format(Msg.Msg_FileNoDataWithPath, sheetIndex));

                IRow headerRow = sheet.GetRow(headerRowIndex);// as HSSFRow;
                if (headerRow == null || headerRow.PhysicalNumberOfCells == 0) throw new Exception(string.Format(Msg.Msg_FileNoDataWithPath, sheetIndex));

                //验证标题列
                PropertyInfo[] properties = (Activator.CreateInstance<T>()).GetType().GetProperties();
                foreach (PropertyInfo property in properties) //每个属性都有对应列
                {
                    if (headerRow.Cells.FindIndex(c => property.Name.Equals(getHeadName(c.StringCellValue), StringComparison.CurrentCultureIgnoreCase)) < 0)
                        throw new Exception(string.Format(Msg.Msg_MissDataColumn, property.Name));
                }

                if (!startRowIndex.HasValue || startRowIndex.Value < 0) startRowIndex = sheet.FirstRowNum + headerRowIndex + 1;
                if (!endRowIndex.HasValue || endRowIndex.Value < 0) endRowIndex = sheet.LastRowNum;
                List<T> lists = new List<T>();
                for (int i = startRowIndex.Value; i <= endRowIndex; i++)
                {
                    IRow row = sheet.GetRow(i);// as HSSFRow;
                    if (row == null || row.Cells == null || row.Cells.TrueForAll(p => string.IsNullOrWhiteSpace(p.ToString()))) break;

                    T entity = Activator.CreateInstance<T>();
                    foreach (PropertyInfo property in properties)
                    {
                        try
                        {
                            int cellIndex = headerRow.Cells.FindIndex(c => property.Name.Equals(getHeadName(c.StringCellValue), StringComparison.CurrentCultureIgnoreCase));
                            if (cellIndex >= 0 && row.GetCell(cellIndex) != null)
                            {
                                object value = getTypeValue1(property.PropertyType, row.GetCell(cellIndex));
                                property.SetValue(entity, value, null);
                            }
                        }
                        catch (Exception e)
                        {
                            string error = string.Format("[列：{0}  行：{1}] ", property.Name, i + 1) + e.ToMessage();
                            throw new Exception(error);
                        }
                    }
                    lists.Add(entity);
                }
                return lists;
            }
            catch (Exception e)
            {
                throw new Exception(e.ToMessage());
            }
        }

        //社保
        private static object getTypeValue1(Type type, ICell value)
        {
            try
            {
                object result = null;
                string typeName = type.Name.ToLower();
                if (typeName == "nullable`1")
                {
                    if (!string.IsNullOrWhiteSpace(value.ToString()))
                    {
                        if (type.GetGenericArguments()[0].ToString().Contains("DateTime"))
                        {
                            DateTime dt = DateTime.ParseExact(value.NumericCellValue.ToString(), "yyyyMM", System.Globalization.CultureInfo.CurrentCulture);
                            return dt;
                        }
                        else
                        {
                            return getTypeValue1(type.GetGenericArguments()[0], value);
                        }
                    }
                }
                else
                {
                    switch (typeName)
                    {

                        case "short":
                        case "int16":
                            try { result = (short)(value.NumericCellValue); }
                            catch (Exception) { result = Convert.ToInt16(value.ToString()); }
                            break;
                        case "int":
                        case "int32":
                            try { result = (int)(value.NumericCellValue); }
                            catch (Exception) { result = Convert.ToInt32(value.ToString()); }
                            break;
                        case "long":
                        case "int64":
                            try { result = (long)(value.NumericCellValue); }
                            catch (Exception) { result = Convert.ToInt64(value.ToString()); }
                            break;
                        case "decimal":
                            try { result = (decimal)(value.NumericCellValue); }
                            catch (Exception) { result = Convert.ToDecimal(value.ToString()); }
                            break;
                        case "float":
                        case "single":
                            try { result = (float)(value.NumericCellValue); }
                            catch (Exception) { result = Convert.ToSingle(value.ToString()); }
                            break;
                        case "datetime":
                            try { result = value.DateCellValue; }
                            catch (Exception) { result = Convert.ToDateTime(value.ToString()); }
                            break;
                        case "bool":
                        case "boolean":
                            try { result = value.BooleanCellValue; }
                            catch (Exception) { result = Convert.ToBoolean(value.ToString()); }
                            break;
                        default:
                            try { result = value.StringCellValue; }
                            catch (Exception) { result = value.ToString(); }
                            break;
                    }
                }
                return result;
            }
            catch (Exception)
            {
                throw new Exception(string.Format(Msg.Msg_ImportDateFormatError, value));
            }
        }
        #endregion

        #region OleDb方式读取excel

        /// <summary>
        /// 读取Excel中数据
        /// </summary>
        /// <param name="strExcelPath"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static DataTable GetExcelTableByOleDB(string strExcelPath, string tableName = "Sheet1")
        {
            //Excel的连接
            OleDbConnection objConn = null;
            try
            {
                DataTable dtExcel = new DataTable();
                //数据表
                DataSet ds = new DataSet();
                //获取文件扩展名
                string strExtension = System.IO.Path.GetExtension(strExcelPath);
                string strFileName = System.IO.Path.GetFileName(strExcelPath);
                switch (strExtension)
                {
                    case ".xls":
                        objConn = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + strExcelPath + ";" + "Extended Properties=\"Excel 8.0;HDR=NO;IMEX=1;\"");
                        break;
                    case ".xlsx":
                        objConn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + strExcelPath + ";" + "Extended Properties=\"Excel 12.0;HDR=NO;IMEX=1;\"");
                        break;
                    default:
                        objConn = null;
                        break;
                }
                if (objConn == null)
                {
                    return null;
                }
                objConn.Open();
                //获取Excel中所有Sheet表的信息
                //System.Data.DataTable schemaTable = objConn.GetOleDbSchemaTable(System.Data.OleDb.OleDbSchemaGuid.Tables, null);
                //获取Excel的第一个Sheet表名
                // string tableName1 = schemaTable.Rows[0][2].ToString().Trim();
                string strSql = "select * from [" + tableName + "$]";
                //获取Excel指定Sheet表中的信息
                OleDbCommand objCmd = new OleDbCommand(strSql, objConn);
                OleDbDataAdapter myData = new OleDbDataAdapter(strSql, objConn);
                myData.Fill(ds, tableName);//填充数据
                objConn.Close();
                //dtExcel即为excel文件中指定表中存储的信息
                dtExcel = ds.Tables[tableName];
                return dtExcel;
            }
            catch (Exception e)
            {
                if (objConn != null)
                {
                    objConn.Close();
                }
                return null;
            }

        }

        #endregion

        /// <summary>
        /// 根据类型获取对应类型的值
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static object getTypeValue(Type type, object value)
        {
            try
            {
                object result = null;
                string typeName = type.Name.ToLower();
                if (typeName == "nullable`1")
                {
                    if (!string.IsNullOrWhiteSpace(value.ToString()))
                    {
                        return getTypeValue(type.GetGenericArguments()[0], value);
                    }
                }
                else
                {
                    switch (typeName)
                    {
                        case "short":
                        case "int16": result = Convert.ToInt16(value.ToString()); break;
                        case "int":
                        case "int32": result = Convert.ToInt32(value.ToString()); break;
                        case "long":
                        case "int64": result = Convert.ToInt64(value.ToString()); break;
                        case "decimal": result = Convert.ToDecimal(value.ToString()); break;
                        case "float":
                        case "single": result = Convert.ToSingle(value.ToString()); break;
                        case "datetime": result = Convert.ToDateTime(value.ToString()); break;
                        case "bool":
                        case "boolean": result = Convert.ToBoolean(value.ToString()); break;
                        default: result = value.ToString(); break;
                    }
                }
                return result;
            }
            catch (Exception)
            {
                throw new Exception(string.Format(Msg.Msg_ImportDateFormatError, value));
            }
        }

        /// <summary>  
        /// DataTable转化为List集合  
        /// </summary>  
        /// <typeparam name="T">实体对象</typeparam>  
        /// <param name="dt">datatable表</param>  
        /// <param name="isStoreDB">是否存入数据库datetime字段，date字段没事，取出不用判断</param>  
        /// <returns>返回list集合</returns>  
        public static List<T> TableToList<T>(DataTable dt, bool isStoreDB = true)
        {
            try
            {
                List<T> list = new List<T>();
                Type type = typeof(T);
                List<string> listColums = new List<string>();
                foreach (DataRow row in dt.Rows)
                {
                    PropertyInfo[] pArray = type.GetProperties(); //集合属性数组  
                    T entity = Activator.CreateInstance<T>(); //新建对象实例  
                    foreach (PropertyInfo p in pArray)
                    {
                        #region 2019-05-03 优化cvs文件转换 识别LqImExport特性
                        string keyName = p.Name;
                        if (true)//启用开关
                        {
                            var arrtibutesValue = p.GetCustomAttribute<LqImExportAttribute>();
                            if (arrtibutesValue != null)
                            {
                                keyName = arrtibutesValue.ExcelName;
                            }
                        }
                        #endregion
                        if (!dt.Columns.Contains(keyName) || row[keyName] == null || row[keyName] == DBNull.Value)
                        {
                            continue;  //DataTable列中不存在集合属性或者字段内容为空则，跳出循环，进行下个循环  
                        }
                        if (isStoreDB && p.PropertyType == typeof(DateTime) && Convert.ToDateTime(row[keyName]) < Convert.ToDateTime("1753-01-01"))
                        {
                            continue;
                        }
                        try
                        {
                            //var obj = Convert.ChangeType(row[keyName], p.PropertyType);//类型强转，将table字段类型转为集合字段类型  
                            var obj = getTypeValue(p.PropertyType, row[keyName]);
                            p.SetValue(entity, obj, null);
                        }
                        catch (Exception)
                        {
                            // throw;  
                        }

                    }
                    list.Add(entity);
                }
                return list;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// 解析CSV格式文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileStream"></param>
        /// <returns></returns>
        public static List<T> GetListFromCSVFile<T>(string filePath, Dictionary<string, string> dictColMapping = null)
        {
            try
            {
                //开头不留空，以行为单位；
                //可含或不含列名，含列名则居文件第一行；
                //一行数据不跨行，无空行；
                //以半角逗号（即,）作分隔符，列为空也要表达其存在；
                //列内容如存在半角逗号（即,），则用半角双引号（""）将该字段值包含起来；
                //列内容如存在半角双引号（即"），则用两个双引号（""）将其替换，再用半角双引号引号（即""）将该字段值包含起来；
                //文件读写时引号，逗号操作规则互逆；
                //内码格式不限，可为 ASCII、Unicode 或者其他；
                //不支持特殊字符。
                DataTable dt = new DataTable();
                using (TextFieldParser parser = new TextFieldParser(filePath))
                {
                    parser.Delimiters = new string[] { "," };
                    parser.HasFieldsEnclosedInQuotes = true;
                    string[] fields;
                    while ((fields = parser.ReadFields()) != null)
                    {
                        DataRow dr = dt.NewRow();
                        dt.Rows.Add(dr);
                        for (int i = 0; i < fields.Length; i++)
                        {
                            //先创建表头
                            if (dt.Columns.Count <= i)
                            {
                                if (i == 0)
                                {
                                    dt.Rows.Remove(dr);
                                }
                                DataColumn dc = new DataColumn(fields[i], typeof(String));
                                dt.Columns.Add(dc);
                            }
                            else
                            {
                                dr[i] = fields[i];
                            }
                        }
                    }
                }

                if (dictColMapping != null)
                    return TableToListByMappingCols<T>(dt, dictColMapping);

                return TableToList<T>(dt);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        ///  DataTable转化为指定实体集合，自定义表头
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <param name="dictColMapping"></param>
        /// <returns></returns>
        public static List<T> TableToListByMappingCols<T>(DataTable dt, Dictionary<string, string> dictColMapping)
        {
            try
            {
                List<T> list = new List<T>();
                Type type = typeof(T);
                List<string> listColums = new List<string>();
                foreach (DataRow row in dt.Rows)
                {
                    PropertyInfo[] pArray = type.GetProperties();
                    T entity = Activator.CreateInstance<T>();
                    foreach (PropertyInfo property in pArray)
                    {
                        if (!dt.Columns.Contains(dictColMapping[property.Name]) || row[dictColMapping[property.Name]] == null || row[dictColMapping[property.Name]] == DBNull.Value)
                        {
                            continue;
                        }
                        var obj = getTypeValue(property.PropertyType, row[dictColMapping[property.Name]]);
                        property.SetValue(entity, obj, null);
                    }

                    list.Add(entity);
                }
                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }

}