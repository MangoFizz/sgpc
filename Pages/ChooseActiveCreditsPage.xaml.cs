using Org.BouncyCastle.Asn1.X509;
using SGSC.Frames;
using SGSC.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
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
using static SGSC.Pages.ViewActiveCreditsPage;

namespace SGSC.Pages
{
    /// <summary>
    /// Interaction logic for ActiveCreditsPage.xaml
    /// </summary>
    public partial class ChooseActiveCreditsPage : Page
    {
		private ObservableCollection<PaymentLayoutHelper.LayoutPayment> Payments;

		public ChooseActiveCreditsPage()
		{
			InitializeComponent();
			collectionExecutiveSidebar.Content = new CollectionExecutiveSidebar("activeCredits");
			GetActiveCredits();
		}

		private void GetActiveCredits()
		{
			try
			{
				using (var context = new sgscEntities())
				{
					DateTime nextFortnight = PaymentLayoutHelper.GetNextFortnight();
					var nextFortnightEnd = nextFortnight.AddDays(15);
					var payments = context.Payments.Where(p => p.PaymentDate >= nextFortnight && p.PaymentDate < nextFortnightEnd).ToList();

					var paymentsArray = payments.ToList();
					Payments = new ObservableCollection<PaymentLayoutHelper.LayoutPayment>();
					foreach (var item in paymentsArray)
					{
						var request = context.CreditRequests.Find(item.CreditRequestId);
						Payments.Add(new PaymentLayoutHelper.LayoutPayment
						{
							IsChecked = false,
							Id = item.PaymentId,
							FileNumber = request.FileNumber,
							Amount = "$ " + item.Amount.Value.ToString("0.00"),
							PaymentDate = item.PaymentDate.Value.ToString("dd/MM/yyyy"),
							CardNumber = request.DirectDebitBankAccount.InterbankCode,
							BankName = request.DirectDebitBankAccount.Bank.Name,
						});
					}
					dgPayments.ItemsSource = Payments;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("Error al intentar obtener los datos de los créditos activos: " + ex.Message);
			}
		}

		private void GenerateLayout(object sender, RoutedEventArgs e)
		{
			var selectedPayments = Payments.Where(p => p.IsChecked).ToList();
			if(selectedPayments.Count == 0)
			{
				return;
			} 
			PaymentLayoutHelper.GenerateAndSaveLayout(selectedPayments);
		}

		private void LogoutButton_Click(object sender, RoutedEventArgs e)
		{
			UserSession.LogOut();
		}

		private void tbFilter_TextChanged(object sender, TextChangedEventArgs e)
		{
			GetActiveCredits();
		}

		private void btnCancel_Click(object sender, RoutedEventArgs e)
		{
			App.Current.MainFrame.GoBack();
		}
	}
}
