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
			homeButtonBackground.Visibility = Visibility.Hidden;
			EmployeesButtonBackground.Visibility = Visibility.Hidden;
			CreditPoliciesButtonBackground.Visibility = Visibility.Hidden;
			CreditPromotionsButtonBackground.Visibility = Visibility.Hidden;
			CreditConditionsButtonBackground.Visibility = Visibility.Hidden;

			homeButtonBackgroundHover.Visibility = Visibility.Hidden;
			EmployeesButtonBackgroundHover.Visibility = Visibility.Hidden;
			CreditPoliciesButtonBackgroundHover.Visibility = Visibility.Hidden;
			CreditPromotionsButtonBackgroundHover.Visibility = Visibility.Hidden;
			CreditConditionsButtonBackgroundHover.Visibility = Visibility.Hidden;
			LogoutButtonBackgroundHover.Visibility = Visibility.Hidden;

			switch (button)
			{
				case "home":
					homeButtonBackground.Visibility = Visibility.Visible;
					break;

				case "employees":
					EmployeesButtonBackground.Visibility = Visibility.Visible;
					break;

				case "creditPolicies":
					CreditPoliciesButtonBackground.Visibility = Visibility.Visible;
					break;

				case "creditPromotions":
					CreditPromotionsButtonBackground.Visibility = Visibility.Visible;
					break;

				case "creditConditions":
					CreditConditionsButtonBackground.Visibility = Visibility.Visible;
					break;
			}
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
			App.Current.MainFrame.Content = new ManageCreditPromotionsPage();
        }

		private void btnHome_MouseEnter(object sender, MouseEventArgs e)
		{
			homeButtonBackgroundHover.Visibility = Visibility.Visible;
		}

		private void btnHome_MouseLeave(object sender, MouseEventArgs e)
		{
			homeButtonBackgroundHover.Visibility = Visibility.Hidden;
		}

		private void btnEmployees_MouseEnter(object sender, MouseEventArgs e)
		{
			EmployeesButtonBackgroundHover.Visibility = Visibility.Visible;
		}

		private void btnEmployees_MouseLeave(object sender, MouseEventArgs e)
		{
			EmployeesButtonBackgroundHover.Visibility = Visibility.Hidden;
		}

		private void btnCreditPolicies_MouseEnter(object sender, MouseEventArgs e)
		{
			CreditPoliciesButtonBackgroundHover.Visibility = Visibility.Visible;
		}

		private void btnCreditPolicies_MouseLeave(object sender, MouseEventArgs e)
		{
			CreditPoliciesButtonBackgroundHover.Visibility = Visibility.Hidden;
		}

		private void btnCreditPromotions_MouseEnter(object sender, MouseEventArgs e)
		{
			CreditPromotionsButtonBackgroundHover.Visibility = Visibility.Visible;
		}

		private void btnCreditPromotions_MouseLeave(object sender, MouseEventArgs e)
		{
			CreditPromotionsButtonBackgroundHover.Visibility = Visibility.Hidden;
		}

		private void btnCreditConditions_MouseEnter(object sender, MouseEventArgs e)
		{
			CreditConditionsButtonBackgroundHover.Visibility = Visibility.Visible;
		}

		private void btnCreditConditions_MouseLeave(object sender, MouseEventArgs e)
		{
			CreditConditionsButtonBackgroundHover.Visibility = Visibility.Hidden;
		}

		private void LogoutButton_MouseEnter(object sender, MouseEventArgs e)
		{
			LogoutButtonBackgroundHover.Visibility = Visibility.Visible;
		}

		private void LogoutButton_MouseLeave(object sender, MouseEventArgs e)
		{
			LogoutButtonBackgroundHover.Visibility = Visibility.Hidden;
		}
	}
}
