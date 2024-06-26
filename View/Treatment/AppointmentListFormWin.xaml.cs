using DentistStudioApp.Controller;
using DentistStudioApp.Model;
using FrontEnd.ExtensionMethods;
using System.Windows;

namespace DentistStudioApp.View
{
    public partial class AppointmentListFormWin : Window
    {
        private AppointmentListController2? Controller;
        public AppointmentListFormWin()
        {
            InitializeComponent();
            this.SetController(new AppointmentListController2());
            Controller = this.GetController<AppointmentListController2>();
        }

        public AppointmentListFormWin(DateTime? date) : this()
        {
            Appointment? appointment = Controller?.AsRecordSource().FirstOrDefault(s => s.DOA.Equals(date));
            Controller?.SetTreatment(appointment?.Treatment);
            Controller?.ReloadFilters();
            Controller?.TriggerFilter(date);
        }


    }
}
