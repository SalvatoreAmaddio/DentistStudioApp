using DentistStudioApp.Controller;
using System.Windows.Controls;

namespace DentistStudioApp.View
{
    public partial class DentistList : Page
    {
        public DentistList()
        {
            InitializeComponent();
            DataContext = new DentistListController();
            ((DentistListController)DataContext).UI = this;
        }
    }
}
