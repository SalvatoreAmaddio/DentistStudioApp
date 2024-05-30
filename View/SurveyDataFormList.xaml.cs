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

        public SurveyDataFormList(IEnumerable<SurveyData> data) : this()
        {
            ((SurveyDataController)DataContext).AsRecordSource().ReplaceRange(data);
        }
    }
}
