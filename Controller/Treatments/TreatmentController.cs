using Backend.ExtensionMethods;
using Backend.Model;
using DentistStudioApp.Model;
using FrontEnd.Controller;
using FrontEnd.Events;
using FrontEnd.Source;
using System.Windows;

namespace DentistStudioApp.Controller
{
    public class TreatmentController : AbstractFormController<Treatment>
    {
        private Patient? _patient;
        public Patient? Patient { get => _patient; set => UpdateProperty(ref value, ref _patient); }
        public AppointmentListController Appointments { get; } = new();
        public override int DatabaseIndex => 7;
        public TreatmentController()
        {
            AddSubControllers(Appointments);
            NewRecordEvent += TreatmentController_NewRecordEvent;
        }

        protected override async void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            SearchQry.AddParameter("patientID", Patient?.PatientID);
            RecordSource<Treatment> results = await CreateFromAsyncList(SearchQry.Statement(), SearchQry.Params());
            AsRecordSource().ReplaceRange(results);
            GoFirst();
        }

        private void TreatmentController_NewRecordEvent(object? sender, AllowRecordMovementArgs e)
        {
            if (CurrentRecord!=null) 
            {
                CurrentRecord.Patient = Patient;
                CurrentRecord.IsDirty = false;
            }
        }

        public override AbstractClause InstantiateSearchQry()
        {
            return new Treatment().From().Where().EqualsTo("PatientID","@patientID").OrderBy().Field("StartDate DESC");
        }
    }
}
