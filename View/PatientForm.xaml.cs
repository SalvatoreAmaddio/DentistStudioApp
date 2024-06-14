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

        public PatientForm(Patient? patient) : this()
        {
            this.GetController<PatientController>().GoAt(patient);
        }

        private void OpenGenderWindow(object sender, RoutedEventArgs e)
        {
            Window window = new()
            {
                Title = "Genders",
                Content = new GenderList()
            };
            window.ShowDialog();
        }

        private void OpenJobTitleWindow(object sender, RoutedEventArgs e)
        {
            Window window = new()
            {
                Title = "Job Titles",
                Content = new JobTitleList()
            };
            window.ShowDialog();
        }
    }
}
