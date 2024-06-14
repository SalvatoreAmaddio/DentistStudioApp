using DentistStudioApp.Controller;
using FrontEnd.ExtensionMethods;
using System.Windows.Controls;

namespace DentistStudioApp.View
{
    public partial class JobTitleList : Page
    {
        public JobTitleList()
        {
            InitializeComponent();
            this.SetController(new JobTitleListController());
        }
    }
}
