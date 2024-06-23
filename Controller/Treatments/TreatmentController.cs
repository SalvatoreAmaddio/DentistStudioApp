using Backend.ExtensionMethods;
using Backend.Model;
using DentistStudioApp.Model;
using DentistStudioApp.View;
using FrontEnd.Controller;
using FrontEnd.Events;
using FrontEnd.ExtensionMethods;
using FrontEnd.Source;
using FrontEnd.Utils;
using System.Windows;
using System.Windows.Input;

namespace DentistStudioApp.Controller
{
    public class TreatmentController : AbstractFormController<Treatment>
    {
        #region Variables
        private readonly Appointment? Appointment;

        private Patient? _patient;
        #endregion

        #region Properties
        public Patient? Patient { get => _patient; set => UpdateProperty(ref value, ref _patient); }
        public AppointmentListController Appointments { get; } = new();
        public override int DatabaseIndex => 7;
        public ICommand OpenServicesCMD { get; }
        public ICommand OpenDentistsCMD { get; }
        public ICommand OpenClinicsCMD { get; }
        #endregion

        #region Constructor
        internal TreatmentController()
        {
            AddSubControllers(Appointments);
            OpenServicesCMD = new CMD(OpenServices);
            OpenDentistsCMD = new CMD(OpenDentists);
            OpenClinicsCMD = new CMD(OpenClinics);
            WindowLoaded += OnWindowLoaded;
            WindowClosed += OnWindowClosed;
            AfterUpdate += OnAfterUpdate;
        }
        public TreatmentController(Treatment treatment, bool readOnly = false) : this()
        {
            Patient = treatment.Patient;
            CurrentRecord = treatment;

            if (readOnly)
            {
                AllowNewRecord = false;
                Lock(true);
            }
        }
        public TreatmentController(Appointment appointment) : this(appointment.Treatment ?? throw new NullReferenceException())
        {
            Appointment = appointment;
            Appointments.AfterSubFormFilter += OnAppointmentsAfterSubFormFilter;
        }
        #endregion

        private void Lock(bool value)
        {
            ReadOnly = value;
            Appointments.ReadOnly = value;
            Appointments.AllowNewRecord = !value;
        }

        #region CommandActions
        private void OpenServices() => Helper.OpenWindowDialog("Services", new ServiceList());
        private void OpenDentists() => Helper.OpenWindowDialog("Dentists", new DentistList());
        private void OpenClinics() => Helper.OpenWindowDialog("Clinics", new ClinicList());
        #endregion

        #region Events Subscriptions
        private void OnAfterUpdate(object? sender, AfterUpdateArgs e)
        {
            if (e.Is(nameof(CurrentModel)))
            {
                CurrentRecord.Patient = Patient;
                CurrentRecord.Clean();
                bool invoiced = CurrentRecord != null && CurrentRecord.Invoiced;
                Lock(invoiced);
            }
        }
        private async void OnWindowLoaded(object? sender, RoutedEventArgs e)
        {
            SearchQry.AddParameter("patientID", Patient?.PatientID);
            RecordSource<Treatment> results = await Task.Run(() => CreateFromAsyncList(SearchQry.Statement(), SearchQry.Params()));
            AsRecordSource().ReplaceRange(results);
            if (CurrentRecord != null)
            {
                if (CurrentRecord.IsNewRecord())
                    GoNew();
                else
                    GoAt(CurrentRecord);

                CurrentRecord.Patient = Patient;
                CurrentRecord.Clean();
            }
        }
        private async void OnWindowClosed(object? sender, EventArgs e)
        {
            TreatmentListController? treatmentListController = Helper.GetActiveWindow()?.GetController<PatientController>()?.GetSubController<TreatmentListController>(0);
            if (treatmentListController != null)
                await treatmentListController.RequeryAsync();
            UnsubscribeWindowClosedEvent();
        }
        private void OnAppointmentsAfterSubFormFilter(object? sender, EventArgs e)
        {
            Appointments.ServiceOptions.FirstOrDefault(s => s.Record.Equals(Appointment?.Service))?.Select();
            Appointments.DatesOptions.FirstOrDefault(s => ((DateTime?)s.Value) == Appointment?.DOA)?.Select();
            Appointments.TimesOptions.FirstOrDefault(s => ((TimeSpan?)s.Value) == Appointment?.TOA)?.Select();
            Appointments.OnOptionFilterClicked(new());
            Appointments.AfterSubFormFilter -= OnAppointmentsAfterSubFormFilter;
        }
        #endregion

        public override AbstractClause InstantiateSearchQry() =>
        new Treatment()
            .Select()
            .From()
            .Where()
                .EqualsTo("PatientID", "@patientID")
            .OrderBy().Field("StartDate DESC");
    }
}