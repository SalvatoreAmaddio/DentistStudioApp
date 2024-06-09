﻿using Backend.Database;
using Backend.Source;
using DentistStudioApp.Model;
using FrontEnd.Controller;
using FrontEnd.Dialogs;
using FrontEnd.Events;

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
        public RecordSource PaymentTypes { get; private set; } = new(DatabaseManager.Find<PaymentType>()!);

        public InvoiceController() 
        {
            TreatmentsToInvoice.NotifyParentEvent += OnNotifyParentEvent;
            TreatmentsInvoiced.NotifyParentEvent += OnNotifyParentEvent;
            AddSubControllers(TreatmentsToInvoice);
            AddSubControllers(TreatmentsInvoiced);
            NewRecordEvent += OnNewRecordEvent;
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
    }
}