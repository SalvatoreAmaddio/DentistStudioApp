using Backend.Database;
using Backend.Source;
using DentistStudioApp.Model;
using FrontEnd.Controller;
using FrontEnd.Events;
using FrontEnd.FilterSource;
using Backend.ExtensionMethods;

namespace DentistStudioApp.Controller
{
    public class DentistListController : AbstractFormListController<Dentist>
    {
        public SourceOption ClinicOptions { get; private set; }

        public override string SearchQry { get; set; } = new Dentist().Where().OpenBracket().Like("LOWER(FirstName)", "@name").OR().Like("LOWER(LastName)", "@name").CloseBracket().Statement();
        public RecordSource Clinics { get; private set; } = new(DatabaseManager.Find<Clinic>()!);
        public override int DatabaseIndex => 10;

        public DentistListController() 
        {
            ClinicOptions = new(Clinics, "ClinicName");
            OpenWindowOnNew = false;
            AfterUpdate += OnAfterUpdate;
        }

        private void OnAfterUpdate(object? sender, AfterUpdateArgs e)
        {
            if (!e.Is(nameof(Search))) return;
            OnSearchPropertyRequery(sender);
        }

        public override void OnOptionFilter(FilterEventArgs e)
        {
            QueryBuiler.Clear();
            QueryBuiler.AddCondition(ClinicOptions.Conditions(QueryBuiler));
            OnAfterUpdate(e, new(null, null, nameof(Search)));
        }

        public override async Task<IEnumerable<Dentist>> SearchRecordAsync()
        {
            QueryBuiler.AddParameter("name", Search.ToLower() + "%");
            QueryBuiler.AddParameter("name", Search.ToLower() + "%");
            return await CreateFromAsyncList(QueryBuiler.Query, QueryBuiler.Params);
        }

        protected override void Open(Dentist? model) { }
    }
}