using System;

namespace WCloud.Framework.IO
{
    public static class OfficeExtension
    {
        public static short ToNpoiColorIndex(this ExcelColorEnum color)
        {
            return (short)color;
        }
    }

    public enum ExcelColorEnum : short
    {
        Black = NPOI.HSSF.Util.HSSFColor.Black.Index,
        White = NPOI.HSSF.Util.HSSFColor.White.Index,
        Red = NPOI.HSSF.Util.HSSFColor.Red.Index,
        Green = NPOI.HSSF.Util.HSSFColor.Green.Index,
        Yellow = NPOI.HSSF.Util.HSSFColor.Yellow.Index,
        SkyBlue = NPOI.HSSF.Util.HSSFColor.SkyBlue.Index,
        Orange = NPOI.HSSF.Util.HSSFColor.Orange.Index,
        Grey80Percent = NPOI.HSSF.Util.HSSFColor.Grey80Percent.Index,
        Violet = NPOI.HSSF.Util.HSSFColor.Violet.Index,
    }

    public class ExcelInfoAttribute : Attribute
    {
        public virtual string SheetName { get; set; }
        public virtual string HeaderName { get; set; }
        public virtual int Index { get; set; }
        public virtual ExcelColorEnum? Color { get; set; }
        public virtual ExcelColorEnum? BackgroundColor { get; set; }
    }
}
