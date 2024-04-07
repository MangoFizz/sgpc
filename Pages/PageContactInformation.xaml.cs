﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using System.Data.Entity.Validation;
using System.Collections.Generic;
using System.Windows.Media;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Data.Entity.Core;
using SGSC.Messages;
using System.Threading.Tasks;

namespace SGSC.Pages
{
    public partial class PageContactInformation : Page
    {
        
        String PhoneOne = "";
        String PhoneTwo = "";
        String Email = "";
        int CustomerId;
        private sgscEntities dbContext;
        SGSC.CustomerContactInfo userContactInformation = null;
        bool isEditable = false;
        Dictionary<TextBox, System.Windows.Controls.Label> textBoxLabelMap;


        public PageContactInformation(bool isEditable, int CustomerId)
        {
            InitializeComponent();
            dbContext = new sgscEntities();
            this.CustomerId = CustomerId;
            this.isEditable = isEditable;
            txtPhoneOne.PreviewTextInput += AllowPhoneNumber;
            txtPhoneTwo.PreviewTextInput += AllowPhoneNumber;

            if (isEditable)
            {
                try
                {
                    userContactInformation = dbContext.CustomerContactInfoes.FirstOrDefault(c => c.CustomerContactInfoId == 2);
                    ShowInformationContact(userContactInformation);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al intentar recuperar la información de la base de datos: {ex.Message}");
                }
            }
            textBoxLabelMap = new Dictionary<TextBox, System.Windows.Controls.Label>
            {
                {txtPhoneOne, lbPhoneOneIsEmpty},
                {txtPhoneTwo, lbPhoneTwoIsEmpty},
                {txtEmail, lbEmailIsEmpty},
            };
        }

        public void ShowInformationContact(SGSC.CustomerContactInfo userContactInformation)
        {
            if (userContactInformation != null)
            {
                txtPhoneOne.Text = userContactInformation.PhoneNumberOne;
                txtPhoneTwo.Text = userContactInformation.PhoneNumberTwo;
                txtEmail.Text = userContactInformation.Email;
            }
            else
            {
                MessageBox.Show("No se encontró información para el usuario con Id = 1.");
            }
        }

        public bool ValidateFields()
        {
            bool IsValidate = true;

            if (string.IsNullOrWhiteSpace(txtPhoneOne.Text)
                || string.IsNullOrWhiteSpace(txtPhoneTwo.Text)
                || string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                IsValidate = false;


                foreach (var pair in textBoxLabelMap)
                {
                    CheckAndSetLabelVisibility(pair.Value, pair.Key);
                }
            }
            else
            {
                IsValidate = true;
            }
            return IsValidate;
        }

        private void BtnClicContinue(object sender, RoutedEventArgs e)
        {
            if (ValidateFields())
            {
                if (!IsValidEmail(txtEmail.Text))
                {
                    lbEmailIsEmpty.Visibility = Visibility.Visible;
                    return;
                }
                else
                {
                    lbEmailIsEmpty.Visibility = Visibility.Hidden;
                }

                try
                {
                    if (isEditable)
                    {
                        userContactInformation.PhoneNumberOne = txtPhoneOne.Text;
                        userContactInformation.PhoneNumberTwo = txtPhoneTwo.Text;
                        userContactInformation.Email = txtEmail.Text;
                    }
                    else
                    {
                        // Code when there is a new customer
                    }

                    dbContext.SaveChanges();

                    ShowNotification("Se ha registrado con éxito la información", "Success");
                    foreach (var pair in textBoxLabelMap)
                    {
                        pair.Value.Visibility = Visibility.Hidden;
                    }
                }
                catch (EntityException ex)
                {
                    ShowNotification("No se puede conectar con la base de datos. " +
                        "Por favor, inténtelo más tarde.", "Error");
                }
            }

        }

        private void CheckAndSetLabelVisibility(System.Windows.Controls.Label label, TextBox textBox)
        {
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                label.Visibility = Visibility.Visible;
                textBox.BorderBrush = Brushes.Red;
            }
            else
            {
                textBox.ClearValue(Border.BorderBrushProperty);
                label.Visibility = Visibility.Hidden;
            }
        }

        private void AllowPhoneNumber(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                string currentText = textBox.Text;
                string newText = currentText + e.Text;

                if (newText.Length > 10)
                {
                    e.Handled = true;
                    return;
                }
            }
            e.Handled = !Regex.IsMatch(e.Text, "^[0-9]+$");
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public void ShowNotification(string Message, String NotificationType)
        {
            var notificationWindow = new ToastNotification(Message, NotificationType);
            notificationWindow.WindowStartupLocation = WindowStartupLocation.Manual;
            notificationWindow.Left = SystemParameters.WorkArea.Left; // Ajustar según sea necesario
            notificationWindow.Top = SystemParameters.WorkArea.Bottom - notificationWindow.Height; // Ajustar según sea necesario

            notificationWindow.Show();
            Task.Delay(3000).ContinueWith(_ =>
            {
                notificationWindow.Dispatcher.Invoke(() =>
                {
                    notificationWindow.Close();
                });
            });
        }

        private void BtnClicPageContactInformation(object sender, RoutedEventArgs e)
        {
            PageContactInformation contactInformation = new PageContactInformation(false, 2);
            if (NavigationService != null)
            {
                NavigationService.Navigate(contactInformation);
            }
        }

        private void BtnClicPageWorkCenter(object sender, RoutedEventArgs e)
        {
            PageWorkCenter workCenter = new PageWorkCenter(false, 2);
            if (NavigationService != null)
            {
                NavigationService.Navigate(workCenter);
            }
        }

        private void Frame_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {

        }
    }
}