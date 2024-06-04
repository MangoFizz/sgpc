using SGSC.Frames;
using SGSC.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for SearchClientPage.xaml
    /// </summary>
    public partial class ManageEmployeesPage : Page
    {
        private class EmployeeEntry
        {
            public int EmployeeId { get; set; }
            public string Fullname { get; set; }
            public string Email { get; set; }
            public string Role { get; set; }
        }

        private ObservableCollection<EmployeeEntry> Employees;
        private int CurrentPage = 1;
        private int TotalPages = 1;
        private const int ItemsPerPage = 10;
        private bool UpdatingPagination = false;

        public ManageEmployeesPage()
        {
            InitializeComponent();
            creditAdvisorSidebar.Content = new Frames.AdminSidebar("searchCustomer");
            GetCustomers();
        }

        private void UpdatePagination()
        {
            UpdatingPagination = true;

            try
            {
                using (var context = new sgscEntities())
                {
                    var employeesCount = context.Employees.Where(employee => employee.Email.Contains(tbEmail.Text) && (employee.Name + " " + employee.FirstSurname + " " + employee.SecondSurname).Contains(tbName.Text)).Count();
                    TotalPages = (int)Math.Ceiling((double)employeesCount / ItemsPerPage);
                    lbCurrentPage.Content = $"Página {CurrentPage}/{TotalPages}";
                    cbPages.Items.Clear();
                    for (uint i = 1; i <= TotalPages; i++)
                    {
                        cbPages.Items.Add(i);
                    }
                    cbPages.SelectedIndex = CurrentPage - 1;

                    btnNextPage.IsEnabled = CurrentPage < TotalPages;
                    btnPreviousPage.IsEnabled = CurrentPage > 1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al intentar actualizar la paginación de la tabla de empleados: " + ex.Message);
            }

            UpdatingPagination = false;

        }

        private void GetCustomers()
        {
            try
            {
                using (var context = new sgscEntities())
                {
                    var employees = context.Employees.Where(employee => employee.Email.Contains(tbEmail.Text) && (employee.Name + " " + employee.FirstSurname + " " + employee.SecondSurname).Contains(tbName.Text));
                    var employeesList = employees.ToList();
                    Employees = new ObservableCollection<EmployeeEntry>();
                    foreach (var item in employeesList)
                    {
                        Employees.Add(new EmployeeEntry
                        {
                            EmployeeId = item.EmployeeId,
                            //Fullname = item.FullName,
                            Email = item.Email,
                            Role = Employee.GetRoleName(item.Role)
                        });
                    }
                    dgEmployees.ItemsSource = Employees;
                }
                UpdatePagination();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al intentar obtener los datos de los empleados: " + ex.Message);
            }
        }

        private void tbFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            GetCustomers();
        }

        private void btnNextPage_Click(object sender, RoutedEventArgs e)
        {
            CurrentPage++;
            GetCustomers();
        }

        private void btnPreviousPage_Click(object sender, RoutedEventArgs e)
        {
            CurrentPage--;
            GetCustomers();
        }

        private void cbPages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (UpdatingPagination)
            {
                return;
            }
            CurrentPage = cbPages.SelectedIndex + 1;
            GetCustomers();
        }

        private void btnRegisterCustomer_Click(object sender, RoutedEventArgs e)
        {
            var customerInfoPage = new CustomerInfoPage();
            if (NavigationService != null)
            {
                NavigationService.Navigate(customerInfoPage);
            }
        }

        private void btnModifyEmployee_Click(object sender, RoutedEventArgs e)
        {
            var employee = dgEmployees.SelectedItem as EmployeeEntry;
            if(employee != null)
            {
                var employeeInfoPage = new EmployeeInfoPage(employee.EmployeeId);
                if (NavigationService != null)
                {
                    NavigationService.Navigate(employeeInfoPage);
                }
            }
            else
            {
                MessageBox.Show("Por favor, seleccione un empleado de la tabla.");
            }
        }

        private void btnDeleteEmployee_Click(object sender, RoutedEventArgs e)
        {
            var employee = dgEmployees.SelectedItem as EmployeeEntry;
            if(employee != null)
            {
                var result = System.Windows.Forms.MessageBox.Show("¿Está seguro que desea eliminar el empleado seleccionado?", "Eliminar empleado", System.Windows.Forms.MessageBoxButtons.YesNo);
                if(result == System.Windows.Forms.DialogResult.Yes)
                {
                    try
                    {
                        using (var context = new sgscEntities())
                        {
                            var employeeToDelete = context.Employees.Find(employee.EmployeeId);
                            context.Employees.Remove(employeeToDelete);
                            context.SaveChanges();
                            GetCustomers();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al intentar eliminar el empleado seleccionado: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Por favor, seleccione un empleado de la tabla.");
            }
        }

        private void btnRegisterEmployee_Click(object sender, RoutedEventArgs e)
        {
            var employeeInfoPage = new EmployeeInfoPage();
            if (NavigationService != null)
            {
                NavigationService.Navigate(employeeInfoPage);
            }
        }
    }
}
