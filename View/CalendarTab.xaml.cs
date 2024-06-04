using DentistStudioApp.Model;
using FrontEnd.Events;
using FrontEnd.Forms.Calendar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DentistStudioApp.View
{
    /// <summary>
    /// Interaction logic for CalendarTab.xaml
    /// </summary>
    public partial class CalendarTab : Window
    {
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
            e.Model = new Patient() { FirstName="Salvo" };
        }
    }
}
