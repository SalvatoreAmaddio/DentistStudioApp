using Backend.Database;
using FrontEnd.Source;
using DentistStudioApp.Model;
using FrontEnd.Controller;
using Backend.Model;

namespace DentistStudioApp.Controller
{
    public class AppointmentController : AbstractFormController<Appointment>
    {
        public RecordSource<Service> Services { get; private set; } = new(DatabaseManager.Find<Service>()!);
        public RecordSource<Dentist> Dentists { get; private set; } = new(DatabaseManager.Find<Dentist>()!);
        public override int DatabaseIndex => 9;

        public override AbstractClause InstantiateSearchQry()
        {
            throw new NotImplementedException();
        }
    }
}
