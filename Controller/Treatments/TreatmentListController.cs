using Backend.Database;
using Backend.ExtensionMethods;
using Backend.Model;
using DentistStudioApp.Model;
using DentistStudioApp.View;
using FrontEnd.Controller;
using FrontEnd.Events;
using FrontEnd.Source;
using System.Windows.Input;

namespace DentistStudioApp.Controller
{
    public class TreatmentListController : AbstractFormListController<Treatment>
    {
        public override int DatabaseIndex => 7;

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

        public override void OnOptionFilterClicked(FilterEventArgs e) 
        {
            throw new NotImplementedException();
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
        protected abstract Task InvoiceTreatmentTask();
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
        protected override async Task InvoiceTreatmentTask()
        {
            if (CurrentRecord == null) throw new NullReferenceException();
            if (InvoicedTreatmentDB == null) throw new NullReferenceException();

            
            Task<object?> total = Task.Run(CurrentRecord.GetTotalCost);

            InvoicedTreatment invoicedTreatment = new()
            {
                Treatment = CurrentRecord,
                Invoice = CurrentInvoice
            };

            object? amount = await total;

            if (amount != null) 
            {
                CurrentInvoice?.SetAmount((double)amount);
                if (ParentController!=null) 
                {
                    CurrentInvoice!.IsDirty = true;
                    ParentController.CurrentModel = CurrentInvoice;
                    ParentController.PerformUpdate();
                }
            }

            InvoicedTreatmentDB.Model = invoicedTreatment;
            InvoicedTreatmentDB.Crud(CRUD.INSERT);

            CurrentRecord.Invoiced = true;
            PerformUpdate();

            if (ParentController != null)
            {
                CurrentInvoice!.IsDirty = true;
                ParentController.CurrentModel = CurrentInvoice;
                ParentController.PerformUpdate();
            }
        }

        public override Task<IEnumerable<Treatment>> FetchInvoiceTask() => Treatment.GetToInvoice(Patient.PatientID);
    }

    public class TreatmentInvoicedListController : AbstractTreatmentInvoice
    {
        protected override async Task InvoiceTreatmentTask()
        {
            if (InvoicedTreatmentDB == null) throw new NullReferenceException();
            if (CurrentRecord == null) throw new NullReferenceException();

            Task<object?> total = Task.Run(CurrentRecord.GetTotalCost);

            string? sql = new InvoicedTreatment().Where().EqualsTo("TreatmentID", "@id").Statement();
            List<QueryParameter> para = [new("id", CurrentRecord.TreatmentID)];

            var records = await RecordSource<InvoicedTreatment>.CreateFromAsyncList(InvoicedTreatmentDB.RetrieveAsync(sql, para).Cast<InvoicedTreatment>());

            InvoicedTreatment? toRemove = records.FirstOrDefault();

            if (toRemove == null) return;

            object? amount = await total;

            if (amount != null)
            {
                CurrentInvoice?.RemoveAmount((double)amount);
                if (ParentController != null)
                {
                    CurrentInvoice!.IsDirty = true;
                    ParentController.CurrentModel = CurrentInvoice;
                    ParentController.PerformUpdate();
                }
            }

            InvoicedTreatmentDB.Model = toRemove;
            InvoicedTreatmentDB.Crud(CRUD.DELETE);

            CurrentRecord.Invoiced = false;
            PerformUpdate();

            if (ParentController != null)
            {
                CurrentInvoice!.IsDirty = true;
                ParentController.CurrentModel = CurrentInvoice;
                ParentController.PerformUpdate();
            }
        }

        public override Task<IEnumerable<Treatment>> FetchInvoiceTask() => Treatment.GetInvoiced(Patient?.PatientID, CurrentInvoice?.InvoiceID);
    }
}