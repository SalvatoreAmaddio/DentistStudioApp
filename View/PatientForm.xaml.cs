using DentistStudioApp.Controller;
using DentistStudioApp.Model;
using FrontEnd.ExtensionMethods;
using System.Windows;

namespace DentistStudioApp.View
{
    public partial class PatientForm : Window
    {
        public PatientForm()
        {
            InitializeComponent();
            this.SetController(new PatientController());
        }

        public PatientForm(Patient? patient) : this() => this.GetController<PatientController>()?.GoAt(patient);

    }
}
