using Backend.Database;
using DentistStudioApp.Model;
using DentistStudioApp.View;
using FrontEnd.Controller;
using FrontEnd.Events;
using FrontEnd.Source;

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
        public IEnumerable<Treatment>? ToInvoice;
        public Patient? Patient;
        private IAbstractDatabase? InvoicedTreatmentDB = DatabaseManager.Find<InvoicedTreatment>();

        private void Invoice() 
        {
            InvoicedTreatment invoicedTreatment = new()
            {
                Treatment = CurrentRecord,
                Invoice = (Invoice?)ParentRecord
            };

            InvoicedTreatmentDB.Model = invoicedTreatment;
            InvoicedTreatmentDB.Crud(CRUD.UPDATE);
            CurrentRecord.Invoiced = true;
            PerformUpdate();
        }

        private async Task  Remove()
        {
            string? sql = $"SELECT * FROM {nameof(InvoicedTreatment)} WHERE TreatmentID = @id";
            List<QueryParameter> para = [];
            para.Add(new("name", CurrentRecord.TreatmentID));
            var records  = await RecordSource<InvoicedTreatment>.CreateFromAsyncList(InvoicedTreatmentDB.RetrieveAsync(sql, para).Cast<InvoicedTreatment>());
            InvoicedTreatment? toRemove = records.FirstOrDefault();
            if (toRemove == null) return;

            InvoicedTreatmentDB.Model = toRemove;
            InvoicedTreatmentDB.Crud(CRUD.DELETE);
            CurrentRecord.Invoiced = false;
            PerformUpdate();
        }

        public override async void OnSubFormFilter()
        {
            List<Task> serviceCountTasks = [];

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
            TreatmentForm? win = new(model);
            win.ShowDialog();
        }
    }
}
