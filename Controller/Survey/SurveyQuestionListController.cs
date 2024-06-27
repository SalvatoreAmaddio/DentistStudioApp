using Backend.Database;
using Backend.ExtensionMethods;
using Backend.Model;
using DentistStudioApp.Model;
using FrontEnd.Controller;
using FrontEnd.Events;
using FrontEnd.FilterSource;
using FrontEnd.Source;

namespace DentistStudioApp.Controller
{
    public class SurveyQuestionListController : AbstractFormListController<SurveyQuestion>
    {
        public RecordSource<SurveyQuestionCategory> Categories { get; private set; } = new(DatabaseManager.Find<SurveyQuestionCategory>()!);
        public SourceOption CategoryOptions { get; private set; }
        public override int DatabaseIndex => 5;

        public SurveyQuestionListController() 
        {
            CategoryOptions = new SourceOption(Categories, "CategoryName");
            AfterUpdate += OnAfterUpdate;
            OpenWindowOnNew = false;
        }

        private async void OnAfterUpdate(object? sender, AfterUpdateArgs e)
        {
            if (!e.Is(nameof(Search))) return;
            await OnSearchPropertyRequeryAsync(sender);
        }

        public override void OnOptionFilterClicked(FilterEventArgs e)
        {
            ReloadSearchQry();
            CategoryOptions.Conditions<WhereClause>(SearchQry);
            OnAfterUpdate(e, new(null, null, nameof(Search)));
        }

        public async override Task<IEnumerable<SurveyQuestion>> SearchRecordAsync()
        {
            SearchQry.AddParameter("question", Search.ToLower() + "%");
            return await CreateFromAsyncList(SearchQry.Statement(), SearchQry.Params());
        }

        protected override void Open(SurveyQuestion model)
        {
        }

        public override AbstractClause InstantiateSearchQry() =>
        new SurveyQuestion()
            .Select().All()
            .From().InnerJoin(new SurveyQuestionCategory())
            .Where().OpenBracket().Like("LOWER(Question)", "@question").CloseBracket();
    }
}
