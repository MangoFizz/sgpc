using Microsoft.Office.Core;
using SGSC.Frames;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Reflection.Emit;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SGSC.Pages
{
    /// <summary>
    /// Lógica de interacción para AddressInformation.xaml
    /// </summary>
    public partial class AddressInformationPage : Page
    {
        private int customerId;
        private int? addressId = null;
        private bool IsRegisteringCreditRequest;
        private bool DoNotUpdateZipCode = false;

        public AddressInformationPage(int customerId, bool isRegisteringCreditRequest = false)
        {
            InitializeComponent();
            IsRegisteringCreditRequest = isRegisteringCreditRequest;

			tbZipCode.FilterMode = AutoCompleteFilterMode.Contains;

			StepsSidebarFrame.Content = new CustomerRegisterStepsSidebar("Address");
			UserSessionFrame.Content = new UserSessionFrame();

			this.customerId = customerId;

            PopulateAddressTypeCombobox();
			UpdateAddressInformation();
        }

        private void PopulateAddressTypeCombobox()
        {
			cbAddressType.Items.Add("Propietario");
			cbAddressType.Items.Add("Hipotecado");
			cbAddressType.Items.Add("Alquiler");
			cbAddressType.Items.Add("Familiar");
		}

        private void AddAddressInformation(object sender, RoutedEventArgs e)
        {
            try
            {

                if (string.IsNullOrWhiteSpace(txtStreet.Text) || string.IsNullOrWhiteSpace(txtExternalNumber.Text) ||
                    string.IsNullOrWhiteSpace(tbZipCode.Text) || cbColony.SelectedIndex == -1 || cbAddressType.SelectedIndex == -1)
                {
                    MessageBox.Show("Por favor, complete todos los campos de dirección.");
                    return;
                }


                if (!Colony.IsValidZipCode(tbZipCode.Text))
                {
                    MessageBox.Show("Por favor, introduzca un código postal válido.");
                    return;
                }

                var colony = cbColony.SelectedItem as Colony;

                var newCustomerAddressInfoes = new CustomerAddress
                {
                    ExternalNumber = txtExternalNumber.Text,
                    Street = txtStreet.Text,
                    ZipCode = colony.Zipcode.ToString(),
					Colony = colony.Name,
                    CustomerId = customerId,
                    State = colony.State,
                    Municipality = colony.Municipality,
                    Type = cbAddressType.SelectedIndex
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

                App.Current.MainFrame.Navigate(new PageWorkCenter(customerId, IsRegisteringCreditRequest));
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
                        cbAddressType.SelectedIndex = customerData.Type;
                        addressId = customerData.CustomerAddressId;
                        DoNotUpdateZipCode = true;
						tbZipCode.Text = customerData.ZipCode;
						PopulateColoniesComboBox();
                        cbColony.Text = customerData.Colony;
					}
				}
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al intentar obtener los datos de contacto: " + ex.Message);
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

        private async Task<List<string>> GetZipCodes(string keyword)
        {
			return await Task.Run(() =>
			{
				var zipCodes = new List<string>();
			    var res = Colony.GetColonies(keyword, 6);
			    foreach (var colony in res)
			    {
				    zipCodes.Add(colony.Zipcode.ToString());
			    }
                return zipCodes;
			});
		}

        private void PopulateColoniesComboBox()
        {
			var colonies = Colony.GetColoniesByZipcode(tbZipCode.Text);
			cbColony.ItemsSource = colonies.ToList();
			cbColony.SelectedIndex = 0;
		}

		private async void tbZipCode_TextChanged(object sender, RoutedEventArgs e)
		{
            if(DoNotUpdateZipCode)
            {
                DoNotUpdateZipCode = false;
                return;
            }
            var zipCodes = await GetZipCodes(tbZipCode.Text);
			tbZipCode.ItemsSource = zipCodes;
            tbZipCode.IsDropDownOpen = true;

            if(tbZipCode.Text.Length == 5)
            {
                PopulateColoniesComboBox();
			}
		}
    }
}
