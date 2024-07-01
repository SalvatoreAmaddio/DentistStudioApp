using DentistStudioApp.Model;
using FrontEnd.Controller;
using FrontEnd.Events;
using Backend.ExtensionMethods;
using FrontEnd.Source;
using Backend.Database;
using FrontEnd.FilterSource;
using Backend.Model;
using DentistStudioApp.View;
using System.Windows.Input;
using Backend.Utils;
using FrontEnd.Reports;
using System.Windows.Controls;
using DentistStudioApp.Converters;
using FrontEnd.Dialogs;

namespace DentistStudioApp.Controller
{
    public class InvoiceListController : AbstractFormListController<Invoice>
    {
        private readonly long? _patientID = null;
        public RecordSource<PaymentType> PaymentTypes { get; private set; } = new(DatabaseManager.Find<PaymentType>()!);
        public SourceOption PaymentTypesOptions { get; private set; }
        public SourceOption PaidOptions { get; private set; }
        public SourceOption DatesOptions { get; private set; }
        public ICommand OpenInvoiceCMD { get; }
        public InvoiceListController()
        {
            AfterUpdate += OnAfterUpdate;
            OpenInvoiceCMD = new CMD<Invoice>(OpenInvoice);
            PaymentTypesOptions = new(PaymentTypes, "PaymentBy");
            PaidOptions = new PrimitiveSourceOption(this, "Paid");
            DatesOptions = new PrimitiveSourceOption(this, "DOI");
            AllowNewRecord = false;
        }
        
        public InvoiceListController(long patientID) : this() 
        {
            WindowLoaded += OnWindowLoaded;
            _patientID = patientID;
        }

        public static async void OpenInvoice(Invoice invoice)
        {
            if (invoice.IsNewRecord() || invoice.IsDirty) 
            {
                Failure.Allert("Please save the invoice first.");
                return;
            }

            Patient? patient = new FetchPatientFromInvoicedTreatment().Convert(invoice);

            if (patient == null)
            {
                Failure.Allert("Patient cannot be null!");
                return;
            }

            if (invoice.DOI == null)
            {
                Failure.Allert("Date of invoice is missing.");
                return;
            }

            Task<IEnumerable<AppointmentServices>> servicesTask = AppointmentServices.GetInvoicedServices(invoice.InvoiceID);

            ReportViewerWindow win = new()
            {
                FileName = $"{patient.FirstName}_{patient.LastName}_Invoice_{invoice.DOI.Value.Month}_{invoice.DOI.Value.Year}",
            };

            IEnumerable<AppointmentServices> services = await servicesTask;

            win.AddPage(new InvoicePage(invoice, patient, services));
            win.SelectedPage = win[0];
            win.Show();
        }

        private async void OnWindowLoaded(object? sender, System.Windows.RoutedEventArgs e)
        {
            IEnumerable<Invoice> results = await SearchRecordAsync();
            RecordSource.ReplaceRange(results);
            GoFirst();
        }

        private async void OnAfterUpdate(object? sender, AfterUpdateArgs e)
        {
            if (e.Is(nameof(Search)))
            {
                await OnSearchPropertyRequeryAsync(sender);
            }
        }

        public override void OnOptionFilterClicked(FilterEventArgs e)
        {
            ReloadSearchQry();
            PaymentTypesOptions.Conditions<WhereClause>(SearchQry);
            PaidOptions.Conditions<WhereClause>(SearchQry);
            DatesOptions.Conditions<WhereClause>(SearchQry);
            OnAfterUpdate(e, new(null, null, nameof(Search)));
        }

        public override async Task<IEnumerable<Invoice>> SearchRecordAsync()
        {
            SearchQry.AddParameter("name", Search.ToLower() + "%");
            SearchQry.AddParameter("name", Search.ToLower() + "%");
            FilterByPatient();
            return await CreateFromAsyncList(SearchQry.Statement(), SearchQry.Params());
        }
        private void FilterByPatient()
        {
            if (_patientID == null) return;
            SearchQry?.GetClause<WhereClause>()?.AND().EqualsTo("Patient.PatientID", "@patientID");
            SearchQry?.AddParameter("patientID", $"{_patientID}");
        }

        protected override void Open(Invoice model)
        {
            InvoiceForm invoideForm = new(new(model, _patientID));
            invoideForm.ShowDialog();
        }

        public override AbstractClause InstantiateSearchQry() =>
        new InvoicedTreatment()
             .Select("Invoice.*")
             .From()
             .InnerJoin("Treatment", "TreatmentID")
             .InnerJoin(new Invoice())
             .InnerJoin(nameof(Invoice), nameof(PaymentType), "PaymentTypeID")
             .InnerJoin(nameof(Treatment), nameof(Patient), "PatientID")
             .Where()
                 .OpenBracket()
                     .Like("LOWER(Patient.FirstName)", "@name")
                     .OR()
                     .Like("LOWER(Patient.LastName)", "@name")
                 .CloseBracket()
             .OrderBy().Field("DOI DESC").Field("Invoice.InvoiceID DESC");
    }
}