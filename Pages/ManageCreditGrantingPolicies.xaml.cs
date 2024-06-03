using SGSC.Frames;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SGSC.Pages
{
    public partial class ManageCreditGrantingPolicies : Page
    {
        private ObservableCollection<CreditPolicyWithStatus> CreditPoliciesFiltered { get; set; }
        private List<CreditPolicyWithStatus> CreditPolicies { get; set; }

		public ManageCreditGrantingPolicies()
        {
            InitializeComponent();
			creditAdvisorSidebar.Content = new Frames.AdminSidebar("creditPolicies");
			GetCreditPolicies();
        }

        private void GetCreditPolicies()
        {
			try
			{
				using (sgscEntities db = new sgscEntities())
				{
					var creditPoliciesFromDb = db.CreditPolicies.ToList();

					CreditPolicies = creditPoliciesFromDb
						.Select(policy => new CreditPolicyWithStatus(
							policy.CreditPolicyId,
							policy.Name,
							policy.Description,
							policy.EffectiveDate ?? DateTime.MinValue, // Asegurarse de manejar fechas nulas
							policy.EffectiveDate > DateTime.Now ? "Vigente" : "Vencida"
						))
						.ToList();

                    ShowCreditPolicies();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error al cargar las políticas de crédito: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

        public void ShowCreditPolicies()
        {
            var filteredPolicies = CreditPolicies.Where(policy => policy.Description.ToLower().Contains(tbDescription.Text.ToLower())).ToList();
            CreditPoliciesFiltered = new ObservableCollection<CreditPolicyWithStatus>(filteredPolicies);
            dgPolicies.ItemsSource = CreditPoliciesFiltered;
        }

        private void btnRegisterPolicy_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddCreditPolicy(null));
        }


        private void btnModifyPolicy_Click(object sender, RoutedEventArgs e)
        {
            if (dgPolicies.SelectedItem != null)
            {
                var selectedPolicy = dgPolicies.SelectedItem as CreditPolicyWithStatus;
				NavigationService.Navigate(new AddCreditPolicy(selectedPolicy));
            }
            else
            {
                MessageBox.Show("Por favor, seleccione una política de crédito para modificar.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

		private void btnViewPolicy_Click(object sender, RoutedEventArgs e)
		{
			if (dgPolicies.SelectedItem != null)
			{
				var selectedPolicy = dgPolicies.SelectedItem as CreditPolicyWithStatus;
				NavigationService.Navigate(new AddCreditPolicy(selectedPolicy, true));
			}
			else
			{
				MessageBox.Show("Por favor, seleccione la política de crédito que desea visualizar.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
			}
		}

		private void tbFilter_TextChanged(object sender, TextChangedEventArgs e)
		{
            ShowCreditPolicies();
		}
	}
}
