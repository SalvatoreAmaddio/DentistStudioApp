using DentistStudioApp.Controller;
using DentistStudioApp.Model;
using FrontEnd.Dialogs;
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

        private void OnPhotoFrameClicked(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            FilePicker filePicker = new();
            filePicker.ShowDialog();
            MessageBox.Show(filePicker.SelectedFile);
        }
    }
}
