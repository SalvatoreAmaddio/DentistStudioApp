﻿using Backend.Database;
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

        public override Task<IEnumerable<Appointment>> SearchRecordAsync() => throw new NotImplementedException();

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

        public override AbstractClause InstantiateSearchQry()
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
            AfterUpdate += OnAfterUpdate;
        }

        private async void OnAfterUpdate(object? sender, AfterUpdateArgs e)
        {
            if (!e.Is(nameof(Search))) return;
            await OnSearchPropertyRequeryAsync(sender);
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
            DatesOptions.FirstOrDefault(s => s.Value.Equals(date))?.Select();
            OnOptionFilterClicked(new());
        }

        public override async Task<IEnumerable<Appointment>> SearchRecordAsync() 
        {
            SearchQry.AddParameter("name", Search.ToLower() + "%");
            SearchQry.AddParameter("name", Search.ToLower() + "%");
            return await CreateFromAsyncList(SearchQry.Statement(), SearchQry.Params());
        }

        public override AbstractClause InstantiateSearchQry()
        {
            return new Appointment()
                .From()
                .InnerJoin(new Dentist()).InnerJoin(new Service()).InnerJoin(new Treatment())
                .InnerJoin("Treatment","Patient","PatientID")
                .Where().OpenBracket().Like("LOWER(Patient.FirstName)", "@name").OR().Like("LOWER(Patient.LastName)", "@name").CloseBracket()
                .OrderBy().Field("DOA").Field("TOA");
        }

    }
}