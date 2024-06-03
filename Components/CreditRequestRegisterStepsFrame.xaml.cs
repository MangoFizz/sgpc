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

namespace SGSC.Components
{
	/// <summary>
	/// Interaction logic for CreditRequestRegisterStepsFrame.xaml
	/// </summary>
	public partial class CreditRequestRegisterStepsFrame : Page
	{
		public CreditRequestRegisterStepsFrame(string currentStep)
		{
			InitializeComponent();
			this.SetActive(currentStep);
		}

		public void SetActive(string active)
		{
			PersonalInfoActive.Visibility = Visibility.Hidden;
			AddressActive.Visibility = Visibility.Hidden;
			BankAccountsActive.Visibility = Visibility.Hidden;
			WorkCenterActive.Visibility = Visibility.Hidden;

			switch (active)
			{
				case "PersonalInfo":
					AddressActive.Visibility = Visibility.Visible;
					break;
				case "Address":
					BankAccountsActive.Visibility = Visibility.Visible;
					break;
				case "WorkCenter":
					WorkCenterActive.Visibility = Visibility.Visible;
					break;
			}
		}
	}
}
