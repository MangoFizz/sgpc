using SGSC.Pages;
using SGSC.Utils;
using System;
using System.Collections.Generic;
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

namespace SGSC.Frames
{
	/// <summary>
	/// Interaction logic for CredirAdvisorSidebar.xaml
	/// </summary>
	public partial class AdminSidebar : Page
	{
		public AdminSidebar(string activeButton)
		{
			InitializeComponent();
			SetActive(activeButton);
		}

		public void SetActive(string button)
		{
			
		}

		private void LogoutButton_Click(object sender, RoutedEventArgs e)
		{
			UserSession.LogOut();
		}

		private void ManageEmployeesButton_Click(object sender, RoutedEventArgs e)
		{
			App.Current.MainFrame.Content = new Pages.ManageEmployeesPage();
		}

        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            App.Current.MainFrame.Content = new Pages.HomePageAdmin();
        }

        private void btnCreditPolicies_Click(object sender, RoutedEventArgs e)
        {
			App.Current.MainFrame.Content = new Pages.ManageCreditGrantingPolicies();
        }

        private void btnCreditPromotions_Click(object sender, RoutedEventArgs e)
        {
			App.Current.MainFrame.Content = new CreditPromotions();
        }
    }
}
