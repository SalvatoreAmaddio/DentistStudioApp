using DentistStudioApp.Model;
using FrontEnd.Controller;
using FrontEnd.Events;

namespace DentistStudioApp.Controller
{
    public class ClinicListController : AbstractFormListController<Clinic>
    {
        public override string SearchQry { get; set; } = string.Empty;

        public override int DatabaseIndex => 11;

        public override void OnOptionFilter(FilterEventArgs e)
        {
        }

        public override Task<IEnumerable<Clinic>> SearchRecordAsync()
        {
            throw new NotImplementedException();
        }

        protected override void Open(Clinic? model)
        {
            throw new NotImplementedException();
        }
    }
}