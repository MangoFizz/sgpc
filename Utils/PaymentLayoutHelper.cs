using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;

namespace SGSC.Utils
{
    public static class PaymentLayoutHelper
    {
        public class LayoutPayment
        {
            public bool IsChecked { get; set; }

			public int Id { get; set; }
            public string FileNumber { get; set; }
            public string Amount { get; set; }
            public string PaymentDate { get; set; }
            public string CardNumber { get; set; }
            public string BankName { get; set; }
        }

        public static void GenerateAndSaveLayout(List<LayoutPayment> rowsData)
        {
            Excel.Application excelApp = new Excel.Application();
            if (excelApp == null)
            {
                MessageBox.Show("Excel no está instalado en este equipo", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Excel.Workbook xlWorkBook = excelApp.Workbooks.Add();
            Excel.Worksheet xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

            xlWorkSheet.Cells[1, 1] = "Folio";
            xlWorkSheet.Cells[1, 2] = "Descuento";
            xlWorkSheet.Cells[1, 3] = "Fecha de cobro";
            xlWorkSheet.Cells[1, 4] = "CLABE";
            xlWorkSheet.Cells[1, 5] = "Banco";

            for(int i = 0; i < rowsData.Count; i++)
            {
                MessageBox.Show(rowsData[i].CardNumber);
                xlWorkSheet.Cells[i + 2, 1] = rowsData[i].FileNumber;
                xlWorkSheet.Cells[i + 2, 2] = rowsData[i].Amount;
                xlWorkSheet.Cells[i + 2, 3] = rowsData[i].PaymentDate;
                xlWorkSheet.Cells[i + 2, 4] = "# " + rowsData[i].CardNumber;
                xlWorkSheet.Cells[i + 2, 4].NumberFormat = "@";
                xlWorkSheet.Cells[i + 2, 5] = rowsData[i].BankName;
            }


			SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv",
                Title = "Guardar archivo CSV"
            };

            DialogResult result = saveFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                string csvFilePath = saveFileDialog.FileName;
                xlWorkBook.SaveAs(csvFilePath, Excel.XlFileFormat.xlCSV, Excel.XlSaveAsAccessMode.xlExclusive);
            }

            xlWorkBook.Close(false, null, null);
            excelApp.Quit();

            Marshal.ReleaseComObject(xlWorkSheet);
            Marshal.ReleaseComObject(xlWorkBook);
            Marshal.ReleaseComObject(excelApp);
        }

        public static DateTime GetNextFortnight()
        {
            DateTime currentDate = DateTime.Now;
            int currentDay = currentDate.Day;
            int currentMonth = currentDate.Month;
            int currentYear = currentDate.Year;

            if (currentDay <= 15)
            {
                return new DateTime(currentYear, currentMonth, 15);
            }
            else
            {
                return new DateTime(currentYear, currentMonth + 1, 1);
            }
        }
    }
}
