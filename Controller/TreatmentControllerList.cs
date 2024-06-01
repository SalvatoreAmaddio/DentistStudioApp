﻿using DentistStudioApp.Model;
using DentistStudioApp.View;
using FrontEnd.Controller;
using FrontEnd.Events;

namespace DentistStudioApp.Controller
{
    public class TreatmentController : AbstractFormController<Treatment>
    {
        public override int DatabaseIndex => 7;

    }

    public class TreatmentControllerList : AbstractFormListController<Treatment>
    {
        public override string SearchQry { get; set; } = $"SELECT * FROM {nameof(Treatment)} WHERE PatientID = @patientID;";

        public override int DatabaseIndex => 7;

        public TreatmentControllerList() 
        {

        }

        public override async void OnSubFormFilter()
        {
            QueryBuiler.Clear();
            QueryBuiler.AddParameter("patientID", ParentRecord?.GetTablePK()?.GetValue());
            var results = await CreateFromAsyncList(QueryBuiler.Query, QueryBuiler.Params);
            AsRecordSource().ReplaceRange(results);
            GoFirst();
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
            if (model.IsNewRecord()) 
            {
                model.Patient = (Patient?)ParentRecord;
                model.IsDirty = false;
            }

            TreatmentForm? win = new(model);
            win.ShowDialog();
        }
    }
}
