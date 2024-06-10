using DentistStudioApp.Controller;
using DentistStudioApp.Model;
using FrontEnd.ExtensionMethods;
using FrontEnd.Model;
using System.Windows;

namespace DentistStudioApp.View
{
    public partial class AppointmentListFormWin : Window
    {
        private AppointmentListController Controller;
        public AppointmentListFormWin()
        {
            InitializeComponent();
            this.SetController(new AppointmentListController());
            Controller = this.GetController<AppointmentListController>();
            Controller.AllowNewRecord = false;
            Controller.OpenWindowOnNew = true;
        }

        public AppointmentListFormWin(DateTime? date) : this()
        {
            Appointment? appointment = Controller.AsRecordSource().FirstOrDefault(s=>s.DOA.Equals(date)); 
            Controller.SetTreatment(appointment?.Treatment);
            Controller.ReloadFilters();
            Controller.DatesOptions.FirstOrDefault(s => s.Value.Equals(date)).IsSelected = true;
            Controller.OnOptionFilterClicked(new());
        }


    }
}
