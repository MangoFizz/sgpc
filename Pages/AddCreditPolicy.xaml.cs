using System;
using System.Windows;
using System.Windows.Controls;

namespace SGSC.Pages
{
    public partial class AddCreditPolicy : Page
    {
        private CreditPolicyWithStatus selectedPolicy;

        public AddCreditPolicy(CreditPolicyWithStatus selectedPolicy)
        {
            InitializeComponent();
            this.selectedPolicy = selectedPolicy;

            // Si se proporciona una política de crédito seleccionada, muestra sus datos en los campos correspondientes
            if (selectedPolicy != null)
            {
                txtWorkPolityCreditName.Text = selectedPolicy.Name;
                txtDescription.Text = selectedPolicy.Description;

                // Asigna la fecha efectiva de la política si está disponible
                if (selectedPolicy.EffectiveDate != DateTime.MinValue)
                {
                    cdDate.SelectedDate = selectedPolicy.EffectiveDate;
                }
                else
                {
                    cdDate.SelectedDate = null; // o alguna fecha por defecto si lo prefieres
                }
            }
        }

        private void BtnClickSave(object sender, RoutedEventArgs e)
        {
            // Obtener los datos ingresados por el usuario
            string name = txtWorkPolityCreditName.Text;
            string description = txtDescription.Text;
            DateTime? effectiveDate = cdDate.SelectedDate;

            // Validar que se hayan ingresado todos los datos necesarios
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(description) || !effectiveDate.HasValue)
            {
                MessageBox.Show("Por favor, complete todos los campos para crear o actualizar la política de crédito.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Crear o actualizar la política de crédito según corresponda
            if (selectedPolicy == null)
            {
                // Crear una nueva política de crédito
                CreditPolicies policy = new CreditPolicies
                {
                    Name = name,
                    Description = description,
                    EffectiveDate = effectiveDate.Value
                };

                // Guardar la nueva política de crédito en la base de datos
                SaveCreditPolicy(policy);

                MessageBox.Show("Se ha creado una nueva política de crédito.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                // Actualizar la política de crédito seleccionada
                selectedPolicy.Name = name;
                selectedPolicy.Description = description;
                selectedPolicy.EffectiveDate = effectiveDate.Value;

                // Guardar los cambios en la base de datos
                UpdateCreditPolicy(selectedPolicy);

                MessageBox.Show("Se ha actualizado la política de crédito.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            // Navegar de vuelta a la página anterior
            NavigationService.Content = new ManageCreditGrantingPolicies();
        }

        private void SaveCreditPolicy(CreditPolicies policy)
        {
            try
            {
                using (sgscEntities db = new sgscEntities())
                {
                    db.CreditPolicies.Add(policy);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar la política de crédito: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateCreditPolicy(CreditPolicyWithStatus policy)
        {
            try
            {
                using (sgscEntities db = new sgscEntities())
                {
                    // Aquí asumimos que `CreditPolicy` es la entidad de tu base de datos que corresponde a `CreditPolicyWithStatus`
                    var dbPolicy = db.CreditPolicies.Find(policy.Id); // Suponiendo que hay una propiedad `Id` que identifica la política
                    if (dbPolicy != null)
                    {
                        dbPolicy.Name = policy.Name;
                        dbPolicy.Description = policy.Description;
                        dbPolicy.EffectiveDate = policy.EffectiveDate;
                        db.SaveChanges();
                    }
                    else
                    {
                        MessageBox.Show("No se encontró la política de crédito en la base de datos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al actualizar la política de crédito: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnReturn(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
