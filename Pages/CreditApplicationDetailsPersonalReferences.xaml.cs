using Org.BouncyCastle.Asn1.Ocsp;
using SGSC.Frames;
using SGSC.Messages;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;

namespace SGSC.Pages
{
    public partial class CreditApplicationDetailsPersonalReferences : Page
    {
        private int RequestId;
        public CreditApplicationDetailsPersonalReferences(int requestId)
        {
            InitializeComponent();
            RequestId = requestId;
            ChangeButtonColor("#F0F6EC");
            UserSessionFrame.Content = new UserSessionFrame();
            
            if (requestId > 0)
            {
                ObtainPersonalReferences();
            }
            else
            {
                ToastNotification notification = new ToastNotification("El ID de la solicitud no está disponible, inténtelo más tarde", "Error");
            }
        }

        public void ObtainPersonalReferences()
        {
            using (sgscEntities db = new sgscEntities())
            {
                var customerId = db.CreditRequests
                                    .Where(cr => cr.CreditRequestId == RequestId)
                                    .Select(cr => cr.CustomerId)
                                    .FirstOrDefault();

                if (customerId == 0) // Supongamos que 0 indica que no se encontró ningún cliente
                {
                    ToastNotification notification = new ToastNotification("No se ha encontrado la solicitud, inténtelo más tarde", "Error");
                    return;
                }

                var contacts = (from c in db.Contacts
                                where c.CustomerId == customerId
                                select c)
                                .Take(2) // Tomar solo las dos primeras referencias
                                .ToList();

                if (contacts.Any())
                {
                    lbNameReferenceOne.Content = contacts.ElementAtOrDefault(0)?.Name;
                    lbRelationshipReferenceOne.Content = contacts.ElementAtOrDefault(0)?.Relationship;
                    lbPhoneReferenceOne.Content = contacts.ElementAtOrDefault(0)?.PhoneNumber;

                    if (contacts.Count > 1) // Verificar si hay al menos dos referencias
                    {
                        lbNameReferenceTwo.Content = contacts.ElementAtOrDefault(1)?.Name;
                        lbRelationshipReferenceTwo.Content = contacts.ElementAtOrDefault(1)?.Relationship;
                        lbPhoneReferenceTwo.Content = contacts.ElementAtOrDefault(1)?.PhoneNumber;
                    }
                    else
                    {
                        ToastNotification notification = new ToastNotification("Solo se encontró una referencia personal para este cliente", "Error");

                    }
                }
                else
                {
                    ToastNotification notification = new ToastNotification("No se encontraron referencias personales para este cliente.", "Error");
                }
            }
        }




        private void BtnClickContinue(object sender, RoutedEventArgs e)
        {
            var pageBankAccounts = new CreditApplicationDetailsBankAccounts(RequestId);

            if (NavigationService != null)
            {
                NavigationService.Navigate(pageBankAccounts);
            }
            else
            {
                ToastNotification notification = new ToastNotification("No se puede realizar la navegación en este momento. Por favor, inténtelo más tarde.", "Error");
            }
        }

        private void ChangeButtonColor(string hexColor)
        {
            System.Windows.Media.Color color = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(hexColor);

            SolidColorBrush brush = new SolidColorBrush(color);

            btnPersonalReferences.Background = brush;
        }

        private void BtnClicPersonalInformation(object sender, RoutedEventArgs e)
        {
            var personalInformationPage = new CreditApplicationDetailsPersonalInformation(RequestId);
            if (NavigationService != null)
            {
                NavigationService.Navigate(personalInformationPage);
            }
            else
            {
                ToastNotification notification = new ToastNotification("No se puede realizar la navegación en este momento. Por favor, inténtelo más tarde.", "Error");
            }
        }

        private void BtnClicWorkCenter(object sender, RoutedEventArgs e)
        {
            var workCenterPage = new CreditApplicationDetailsWorkCenter(RequestId);
            if (NavigationService != null)
            {
                NavigationService.Navigate(workCenterPage);
            }
            else
            {
                ToastNotification notification = new ToastNotification("No se puede realizar la navegación en este momento. Por favor, inténtelo más tarde.", "Error");
            }
        }

        private void BtnClicRequest(object sender, RoutedEventArgs e)
        {
            var requestPage = new CreditApplicationDetailsRequest((int)RequestId);
            if (NavigationService != null)
            {
                NavigationService.Navigate(requestPage);
            }
            else
            {
                ToastNotification notification = new ToastNotification("No se puede realizar la navegación en este momento. Por favor, inténtelo más tarde.", "Error");
            }
        }

        private void BtnClicBankAccounts(object sender, RoutedEventArgs e)
        {
            var bankAccounts = new CreditApplicationDetailsBankAccounts(RequestId);
            if (NavigationService != null)
            {
                NavigationService.Navigate(bankAccounts);
            }
            else
            {
                ToastNotification notification = new ToastNotification("No se puede realizar la navegación en este momento. Por favor, inténtelo más tarde.", "Error");
            }
        }

        private void BtnClicAproveRequest(object sender, RoutedEventArgs e)
        {
            var bankAccounts = new CreditApplicationDetailsApproveCreditApplication(RequestId);
            if (NavigationService != null)
            {
                NavigationService.Navigate(bankAccounts);
            }
            else
            {
                ToastNotification notification = new ToastNotification("No se puede realizar la navegación en este momento. Por favor, inténtelo más tarde.", "Error");
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            App.Current.MainFrame.Content = new HomePageCreditAnalyst();
        }

        private void BtnDocumentation(object sender, RoutedEventArgs e)
        {
            var documentation = new CreditApplicationDocuments(RequestId);
            if (NavigationService != null)
            {
                NavigationService.Navigate(documentation);
            }
            else
            {
                ToastNotification notification = new ToastNotification("No se puede realizar la navegación en este momento. Por favor, inténtelo más tarde.", "Error");

            }
        }
	}
}

