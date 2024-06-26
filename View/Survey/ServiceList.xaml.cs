using DentistStudioApp.Controller;
using FrontEnd.ExtensionMethods;
using System.Windows.Controls;

namespace DentistStudioApp.View
{
    public partial class ServiceList : Page
    {
        public ServiceList()
        {
            InitializeComponent();
            this.SetController(new ServiceListController());
        }
    }
}
