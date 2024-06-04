using SGSC.Utils;
using System.Windows;
using System.Windows.Controls;

namespace SGSC.Pages
{
    public partial class HomePageCollectionExecutive : Page
    {
        public HomePageCollectionExecutive()
        {
            InitializeComponent();
            creditAdvisorSidebar.Content = new Frames.CollectionExecutiveSidebar("home");
        }

        private void btnViewActiveCredits_Click(object sender, RoutedEventArgs e)
        {
            var activeCreditsPage = new ViewActiveCreditsPage();
            if (NavigationService != null)
            {
                NavigationService.Navigate(activeCreditsPage);
            }

        }
    }
}
