using DentistStudioApp.Controller;
using DentistStudioApp.Model;
using FrontEnd.ExtensionMethods;
using FrontEnd.Utils;
using System.Windows;

namespace DentistStudioApp.View
{
    public partial class TreatmentForm : Window
    {
        public TreatmentController? Controller;
        public AppointmentListController? SubController => Controller?.Appointments;
        private Appointment? appointment;

        public TreatmentForm()
        {
            InitializeComponent();
            this.SetController(new TreatmentController());
        }

        public TreatmentForm(Treatment treatment) : this()
        {
            Controller = this.GetController<TreatmentController>();
            Controller.Patient = treatment.Patient;

            if (treatment.IsNewRecord()) 
            {
                Controller.GoAt(treatment);
                return;
            }
            Controller.GoAt(treatment);
        }

        public TreatmentForm(Appointment appointment) : this(appointment.Treatment ?? throw new NullReferenceException())
        {
            this.appointment = appointment;
            if (SubController != null)
                SubController.AfterSubFormFilter += OnAppointmentsAfterSubFormFilter;
        }

        private void OnAppointmentsAfterSubFormFilter(object? sender, EventArgs e)
        {
            SubController?.ServiceOptions.FirstOrDefault(s => s.Record.Equals(appointment?.Service))?.Select();
            SubController?.DatesOptions.FirstOrDefault(s => ((DateTime?)s.Value) == appointment?.DOA)?.Select();
            SubController?.TimesOptions.FirstOrDefault(s => ((TimeSpan?)s.Value) == appointment?.TOA)?.Select();
            SubController?.OnOptionFilterClicked(new());
            if (SubController != null)
                SubController.AfterSubFormFilter -= OnAppointmentsAfterSubFormFilter;
        }

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
