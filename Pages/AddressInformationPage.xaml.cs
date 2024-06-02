using SGSC.Frames;
using System;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace SGSC.Pages
{
    /// <summary>
    /// Lógica de interacción para AddressInformation.xaml
    /// </summary>
    public partial class AddressInformationPage : Page
    {
        private int customerId;
        private int? addressId = null;

        public AddressInformationPage(int customerId)
        {
            this.customerId = customerId;
            InitializeComponent();
            UpdateAddressInformation();

            StepsSidebarFrame.Content = new CustomerRegisterStepsSidebar("Address");
            UserSessionFrame.Content = new UserSessionFrame();
        }

        private void AddAddressInformation(object sender, RoutedEventArgs e)
        {
            try
            {

                if (string.IsNullOrWhiteSpace(txtStreet.Text) || string.IsNullOrWhiteSpace(txtExternalNumber.Text) ||
                    string.IsNullOrWhiteSpace(txtZipCode.Text) || string.IsNullOrWhiteSpace(txtColony.Text))
                {
                    MessageBox.Show("Por favor, complete todos los campos de dirección.");
                    return;
                }


                if (!IsValidZipCode(txtZipCode.Text))
                {
                    MessageBox.Show("Por favor, introduzca un código postal válido.");
                    return;
                }

                var newCustomerAddressInfoes = new CustomerAddress
                {
                    ExternalNumber = txtExternalNumber.Text,
                    Street = txtStreet.Text,
                    ZipCode = txtZipCode.Text,
                    Colony = txtColony.Text,
                    CustomerId = customerId,
                    State = "Veracruz"
                };

                if(!string.IsNullOrWhiteSpace(txtInternalNumber.Text))
                {
                    newCustomerAddressInfoes.InternalNumber = txtInternalNumber.Text;
                }
                else
                {
                    newCustomerAddressInfoes.InternalNumber = null;
                }

                using (sgscEntities context = new sgscEntities())
                {
                    if(addressId != null)
                    {
                        newCustomerAddressInfoes.CustomerAddressId = addressId.Value;
						context.CustomerAddresses.AddOrUpdate(newCustomerAddressInfoes);
						context.SaveChanges();
						App.Current.NotificationsPanel.ShowSuccess("Datos actualizados");
					}
					else
                    {
						newCustomerAddressInfoes = context.CustomerAddresses.Add(newCustomerAddressInfoes);
						context.SaveChanges();
						addressId = newCustomerAddressInfoes.CustomerAddressId;
						App.Current.NotificationsPanel.ShowSuccess("Datos guardados");
					}
                }

                App.Current.MainFrame.Navigate(new PageWorkCenter(customerId));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al intentar guardar los datos de contacto: " + ex.Message);
            }
        }

        private void UpdateAddressInformation()
        {
            try
            {

                using (var context = new sgscEntities())
                {
                    var customerData = context.CustomerAddresses
                        .Where(customerDb => customerDb.CustomerId == customerId)
                        .FirstOrDefault();


                    if (customerData != null)
                    {
                        txtStreet.Text = customerData.Street;
                        txtExternalNumber.Text = customerData.ExternalNumber;
                        txtInternalNumber.Text = customerData.InternalNumber;
                        txtZipCode.Text = customerData.ZipCode;
                        txtColony.Text = customerData.Colony;
                        addressId = customerData.CustomerAddressId;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al intentar obtener los datos de contacto: " + ex.Message);
            }
        }

        private bool IsValidZipCode(string zipCode)
        {
            // Expresión regular para validar un código postal de 5 dígitos
            return Regex.IsMatch(zipCode, @"^\d{5}$");
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            var result = System.Windows.Forms.MessageBox.Show("Está seguro que desea cancelar el registro?\nSi decide cancelarlo puede retomarlo más tarde.", "Cancelar registro", System.Windows.Forms.MessageBoxButtons.YesNo);
            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                App.Current.MainFrame.Content = new HomePageCreditAdvisor();
            }
        }

		private void btnBack_Click(object sender, RoutedEventArgs e)
		{
			App.Current.MainFrame.GoBack();
		}
	}
}
