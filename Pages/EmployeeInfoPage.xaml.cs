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
    public partial class EmployeeInfoPage : Page
    {
        private int? EmployeeId = null;

        public EmployeeInfoPage(int? employeeId = null)
        {
            InitializeComponent();
            PopulateRoles();

            EmployeeId = employeeId;
            if (EmployeeId != null)
            {
                GetEmployeeData(EmployeeId.Value);
            }

            creditAdvisorSidebar.Content = new Frames.AdminSidebar("searchCustomer");
            UserSessionFrame.Content = new UserSessionFrame();

            // Clear the error labels
            lbName.Content = "";
            lbFirstSurname.Content = "";
            lbSecondSurname.Content = "";
            lbEmailError.Content = "";
            lbRoleError.Content = "";
            lbPasswordError.Content = "";
            lbConfirmPasswordError.Content = "";

            if(EmployeeId != null)
            {
                lbPasswordField.Content += "   ";
                lbConfirmPasswordField.Content += "   ";
            }
        }

        private void PopulateRoles()
        {
            cbRole.Items.Add(Employee.GetRoleName((short)Employee.EmployeeRoles.Admin));
            cbRole.Items.Add(Employee.GetRoleName((short)Employee.EmployeeRoles.CreditAdvisor));
            cbRole.Items.Add(Employee.GetRoleName((short)Employee.EmployeeRoles.CreditAnalyst));
            cbRole.Items.Add(Employee.GetRoleName((short)Employee.EmployeeRoles.CollectionExecutive));
            cbRole.SelectedIndex = 0;
        }

        private void btnContinue_Click(object sender, RoutedEventArgs e)
        {
            lbName.Content = "";
            lbFirstSurname.Content = "";
            lbSecondSurname.Content = "";
            lbEmailError.Content = "";
            lbRoleError.Content = "";
            lbPasswordError.Content = "";
            lbConfirmPasswordError.Content = "";

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
            if (!Utils.TextValidator.ValidateEmail(tbEmail.Text))
            {
                valid = false;
                lbEmailError.Content = "Por favor un correo electrónico válido";
            }

            if(EmployeeId == null || pbPassword.Password.Length > 0)
            {
                if (!Utils.TextValidator.CheckPasswordStrength(pbPassword.Password))
                {
                    if (valid)
                    {
                        MessageBox.Show("La contraseña debe tener al menos 8 caracteres, una letra mayúscula, una letra minúscula, un número y un carácter especial");
                    }
                    valid = false;
                    lbPasswordError.Content = "Por favor introduzca una contraseña que cumpla los criterios.";
                }

                if (pbPassword.Password != pbPasswordConfirmation.Password)
                {
                    valid = false;
                    lbConfirmPasswordError.Content = "Las contraseñas no coinciden";
                }
            }

            if (!valid)
            {
                return;
            }
            
            using (sgscEntities db = new sgscEntities())
            {
				Employee employee;
                if (EmployeeId == null)
                {
                    employee = new Employee();
                    db.Employees.Add(employee);
                }
                else
                {
                    employee = db.Employees.Find(EmployeeId);
                }
                if (EmployeeId == null || pbPassword.Password.Length > 0)
                {
                    employee.Password = Utils.Authenticator.HashPassword(pbPassword.Password);
                }
                employee.Name = tbName.Text;
                employee.FirstSurname = tbFirstSurname.Text;
                employee.SecondSurname = tbSecondSurname.Text;
                employee.Email = tbEmail.Text;
                employee.Role = (short)cbRole.SelectedIndex;
                db.SaveChanges();
            }

            MessageBox.Show("Datos guardados correctamente");
            App.Current.MainFrame.Content = new ManageEmployeesPage();
        }   

        private void GetEmployeeData(int employeeId)
        {
            using (sgscEntities db = new sgscEntities())
            {
                var employee = db.Employees.Find(employeeId);
                if (employee == null)
                {
                    MessageBox.Show("No se encontró el cliente seleccionado");
                    App.Current.MainFrame.GoBack();
                    return;
                }
                tbEmail.Text = employee.Email;
                tbName.Text = employee.Name;
                tbFirstSurname.Text = employee.FirstSurname;
                tbSecondSurname.Text = employee.SecondSurname;
                cbRole.SelectedIndex = employee.Role;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            App.Current.MainFrame.Content = new ManageEmployeesPage();
        }
    }
}
