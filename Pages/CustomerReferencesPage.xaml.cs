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

namespace SGSC.Pages
{
    /// <summary>
    /// Lógica de interacción para ReferenciasDelCliente.xaml
    /// </summary>
    public partial class CustomerReferencesPage : Page
    {
        private int customerId;
        private int? reference1Id;
        private int? reference2Id;
        private bool IsRegisteringCreditRequest;

		public CustomerReferencesPage(int customerId, bool isRegisteringCreditRequest = false)
        {
            InitializeComponent();

            this.customerId = customerId;
			IsRegisteringCreditRequest = isRegisteringCreditRequest;

			StepsSidebarFrame.Content = new CustomerRegisterStepsSidebar("References");
            UserSessionFrame.Content = new UserSessionFrame();

            GetReferences();
        }

        private void GetReferences()
        {
            try
            {
                using (sgscEntities db = new sgscEntities())
                {
                    var customerContacts = db.Contacts.Where(ba => ba.CustomerId == customerId).ToArray();
                    if(customerContacts.Length > 0)
                    {
                        tbName.Text = customerContacts[0].Name;
                        tbFirstSurname.Text = customerContacts[0].FirstSurname;
                        tbSecondSurname.Text = customerContacts[0].SecondSurname;
                        tbPhoneNumber.Text = customerContacts[0].PhoneNumber;
                        reference1Id = customerContacts[0].ContactId;
                    }

                    if (customerContacts.Length > 1)
                    {
                        tbName1.Text = customerContacts[1].Name;
                        tbFirstSurname2.Text = customerContacts[1].FirstSurname;
                        tbSecondSurname2.Text = customerContacts[1].SecondSurname;
                        tbPhoneNumber2.Text = customerContacts[1].PhoneNumber;
                        reference2Id = customerContacts[1].ContactId;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener las cuentas bancarias del cliente: " + ex.Message);
            }
        }

        private void btnContinue_Click(object sender, RoutedEventArgs e)
        {
            List<string> names = new List<string> { tbName.Text, tbFirstSurname.Text, tbSecondSurname.Text, tbName1.Text, tbFirstSurname2.Text, tbSecondSurname2.Text };
            List<string> phoneNumbers = new List<string> { tbPhoneNumber.Text, tbPhoneNumber2.Text };

            if (!names.All(name => !string.IsNullOrEmpty(name)))
            {
                MessageBox.Show("Los campos no pueden estar vacios", "Campos incompletos", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if(!phoneNumbers.All(phoneNumber => TextValidator.ValidateTextNumeric(phoneNumber, 10, 10, false)))
            {
                MessageBox.Show("Los campos de teléfono no cumplen con el formato correcto", "Campos incorrectos", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            //validate the fields
            if (Utils.TextValidator.ValidateMultipleNames(names))
            {
                using (sgscEntities db = new sgscEntities())
                {
                    try
                    {
                        Contact contact1 = new Contact();
                        contact1.Name = tbName.Text;
                        contact1.FirstSurname = tbFirstSurname.Text;
                        contact1.SecondSurname = tbSecondSurname.Text;
                        contact1.PhoneNumber = tbPhoneNumber.Text;
                        contact1.CustomerId = customerId;
                        contact1.Relationship = "Primo";
                        if (reference1Id != null)
                        {
                            contact1.ContactId = reference1Id.Value;
                        }
                        db.Contacts.AddOrUpdate(contact1);

						Contact contact2 = new Contact();
                        contact2.Name = tbName1.Text;
                        contact2.FirstSurname = tbFirstSurname2.Text;
                        contact2.SecondSurname = tbSecondSurname2.Text;
                        contact2.PhoneNumber = tbPhoneNumber2.Text;
                        contact2.CustomerId = customerId;
                        contact2.Relationship = "Primo";
                        if (reference2Id != null)
                        {
                            contact2.ContactId = reference2Id.Value;
                        }
                        db.Contacts.AddOrUpdate(contact2);

                        db.SaveChanges();

                        App.Current.NotificationsPanel.ShowSuccess("Datos guardados");

						if (IsRegisteringCreditRequest)
						{
							App.Current.MainFrame.Content = new RegisterCreditRequestPage(customerId);
						}
						else
						{
							App.Current.MainFrame.Content = new HomePageCreditAdvisor();
						}
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al registrar el cliente: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Los campos no cumplen con el formato correcto", "Campos incorrectos", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            var result = System.Windows.Forms.MessageBox.Show("¿Está seguro que desea cancelar el registro?\nSi decide cancelarlo puede retomarlo más tarde.", "Cancelar registro", System.Windows.Forms.MessageBoxButtons.YesNo);
            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                App.Current.MainFrame.Content = new HomePageCreditAdvisor();
            }
        }
		private void btnBack_Click(object sender, RoutedEventArgs e)
		{
			App.Current.MainFrame.GoBack();
		}

		private void tbName_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(tbName.Text + e.Text, @"[^a-zA-ZáéíóúÁÉÍÓÚüÜñÑ\s]+$"))
            {
                e.Handled = true;
            }
        }

        private void tbName_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
