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

        public override AbstractClause InstantiateSearchQry()
        {
            return new JobTitle().Select().AllFields().From().Where().Like("LOWER(Title)", "@name");
        }

        public override void OnOptionFilterClicked(FilterEventArgs e)
        {
        }

        public override async Task<IEnumerable<JobTitle>> SearchRecordAsync()
        {
            SearchQry.AddParameter("name", Search.ToLower() + "%");
            return await CreateFromAsyncList(SearchQry.Statement(), SearchQry.Params());
        }

        protected override void Open(JobTitle model)
        {

        }
    }
}
