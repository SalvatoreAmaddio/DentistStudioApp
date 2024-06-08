using Backend.Database;
using Backend.Source;
using DentistStudioApp.Model;
using FrontEnd.Controller;

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
                Treatments.Patient = value;
                Treatments2.Patient = value;
            }
        }
        public TreatmentToInvoiceListController Treatments { get; } = new();
        public TreatmentInvoicedListController Treatments2 { get; } = new();
        public override int DatabaseIndex => 12;
        public RecordSource PaymentTypes { get; private set; } = new(DatabaseManager.Find<PaymentType>()!);

        public InvoiceController() 
        {
            Treatments.NotifyParentEvent += Treatments_NotifyParentEvent;
            Treatments2.NotifyParentEvent += Treatments_NotifyParentEvent;
            AddSubControllers(Treatments);
            AddSubControllers(Treatments2);
        }

        private async void Treatments_NotifyParentEvent(object? sender, EventArgs e)
        {
            Task t1 = Treatments.RequeryAsync();
            Task t2 = Treatments2.RequeryAsync();
            await Task.WhenAll(t1, t2);
        }
    }
}
