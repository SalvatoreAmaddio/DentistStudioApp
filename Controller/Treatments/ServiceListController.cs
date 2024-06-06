using DentistStudioApp.Model;
using FrontEnd.Controller;
using FrontEnd.Events;
using Backend.ExtensionMethods;
using Backend.Model;

namespace DentistStudioApp.Controller
{
    public class ServiceListController : AbstractFormListController<Service>
    {
        public override int DatabaseIndex => 8;

        public ServiceListController() 
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

        public override void OnOptionFilter(FilterEventArgs e) { }

        public override async Task<IEnumerable<Service>> SearchRecordAsync()
        {
            SearchQry.AddParameter("name", Search.ToLower() + "%");
            return await CreateFromAsyncList(SearchQry.Statement(), SearchQry.Params());
        }

        protected override void Open(Service? model)
        {

        }

        public override SelectBuilder InstantiateSearchQry()
        {
            return new Service().Where().Like("LOWER(ServiceName)", "@name");
        }
    }
}