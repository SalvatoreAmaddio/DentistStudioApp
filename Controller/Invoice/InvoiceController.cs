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
        public InvoiceController() 
        {
            TreatmentsToInvoice.NotifyParentControllerEvent += OnNotifyParentEvent;
            TreatmentsInvoiced.NotifyParentControllerEvent += OnNotifyParentEvent;
            AddSubControllers(TreatmentsToInvoice);
            AddSubControllers(TreatmentsInvoiced);
            NewRecordEvent += OnNewRecordEvent;
            OpenPaymentWindowCMD = new CMD(OpenPaymentWindow);
        }

        public InvoiceController(Patient patient) : this()
        {
            GoNew();
            Patient = patient;
            CurrentRecord?.Dirt();
        }

        public InvoiceController(Invoice invoice) : this() 
        {
           FetchPatientFromInvoicedTreatment fetchPatient = new ();
           GoAt(invoice);
           Patient = (Patient?)fetchPatient.Convert(invoice, null, null, null);
        }

        private void OpenPaymentWindow() 
        {
            Helper.OpenWindowDialog("Payment Methods", new PaymentTypeList());
        }
        private void OnNewRecordEvent(object? sender, AllowRecordMovementArgs e)
        {
            if (TreatmentsToInvoice.Source.Count == 0) 
            {
                Failure.Allert("No more treatments to invoice");
                GoPrevious();
                e.Cancel = true;
            }
        }

        private async void OnNotifyParentEvent(object? sender, EventArgs e)
        {
            Task t1 = TreatmentsToInvoice.RequeryAsync();
            Task t2 = TreatmentsInvoiced.RequeryAsync();
            await Task.WhenAll(t1, t2);
        }

        public override AbstractClause InstantiateSearchQry()
        {
            return new Invoice().From();
        }
    }
}