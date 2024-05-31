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
using System.Windows.Shapes;

namespace SGSC.Messages
{
    public partial class ToastNotification : Window
    {
        String Message;
        String NotificationType;

        public ToastNotification(string Message, string NotificationType)
        {
            InitializeComponent();
            this.WindowStyle = WindowStyle.None;
            this.Message = Message;
            this.NotificationType = NotificationType;
            ShowMessage(Message, NotificationType);
        }

        public void ShowMessage(string Message, String NotificationType)
        {
            if (NotificationType == "Error")
            {
                ErrorIcon.Visibility = Visibility.Visible;
                lbMesagge.Content = Message;
            }
            else
            {
                if (NotificationType == "Success")
                {
                    SuccessIcon.Visibility = Visibility.Visible;
                    lbMesagge.Content = Message;
                }
            }
        }
    }
}
