﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SGSC.Pages
{
    /// <summary>
    /// Lógica de interacción para DatosDeCliente.xaml
    /// </summary>
    public partial class DatosDeCliente : Page
    {
        int idCustomer = -1;
        public DatosDeCliente(/*int idCliente*/)
        {
            InitializeComponent();
            if (idCustomer != -1)
            {
                getCustomerInfo(idCustomer);
            }
        }


        private void btnContinue_Click(object sender, RoutedEventArgs e)
        {
            lbName.Content = "";
            lbFirstSurname.Content = "";
            lbSecondSurname.Content = "";
            lbCurp.Content = "";

            bool valid = true;
            if (string.IsNullOrEmpty(tbName.Text))
            {
                valid = false;
                lbName.Content = "Porfavor introduzca el nombre";
            }
            if (string.IsNullOrEmpty(tbFirstSurname.Text))
            {
                valid = false;
                lbFirstSurname.Content = "Porfavor introduzca el apellido paterno";
            }
            if (string.IsNullOrEmpty(tbSecondSurname.Text))
            {
                valid = false;
                lbSecondSurname.Content = "Porfavor introduzca el apellido materno";
            }
            if (string.IsNullOrEmpty(tbCURP.Text))
            {
                valid = false;
                lbCurp.Content = "Porfavor introduzca el CURP";
            }
            if (!Validator.ValidateCURP(tbCURP.Text))
            {
                valid = false;
                lbCurp.Content = "Porfavor introduzca un CURP válido";
            }
            if (!valid)
            {
                return;
            }
            if (idCustomer == -1)
            {
                registerCustomer();
            }
            else
            {
                updateCustomer();
            }
        }   

        private void registerCustomer()
        {
            using (sgscEntities db = new sgscEntities())
            {
                Customer customerToRegister = new Customer();
                customerToRegister.Curp = tbCURP.Text;
                customerToRegister.Name = tbName.Text;
                customerToRegister.FirstSurname = tbFirstSurname.Text;
                customerToRegister.SecondSurname = tbSecondSurname.Text;
                db.Customers.Add(customerToRegister);

                try
                {
                    db.SaveChanges();
                    MessageBox.Show("Cliente registrado exitosamente.");
                    tbCURP.Text = "";
                    tbName.Text = "";
                    tbFirstSurname.Text = "";
                    tbSecondSurname.Text = "";
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ocurrió un error al intentar registrar el cliente: " + ex.Message);
                }
            }
        }

        private void updateCustomer()
        {
            using (sgscEntities db = new sgscEntities())
            {
                Customer customerToUpdate = db.Customers.Find(idCustomer);
                customerToUpdate.Curp = tbCURP.Text;
                customerToUpdate.Name = tbName.Text;
                customerToUpdate.FirstSurname = tbFirstSurname.Text;
                customerToUpdate.SecondSurname = tbSecondSurname.Text;

                try
                {
                    db.SaveChanges();
                    Console.WriteLine("Cliente actualizado exitosamente.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ocurrió un error al intentar actualizar el cliente: " + ex.Message);
                }
            }
        }

        private void tbCURP_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (tbCURP.Text.Length >= 18 || System.Text.RegularExpressions.Regex.IsMatch(tbCURP.Text + e.Text, @"[^a-zA-Z0-9]+$"))
            {
                e.Handled = true;
            }
        }

        private void getCustomerInfo(int idCustomer)
        {
            using (sgscEntities db = new sgscEntities())
            {
                Customer customer = db.Customers.Find(idCustomer);
                tbCURP.Text = customer.Curp;
                tbName.Text = customer.Name;
                tbFirstSurname.Text = customer.FirstSurname;
                tbSecondSurname.Text = customer.SecondSurname;
            }
        }
    }
}
