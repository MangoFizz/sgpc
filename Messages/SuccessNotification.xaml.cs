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

namespace SGSC.Messages
{
	/// <summary>
	/// Interaction logic for SuccessNotification.xaml
	/// </summary>
	public partial class SuccessNotification : Page
	{
		public SuccessNotification(string message)
		{
			InitializeComponent();
			Message.Text = message;
		}
	}
}
