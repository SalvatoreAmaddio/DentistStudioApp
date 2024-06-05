using DentistStudioApp.Controller;
using DentistStudioApp.Model;
using System.Windows;

namespace DentistStudioApp.View
{
    public partial class InvoiceForm : Window
    {
        public InvoiceForm()
        {
            InitializeComponent();
            DataContext = new InvoiceController();
            ((InvoiceController)DataContext).UI = this;
        }

        public InvoiceForm(Patient? patient) : this()
        {
            ((PatientController)DataContext).GoAt(patient);
        }

    }
}
