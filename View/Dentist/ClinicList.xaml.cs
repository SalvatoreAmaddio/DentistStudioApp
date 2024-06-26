using DentistStudioApp.Controller;
using FrontEnd.ExtensionMethods;
using System.Windows.Controls;

namespace DentistStudioApp.View
{
    public partial class ClinicList : Page
    {
        public ClinicList()
        {
            InitializeComponent();
            this.SetController(new ClinicListController());
        }
    }
}
