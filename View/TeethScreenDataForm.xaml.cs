using DentistStudioApp.Controller;
using DentistStudioApp.Model;
using FrontEnd.ExtensionMethods;
using System.Windows;

namespace DentistStudioApp.View
{
    public partial class TeethScreenDataForm : Window
    {
        public TeethScreenDataForm()
        {
            InitializeComponent();
        }

        public TeethScreenDataForm(TeethScreen teethScreen) : this()
        {
            this.SetController(new TeethScreenController(teethScreen));
        }
    }
}
