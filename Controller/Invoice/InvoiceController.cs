using Backend.Database;
using Backend.Events;
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
using System.ComponentModel;
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
        public RecordSource<PaymentType> PaymentTypes { get; private set; } = new(DatabaseManager.Find<PaymentType>()!);
        public ICommand OpenPaymentWindowCMD { get; }
        #endregion

        #region Constructor
        internal InvoiceController()
        {
            AfterUpdate += OnAfterUpdate;
            AddSubControllers(TreatmentsToInvoice);
            AddSubControllers(TreatmentsInvoiced);
            TreatmentsToInvoice.NotifyParentController += OnNotifyParentEvent;
            TreatmentsInvoiced.NotifyParentController += OnNotifyParentEvent;
            OpenPaymentWindowCMD = new CMD(OpenPaymentWindow);
            WindowClosing += OnWindowClosing;
            BeforeRecordNavigation += OnBeforeRecordNavigation;
        }

        public InvoiceController(Patient patient) : this()
        {
            GoNew();
            Patient = patient;
            CurrentRecord?.Dirt();
            AfterRecordNavigation += OnNewRecord;
            WindowLoaded += OnWindowLoaded;
        }

        public InvoiceController(Invoice invoice, long? patientID = null) : this()
        {
            GoAt(invoice);
            Patient = _fetchPatient.Convert(invoice);
            AllowNewRecord = false;
            AfterRecordNavigation += OnRecordMoving;
            if (patientID != null) 
            {
                AfterRecordNavigation -= OnRecordMoving;
                AfterRecordNavigation += OnNewRecord;
                WindowLoaded += OnWindowLoaded;
                AllowNewRecord = true;
            }
        }
        #endregion

        protected override bool Update(Invoice? model)
        {
            BeforeRecordNavigation -= OnBeforeRecordNavigation;
            bool result = base.Update(model);
            BeforeRecordNavigation += OnBeforeRecordNavigation;
            return result;
        }

        public override bool PerformUpdate()
        {
            if (TreatmentsInvoiced.Source.Count == 0)
            {
                MessageBox.Show("Cannot save an invoice without adding at least one treatment", "Action Denied");
                return false;
            }
            return base.PerformUpdate();
        }

        #region Event Subscriptions
        private void OnBeforeRecordNavigation(object? sender, AllowRecordMovementArgs e)
        {
            if (CurrentRecord!=null && CurrentRecord.IsNewRecord()) return;
            if (TreatmentsInvoiced.Source.Count == 0)
            {
                Failure.Allert("You must invoice at least one treatment.");
                e.Cancel = true;
            }
        }

        private void OnWindowClosing(object? sender, CancelEventArgs e)
        {
            if (TreatmentsInvoiced.Source.Count == 0)
            {
                DialogResult result = UnsavedDialog.Ask("You must invoice at least one treatment. Alternatively, would like to delete this invoice?");
                if (result == DialogResult.Yes)
                {
                    if (CurrentRecord!=null && CurrentRecord.IsNewRecord()) 
                    {
                        CurrentRecord.IsDirty = false;
                        return;
                    }

                    BeforeRecordNavigation -= OnBeforeRecordNavigation;
                    Delete(CurrentRecord);
                    return;
                }
                e.Cancel = true;
            }
        }

        private async void OnWindowLoaded(object? sender, RoutedEventArgs e)
        {
            SearchQry.GetClause<FromClause>()?.InnerJoin(nameof(InvoicedTreatment), "InvoiceID")
                                              .InnerJoin(nameof(InvoicedTreatment), nameof(Treatment), "TreatmentID")
                                              .Where().EqualsTo("PatientID", "@patientID")
                                              .OrderBy().Field("DOI DESC").Field("InvoiceID DESC");

            SearchQry.AddParameter("patientID", Patient?.PatientID);
            RecordSource<Invoice> results = await Task.Run(() => CreateFromAsyncList(SearchQry.Statement(), SearchQry.Params()));
            RecordSource.ReplaceRange(results);
            GoAt(CurrentRecord);
        }

        private void OnAfterUpdate(object? sender, AfterUpdateArgs e)
        {
            if (e.Is(nameof(Patient))) 
            {
                TreatmentsToInvoice.Patient = Patient;
                TreatmentsInvoiced.Patient = Patient;
                return;
            }

            if (e.Is(nameof(CurrentRecord)) && CurrentRecord != null && CurrentRecord.IsNewRecord()) 
            {
                CurrentRecord.Dirt();
            }
        }

        private async void OnNewRecord(object? sender, AllowRecordMovementArgs e)
        {
            if (!e.NewRecord) return;

            if (Patient == null) throw new NullReferenceException();
            long? result = await Patient.TreatmentCount();
            if (result == 0 || result is null)
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