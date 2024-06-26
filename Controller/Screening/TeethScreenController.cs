using Backend.Model;
using DentistStudioApp.Model;
using FrontEnd.Controller;
using Backend.ExtensionMethods;
using System.Windows;
using Backend.Events;

namespace DentistStudioApp.Controller
{
    public class TeethScreenController : AbstractFormController<TeethScreen>
    {
        private readonly Patient? _patient;
        public TeethScreenDataController ScreeningController { get; } = new();
        public override int DatabaseIndex => 15;

        public TeethScreenController(TeethScreen teethScreen)
        {
            CurrentRecord = teethScreen;
            _patient = CurrentRecord.Patient;
            AddSubControllers(ScreeningController);
            WindowLoaded += OnWindowLoaded;
            AfterRecordNavigation += OnAfterRecordNavigation;
        }

        #region Event Subscriptions
        private void OnAfterRecordNavigation(object? sender, AllowRecordMovementArgs e)
        {
            if (CurrentRecord!=null)
            {
                CurrentRecord.Patient = _patient;
                CurrentRecord.Clean();
            }

            if (e.NewRecord)
            {
                CurrentRecord?.Dirt();
            }
        }

        private async void OnWindowLoaded(object? sender, RoutedEventArgs e)
        {
            SearchQry.AddParameter("patientID", $"{CurrentRecord?.Patient?.PatientID}");
            IEnumerable<TeethScreen> results = await CreateFromAsyncList(SearchQry.Statement(), SearchQry.Params());
            AsRecordSource().ReplaceRange(results);
            GoAt(CurrentRecord);
        }
        #endregion

        public override AbstractClause InstantiateSearchQry() =>
           new TeethScreen()
               .Select().All()
               .From()
               .Where().EqualsTo("PatientID", "@patientID")
               .OrderBy().Field("DOS DESC");
    }
}