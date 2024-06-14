using DentistStudioApp.Model;
using FrontEnd.Controller;


namespace DentistStudioApp.Controller
{
    public class TreatmentController : AbstractFormController<Treatment>
    {
        public AppointmentListController Appointments { get; } = new();
        public override int DatabaseIndex => 7;
        public TreatmentController() 
        {
            AsRecordSource().ReplaceRecords(AsRecordSource().OrderByDescending(s => s.StartDate).ToList());
            GoFirst();
            AddSubControllers(Appointments);
        }
    }
}
