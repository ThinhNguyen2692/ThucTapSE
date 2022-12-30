using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;

namespace TicketDesk.Web.Client.Models.Extensions
{
    public static class ExportExcell
    {
        public static Stream Excell<T>(List<T> datasource, FileInfo filepath)
        {
            Stream stream = new MemoryStream();
            ExcelPackage.LicenseContext = LicenseContext.Commercial;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage pck = new ExcelPackage(filepath))
            {
                ExcelWorksheet excelWorksheet = pck.Workbook.Worksheets.Add("Sheet1");
                excelWorksheet.Cells["A1"].LoadFromCollection<T>(datasource, true, TableStyles.Light1);
                excelWorksheet.Cells.AutoFitColumns();
                pck.SaveAs(stream);
                stream.Position = 0;
            }
            return stream;
        }
            
    }
}