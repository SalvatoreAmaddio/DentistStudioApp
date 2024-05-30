using DentistStudioApp.Controller;
using System.Windows;

namespace DentistStudioApp.View
{
    /// <summary>
    /// Interaction logic for Survey.xaml
    /// </summary>
    public partial class Survey : Window
    {
        public Survey()
        {
            InitializeComponent();
            DataContext = new SurveyDataController();
            ((SurveyDataController)DataContext).Window = this;
        }
    }
}
