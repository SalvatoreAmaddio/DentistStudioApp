using DentistStudioApp.Controller;
using System.Windows.Controls;

namespace DentistStudioApp.View
{
    public partial class ClinicList : Page
    {
        public ClinicList()
        {
            InitializeComponent();
            DataContext = new ClinicListController();
            ((ClinicListController)DataContext).UI = this;
        }
    }
}
