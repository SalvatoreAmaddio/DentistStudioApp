using Backend.Model;
using DentistStudioApp.Model;
using DentistStudioApp.View;
using FrontEnd.Controller;
using FrontEnd.Events;
using FrontEnd.FilterSource;
using Backend.ExtensionMethods;

namespace DentistStudioApp.Controller
{
    public class TeethScreenListController : AbstractFormListController<TeethScreen>
    {
        private readonly Patient? _patient;
        public override int DatabaseIndex => 15;

        public SourceOption DatesOptions { get; private set; }
        public TeethScreenListController(Patient patient) 
        {
            _patient = patient;
            DatesOptions = new PrimitiveSourceOption(this, "DOS");
            WindowLoaded += OnWindowLoaded;
        }

        private async void OnWindowLoaded(object? sender, System.Windows.RoutedEventArgs e)
        {
            IEnumerable<TeethScreen> results = await SearchRecordAsync();
            AsRecordSource().ReplaceRange(results);
            GoFirst();
        }

        public override async void OnOptionFilterClicked(FilterEventArgs e)
        {
            ReloadSearchQry();
            DatesOptions.Conditions<WhereClause>(SearchQry);
            IEnumerable<TeethScreen> results = await SearchRecordAsync();
            AsRecordSource().ReplaceRange(results);
            GoFirst();
        }

        public override async Task<IEnumerable<TeethScreen>> SearchRecordAsync()
        {
            SearchQry.AddParameter("patientID", $"{_patient?.PatientID}");
            return await CreateFromAsyncList(SearchQry.Statement(), SearchQry.Params());
        }

        protected override void Open(TeethScreen model)
        {
            model.Patient = _patient;
            if (!model.IsNewRecord())
                model.Clean();
            TeethScreenDataForm screenForm = new(model);
            screenForm.ShowDialog();
        }

        public override AbstractClause InstantiateSearchQry() =>
           new TeethScreen()
               .Select().All()
               .From()
               .Where().EqualsTo("PatientID", "@patientID")
               .OrderBy().Field("DOS DESC");
    }
}