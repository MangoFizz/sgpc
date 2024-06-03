using SGSC.Frames;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SGSC.Pages
{
    /// <summary>
    /// Lógica de interacción para DatosDeCliente.xaml
    /// </summary>
    public partial class CustomerInfoPage : Page
    {
        private int? CustomerId = null;
        private bool IsRegisteringCreditRequest;


		public CustomerInfoPage(int? customerId = null, bool isRegisteringCreditRequest = false)
        {
            InitializeComponent();
            CustomerId = customerId;
            IsRegisteringCreditRequest = isRegisteringCreditRequest;

			if (CustomerId != null)
            {
                getCustomerInfo(CustomerId.Value);
            }

            StepsSidebarFrame.Content = new CustomerRegisterStepsSidebar("PersonalInfo");
            UserSessionFrame.Content = new UserSessionFrame();

            // Clear the error labels
            lbName.Content = "";
            lbFirstSurname.Content = "";
            lbSecondSurname.Content = "";
            lbCurp.Content = "";
            lbRfcError.Content = "";
            lbGenreError.Content = "";
            lbBirthdateError.Content = "";
            lbCivilStatusError.Content = "";

            PopulateGenres();
            PopulateCivilStatus();
        }

        private void PopulateGenres()
        {
            cbGenre.Items.Add("Masculino");
            cbGenre.Items.Add("Femenino");
            cbGenre.SelectedIndex = 0;
        }

        private void PopulateCivilStatus()
        {
            cbCivilStatus.Items.Add(Customer.GetCivilStatusString(Customer.CivilStatuses.Single));
            cbCivilStatus.Items.Add(Customer.GetCivilStatusString(Customer.CivilStatuses.Married));
            cbCivilStatus.Items.Add(Customer.GetCivilStatusString(Customer.CivilStatuses.Divorced));
            cbCivilStatus.Items.Add(Customer.GetCivilStatusString(Customer.CivilStatuses.Widowed));
            cbCivilStatus.SelectedIndex = 0;
        }

        private void btnContinue_Click(object sender, RoutedEventArgs e)
        {
            lbName.Content = "";
            lbFirstSurname.Content = "";
            lbSecondSurname.Content = "";
            lbCurp.Content = "";
            lbRfcError.Content = "";
            lbGenreError.Content = "";
            lbBirthdateError.Content = "";
            lbCivilStatusError.Content = "";

            bool valid = true;
            if (string.IsNullOrEmpty(tbName.Text))
            {
                valid = false;
                lbName.Content = "Por favor introduzca el nombre";
            }
            if (string.IsNullOrEmpty(tbFirstSurname.Text))
            {
                valid = false;
                lbFirstSurname.Content = "Por favor introduzca el apellido paterno";
            }
            if (string.IsNullOrEmpty(tbSecondSurname.Text))
            {
                valid = false;
                lbSecondSurname.Content = "Por favor introduzca el apellido materno";
            }
            if (string.IsNullOrEmpty(tbCURP.Text))
            {
                valid = false;
                lbCurp.Content = "Por favor introduzca el CURP";
            }
            if (!Utils.TextValidator.ValidateCURP(tbCURP.Text))
            {
                valid = false;
                lbCurp.Content = "Por favor introduzca un CURP válido";
            }
            try
            {
                if (!Utils.TextValidator.ValidateRFC(tbRfc.Text))
                {
                    valid = false;
                    lbRfcError.Content = "Por favor introduzca un RFC válido";
                }
            }
            catch(Exception ex)
            {
                valid = false;
                lbRfcError.Content = "Por favor introduzca un RFC válido";
            }
            if (dpBirthdate.SelectedDate == null)
            {
                valid = false;
                lbBirthdateError.Content = "Por favor introduzca la fecha de nacimiento";
            }
            else
            {
                if (dpBirthdate.SelectedDate.Value.AddYears(18) > DateTime.Now)
                {
                    valid = false;
                    lbBirthdateError.Content = "El cliente debe ser mayor de edad";
                }
            }
            if (!valid)
            {
                return;
            }
            if (CustomerId == null)
            {
                registerCustomer();
            }
            else
            {
                updateCustomer();
            }
        }   

        private void registerCustomer()
        {
            using (sgscEntities db = new sgscEntities())
            {
                try
                {
                    Customer customerToRegister = new Customer();
                    customerToRegister.Curp = tbCURP.Text.ToUpper().Trim();
                    customerToRegister.Name = tbName.Text.Trim();
                    customerToRegister.FirstSurname = tbFirstSurname.Text.Trim();
                    customerToRegister.SecondSurname = tbSecondSurname.Text.Trim();
                    customerToRegister.Rfc = tbRfc.Text.ToUpper().Trim();
                    customerToRegister.BirthDate = dpBirthdate.SelectedDate.Value;
                    customerToRegister.Genre = cbGenre.Text == "Masculino" ? "M" : "F";
                    customerToRegister.CivilStatus = cbCivilStatus.SelectedIndex;

                    customerToRegister = db.Customers.Add(customerToRegister);
					db.SaveChanges();

                    CustomerId = customerToRegister.CustomerId;
                    
                    App.Current.NotificationsPanel.ShowSuccess("Datos guardados");
                    App.Current.MainFrame.Content = new AddressInformationPage(CustomerId.Value, IsRegisteringCreditRequest);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ocurrió un error al intentar registrar el cliente: " + ex.Message);
                    MessageBox.Show("Ocurrió un error al intentar actualizar el cliente.");
                }
            }
        }

        private void updateCustomer()
        {
            using (sgscEntities db = new sgscEntities())
            {
                try
                {
                    Customer customerToUpdate = db.Customers.Find(CustomerId);
                    customerToUpdate.Curp = tbCURP.Text.ToUpper().Trim();
                    customerToUpdate.Name = tbName.Text.Trim();
                    customerToUpdate.FirstSurname = tbFirstSurname.Text.Trim();
                    customerToUpdate.SecondSurname = tbSecondSurname.Text.Trim();
                    customerToUpdate.Rfc = tbRfc.Text.ToUpper().Trim();
                    customerToUpdate.BirthDate = dpBirthdate.SelectedDate.Value;
                    customerToUpdate.Genre = cbGenre.Text == "Masculino" ? "M" : "F";
                    customerToUpdate.CivilStatus = cbCivilStatus.SelectedIndex;

                    db.SaveChanges();
                    
                    App.Current.NotificationsPanel.ShowSuccess("Datos actualizados");
                    App.Current.MainFrame.Navigate(new AddressInformationPage(CustomerId.Value, IsRegisteringCreditRequest));
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ocurrió un error al intentar actualizar el cliente.");
                    Console.WriteLine("Ocurrió un error al intentar actualizar el cliente: " + ex.Message);
                }
            }
        }

        private void tbCURP_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (tbCURP.Text.Length >= 18 || System.Text.RegularExpressions.Regex.IsMatch(tbCURP.Text + e.Text, @"[^a-zA-Z0-9]+$"))
            {
                e.Handled = true;
            }
        }

        private void getCustomerInfo(int idCustomer)
        {
            using (sgscEntities db = new sgscEntities())
            {
                Customer customer = db.Customers.Find(idCustomer);
                if (customer == null)
                {
                    MessageBox.Show("No se encontró la solicitud solicitada, por favor, inténtelo más tarde");
                    App.Current.MainFrame.GoBack();
                    return;
                }
                tbCURP.Text = customer.Curp;
                tbName.Text = customer.Name;
                tbFirstSurname.Text = customer.FirstSurname;
                tbSecondSurname.Text = customer.SecondSurname;
                tbRfc.Text = customer.Rfc;
                dpBirthdate.SelectedDate = customer.BirthDate;
                cbGenre.SelectedIndex = customer.Genre == "M" ? 0 : 1;
                cbCivilStatus.SelectedIndex = customer.CivilStatus;
            }
        }

		private void btnBack_Click(object sender, RoutedEventArgs e)
		{
			App.Current.MainFrame.GoBack();
		}

		private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            App.Current.MainFrame.Content = new HomePageCreditAdvisor();
        }
    }
}
