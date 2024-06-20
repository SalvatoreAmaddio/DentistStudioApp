using Backend.Model;
using DentistStudioApp.Model;
using FrontEnd.Controller;

namespace DentistStudioApp.Controller
{
    public class DentistController : AbstractFormController<Dentist>
    {
        public override int DatabaseIndex => 10;

        public override AbstractClause InstantiateSearchQry()
        {
            throw new NotImplementedException();
        }
    }
}
