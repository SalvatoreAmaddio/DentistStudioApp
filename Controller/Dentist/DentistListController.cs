using Backend.Database;
using Backend.Source;
using DentistStudioApp.Model;
using FrontEnd.Controller;
using FrontEnd.Events;
using FrontEnd.FilterSource;

namespace DentistStudioApp.Controller
{
    public class DentistListController : AbstractFormListController<Dentist>
    {
        public SourceOption ClinicOptions { get; private set; }
        public override string SearchQry { get; set; } = string.Empty;
        public RecordSource Clinics { get; private set; } = new(DatabaseManager.Find<Clinic>()!);
        public override int DatabaseIndex => 10;

        public DentistListController() 
        {
            ClinicOptions = new(Clinics, "ClinicName");
        }

        public override void OnOptionFilter(FilterEventArgs e)
        {
        }

        public override Task<IEnumerable<Dentist>> SearchRecordAsync()
        {
            throw new NotImplementedException();
        }

        protected override void Open(Dentist? model)
        {
            throw new NotImplementedException();
        }
    }
}