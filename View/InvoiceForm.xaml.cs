using DentistStudioApp.Controller;
using DentistStudioApp.Converters;
using DentistStudioApp.Model;
using FrontEnd.ExtensionMethods;
using System.Windows;

namespace DentistStudioApp.View
{
    public partial class InvoiceForm : Window
    {
        private InvoiceController? _Controller;
        public InvoiceForm()
        {
            InitializeComponent();
            this.SetController(new InvoiceController());
        }

        public InvoiceForm(Patient? patient) : this()
        {
            _Controller = this.GetController<InvoiceController>();
            _Controller?.GoNew();
            _Controller.Patient = patient;
            _Controller.CurrentRecord?.Dirt();
        }

        public InvoiceForm(Invoice? invoice) : this() 
        {
            FetchPatientFromInvoicedTreatment fetchPatient = new ();
            _Controller = this.GetController<InvoiceController>();
            _Controller?.GoAt(invoice);
            _Controller.Patient = (Patient?)fetchPatient.Convert(invoice, null, null, null);
        }
    }
}