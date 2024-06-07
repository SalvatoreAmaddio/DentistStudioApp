using DentistStudioApp.Model;
using FrontEnd.Controller;
using FrontEnd.Events;
using Backend.ExtensionMethods;
using Backend.Source;
using Backend.Database;
using FrontEnd.FilterSource;
using Backend.Model;
using DentistStudioApp.View;

namespace DentistStudioApp.Controller
{
    public class InvoiceListController : AbstractFormListController<Invoice>
    {
        public RecordSource PaymentTypes { get; private set; } = new(DatabaseManager.Find<PaymentType>()!);

        public override int DatabaseIndex => 12;
        public SourceOption PaymentTypesOptions { get; private set; }

        public InvoiceListController()
        {
            AfterUpdate += OnAfterUpdate;
            PaymentTypesOptions = new(PaymentTypes, "PaymentBy");
            AllowNewRecord = false;
        }

        private void OnAfterUpdate(object? sender, AfterUpdateArgs e)
        {
            if (e.Is(nameof(Search))) 
            {
                OnSearchPropertyRequery(sender);
            }
        }

        public override void OnOptionFilterClicked(FilterEventArgs e)
        {
            ReloadSearchQry();
            PaymentTypesOptions.Conditions(SearchQry);
            OnAfterUpdate(e, new(null, null, nameof(Search)));
        }

        public override async Task<IEnumerable<Invoice>> SearchRecordAsync()
        {
            SearchQry.AddParameter("name", Search.ToLower() + "%");
            SearchQry.AddParameter("name", Search.ToLower() + "%");
            return await CreateFromAsyncList(SearchQry.Statement(), SearchQry.Params());
        }

        protected override void Open(Invoice? model)
        {
            InvoiceForm invoideForm = new(model);
            invoideForm.ShowDialog();
        }

        public override SelectBuilder InstantiateSearchQry()
        {
            return new InvoicedTreatment()
             .SelectFields("Invoice.*")
             .OpenBracket()
                 .InnerJoin("Treatment", "TreatmentID")
                 .InnerJoin(new Invoice())
                 .InnerJoin("PaymentType", "Invoice", "PaymentTypeID")
             .CloseBracket()
             .InnerJoin("Patient", "Treatment", "PatientID")
             .Where()
             .OpenBracket()
             .Like("LOWER(Patient.FirstName)", "@name")
             .OR()
             .Like("LOWER(Patient.LastName)", "@name")
             .CloseBracket();
        }
    }
}