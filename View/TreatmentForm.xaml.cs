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
            ((TreatmentController)DataContext).Window = this;
        }

        public TreatmentForm(Treatment? treatment) : this()
        {
            if (treatment.IsNewRecord())
                ((TreatmentController)DataContext).CurrentRecord = treatment;
            else 
                ((TreatmentController)DataContext).GoAt(treatment);
        }

    }
}
