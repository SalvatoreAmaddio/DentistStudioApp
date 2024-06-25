using DentistStudioApp.Controller;
using DentistStudioApp.Model;
using FrontEnd.ExtensionMethods;
using System.Windows;

namespace DentistStudioApp.View
{
    public partial class ScreenForm : Window
    {
        public ScreenForm()
        {
            InitializeComponent();
        }

        public ScreenForm(TeethScreen teethScreen) : this()
        {
            this.SetController(new TeethScreenController(teethScreen));
        }
    }
}
