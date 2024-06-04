using Org.BouncyCastle.Asn1.Ocsp;
using SGSC.Frames;
using SGSC.Messages;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Contexts;
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
    public partial class CreditApplicationDetailsBankAccounts : Page
    {
        private int? requestId;
        public CreditApplicationDetailsBankAccounts(int? requestId)
        {
            InitializeComponent();
            ChangeButtonColor("#F0F6EC");
            UserSessionFrame.Content = new UserSessionFrame();
            this.requestId = requestId;

            if (requestId > 0)
            {
                getBankAccounts(requestId.Value);
            }
            else
            {
                ToastNotification notification = new ToastNotification("El ID de la solicitud no está disponible, inténtelo más tarde", "Error");
            }
        }
        
        private void getBankAccounts(int requestId)
        {
            using (sgscEntities db = new sgscEntities())
            {
                // Obtener cuenta de transferencia
                /*var transferenciaAccount = (from ba in db.BankAccounts
                                            join cr in db.CreditRequests on ba.BankAccountId equals cr.TransferBankAccount.BankAccountId
                                            join b in db.Banks on ba.BankBankId equals b.BankId
                                            where cr.CreditRequestId == requestId
                                            select new
                                            {
                                                BankName = b.Name,
                                                ba.InterbankCode,
                                                ba.CardNumber
                                            }).FirstOrDefault();*/

                /*if (transferenciaAccount == null)
                {
                    ToastNotification notification = new ToastNotification("No se encontró la solicitud especificada o no hay cuenta de transferencia asociada.", "Error");
                }
                else
                {
                    lbNameBankAccountTransfer.Content = transferenciaAccount.BankName;
                    lbClabeAccountTransfer.Content = transferenciaAccount.InterbankCode;
                    lbTargetNumberAccountTransfer.Content = transferenciaAccount.CardNumber;
                }
                
                var domicializationAccount = (from ba in db.BankAccounts
                                              join cr in db.CreditRequests on ba.BankAccountId equals cr.DirectDebitBankAccounts.BankAccountId
                                              join b in db.Banks on ba.BankBankId
                                              equals b.BankId
                                              where cr.CreditRequestId == requestId
                                              select new
                                              {
                                                  BankName = b.Name,
                                                  ba.InterbankCode,
                                                  ba.CardNumber
                                              }).FirstOrDefault();

                if (domicializationAccount == null)
                {
                    ToastNotification notification = new ToastNotification("No se encontró la solicitud especificada o no hay cuenta de domiciliación asociada.", "Error");
                }
                else
                {
                    lbNameBankAccountDomicialization.Content = domicializationAccount.BankName;
                    lbClabeAccountDomicialization.Content = domicializationAccount.InterbankCode;
                    lbTargetNumberAccountDomicialization.Content = domicializationAccount.CardNumber;
                } */
            }
        }

        private void BtnClicContinue(object sender, RoutedEventArgs e)
        {
            var creditAprovePage = new CreditApplicationDetailsApproveCreditApplication(requestId);

            if (NavigationService != null)
            {
                NavigationService.Navigate(creditAprovePage);
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

            btnBankAccounts.Background = brush;
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

        private void BtnClicAproveRequest(object sender, RoutedEventArgs e)
        {
            var aproveRequestPage = new CreditApplicationDetailsApproveCreditApplication(requestId);
            if (NavigationService != null)
            {
                NavigationService.Navigate(aproveRequestPage);
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
