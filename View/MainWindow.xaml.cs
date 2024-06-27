using Backend.Utils;
using FrontEnd.Utils;
using System.Windows;

namespace DentistStudioApp.View
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Helper.ManageTabClosing(MainTab);
            Curtain.SoftwareInfo = new SoftwareInfo("Salvatore Amaddio", "www.salvatoreamaddio.co.uk", "Mister J", "2024");
        }

        private void OpenCurtain_Click(object sender, RoutedEventArgs e)
        {
            Curtain.Open();
        }

        private void OpenSurveyQuestions(object sender, RoutedEventArgs e)
        {
            SurveyQuestionWindow questionWindow = new();
            questionWindow.ShowDialog();
        }

        private void OpenSurveyQuestionCategory(object sender, RoutedEventArgs e)
        {
            SurveyQuestionCategoryWindow questionWindow = new();
            questionWindow.ShowDialog();
        }
    }
}
