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
    public partial class SearchCustomerPage : Page
    {
        private class CustomerEntry
        {
            public int CustomerId { get; set; }
            public string Fullname { get; set; }
            public string Genre { get; set; }
            public string Birthdate { get; set; }
            public string RFC { get; set; }
        }

        private ObservableCollection<CustomerEntry> Customers;
        private int CurrentPage = 1;
        private int TotalPages = 1;
        private const int ItemsPerPage = 10;
        private bool UpdatingPagination = false;

        public SearchCustomerPage()
        {
            InitializeComponent();
            creditAdvisorSidebar.Content = new Frames.CreditAdvisorSidebar("searchCustomer");
            GetCustomers();
        }

        private void UpdatePagination()
        {
            UpdatingPagination = true;

            try
            {
                using (var context = new sgscEntities())
                {
                    var customersCount = context.Customers.Where(customer => customer.Rfc.Contains(tbRfc.Text) && (customer.Name + " " + customer.FirstSurname + " " + customer.SecondSurname).Contains(tbName.Text)).Count();
                    TotalPages = (int)Math.Ceiling((double)customersCount / ItemsPerPage);
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
                MessageBox.Show("Error al intentar actualizar la paginación de la tabla de clientes: " + ex.Message);
            }

            UpdatingPagination = false;

        }

        private void GetCustomers()
        {
            try
            {
                using (var context = new sgscEntities())
                {
                    var customers = context.Customers.Where(customer => customer.Rfc.Contains(tbRfc.Text) && (customer.Name + " " + customer.FirstSurname + " " + customer.SecondSurname).Contains(tbName.Text)).OrderBy(customer => customer.Curp).Skip((CurrentPage - 1) * ItemsPerPage).Take(ItemsPerPage);
                    var customersList = customers.ToList();
                    Customers = new ObservableCollection<CustomerEntry>();
                    foreach (var item in customersList)
                    {
                        Customers.Add(new CustomerEntry
                        {
                            CustomerId = item.CustomerId,
                            //Fullname = item.FullName,
                            Genre = item.Genre == "M" ? "Masculino" : "Femenino",
                            Birthdate = item.BirthDate.ToString("dd/MM/yyyy"),
                            RFC = item.Rfc
                        });
                    }
                    dgCustomers.ItemsSource = Customers;
                }
                UpdatePagination();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al intentar obtener los datos de los clientes: " + ex.Message);
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

        private void btnSelectCustomer_Click(object sender, RoutedEventArgs e)
        {
            var customer = dgCustomers.SelectedItem as CustomerEntry;
            if(customer != null)
            {
                var customerInfoPage = new CustomerInfoPage(customer.CustomerId);
                if (NavigationService != null)
                {
                    NavigationService.Navigate(customerInfoPage);
                }
            }
            else
            {
                MessageBox.Show("Por favor, seleccione un cliente de la tabla.");
            }
        }
    }
}
