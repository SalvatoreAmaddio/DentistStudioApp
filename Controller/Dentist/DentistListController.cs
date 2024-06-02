using DentistStudioApp.Model;
using FrontEnd.Controller;
using FrontEnd.Events;

namespace DentistStudioApp.Controller
{
    public class DentistListController : AbstractFormListController<Dentist>
    {
        public override string SearchQry { get; set; } = string.Empty;

        public override int DatabaseIndex => 10;

        public override void OnOptionFilter(FilterEventArgs e)
        {
        }

        public override Task<IEnumerable<Dentist>> SearchRecordAsync()
        {
            throw new NotImplementedException();
        }

        protected override void Open(Dentist? model)
        {
            throw new NotImplementedException();
        }
    }
}