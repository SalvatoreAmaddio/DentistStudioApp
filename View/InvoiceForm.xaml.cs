using DentistStudioApp.Controller;
using DentistStudioApp.Converters;
using DentistStudioApp.Model;
using FrontEnd.ExtensionMethods;
using System.Windows;

namespace DentistStudioApp.View
{
    public partial class InvoiceForm : Window
    {
        public InvoiceForm()
        {
            InitializeComponent();
            this.SetController(new InvoiceController());
        }

        public InvoiceForm(Patient? patient) : this()
        {
            InvoiceController controller = this.GetController<InvoiceController>();
            controller.GoNew();
            controller.Patient = patient;
        }

        public InvoiceForm(Invoice? invoice) : this() 
        {
            FetchPatient fetchPatient = new ();
            InvoiceController controller = this.GetController<InvoiceController>();
            controller.GoAt(invoice);
            controller.Patient = (Patient?)fetchPatient.Convert(invoice, null, null, null);
        }
    }
}
