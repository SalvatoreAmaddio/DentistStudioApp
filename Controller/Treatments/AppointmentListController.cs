using Backend.Database;
using Backend.ExtensionMethods;
using Backend.Model;
using FrontEnd.Source;
using DentistStudioApp.Model;
using DentistStudioApp.View;
using FrontEnd.Controller;
using FrontEnd.Events;
using FrontEnd.FilterSource;

namespace DentistStudioApp.Controller
{
    public abstract class AbstractAppointmentListController : AbstractFormListController<Appointment>
    {
        public SourceOption ServiceOptions { get; private set; }
        public SourceOption DentistOptions { get; private set; }
        public SourceOption AttendedOptions { get; private set; }
        public SourceOption DatesOptions { get; private set; }
        public SourceOption TimesOptions { get; private set; }
        public SourceOption RoomsOptions { get; private set; }
        public RecordSource<Service> Services { get; private set; } = new(DatabaseManager.Find<Service>()!);
        public RecordSource<Dentist> Dentists { get; private set; } = new(DatabaseManager.Find<Dentist>()!);
        public override int DatabaseIndex => 9;

        public AbstractAppointmentListController()
        {
            ServiceOptions = new(Services, "ServiceName");
            DentistOptions = new(Dentists, "FullName");
            AttendedOptions = new PrimitiveSourceOption(this, "Attended");
            DatesOptions = new PrimitiveSourceOption(this, "DOA");
            TimesOptions = new PrimitiveSourceOption(this, "TOA");
            RoomsOptions = new PrimitiveSourceOption(this, "RoomNumber");
            OpenWindowOnNew = false;
        }

        public void ReloadFilters()
        {
            ServiceOptions = new(Services, "ServiceName");
            DentistOptions = new(Dentists, "FullName");
            AttendedOptions = new PrimitiveSourceOption(this, "Attended");
            DatesOptions = new PrimitiveSourceOption(this, "DOA");
            TimesOptions = new PrimitiveSourceOption(this, "TOA");
            RoomsOptions = new PrimitiveSourceOption(this, "RoomNumber");
        }

        public override Task<IEnumerable<Appointment>> SearchRecordAsync()
        {
            throw new NotImplementedException();
        }

        protected override async void Open(Appointment? model)
        {
            if (model == null) return;
            await model.SetTreatmentAsync();
            TreatmentForm treatmentForm = new(model);
            treatmentForm.ShowDialog();
        }
    }

    public class AppointmentListController : AbstractAppointmentListController
    {
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
            SearchQry.AddParameter("treatmentID", ParentRecord?.GetPrimaryKey()?.GetValue());
            var results = await CreateFromAsyncList(SearchQry.Statement(), SearchQry.Params());
            AsRecordSource().ReplaceRecords(results);
            GoFirst();
            RunAfterSubFormFilterEvent();
        }

        public override async void OnOptionFilterClicked(FilterEventArgs e)
        {
            ReloadSearchQry();

            SearchQry.AddParameter("treatmentID", ParentRecord?.GetPrimaryKey()?.GetValue());
            ServiceOptions.Conditions(SearchQry);
            DentistOptions.Conditions(SearchQry);
            AttendedOptions.Conditions(SearchQry);
            RoomsOptions.Conditions(SearchQry);
            DatesOptions.Conditions(SearchQry);
            TimesOptions.Conditions(SearchQry);

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
            AttendedOptions.Conditions(SearchQry);
            RoomsOptions.Conditions(SearchQry);
            DatesOptions.Conditions(SearchQry);
            TimesOptions.Conditions(SearchQry);

            if (!SearchQry.HasWhereConditions()) 
                SearchQry.RemoveLastChange(); // remove WHERE

            string sql = SearchQry.OrderBy().Field("DOA").Field("TOA").Statement();
            RecordSource<Appointment> results = await CreateFromAsyncList(sql, SearchQry.Params());
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