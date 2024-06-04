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
    public partial class RegisterBankAccountsPage : Page
    {
        private int CustomerId;
        private BankAccount.AccountTypes AccountType;
		private int? AccountBankId = null;
        private IBankAccountRegisterCallback CallbackInterface;

        public RegisterBankAccountsPage(int customerId, BankAccount.AccountTypes type, IBankAccountRegisterCallback callbackInterface)
        {
            InitializeComponent();
            CustomerId = customerId;
            AccountType = type;
            CallbackInterface = callbackInterface;
            UserSessionFrame.Content = new UserSessionFrame();
            clearErrors();
		}

        private void clearErrors()
        {
            lbTansAccCardNumberError.Content = "";
            lbTansAccInterbankCodeError.Content = "";
        }

        private void SaveBankAccounts(object sender, RoutedEventArgs e)
        {
            bool valid = true;

            clearErrors();

            if (string.IsNullOrEmpty(tbTansAccCardNumber.Text))
            {
                valid = false;
                lbTansAccCardNumberError.Content = "Por favor introduzca el número de tarjeta";
            }

            // Check if cardnumber is valid
            if (!Utils.TextValidator.ValidateCardNumber(tbTansAccCardNumber.Text))
            {
                valid = false;
                lbTansAccCardNumberError.Content = "Por favor introduzca un número de tarjeta válido";
            }

            if (string.IsNullOrEmpty(tbTansAccInterbankCode.Text))
            {
                valid = false;
                lbTansAccInterbankCodeError.Content = "Por favor introduzca el código interbancario";
            }

            // Check if interbank code is valid
            if (!Utils.TextValidator.ValidateTextNumeric(tbTansAccInterbankCode.Text, 20))
            {
                valid = false;
                lbTansAccInterbankCodeError.Content = "Por favor introduzca una CLABE válida";
            }

			if (!valid)
            {
                return;
            }

            try
            {
                using (sgscEntities context = new sgscEntities())
                {
                    var bankAccount = new BankAccount
                    {
                        CardNumber = tbTansAccCardNumber.Text,
                        BankBankId = AccountBankId,
                        InterbankCode = tbTansAccInterbankCode.Text,
                        AccountType = (int)AccountType,
                        CardType = (int)BankAccount.CardTypes.Debit,
                        CustomerId = CustomerId
                    };

                    context.BankAccounts.Add(bankAccount);
                    context.SaveChanges();

                    App.Current.NotificationsPanel.ShowSuccess("Cuenta guardada");
                    CallbackInterface.OnBankAccountRegistered(bankAccount.BankAccountId);
                    App.Current.MainFrame.GoBack();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar la cuenta bancaria del cliente: " + ex.Message);
            }
        }

        private void CancelRegister(object sender, RoutedEventArgs e)
        {
            var result = System.Windows.Forms.MessageBox.Show("Está seguro que desea cancelar el registro de la cuenta bancaria?", "Cancelar registro", System.Windows.Forms.MessageBoxButtons.YesNo);
            if (result == System.Windows.Forms.DialogResult.Yes)
            {
				App.Current.MainFrame.GoBack();
			}
        }

		private void tbTansAccInterbankCode_TextChanged(object sender, TextChangedEventArgs e)
		{
            var code = tbTansAccInterbankCode.Text;
            if(code.Length >= 3)
            {
                var bank = Bank.BankFromInterbankCodePrefix(code.Substring(0, 3));
				if (bank != null)
				{
					tbTansAccBank.Text = bank.Name;
					AccountBankId = bank.BankId;
				}
				else
				{
					tbTansAccBank.Text = "Banco Desconocido";
                    AccountBankId = null;
				}

				if (AccountBankId == null)
				{
					lbTansAccInterbankCodeError.Content = "Por favor introduzca una CLABE válida";
				}
				else
				{
					lbTansAccInterbankCodeError.Content = "";
				}
			}
            else
            {
                tbTansAccBank.Text = "";
                AccountBankId = null;
				lbTansAccInterbankCodeError.Content = "";
			}
		}
	}
}
