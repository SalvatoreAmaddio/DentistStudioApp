using DentistStudioApp.Model;
using FrontEnd.Controller;
using FrontEnd.Events;
using Backend.ExtensionMethods;
using Backend.Model;

namespace DentistStudioApp.Controller
{
    public class ClinicListController : AbstractFormListController<Clinic>
    {
        public override int DatabaseIndex => 11;
        
        public ClinicListController() 
        {
            AfterUpdate += OnAfterUpdate;
            OpenWindowOnNew = false;
        }

        private void OnAfterUpdate(object? sender, AfterUpdateArgs e)
        {
            if (e.Is(nameof(Search)))
            {
                OnSearchPropertyRequery(sender);
            }
        }

        public override void OnOptionFilterClicked(FilterEventArgs e) { }

        public override async Task<IEnumerable<Clinic>> SearchRecordAsync()
        {
            SearchQry.AddParameter("name", Search.ToLower() + "%");
            return await CreateFromAsyncList(SearchQry.Statement(), SearchQry.Params());
        }

        protected override void Open(Clinic? model) { }

        public override SelectBuilder InstantiateSearchQry()
        {
            return new Clinic().Where().Like("LOWER(ClinicName)", "@name");
        }
    }
}