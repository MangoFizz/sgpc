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
	public partial class CreditAnalystSidebar : Page
	{
		public CreditAnalystSidebar(string activeButton)
		{
			InitializeComponent();
			SetActive(activeButton);
		}

		public void SetActive(string button)
		{
			homeButtonBackground.Visibility = Visibility.Hidden;
			creditRequestButtonBackground.Visibility = Visibility.Hidden;

			homeButtonBackgroundHover.Visibility = Visibility.Hidden;
			creditRequestButtonBackgroundHover.Visibility = Visibility.Hidden;
			LogoutButtonBackgroundHover.Visibility = Visibility.Hidden;

			switch (button)
			{
				case "home":
					homeButtonBackground.Visibility = Visibility.Visible;
					break;

				case "creditRequest":
					creditRequestButtonBackground.Visibility = Visibility.Visible;
					break;
			}
		}

		private void LogoutButton_Click(object sender, RoutedEventArgs e)
		{
			UserSession.LogOut();
		}

		private void HomeButton_Click(object sender, RoutedEventArgs e)
		{
			App.Current.MainFrame.Content = new Pages.HomePageCreditAnalyst();
		}

        private void btnCreditRequests_Click(object sender, RoutedEventArgs e)
        {
            App.Current.MainFrame.Content = new Pages.ViewPendingCreditRequests();
        }

		private void HomeButton_MouseEnter(object sender, MouseEventArgs e)
		{
			homeButtonBackgroundHover.Visibility = Visibility.Visible;
		}

		private void HomeButton_MouseLeave(object sender, MouseEventArgs e)
		{
			homeButtonBackgroundHover.Visibility = Visibility.Hidden;
		}

		private void btnCreditRequests_MouseEnter(object sender, MouseEventArgs e)
		{
			creditRequestButtonBackgroundHover.Visibility = Visibility.Visible;
		}

		private void btnCreditRequests_MouseLeave(object sender, MouseEventArgs e)
		{
			creditRequestButtonBackgroundHover.Visibility = Visibility.Hidden;
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
