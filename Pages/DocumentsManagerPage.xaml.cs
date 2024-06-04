using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
using Microsoft.Win32;
using SGSC.Utils;

namespace SGSC.Pages
{
    /// <summary>
    /// Interaction logic for DocumentsManagerPage.xaml
    /// </summary>
    public partial class DocumentsManagerPage : Page
    {


        private sgscEntities _context;
        private ObservableCollection<Document> documentsDataAux;
        private int? creditRequestId;

        public DocumentsManagerPage(int? creditRequestId)
        {
            InitializeComponent();
            _context = new sgscEntities();
            this.creditRequestId = creditRequestId;
            LoadDocuments(creditRequestId);
        }

        private void LoadDocuments(int? creditRequestId)
        {
            try
            {
                using (sgscEntities db = new sgscEntities())
                {
                    var documents = db.Documents.ToList();
                    documentsDataAux = new ObservableCollection<Document>(documents);
                    foreach (var document in documents)
                    {
                        CreateDocumentCard(document.FileName, document.DocumentId, creditRequestId);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió un error al intentar obtener los documentos. Por favor, inténtelo de nuevo más tarde.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void UploadButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                string fileName = System.IO.Path.GetFileName(openFileDialog.FileName);
                byte[] fileContent = File.ReadAllBytes(openFileDialog.FileName);

                try
                {
                    Document document = new Document
                    {
                        FileName = fileName,
                        FileContent = fileContent,
                        CreditRequestId = creditRequestId.Value 
                    };

                    _context.Documents.Add(document);
                    _context.SaveChanges();

                    documentsDataAux.Add(document);
                    CreateDocumentCard(document.FileName, document.DocumentId, document.CreditRequestId);

                    MessageBox.Show("Documento subido con éxito.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ocurrió un error al intentar subir el documento. Por favor, inténtelo de nuevo más tarde.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void CreateDocumentCard(string fileName, int documentId, int? creditRequestId)
        {
            StackPanel cardPanel = new StackPanel
            {
                Margin = new Thickness(10),
                Background = new SolidColorBrush(Color.FromRgb(240, 240, 240)),
                Width = 200,
                Height = 100
            };

            Label nameLabel = new Label
            {
                Content = fileName,
                Margin = new Thickness(5)
            };

            Button downloadButton = new Button
            {
                Content = "Descargar",
                Tag = documentId,
                Margin = new Thickness(5)
            };
            downloadButton.Click += DownloadButton_Click;
            downloadButton.DataContext = creditRequestId; 

            cardPanel.Children.Add(nameLabel);
            cardPanel.Children.Add(downloadButton);

            DocumentsPanel.Children.Add(cardPanel);
        }

        private void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int documentId)
            {
                var document = _context.Documents.FirstOrDefault(d => d.DocumentId == documentId);
                if (document != null)
                {
                    SaveFileDialog saveFileDialog = new SaveFileDialog
                    {
                        FileName = document.FileName
                    };

                    if (saveFileDialog.ShowDialog() == true)
                    {
                        File.WriteAllBytes(saveFileDialog.FileName, document.FileContent);
                        MessageBox.Show("Documento descargado con éxito.");

                        
                        int? creditRequestId = (int?)((Button)sender).DataContext;
                        
                    }
                }
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            UserSession.LogOut();
        }

        private void HomePageCreditAdvisorMenu(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new HomePageCreditAdvisor());
        }
    }
}
