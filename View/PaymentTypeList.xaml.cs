using DentistStudioApp.Controller;
using System.Windows.Controls;

namespace DentistStudioApp.View
{
    public partial class PaymentTypeList : Page
    {
        public PaymentTypeList()
        {
            InitializeComponent();
            DataContext = new PaymentTypeListController();
            ((PaymentTypeListController)DataContext).UI = this;
        }
    }
}
