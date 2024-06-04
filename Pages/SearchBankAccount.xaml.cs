using Microsoft.Office.Interop.Excel;
using SGSC.Components;
using SGSC.Frames;
using SGSC.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Migrations;
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
using Page = System.Windows.Controls.Page;

namespace SGSC.Pages
{
    /// <summary>
    /// Interaction logic for CustomerBankAccountsPage.xaml
    /// </summary>
    public partial class SearchBankAccount : Page
    {
		private class BankAccountsTableEntry
		{
			public int Id { get; set; }
			public string InterbankCode { get; set; }
			public string CardNumber { get; set; }
			public string BankName { get; set; }
		}

		private ObservableCollection<BankAccountsTableEntry> BankAccounts;
		private int CurrentPage = 1;
		private int TotalPages = 1;
		private const int ItemsPerPage = 10;
		private bool UpdatingPagination = false;
		private int CustomerId;
		private BankAccount.AccountTypes AccountType;
		private IBankAccountSearchCallback CallbackInterface;

		public SearchBankAccount(int customerId, BankAccount.AccountTypes accountType, IBankAccountSearchCallback callbackInterface)
		{
			CustomerId = customerId;
			AccountType = accountType;
			CallbackInterface = callbackInterface;
			InitializeComponent();
			GetBankAccounts();
		}

		private void GetBankAccounts()
		{
			try
			{
				using (var context = new sgscEntities())
				{
					var accounts = context.BankAccounts
						.Where(account => account.CustomerId == CustomerId)
						.Where(account => account.AccountType == (int)AccountType)
						.Where(account => account.InterbankCode.Contains(tbInterbankCode.Text))
						.Where(account => account.CardNumber.Contains(tbCardNumber.Text));

					var accountsList = accounts.ToList();
					BankAccounts = new ObservableCollection<BankAccountsTableEntry>();
					foreach (var item in accountsList)
					{
						BankAccounts.Add(new BankAccountsTableEntry()
						{
							Id = item.BankAccountId,
							InterbankCode = item.InterbankCode,
							CardNumber = item.CardNumber,
							BankName = item.Bank.Name
						});
					}
					dgBankAccounts.ItemsSource = BankAccounts;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("Error al intentar obtener las cuentas bancarias del cliente: " + ex.Message);
			}
		}

		private void tbFilter_TextChanged(object sender, TextChangedEventArgs e)
		{
			GetBankAccounts();
		}

		private void btnRegisterBankAccount_Click(object sender, RoutedEventArgs e)
		{
			var customerInfoPage = new RegisterBankAccountsPage(CustomerId, AccountType, this as IBankAccountRegisterCallback);
			if (NavigationService != null)
			{
				NavigationService.Navigate(customerInfoPage);
			}
		}

		private void btnSelectBankAccount_Click(object sender, RoutedEventArgs e)
		{
			var accounts = dgBankAccounts.SelectedItem as BankAccountsTableEntry;
			if (accounts != null)
			{
				CallbackInterface.OnBankAccountSelected(accounts.Id);
				App.Current.MainFrame.GoBack();
			}
			else
			{
				MessageBox.Show("Por favor, seleccione un cliente de la tabla.");
			}
		}

		private void btnCancel_Click(object sender, RoutedEventArgs e)
		{
			NavigationService.GoBack();
		}
	}

	public partial class SearchBankAccount : IBankAccountRegisterCallback
	{
		public void OnBankAccountRegistered(int bankAccountId)
		{
			CallbackInterface.OnBankAccountSelected(bankAccountId);
			App.Current.MainFrame.GoBack();
		}
	}
}
