using DentistStudioApp.Controller;
using FrontEnd.ExtensionMethods;
using System.Windows;

namespace DentistStudioApp.View
{
    public partial class TreatmentForm : Window
    {

        public TreatmentForm() => InitializeComponent();

        public TreatmentForm(TreatmentController controller) : this() => this.SetController(controller);

    }
}