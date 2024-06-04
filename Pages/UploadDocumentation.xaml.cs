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

        public UploadDocumentation(int idCreditRequest)
        {
            this.IdCreditRequest = idCreditRequest;
            InitializeComponent();
            GetRequestInfo();
            GetPersonalInformation();
            GetWorkCenterInformation();
            ObtainPersonalReferences();
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
                var workCenterQuery = (from cr in db.CreditRequests
                                       join wc in db.WorkCenters on cr.CustomerId equals wc.Customers.CustomerId
                                       where cr.CreditRequestId == IdCreditRequest
                                       select new
                                       {
                                           WorkCenterName = wc.CenterName,
                                           PhoneNumber = wc.PhoneNumber,
                                           Street = wc.Street,
                                           Colony = wc.Colony,
                                           InnerNumber = wc.InnerNumber,
                                           OutsideNumber = wc.OutsideNumber,
                                           ZipCode = wc.ZipCode,
                                           FileNumber = cr.FileNumber
                                       }).FirstOrDefault();
                if (workCenterQuery == null)
                {
                    ToastNotification notification = new ToastNotification("No se ha encontrado la solicitud, inténtelo más tarde", "Error");
                    return;
                }

                WorkCenterName = workCenterQuery.WorkCenterName;
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
                                var newDocument = new Documents
                                {
                                    FileName = fileName,
                                    FileContent = fileContent,
                                    CreditRequestId = IdCreditRequest, // Usa el ID correcto
                                    CreditRequest_CreditRequestId = IdCreditRequest // Usa el ID correcto
                                };

                                db.Documents.Add(newDocument);
                            }
                            catch (Exception ex)
                            {
                                System.Windows.MessageBox.Show($"Error al intentar agregar el archivo {fileName} a la base de datos: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                    System.Windows.MessageBox.Show("Archivos guardados correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"Error al intentar guardar los archivos en la base de datos: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnClicDescargarSolicitud(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "PDF file|*.pdf",
                Title = "Guardar PDF",
                FileName = "Solicitud_de_apertura_de_crédito.pdf"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                GeneratePdfRequest();
            }
        }

        public void GeneratePdfRequest()
        {
            string outputPath = @"C:\Users\DMS19\OneDrive\Escritorio\CreditRequest.pdf";

            Document doc = new Document(PageSize.A4);
            PdfWriter.GetInstance(doc, new FileStream(outputPath, FileMode.Create));

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
                GeneratePdfCover();
            }
        }


        /// <summary>
        /// FALTA ESTOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO
        /// </summary>
        public void GeneratePdfCover()
        {
            string outputPath = @"C:\Users\DMS19\OneDrive\Escritorio\Cover.pdf";

            Document doc = new Document(PageSize.A4);
            PdfWriter.GetInstance(doc, new FileStream(outputPath, FileMode.Create));

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
            openFileDialog.Filter = "Archivos PDF|*.pdf|Imágenes|*.jpg;*.jpeg;*.png;*.gif";

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
    }
}
