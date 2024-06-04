using SGSC.Frames;
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
    public partial class CustomerBankAccountsPage : Page
    {
        private int customerId;
        private int creditRequestId;
        private int? tansferAccountId = null;
        private int? transferAccountBankId = null;
        private int? directDebitAccountId = null;
		private int? directDebitAccountBankId = null;
		private bool edited = false;

        public CustomerBankAccountsPage(int customerId, int creditRequestID)
        {
            InitializeComponent();
            this.customerId = customerId;
            this.creditRequestId = creditRequestID;

            creditAdvisorSidebar.Content = new CreditAdvisorSidebar("");
            UserSessionFrame.Content = new UserSessionFrame();

            clearErrors();
        }

        private void clearErrors()
        {
            lbTansAccCardNumberError.Content = "";
            lbTansAccInterbankCodeError.Content = "";
            lbDomAccBankInterbankCodeError.Content = "";
            lbDomAccBankCardNumberError.Content = "";
        }

        private void getBankAccounts()
        {
            try
            {
                using (sgscEntities db = new sgscEntities())
                {
                    /*BankAccounts transferAccount = db.BankAccounts.Where(ba => ba.CustomerId == customerId && ba.AccountType == (int)BankAccounts.AccountTypes.TransferAccount).FirstOrDefault();
                    if(transferAccount != null)
                    {
                        tbTansAccCardNumber.Text = transferAccount.CardNumber;
                        tbTansAccBank.Text = transferAccount.Banks.Name;
                        tbTansAccInterbankCode.Text = transferAccount.InterbankCode;
                        tansferAccountId = transferAccount.BankAccountId;
                        transferAccountBankId = transferAccount.Banks.BankId;
					}

                    BankAccounts directDebitAccount = db.BankAccounts.Where(ba => ba.CustomerId == customerId && ba.AccountType == (int)BankAccounts.AccountTypes.DirectDebitAccount).FirstOrDefault();
                    if (directDebitAccount != null)
                    {
                        tbDomAccBankCardNumber.Text = directDebitAccount.CardNumber;
                        tbDomAccBank.Text = directDebitAccount.Banks.Name;
                        tbDomAccBankInterbankCode.Text = directDebitAccount.InterbankCode;
                        directDebitAccountId = directDebitAccount.BankAccountId;
						directDebitAccountBankId = directDebitAccount.Banks.BankId;
					}*/
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener las cuentas bancarias del cliente: " + ex.Message);
            }
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
                lbTansAccInterbankCodeError.Content = "Por favor introduzca un código interbancario válido";
            }

            if (string.IsNullOrEmpty(tbDomAccBankCardNumber.Text))
            {
                valid = false;
                lbDomAccBankCardNumberError.Content = "Por favor introduzca el número de tarjeta";
            }

            // Check if cardnumber is valid
            if (!Utils.TextValidator.ValidateCardNumber(tbDomAccBankCardNumber.Text))
            {
                valid = false;
                lbDomAccBankCardNumberError.Content = "Por favor introduzca un número de tarjeta válido";
            }

            if (string.IsNullOrEmpty(tbDomAccBankInterbankCode.Text))
            {
                valid = false;
                lbDomAccBankInterbankCodeError.Content = "Por favor introduzca el código interbancario";
            }

            // Check if interbank code is valid
            if (!Utils.TextValidator.ValidateTextNumeric(tbDomAccBankInterbankCode.Text, 20))
            {
                valid = false;
                lbDomAccBankInterbankCodeError.Content = "Por favor introduzca un código interbancario válido";
            }

            if (transferAccountBankId == null)
            {
                valid = false;
				lbTansAccInterbankCodeError.Content = "Por favor introduzca una CLABE válida";
			}

			if (directDebitAccountBankId == null)
			{
				valid = false;
				lbDomAccBankInterbankCodeError.Content = "Por favor introduzca una CLABE válida";
			}

			if (!valid)
            {
                return;
            }

            try
            {
                using (sgscEntities context = new sgscEntities())
                {
                    var transferAccount = new BankAccounts
                    {
                        CardNumber = tbTansAccCardNumber.Text,
                        BankBankId = transferAccountBankId,
                        InterbankCode = tbTansAccInterbankCode.Text,
                        AccountType = (int)BankAccount.AccountTypes.TransferAccount,
                        CardType = (int)BankAccount.CardTypes.Debit,
                        CustomerId = customerId
                    };

                    if (tansferAccountId != null)
                    {
                        transferAccount.BankAccountId = tansferAccountId.Value;
                    }

                    var directDebitAccount = new BankAccounts
                    {
                        CardNumber = tbDomAccBankCardNumber.Text,
                        BankBankId = directDebitAccountBankId,
                        InterbankCode = tbDomAccBankInterbankCode.Text,
                        AccountType = (int)BankAccount.AccountTypes.DirectDebitAccount,
                        CardType = (int)BankAccount.CardTypes.Debit,
                        CustomerId = customerId
					};

                    if (directDebitAccountId != null)
                    {
                        directDebitAccount.BankAccountId = directDebitAccountId.Value;
                    }

                    var creditRequest = context.CreditRequests.Where(cr => cr.CreditRequestId == creditRequestId).FirstOrDefault();
                    //creditRequest.TransferBankAccount = transferAccount;
                    //creditRequest.DirectDebitBankAccount = directDebitAccount;

                    context.BankAccounts.AddOrUpdate(transferAccount);
                    context.BankAccounts.AddOrUpdate(directDebitAccount);
                    context.SaveChanges();

                    MessageBox.Show("Cuentas bancarias guardadas exitosamente.");
                    App.Current.MainFrame.Content = new HomePageCreditAdvisor();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar las cuentas bancarias del cliente: " + ex.Message);
            }
        }

        private void CancelRegister(object sender, RoutedEventArgs e)
        {
            var result = System.Windows.Forms.MessageBox.Show("Está seguro que desea cancelar el registro?\nSi decide cancelarlo puede retomarlo más tarde.", "Cancelar registro", System.Windows.Forms.MessageBoxButtons.YesNo);
            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                App.Current.MainFrame.Content = new HomePageCreditAdvisor();
            }
        }

        private void cbUseTheSameAccount_Checked(object sender, RoutedEventArgs e)
        {
            // Disable debit account fields
            tbDomAccBankCardNumber.IsEnabled = false;
            tbDomAccBankInterbankCode.IsEnabled = false;

            // Copy transfer account fields to debit account fields
            tbDomAccBankCardNumber.Text = tbTansAccCardNumber.Text;
            tbDomAccBank.Text = tbTansAccBank.Text;
            tbDomAccBankInterbankCode.Text = tbTansAccInterbankCode.Text;
            directDebitAccountBankId = transferAccountBankId;
        }

        private void cbUseTheSameAccount_Unchecked(object sender, RoutedEventArgs e)
        {
            // Enable debit account fields
            tbDomAccBankCardNumber.IsEnabled = true;
            tbDomAccBankInterbankCode.IsEnabled = true;
        }

		private void tbTansAccInterbankCode_TextChanged(object sender, TextChangedEventArgs e)
		{
            var code = tbTansAccInterbankCode.Text;
            if(code.Length >= 3)
            {
                /*var bank = Banks.BankFromInterbankCodePrefix(code.Substring(0, 3));
				if (bank != null)
				{
					tbTansAccBank.Text = banks.Name;
					transferAccountBankId = banks.BankId;
				}
				else
				{
					tbTansAccBank.Text = "Banco Desconocido";
                    transferAccountBankId = null;
				}

				if (transferAccountBankId == null)
				{
					lbTansAccInterbankCodeError.Content = "Por favor introduzca una CLABE válida";
				}
				else
				{
					lbTansAccInterbankCodeError.Content = "";
				}*/
			}
            else
            {
                tbTansAccBank.Text = "";
                transferAccountBankId = null;
				lbTansAccInterbankCodeError.Content = "";
			}
		}
		private void tbDomAccBank_TextChanged(object sender, TextChangedEventArgs e)
		{
			var code = tbDomAccBankInterbankCode.Text;
			if (code.Length >= 3)
			{
				/*var bank = Banks.BankFromInterbankCodePrefix(code.Substring(0, 3));
				if (bank != null)
				{
					tbDomAccBank.Text = banks.Name;
					directDebitAccountBankId = bank.BankId;
				}
				else
				{
					tbTansAccBank.Text = "Banco Desconocido";
                    directDebitAccountBankId = null;
				}

				if (directDebitAccountBankId == null)
				{
					lbDomAccBankInterbankCodeError.Content = "Por favor introduzca una CLABE válida";
				}
				else
				{
					lbDomAccBankInterbankCodeError.Content = "";
				}*/
			}
			else
			{
				tbTansAccBank.Text = "";
                directDebitAccountBankId = null;
				lbDomAccBankInterbankCodeError.Content = "";
			}
		}
	}
}
