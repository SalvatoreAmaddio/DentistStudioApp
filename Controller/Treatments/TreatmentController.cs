using DentistStudioApp.Model;
using FrontEnd.Controller;
using FrontEnd.Events;

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
            AsRecordSource().ReplaceRecords(AsRecordSource().OrderByDescending(s => s.StartDate).ToList());
            GoFirst();
            AddSubControllers(Appointments);
            NewRecordEvent += TreatmentController_NewRecordEvent;
        }

        private void TreatmentController_NewRecordEvent(object? sender, AllowRecordMovementArgs e)
        {
            if (CurrentRecord!=null) 
            {
                CurrentRecord.Patient = Patient;
                CurrentRecord.IsDirty = false;
            }
        }
    }
}
