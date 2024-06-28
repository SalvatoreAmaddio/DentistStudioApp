using DentistStudioApp.Controller;
using DentistStudioApp.Model;
using FrontEnd.ExtensionMethods;
using System.Windows;

namespace DentistStudioApp.View
{
    public partial class SurveyDataFormList : Window
    {
        public SurveyDataFormList()
        {
            InitializeComponent();
            this.SetController(new SurveyDataListController());
        }

        public SurveyDataFormList(Survey survey, IEnumerable<SurveyData> data) : this()
        {
            SurveyDataListController? controller = this.GetController<SurveyDataListController>();
            controller.SurveyController.GoAt(survey);
            controller.RecordSource.ReplaceRecords(data);
            controller.GoFirst();
        }
    }
}
