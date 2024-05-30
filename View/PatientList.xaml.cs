using DentistStudioApp.Controller;
using System.Windows;

namespace DentistStudioApp.View
{
    public partial class PatientList : Window
    {
        public PatientList()
        {
            InitializeComponent();
            DataContext = new PatientControllerList();
            ((PatientControllerList)DataContext).Window = this;
        }
    }
}
