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
            DataContext = new SurveyDataListController();
            ((SurveyDataListController)DataContext).UI = this;
        }

        public SurveyDataFormList(Survey survey, IEnumerable<SurveyData> data) : this()
        {
            SurveyDataListController controller = (SurveyDataListController)DataContext;
            controller.SurveyController.GoAt(survey);
            controller.AsRecordSource().ReplaceRange(data);
        }
    }
}
