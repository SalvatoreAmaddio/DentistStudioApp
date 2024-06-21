using Backend.ExtensionMethods;
using Backend.Model;
using DentistStudioApp.Model;
using FrontEnd.Controller;
using FrontEnd.Events;
using FrontEnd.ExtensionMethods;
using FrontEnd.Source;
using FrontEnd.Utils;
using System.ComponentModel;
using System.Windows;

namespace DentistStudioApp.Controller
{
    public class TreatmentController : AbstractFormController<Treatment>
    {
        private Patient? _patient;
        public Patient? Patient { get => _patient; set => UpdateProperty(ref value, ref _patient); }
        public AppointmentListController Appointments { get; } = new();
        public override int DatabaseIndex => 7;
        public TreatmentController()
        {
            AddSubControllers(Appointments);
            NewRecordEvent += OnNewRecordEvent;
        }

        protected override async void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            SearchQry.AddParameter("patientID", Patient?.PatientID);
            RecordSource<Treatment> results = await Task.Run(()=>CreateFromAsyncList(SearchQry.Statement(), SearchQry.Params()));
            AsRecordSource().ReplaceRange(results);
            GoFirst();
        }

        protected override async void OnWindowClosed(object? sender, EventArgs e)
        {
            TreatmentListController? treatmentListController = Helper.GetActiveWindow()?.GetController<PatientController>()?.GetSubController<TreatmentListController>(0);
            if (treatmentListController != null)
                await treatmentListController.RequeryAsync();
            DisposeWindow();
        }

        private void OnNewRecordEvent(object? sender, AllowRecordMovementArgs e)
        {
            if (CurrentRecord!=null) 
            {
                CurrentRecord.Patient = Patient;
                CurrentRecord.IsDirty = false;
            }
        }

        public override AbstractClause InstantiateSearchQry() =>
        new Treatment().From().Where().EqualsTo("PatientID","@patientID").OrderBy().Field("StartDate DESC");
    }
}
