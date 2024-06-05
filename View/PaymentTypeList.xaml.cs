using DentistStudioApp.Controller;
using FrontEnd.ExtensionMethods;
using System.Windows.Controls;

namespace DentistStudioApp.View
{
    public partial class PaymentTypeList : Page
    {
        public PaymentTypeList()
        {
            InitializeComponent();
            this.SetController(new PaymentTypeListController());
        }
    }
}
