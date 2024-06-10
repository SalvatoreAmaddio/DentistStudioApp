using DentistStudioApp.Controller;
using DentistStudioApp.Model;
using FrontEnd.ExtensionMethods;
using System.Windows;

namespace DentistStudioApp.View
{
    public partial class AppointmentListFormWin : Window
    {
        private AppointListController2 Controller;
        public AppointmentListFormWin()
        {
            InitializeComponent();
            this.SetController(new AppointListController2());
            Controller = this.GetController<AppointListController2>();
        }

        public AppointmentListFormWin(DateTime? date) : this()
        {
            Appointment? appointment = Controller.AsRecordSource().FirstOrDefault(s=>s.DOA.Equals(date)); 
            Controller.SetTreatment(appointment?.Treatment);
            Controller.ReloadFilters();
            Controller.TriggerFilter(date);
        }


    }
}
