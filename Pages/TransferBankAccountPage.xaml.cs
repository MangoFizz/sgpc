﻿using SGSC.Components;
using SGSC.Frames;
using SGSC.Utils;
using System;
using System.Collections.Generic;
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

namespace SGSC.Pages
{
    /// <summary>
    /// Interaction logic for CustomerBankAccountsPage.xaml
    /// </summary>
    public partial class TransferBankAccountPage : Page
    {
		private int CreditRequestId;
		private int CustomerId;
		private int? BankAccountId;

        public TransferBankAccountPage(int customerId, int creditRequestID)
        {
			CustomerId = customerId;
			CreditRequestId = creditRequestID;
            InitializeComponent();
			StepsSidebarFrame.Content = new CreditRequestRegisterStepsFrame("Address");
			UserSessionFrame.Content = new UserSessionFrame();
			GetBankAccount();
		}

		private void GetBankAccount()
		{
			using (var context = new sgscEntities())
			{
				var creditRequest = context.CreditRequests.Find(CreditRequestId);
				if (creditRequest != null)
				{
					if (creditRequest.TransferBankAccountId != null)
					{
						var bankAccount = context.BankAccounts.Find(creditRequest.TransferBankAccountId);
						if (bankAccount != null)
						{
							tbTansAccBank.Text = bankAccount.Bank.Name;
							tbTansAccInterbankCode.Text = bankAccount.InterbankCode;
							tbTansAccCardNumber.Text = bankAccount.CardNumber;
							BankAccountId = bankAccount.BankAccountId;
						}
					}
				}
			}
		}

		private void btnSearchAccount_Click(object sender, RoutedEventArgs e)
		{
			App.Current.MainFrame.Navigate(new SearchBankAccount(CustomerId, BankAccount.AccountTypes.TransferAccount, this));
		}

		private void btnCancel_Click(object sender, RoutedEventArgs e)
		{
			var result = System.Windows.Forms.MessageBox.Show("¿Está seguro que desea cancelar el registro?", "Cancelar registro", System.Windows.Forms.MessageBoxButtons.YesNo);
			if (result == System.Windows.Forms.DialogResult.Yes)
			{
				App.Current.MainFrame.Content = new HomePageCreditAdvisor();
			}
		}

		private void btnContinue_Click(object sender, RoutedEventArgs e)
		{
			if(BankAccountId == null)
			{
				MessageBox.Show("Debe seleccionar una cuenta de transferencia");
				return;
			}

			using (var context = new sgscEntities())
			{
				var creditRequest = context.CreditRequests.Find(CreditRequestId);
				if (creditRequest != null)
				{
					creditRequest.TransferBankAccountId = BankAccountId.Value;
					context.CreditRequests.AddOrUpdate(creditRequest);
					context.SaveChanges();
					App.Current.NotificationsPanel.ShowSuccess("Cuenta asignada");
					App.Current.MainFrame.Navigate(new DirectDebtBankAccountPage(CustomerId, CreditRequestId));
				}
			}
		}

		private void btnBack_Click(object sender, RoutedEventArgs e)
		{
			App.Current.MainFrame.GoBack();
		}
	}

	public partial class TransferBankAccountPage : IBankAccountSearchCallback
	{
		public void OnBankAccountSelected(int bankAccountId)
		{
			using (var context = new sgscEntities())
			{
				var bankAccount = context.BankAccounts.Find(bankAccountId);
				if (bankAccount != null)
				{
					tbTansAccBank.Text = bankAccount.Bank.Name;
					tbTansAccInterbankCode.Text = bankAccount.InterbankCode;
					tbTansAccCardNumber.Text = bankAccount.CardNumber;
					BankAccountId = bankAccountId;
				}
			}
		}
	}
}
