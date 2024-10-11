using LibraryManagmentSystem.Interfaces;
using OfficeOpenXml;

namespace LibraryManagmentSystem.Services
{
    public class ExcelService : IExcelService
    {
        public byte[] GenerateExcelSheet<T>(IEnumerable<T> data, string sheetName)
        {

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add(sheetName);

                // loadFromCollection method loads the list data into the worksheet starting at cell A1

                //true argument indicates that the method should use the property names of the objects in the list as the headers of the columns.

                worksheet.Cells["A1"].LoadFromCollection(data, true); 

                // Format columns with DateTime to show as date and time
                for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
                {
                    if (worksheet.Cells[2, col].Value is DateTime)
                    {
                        worksheet.Column(col).Style.Numberformat.Format = "yyyy-mm-dd hh:mm:ss";

                    }
                }
                //the byte array is used to return the Excel file to the client as a file download.
                return package.GetAsByteArray();

                /*This method takes a list of data, generates an Excel file with the data,
                 * and returns the Excel file as a byte array, which can be sent to the client as a downloadable file.*/
            }
        }


    }
}
