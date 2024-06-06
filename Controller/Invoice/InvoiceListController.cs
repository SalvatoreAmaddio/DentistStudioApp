using DentistStudioApp.Model;
using FrontEnd.Controller;
using FrontEnd.Events;
using Backend.ExtensionMethods;
using Backend.Source;
using Backend.Database;
using FrontEnd.FilterSource;

namespace DentistStudioApp.Controller
{
    public class InvoiceListController : AbstractFormListController<Invoice>
    {
        public RecordSource PaymentTypes { get; private set; } = new(DatabaseManager.Find<PaymentType>()!);
        public override string SearchQry { get; set; } = new InvoicedTreatment()
            .SelectFields("Invoice.*")
            .OpenBracket()
            .InnerJoin("Treatment", "TreatmentID")
            .InnerJoin(new Invoice())
            .CloseBracket()
            .InnerJoin("Patient", "Treatment", "PatientID", "PatientID")
            .Where()
            .OpenBracket()
            .Like("LOWER(Patient.FirstName)", "@name")
            .OR()
            .Like("LOWER(Patient.LastName)", "@name")
            .CloseBracket()
            .Statement();

        public override int DatabaseIndex => 12;
        public SourceOption PaymentTypesOptions { get; private set; }

        public InvoiceListController()
        {
            AfterUpdate += OnAfterUpdate;
            PaymentTypesOptions = new(PaymentTypes, "PaymentBy");
        }

        private async void OnAfterUpdate(object? sender, AfterUpdateArgs e)
        {
            if (e.Is(nameof(Search))) 
            {
                var results = await Task.Run(SearchRecordAsync);
                AsRecordSource().ReplaceRange(results);

                if (sender is not FilterEventArgs filterEvtArgs)
                    GoFirst();
            }
        }

        public override void OnOptionFilter(FilterEventArgs e)
        {
            QueryBuiler.Clear();
            QueryBuiler.AddCondition(PaymentTypesOptions.Conditions(QueryBuiler));
            OnAfterUpdate(e, new(null, null, nameof(Search)));
        }

        public override async Task<IEnumerable<Invoice>> SearchRecordAsync()
        {
            QueryBuiler.AddParameter("name", Search.ToLower() + "%");
            QueryBuiler.AddParameter("name", Search.ToLower() + "%");
            return await CreateFromAsyncList(QueryBuiler.Query, QueryBuiler.Params);
        }

        protected override void Open(Invoice? model)
        {
        }

    }
}