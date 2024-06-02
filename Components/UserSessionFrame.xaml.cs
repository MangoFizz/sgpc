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
    /// Interaction logic for UserSessionFrame.xaml
    /// </summary>
    public partial class UserSessionFrame : Page
    {
        public UserSessionFrame()
        {
            InitializeComponent();
            UserName.Text = UserSession.Instance.FullName;
            RoleName.Text = Employee.GetRoleName(UserSession.Instance.Role);
        }
    }
}
