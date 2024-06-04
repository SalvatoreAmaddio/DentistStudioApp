using DentistStudioApp.Controller;
using System.Windows.Controls;

namespace DentistStudioApp.View
{
    public partial class PatientList : Page
    {
        public PatientList()
        {
            InitializeComponent();
            DataContext = new PatientListController();
            ((PatientListController)DataContext).UI = this;
        }
    }
}
