using Backend.Database;
using DentistStudioApp.Model;
using FrontEnd.Events;
using FrontEnd.Forms.Calendar;
using System.Windows;
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
            Treatment? treatment = ((CalendarForm)sender).SelectedSlot.Model as Treatment;
            if (treatment == null) return;
            await treatment.FetchPatientAsync();
            TreatmentForm treatmentForm = new(treatment);
            treatmentForm.ShowDialog();
        }

        private void CalendarForm_OnPreparing(object sender, OnPreparingCalendarFormEventArgs e)
        {
            Appointment? firstAppointment = AppointmentDB?.MasterSource.Cast<Appointment>().FirstOrDefault(s => s.DOA == e.Date);
            if (firstAppointment != null)
                e.Model = treatmentDB?.MasterSource.Cast<Treatment>().FirstOrDefault(s => s.Equals(firstAppointment.Treatment));
        }
    }
}
