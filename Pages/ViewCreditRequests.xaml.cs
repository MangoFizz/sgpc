using SGSC.Frames;
using SGSC.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Printing;
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
using System.Windows.Threading;
using System.Globalization;


namespace SGSC.Pages
{
    /// <summary>
    /// Interaction logic for ViewCreditRequests.xaml
    /// </summary>
    public partial class ViewCreditRequests : Page
    {
		private class PendingRequestsTableEntry
		{
			public int Id { get; set; }
			public int CustomerId { get; set; }
			public string FileNumber { get; set; }
			public string CustomerFullName { get; set; }
			public string Period { get; set; }
			public string Amount { get; set; }
			public string InterestRate { get; set; }
			public string CustomerRFC { get; set; }
			public string CreationDate { get; set; }
			public string State { get; set; }
		}

		private ObservableCollection<PendingRequestsTableEntry> ActiveCredits;
		private int CurrentPage = 1;
		private int TotalPages = 1;
		private const int ItemsPerPage = 10;
		private bool UpdatingPagination = false;

		public ViewCreditRequests()
		{
			InitializeComponent();
			creditAdvisorSidebar.Content = new CreditAdvisorSidebar("creditRequest");
			GetPendingRequests();
		}

		private void UpdatePagination()
		{
			UpdatingPagination = true;

			try
			{
				using (var context = new sgscEntities())
				{
					var activeCreditsCount = context.CreditRequests
						.Where(request => request.Status == (int)CreditRequest.RequestStatus.Pending || 
							request.Status == (int)CreditRequest.RequestStatus.Captured || 
							request.Status == (int)CreditRequest.RequestStatus.WaitingForCorrection)
						.Where(request => request.FileNumber.Contains(tbPageNumberFilter.Text))
						.Where(request => (request.Customer.Name + " " + request.Customer.FirstSurname + " " + request.Customer.SecondSurname).Contains(tbCustomerNameFilter.Text))
						.Count();
					TotalPages = (int)Math.Ceiling((double)activeCreditsCount / ItemsPerPage);
					lbCurrentPage.Content = $"Página {CurrentPage}/{TotalPages}";
					cbPages.Items.Clear();
					for (uint i = 1; i <= TotalPages; i++)
					{
						cbPages.Items.Add(i);
					}
					cbPages.SelectedIndex = CurrentPage - 1;

					btnNextPage.IsEnabled = CurrentPage < TotalPages;
					btnPreviousPage.IsEnabled = CurrentPage > 1;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("Error al intentar obtener los datos de los créditos activos: " + ex.Message);
			}

			UpdatingPagination = false;

		}

		private void GetPendingRequests()
		{
			try
			{
				using (var context = new sgscEntities())
				{
					var activeCredits = context.CreditRequests
						.Where(request => request.Status == (int)CreditRequest.RequestStatus.Pending ||
							request.Status == (int)CreditRequest.RequestStatus.Captured ||
							request.Status == (int)CreditRequest.RequestStatus.WaitingForCorrection)
						.Where(request => request.FileNumber.Contains(tbPageNumberFilter.Text))
						.Where(request => (request.Customer.Name + " " + request.Customer.FirstSurname + " " + request.Customer.SecondSurname).Contains(tbCustomerNameFilter.Text))
						.OrderBy(request => request.FileNumber)
						.Skip((CurrentPage - 1) * ItemsPerPage)
						.Take(ItemsPerPage);

					var activeCreditsArray = activeCredits.ToList();
					ActiveCredits = new ObservableCollection<PendingRequestsTableEntry>();
					foreach (var item in activeCreditsArray)
					{
						ActiveCredits.Add(new PendingRequestsTableEntry
						{
							Id = item.CreditRequestId,
							CustomerId = item.Customer.CustomerId,
							FileNumber = item.FileNumber,
							CustomerFullName = item.Customer.FullName,
							Period = item.TimePeriod + (item.PaymentsInterval == 0 ? " quincenas" : " meses"),
							Amount = "$ " + item.Amount.Value.ToString("0.00"),
							InterestRate = item.InterestRate + "%",
							CustomerRFC = item.Customer.Rfc,
							CreationDate = item.CreationDate.Value.ToString("dd/MM/yyyy"),
							State = CreditRequest.RequestStatusToString((CreditRequest.RequestStatus)item.Status.Value)
						});
					}
					dgCredits.ItemsSource = ActiveCredits;
				}
				UpdatePagination();
			}
			catch (Exception ex)
			{
				MessageBox.Show("Error al intentar obtener los datos de los créditos activos: " + ex.Message);
			}
		}

		private void LogoutButton_Click(object sender, RoutedEventArgs e)
		{
			UserSession.LogOut();
		}

		private void tbFilter_TextChanged(object sender, TextChangedEventArgs e)
		{
			GetPendingRequests();
		}

		private void btnNextPage_Click(object sender, RoutedEventArgs e)
		{
			CurrentPage++;
			GetPendingRequests();
		}

		private void btnPreviousPage_Click(object sender, RoutedEventArgs e)
		{
			CurrentPage--;
			GetPendingRequests();
		}

		private void cbPages_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (UpdatingPagination)
			{
				return;
			}
			CurrentPage = cbPages.SelectedIndex + 1;
			GetPendingRequests();
		}

		private void btnDeleteRequest_Click(object sender, RoutedEventArgs e)
		{
			var request = dgCredits.SelectedItem as PendingRequestsTableEntry;
			if (request != null)
			{
				var result = System.Windows.Forms.MessageBox.Show("¿Está seguro que desea eliminar la solicitud seleccionada?\n\n¡ESTA ACCIÓN ES IRREVERSIBLE!", "Eliminar empleado", System.Windows.Forms.MessageBoxButtons.YesNo);
				if (result == System.Windows.Forms.DialogResult.Yes)
				{
					try
					{
						using (var context = new sgscEntities())
						{
							var requestToDelete = context.CreditRequests.Find(request.Id);
							context.CreditRequests.Remove(requestToDelete);
							context.SaveChanges();
							GetPendingRequests();
						}
					}
					catch (Exception ex)
					{
						MessageBox.Show("Error al intentar eliminar la solicitud seleccionada: " + ex.Message);
					}
				}
			}
			else
			{
				MessageBox.Show("Por favor, seleccione una solicitud de la tabla.");
			}
		}

		private void btnModifyRequest_Click(object sender, RoutedEventArgs e)
		{
			var request = dgCredits.SelectedItem as PendingRequestsTableEntry;
			if (request != null)
			{
				App.Current.MainFrame.Navigate(new RegisterCreditRequestPage(request.CustomerId, request.Id));
			}
			else
			{
				MessageBox.Show("Por favor, seleccione una solicitud de la tabla.");
			}
		}
	}
}