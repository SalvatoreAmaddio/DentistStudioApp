using DentistStudioApp.Controller;
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

        public InvoiceForm(Patient patient) : this()
        {
            InvoiceController controller = this.GetController<InvoiceController>();
            controller.GoNew();
            controller.Patient = patient;
        }

    }
}
