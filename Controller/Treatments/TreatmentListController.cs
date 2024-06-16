using Backend.Database;
using Backend.ExtensionMethods;
using Backend.Model;
using DentistStudioApp.Model;
using DentistStudioApp.View;
using FrontEnd.Controller;
using FrontEnd.Events;
using FrontEnd.FilterSource;
using FrontEnd.Source;
using System.Windows.Input;
using Backend.Enums;

namespace DentistStudioApp.Controller
{
    public class TreatmentListController : AbstractFormListController<Treatment>
    {
        #region Properties
        public override int DatabaseIndex => 7;
        public SourceOption DatesOptions { get; private set; }
        public SourceOption DatesOptions2 { get; private set; }
        public SourceOption ServiceCountOptions { get; private set; }
        #endregion

        public TreatmentListController()
        {
            DatesOptions = new PrimitiveSourceOption(this, "StartDate");
            DatesOptions2 = new PrimitiveSourceOption(this, "EndDate");
            ServiceCountOptions = new PrimitiveSourceOption(this, "ServiceCount");
        }

        public override async void OnSubFormFilter()
        {
            ReloadSearchQry();
            SearchQry.AddParameter("patientID", ParentRecord?.GetPrimaryKey()?.GetValue());
            RecordSource<Treatment> results = await CreateFromAsyncList(SearchQry.Statement(), SearchQry.Params());            
            AsRecordSource().ReplaceRange(results);
            GoFirst();
        }
        

        public override async void OnOptionFilterClicked(FilterEventArgs e) 
        {
            ReloadSearchQry();
            DatesOptions.Conditions<WhereClause>(SearchQry);
            DatesOptions2.Conditions<WhereClause>(SearchQry);
            ServiceCountOptions.Conditions<HavingClause>(SearchQry);
            SearchQry.AddParameter("patientID", ParentRecord?.GetPrimaryKey()?.GetValue());
            RecordSource<Treatment> results = await CreateFromAsyncList(SearchQry.Statement(), SearchQry.Params());
            AsRecordSource().ReplaceRange(results);
            GoFirst();
        }

        public override Task<IEnumerable<Treatment>> SearchRecordAsync() => throw new NotImplementedException();

        protected override void Open(Treatment model)
        {
            model.Patient = (Patient?)ParentRecord;
            model.IsDirty = false;
            TreatmentForm? win = new(model);
            win.ShowDialog();
        }

        public override AbstractClause InstantiateSearchQry() =>
            new Treatment().Select()
                .All()
                .Fields("count(Service.ServiceID) AS ServiceCount")
                .From().LeftJoin(nameof(Appointment), "TreatmentID")
                .LeftJoin(nameof(Appointment), nameof(Service), "ServiceID")
                .Where().EqualsTo("PatientID", "@patientID")
                .GroupBy().Fields("Treatment.TreatmentID")
                .OrderBy().Field("StartDate DESC");
    }

    public abstract class AbstractTreatmentInvoice : TreatmentListController
    {
        public Patient? Patient;

        #region Properties
        protected IAbstractDatabase? InvoicedTreatmentDB = DatabaseManager.Find<InvoicedTreatment>();
        public Invoice? CurrentInvoice => (Invoice?)ParentRecord;
        public ICommand InvoiceTreatmentCMD { get; }
        public abstract CRUD Crud { get; }
        private bool Invoicing => Crud == CRUD.INSERT;
        #endregion

        public AbstractTreatmentInvoice() => InvoiceTreatmentCMD = new CMDAsync(InvoiceTreatmentTask);
        protected override void Open(Treatment model)
        {
            model.Patient = Patient;
            model.IsDirty = false;
            TreatmentForm? win = new(model);
            win.ShowDialog();
        }
        protected virtual async Task InvoiceTreatmentTask()
        {
            if (InvoicedTreatmentDB == null) throw new NullReferenceException();
            if (CurrentRecord == null) throw new NullReferenceException();

            Task<object?> total = Task.Run(CurrentRecord.GetTotalCost);

            string? sql = new InvoicedTreatment().Where().EqualsTo("TreatmentID", "@id").Statement();
            List<QueryParameter> para = [new("id", CurrentRecord.TreatmentID)];

            InvoicedTreatment? invoicedTreatment = null;

            if (Invoicing) 
                invoicedTreatment = new(CurrentInvoice, CurrentRecord);
            else 
            {
                RecordSource<InvoicedTreatment> records = await RecordSource<InvoicedTreatment>.CreateFromAsyncList(InvoicedTreatmentDB.RetrieveAsync(sql, para).Cast<InvoicedTreatment>());
                invoicedTreatment = records.FirstOrDefault();
            }

            if (invoicedTreatment == null) return;

            object? amount = await total;

            if (amount != null)
                if (Invoicing)
                    CurrentInvoice?.SetAmount((double)amount);
                else 
                    CurrentInvoice?.RemoveAmount((double)amount);

            InvoicedTreatmentDB.Model = invoicedTreatment;
            InvoicedTreatmentDB.Crud(Crud);

            CurrentRecord.Invoiced = Invoicing;
            PerformUpdate();

            if (ParentController != null) //Requery ChildSources of Invoices
            {
                CurrentInvoice!.IsDirty = true;
                ParentController.CurrentModel = CurrentInvoice;
                ParentController.PerformUpdate();
            }
        }
        public override async void OnSubFormFilter()
        {
            ReloadSearchQry();
            IEnumerable<Treatment> results = await SearchRecordAsync();

            if (results == null) throw new NullReferenceException();
            AsRecordSource().ReplaceRange(results);

            GoFirst();
        }
        public override async void OnOptionFilterClicked(FilterEventArgs e)
        {
            ReloadSearchQry();
            DatesOptions.Conditions<WhereClause>(SearchQry);
            DatesOptions2.Conditions<WhereClause>(SearchQry);
            ServiceCountOptions.Conditions<HavingClause>(SearchQry);
            IEnumerable<Treatment> results = await SearchRecordAsync();
            AsRecordSource().ReplaceRange(results);
            GoFirst();
        }
    }

    public class TreatmentToInvoiceListController : AbstractTreatmentInvoice
    {
        public override CRUD Crud => CRUD.INSERT;

        public override async Task<IEnumerable<Treatment>> SearchRecordAsync()
        {
            SearchQry.AddParameter("patientID", Patient?.PatientID);
            return await CreateFromAsyncList(SearchQry.Statement(), SearchQry.Params());
        }

        public override AbstractClause InstantiateSearchQry() =>
                new Treatment().Select().All().Fields("count(Service.ServiceID) AS ServiceCount")
                .From()
                .LeftJoin(nameof(InvoicedTreatment), "TreatmentID")
                .LeftJoin(nameof(Appointment), "TreatmentID")
                .LeftJoin(nameof(Appointment), nameof(Service), "ServiceID")
                .Where()
                .EqualsTo("Treatment.PatientID", "@patientID").AND().IsNull("InvoicedTreatment.InvoiceID")
                .GroupBy().Fields("Treatment.TreatmentID").OrderBy().Field("StartDate DESC");
    }

    public class TreatmentInvoicedListController : AbstractTreatmentInvoice
    {
        public override CRUD Crud => CRUD.DELETE;

        public async override Task<IEnumerable<Treatment>> SearchRecordAsync()
        {
            SearchQry.AddParameter("patientID", Patient?.PatientID);
            SearchQry.AddParameter("invoiceID", CurrentInvoice?.InvoiceID);
            return await CreateFromAsyncList(SearchQry.Statement(), SearchQry.Params());
        }

        public override AbstractClause InstantiateSearchQry() =>
            new Treatment().Select().All().Fields("count(Service.ServiceID) AS ServiceCount")
            .From()
            .LeftJoin(nameof(InvoicedTreatment), "TreatmentID")
            .LeftJoin(nameof(Appointment), "TreatmentID")
            .LeftJoin(nameof(Appointment), nameof(Service), "ServiceID")
            .Where()
            .EqualsTo("Treatment.PatientID", "@patientID").AND().EqualsTo("InvoicedTreatment.InvoiceID", "@invoiceID")
            .GroupBy().Fields("Treatment.TreatmentID").OrderBy().Field("StartDate DESC");
    }
}