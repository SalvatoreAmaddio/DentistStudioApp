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

namespace DentistStudioApp.Controller
{
    public class TreatmentListController : AbstractFormListController<Treatment>
    {
        public override int DatabaseIndex => 7;
        public SourceOption DatesOptions { get; private set; }
        public SourceOption DatesOptions2 { get; private set; }

        public TreatmentListController() 
        {
            DatesOptions = new PrimitiveSourceOption(this, "StartDate");
            DatesOptions2 = new PrimitiveSourceOption(this, "EndDate");
        }

        public override async void OnSubFormFilter()
        {
            List<Task> serviceCountTasks = [];
            ReloadSearchQry();
            SearchQry.AddParameter("patientID", ParentRecord?.GetTablePK()?.GetValue());
            var results = await CreateFromAsyncList(SearchQry.Statement(), SearchQry.Params());
            
            if (results.Count > 0)
                foreach (Treatment record in results)
                    serviceCountTasks.Add(record.CountServices());

            AsRecordSource().ReplaceRange(results);
            GoFirst();
            await Task.WhenAll(serviceCountTasks);
        }

        public override async void OnOptionFilterClicked(FilterEventArgs e) 
        {
            ReloadSearchQry();
            DatesOptions.Conditions(SearchQry);
            DatesOptions2.Conditions(SearchQry);
            SearchQry.AddParameter("patientID", ParentRecord?.GetTablePK()?.GetValue());
            RecordSource<Treatment> results = await CreateFromAsyncList(SearchQry.Statement(), SearchQry.Params());
            AsRecordSource().ReplaceRange(results);
            GoFirst();

        }

        public override Task<IEnumerable<Treatment>> SearchRecordAsync()
        {
            throw new NotImplementedException();
        }

        protected override void Open(Treatment? model)
        {
            if (model!=null) 
            {
                model.Patient = (Patient?)ParentRecord;
                model.IsDirty = false;
            }
            TreatmentForm? win = new(model);
            win.ShowDialog();
        }

        public override IWhereClause InstantiateSearchQry() => new Treatment().Where().EqualsTo("PatientID", "@patientID");
    }

    public abstract class AbstractTreatmentInvoice : TreatmentListController 
    {
        public Patient? Patient;
        protected IAbstractDatabase? InvoicedTreatmentDB = DatabaseManager.Find<InvoicedTreatment>();
        public Invoice? CurrentInvoice => (Invoice?)ParentRecord;
        public ICommand InvoiceTreatmentCMD { get; }
        public abstract CRUD Crud { get; }
        private bool Invoicing => Crud == CRUD.INSERT;
        public AbstractTreatmentInvoice() => InvoiceTreatmentCMD = new CMDAsync(InvoiceTreatmentTask);
        protected override void Open(Treatment? model)
        {
            if (model != null)
            {
                model.Patient = Patient;
                model.IsDirty = false;
            }
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
        public abstract Task<IEnumerable<Treatment>> FetchInvoiceTask();
        public override async void OnSubFormFilter()
        {
            List<Task> serviceCountTasks = [];

            IEnumerable<Treatment> ToInvoice = await FetchInvoiceTask();

            if (ToInvoice?.Count() > 0)
                foreach (Treatment record in ToInvoice)
                    serviceCountTasks.Add(record.CountServices());

            if (ToInvoice == null) throw new NullReferenceException();
            AsRecordSource().ReplaceRange(ToInvoice);

            GoFirst();

            await Task.WhenAll(serviceCountTasks);
        }
    }

    public class TreatmentToInvoiceListController : AbstractTreatmentInvoice
    {
        public override CRUD Crud => CRUD.INSERT;

        public override Task<IEnumerable<Treatment>> FetchInvoiceTask() => Treatment.GetToInvoice(Patient?.PatientID);
    }

    public class TreatmentInvoicedListController : AbstractTreatmentInvoice
    {
        public override CRUD Crud => CRUD.DELETE;
        public override Task<IEnumerable<Treatment>> FetchInvoiceTask() => Treatment.GetInvoiced(Patient?.PatientID, CurrentInvoice?.InvoiceID);
    }
}