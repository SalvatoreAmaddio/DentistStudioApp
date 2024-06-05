using Backend.Database;
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
        public override string SearchQry { get; set; } = $"SELECT * FROM {nameof(Treatment)} WHERE PatientID = @patientID;";

        public override int DatabaseIndex => 7;

        public TreatmentListController()
        {
        }

        public override async void OnSubFormFilter()
        {
            List<Task> serviceCountTasks = [];
            QueryBuiler.Clear();
            QueryBuiler.AddParameter("patientID", ParentRecord?.GetTablePK()?.GetValue());
            var results = await CreateFromAsyncList(QueryBuiler.Query, QueryBuiler.Params);
            
            if (results.Count > 0)
                foreach (Treatment record in results)
                    serviceCountTasks.Add(record.CountServices());

            AsRecordSource().ReplaceRange(results);
            GoFirst();
            await Task.WhenAll(serviceCountTasks);
        }

        public override void OnOptionFilter(FilterEventArgs e)
        {
        }

        public override Task<IEnumerable<Treatment>> SearchRecordAsync()
        {
            throw new NotImplementedException();
        }

        
        protected override void Open(Treatment? model)
        {
            model.Patient = (Patient?)ParentRecord;
            model.IsDirty = false;
            TreatmentForm? win = new(model);
            win.ShowDialog();
        }
    }

    public class TreatmentToInvoiceListController : TreatmentListController
    {
        public Patient? Patient;
        private IAbstractDatabase? InvoicedTreatmentDB = DatabaseManager.Find<InvoicedTreatment>();
        public ICommand AddInvoiceCMD { get; }
        public ICommand RemoveInvoiceCMD { get; }

        public TreatmentToInvoiceListController() 
        {
            AddInvoiceCMD = new CMDAsync(Invoice);
            RemoveInvoiceCMD = new CMDAsync(Remove);
        }

        public Invoice? CurrentInvoice => (Invoice?)ParentRecord;
        private async Task Invoice() 
        {
            Task<object> total = Task.Run(CurrentRecord.GetTotalCost);
            InvoicedTreatment invoicedTreatment = new()
            {
                Treatment = CurrentRecord,
                Invoice = CurrentInvoice
            };

            InvoicedTreatmentDB.Model = invoicedTreatment;
            InvoicedTreatmentDB.Crud(CRUD.INSERT);
            CurrentRecord.Invoiced = true;
            CurrentRecord.IsDirty = false;
            object? amount = await total;
            CurrentInvoice?.SetAmount((double)amount);
            PerformUpdate();
        }

        private async Task  Remove()
        {
            Task<object> total = Task.Run(CurrentRecord.GetTotalCost);
            string? sql = $"SELECT * FROM {nameof(InvoicedTreatment)} WHERE TreatmentID = @id";
            List<QueryParameter> para = [];
            para.Add(new("id", CurrentRecord.TreatmentID));
            var records  = await RecordSource<InvoicedTreatment>.CreateFromAsyncList(InvoicedTreatmentDB.RetrieveAsync(sql, para).Cast<InvoicedTreatment>());
            InvoicedTreatment? toRemove = records.FirstOrDefault();
            if (toRemove == null) return;
            object? amount = await total;
            CurrentInvoice?.RemoveAmount((double)amount);
            InvoicedTreatmentDB.Model = toRemove;
            InvoicedTreatmentDB.Crud(CRUD.DELETE);
            CurrentRecord.Invoiced = false;
            CurrentRecord.IsDirty = false;
            PerformUpdate();
        }

        public override async void OnSubFormFilter()
        {
            List<Task> serviceCountTasks = [];

            IEnumerable<Treatment> ToInvoice = await Treatment.GetUnvoiced(Patient.PatientID);

            if (ToInvoice?.Count() > 0)
                foreach (Treatment record in ToInvoice)
                    serviceCountTasks.Add(record.CountServices());

            AsRecordSource().ReplaceRange(ToInvoice);
            GoFirst();
            await Task.WhenAll(serviceCountTasks);
        }

        protected override void Open(Treatment? model)
        {
            model.Patient = Patient;
            model.IsDirty = false;
            TreatmentForm? win = new(model);
            win.ShowDialog();
        }
    }
}
