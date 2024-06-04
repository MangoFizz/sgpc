using SGSC.Frames;
using SGSC.Messages;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SGSC.Pages
{
    public partial class PageWorkCenter : Page
    {
        private bool IsRegisteringCreditRequest;
		private string WorkCenterName = "";
        private string Phone = "";
        private string Street = "";
        private string InnerNumber = "";
        private string OutsideNumber = "";
        private string Colony = "";
        private string ZipCode = "";
        private int customerId;
        private int? workCenterId = null;

        private sgscEntities dbContext;

        Dictionary<TextBox, Label> textBoxLabelMap;

        public PageWorkCenter(int customerId, bool isRegisteringCreditRequest = false)
        {
            InitializeComponent();
            dbContext = new sgscEntities();
			IsRegisteringCreditRequest = isRegisteringCreditRequest;
			this.customerId = customerId;

            txtWorkCenterName.PreviewTextInput += AllowWriteLetters;
            txtColony.PreviewTextInput += AllowWriteLetters;

            txtPhone.PreviewTextInput += AllowPhoneNumber;
            txtInnerNumber.PreviewTextInput += AllowWriteNumbers;
            txtOutsideNumber.PreviewTextInput += AllowWriteNumbers;
            txtZipCode.PreviewTextInput += AllowZipCode;

            StepsSidebarFrame.Content = new CustomerRegisterStepsSidebar("WorkCenter");
            UserSessionFrame.Content = new UserSessionFrame();

            try
            {
                WorkCenter workCenter = dbContext.WorkCenters.FirstOrDefault(c => c.CustomerId == customerId);
                if(workCenter != null)
                {
                    ShowInformationWorkCenter(workCenter);
                    workCenterId = workCenter.WorkCenterId;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al intentar recuperar la información de la base de datos: {ex.Message}");
            }

            textBoxLabelMap = new Dictionary<TextBox, Label>
            {
                {txtWorkCenterName, lbIsEmptyCenterName},
                {txtPhone, lbIsEmptyPhone},
                {txtStreet, lbIsEmptyStreet},
                {txtColony, lbIsEmptyColony},
                {txtOutsideNumber, lbIsEmptyOutsideNumber},
                {txtZipCode, lbIsEmptyZipCode}
            };

            foreach (var pair in textBoxLabelMap)
            {
                pair.Value.Visibility = Visibility.Hidden;
            }
        }

        public void ShowInformationWorkCenter(SGSC.WorkCenter userWorkCenter)
        {
            if (userWorkCenter != null)
            {
                txtWorkCenterName.Text = userWorkCenter.CenterName;
                txtPhone.Text = userWorkCenter.PhoneNumber;
                txtStreet.Text = userWorkCenter.Street;
                txtStreet.Text = userWorkCenter.Street;
                txtColony.Text = userWorkCenter.Colony;
                txtZipCode.Text = userWorkCenter.ZipCode.ToString();
                txtInnerNumber.Text = userWorkCenter.InnerNumber.ToString();
                txtOutsideNumber.Text = userWorkCenter.OutsideNumber.ToString();
            }
        }

        public bool ValidateFields()
        {
            bool IsValidate = true;

            if (string.IsNullOrWhiteSpace(txtWorkCenterName.Text) 
                || string.IsNullOrWhiteSpace(txtPhone.Text) 
                || string.IsNullOrWhiteSpace(txtStreet.Text)
                || string.IsNullOrWhiteSpace(txtColony.Text) 
                || string.IsNullOrWhiteSpace(txtOutsideNumber.Text) 
                || string.IsNullOrWhiteSpace(txtZipCode.Text))
            {
                IsValidate = false;
                

                foreach (var pair in textBoxLabelMap)
                {
                    CheckAndSetLabelVisibility(pair.Value, pair.Key);
                }
            } else
            {
                IsValidate = true;
            }
            return IsValidate;
        }

        private void CheckAndSetLabelVisibility(Label label, TextBox textBox)
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


        private void BtnClicContinue(object sender, RoutedEventArgs e)
        {
            if (ValidateFields())
            {
                try
                {
                    WorkCenterName = txtWorkCenterName.Text;
                    Phone = txtPhone.Text;
                    Street = txtStreet.Text;
                    InnerNumber = txtInnerNumber.Text;
                    OutsideNumber = txtOutsideNumber.Text;
                    int IntOutsideNumber = int.Parse(OutsideNumber);
                    Colony = txtColony.Text;
                    ZipCode = txtZipCode.Text;
                    int IntZipCode = int.Parse(ZipCode);

					WorkCenter NewWorkcenter = new WorkCenter
					{
                        CenterName = WorkCenterName,
                        PhoneNumber = Phone,
                        Street = Street,
                        OutsideNumber = IntOutsideNumber,
                        Colony = Colony,
                        ZipCode = IntZipCode,
                        CustomerId = customerId
                    };

                    if (!string.IsNullOrWhiteSpace(txtInnerNumber.Text))
                    {
                        int IntInnerNumber = int.Parse(InnerNumber);
                        NewWorkcenter.InnerNumber = IntInnerNumber;
                    }

                    if(workCenterId != null)
                    {
                        NewWorkcenter.WorkCenterId = workCenterId.Value;
                        dbContext.WorkCenters.AddOrUpdate(NewWorkcenter);
                        dbContext.SaveChanges();
                        App.Current.NotificationsPanel.ShowSuccess("Datos actualizados");
                    }
                    else
                    {
						NewWorkcenter = dbContext.WorkCenters.Add(NewWorkcenter);
						dbContext.SaveChanges();
                        workCenterId = NewWorkcenter.WorkCenterId;
						App.Current.NotificationsPanel.ShowSuccess("Datos guardados");
					}

                    foreach (var pair in textBoxLabelMap)
                    {
                        pair.Value.Visibility = Visibility.Hidden;
                    }

                    App.Current.MainFrame.Navigate(new CustomerContactInfo(customerId, IsRegisteringCreditRequest));
                }
                catch (Exception ex)
                {
                    MessageBox.Show("No se puede conectar con la base de datos. \nPor favor, inténtelo más tarde.", "Error");
                }
            }
        }

        private void AllowWriteLetters(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, "^[a-zA-Z]+$");
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

        private void AllowZipCode(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                string currentText = textBox.Text;

                string newText = currentText + e.Text;

                if (newText.Length > 6)
                {
                    e.Handled = true;
                    return;
                }
            }
            e.Handled = !Regex.IsMatch(e.Text, "^[0-9]+$"); 
        }

        private void AllowWriteNumbers(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                string currentText = textBox.Text;

                string newText = currentText + e.Text;

                if (newText.Length > 5)
                {
                    e.Handled = true;
                    return;
                }
            }
            e.Handled = !Regex.IsMatch(e.Text, "^[0-9]+$");
        }

        private void CancelRegister(object sender, RoutedEventArgs e)
        {
            var result = System.Windows.Forms.MessageBox.Show("¿Está seguro que desea cancelar el registro?\nSi decide cancelarlo puede retomarlo más tarde.", "Cancelar registro", System.Windows.Forms.MessageBoxButtons.YesNo);
            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                App.Current.MainFrame.Content = new HomePageCreditAdvisor();
            }
        }

		private void btnBack_Click(object sender, RoutedEventArgs e)
		{
			App.Current.MainFrame.GoBack();
		}
	}

   
}
