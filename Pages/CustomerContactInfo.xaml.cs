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
    public partial class CustomerContactInfo : Page
    {
        private int customerId;
        private int? contactInfoId = null;
        
        public CustomerContactInfo(int customerId)
        {
            InitializeComponent();
            this.customerId = customerId;

            StepsSidebarFrame.Content = new CustomerRegisterStepsSidebar("ContactInfo");
            UserSessionFrame.Content = new UserSessionFrame();

            clearErrors();
            getContactInfo();

        }

        private void clearErrors()
        {
            lbEmailAddressError.Content = "";
            lbPhoneNumber1Error.Content = "";
            lbPhoneNumber2Error.Content = "";
        }

        private void getContactInfo()
        {
            try
            {
                using (sgscEntities db = new sgscEntities())
                {
                    var customerContactInfo = db.CustomerContactInfoes.Where(ci => ci.CustomerId == customerId).FirstOrDefault();
                    if (customerContactInfo != null)
                    {
                        tbPhoneNumber1.Text = customerContactInfo.PhoneNumber1;
                        tbPhoneNumber2.Text = customerContactInfo.PhoneNumber2;
                        tbEmailAddress.Text = customerContactInfo.Email;
                        contactInfoId = customerContactInfo.CustomerContactInfoId;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener los datos de contacto del cliente: " + ex.Message);
            }
        }

        private void btnSaveContactInfo_Click(object sender, RoutedEventArgs e)
        {
            bool valid = true;

            clearErrors();

            if(!TextValidator.ValidateTextNumeric(tbPhoneNumber1.Text, 10, 10, false))
            {
                valid = false;
                lbPhoneNumber1Error.Content = "Por favor introduzca un número de teléfono válido.";
            }

            if (!TextValidator.ValidateTextNumeric(tbPhoneNumber2.Text, 10, 10, false))
            {
                valid = false;
                lbPhoneNumber2Error.Content = "Por favor introduzca un número de teléfono válido.";
            }

            if(tbPhoneNumber1.Text.Equals(tbPhoneNumber2.Text))
            {
                valid = false;
                lbPhoneNumber2Error.Content = "Los números de teléfono no pueden ser iguales.";
            }

            if (string.IsNullOrEmpty(tbEmailAddress.Text))
            {
                valid = false;
                lbEmailAddressError.Content = "Por favor introduzca una dirección de correo.";
            }

            if (!TextValidator.ValidateEmail(tbEmailAddress.Text))
            {
                valid = false;
                lbEmailAddressError.Content = "Por favor una dirección de correo válida.";
            }

            if(!valid)
            {
                return;
            }

            try
            {
                var contactInfo = new SGSC.CustomerContactInfo
                {
                    PhoneNumber1 = tbPhoneNumber1.Text,
                    PhoneNumber2 = tbPhoneNumber2.Text,
                    Email = tbEmailAddress.Text,
                    CustomerId = customerId
                };

                if(contactInfoId != null)
                {
                    contactInfo.CustomerContactInfoId = contactInfoId.Value;
                }

                using (sgscEntities db = new sgscEntities())
                {
                    db.CustomerContactInfoes.AddOrUpdate(contactInfo);
                    db.SaveChanges();
                }

                App.Current.NotificationsPanel.ShowSuccess("Datos guardados.");

                App.Current.MainFrame.Content = new CustomerReferencesPage(customerId);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar la información de contacto del cliente: " + ex.Message);
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
    }
}
