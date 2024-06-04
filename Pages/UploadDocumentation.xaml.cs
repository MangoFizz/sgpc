using SGSC.Messages;
using System.Windows;
using System.Windows.Controls;
using System.Linq; // Asegúrate de tener esto
using System.Data.Entity;
using System; // Asegúrate de tener esto también
using System.Windows.Forms;
using System.IO;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.Windows.Media;
using WpfButton = System.Windows.Controls.Button;
using System.Xml.Linq;
using ReportDocument = iTextSharp.text.Document;
using MessageBox = System.Windows.MessageBox;
using SGSC.Components;
using SGSC.Frames;


namespace SGSC.Pages
{
	public partial class UploadDocumentation : Page
	{
		private const long MaxFileSize = 10 * 1024 * 1024; // 10MB en bytes

		private int IdCreditRequest;
		private string VendorName;
		private string RequestNumber;
		private string CreationDate;
		private string RequestedAmountNumber;
		private string InterestRate;
		private string Purpose;
		private string TimePeriod;
		private string Name;
		private string FirstSurname;
		private string SecondSurname;
		private string DateOfBirthay;
		private string Gender;
		private string Curp;
		private string Email;
		private string Street;
		private string ExternalNumber;
		private string InternalNumber;
		private string State;
		private string Colony;
		private string ZipCode;
		private string AddressType;
		private string PhoneOne;
		private string PhoneTwo;
		private string MaritalStatus;

		private string WorkCenterName;
		private string PhoneNumberWorkCenter;
		private string InnerNumberWorkCenter;
		private string OutsideNumberWorkCenter;
		private string ColonyWorkCenter;
		private string StreetWorkCenter;
		private string ZipCodeWorkCenter;

		private string NameReferenceOne;
		private string RelationshipReferenceOne;
		private string PhoneReferenceOne;

		private string NameReferenceTwo;
		private string RelationshipReferenceTwo;
		private string PhoneReferenceTwo;

		private string NameBankAccountTransfer;
		private string InterbankCodeAccountTransfer;
		private string NumberAccountTransfer;

		private string NameDomicialization;
		private string InterbankCodeDomicialization;
		private string NumberAccountDomicialization;

		public UploadDocumentation(int idCreditRequest)
		{
			this.IdCreditRequest = idCreditRequest;
			InitializeComponent();
			StepsSidebarFrame.Content = new CreditRequestRegisterStepsFrame("WorkCenter");
			UserSessionFrame.Content = new UserSessionFrame();
			GetRequestInfo();
			GetPersonalInformation();
			GetWorkCenterInformation();
			ObtainPersonalReferences();
			GetBankAccounts();
		}



		private void GetBankAccounts()
		{
			using (sgscEntities db = new sgscEntities())
			{
				var transferenciaAccount = (from ba in db.BankAccounts
											join cr in db.CreditRequests on ba.BankAccountId equals cr.TransferBankAccountId
											join b in db.Banks on ba.BankBankId equals b.BankId
											where cr.CreditRequestId == IdCreditRequest
											select new
											{
												BankName = b.Name,
												ba.InterbankCode,
												ba.CardNumber
											}).FirstOrDefault();

				if (transferenciaAccount == null)
				{
					ToastNotification notification = new ToastNotification("No se encontró la solicitud especificada o no hay cuenta de transferencia asociada.", "Error");
				}
				else
				{
					NameBankAccountTransfer = transferenciaAccount.BankName;
					InterbankCodeAccountTransfer = transferenciaAccount.InterbankCode;
					NumberAccountTransfer = transferenciaAccount.CardNumber;
				}

				var domicializationAccount = (from ba in db.BankAccounts
											  join cr in db.CreditRequests on ba.BankAccountId equals cr.DirectDebitBankAccountId
											  join b in db.Banks on ba.BankBankId
											  equals b.BankId
											  where cr.CreditRequestId == IdCreditRequest
											  select new
											  {
												  BankName = b.Name,
												  ba.InterbankCode,
												  ba.CardNumber
											  }).FirstOrDefault();

				if (domicializationAccount == null)
				{
					ToastNotification notification = new ToastNotification("No se encontró la solicitud especificada o no hay cuenta de domiciliación asociada.", "Error");
				}
				else
				{
					NameDomicialization = domicializationAccount.BankName;
					InterbankCodeDomicialization = domicializationAccount.InterbankCode;
					NumberAccountDomicialization = domicializationAccount.CardNumber;
				}
			}
		}

		private void GetRequestInfo()
		{
			using (sgscEntities db = new sgscEntities())
			{
				var requestInfo = (from request in db.CreditRequests
								   join employee in db.Employees on request.EmployeeId equals employee.EmployeeId
								   where request.CreditRequestId == IdCreditRequest
								   select new
								   {
									   VendorName = employee.Name + " " + employee.FirstSurname + " " + employee.SecondSurname,
									   RequestNumber = request.FileNumber,
									   CreationDate = request.CreationDate,
									   TimePeriod = request.TimePeriod,
									   RequestedAmountNumber = request.Amount,
									   Period = request.TimePeriod,
									   InterestRate = request.InterestRate,
									   Purpose = request.Purpose,
								   }).FirstOrDefault();

				if (requestInfo == null)
				{
					ToastNotification notification = new ToastNotification("No se ha encontrado la solicitud, inténtelo más tarde", "Error");
					notification.Show();
				}
				else
				{
					VendorName = requestInfo.VendorName;
					RequestNumber = requestInfo.RequestNumber;
					CreationDate = requestInfo.CreationDate.ToString();
					RequestedAmountNumber = requestInfo.RequestedAmountNumber.ToString() + " (" + Utils.Utils.NumberALetter(requestInfo.RequestedAmountNumber) + ")"; ;
					InterestRate = requestInfo.InterestRate.ToString();
					Purpose = requestInfo.Purpose;
					TimePeriod = requestInfo.TimePeriod.ToString() + " Quincenas";
				}
			}
		}

		public void GetPersonalInformation()
		{
			using (sgscEntities db = new sgscEntities())
			{
				var creditRequest = (from cr in db.CreditRequests
									 where cr.CreditRequestId == IdCreditRequest
									 join cus in db.Customers on cr.CustomerId equals cus.CustomerId
									 join cc in db.CustomerContactInfoes on cus.CustomerId equals cc.CustomerId
									 join ca in db.CustomerAddresses on cus.CustomerId equals ca.CustomerId
									 select new
									 {
										 Name = cus.Name,
										 FirstSurname = cus.FirstSurname,
										 SecondSurname = cus.SecondSurname,
										 DateOfBirthay = cus.BirthDate,
										 Gender = cus.Genre,
										 Curp = cus.Curp,
										 Email = cc.Email,
										 MaritalStatus = cus.CivilStatus,
										 PhoneOne = cc.PhoneNumber1,
										 PhoneTwo = cc.PhoneNumber2,
										 FileNumber = cr.FileNumber,
										 Addresses = (from addressCustomer in db.CustomerAddresses
													  where addressCustomer.CustomerId == cus.CustomerId
													  select new
													  {
														  addressCustomer.Street,
														  addressCustomer.ZipCode,
														  addressCustomer.ExternalNumber,
														  addressCustomer.InternalNumber,
														  addressCustomer.Colony,
														  addressCustomer.State,
														  addressCustomer.Type
													  }).ToList()
									 }).FirstOrDefault();

				if (creditRequest == null)
				{
					ToastNotification notification = new ToastNotification("No se ha encontrado la solicitud, inténtelo más tarde", "Error");
					return;
				}
				Name = creditRequest.Name;
				FirstSurname = creditRequest.FirstSurname;
				SecondSurname = creditRequest.SecondSurname;
				DateOfBirthay = creditRequest.DateOfBirthay.ToString();
				Gender = DefineGender(creditRequest.Gender);
				Curp = creditRequest.Curp;
				Email = creditRequest.Email;
				MaritalStatus = DefineMaritalStatus((Customer.CivilStatuses)creditRequest.MaritalStatus);

				var address = creditRequest.Addresses.FirstOrDefault();
				if (address != null)
				{
					Street = address.Street;
					ExternalNumber = address.ExternalNumber;
					InternalNumber = address.InternalNumber;
					State = address.State;
					Colony = address.Colony;
					ZipCode = address.ZipCode;
					AddressType = DefineTypeAddress(address.Type);
				}

				PhoneOne = creditRequest.PhoneOne;
				PhoneTwo = creditRequest.PhoneTwo;

			}
		}

		public string DefineMaritalStatus(Customer.CivilStatuses maritalStatus)
		{
			string statusMarital = "";
			switch (maritalStatus)
			{
				case Customer.CivilStatuses.Single:
					statusMarital = "Soltero(a)";
					break;
				case Customer.CivilStatuses.Married:
					statusMarital = "Casado(a)";
					break;
				case Customer.CivilStatuses.Divorced:
					statusMarital = "Divorciado(a)";
					break;
				case Customer.CivilStatuses.Widowed:
					statusMarital = "Viudo(a)";
					break;
				case Customer.CivilStatuses.Concubinage:
					statusMarital = "Unión libre";
					break;
			}
			return statusMarital;
		}


		public string DefineGender(string gender)
		{
			string sex = "";
			switch (gender.ToLower())
			{
				case "femenino":
					sex = "Femenino";
					break;
				case "masculino":
					sex = "Masculino";
					break;
			}
			return sex;
		}

		public string DefineTypeAddress(int typeAddress)
		{
			string AddressType = "";
			Utils.AddressCustomer.GetTypeAddress(typeAddress);

			switch (Utils.AddressCustomer.GetTypeAddress(typeAddress))
			{
				case "Propietario":
					AddressType = "Propietario";
					break;
				case "Hipotecado":
					AddressType = "Hipotecado";
					break;
				case "Alquiler":
					AddressType = "Alquiler";
					break;
				case "Familiar":
					AddressType = "Familiar";
					break;
			}
			return AddressType;
		}

		public void GetWorkCenterInformation()
		{
			using (sgscEntities db = new sgscEntities())
			{
				var request = db.CreditRequests.Find(IdCreditRequest);
				var workCenterQuery = db.WorkCenters.Where(wc => wc.CustomerId == request.CustomerId).FirstOrDefault();


				if (workCenterQuery == null)
				{
					ToastNotification notification = new ToastNotification("No se ha encontrado la solicitud, inténtelo más tarde", "Error");
					return;
				}

				WorkCenterName = workCenterQuery.CenterName;
				PhoneNumberWorkCenter = workCenterQuery.PhoneNumber;
				InnerNumberWorkCenter = workCenterQuery.InnerNumber.ToString();
				OutsideNumberWorkCenter = workCenterQuery.OutsideNumber.ToString();
				ColonyWorkCenter = workCenterQuery.Colony;
				StreetWorkCenter = workCenterQuery.Street;
				ZipCodeWorkCenter = workCenterQuery.ZipCode.ToString();
			}
		}

		public void ObtainPersonalReferences()
		{
			using (sgscEntities db = new sgscEntities())
			{
				var customerId = db.CreditRequests
									.Where(cr => cr.CreditRequestId == IdCreditRequest)
									.Select(cr => cr.CustomerId)
									.FirstOrDefault();

				if (customerId == 0)
				{
					ToastNotification notification = new ToastNotification("No se ha encontrado la solicitud, inténtelo más tarde", "Error");
					return;
				}

				var contacts = (from c in db.Contacts
								where c.CustomerId == customerId
								select c)
								.Take(2)
								.ToList();

				if (contacts.Any())
				{
					NameReferenceOne = contacts.ElementAtOrDefault(0)?.Name;
					RelationshipReferenceOne = contacts.ElementAtOrDefault(0)?.Relationship;
					PhoneReferenceOne = contacts.ElementAtOrDefault(0)?.PhoneNumber;

					if (contacts.Count > 1)
					{
						NameReferenceTwo = contacts.ElementAtOrDefault(1)?.Name;
						RelationshipReferenceTwo = contacts.ElementAtOrDefault(1)?.Relationship;
						PhoneReferenceTwo = contacts.ElementAtOrDefault(1)?.PhoneNumber;
					}
					else
					{
						ToastNotification notification = new ToastNotification("Solo se encontró una referencia personal para este cliente", "Error");
						notification.Show();
					}
				}
				else
				{
					ToastNotification notification = new ToastNotification("No se encontraron referencias personales para este cliente.", "Error");
					notification.Show();
				}
			}
		}


		private void BtnClicGuardar(object sender, RoutedEventArgs e)
		{
			// Verificar si hay exactamente 3 archivos en el DataGrid
			if (dataGridCreditPolicies.Items.Count != 3)
			{
				System.Windows.MessageBox.Show("Debes agregar exactamente 3 documentos antes de guardar.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			// Guardar los PDFs en la base de datos
			using (sgscEntities db = new sgscEntities())
			{
				foreach (var item in dataGridCreditPolicies.Items)
				{
					dynamic dataItem = item as dynamic;
					if (dataItem != null)
					{
						string fileName = dataItem.FileName;
						byte[] fileContent = dataItem.FileContent;

						if (fileContent != null)
						{
							try
							{
								var newDocument = new Document
								{
									FileName = fileName,
									FileContent = fileContent,
									CreditRequestId = IdCreditRequest // Usa el ID correcto
								};

								db.Documents.Add(newDocument);
							}
							catch (Exception ex)
							{
								System.Windows.MessageBox.Show($"Error al intentar agregar el archivo {fileName} a la base de datos: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
								return;
							}
						}
						else
						{
							System.Windows.MessageBox.Show("El contenido del archivo está vacío.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
						}
					}
				}

				try
				{
					db.SaveChanges();
				}
				catch (Exception ex)
				{
					System.Windows.MessageBox.Show($"Error al intentar guardar los archivos en la base de datos: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
					return;
				}

				var result = System.Windows.Forms.MessageBox.Show("¿Está seguro de que desea enviar la solicitud a revisión?", "Confirmar registro", System.Windows.Forms.MessageBoxButtons.YesNo);
				if (result == System.Windows.Forms.DialogResult.Yes)
				{
					try
					{
						using (sgscEntities context = new sgscEntities())
						{
							var request = context.CreditRequests.Find(IdCreditRequest);
							request.Status = (int)CreditRequest.RequestStatus.Pending;
							context.SaveChanges();
							System.Windows.MessageBox.Show("Solicitud enviada a revisión correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

							App.Current.MainFrame.Content = new ViewCapturedCreditRequests();
						}
					}
					catch (Exception ex)
					{
						System.Windows.MessageBox.Show("Error al enviar la solicitud a revisión: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
					}
				}
			}
		}

		private void BtnClicDescargarSolicitud(object sender, RoutedEventArgs e)
		{
			SaveFileDialog saveFileDialog = new SaveFileDialog
			{
				Filter = "PDF file|*.pdf",
				Title = "Guardar PDF",
				FileName = "solicitud.pdf"
			};

			if (saveFileDialog.ShowDialog() == DialogResult.OK)
			{
				GeneratePdfRequest(saveFileDialog.FileName);
			}
		}

		public void GeneratePdfRequest(string outputPath)
		{
			var font = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10);

			var doc = new ReportDocument(PageSize.A4);
			PdfWriter.GetInstance(doc, new FileStream(outputPath, FileMode.Create));

			doc.Open();

			Paragraph title = new Paragraph("Solicitud de apertura de crédito", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 18, iTextSharp.text.Font.BOLD));
			title.Alignment = Element.ALIGN_CENTER;
			doc.Add(title);

			doc.Add(new Paragraph("\n"));

			PdfPTable table = new PdfPTable(3);
			table.WidthPercentage = 100;

			BaseColor sectionColor = new BaseColor(96, 138, 211);

			void AddSectionHeader(string sectionTitle)
			{
				PdfPCell sectionCell = new PdfPCell(new Phrase(sectionTitle, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12, iTextSharp.text.Font.BOLD)));
				sectionCell.Colspan = 3;
				sectionCell.BackgroundColor = sectionColor;
				sectionCell.HorizontalAlignment = Element.ALIGN_CENTER;
				table.AddCell(sectionCell);
			}

			void AddDataRow(params string[] cellValues)
			{
				foreach (var cellValue in cellValues)
				{
					PdfPCell cell;
					if (cellValue.Contains("Propósito") || cellValue.Contains("Período de Tiempo") || cellValue.Contains("Nombre") ||
						cellValue.Contains("Fecha de Creación") || cellValue.Contains("Cantidad Solicitada") || cellValue.Contains("Tasa de Interés") ||
						cellValue.Contains("Primer Apellido") || cellValue.Contains("Segundo Apellido") || cellValue.Contains("Fecha de Nacimiento") ||
						cellValue.Contains("Género") || cellValue.Contains("CURP") || cellValue.Contains("Email") ||
						cellValue.Contains("Calle") || cellValue.Contains("Número Exterior") || cellValue.Contains("Número Interior") ||
						cellValue.Contains("Estado") || cellValue.Contains("Colonia") || cellValue.Contains("Código Postal") ||
						cellValue.Contains("Tipo de Dirección") || cellValue.Contains("Teléfono 1") || cellValue.Contains("Teléfono 2") ||
						cellValue.Contains("Nombre del Centro Laboral") || cellValue.Contains("Teléfono del Centro Laboral") ||
						cellValue.Contains("Número Interior del Centro Laboral") || cellValue.Contains("Número Exterior del Centro Laboral") ||
						cellValue.Contains("Colonia del Centro Laboral") || cellValue.Contains("Calle del Centro Laboral") ||
						cellValue.Contains("Código Postal del Centro Laboral") || cellValue.Contains("Nombre Referencia 1") ||
						cellValue.Contains("Parentesco Referencia 1") || cellValue.Contains("Teléfono Referencia 1") ||
						cellValue.Contains("Nombre Referencia 2") || cellValue.Contains("Parentesco Referencia 2") || cellValue.Contains("Teléfono Referencia 2"))
					{
						cell = new PdfPCell(new Phrase(cellValue, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10, iTextSharp.text.Font.BOLD)));
					}
					else
					{
						cell = new PdfPCell(new Phrase(cellValue, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10)));
					}
					table.AddCell(cell);
				}
			}

			AddSectionHeader("Solicitud");
			AddDataRow("ID Solicitud", "Nombre del Vendedor", "Número de Solicitud");
			AddDataRow(IdCreditRequest.ToString(), VendorName, RequestNumber);
			AddDataRow("Fecha de Creación", "Cantidad Solicitada", "Tasa de Interés");
			AddDataRow(CreationDate, RequestedAmountNumber, InterestRate);

			AddSectionHeader("Datos Personales");
			AddDataRow("Propósito", "Período de Tiempo", "Nombre");
			AddDataRow(Purpose, TimePeriod, Name);
			AddDataRow("Primer Apellido", "Segundo Apellido", "Fecha de Nacimiento");
			AddDataRow(FirstSurname, SecondSurname, DateOfBirthay);
			AddDataRow("Género", "CURP", "Email");
			AddDataRow(Gender, Curp, Email);

			AddSectionHeader("Domicilio");
			AddDataRow("Calle", "Número Exterior", "Número Interior");
			if(InternalNumber == null)
			{
				InternalNumber = "";
			}
			AddDataRow(Street, ExternalNumber, InternalNumber);
			AddDataRow("Estado", "Colonia", "Código Postal");
			AddDataRow(State, Colony, ZipCode);
			AddDataRow("Tipo de Dirección", "Teléfono 1", "Teléfono 2");
			AddDataRow(AddressType, PhoneOne, PhoneTwo);

			AddSectionHeader("Información Laboral");
			AddDataRow("Nombre del Centro Laboral", "Teléfono del Centro Laboral", "");
			AddDataRow(WorkCenterName, PhoneNumberWorkCenter, "");
			AddDataRow("Número Interior del Centro Laboral", "Número Exterior del Centro Laboral", "Colonia del Centro Laboral");
			AddDataRow(InnerNumberWorkCenter, OutsideNumberWorkCenter, ColonyWorkCenter);
			AddDataRow("Calle del Centro Laboral", "Código Postal del Centro Laboral", "");
			AddDataRow(StreetWorkCenter, ZipCodeWorkCenter, "");

			AddSectionHeader("Referencias Personales");
			AddDataRow("Nombre Referencia 1", "Parentesco Referencia 1", "Teléfono Referencia 1");
			AddDataRow(NameReferenceOne, RelationshipReferenceOne, PhoneReferenceOne);
			AddDataRow("Nombre Referencia 2", "Parentesco Referencia 2", "Teléfono Referencia 2");
			AddDataRow(NameReferenceTwo, RelationshipReferenceTwo, PhoneReferenceTwo);

			doc.Add(table);

			doc.Add(new Paragraph("\n"));
			doc.Add(new Paragraph("Yo, " + VendorName + " , en mi capacidad como asesor financiero autorizado por SGSC, certifico que he revisado y verificado la información proporcionada en esta solicitud de crédito en nombre de mi cliente," + Name + " " + FirstSurname + " " + SecondSurname +
				" Declaro que, según mi leal saber y entender, la información proporcionada es precisa y completa en todos los aspectos relevantes.", font));

			doc.Add(new Paragraph("\n"));
			doc.Add(new Paragraph(""));
			doc.Add(new Paragraph("Firma Asesor de Crédito"));
			doc.Add(new Paragraph("\n"));
			doc.Add(new Paragraph(""));
			doc.Add(new Paragraph("Firma Solicitante"));
			doc.Close();
		}

		private void BtnClicDescargarCaratula(object sender, RoutedEventArgs e)
		{
			SaveFileDialog saveFileDialog = new SaveFileDialog
			{
				Filter = "PDF file|*.pdf",
				Title = "Guardar PDF",
				FileName = "Solicitud_de_apertura_de_crédito.pdf"
			};

			if (saveFileDialog.ShowDialog() == DialogResult.OK)
			{
				GeneratePdfCover(saveFileDialog.FileName);
			}
		}


		/// <summary>
		/// FALTA ESTOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO
		/// </summary>
		public void GeneratePdfCover(string outputPath)
		{
			var doc = new ReportDocument(PageSize.A4);
			PdfWriter.GetInstance(doc, new FileStream(@outputPath, FileMode.Create));

			doc.Open();

			PdfPTable table = new PdfPTable(3);
			table.WidthPercentage = 100;

			table.AddCell("ID Solicitud");
			table.AddCell("Nombre del Vendedor");
			table.AddCell("Número de Solicitud");

			table.AddCell(IdCreditRequest.ToString());
			table.AddCell(VendorName);
			table.AddCell(RequestNumber);

			table.AddCell("Fecha de Creación");
			table.AddCell("Cantidad Solicitada");
			table.AddCell("Tasa de Interés");

			table.AddCell(CreationDate);
			table.AddCell(RequestedAmountNumber);
			table.AddCell(InterestRate);

			table.AddCell("Propósito");
			table.AddCell("Período de Tiempo");
			table.AddCell("Nombre");

			table.AddCell(Purpose);
			table.AddCell(TimePeriod);
			table.AddCell(Name);

			table.AddCell("Primer Apellido");
			table.AddCell("Segundo Apellido");
			table.AddCell("Fecha de Nacimiento");

			table.AddCell(FirstSurname);
			table.AddCell(SecondSurname);
			table.AddCell(DateOfBirthay);

			table.AddCell("Género");
			table.AddCell("CURP");
			table.AddCell("Email");

			table.AddCell(Gender);
			table.AddCell(Curp);
			table.AddCell(Email);

			table.AddCell("Calle");
			table.AddCell("Número Exterior");
			table.AddCell("Número Interior");

			table.AddCell(Street);
			table.AddCell(ExternalNumber);
			table.AddCell(InternalNumber);

			table.AddCell("Estado");
			table.AddCell("Colonia");
			table.AddCell("Código Postal");

			table.AddCell(State);
			table.AddCell(Colony);
			table.AddCell(ZipCode);

			table.AddCell("Tipo de Dirección");
			table.AddCell("Teléfono 1");
			table.AddCell("Teléfono 2");

			table.AddCell(AddressType);
			table.AddCell(PhoneOne);
			table.AddCell(PhoneTwo);

			table.AddCell("Estado Civil");
			table.AddCell("Nombre del Centro Laboral");
			table.AddCell("Teléfono del Centro Laboral");

			table.AddCell(MaritalStatus);
			table.AddCell(WorkCenterName);
			table.AddCell(PhoneNumberWorkCenter);

			table.AddCell("Número Interior del Centro Laboral");
			table.AddCell("Número Exterior del Centro Laboral");
			table.AddCell("Colonia del Centro Laboral");

			table.AddCell(InnerNumberWorkCenter);
			table.AddCell(OutsideNumberWorkCenter);
			table.AddCell(ColonyWorkCenter);

			table.AddCell("Calle del Centro Laboral");
			table.AddCell("Código Postal del Centro Laboral");
			table.AddCell("");

			table.AddCell(StreetWorkCenter);
			table.AddCell(ZipCodeWorkCenter);
			table.AddCell("");

			table.AddCell("Nombre Referencia 1");
			table.AddCell("Parentesco Referencia 1");
			table.AddCell("Teléfono Referencia 1");

			table.AddCell(NameReferenceOne);
			table.AddCell(RelationshipReferenceOne);
			table.AddCell(PhoneReferenceOne);

			table.AddCell("Nombre Referencia 2");
			table.AddCell("Parentesco Referencia 2");
			table.AddCell("Teléfono Referencia 2");

			table.AddCell(NameReferenceTwo);
			table.AddCell(RelationshipReferenceTwo);
			table.AddCell(PhoneReferenceTwo);

			doc.Add(table);

			doc.Close();
		}

		private void BtnClicDescargarDomicializacion(object sender, RoutedEventArgs e)
		{

		}



		private void BtnClicSubirDocumentos(object sender, RoutedEventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Multiselect = true; // Permitir selección múltiple
			openFileDialog.Filter = "PDF files|*.pdf";

			// Mostrar el diálogo de apertura de archivo
			DialogResult result = openFileDialog.ShowDialog();

			// Verificar si el usuario seleccionó algún archivo
			if (result == DialogResult.OK)
			{
				// Número de archivos ya presentes en el DataGrid
				int currentFilesCount = dataGridCreditPolicies.Items.Count;

				// Número de archivos que se intentan agregar
				int selectedFilesCount = openFileDialog.FileNames.Length;

				// Verificar si el total de archivos excede el límite de 3
				if (currentFilesCount + selectedFilesCount > 3)
				{
					System.Windows.MessageBox.Show("No puedes agregar más de 3 documentos en total.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
					return; // Salir del método sin agregar ningún archivo
				}

				foreach (string filePath in openFileDialog.FileNames)
				{
					// Verificar el tamaño del archivo
					if (new FileInfo(filePath).Length > MaxFileSize)
					{
						System.Windows.MessageBox.Show($"El archivo {Path.GetFileName(filePath)} excede el tamaño máximo permitido de 10MB.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
						return;
					}

					// Leer el contenido del archivo
					byte[] fileContent = File.ReadAllBytes(filePath);

					// Agregar el archivo al DataGrid con el contenido
					dataGridCreditPolicies.Items.Add(new
					{
						FileName = Path.GetFileName(filePath),
						FileContent = fileContent
					});
				}
			}
			else
			{
				Console.WriteLine("No se seleccionó ningún archivo.");
			}
		}

		private void BtnClicFileDelete(object sender, RoutedEventArgs e)
		{
			var selectedItem = dataGridCreditPolicies.SelectedItem;
			if (selectedItem != null)
			{
				dataGridCreditPolicies.Items.Remove(selectedItem);
			}
			else
			{
				System.Windows.MessageBox.Show("Por favor, selecciona una fila para eliminar.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void BtnClicDownloadDomicialization(object sender, RoutedEventArgs e)
		{
			SaveFileDialog saveFileDialog = new SaveFileDialog
			{
				Filter = "PDF file|*.pdf",
				Title = "Guardar PDF",
				FileName = "domiciliacion.pdf"
			};

			if (saveFileDialog.ShowDialog() == DialogResult.OK)
			{
				GeneratePdfDomicialization(saveFileDialog.FileName); // Pasa la ruta seleccionada por el usuario
			}
		}

		public void GeneratePdfDomicialization(string outputPath)
		{
			var font = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10);
			var doc = new ReportDocument(PageSize.A4);
			PdfWriter.GetInstance(doc, new FileStream(outputPath, FileMode.Create));

			doc.Open();
			BaseColor sectionColor = new BaseColor(96, 138, 211);

			Paragraph title = new Paragraph("Autorización para domiciliación de pagos", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 14, iTextSharp.text.Font.BOLD));
			title.Alignment = Element.ALIGN_CENTER;
			doc.Add(title);

			doc.Add(new Paragraph("Fecha:" + CreationDate, font));

			doc.Add(new Paragraph("Solicito y autorizo que con base en la información que se indica en esta comunicación se realicen cargos periódicos en mi cuenta conforme a lo siguiente:", font));
			doc.Add(new Paragraph("\n"));
			doc.Add(new Paragraph("1. Nombre del proveedor del crédito que pretende pagarse:", font));
			doc.Add(new Paragraph("2. Crédito a pagar: Contrato de Apertura de crédito", font));
			doc.Add(new Paragraph("3. Periocidad del pago (Facturación): " + TimePeriod, font));
			doc.Add(new Paragraph("4. Nombre del banco que lleva la cuenta de depósito a la vista o de ahorro en la que se realizará el cargo", font));
			doc.Add(new Paragraph("5. Cualquiera de los datos de identificación son los siguientes:", font));

			doc.Add(new Paragraph("Cuenta de transferencia", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12, iTextSharp.text.Font.BOLD)));
			PdfPTable transferTable = new PdfPTable(3);
			transferTable.WidthPercentage = 100;
			transferTable.AddCell(GetCellWithColor("Nombre del banco", sectionColor, font));
			transferTable.AddCell(GetCellWithColor("Clabe", sectionColor, font));
			transferTable.AddCell(GetCellWithColor("Número de tarjeta", sectionColor, font));
			transferTable.AddCell(NameBankAccountTransfer);
			transferTable.AddCell(InterbankCodeAccountTransfer);
			transferTable.AddCell(NumberAccountTransfer);
			doc.Add(transferTable);

			doc.Add(new Paragraph("\n"));

			doc.Add(new Paragraph("Cuenta de domicialización", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12, iTextSharp.text.Font.BOLD)));
			PdfPTable domicializationTable = new PdfPTable(3);
			domicializationTable.WidthPercentage = 100;
			domicializationTable.AddCell(GetCellWithColor("Nombre del banco", sectionColor, font));
			domicializationTable.AddCell(GetCellWithColor("Clabe", sectionColor, font));
			domicializationTable.AddCell(GetCellWithColor("Número de tarjeta", sectionColor, font));
			domicializationTable.AddCell(NameDomicialization);
			domicializationTable.AddCell(InterbankCodeDomicialization);
			domicializationTable.AddCell(NumberAccountDomicialization);
			doc.Add(domicializationTable);

			doc.Add(new Paragraph("\n"));
			doc.Add(new Paragraph("Estoy enterado de que en cualquier momento podré solicitar la cancelación de la presente domiciliación sin costo a mi cargo.", font));
			doc.Add(new Paragraph("\nAtentamente", font));
			doc.Add(new Paragraph("", font));
			doc.Add(new Paragraph("Nombre del titular de la cuenta", font));
			doc.Add(new Paragraph("\n__", font));
			doc.Add(new Paragraph("Firma del titular de la cuenta", font));

			doc.Close();
		}
		private PdfPCell GetCellWithColor(string content, BaseColor color, iTextSharp.text.Font font)
		{
			PdfPCell cell = new PdfPCell(new Phrase(content, font));
			cell.BackgroundColor = color;
			return cell;
		}

		private void btnCancel_Click(object sender, RoutedEventArgs e)
		{
			var result = System.Windows.Forms.MessageBox.Show("Está seguro que desea cancelar el registro?\nPuede retomarlo desde este punto más tarde.", "Cancelar registro", System.Windows.Forms.MessageBoxButtons.YesNo);
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