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
        private class LayoutPayment
        {
            public string FileNumber { get; set; }
            public double Amount { get; set; }
            public string PaymentDate { get; set; }
            public string CardNumber { get; set; }
            public string BankName { get; set; }
        }

        private static void GenerateAndSaveLayout(List<LayoutPayment> rowsData)
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
            xlWorkSheet.Cells[1, 4] = "Número de tarjeta";
            xlWorkSheet.Cells[1, 5] = "Banco";

            for(int i = 0; i < rowsData.Count; i++)
            {
                xlWorkSheet.Cells[i + 2, 1] = rowsData[i].FileNumber;
                xlWorkSheet.Cells[i + 2, 2] = rowsData[i].Amount;
                xlWorkSheet.Cells[i + 2, 3] = rowsData[i].PaymentDate;
                xlWorkSheet.Cells[i + 2, 4] = rowsData[i].CardNumber;
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

        private static DateTime GetNextFortnight()
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

        public static void GeneratePaymentLayout()
        {
            DateTime nextFortnight = GetNextFortnight();
            using (var context = new sgscEntities())
            {
                var payments = context.Payments.Where(p => p.PaymentDate >= nextFortnight && p.PaymentDate < nextFortnight.AddDays(15)).ToList();

                if(payments.Count == 0)
                {
                    MessageBox.Show("No hay pagos para la quincena actual", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                List<LayoutPayment> rowsData = new List<LayoutPayment>();
                foreach (var payment in payments)
                {
                    rowsData.Add(new LayoutPayment
                    {
                        FileNumber = payment.FileNumber,
                        Amount = (double)payment.Amount,
                        PaymentDate = payment.PaymentDate.Value.ToString("dd/MM/yyyy"),
                        //CardNumber = payment.CreditRequests.DirectDebitBankAccount.CardNumber,
                        //BankName = payment.CreditRequests.DirectDebitBankAccount.Banks.Name
                    });
                }

                GenerateAndSaveLayout(rowsData);
            }
        }
    }
}
