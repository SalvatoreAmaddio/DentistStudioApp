using Backend.Database;
using DentistStudioApp.Controller;
using DentistStudioApp.Model;
using FrontEnd.Events;
using FrontEnd.Forms.Calendar;
using FrontEnd.Model;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DentistStudioApp.View
{
    public partial class CalendarTab : Page
    {
        private AppointmentListController AppointmentListController = new();
        private IAbstractDatabase? treatmentDB = DatabaseManager.Find<Treatment>();
        public CalendarTab()
        {
            InitializeComponent();
        }
        
        private void CalendarForm_MouseUp(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show($"{((CalendarForm)sender).SelectedSlot.Model}");
        }

        private void CalendarForm_OnPreparing(object sender, OnPreparingCalendarFormEventArgs e)
        {
            Appointment? firstAppointment = AppointmentListController.AsRecordSource().FirstOrDefault(s=>s.DOA == e.Date);
            if (firstAppointment != null) 
                e.Model = (AbstractModel?)treatmentDB?.MasterSource.FirstOrDefault(s => s.Equals(firstAppointment.Treatment));
        }
    }
}
