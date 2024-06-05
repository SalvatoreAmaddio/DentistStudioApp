using Backend.Database;
using Backend.Source;
using DentistStudioApp.Model;
using FrontEnd.Controller;

namespace DentistStudioApp.Controller
{
    public class InvoiceController : AbstractFormController<Invoice>
    {
        public override int DatabaseIndex => 12;
        public RecordSource Genders { get; private set; } = new(DatabaseManager.Find<PaymentType>()!);

    }
}
