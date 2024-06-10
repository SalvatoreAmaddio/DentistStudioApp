using Backend.Database;
using DentistStudioApp.Model;
using FrontEnd.Events;
using FrontEnd.Forms.Calendar;
using System.Windows.Controls;
using System.Windows.Input;
using Backend.ExtensionMethods;

namespace DentistStudioApp.View
{
    public partial class CalendarTab : Page
    {
        private IAbstractDatabase? AppointmentDB = DatabaseManager.Find<Appointment>();
        public CalendarTab()
        {
            InitializeComponent();
        }
        
        private void CalendarForm_MouseUp(object sender, MouseButtonEventArgs e)
        {
            DateTime? date = ((CalendarForm?)sender)?.SelectedSlot?.Date;
            AppointmentListFormWin win = new (date);
            win.ShowDialog();
        }

        private void CalendarForm_OnPreparing(object sender, OnPreparingCalendarFormEventArgs e)
        {
            e.Records = AppointmentDB?.MasterSource.Cast<Appointment>().Where(s => s.DOA == e.Date);
        }
    }
}
