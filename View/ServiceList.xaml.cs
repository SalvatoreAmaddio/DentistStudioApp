using DentistStudioApp.Controller;
using System.Windows.Controls;

namespace DentistStudioApp.View
{
    public partial class ServiceList : Page
    {
        public ServiceList()
        {
            InitializeComponent();
//            SetController(new ServiceListController());
            ((ServiceListController)DataContext).UI = this;
        }
    }
}
