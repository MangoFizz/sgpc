using SGSC.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
	/// Interaction logic for NotificationsPanelFrame.xaml
	/// </summary>
	public partial class NotificationsPanelFrame : Page
	{
		public NotificationsPanelFrame()
		{
			InitializeComponent();
		}

		private void SetNotificationEntryCloseTimer(Frame notificationFrame)
		{
			Task.Factory.StartNew(() => {
				Thread.Sleep(5600);
				Dispatcher.Invoke(() => {
					NotificationsStackPanel.Children.Remove(notificationFrame);
				});
			});
		}

		public void ShowSuccess(string message)
		{
			var page = new SuccessNotification(message);
			page.VerticalAlignment = VerticalAlignment.Bottom;
			var inviteFrame = new Frame()
			{
				Content = page
			};
			NotificationsStackPanel.Children.Add(inviteFrame);

			SetNotificationEntryCloseTimer(inviteFrame);
		}
	}
}
