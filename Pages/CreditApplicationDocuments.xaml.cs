using Org.BouncyCastle.Asn1.Ocsp;
using SGSC.Messages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static SGSC.Pages.CollectionEfficienciesPage;

namespace SGSC.Pages
{
    /// <summary>
    /// Lógica de interacción para CreditApplicationDocuments.xaml
    /// </summary>
    public partial class CreditApplicationDocuments : Page
    {
        private int IdCreditRequest;
        private List<string> selectedDocuments = new List<string>();
        private dynamic selectedDocument;

        public CreditApplicationDocuments(int idCreditRequest)
        {
            InitializeComponent();
            IdCreditRequest = idCreditRequest;
            LoadDocumentNames();
            Console.WriteLine("Valor id: " + IdCreditRequest);
        }

        private void LoadDocumentNames()
        {
            try
            {
                using (sgscEntities db = new sgscEntities())
                {
                    dataGridCreditPolicies.ItemsSource = null; // Establece el origen de datos como nulo
                    var documentNames = db.Documents
                        .Where(d => d.CreditRequestId == IdCreditRequest && d.CreditRequest_CreditRequestId == IdCreditRequest)
                        .Select(d => d.FileName)
                        .ToList();

                    dataGridCreditPolicies.ItemsSource = documentNames; // Asigna el nuevo origen de datos

                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error al cargar los documentos para CreditRequestId = {IdCreditRequest}: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnClicContinue(object sender, RoutedEventArgs e)
        {
            var customerInfoPage = new CreditApplicationDetailsPersonalReferences((int)this.IdCreditRequest);

            if (NavigationService != null)
            {
                NavigationService.Navigate(customerInfoPage);
            }
            else
            {
                ToastNotification notification = new ToastNotification("No se puede realizar la navegación en este momento. Por favor, inténtelo más tarde.", "Error");

            }
        }

        private void btnClicDescargar(object sender, RoutedEventArgs e)
        {
            try
            {
                // Verificar si hay una fila seleccionada
                if (dataGridCreditPolicies.SelectedItem != null)
                {
                    // Obtener el nombre del documento seleccionado
                    string selectedFileName = dataGridCreditPolicies.SelectedItem.ToString();

                    using (sgscEntities db = new sgscEntities())
                    {
                        // Buscar el documento en la base de datos por su nombre
                        var selectedDocument = db.Documents.FirstOrDefault(d => d.FileName == selectedFileName);

                        // Verificar si se encontró el documento
                        if (selectedDocument != null)
                        {
                            // Obtener el nombre y el contenido del archivo
                            string fileName = selectedDocument.FileName;
                            byte[] fileContent = selectedDocument.FileContent;

                            // Verificar que el contenido del archivo no sea nulo o vacío
                            if (fileContent != null && fileContent.Length > 0)
                            {
                                // Mostrar un cuadro de diálogo para guardar el archivo
                                SaveFileDialog saveFileDialog = new SaveFileDialog();
                                saveFileDialog.FileName = fileName;

                                // Mostrar el cuadro de diálogo y guardar el archivo si se selecciona un destino
                                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                                {
                                    string filePath = saveFileDialog.FileName;

                                    // Guardar el contenido del archivo en la ubicación especificada
                                    File.WriteAllBytes(filePath, fileContent);

                                    // Mostrar un mensaje de éxito
                                    System.Windows.MessageBox.Show($"El archivo '{fileName}' se ha descargado exitosamente.", "Descarga Exitosa", MessageBoxButton.OK, MessageBoxImage.Information);
                                }
                            }
                            else
                            {
                                System.Windows.MessageBox.Show($"El contenido del archivo '{fileName}' está vacío.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                        else
                        {
                            System.Windows.MessageBox.Show("La fila seleccionada no contiene un documento válido.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                else
                {
                    System.Windows.MessageBox.Show("Por favor, seleccione una fila para descargar el documento.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error al descargar el documento: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }







        private void btnEliminar(object sender, RoutedEventArgs e)
        {
            try
            {
                // Verificar si hay una fila seleccionada
                if (dataGridCreditPolicies.SelectedItem != null)
                {
                    // Obtener el nombre del documento seleccionado
                    string selectedFileName = dataGridCreditPolicies.SelectedItem.ToString();

                    using (sgscEntities db = new sgscEntities())
                    {
                        // Buscar el documento en la base de datos por su nombre
                        var selectedDocument = db.Documents.FirstOrDefault(d => d.FileName == selectedFileName);

                        // Verificar si se encontró el documento
                        if (selectedDocument != null)
                        {
                            // Eliminar el documento de la base de datos
                            db.Documents.Remove(selectedDocument);
                            db.SaveChanges();

                            // Eliminar la fila del DataGrid
                            var documentNames = dataGridCreditPolicies.ItemsSource.Cast<string>().ToList();
                            documentNames.Remove(selectedFileName);
                            dataGridCreditPolicies.ItemsSource = documentNames;

                            // Mostrar un mensaje de éxito
                            System.Windows.MessageBox.Show($"El archivo '{selectedFileName}' se ha eliminado exitosamente.", "Eliminación Exitosa", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            System.Windows.MessageBox.Show("La fila seleccionada no contiene un documento válido.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                else
                {
                    System.Windows.MessageBox.Show("Por favor, seleccione una fila para eliminar el documento.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error al eliminar el documento: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void dataGridCreditPolicies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           
        }

        private void BtnClicPersonalInformation(object sender, RoutedEventArgs e)
        {
            try
            {
                var personalInformationPage = new CreditApplicationDetailsPersonalInformation(IdCreditRequest);
                if (NavigationService != null)
                {
                    NavigationService.Navigate(personalInformationPage);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error al navegar a la información personal: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnClicWorkCenter(object sender, RoutedEventArgs e)
        {
            try
            {
                var workCenterPage = new CreditApplicationDetailsWorkCenter(IdCreditRequest);
                if (NavigationService != null)
                {
                    NavigationService.Navigate(workCenterPage);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error al navegar al centro de trabajo: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnClicPersonalReferences(object sender, RoutedEventArgs e)
        {
            try
            {
                var personalReferences = new CreditApplicationDetailsPersonalReferences((int)IdCreditRequest);
                if (NavigationService != null)
                {
                    NavigationService.Navigate(personalReferences);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error al navegar a las referencias personales: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnClicRequest(object sender, RoutedEventArgs e)
        {
            try
            {
                var requestPage = new CreditApplicationDetailsRequest((int)IdCreditRequest);
                if (NavigationService != null)
                {
                    NavigationService.Navigate(requestPage);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error al navegar a la solicitud: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnClicBankAccounts(object sender, RoutedEventArgs e)
        {
            try
            {
                var bankAccountstPage = new CreditApplicationDetailsBankAccounts(IdCreditRequest);
                if (NavigationService != null)
                {
                    NavigationService.Navigate(bankAccountstPage);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error al navegar a las cuentas bancarias: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            App.Current.MainFrame.Content = new HomePageCreditAnalyst();
        }


    }
}
