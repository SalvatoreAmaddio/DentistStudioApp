using DentistStudioApp.Model;
using FrontEnd.Controller;
using FrontEnd.Events;
using Backend.ExtensionMethods;
using Backend.Source;
using Backend.Database;

namespace DentistStudioApp.Controller
{
    public class InvoiceListController : AbstractFormListController<Invoice>
    {
        public RecordSource PaymentTypes { get; private set; } = new(DatabaseManager.Find<PaymentType>()!);
        public override string SearchQry { get; set; } = string.Empty;

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