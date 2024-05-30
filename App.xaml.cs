using SGSC.Frames;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SGSC
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Application instance
        /// </summary>
        public static new App Current
        {
            get
            {
                return (App)Application.Current;
            }
        }

        /// <summary>
        /// Main window instance
        /// </summary>
        public new MainWindow MainWindow
        {
            get
            {
                return (MainWindow)base.MainWindow;
            }
        }

        public Frame MainFrame
        {
            get
            {
                return MainWindow.MainFrame;
            }
        }

        public NotificationsPanelFrame NotificationsPanel
        {
            get
            {
                return (NotificationsPanelFrame)MainWindow.NotificationsFrame.Content;
            }
        }
    }
}
