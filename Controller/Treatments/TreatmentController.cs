using Backend.ExtensionMethods;
using Backend.Model;
using DentistStudioApp.Model;
using FrontEnd.Controller;
using FrontEnd.Events;
using FrontEnd.ExtensionMethods;
using FrontEnd.Source;
using FrontEnd.Utils;
using System.Windows;

namespace DentistStudioApp.Controller
{
    public class TreatmentController : AbstractFormController<Treatment>
    {
        private readonly Appointment? Appointment;

        private Patient? _patient;
        public Patient? Patient { get => _patient; set => UpdateProperty(ref value, ref _patient); }
        public AppointmentListController Appointments { get; } = new();
        public override int DatabaseIndex => 7;

        public TreatmentController()
        {
            AddSubControllers(Appointments);
            RecordMovingEvent += OnRecordMoving;
        }

        public TreatmentController(Treatment treatment) : this()
        {
            Patient = treatment.Patient;
            CurrentRecord = treatment;
            if (CurrentRecord.IsNewRecord()) 
                return;
            GoAt(treatment);
        }

        public TreatmentController(Appointment appointment) : this(appointment.Treatment ?? throw new NullReferenceException())
        {
            Appointment = appointment;
            Appointments.AfterSubFormFilter += OnAppointmentsAfterSubFormFilter;
        }

        private void OnAppointmentsAfterSubFormFilter(object? sender, EventArgs e)
        {
            Appointments.ServiceOptions.FirstOrDefault(s => s.Record.Equals(Appointment?.Service))?.Select();
            Appointments.DatesOptions.FirstOrDefault(s => ((DateTime?)s.Value) == Appointment?.DOA)?.Select();
            Appointments.TimesOptions.FirstOrDefault(s => ((TimeSpan?)s.Value) == Appointment?.TOA)?.Select();
            Appointments.OnOptionFilterClicked(new());
            Appointments.AfterSubFormFilter -= OnAppointmentsAfterSubFormFilter;
        }

        protected override async void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            SearchQry.AddParameter("patientID", Patient?.PatientID);
            RecordSource<Treatment> results = await Task.Run(() => CreateFromAsyncList(SearchQry.Statement(), SearchQry.Params()));
            AsRecordSource().ReplaceRange(results);
            if (CurrentRecord != null && CurrentRecord.IsNewRecord()) 
                GoNew();
            else
                GoFirst();
        }

        protected override async void OnWindowClosed(object? sender, EventArgs e)
        {
            TreatmentListController? treatmentListController = Helper.GetActiveWindow()?.GetController<PatientController>()?.GetSubController<TreatmentListController>(0);
            if (treatmentListController != null)
                await treatmentListController.RequeryAsync();
            DisposeWindow();
        }

        private void OnRecordMoving(object? sender, AllowRecordMovementArgs e)
        {
            if (e.NewRecord) 
            {
                if (CurrentRecord != null)
                {
                    CurrentRecord.Patient = Patient;
                    CurrentRecord.IsDirty = false;
                }
            }
        }

        public override AbstractClause InstantiateSearchQry() =>
        new Treatment().From().Where().EqualsTo("PatientID","@patientID").OrderBy().Field("StartDate DESC");
    }
}
