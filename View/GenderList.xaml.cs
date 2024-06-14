using DentistStudioApp.Controller;
using FrontEnd.ExtensionMethods;
using System.Windows.Controls;

namespace DentistStudioApp.View
{
    public partial class GenderList : Page
    {
        public GenderList()
        {
            InitializeComponent();
            this.SetController(new GenderListController());
        }
    }
}
