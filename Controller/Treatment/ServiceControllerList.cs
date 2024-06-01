using DentistStudioApp.Model;
using FrontEnd.Controller;
using FrontEnd.Events;

namespace DentistStudioApp.Controller
{
    public class ServiceControllerList : AbstractFormListController<Service>
    {
        public override string SearchQry { get; set; } = string.Empty;

        public override int DatabaseIndex => 8;

        public override void OnOptionFilter(FilterEventArgs e)
        {
        }

        public override Task<IEnumerable<Service>> SearchRecordAsync()
        {
            throw new NotImplementedException();
        }

        protected override void Open(Service? model)
        {
            throw new NotImplementedException();
        }
    }
}
