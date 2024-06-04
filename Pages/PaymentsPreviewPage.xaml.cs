using SGSC.Components;
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

namespace SGSC.Pages
{
	/// <summary>
	/// Interaction logic for CreditRequestPaymentsTable.xaml
	/// </summary>
	public partial class PaymentsPreviewPage : Page
	{
		private class PaymentsTableEntry
		{
			public string Number { get; set; }
			public string Date { get; set; }
			public string Amount { get; set; }
		}

		private ObservableCollection<PaymentsTableEntry> Payments;

		public PaymentsPreviewPage(double amount, double interestRate, int payments, CreditRequest.TimeIntervals interval)
		{
			InitializeComponent();
			StepsSidebarFrame.Content = new CreditRequestRegisterStepsFrame("PersonalInfo");
			GeneratePaymentsTable(amount, interestRate, payments, interval);
		}

		private void GeneratePaymentsTable(double amount, double interestRate, int payments, CreditRequest.TimeIntervals interval)
		{
			var totalAmount = amount + (amount * (interestRate / 100));
			var paymentAmount = totalAmount / payments;
			
			Payments = new ObservableCollection<PaymentsTableEntry>();
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

				Payments.Add(new PaymentsTableEntry
				{
					Number = $"#{i}",
					Date = paymentDate.ToString("dd/MM/yyyy"),
					Amount = "$ " + paymentAmount.ToString("0.00")
				});
			}

			dgPayments.ItemsSource = Payments;
		}

		private void btnBack_Click(object sender, RoutedEventArgs e)
		{
			App.Current.MainFrame.GoBack();
		}
	}
}
