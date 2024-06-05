using Backend.Database;
using Backend.Source;
using DentistStudioApp.Model;
using FrontEnd.Controller;
using FrontEnd.Events;
using FrontEnd.FilterSource;

namespace DentistStudioApp.Controller
{
    public class DentistListController : AbstractFormListController<Dentist>
    {
        public SourceOption ClinicOptions { get; private set; }
        public override string SearchQry { get; set; } = $"SELECT * FROM {nameof(Dentist)} WHERE (LOWER(FirstName) LIKE @name OR LOWER(LastName) LIKE @name)";
        public RecordSource Clinics { get; private set; } = new(DatabaseManager.Find<Clinic>()!);
        public override int DatabaseIndex => 10;

        public DentistListController() 
        {
            ClinicOptions = new(Clinics, "ClinicName");
            OpenWindowOnNew = false;
            AfterUpdate += OnAfterUpdate;
        }

        private async void OnAfterUpdate(object? sender, AfterUpdateArgs e)
        {
            if (!e.Is(nameof(Search))) return;
            var results = await Task.Run(SearchRecordAsync);
            AsRecordSource().ReplaceRange(results);

            if (sender is not FilterEventArgs filterEvtArgs)
                GoFirst();
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