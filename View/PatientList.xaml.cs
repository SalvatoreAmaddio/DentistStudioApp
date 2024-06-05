using DentistStudioApp.Controller;
using FrontEnd.ExtensionMethods;
using System.Windows.Controls;

namespace DentistStudioApp.View
{
    public partial class PatientList : Page
    {
        public PatientList()
        {
            InitializeComponent();
            this.SetController(new PatientListController());
        }
    }
}
