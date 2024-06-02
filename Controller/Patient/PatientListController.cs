using Backend.Database;
using Backend.Source;
using DentistStudioApp.Model;
using DentistStudioApp.View;
using FrontEnd.Controller;
using FrontEnd.Events;
using FrontEnd.FilterSource;

namespace DentistStudioApp.Controller
{
    public class PatientListController : AbstractFormListController<Patient>
    {
        public SourceOption TitleOptions { get; private set; }
        public SourceOption GenderOptions { get; private set; }
        public override string SearchQry { get; set; } = $"SELECT * FROM {nameof(Patient)} WHERE (LOWER(FirstName) LIKE @name OR LOWER(LastName) LIKE @name)"; 
        public RecordSource Genders { get; private set; } = new(DatabaseManager.Find<Gender>()!);
        public RecordSource Titles { get; private set; } = new(DatabaseManager.Find<JobTitle>()!); 
        
        public override int DatabaseIndex => 0;

        public PatientListController() 
        {
            TitleOptions = new(Titles, "Title");
            GenderOptions = new(Genders, "GenderName");
            AfterUpdate += OnAfterUpdate; ;
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
            QueryBuiler.AddCondition(GenderOptions.Conditions(QueryBuiler));
            QueryBuiler.AddCondition(TitleOptions.Conditions(QueryBuiler));
            OnAfterUpdate(e, new(null, null, nameof(Search)));
        }

        public override async Task<IEnumerable<Patient>> SearchRecordAsync()
        {
            QueryBuiler.AddParameter("name", Search.ToLower() + "%");
            QueryBuiler.AddParameter("name", Search.ToLower() + "%");
            return await CreateFromAsyncList(QueryBuiler.Query, QueryBuiler.Params);
        }

        protected override void Open(Patient? model)
        {
            PatientForm win = new(model);
            win.ShowDialog();
        }
    }
}
