using Backend.Model;
using DentistStudioApp.Model;
using FrontEnd.Controller;

namespace DentistStudioApp.Controller
{
    public class ClinicController : AbstractFormController<Clinic>
    {
        public override int DatabaseIndex => 11;

        public override AbstractClause InstantiateSearchQry()
        {
            throw new NotImplementedException();
        }
    }
}
