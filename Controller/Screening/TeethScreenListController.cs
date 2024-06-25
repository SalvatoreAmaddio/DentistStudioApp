using DentistStudioApp.Model;
using FrontEnd.Controller;
using FrontEnd.Events;

namespace DentistStudioApp.Controller
{
    public class TeethScreenListController : AbstractFormListController<TeethScreen>
    {
        private Patient? _patient;
        public override int DatabaseIndex => 15;
        public TeethScreenListController(Patient patient) 
        {
            _patient = patient;
            WindowLoaded += OnWindowLoaded;
        }

        private void OnWindowLoaded(object? sender, System.Windows.RoutedEventArgs e)
        {

        }

        public override void OnOptionFilterClicked(FilterEventArgs e)
        {
        }

        public override Task<IEnumerable<TeethScreen>> SearchRecordAsync()
        {
            throw new NotImplementedException();
        }

        protected override void Open(TeethScreen model)
        {
        }
    }
}