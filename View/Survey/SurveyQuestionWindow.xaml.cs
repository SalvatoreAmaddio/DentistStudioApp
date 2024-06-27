using DentistStudioApp.Controller;
using FrontEnd.ExtensionMethods;
using System.Windows;

namespace DentistStudioApp.View
{
    public partial class SurveyQuestionWindow : Window
    {
        public SurveyQuestionWindow()
        {
            InitializeComponent();
            this.SetController(new SurveyQuestionListController());
        }
    }
}
