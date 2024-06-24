using DentistStudioApp.Controller;
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

        public InvoiceList(InvoiceListController controller)
        {
            InitializeComponent();
            this.SetController(controller);
        }
    }
}
