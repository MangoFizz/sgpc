﻿using SGSC.Utils;
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

namespace SGSC.Pages
{
	/// <summary>
	/// Interaction logic for LogIn.xaml
	/// </summary>
	public partial class LogIn : Page
	{
		public LogIn()
		{
			InitializeComponent();
            tblEmailError.Text = "";
            tblPasswordError.Text = "";
            DataBaseError();
        }

		private void btnLogIn_Click(object sender, RoutedEventArgs e)
		{
            tblEmailError.Text = "";
            tblPasswordError.Text = "";

			bool valid = true;

            if(tbEmail.Text.Equals(""))
            {
                tblEmailError.Text = "Por favor ingresa tu correo electrónico.";
				valid = false;
            }

			if(pbPassword.Password.Equals(""))
			{
                tblPasswordError.Text = "Por favor ingresa tu contraseña.";
				valid = false;
            }

			if(!valid)
			{
                return;
            }

			var email = tbEmail.Text;
			var password = pbPassword.Password;

			var res = Utils.Authenticator.AuthUser(email, password, true);
			switch(res)
			{
				case Utils.Authenticator.AuthResult.Success:
                    break;

				case Utils.Authenticator.AuthResult.InvalidCredentials:
					MessageBox.Show("Credenciales inválidas.");
					break;

				case Utils.Authenticator.AuthResult.DatabaseError:
					MessageBox.Show("Error de conexión. Intentelo más tarde.");
					break;

				default:
					MessageBox.Show("Error desconocido.");
					break;
            }
		}


        private void Label_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
