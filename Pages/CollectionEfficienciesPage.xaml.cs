using SGSC.Utils;
using SGSC.Frames;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static SGSC.Pages.CollectionEfficienciesPage;

namespace SGSC.Pages
{
    /// <summary>
    /// Interaction logic for CollectionEfficienciesPage.xaml
    /// </summary>
    public partial class CollectionEfficienciesPage : Page
    {
        public int CreditRequestId { get; set; }

        public class CreditRequest
        {
            public int CreditRequestID { get; set; }
            public string Folio { get; set; }
            public string ClientName { get; set; }
            public int Term { get; set; }
            public double TotalAmount { get; set; }
            public string TotalAmountString { get; set; }
            public double OutstandingBalance { get; set; }
            public decimal Efficiency { get; set; }
        }

        public class Payment
        {
            public int PaymentID { get; set; }
            public int CreditRequestID { get; set; }
            public string FileNumber { get; set; }
            public DateTime PaymentDate { get; set; }
            public decimal Amount { get; set; }
            public decimal? Efficiency { get; set; }
            public decimal? AmountCharged { get; set; }
            public bool IsTotalRow { get; set; }

            public string EfficiencyString => $"{Efficiency:F2}%";
        }

        public CollectionEfficienciesPage(int creditRequestId)
        {
            InitializeComponent();
            CreditRequestId = creditRequestId;
            UserSessionFrame.Content = new UserSessionFrame();
            creditAdvisorSidebar.Content = new CreditAdvisorSidebar("creditRequest");
            this.Loaded += CollectionEfficienciesPage_Loaded;
        }

        private void CollectionEfficienciesPage_Loaded(object sender, RoutedEventArgs e)
        {

            var creditRequest = GetCreditRequestById(CreditRequestId);
            if (creditRequest != null)
            {
                DisplayCreditRequestDetails(creditRequest);
                DisplayPaymentSchedule(creditRequest);
            }
            else
            {
                MessageBox.Show("Solicitud de crédito no encontrada.");
            }
        }

        private CreditRequest GetCreditRequestById(int creditRequestId)
        {
            try
            {
                using (sgscEntities db = new sgscEntities())
                {
                    var creditRequest = (from cr in db.CreditRequests
                                         join c in db.Customers on cr.CustomerId equals c.CustomerId
                                         where cr.CreditRequestId == creditRequestId
                                         select new
                                         {
                                             cr.CreditRequestId,
                                             cr.FileNumber,
                                             ClientName = c.Name + " " + c.FirstSurname + " " + c.SecondSurname,
                                             cr.TimePeriod,
                                             cr.Amount,
                                             InterestRate = cr.InterestRate ?? 0.0,
                                             Payments = db.Payments
                                                          .Where(p => p.CreditRequestId == cr.CreditRequestId)
                                                          .Select(p => new
                                                          {
                                                              p.PaymentId,
                                                              p.CreditRequestId,
                                                              p.FileNumber,
                                                              p.PaymentDate,
                                                              p.Amount,
                                                              p.AmountCharged
                                                          }).ToList()
                                         }).FirstOrDefault();

                    if (creditRequest != null)
                    {
                        double totalAmount = creditRequest.Amount.HasValue ? Convert.ToDouble(creditRequest.Amount.Value) : 0.0;
                        double outstandingBalance = totalAmount * (1 - creditRequest.InterestRate / 100);

                        var payments = creditRequest.Payments.Select(p => new Payment
                        {
                            PaymentID = p.PaymentId,
                            CreditRequestID = p.CreditRequestId ?? 0,
                            FileNumber = p.FileNumber,
                            PaymentDate = p.PaymentDate ?? DateTime.MinValue,
                            Amount = p.Amount ?? 0.0m,
                            AmountCharged = p.AmountCharged,
                            Efficiency = p.Amount.HasValue && p.Amount.Value != 0 ? ((p.AmountCharged) / p.Amount.Value) * 100 : (decimal?)null,

                            IsTotalRow = false
                        }).ToList();

                        CreditRequest mappedCreditRequest = new CreditRequest
                        {
                            CreditRequestID = creditRequest.CreditRequestId,
                            Folio = creditRequest.FileNumber,
                            ClientName = creditRequest.ClientName,
                            Term = creditRequest.TimePeriod ?? 0,
                            TotalAmount = totalAmount,
                            TotalAmountString = totalAmount.ToString("C"),
                            OutstandingBalance = outstandingBalance,
                            Efficiency = payments.Any(p => p.Efficiency.HasValue) ? payments.Average(p => p.Efficiency.Value) : 0.0m
                        };

                        return mappedCreditRequest;
                    }

                    return null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error retrieving Credit Request: {ex.Message}");
                return null;
            }
        }



        private void DisplayCreditRequestDetails(CreditRequest creditRequest)
        {
            lblCustomerName.Content = creditRequest.ClientName;
            lblFileNumber.Content = creditRequest.Folio;
            lblCreditAmount.Content = creditRequest.TotalAmountString;
        }

        public List<Payment> GeneratePaymentSchedule(CreditRequest creditRequest, List<Payment> existingPayments)
        {
            List<Payment> paymentSchedule = new List<Payment>();
            int paymentNumber = 1; // Iniciar contador de número de pago

            // Recorrer los pagos existentes y asignar el número de pago
            foreach (var existingPayment in existingPayments.OrderBy(p => p.PaymentDate))
            {
                Payment payment = new Payment
                {
                    CreditRequestID = existingPayment.CreditRequestID,
                    FileNumber = paymentNumber.ToString(), // Asignar número de pago
                    PaymentDate = existingPayment.PaymentDate,
                    Amount = existingPayment.Amount,
                    AmountCharged = existingPayment.AmountCharged == -1 ? null : existingPayment.AmountCharged,
					Efficiency = existingPayment.Efficiency,
                    IsTotalRow = false
                };

                paymentSchedule.Add(payment);
                paymentNumber++; // Incrementar el número de pago
            }

            return paymentSchedule;
        }

        private void DisplayPaymentSchedule(CreditRequest creditRequest)
        {
            var existingPayments = GetExistingPayments(creditRequest.CreditRequestID);
            var paymentSchedule = GeneratePaymentSchedule(creditRequest, existingPayments);

            CalculateEfficiency(paymentSchedule);

            // Verificar si se están asignando los datos a la interfaz de usuario
            foreach (var payment in paymentSchedule)
            {
                Console.WriteLine($"Displaying Payment: FileNumber={payment.FileNumber}, Amount={payment.Amount}, AmountCharged={payment.AmountCharged}");
            }

            // Asignar el origen de datos a la tabla
            creditRequestsDataGrid.ItemsSource = paymentSchedule;
        }



        private List<Payment> GetExistingPayments(int creditRequestId)
        {
            try
            {
                using (sgscEntities db = new sgscEntities())
                {
                    var payments = (from p in db.Payments
                                    where p.CreditRequestId == creditRequestId
                                    select new Payment
                                    {
                                        PaymentID = p.PaymentId,
                                        CreditRequestID = p.CreditRequestId ?? 0,
                                        FileNumber = p.FileNumber,
                                        PaymentDate = p.PaymentDate ?? DateTime.MinValue,
                                        Amount = p.Amount ?? 0.0m,
                                        AmountCharged = p.AmountCharged,
                                        Efficiency = p.Amount.HasValue && p.Amount.Value != 0 ? ((p.AmountCharged) / p.Amount.Value) * 100 : (decimal?)null,
                                        IsTotalRow = false
                                    }).ToList();

                    return payments;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error retrieving Payments: {ex.Message}");
                return new List<Payment>();
            }
        }


        private void CalculateEfficiency(List<Payment> paymentSchedule)
        {
            foreach (var payment in paymentSchedule)
            {
                if (payment.Amount != 0)
                {
                    payment.Efficiency = (payment.AmountCharged / payment.Amount) * 100;
                }
                else
                {
                    payment.Efficiency = 0.0m;
                }
            }

            AddTotalRow(paymentSchedule);
        }

        private void AddTotalRow(List<Payment> paymentSchedule)
        {
            var totalAmount = paymentSchedule.Sum(p => p.Amount);
            var totalAmountCharged = paymentSchedule.Sum(p => p.AmountCharged);
            var totalEfficiency = totalAmount != 0 ? (totalAmountCharged / totalAmount) * 100 : 0.0m;

            Payment totalRow = new Payment
            {
                FileNumber = "Cobro total",
                Amount = totalAmount,
                AmountCharged = totalAmountCharged,
                Efficiency = totalEfficiency,
                IsTotalRow = true
            };

            paymentSchedule.Add(totalRow);
        }


        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            UserSession.LogOut();
        }

        private void HomePageCreditAdvisorMenu(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new HomePageCreditAdvisor());
        }

        private void DocumentsManager(object sender, RoutedEventArgs e)
        {
            int? creditRequestId = ObtainCreditRequestId();

            NavigationService.Navigate(new DocumentsManagerPage(creditRequestId));
        }

        private int? ObtainCreditRequestId()
        {
            return CreditRequestId;
        }
    }
}