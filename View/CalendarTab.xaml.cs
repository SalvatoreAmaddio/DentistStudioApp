using Backend.Database;
using DentistStudioApp.Model;
using FrontEnd.Events;
using FrontEnd.Forms.Calendar;
using System.Windows.Controls;
using System.Windows.Input;

namespace DentistStudioApp.View
{
    public partial class CalendarTab : Page
    {
        private IAbstractDatabase? AppointmentDB = DatabaseManager.Find<Appointment>();
        private IAbstractDatabase? treatmentDB = DatabaseManager.Find<Treatment>();
        public CalendarTab()
        {
            InitializeComponent();
        }
        
        private async void CalendarForm_MouseUp(object sender, MouseButtonEventArgs e)
        {
            IEnumerable<Treatment>? treatments = ((CalendarForm)sender).SelectedSlot.Models as IEnumerable<Treatment>;
            if (treatments == null || treatments.Count()==0) return;
            foreach(Treatment treatment in treatments) 
            {
                await treatment.FetchPatientAsync();
            }
//            TreatmentForm treatmentForm = new(treatment);
//            treatmentForm.ShowDialog();
        }

        private void CalendarForm_OnPreparing(object sender, OnPreparingCalendarFormEventArgs e)
        {
            Appointment? firstAppointment = AppointmentDB?.MasterSource.Cast<Appointment>().FirstOrDefault(s => s.DOA == e.Date);
            if (firstAppointment != null)
                e.Model = treatmentDB?.MasterSource.Cast<Treatment>().Where(s => s.Equals(firstAppointment.Treatment));
        }
    }
}
