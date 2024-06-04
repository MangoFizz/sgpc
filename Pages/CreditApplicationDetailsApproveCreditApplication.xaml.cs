using SGSC.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using SGSC.Utils;
using static SGSC.Pages.CollectionEfficienciesPage;
using System.Collections.ObjectModel;

namespace SGSC.Pages
{
    public partial class CreditApplicationDetailsApproveCreditApplication : Page
    {
        private int? requestId;
        private int Status;
        private string toStringStatus;

        public CreditApplicationDetailsApproveCreditApplication(int? requestId)
        {
            InitializeComponent();
            ChangeButtonColor("#F0F6EC");
            this.requestId = requestId;
            try
            {
                InformationCreditRequestStatus(); // Llamar aquí para cargar el estado de la solicitud
                GetObservation();
                ShowCreditPolicies();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al inicializar la página: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

		private void ChangeButtonColor(string hexColor)
		{
			System.Windows.Media.Color color = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(hexColor);

			SolidColorBrush brush = new SolidColorBrush(color);

			btnAproveRequest.Background = brush;
		}

		public void GetObservation()
        {
            try
            {
                using (sgscEntities db = new sgscEntities())
                {
                    var comment = db.CreditRequests
                                    .Where(cr => cr.CreditRequestId == requestId)
                                    .Select(cr => cr.Description)
                                    .FirstOrDefault();
                    txtObservations.Text = comment;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al obtener observaciones: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void InformationCreditRequestStatus()
        {
            try
            {
                using (sgscEntities db = new sgscEntities())
                {
                    var status = db.CreditRequests
                                   .Where(cr => cr.CreditRequestId == requestId)
                                   .Select(cr => cr.Status)
                                   .FirstOrDefault();

                    if (status != null)
                    {
                        this.Status = status.Value;
                        toStringStatus = Utils.CreditRequestStatus.GetRequestStatus(Status);

                        // Actualiza los controles de la interfaz según el estado
                        switch (toStringStatus)
                        {
                            case "Autorizar":
                                rbtAutorize.IsChecked = true;
                                break;
                            case "Corregir":
                                rbtCorrect.IsChecked = true;
                                break;
                            case "Rechazar":
                                rbtReject.IsChecked = true;
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        ToastNotification notification = new ToastNotification("El ID de la solicitud no está disponible, inténtelo más tarde", "Error");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al obtener el estado de la solicitud: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public List<CreditPolicy> GetAllCreditPolicies()
        {
            try
            {
                using (sgscEntities db = new sgscEntities())
                {
                    return db.CreditPolicies.ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al obtener políticas de crédito: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return new List<CreditPolicy>();
            }
        }

        public List<int> GetCreditPolicyIdsForRequest()
        {
            try
            {
                using (sgscEntities db = new sgscEntities())
                {
                    return db.CreditRequestCreditPolicies
                             .Where(crcp => crcp.CreditRequestId == requestId && crcp.CreditPolicyId.HasValue)
                             .Select(crcp => crcp.CreditPolicyId.Value)
                             .ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al obtener las políticas de crédito para la solicitud: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return new List<int>();
            }
        }

        public void ShowCreditPolicies()
        {
            try
            {
                // Limpia el panel antes de añadir nuevos CheckBox
                CreditPoliciesPanel.Children.Clear();

                // Obtén todas las políticas de crédito
                List<CreditPolicy> allPolicies = GetAllCreditPolicies();

                // Obtén los IDs de las políticas de crédito para la solicitud
                List<int> creditPolicyIdsForRequest = GetCreditPolicyIdsForRequest();

                // Agrega dinámicamente los CheckBox al StackPanel
                foreach (CreditPolicy policy in allPolicies)
                {
                    CheckBox cb = new CheckBox
                    {
                        Content = policy.Description,
                        FontFamily = (FontFamily)FindResource("FontNunito"),
                        FontSize = 16,
                        Margin = new Thickness(0, 5, 0, 0),
                        IsChecked = creditPolicyIdsForRequest.Contains(policy.CreditPolicyId)
                    };

                    CreditPoliciesPanel.Children.Add(cb);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al mostrar las políticas de crédito: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void DisableFields()
        {
            txtObservations.IsEnabled = false;
            rbtAutorize.IsEnabled = false;
            rbtCorrect.IsEnabled = false;
            rbtReject.IsEnabled = false;
            btnAcept.IsEnabled = false;
            btnCancel.IsEnabled = false;
            foreach (var child in CreditPoliciesPanel.Children)
            {
                if (child is CheckBox cb)
                {
                    cb.IsEnabled = false;
                }
            }
        }

        public void SaveCreditPolicies()
        {
            try
            {
                List<int> creditPolicyIds = new List<int>();

                foreach (var child in CreditPoliciesPanel.Children)
                {
                    if (child is CheckBox cb && cb.IsChecked == true)
                    {
                        using (sgscEntities db = new sgscEntities())
                        {
                            var policy = db.CreditPolicies.FirstOrDefault(p => p.Description == cb.Content.ToString());
                            if (policy != null)
                            {
                                creditPolicyIds.Add(policy.CreditPolicyId);
                            }
                        }
                    }
                }

                List<int> previousSelectedPolicyIds = GetCreditPolicyIdsForRequest();

                using (sgscEntities db = new sgscEntities())
                {
                    foreach (int previousPolicyId in previousSelectedPolicyIds)
                    {
                        if (!creditPolicyIds.Contains(previousPolicyId))
                        {
                            CreditRequestCreditPolicy associationToRemove = db.CreditRequestCreditPolicies.FirstOrDefault(x => x.CreditRequestId == requestId && x.CreditPolicyId == previousPolicyId);

                            if (associationToRemove != null)
                            {
                                db.CreditRequestCreditPolicies.Remove(associationToRemove);
                            }
                        }
                    }

                    foreach (int policyId in creditPolicyIds)
                    {
                        if (!previousSelectedPolicyIds.Contains(policyId))
                        {
                            CreditRequestCreditPolicy newAssociation = new CreditRequestCreditPolicy
                            {
                                CreditRequestId = (int)requestId,
                                CreditPolicyId = policyId
                            };

                            db.CreditRequestCreditPolicies.Add(newAssociation);
                        }
                    }

                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar las políticas de crédito: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public string GenerateRandomPaymentFileNumber()
        {
            return "PAGO" + new Random().Next(100000, 999999).ToString();
        }

        public void GeneratePayments()
        {
			using (sgscEntities db = new sgscEntities())
            {
				var request = db.CreditRequests.FirstOrDefault(cr => cr.CreditRequestId == requestId);
                var amount = request.Amount;
                var interestRate = request.InterestRate;
                var payments = request.TimePeriod;
                var interval = (CreditRequest.TimeIntervals)request.PaymentsInterval;

				var totalAmount = amount + (amount * (interestRate / 100));
				var paymentAmount = totalAmount / payments;
				var Payments = new List<Payment>();

				for (int i = 1; i <= payments; i++)
				{
					DateTime paymentDate;
					if (interval == CreditRequest.TimeIntervals.Monthly)
					{
						paymentDate = DateTime.Now.AddMonths(i);
					}
					else
					{
						paymentDate = DateTime.Now.AddMonths(i / 2);
						if (i % 2 == 1)
						{
							paymentDate = paymentDate.AddDays(15);
						}
					}

                    Payments.Add(new Payment
                    {
                        FileNumber = GenerateRandomPaymentFileNumber(),
                        PaymentDate = paymentDate,
                        Amount = (decimal)paymentAmount,
                        CreditRequestId = request.CreditRequestId,
                        AmountCharged = -1
                    });
				}

                db.Payments.AddRange(Payments);

                db.SaveChanges();
			}
		}

        public void SaveCreditRequestStatus()
        {
            try
            {
                using (sgscEntities db = new sgscEntities())
                {
                    var solicitud = db.CreditRequests.FirstOrDefault(cr => cr.CreditRequestId == requestId);

                    if (solicitud != null)
                    {
                        if (rbtAutorize.IsChecked == true)
                        {
                            solicitud.Status = (int)CreditRequest.RequestStatus.Authorized;



                        }
                        else if (rbtCorrect.IsChecked == true)
                        {
                            solicitud.Status = (int)CreditRequest.RequestStatus.WaitingForCorrection;
                        }
                        else if (rbtReject.IsChecked == true)
                        {
                            solicitud.Status = (int)CreditRequest.RequestStatus.Rejected;
                        }

                        db.SaveChanges();

                        GeneratePayments();
					}
                    else
                    {
                        MessageBox.Show("No se encontró ninguna solicitud con el ID proporcionado.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar el estado de la solicitud: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void SaveDescription()
        {
            try
            {
                using (sgscEntities db = new sgscEntities())
                {
                    var solicitud = db.CreditRequests.FirstOrDefault(cr => cr.CreditRequestId == requestId);

                    if (solicitud != null)
                    {
                        solicitud.Description = txtObservations.Text;
                        db.SaveChanges();
                    }
                    else
                    {
                        Console.WriteLine("No se encontró ninguna solicitud con el ID proporcionado.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar la descripción: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnClicAcept(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ValidateInputs())
                {
                    SaveCreditPolicies();
                    SaveDescription();
                    SaveCreditRequestStatus();
                    MessageBox.Show("Se ha actualizado el estado de la solicitud.");
                    App.Current.MainFrame.Content = new ViewPendingCreditRequests();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar la solicitud: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnClicPersonalInformation(object sender, RoutedEventArgs e)
        {
            try
            {
                var personalInformationPage = new CreditApplicationDetailsPersonalInformation(requestId);
                if (NavigationService != null)
                {
                    NavigationService.Navigate(personalInformationPage);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al navegar a la información personal: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnClicWorkCenter(object sender, RoutedEventArgs e)
        {
            try
            {
                var workCenterPage = new CreditApplicationDetailsWorkCenter(requestId);
                if (NavigationService != null)
                {
                    NavigationService.Navigate(workCenterPage);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al navegar al centro de trabajo: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnClicPersonalReferences(object sender, RoutedEventArgs e)
        {
            try
            {
                var personalReferences = new CreditApplicationDetailsPersonalReferences((int)requestId);
                if (NavigationService != null)
                {
                    NavigationService.Navigate(personalReferences);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al navegar a las referencias personales: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnClicRequest(object sender, RoutedEventArgs e)
        {
            try
            {
                var requestPage = new CreditApplicationDetailsRequest((int)requestId);
                if (NavigationService != null)
                {
                    NavigationService.Navigate(requestPage);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al navegar a la solicitud: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnClicBankAccounts(object sender, RoutedEventArgs e)
        {
            try
            {
                var bankAccountstPage = new CreditApplicationDetailsBankAccounts(requestId);
                if (NavigationService != null)
                {
                    NavigationService.Navigate(bankAccountstPage);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al navegar a las cuentas bancarias: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public bool ValidateInputs()
        {
            // Validar que al menos una política de crédito esté seleccionada
            bool isCreditPolicySelected = false;
            foreach (var child in CreditPoliciesPanel.Children)
            {
                if (child is CheckBox cb && cb.IsChecked == true)
                {
                    isCreditPolicySelected = true;
                    break;
                }
            }
            if (!isCreditPolicySelected)
            {
                lbErrorCreditPolicies.Content = "Debe seleccionar al menos una política de crédito.";
                return false;
            }

            // Validar que el campo de descripción no esté vacío
            if (string.IsNullOrWhiteSpace(txtObservations.Text))
            {
                lbErrorDescription.Content = "El campo de observaciones no puede estar vacío.";
                return false;
            }

            return true;
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            App.Current.MainFrame.Content = new HomePageCreditAnalyst();
        }

        private void BtnDocumentation(object sender, RoutedEventArgs e)
        {
            var documentation = new CreditApplicationDocuments(requestId.Value);
            if (NavigationService != null)
            {
                NavigationService.Navigate(documentation);
            }
            else
            {
                ToastNotification notification = new ToastNotification("No se puede realizar la navegación en este momento. Por favor, inténtelo más tarde.", "Error");

            }
        }

		private void BtnClicAproveRequest(object sender, RoutedEventArgs e)
		{
			var bankAccounts = new CreditApplicationDetailsApproveCreditApplication(requestId);
			if (NavigationService != null)
			{
				NavigationService.Navigate(bankAccounts);
			}
			else
			{
				ToastNotification notification = new ToastNotification("No se puede realizar la navegación en este momento. Por favor, inténtelo más tarde.", "Error");

			}
		}

		private void btnCancel_Click(object sender, RoutedEventArgs e)
		{
			var result = System.Windows.Forms.MessageBox.Show("¿Está seguro que desea cancelar el dictamen de la solicitud?", "Cancelar registro", System.Windows.Forms.MessageBoxButtons.YesNo);
			if (result == System.Windows.Forms.DialogResult.Yes)
			{
				App.Current.MainFrame.Content = new HomePageCreditAnalyst();
			}
		}
	}
}
