using Backend.Database;
using Backend.ExtensionMethods;
using Backend.Model;
using Backend.Source;
using DentistStudioApp.Model;
using FrontEnd.Controller;
using FrontEnd.Events;
using FrontEnd.FilterSource;
using System.IO;

namespace DentistStudioApp.Controller
{
    public class SurveyDataListController : AbstractFormListController<SurveyData>
    {
        public SourceOption CategoryOptions { get; private set; }
        public RecordSource SurveyQuestionCategories { get; private set; } = new(DatabaseManager.Find<SurveyQuestionCategory>()!);
        public SurveyController SurveyController { get; } = new();

        public override int DatabaseIndex => 4;

        public SurveyDataListController()
        {
            CategoryOptions = new(SurveyQuestionCategories, "CategoryName");
            AllowNewRecord = false;
            AllowAutoSave = true;
            AfterUpdate += OnAfterUpdate;
        }

        private void OnAfterUpdate(object? sender, AfterUpdateArgs e)
        {
            if (!e.Is(nameof(Search))) return;
            OnSearchPropertyRequery(sender);
        }

        public override void OnOptionFilterClicked(FilterEventArgs e)
        {
            ReloadSearchQry();
            CategoryOptions.Conditions(SearchQry);
            OnAfterUpdate(e, new(null, null, nameof(Search)));
        }

        public override async Task<IEnumerable<SurveyData>> SearchRecordAsync()
        {
            SearchQry.AddParameter("id", SurveyController?.CurrentRecord?.SurveyID);
            SearchQry.AddParameter("question", Search.ToLower() + "%");
            return await CreateFromAsyncList(SearchQry.Statement(), SearchQry.Params());
        }

        protected override void Open(SurveyData? model)
        {
        }

        public override SelectBuilder InstantiateSearchQry()
        {
            return new SurveyData()
                .InnerJoin(nameof(SurveyQuestion), nameof(SurveyData), "SurveyQuestionID")
                .InnerJoin(nameof(SurveyQuestionCategory), nameof(SurveyQuestion), "SurveyQuestionCategoryID")
                .Where()
                .EqualsTo("SurveyID", "@id")
                .AND().OpenBracket()
                .Like("Question", "@question")
                .CloseBracket();
        }
    }

}