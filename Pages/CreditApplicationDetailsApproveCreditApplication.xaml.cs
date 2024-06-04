using SGSC.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using SGSC.Utils;

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

        public List<CreditPolicies> GetAllCreditPolicies()
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
                return new List<CreditPolicies>();
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
                List<CreditPolicies> allPolicies = GetAllCreditPolicies();

                // Obtén los IDs de las políticas de crédito para la solicitud
                List<int> creditPolicyIdsForRequest = GetCreditPolicyIdsForRequest();

                // Agrega dinámicamente los CheckBox al StackPanel
                foreach (CreditPolicies policy in allPolicies)
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
                            CreditRequestCreditPolicies associationToRemove = db.CreditRequestCreditPolicies.FirstOrDefault(x => x.CreditRequestId == requestId && x.CreditPolicyId == previousPolicyId);

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
                            CreditRequestCreditPolicies newAssociation = new CreditRequestCreditPolicies
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
                            solicitud.Status = 0; // "Autorizado"
                        }
                        else if (rbtCorrect.IsChecked == true)
                        {
                            solicitud.Status = 1; // "Corregir"
                        }
                        else if (rbtReject.IsChecked == true)
                        {
                            solicitud.Status = 2; // "Rechazado"
                        }

                        db.SaveChanges();
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
    }
}
