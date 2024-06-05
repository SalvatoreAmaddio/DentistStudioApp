using DentistStudioApp.Model;
using FrontEnd.Controller;
using FrontEnd.Events;
using Backend.ExtensionMethods;

namespace DentistStudioApp.Controller
{
    public class ClinicListController : AbstractFormListController<Clinic>
    {
        public override string SearchQry { get; set; } = new Clinic().Where().Like("LOWER(ClinicName)", "@name").Statement();

        public override int DatabaseIndex => 11;
        
        public ClinicListController() 
        {
            AfterUpdate += OnAfterUpdate;
            OpenWindowOnNew = false;
        }

        private async void OnAfterUpdate(object? sender, AfterUpdateArgs e)
        {
            if (e.Is(nameof(Search)))
            {
                var results = await Task.Run(SearchRecordAsync);
                AsRecordSource().ReplaceRange(results);
                GoFirst();
            }
        }

        public override void OnOptionFilter(FilterEventArgs e) { }

        public override async Task<IEnumerable<Clinic>> SearchRecordAsync()
        {
            QueryBuiler.AddParameter("name", Search.ToLower() + "%");
            return await CreateFromAsyncList(QueryBuiler.Query, QueryBuiler.Params);
        }

        protected override void Open(Clinic? model) { }
    }
}