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
    public partial class RegisterCreditRequestPage : Page
    {
        private int CustomerId = -1;
        private int? CreditRequestId = null;
        private double totalAmount = 0.0;

        public RegisterCreditRequestPage(int idCustomer, int? creditRequestId = null)
        {
            InitializeComponent();

			StepsSidebarFrame.Content = new CreditRequestRegisterStepsFrame("PersonalInfo");
			UserSessionFrame.Content = new UserSessionFrame();

			CustomerId = idCustomer;
            CreditRequestId = creditRequestId;

            lbAmountError.Content = "";
            lbPromotionError.Content = "";
            lbPurposeError.Content = "";
            
            retrieveCreditPromotions();
            
            if(CreditRequestId != null)
            {
				GetCreditRequest();
			}
		}

        private void GetCreditRequest()
        {
            using (var context = new sgscEntities())
            {
				var creditRequest = context.CreditRequests.Where(cr => cr.CreditRequestId == CreditRequestId).FirstOrDefault();
				if (creditRequest != null)
                {
					tbPurpose.Text = creditRequest.Purpose;
					tbAmount.Text = creditRequest.Amount.ToString();
				}
			}
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
                lbInterestRate.Content = selectedPromotion.InterestRate.ToString() + "%";
                calculateTotalAmount();
            }
        }

        private void tbAmount_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);
        }

        private void tbAmount_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(tbAmount.Text.Length >= 1)
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

                var interval = selectedPromotion.Interval == 0 ? "Quincenas" : "Meses";
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
                    lbDiscount.Content = "$ " + (totalAmount / selectedPromotion.TimePeriod).ToString("0.00");
                    this.totalAmount = totalAmount;
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
                if(CreditRequestId == null)
                {
                    RegisterCreditRequest();
                }
                else
                {
                    UpdateCreditRequest();
				}
            }
            
        }

        private void RegisterCreditRequest()
        {
            var selectedPromotion = (CreditPromotion)cbCreditPromotions.SelectedItem;
            using (var context = new sgscEntities())
            {
                var creditRequest = new CreditRequest();
                var filenumber = "CR" + DateTime.Now.ToString("yyyyMMddHHmmss");
                creditRequest.FileNumber = filenumber;
                creditRequest.Amount = this.totalAmount;
                creditRequest.Status = (int)CreditRequest.RequestStatus.Captured;
                creditRequest.TimePeriod = selectedPromotion.TimePeriod;
                creditRequest.Purpose = tbPurpose.Text;
                creditRequest.InterestRate = selectedPromotion.InterestRate;
                creditRequest.CreationDate = DateTime.Now;
                creditRequest.EmployeeId = Utils.UserSession.Instance.Id;
                creditRequest.CustomerId = CustomerId;
                creditRequest.PaymentsInterval = selectedPromotion.Interval;
                creditRequest.Description = "";
				creditRequest.SettlementDate = DateTime.Now.AddMonths((int)(selectedPromotion.Interval == 0 ? Math.Round((decimal)selectedPromotion.TimePeriod / 2) : selectedPromotion.TimePeriod));

				context.CreditRequests.Add(creditRequest);
                try
                {
                    context.SaveChanges();
                    App.Current.NotificationsPanel.ShowSuccess("Datos guardados");
                    App.Current.MainFrame.Navigate(new TransferBankAccountPage(CustomerId, creditRequest.CreditRequestId));
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al registrar la solicitud de crédito: " + ex.Message);
                    throw;
                }
            }
        }   

        private void UpdateCreditRequest()
        {
			using (var context = new sgscEntities())
            {
				var creditRequest = context.CreditRequests.Where(cr => cr.CreditRequestId == CreditRequestId).FirstOrDefault();
				if (creditRequest != null)
                {
					var selectedPromotion = (CreditPromotion)cbCreditPromotions.SelectedItem;
					creditRequest.Amount = this.totalAmount;
					creditRequest.TimePeriod = selectedPromotion.TimePeriod;
					creditRequest.Purpose = tbPurpose.Text;
					creditRequest.InterestRate = selectedPromotion.InterestRate;
					creditRequest.PaymentsInterval = selectedPromotion.Interval;
					creditRequest.SettlementDate = DateTime.Now.AddMonths((int)(selectedPromotion.Interval == 0 ? Math.Round((decimal)selectedPromotion.TimePeriod / 2) : selectedPromotion.TimePeriod));
					try
                    {
						context.SaveChanges();
						App.Current.NotificationsPanel.ShowSuccess("Datos actualizados");
						App.Current.MainFrame.Navigate(new TransferBankAccountPage(CustomerId, creditRequest.CreditRequestId));
					}
					catch (Exception ex)
                    {
						MessageBox.Show("Error al actualizar la solicitud de crédito: " + ex.Message);
						throw;
					}
				}
			}
		}

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
			var result = System.Windows.Forms.MessageBox.Show("Está seguro que desea cancelar el registro?", "Cancelar registro", System.Windows.Forms.MessageBoxButtons.YesNo);
			if (result == System.Windows.Forms.DialogResult.Yes)
			{
			    App.Current.MainFrame.Content = new HomePageCreditAdvisor();
			}
        }

		private void btnPaymentsPreview_Click(object sender, RoutedEventArgs e)
		{
            if(tbAmount.Text.Length < 1)
            {
				lbAmountError.Content = "Introduzca un monto";
			}
			else if (cbCreditPromotions.SelectedIndex == -1)
            {
				lbPromotionError.Content = "Seleccione una promoción";
			}
			else
            {
                var promotion = cbCreditPromotions.SelectedItem as CreditPromotion;
                var interval = (CreditRequest.TimeIntervals)promotion.Interval;
                var amount = int.Parse(tbAmount.Text);
				App.Current.MainFrame.Navigate(new PaymentsPreviewPage(amount, promotion.InterestRate, promotion.TimePeriod, interval));
			}
		}
	}
}
