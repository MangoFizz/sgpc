using SGSC.Frames;
using SGSC.Messages;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;

namespace SGSC.Pages
{
    public partial class CreditApplicationDetailsRequest : Page
    {
        private int? requestId = null;
        
        public CreditApplicationDetailsRequest(int requestId)
        {
            InitializeComponent();
            UserSessionFrame.Content = new UserSessionFrame();
            this.requestId = requestId;

            ChangeButtonColor("#F0F6EC");

            if (requestId > 0)
            {
                getRequestInfo(requestId);
            }
            else
            {
                ToastNotification notification = new ToastNotification("El ID de la solicitud no está disponible, inténtelo más tarde", "Error");
            }
        }

        private void getRequestInfo(int RequestId)
        {
            using (sgscEntities db = new sgscEntities())
            {
                var requestInfo = (from request in db.CreditRequests
                                   join employee in db.Employees on request.EmployeeId equals employee.EmployeeId
                                   where request.CreditRequestId == RequestId
                                   select new
                                   {
                                       VendorName = employee.Name + " " + employee.FirstSurname + " " + employee.SecondSurname,
                                       RequestNumber = request.FileNumber,
                                       CreationDate = request.CreationDate,
                                       TimePeriod = request.TimePeriod,
                                       RequestedAmountNumber = request.Amount,
                                       Period = request.TimePeriod,
                                       InterestRate = request.InterestRate,
                                       Purpose = request.Purpose,
                                   }).FirstOrDefault();

                if (requestInfo == null)
                {
                   ToastNotification notification = new ToastNotification("No se ha encontrado la solicitud, inténtelo más tarde", "Error");
                    return;
                }

                lbSeller.Content = requestInfo.VendorName;
                lbRequestRequestNumber.Content = requestInfo.RequestNumber;
                lbCreationDate.Content = requestInfo.CreationDate.ToString();
                lbAmount.Content = " " + requestInfo.RequestedAmountNumber.Value.ToString("0.00") + " (" + Utils.Utils.NumberALetter(requestInfo.RequestedAmountNumber) + ")";
                lbInterestRate.Content = requestInfo.InterestRate.ToString();
                lbDestination.Content = requestInfo.Purpose;
                lbDeadline.Content = requestInfo.TimePeriod.ToString() + " Quincenas";
            }
        }

        private void BtnClicContinue(object sender, RoutedEventArgs e)
        {
            var customerInfoPage = new CreditApplicationDetailsPersonalInformation(requestId);

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

            btnRequest.Background = brush;

            if (color.R * 0.299 + color.G * 0.587 + color.B * 0.114 > 186)
            {
                btnRequest.Foreground = System.Windows.Media.Brushes.Black; 
            }
            else
            {
                btnRequest.Foreground = System.Windows.Media.Brushes.White;
            }
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

        private void BtnClicWorkCenter(object sender, RoutedEventArgs e)
        {
            var workCenterPage = new CreditApplicationDetailsWorkCenter(requestId);
            if (NavigationService != null)
            {
                NavigationService.Navigate(workCenterPage);
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
            } else
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
            } else
            {
                ToastNotification notification = new ToastNotification("No se puede realizar la navegación en este momento. Por favor, inténtelo más tarde.", "Error");

            }
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

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            App.Current.MainFrame.Content = new HomePageCreditAnalyst();
        }

        private void Frame_Navigated(object sender, NavigationEventArgs e)
        {

        }
	}
}
