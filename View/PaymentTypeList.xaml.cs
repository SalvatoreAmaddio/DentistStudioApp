using DentistStudioApp.Controller;
using System.Windows.Controls;

namespace DentistStudioApp.View
{
    public partial class PaymentType : Page
    {
        public PaymentType()
        {
            InitializeComponent();
            DataContext = new PaymentTypeListController();
            ((PaymentTypeListController)DataContext).UI = this;
        }
    }
}
