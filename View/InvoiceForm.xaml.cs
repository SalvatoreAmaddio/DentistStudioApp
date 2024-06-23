using DentistStudioApp.Controller;
using FrontEnd.ExtensionMethods;
using System.Windows;

namespace DentistStudioApp.View
{
    public partial class InvoiceForm : Window
    {
        public InvoiceForm() => InitializeComponent();

        public InvoiceForm(InvoiceController controller) : this() => this.SetController(controller);

    }
}