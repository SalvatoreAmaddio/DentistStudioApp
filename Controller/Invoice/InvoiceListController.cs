using DentistStudioApp.Model;
using FrontEnd.Controller;
using FrontEnd.Events;

namespace DentistStudioApp.Controller
{
    public class InvoiceListController : AbstractFormListController<Invoice>
    {
        public override string SearchQry { get; set; } = $"SELECT * FROM {nameof(Invoice)}";

        public override int DatabaseIndex => 12;

        public override void OnOptionFilter(FilterEventArgs e)
        {
        }

        public override Task<IEnumerable<Invoice>> SearchRecordAsync()
        {
            throw new NotImplementedException();
        }

        protected override void Open(Invoice? model)
        {
        }
    }
}