using SGSC.Frames;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace SGSC.Pages
{
    public partial class HomePageCreditAnalyst : Page
    {
        public HomePageCreditAnalyst()
        {
            InitializeComponent();
			creditAdvisorSidebar.Content = new CreditAnalystSidebar("home");
		}

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int requestId = 6;
            var customerInfoPage = new CreditApplicationDetailsRequest(requestId);
            if (NavigationService != null)
            {
                NavigationService.Navigate(customerInfoPage);
            }
        }
    }
}
