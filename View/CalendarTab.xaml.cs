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
            var x = ((CalendarForm)sender)?.SelectedSlot?.Models?.Count();
            MessageBox.Show(x?.ToString());
        }

        private void CalendarForm_OnPreparing(object sender, OnPreparingCalendarFormEventArgs e)
        {
            e.Model = AppointmentDB?.MasterSource.Cast<Appointment>().Where(s => s.DOA == e.Date);
        }
    }
}
