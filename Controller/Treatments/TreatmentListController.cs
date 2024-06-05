using DentistStudioApp.Model;
using DentistStudioApp.View;
using FrontEnd.Controller;
using FrontEnd.Events;

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
