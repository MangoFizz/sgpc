using SGSC.Components;
using SGSC.Frames;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace SGSC.Pages
{
    /// <summary>
    /// Lógica de interacción para RegisterCreditRequest.xaml
    /// </summary>
    public partial class RegisterCreditRequest : Page
    {
        private int idCustomer = -1;
        private double totalAmount = 0.0;
        public RegisterCreditRequest(int idCustomer)
        {
            InitializeComponent();

			StepsSidebarFrame.Content = new CreditRequestRegisterStepsFrame("PersonalInfo");
			UserSessionFrame.Content = new UserSessionFrame();

			this.idCustomer = idCustomer;
            retrieveCreditPromotions();
            lbAmountError.Content = "";
            lbPromotionError.Content = "";
            lbPurposeError.Content = "";
        }

        private void retrieveCreditPromotions()
        {
            using (var context = new sgscEntities())
            {
                var currentDate = DateTime.Now;
                var creditPromotions = context.CreditPromotions
                    .Where(cp => cp.StartDate <= currentDate && cp.EndDate >= currentDate)
                    .ToList();
                if (creditPromotions != null)
                {
                    foreach (var creditPromotion in creditPromotions)
                    {
                        cbCreditPromotions.Items.Add(creditPromotion);
                    }
                }
            }
            cbCreditPromotions.DisplayMemberPath = "Name";
        }

        private void cbCreditPromotions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbCreditPromotions.SelectedIndex != -1)
            {
                var selectedPromotion = (CreditPromotion)cbCreditPromotions.SelectedItem;
                if(selectedPromotion.Interval == 1)
                {
                    // lbTimePeriod.Content = selectedPromotion.TimePeriod+" Quincenas";
                }
                else if(selectedPromotion.Interval == 2)
                {
                    // lbTimePeriod.Content = selectedPromotion.TimePeriod+" Meses";
                }
                lbInterestRate.Content = selectedPromotion.InterestRate.ToString() + "%";

                //aqui que calcule el monto
                calculateTotalAmount();
            }
        }

        private void tbAmount_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);
        }

        private void tbAmount_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(tbAmount.Text.Length < 1)
            {
                // lbTotalAmount.Content = "0.0";
            }
            else
            {
            calculateTotalAmount();
            }
            
        }

        private void calculateTotalAmount()
        {
            if (cbCreditPromotions.SelectedIndex != -1)
            {
                var selectedPromotion = (CreditPromotion)cbCreditPromotions.SelectedItem;
                var amountIntroduced = 0.0;
                var timePeriodInMonths = 0.0;

                lbInterestRate.Content = selectedPromotion.InterestRate.ToString("0.00") + "%";

                var interval = selectedPromotion.Interval == 1 ? "Quincenas" : "Meses";
				lbPeriod.Content = $"{selectedPromotion.TimePeriod} {interval}";

			    if (double.TryParse(tbAmount.Text, out amountIntroduced))
                {
                    if (selectedPromotion.Interval == 1)
                    {
                        timePeriodInMonths = (double)(selectedPromotion.TimePeriod / 2);
                    }
                    if (selectedPromotion.Interval == 2)
                    {
                        timePeriodInMonths = (double)selectedPromotion.TimePeriod;
                    }

                    var totalAmount = amountIntroduced + (amountIntroduced * (selectedPromotion.InterestRate / 100));
                    lbTotal.Content = "$ " + totalAmount.ToString("0.00");
                    lbDiscount.Content = "$ " + (totalAmount / selectedPromotion.TimePeriod).ToString("0.00") + " / pago";
				}
            }
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            lbPurposeError.Content = "";
            lbAmountError.Content = "";
            lbPromotionError.Content = "";

            bool isValid = true;
            if (cbCreditPromotions.SelectedIndex == -1)
            {
                lbPromotionError.Content= "Seleccione una promoción";
                isValid = false;
            }
            if (tbAmount.Text.Length < 1)
            {
                lbAmountError.Content = "Introduzca un monto";
                isValid = false;
            }
            if (tbPurpose.Text.Length < 1)
            {
                lbPurposeError.Content = "Introduzca un propósito";
                isValid = false;
            }
            if (isValid)
            {
                registerCreditRequest();
            }
            
        }

        private void registerCreditRequest()
        {
            var selectedPromotion = (CreditPromotion)cbCreditPromotions.SelectedItem;
            using (var context = new sgscEntities())
            {
                var creditRequest = new CreditRequest();
                var filenumber = "CR" + DateTime.Now.ToString("yyyyMMddHHmmss");
                creditRequest.FileNumber = filenumber;
                creditRequest.Amount = this.totalAmount;
                creditRequest.Status = 0;
                creditRequest.TimePeriod = selectedPromotion.TimePeriod;
                creditRequest.Purpose = tbPurpose.Text;
                creditRequest.InterestRate = selectedPromotion.InterestRate;
                creditRequest.CreationDate = DateTime.Now;
                creditRequest.EmployeeId = Utils.UserSession.Instance.Id;
                creditRequest.CustomerId = idCustomer;
                creditRequest.PaymentsInterval = selectedPromotion.Interval;
                creditRequest.Description = "";

                context.CreditRequests.Add(creditRequest);
                try
                {
                    context.SaveChanges();
                    MessageBox.Show("Solicitud de crédito registrada exitosamente");

                    var cr = context.CreditRequests.Where(c => c.FileNumber == filenumber).FirstOrDefault();
                    App.Current.MainFrame.Content = new CustomerBankAccountsPage(idCustomer, cr.CreditRequestId);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al registrar la solicitud de crédito: " + ex.Message);
                    throw;
            }
        }
    }   

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            App.Current.MainFrame.Content = new HomePageCreditAdvisor();
        }
    }
}
