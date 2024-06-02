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
            Treatment? treatment = (Treatment?)ParentRecord;
            if (CurrentRecord!= null) 
            {
                CurrentRecord.Treatment = treatment;
                CurrentRecord.IsDirty = false;
            }
        }

        public override async void OnSubFormFilter()
        {
            QueryBuiler.Clear();
            QueryBuiler.AddParameter("treatmentID", ParentRecord?.GetTablePK()?.GetValue());
            var results = await CreateFromAsyncList(QueryBuiler.Query, QueryBuiler.Params);
            AsRecordSource().ReplaceRange(results);
            GoFirst();
        }

        private void Treatment_ServiceUpdateEvent(object? sender, EventArgs e)
        {
            throw new NotImplementedException();
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

        protected override bool Update(Appointment? model)
        {
            bool? IsNewRecord = model?.IsNewRecord();
            bool result = base.Update(model);
            if (result) 
                if (IsNewRecord!=null) 
                    if (IsNewRecord.Value) 
                        ((Treatment?)ParentRecord)?.UpdateTotalServiceCount(ArithmeticOperation.ADD);
            return result;
        }

        protected override bool Delete(Appointment? model)
        {
            bool result = base.Delete(model);
            if (result)
                ((Treatment?)ParentRecord)?.UpdateTotalServiceCount(ArithmeticOperation.SUBTRACT);
            return result;
        }
    }
}