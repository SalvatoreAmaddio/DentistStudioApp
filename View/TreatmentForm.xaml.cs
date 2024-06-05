using DentistStudioApp.Controller;
using DentistStudioApp.Model;
using FrontEnd.ExtensionMethods;
using System.Windows;

namespace DentistStudioApp.View
{
    public partial class TreatmentForm : Window
    {
        public TreatmentForm()
        {
            InitializeComponent();
            this.SetController(new TreatmentController());
        }

        public TreatmentForm(Treatment? treatment) : this()
        {
            TreatmentController controller = this.GetController<TreatmentController>();

            if (treatment.IsNewRecord()) 
            {
                controller.GoAt(treatment);
                controller.CurrentRecord.Patient = treatment.Patient;
                controller.CurrentRecord.IsDirty = false;
                return;
            }
            controller.GoAt(treatment);
        }

    }
}
