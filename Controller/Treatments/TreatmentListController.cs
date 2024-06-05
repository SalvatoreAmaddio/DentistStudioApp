﻿using Backend.Database;
using Backend.ExtensionMethods;
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
        public override string SearchQry { get; set; } =  new Treatment().Where().EqualsTo("PatientID", "@patientID").Statement();

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
            if (model!=null) 
            {
                model.Patient = (Patient?)ParentRecord;
                model.IsDirty = false;
            }
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
            AddInvoiceCMD = new CMDAsync(InvoiceTreatment);
            RemoveInvoiceCMD = new CMDAsync(RemoveTreatment);
        }

        public Invoice? CurrentInvoice => (Invoice?)ParentRecord;
        private async Task InvoiceTreatment() 
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
                CurrentInvoice?.SetAmount((double)amount);

            InvoicedTreatmentDB.Model = invoicedTreatment;
            InvoicedTreatmentDB.Crud(CRUD.INSERT);

            CurrentRecord.Invoiced = true;
            PerformUpdate();
        }

        private async Task RemoveTreatment()
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
                CurrentInvoice?.RemoveAmount((double)amount);
 
            InvoicedTreatmentDB.Model = toRemove;
            InvoicedTreatmentDB.Crud(CRUD.DELETE);

            CurrentRecord.Invoiced = false;
            PerformUpdate();
        }

        public override async void OnSubFormFilter()
        {
            List<Task> serviceCountTasks = [];

            if (Patient == null) throw new NullReferenceException();

            IEnumerable<Treatment> ToInvoice = await Treatment.GetUnvoiced(Patient.PatientID);

            if (ToInvoice?.Count() > 0)
                foreach (Treatment record in ToInvoice)
                    serviceCountTasks.Add(record.CountServices());

            if (ToInvoice == null) throw new NullReferenceException();
            AsRecordSource().ReplaceRange(ToInvoice);

            GoFirst();

            await Task.WhenAll(serviceCountTasks);
        }

        protected override void Open(Treatment? model)
        {
            if (model!=null) 
            {
                model.Patient = Patient;
                model.IsDirty = false;
            }
            TreatmentForm? win = new(model);
            win.ShowDialog();
        }
    }
}