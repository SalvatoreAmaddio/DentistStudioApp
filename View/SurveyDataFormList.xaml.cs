using DentistStudioApp.Controller;
using DentistStudioApp.Model;
using System.Windows;

namespace DentistStudioApp.View
{
    public partial class SurveyDataFormList : Window
    {
        public SurveyDataFormList()
        {
            InitializeComponent();
            DataContext = new SurveyDataController();
            ((SurveyDataController)DataContext).Window = this;
        }

        public SurveyDataFormList(Survey survey, IEnumerable<SurveyData> data) : this()
        {
            SurveyDataController controller = (SurveyDataController)DataContext;
            controller.SurveyController.CurrentRecord = survey;
            controller.AsRecordSource().ReplaceRange(data);
        }
    }
}
