using DentistStudioApp.Controller;
using FrontEnd.ExtensionMethods;
using System.Windows;

namespace DentistStudioApp.View
{
    public partial class SurveyQuestionCategoryWindow : Window
    {
        public SurveyQuestionCategoryWindow()
        {
            InitializeComponent();
            this.SetController(new SurveyQuestionCategoryListController());
        }
    }
}
