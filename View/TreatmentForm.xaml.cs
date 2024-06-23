using DentistStudioApp.Controller;
using FrontEnd.ExtensionMethods;
using FrontEnd.Utils;
using System.Windows;

namespace DentistStudioApp.View
{
    public partial class TreatmentForm : Window
    {

        public TreatmentForm() => InitializeComponent();

        public TreatmentForm(TreatmentController controller) : this() => this.SetController(controller);

        private void OpenServices(object sender, RoutedEventArgs e)
        {
            Helper.OpenWindowDialog("Services", new ServiceList());
        }

        private void OpenDentists(object sender, RoutedEventArgs e)
        {
            Helper.OpenWindowDialog("Dentists", new DentistList());
        }

        private void OpenClinics(object sender, RoutedEventArgs e)
        {
            Helper.OpenWindowDialog("Clinics", new ClinicList());
        }
    }
}