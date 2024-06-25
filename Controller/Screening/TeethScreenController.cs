using Backend.Model;
using DentistStudioApp.Model;
using FrontEnd.Controller;
using Backend.ExtensionMethods;

namespace DentistStudioApp.Controller
{
    public class TeethScreenController : AbstractFormController<TeethScreen>
    {
        public ScreeningController ScreeningController { get; } = new();
        public override int DatabaseIndex => 15;

        public TeethScreenController(TeethScreen teethScreen)
        {
            CurrentRecord = teethScreen;
            AddSubControllers(ScreeningController);
            WindowLoaded += TeethScreenController_WindowLoaded;
        }

        private async void TeethScreenController_WindowLoaded(object? sender, System.Windows.RoutedEventArgs e)
        {
            SearchQry.AddParameter("patientID", $"{CurrentRecord?.Patient?.PatientID}");
            IEnumerable<TeethScreen> results = await CreateFromAsyncList(SearchQry.Statement(), SearchQry.Params());
            AsRecordSource().ReplaceRange(results);
            GoAt(CurrentRecord);
        }

        public override AbstractClause InstantiateSearchQry() =>
           new TeethScreen()
               .Select().All()
               .From()
               .Where().EqualsTo("PatientID", "@patientID")
               .OrderBy().Field("DOS DESC");
    }
}
