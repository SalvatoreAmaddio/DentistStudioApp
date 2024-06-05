using DentistStudioApp.Controller;
using FrontEnd.ExtensionMethods;
using System.Windows.Controls;

namespace DentistStudioApp.View
{
    public partial class DentistList : Page
    {
        public DentistList()
        {
            InitializeComponent();
            this.SetController(new DentistListController());
        }
    }
}
