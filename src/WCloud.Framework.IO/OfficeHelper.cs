using Lib.extension;
using Lib.helper;
using Lib.io;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace WCloud.Framework.IO
{
    /// <summary>
    ///Excel 的摘要说明
    /// </summary>
    public class ExcelHelper
    {
        public readonly string ContentType = "application/ms-excel";

        public readonly IReadOnlyList<Type> SupportedTypes = new List<Type>()
        {
            typeof(bool),typeof(string),
            typeof(int),typeof(double),typeof(float),typeof(decimal),
            typeof(DateTime)
        }.AsReadOnly();

        public ICellStyle GetStyle(XSSFWorkbook workbook,
            short background,
            short color,
            bool BoldFont = false)
        {
            var style = workbook.CreateCellStyle();
            style.Alignment = HorizontalAlignment.Center;
            style.VerticalAlignment = VerticalAlignment.Center;
            style.FillPattern = FillPattern.SolidForeground;
            style.BorderTop = BorderStyle.Thin;
            style.BorderLeft = BorderStyle.Thin;
            style.BorderRight = BorderStyle.Thin;
            style.BorderBottom = BorderStyle.Thin;
            style.TopBorderColor = NPOI.HSSF.Util.HSSFColor.Black.Index;
            style.LeftBorderColor = NPOI.HSSF.Util.HSSFColor.Black.Index;
            style.RightBorderColor = NPOI.HSSF.Util.HSSFColor.Black.Index;
            style.BottomBorderColor = NPOI.HSSF.Util.HSSFColor.Black.Index;
            style.FillForegroundColor = background;

            var font = workbook.CreateFont();
            font.FontHeightInPoints = 14;

            if (BoldFont)
                font.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;

            font.Color = color;
            style.SetFont(font);
            return style;
        }

        /// <summary>
        /// return File(bs, "application/vnd.ms-excel");
        /// </summary>
        public byte[] ObjectToExcel<T>(List<T> list, string sheet_name = "sheet")
        {
            list = list ?? throw new Exception("参数为空");
            var t = typeof(T);

            var table = new DataTable();
            table.TableName = sheet_name ??
                t.GetCustomAttributes_<ExcelInfoAttribute>().FirstOrDefault()?.SheetName ??
                throw new Exception($"{nameof(sheet_name)}不能为空");

            var props = t.GetProperties().Select(x => new
            {
                p = x,
                attr = x.GetCustomAttributes_<ExcelInfoAttribute>().FirstOrDefault()
            });
            //根据索引排序
            props = props.OrderBy(x => x.attr?.Index ?? 0);

            foreach (var p in props)
            {
                //优先取标签name
                var name = p.attr?.HeaderName ?? p.p.Name;
                table.Columns.Add(name, typeof(string));
            }
            foreach (var m in list)
            {
                var data = props.Select(x => ConvertHelper.GetString(x.p.GetValue(m))).ToArray();
                table.Rows.Add(data);
            }

            return DataTableToExcel(table);
        }

        /// <summary>
        /// return File(bs, "application/vnd.ms-excel");
        /// </summary>
        public byte[] DataTableToExcel(DataTable tb, bool show_header = true)
        {
            tb = tb ?? throw new ArgumentNullException($"无法把空{nameof(DataTable)}转成Excel");

            using (var ms = new MemoryStream())
            {
                var workbook = new NPOI.XSSF.UserModel.XSSFWorkbook();
                var sheet = workbook.CreateSheet(ValidateHelper.IsNotEmpty(tb.TableName) ? tb.TableName : "sheet");

                var columns = tb.Columns.AsEnumerable_<DataColumn>();

                var row_index = 0;
                IRow NewRow() => sheet.CreateRow(row_index++);
                if (show_header)
                {
                    //头部
                    var header_style = this.GetStyle(workbook,
                        NPOI.HSSF.Util.HSSFColor.Black.Index,
                        NPOI.HSSF.Util.HSSFColor.White.Index);

                    var header = NewRow();
                    var cell_index = 0;
                    foreach (var col in columns)
                    {
                        var cell = header.CreateCell(cell_index++, CellType.String);
                        var data = col.ColumnName;
                        cell.SetCellValue(data);
                        cell.CellStyle = header_style;
                    }
                }

                var style = this.GetStyle(workbook,
                    NPOI.HSSF.Util.HSSFColor.White.Index,
                    NPOI.HSSF.Util.HSSFColor.Black.Index);

                foreach (var tb_row in tb.Rows.AsEnumerable_<DataRow>())
                {
                    var row = NewRow();
                    var cell_index = 0;
                    foreach (var tb_col in columns)
                    {
                        var cell = row.CreateCell(cell_index++, CellType.String);
                        var data = tb_row[tb_col.ColumnName];
                        cell.SetCellValue(ConvertHelper.GetString(data));
                        cell.CellStyle = style;
                    }
                }

                workbook.Write(ms);
                workbook.Clear();
                tb.Clear();

                var bs = ms.ToArray();
                return bs;
            }
        }

        private object GetCellValue(NPOI.SS.UserModel.ICell cell)
        {
            object value = null;
            switch (cell.CellType)
            {
                case CellType.Blank:
                    value = string.Empty;
                    break;
                case CellType.Boolean:
                    value = cell.BooleanCellValue;
                    break;
                case CellType.Error:
                    value = cell.ErrorCellValue;
                    break;
                case CellType.Formula:
                    value = cell.CellFormula;
                    break;
                case CellType.Numeric:
                    value = cell.NumericCellValue;
                    break;
                case CellType.String:
                    value = cell.StringCellValue;
                    break;
                case CellType.Unknown:
                default:
                    value = cell.StringCellValue ?? cell.ToString();
                    break;
            }
            return value;
        }

        private List<List<object>> SheetToList(ISheet sheet)
        {
            sheet = sheet ?? throw new ArgumentNullException(nameof(sheet));

            var list = new List<List<object>>();
            for (int i = 0; i < sheet.LastRowNum; ++i)
            {
                var row = sheet.GetRow(i);
                if (row == null) { continue; }
                var rowlist = new List<object>();
                for (int j = 0; j < row.LastCellNum; ++j)
                {
                    var cell = row.GetCell(j);
                    rowlist.Add(GetCellValue(cell));
                }
                list.Add(rowlist);
            }
            return list;
        }

        public List<List<List<object>>> ExcelToList(string path)
        {
            if (!IOHelper.FileHelper.Exists(path))
                throw new FileNotFoundException(nameof(path));

            var list = new List<List<List<object>>>();
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                var workbook = new XSSFWorkbook(stream);
                for (var i = 0; i < workbook.NumberOfSheets; ++i)
                {
                    var sheet = workbook.GetSheetAt(i);
                    list.Add(SheetToList(sheet));
                }
            }
            return list;
        }
    }
}
