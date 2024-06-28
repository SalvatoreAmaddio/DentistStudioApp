using Backend.Database;
using DentistStudioApp.Model;
using FrontEnd.Controller;
using FrontEnd.Events;
using FrontEnd.FilterSource;
using Backend.ExtensionMethods;
using Backend.Model;
using FrontEnd.Source;

namespace DentistStudioApp.Controller
{
    public class DentistListController : AbstractFormListController<Dentist>
    {
        public SourceOption ClinicOptions { get; private set; }
        public RecordSource<Clinic> Clinics { get; private set; } = new(DatabaseManager.Find<Clinic>()!);

        public DentistListController() 
        {
            ClinicOptions = new(Clinics, "ClinicName");
            OpenWindowOnNew = false;
            AfterUpdate += OnAfterUpdate;
        }

        private async void OnAfterUpdate(object? sender, AfterUpdateArgs e)
        {
            if (!e.Is(nameof(Search))) return;
            await OnSearchPropertyRequeryAsync(sender);
        }

        public override void OnOptionFilterClicked(FilterEventArgs e)
        {
            ReloadSearchQry();
            ClinicOptions.Conditions<WhereClause>(SearchQry);
            OnAfterUpdate(e, new(null, null, nameof(Search)));
        }

        public override async Task<IEnumerable<Dentist>> SearchRecordAsync()
        {
            SearchQry.AddParameter("name", Search.ToLower() + "%");
            SearchQry.AddParameter("name", Search.ToLower() + "%");
            return await CreateFromAsyncList(SearchQry.Statement(), SearchQry.Params());
        }

        protected override void Open(Dentist? model) { }

        public override AbstractClause InstantiateSearchQry() =>
        new Dentist()
            .Select()
            .From()
            .InnerJoin(new Clinic())
            .Where()
                .OpenBracket()
                    .Like("LOWER(FirstName)", "@name")
                    .OR()
                    .Like("LOWER(LastName)", "@name")
                .CloseBracket();
    }
}