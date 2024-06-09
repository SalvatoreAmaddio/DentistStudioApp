using DentistStudioApp.Controller;
using DentistStudioApp.Model;
using FrontEnd.ExtensionMethods;
using System.Windows.Controls;

namespace DentistStudioApp.View
{
    public partial class InvoiceList : Page
    {
        public InvoiceList()
        {
            InitializeComponent();
            this.SetController(new InvoiceListController());
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Invoice newInvoice = new();
            newInvoice.DOI = DateTime.Today;
            newInvoice.PaymentType = new PaymentType(1);
            newInvoice.Amount = 10;
            this.GetController<InvoiceListController>().CurrentRecord = newInvoice;
            this.GetController<InvoiceListController>().PerformUpdate();
        }
    }
}
