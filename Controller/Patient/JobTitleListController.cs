using Backend.Model;
using DentistStudioApp.Model;
using FrontEnd.Controller;
using FrontEnd.Events;
using Backend.ExtensionMethods;

namespace DentistStudioApp.Controller
{
    public class JobTitleListController : AbstractFormListController<JobTitle>
    {
        public override int DatabaseIndex => 2;

        public JobTitleListController()
        {
            AfterUpdate += OnAfterUpdate;
            OpenWindowOnNew = false;
        }

        private async void OnAfterUpdate(object? sender, AfterUpdateArgs e)
        {
            if (e.Is(nameof(Search)))
            {
                await OnSearchPropertyRequeryAsync(sender);
            }
        }
        public override void OnOptionFilterClicked(FilterEventArgs e) { }
        protected override void Open(JobTitle model) { }
        public override async Task<IEnumerable<JobTitle>> SearchRecordAsync()
        {
            SearchQry.AddParameter("name", Search.ToLower() + "%");
            return await CreateFromAsyncList(SearchQry.Statement(), SearchQry.Params());
        }
        public override AbstractClause InstantiateSearchQry() =>
        new JobTitle()
            .Select().All()
            .From()
            .Where().Like("LOWER(Title)", "@name");
    }
}
