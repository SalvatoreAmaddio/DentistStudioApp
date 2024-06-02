using Backend.Database;
using Backend.Source;
using DentistStudioApp.Model;
using FrontEnd.Controller;
using FrontEnd.Events;

namespace DentistStudioApp.Controller
{
    public class AppointmentListController : AbstractFormListController<Appointment>
    {
        public RecordSource Services { get; private set; } = new(DatabaseManager.Find<Service>()!);
        public RecordSource Dentists { get; private set; } = new(DatabaseManager.Find<Dentist>()!);
        public override string SearchQry { get; set; } = $"SELECT * FROM {nameof(Appointment)} WHERE TreatmentID = @treatmentID;";

        public override int DatabaseIndex => 9;
        
        public AppointmentListController() 
        {
            NewRecordEvent += OnNewRecordEvent;
            OpenWindowOnNew = false;
        }

        private void OnNewRecordEvent(object? sender, EventArgs e)
        {


        }

        public override async void OnSubFormFilter()
        {
            QueryBuiler.Clear();
            QueryBuiler.AddParameter("treatmentID", ParentRecord?.GetTablePK()?.GetValue());
            var results = await CreateFromAsyncList(QueryBuiler.Query, QueryBuiler.Params);
            AsRecordSource().ReplaceRange(results);
            GoFirst();
        }
        public override void OnOptionFilter(FilterEventArgs e)
        {
        }

        public override Task<IEnumerable<Appointment>> SearchRecordAsync()
        {
            throw new NotImplementedException();
        }

        protected override void Open(Appointment? model)
        {
            throw new NotImplementedException();
        }
    }
}