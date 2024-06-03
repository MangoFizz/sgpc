using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Xml.Linq;

namespace SGSC.Pages
{
    /// <summary>
    /// Lógica de interacción para CreditPromotions.xaml
    /// </summary>
    public partial class ManageCreditPromotionsPage : Page
    {
		public class CreditPromotionTableEntry
		{
			public int Id { get; set; }
			public string Name { get; set; }
			public string InterestRate { get; set; }
			public string Payments { get; set; }
			public string Interval { get; set; }
			public string Deadline { get; set; }
			public string Status { get; set; }
		}

		private ObservableCollection<CreditPromotionTableEntry> CreditPromotionsFiltered { get; set; }
		private List<CreditPromotionTableEntry> CreditPromotions { get; set; }

		public ManageCreditPromotionsPage()
		{
			InitializeComponent();
			creditAdvisorSidebar.Content = new Frames.AdminSidebar("creditPromotions");
			GetCreditPromotions();
		}

		private void GetCreditPromotions()
		{
			try
			{
				using (sgscEntities db = new sgscEntities())
				{
					var creditPromotionsFromDb = db.CreditPromotions.ToList();

					CreditPromotions = creditPromotionsFromDb
						.Select(prom => new CreditPromotionTableEntry()
						{
							Id = prom.CreditPromotionId,
							Name = prom.Name,
							InterestRate = $"{prom.InterestRate}%",
							Payments = prom.TimePeriod.ToString(),
							Interval = prom.Interval == 0 ? "Quincenal" : "Mensual",
							Deadline = prom.EndDate.ToString(),
							Status = prom.EndDate > DateTime.Now ? "Vigente" : "Vencida"
						}).ToList();

					ShowCreditPolicies();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error al cargar las promociones de crédito: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		public void ShowCreditPolicies()
		{
			var filteredPromotions = CreditPromotions.Where(promotion => promotion.Name.ToLower().Contains(tbDescription.Text.ToLower())).ToList();
			CreditPromotionsFiltered = new ObservableCollection<CreditPromotionTableEntry>(filteredPromotions);
			dgPromotions.ItemsSource = CreditPromotionsFiltered;
		}

		private void btnRegisterPolicy_Click(object sender, RoutedEventArgs e)
		{
			NavigationService.Navigate(new CreditPromotionDetails(-1));
		}


		private void btnModifyPolicy_Click(object sender, RoutedEventArgs e)
		{
			if (dgPromotions.SelectedItem != null)
			{
				var selectedPromotion = dgPromotions.SelectedItem as CreditPromotionTableEntry;
				NavigationService.Navigate(new CreditPromotionDetails(selectedPromotion.Id));
			}
			else
			{
				MessageBox.Show("Por favor, seleccione una promoción de crédito para modificar.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
			}
		}

		private void btnViewPolicy_Click(object sender, RoutedEventArgs e)
		{
			if (dgPromotions.SelectedItem != null)
			{
				var selectedPromotion = dgPromotions.SelectedItem as CreditPromotionTableEntry;
				NavigationService.Navigate(new CreditPromotionDetails(selectedPromotion.Id));
			}
			else
			{
				MessageBox.Show("Por favor, seleccione la promoción de crédito que desea visualizar.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
			}
		}

		private void tbFilter_TextChanged(object sender, TextChangedEventArgs e)
		{
			ShowCreditPolicies();
		}
	}
}
