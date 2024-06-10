using Backend.Database;
using Backend.ExtensionMethods;
using Backend.Model;
using Backend.Source;
using DentistStudioApp.Model;
using FrontEnd.Controller;
using FrontEnd.Events;
using FrontEnd.FilterSource;
using FrontEnd.Source;

namespace DentistStudioApp.Controller
{
    public abstract class AbstractAppointmentListController : AbstractFormListController<Appointment> 
    {
        public SourceOption ServiceOptions { get; private set; }
        public SourceOption DentistOptions { get; private set; }
        public SourceOption AttendedOptions { get; private set; }
        public SourceOption DatesOptions { get; private set; }
        public RecordSource Services { get; private set; } = new(DatabaseManager.Find<Service>()!);
        public RecordSource Dentists { get; private set; } = new(DatabaseManager.Find<Dentist>()!);
        public override int DatabaseIndex => 9;

        public AbstractAppointmentListController()
        {
            ServiceOptions = new(Services, "ServiceName");
            DentistOptions = new(Dentists, "FullName");
            AttendedOptions = new PrimitiveSourceOption(AsRecordSource(), "AppointmentID", "Attended");
            DatesOptions = new PrimitiveSourceOption(AsRecordSource(), "AppointmentID", "DOA");
            OpenWindowOnNew = false;
        }

        public void ReloadFilters()
        {
            ServiceOptions = new(Services, "ServiceName");
            DentistOptions = new(Dentists, "FullName");
            AttendedOptions = new PrimitiveSourceOption(AsRecordSource(), "AppointmentID", "Attended");
            DatesOptions = new PrimitiveSourceOption(AsRecordSource(), "AppointmentID", "DOA");
        }
        public override Task<IEnumerable<Appointment>> SearchRecordAsync()
        {
            throw new NotImplementedException();
        }

        protected override void Open(Appointment? model)
        {
            
        }

    }

    public class AppointmentListController : AbstractAppointmentListController
    {
        
        public AppointmentListController() => NewRecordEvent += OnNewRecordEvent;

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
            ReloadSearchQry();
            SearchQry.AddParameter("treatmentID", ParentRecord?.GetTablePK()?.GetValue());
            var results = await CreateFromAsyncList(SearchQry.Statement(), SearchQry.Params());
            AsRecordSource().ReplaceRange(results);
            GoFirst();
        }

        public override async void OnOptionFilterClicked(FilterEventArgs e)
        {
            ReloadSearchQry();
            SearchQry.AddParameter("treatmentID", ParentRecord?.GetTablePK()?.GetValue());
            ServiceOptions.Conditions(SearchQry);
            DentistOptions.Conditions(SearchQry);
            DatesOptions.Conditions(SearchQry);
            var results = await CreateFromAsyncList(SearchQry.Statement(), SearchQry.Params());
            AsRecordSource().ReplaceRange(results);
            GoFirst();
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

        public override IWhereClause InstantiateSearchQry()
        {
            return new Appointment().From().InnerJoin(new Dentist()).InnerJoin(new Service()).Where().EqualsTo("TreatmentID", "@treatmentID");
        }
    }

    public class AppointListController2 : AbstractAppointmentListController 
    {
        public AppointListController2() 
        {
            AllowNewRecord = false;
            OpenWindowOnNew = true;
        }

        public void SetTreatment(Treatment? treatment) => ParentRecord = treatment;

        public override async void OnOptionFilterClicked(FilterEventArgs e)
        {
            ReloadSearchQry();
            ServiceOptions.Conditions(SearchQry);
            DentistOptions.Conditions(SearchQry);
            DatesOptions.Conditions(SearchQry);
            RecordSource<Appointment> results = await CreateFromAsyncList(SearchQry.Statement(), SearchQry.Params());
            AsRecordSource().ReplaceRange(results);
            GoFirst();
        }
        
        public void TriggerFilter(DateTime? date) 
        {
            DatesOptions.FirstOrDefault(s => s.Value.Equals(date)).IsSelected = true;
            OnOptionFilterClicked(new());
        }

        public override IWhereClause InstantiateSearchQry()
        {
            return new Appointment().From().InnerJoin(new Dentist()).InnerJoin(new Service()).Where();
        }

    }
}