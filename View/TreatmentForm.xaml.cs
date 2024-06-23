using DentistStudioApp.Controller;
using DentistStudioApp.Model;
using FrontEnd.ExtensionMethods;
using FrontEnd.Utils;
using System.Windows;

namespace DentistStudioApp.View
{
    public partial class TreatmentForm : Window
    {

        public TreatmentForm()
        {
            InitializeComponent();
            this.SetController(new TreatmentController());
            Factory.A(this.GetController<TreatmentController>()!);
        }

        public TreatmentForm(Treatment treatment) : this()
        {
            Factory.B(treatment);
        }

        public TreatmentForm(Appointment appointment) : this(appointment.Treatment ?? throw new NullReferenceException())
        {
            Factory.C(appointment);
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

    public class Factory 
    {
        private static TreatmentController Controller = null!;
        private static AppointmentListController SubController => Controller.Appointments;
        private static Appointment? Appointment;

        public static void A(TreatmentController controller) => Controller = controller;
        public static void B(Treatment treatment) 
        {
            Controller.Patient = treatment.Patient;

            if (treatment.IsNewRecord())
            {
                Controller.GoAt(treatment);
                return;
            }
            Controller.GoAt(treatment);
        }

        public static void C(Appointment appointment) 
        {
            Appointment = appointment;
            if (SubController != null)
                SubController.AfterSubFormFilter += OnAppointmentsAfterSubFormFilter;
        }

        private static void OnAppointmentsAfterSubFormFilter(object? sender, EventArgs e)
        {
            SubController?.ServiceOptions.FirstOrDefault(s => s.Record.Equals(Appointment?.Service))?.Select();
            SubController?.DatesOptions.FirstOrDefault(s => ((DateTime?)s.Value) == Appointment?.DOA)?.Select();
            SubController?.TimesOptions.FirstOrDefault(s => ((TimeSpan?)s.Value) == Appointment?.TOA)?.Select();
            SubController?.OnOptionFilterClicked(new());
        }
    }
}