using DentistStudioApp.Controller;
using DentistStudioApp.Model;
using System.Windows;

namespace DentistStudioApp.View
{
    public partial class TreatmentForm : Window
    {
        public TreatmentForm()
        {
            InitializeComponent();
            DataContext = new TreatmentController();
            ((TreatmentController)DataContext).UI = this;
        }

        public TreatmentForm(Treatment? treatment) : this()
        {
            TreatmentController controller = (TreatmentController)DataContext;

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
