using Backend.ExtensionMethods;
using Backend.Model;
using DentistStudioApp.Model;
using FrontEnd.Controller;
using FrontEnd.Events;

namespace DentistStudioApp.Controller
{
    public class SurveyQuestionCategoryListController : AbstractFormListController<SurveyQuestionCategory>
    {
        public override int DatabaseIndex => 6;
        
        public SurveyQuestionCategoryListController() 
        {
            OpenWindowOnNew = false;
            AfterUpdate += OnAfterUpdate;
        }

        private async void OnAfterUpdate(object? sender, AfterUpdateArgs e)
        {
            if (!e.Is(nameof(Search))) return;
            await OnSearchPropertyRequeryAsync(sender);
        }

        public override void OnOptionFilterClicked(FilterEventArgs e)
        { }

        public override async Task<IEnumerable<SurveyQuestionCategory>> SearchRecordAsync()
        {
            SearchQry.AddParameter("category", Search.ToLower() + "%");
            var sql = SearchQry.Statement();
            return await CreateFromAsyncList(SearchQry.Statement(), SearchQry.Params());
        }

        protected override void Open(SurveyQuestionCategory model) { }

        public override AbstractClause InstantiateSearchQry() =>
            new SurveyQuestionCategory()
            .Select().All()
            .From()
            .Where().Like("LOWER(CategoryName)", "@category");

    }
}