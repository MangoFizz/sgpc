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
    /// Lógica de interacción para CreditPromotionDetails.xaml
    /// </summary>
    public partial class CreditPromotionDetails : Page
    {
        private bool editMode = false;
        private int idPromotion = -1;
        public CreditPromotionDetails(int idPromotion)
        {
            InitializeComponent();
            adminSidebar.Content = new Frames.AdminSidebar("ManageCreditGrantingPolicies");
            UserSessionFrame.Content = new Frames.UserSessionFrame();

            this.idPromotion = idPromotion;
            tbName.Focus();
            btnModify.Visibility = Visibility.Hidden;
            btnModify.IsEnabled = false;
            
            lbName.Content = "";
            lbInterestRate.Content = "";
            lbTimePeriod.Content = "";
            lbStartDate.Content = "";
            lbEndDate.Content = "";
            lbInterval.Content = "";

            if (idPromotion != -1)
            {
                retrievePromotionDetails(idPromotion);
                btnModify.Visibility = Visibility.Visible;
                btnModify.IsEnabled = true;
                btnRegister.Visibility = Visibility.Hidden;
                btnRegister.IsEnabled = false;
                cbInterval.IsEnabled = false;
            }
        }

        private void enableEdit()
        {
            tbName.IsReadOnly = false;
            tbTimePeriod.IsReadOnly = false;
            tbInterestRate.IsReadOnly = false;
            dpEndDate.IsEnabled = true;
            dpStartDate.IsEnabled = true;
            btnModify.Visibility = Visibility.Hidden;
            btnModify.IsEnabled = false;
            btnRegister.Visibility = Visibility.Visible;
            btnRegister.IsEnabled = true;
            cbInterval.IsEnabled = true;
            editMode = true;
        }

        public void retrievePromotionDetails(int idPromotion)
        {
            using (sgscEntities db = new sgscEntities())
            {
                var promotion = db.CreditPromotions.Where(p => p.CreditPromotionId == idPromotion).FirstOrDefault();
                if (promotion != null)
                {
                    tbName.Text = promotion.Name;
                    tbTimePeriod.Text = promotion.TimePeriod.ToString();
                    tbInterestRate.Text = promotion.InterestRate.ToString();
                    dpStartDate.SelectedDate = promotion.StartDate;
                    dpEndDate.SelectedDate = promotion.EndDate;

                    tbName.IsReadOnly = true;
                    tbTimePeriod.IsReadOnly = true;
                    tbInterestRate.IsReadOnly = true;
                    dpEndDate.IsEnabled = false;
                    dpStartDate.IsEnabled = false;
                }
            }
        }

        private void btnModify_Click(object sender, RoutedEventArgs e)
        {
            enableEdit();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            App.Current.MainFrame.Content = new CreditPromotions();
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            lbName.Content = "";
            lbInterestRate.Content = "";
            lbTimePeriod.Content = "";
            lbStartDate.Content = "";
            lbEndDate.Content = "";
            lbInterval.Content = "";
            bool valid = true;

            if (string.IsNullOrEmpty(tbName.Text))
            {
                valid = false;
                lbName.Content= "Por favor introduzca el nombre";
            }
            if (string.IsNullOrEmpty(tbInterestRate.Text))
            {
                valid = false;
                lbInterestRate.Content = "Por favor introduzca la tasa de interés";
            }
            if (!double.TryParse(tbInterestRate.Text, out double interestRate))
            {
                valid = false;
                lbInterestRate.Content = "Por favor introduzca una tasa de interés valida";
            }
            if (string.IsNullOrEmpty(tbTimePeriod.Text))
            {
                valid = false;
                lbTimePeriod.Content = "Por favor introduzca el periodo de tiempo";
            }
            if (!int.TryParse(tbTimePeriod.Text, out int timePeriod))
            {
                valid = false;
                lbTimePeriod.Content = "Por favor introduzca un periodo de tiempo valido";
            }
            if (dpStartDate.SelectedDate == null)
            {
                valid = false;
                lbStartDate.Content = "Por favor introduzca la fecha de inicio";
            }
            if (dpEndDate.SelectedDate == null)
            {
                valid = false;
                lbEndDate.Content = "Por favor introduzca la fecha de fin";
            }
            if (dpStartDate.SelectedDate > dpEndDate.SelectedDate)
            {
                valid = false;
                lbEndDate.Content = "La fecha de fin debe ser mayor a la fecha de inicio";
            }
            if (cbInterval.SelectedIndex == -1)
            {
                valid = false;
                lbInterval.Content = "Por favor seleccione un intervalo";
            }
            if (valid)
            {
                if (editMode)
                {
                    updatePromotion();
                }
                else
                {
                    registerPromotion();
                }
            }
        }

        private void registerPromotion()
        {
            using (sgscEntities db = new sgscEntities())
            {
               /* CreditPromotions promotion = new CreditPromotions();
                promotion.Name = tbName.Text;
                promotion.TimePeriod = int.Parse(tbTimePeriod.Text);
                promotion.InterestRate = double.Parse(tbInterestRate.Text);
                promotion.StartDate = dpStartDate.SelectedDate.Value;
                promotion.EndDate = dpEndDate.SelectedDate.Value;
                promotion.Interval = cbInterval.SelectedIndex+1;

                db.CreditPromotions.Add(promotion);
                db.SaveChanges();

                tbName.Text = "";
                tbTimePeriod.Text = "";
                tbInterestRate.Text = "";
                dpStartDate.SelectedDate = DateTime.Now;
                dpEndDate.SelectedDate = null;

                MessageBox.Show("Promoción registrada con éxito", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                App.Current.MainFrame.Content = new CreditPromotions();*/
            }
        }

        private void updatePromotion()
        {
            using (sgscEntities db = new sgscEntities())
            {
                var promotion = db.CreditPromotions.Where(p => p.CreditPromotionId == idPromotion).FirstOrDefault();
                if (promotion != null)
                {
                    promotion.Name = tbName.Text;
                    promotion.TimePeriod = int.Parse(tbTimePeriod.Text);
                    promotion.InterestRate = double.Parse(tbInterestRate.Text);
                    promotion.StartDate = dpStartDate.SelectedDate.Value;
                    promotion.EndDate = dpEndDate.SelectedDate.Value;
                    promotion.Interval = cbInterval.SelectedIndex+1;

                    db.SaveChanges();

                    MessageBox.Show("Promoción actualizada con éxito", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    App.Current.MainFrame.Content = new CreditPromotions();
                }
            }
        }

    }
}
