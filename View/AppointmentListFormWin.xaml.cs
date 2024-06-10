using DentistStudioApp.Controller;
using FrontEnd.ExtensionMethods;
using System.Windows;

namespace DentistStudioApp.View
{
    public partial class AppointmentListFormWin : Window
    {
        public AppointmentListFormWin()
        {
            InitializeComponent();
            this.SetController(new AppointmentListController());
        }



    }
}
