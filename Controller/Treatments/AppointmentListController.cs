using Backend.Database;
using Backend.ExtensionMethods;
using Backend.Model;
using FrontEnd.Source;
using DentistStudioApp.Model;
using DentistStudioApp.View;
using FrontEnd.Controller;
using FrontEnd.Events;
using FrontEnd.FilterSource;
using Backend.Events;

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

        public override Task<IEnumerable<Appointment>> SearchRecordAsync() => throw new NotImplementedException();

        protected override async void Open(Appointment? model)
        {
            if (model == null) return;
            await model.SetTreatmentAsync();
            TreatmentForm treatmentForm = new(new(model));
            treatmentForm.ShowDialog();
        }
    }

    public class AppointmentListController : AbstractAppointmentListController
    {
        public AppointmentListController() => AfterRecordNavigation += OnRecordMoving;

        private void OnRecordMoving(object? sender, AllowRecordMovementArgs e)
        {
            if (e.NewRecord)
            {
                Treatment? treatment = (Treatment?)ParentRecord;
                if (CurrentRecord != null)
                {
                    CurrentRecord.Treatment = treatment;
                    CurrentRecord.Clean();
                }
            }
        }

        public override async void OnSubFormFilter()
        {
            ReloadSearchQry();
            SearchQry.AddParameter("treatmentID", ParentRecord?.GetPrimaryKey()?.GetValue());
            var results = await CreateFromAsyncList(SearchQry.Statement(), SearchQry.Params());
            AsRecordSource().ReplaceRecords(results);
            GoFirst();
            InvokeAfterSubFormFilterEvent();
        }

        public override async void OnOptionFilterClicked(FilterEventArgs e)
        {
            ReloadSearchQry();

            SearchQry.AddParameter("treatmentID", ParentRecord?.GetPrimaryKey()?.GetValue());
            ServiceOptions.Conditions<WhereClause>(SearchQry);
            DentistOptions.Conditions<WhereClause>(SearchQry);
            AttendedOptions.Conditions<WhereClause>(SearchQry);
            RoomsOptions.Conditions<WhereClause>(SearchQry);
            DatesOptions.Conditions<WhereClause>(SearchQry);
            TimesOptions.Conditions<WhereClause>(SearchQry);
                
            var results = await CreateFromAsyncList(SearchQry.Statement(), SearchQry.Params());
            AsRecordSource().ReplaceRange(results);
            GoFirst();
        }

        public override AbstractClause InstantiateSearchQry() =>
        new Appointment()
            .Select()
            .From().InnerJoin(new Dentist()).InnerJoin(new Service())
            .Where().EqualsTo("TreatmentID", "@treatmentID");
    }

    public class AppointmentListController2 : AbstractAppointmentListController 
    {
        public AppointmentListController2()
        {
            AllowNewRecord = false;
            OpenWindowOnNew = true;
            AfterUpdate += OnAfterUpdate;
        }

        private async void OnAfterUpdate(object? sender, AfterUpdateArgs e)
        {
            if (!e.Is(nameof(Search))) return;
            await OnSearchPropertyRequeryAsync(sender);
            if (sender is FilterEventArgs filterArgs)
            { 
                if (filterArgs.HasMessages) 
                    GoFirst();
            }
        }

        public void SetTreatment(Treatment? treatment) => ParentRecord = treatment;

        public override void OnOptionFilterClicked(FilterEventArgs e)
        {
            ReloadSearchQry();
            ServiceOptions.Conditions<WhereClause>(SearchQry);
            DentistOptions.Conditions<WhereClause>(SearchQry);
            AttendedOptions.Conditions<WhereClause>(SearchQry);
            RoomsOptions.Conditions<WhereClause>(SearchQry);
            DatesOptions.Conditions<WhereClause>(SearchQry);
            TimesOptions.Conditions<WhereClause>(SearchQry);
            OnAfterUpdate(e, new(null, null, nameof(Search)));
        }

        public void TriggerFilter(DateTime? date)
        {
            DatesOptions.FirstOrDefault(s => PredicateTriggerFilter(s, date))?.Select();
            OnOptionFilterClicked(new() { Messages = ["GoFirst"] });
        }

        private static bool PredicateTriggerFilter(IFilterOption? option, DateTime? date)
        {
            if (option == null) return false;
            return (option.Value == null) ? false : option.Value.Equals(date);
        }

        public override async Task<IEnumerable<Appointment>> SearchRecordAsync() 
        {
            SearchQry.AddParameter("name", Search.ToLower() + "%");
            SearchQry.AddParameter("name", Search.ToLower() + "%");
            return await CreateFromAsyncList(SearchQry.Statement(), SearchQry.Params());
        }

        public override AbstractClause InstantiateSearchQry() =>
        new Appointment()
            .Select()
            .From()
                .InnerJoin(new Dentist()).InnerJoin(new Service()).InnerJoin(new Treatment())
                .InnerJoin("Treatment","Patient","PatientID")
            .Where()
                .OpenBracket()
                    .Like("LOWER(Patient.FirstName)", "@name")
                    .OR()
                    .Like("LOWER(Patient.LastName)", "@name")
                .CloseBracket()
            .OrderBy().Field("DOA").Field("TOA");
    }
}