using Backend.Database;
using Backend.ExtensionMethods;
using Backend.Model;
using DentistStudioApp.Converters;
using DentistStudioApp.Model;
using DentistStudioApp.View;
using FrontEnd.Controller;
using FrontEnd.Dialogs;
using FrontEnd.Events;
using FrontEnd.Source;
using FrontEnd.Utils;
using System.Windows.Input;

namespace DentistStudioApp.Controller
{
    public class InvoiceController : AbstractFormController<Invoice>
    {
        private FetchPatientFromInvoicedTreatment _fetchPatient = new();

        private Patient? _patient;
        public Patient? Patient
        { 
            get => _patient;
            set
            {
                UpdateProperty(ref value, ref _patient);
                TreatmentsToInvoice.Patient = value;
                TreatmentsInvoiced.Patient = value;
            }
        }

        public TreatmentToInvoiceListController TreatmentsToInvoice { get; } = new();
        public TreatmentInvoicedListController TreatmentsInvoiced { get; } = new();
        public override int DatabaseIndex => 12;
        public RecordSource<PaymentType> PaymentTypes { get; private set; } = new(DatabaseManager.Find<PaymentType>()!);
        public ICommand OpenPaymentWindowCMD { get; }
        internal InvoiceController()
        {
            AfterUpdate += OnAfterUpdate;
            AddSubControllers(TreatmentsToInvoice);
            AddSubControllers(TreatmentsInvoiced);
            TreatmentsToInvoice.NotifyParentControllerEvent += OnNotifyParentEvent;
            TreatmentsInvoiced.NotifyParentControllerEvent += OnNotifyParentEvent;
            OpenPaymentWindowCMD = new CMD(OpenPaymentWindow);
        }

        private void OnAfterUpdate(object? sender, AfterUpdateArgs e)
        {
            if (e.Is(nameof(Patient))) 
            {
                TreatmentsToInvoice.Patient = Patient;
                TreatmentsInvoiced.Patient = Patient;
            }
        }

        public InvoiceController(Patient patient) : this()
        {
            GoNew();
            Patient = patient;
            CurrentRecord?.Dirt();
            RecordMovingEvent += OnRecordMoving;
        }

        public InvoiceController(Invoice invoice) : this() 
        {
            GoAt(invoice);
            Patient = _fetchPatient.Convert(invoice);
            AllowNewRecord = false;
            RecordMovingEvent += OnRecordMoving;
        }

        private void OpenPaymentWindow() => Helper.OpenWindowDialog("Payment Methods", new PaymentTypeList());
        private void OnRecordMoving(object? sender, AllowRecordMovementArgs e) => Patient = _fetchPatient.Convert(CurrentRecord);

        private async void OnNotifyParentEvent(object? sender, EventArgs e)
        {
            Task t1 = TreatmentsToInvoice.RequeryAsync();
            Task t2 = TreatmentsInvoiced.RequeryAsync();
            await Task.WhenAll(t1, t2);
        }

        public override AbstractClause InstantiateSearchQry() => new Invoice().Select().From();
    }
}