using SGSC.Frames;
using SGSC.Messages;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;

namespace SGSC.Pages
{
    public partial class CreditApplicationDetailsPersonalInformation : Page
    {
        private int? requestId;

        public CreditApplicationDetailsPersonalInformation(int? requestId)
        {
            InitializeComponent();
            UserSessionFrame.Content = new UserSessionFrame();
            this.requestId = requestId;
            ChangeButtonColor("#F0F6EC");

            if (requestId > 0)
            {
                getPersonalInformation();
            }
            else
            {
                ToastNotification notification = new ToastNotification("El ID de la solicitud no está disponible, inténtelo más tarde", "Error");
            }
        }

        public void getPersonalInformation()
        {
            using (sgscEntities db = new sgscEntities())
            {
                var creditRequest = (from cr in db.CreditRequests
                                     where cr.CreditRequestId == requestId
                                     join cus in db.Customers on cr.CustomerId equals cus.CustomerId
                                     join cc in db.CustomerContactInfoes on cus.CustomerId equals cc.CustomerId
                                     join ca in db.CustomerAddresses on cus.CustomerId equals ca.CustomerId
                                     select new
                                     {
                                         Name = cus.Name,
                                         FirstSurname = cus.FirstSurname,
                                         SecondSurname = cus.SecondSurname,
                                         DateOfBirthay = cus.BirthDate,
                                         Gender = cus.Genre,
                                         Curp = cus.Curp,
                                         Email = cc.Email,
                                         MaritalStatus = cus.CivilStatus,
                                         PhoneOne = cc.PhoneNumber1,
                                         PhoneTwo = cc.PhoneNumber2,
                                         FileNumber = cr.FileNumber,
                                         Addresses = (from addressCustomer in db.CustomerAddresses
                                                      where addressCustomer.CustomerId == cus.CustomerId
                                                      select new
                                                      {
                                                          addressCustomer.Street,
                                                          addressCustomer.ZipCode,
                                                          addressCustomer.ExternalNumber,
                                                          addressCustomer.InternalNumber,
                                                          addressCustomer.Colony,
                                                          addressCustomer.State,
                                                          addressCustomer.Type
                                                      }).ToList()
                                     }).FirstOrDefault();

                if (creditRequest == null)
                {
                    ToastNotification notification = new ToastNotification("No se ha encontrado la solicitud, inténtelo más tarde", "Error");
                    return;
                }

                lbName.Content = creditRequest.Name;
                lbFirstSurname.Content = creditRequest.FirstSurname;
                lbSecondSurname.Content = creditRequest.SecondSurname;
                lbDateOfBirth.Content = creditRequest.DateOfBirthay;
                CheckCheckboxGender(creditRequest.Gender);
                lbCurp.Content = creditRequest.Curp;
                lbEmail.Content = creditRequest.Email;

                var address = creditRequest.Addresses.FirstOrDefault();
                if (address != null)
                {
                    lbStreet.Content = address.Street;
                    lbOutsideNumber.Content = address.ExternalNumber;
                    lbInnerNumber.Content = address.InternalNumber;
                    lbState.Content = address.State;
                    lbColony.Content = address.Colony;
                    lbCp.Content = address.ZipCode;
                    CheckCheckboxTypeAddress(address.Type);
                }

                lbPhoneOne.Content = creditRequest.PhoneOne;
                lbPhoneTwo.Content = creditRequest.PhoneTwo;
                CheckCheckboxMaritalStatus((Customer.CivilStatuses)creditRequest.MaritalStatus);

                lbRequestRequestNumber.Content = creditRequest.FileNumber;
            }
        }


        public void CheckCheckboxGender(String gender)
        {
            switch (gender.ToLower())
            {
                case "femenino":
                    cbxWoman.IsChecked = true;
                    break;
                case "masculino":
                    cbxMan.IsChecked = true;
                    break;
            }
        }

        public void CheckCheckboxMaritalStatus(Customer.CivilStatuses maritalStatus)
        {
            switch (maritalStatus)
            {
                case Customer.CivilStatuses.Single:
                    cbxSingle.IsChecked = true;
                    break;
                case Customer.CivilStatuses.Married:
                    cbxMarried.IsChecked = true;
                    break;
                case Customer.CivilStatuses.Divorced:
                    cbxDivorced.IsChecked = true;
                    break;
                case Customer.CivilStatuses.Widowed:
                    cbxMortgaged.IsChecked = true;
                    break;
                case Customer.CivilStatuses.Concubinage:
                    cbxFreeUnion.IsChecked = true;
                    break;
            }
        }

        public void CheckCheckboxTypeAddress(int typeAddress)
        {
            Utils.AddressCustomer.GetTypeAddress(typeAddress);

            switch (Utils.AddressCustomer.GetTypeAddress(typeAddress))
            {
                case "Propietario":
                    cbxOwner.IsChecked = true;
                    break;
                case "Hipotecado":
                    cbxMortgaged.IsChecked = true;
                    break;
                case "Alquiler":
                    cbxRent.IsChecked = true;
                    break;
                case "Familiar":
                    cbxFamily.IsChecked = true;
                    break;
            }
        }

        private void BtnClicContinue(object sender, RoutedEventArgs e)
        {
            var customerInfoPage = new CreditApplicationDetailsWorkCenter(requestId);

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

            BtnPersonalReferences.Background = brush;
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
    }
}
