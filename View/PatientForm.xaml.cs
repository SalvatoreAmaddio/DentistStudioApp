using DentistStudioApp.Controller;
using DentistStudioApp.Model;
using System.Windows;

namespace DentistStudioApp.View
{
    public partial class PatientForm : Window
    {
        public PatientForm()
        {
            InitializeComponent();
            DataContext = new PatientController();
            ((PatientController)DataContext).Window = this;
        }

        public PatientForm(Patient? patient) : this()
        {
            ((PatientController)DataContext).GoAt(patient);
        }
    }
}
