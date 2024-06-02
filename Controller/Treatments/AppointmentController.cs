using Backend.Database;
using Backend.Source;
using DentistStudioApp.Model;
using FrontEnd.Controller;

namespace DentistStudioApp.Controller
{
    public class AppointmentController : AbstractFormController<Appointment>
    {
        public RecordSource Services { get; private set; } = new(DatabaseManager.Find<Service>()!);
        public RecordSource Dentists { get; private set; } = new(DatabaseManager.Find<Dentist>()!);
        public override int DatabaseIndex => 9;
    }
}
