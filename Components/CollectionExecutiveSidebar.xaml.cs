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
	public partial class CollectionExecutiveSidebar : Page
	{
		public CollectionExecutiveSidebar(string activeButton)
		{
			InitializeComponent();
			SetActive(activeButton);
		}

		public void SetActive(string button)
		{
			homeButtonBackground.Visibility = Visibility.Hidden;
			creditRequestButtonBackground.Visibility = Visibility.Hidden;

			switch(button)
			{
				case "home":
					homeButtonBackground.Visibility = Visibility.Visible;
					break;

				case "activeCredits":
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
			App.Current.MainFrame.Content = new Pages.HomePageCollectionExecutive();
		}

		private void ActiveCreditsButton_Click(object sender, RoutedEventArgs e)
		{
			App.Current.MainFrame.Content = new Pages.ActiveCreditsPage();
		}
	}
}
