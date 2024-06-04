using SGSC.Frames;
using SGSC.Messages;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;

namespace SGSC.Pages
{
    public partial class CreditApplicationDetailsWorkCenter : Page
    {
        private int? requestId;

        public CreditApplicationDetailsWorkCenter(int? requestId)
        {
            InitializeComponent();
            UserSessionFrame.Content = new UserSessionFrame();
            this.requestId = requestId;
            ChangeButtonColor("#F0F6EC");
            
            if (requestId > 0)
            {
                getWorkCenterInformation(requestId);
            }
            else
            {
                ToastNotification notification = new ToastNotification("El ID de la solicitud no está disponible, inténtelo más tarde", "Error");
            }
        }

        public void getWorkCenterInformation(int? requestId)
        {
            using (sgscEntities db = new sgscEntities())
            {
                var workCenterQuery = (from cr in db.CreditRequests
                                       join wc in db.WorkCenters on cr.CustomerId equals wc.Customer.CustomerId
                                       where cr.CreditRequestId == requestId
                                       select new
                                       {
                                           WorkCenterName = wc.CenterName,
                                           PhoneNumber = wc.PhoneNumber,
                                           Street = wc.Street,
                                           Colony = wc.Colony,
                                           InnerNumber = wc.InnerNumber,
                                           OutsideNumber = wc.OutsideNumber,
                                           ZipCode = wc.ZipCode,
                                           FileNumber = cr.FileNumber
                                       }).FirstOrDefault();
                if (workCenterQuery == null)
                {
                    ToastNotification notification = new ToastNotification("No se ha encontrado la solicitud, inténtelo más tarde", "Error");
                    return;
                }

                lbCompanyName.Content = workCenterQuery.WorkCenterName;
                lbPhoneNumber.Content = workCenterQuery.PhoneNumber;
                lbInnerNumber.Content = workCenterQuery.InnerNumber;
                lbOutsideNumber.Content = workCenterQuery.OutsideNumber;
                lbColony.Content = workCenterQuery.Colony;
                lbStreet.Content = workCenterQuery.Street;
                lbZipCode.Content = workCenterQuery.ZipCode;

                lbRequestRequestNumber.Content = workCenterQuery.FileNumber;
            }
        }

        private void btnClicContinue(object sender, RoutedEventArgs e)
        {
            var customerInfoPage = new CreditApplicationDetailsPersonalReferences((int)this.requestId);

            if (NavigationService != null)
            {
                NavigationService.Navigate(customerInfoPage);
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

            btnWorkCenter.Background = brush;
        }

        private void BtnClicPersonalInformation(object sender, RoutedEventArgs e)
        {
            var personalInformationPage = new CreditApplicationDetailsPersonalInformation(requestId);
            if (NavigationService != null)
            {
                NavigationService.Navigate(personalInformationPage);
            }
            else
            {
                ToastNotification notification = new ToastNotification("No se puede realizar la navegación en este momento. Por favor, inténtelo más tarde.", "Error");

            }
        }

        private void BtnClicRequest(object sender, RoutedEventArgs e)
        {
            var requestPage = new CreditApplicationDetailsRequest((int)requestId);
            if (NavigationService != null)
            {
                NavigationService.Navigate(requestPage);
            }
            else
            {
                ToastNotification notification = new ToastNotification("No se puede realizar la navegación en este momento. Por favor, inténtelo más tarde.", "Error");

            }
        }

        private void BtnClicPersonalReferences(object sender, RoutedEventArgs e)
        {
            var personalReferences = new CreditApplicationDetailsPersonalReferences((int)requestId);
            if (NavigationService != null)
            {
                NavigationService.Navigate(personalReferences);
            }
            else
            {
                ToastNotification notification = new ToastNotification("No se puede realizar la navegación en este momento. Por favor, inténtelo más tarde.", "Error");

            }
        }

        private void BtnClicBankAccounts(object sender, RoutedEventArgs e)
        {
            var bankAccounts = new CreditApplicationDetailsBankAccounts(requestId);
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
            var bankAccounts = new CreditApplicationDetailsApproveCreditApplication(requestId);
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
            var documentation = new CreditApplicationDocuments(requestId.Value);
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
