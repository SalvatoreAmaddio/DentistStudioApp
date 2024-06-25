using DentistStudioApp.Controller;
using DentistStudioApp.Model;
using FrontEnd.ExtensionMethods;
using System.Windows;

namespace DentistStudioApp.View
{
    public partial class TeethScreenList : Window
    {
        public TeethScreenList() => InitializeComponent();

        public TeethScreenList(Patient patient) : this() => this.SetController(new TeethScreenListController(patient));
    }
}
