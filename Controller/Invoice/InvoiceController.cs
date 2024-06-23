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
using System.Windows;
using System.Windows.Input;

namespace DentistStudioApp.Controller
{
    public class InvoiceController : AbstractFormController<Invoice>
    {
        #region Variables
        private readonly FetchPatientFromInvoicedTreatment _fetchPatient = new();
        private Patient? _patient;
        #endregion

        #region Properties
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
        #endregion

        #region Constructor
        internal InvoiceController()
        {
            AfterUpdate += OnAfterUpdate;
            AddSubControllers(TreatmentsToInvoice);
            AddSubControllers(TreatmentsInvoiced);
            TreatmentsToInvoice.NotifyParentControllerEvent += OnNotifyParentEvent;
            TreatmentsInvoiced.NotifyParentControllerEvent += OnNotifyParentEvent;
            OpenPaymentWindowCMD = new CMD(OpenPaymentWindow);
        }

        public InvoiceController(Patient patient) : this()
        {
            GoNew();
            Patient = patient;
            CurrentRecord?.Dirt();
            RecordMovingEvent += OnNewRecord;
        }

        public InvoiceController(Invoice invoice) : this()
        {
            GoAt(invoice);
            Patient = _fetchPatient.Convert(invoice);
            AllowNewRecord = false;
            RecordMovingEvent += OnRecordMoving;
        }
        #endregion

        #region Event Subscriptions
        private void OnAfterUpdate(object? sender, AfterUpdateArgs e)
        {
            if (e.Is(nameof(Patient))) 
            {
                TreatmentsToInvoice.Patient = Patient;
                TreatmentsInvoiced.Patient = Patient;
            }
        }
        private async void OnNewRecord(object? sender, AllowRecordMovementArgs e)
        {
            if (!e.NewRecord) return;
            if (Patient == null) throw new NullReferenceException();
            long? result = await Patient.TreatmentCount();
            if (result == 0)
            {
                Failure.Allert("There are no more treatments to invoice.");
                e.Cancel = true;
            }
        }
        private void OnRecordMoving(object? sender, AllowRecordMovementArgs e) 
        {
            if (CurrentRecord == null || CurrentRecord.IsNewRecord()) return;
            Patient = _fetchPatient.Convert(CurrentRecord);
        }
        private async void OnNotifyParentEvent(object? sender, EventArgs e)
        {
            Task t1 = TreatmentsToInvoice.RequeryAsync();
            Task t2 = TreatmentsInvoiced.RequeryAsync();
            await Task.WhenAll(t1, t2);
        }
        #endregion
        private void OpenPaymentWindow() => Helper.OpenWindowDialog("Payment Methods", new PaymentTypeList());
        public override AbstractClause InstantiateSearchQry() => new Invoice().Select().From();
    }
}