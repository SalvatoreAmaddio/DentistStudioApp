using Backend.Exceptions;
using Backend.Model;
using Backend.Office;
using Backend.Utils;
using DentistStudioApp.Model;
using DentistStudioApp.View;
using FrontEnd.Controller;
using FrontEnd.Dialogs;
using FrontEnd.ExtensionMethods;
using System.Windows;
using System.Windows.Input;

namespace DentistStudioApp.Controller
{
    public class MainWindowController
    {
        private readonly MainWindow _mainWin;
        private Excel _excel = null!;

        #region Commands
        public ICommand OpenCurtainCMD { get; }
        public ICommand OpenSurveyQuestionsCMD { get; }
        public ICommand OpenSurveyQuestionCategoryCMD { get; }
        #endregion

        public MainWindowController(MainWindow mainWin)
        {
            _mainWin = mainWin;
            _mainWin.DataContext = this;
            OpenCurtainCMD = new CMD(OpenCurtain);
            OpenSurveyQuestionsCMD = new CMD(OpenSurveyQuestions);
            OpenSurveyQuestionCategoryCMD = new CMD(OpenSurveyQuestionCategory);
        }
        //            _data = mainTab?.CurrentTabController()?.Source.Cast<ISQLModel>()?.ToList();

        private void OpenSurveyQuestionCategory()
        {
            SurveyQuestionCategoryWindow questionWindow = new();
            questionWindow.ShowDialog();
        }
        private void OpenSurveyQuestions()
        {
            SurveyQuestionWindow questionWindow = new();
            questionWindow.ShowDialog();
        }
        private void OpenCurtain() => _mainWin.Curtain.Open();

        public async void x(string sheetName, List<ISQLModel>? data)
        {
            _mainWin.MainTab.CurrentTabController()?.SetLoading(true);

            try
            {
                await Task.Run(() => WriteExcel(data, sheetName));
            }
            catch (WorkbookException ex)
            {
                Failure.Allert(ex.Message, "Carefull!");
                return;
            }
            finally
            {
                _mainWin.MainTab.CurrentTabController()?.SetLoading(false);
            }

            SuccessDialog.Display("Report Successfully Exported!");
        }

        private void IstantiateExcel(string sheetName)
        {
            _excel = new();
            _excel.Create();
            _excel.Worksheet?.SetName(sheetName);
        }

        private void SetHeaders(string sheetName)
        {
            string[] headers = [];
            switch (sheetName)
            {
                case nameof(Patient):
                    headers = ["Employee ID", "First Name", "Last Name", "DOB", "Gender", "Department", "Job Title", "Email"];
                    break;
            }

            _excel.Worksheet?.PrintHeader(headers);
        }

        private void WriteData(List<ISQLModel> source) 
        {
            _excel.Worksheet?.PrintData(source);
        }

        private Task SaveAndClose(string sheetName)
        {
            try
            {
                _excel.Save($"{Sys.Desktop}\\{sheetName}.xlsx");
            }
            catch (WorkbookException ex)
            {
                return Task.FromException(ex);
            }
            finally
            {
                _excel.Close();
            }
            return Task.CompletedTask;
        }

        private async Task WriteExcel(List<ISQLModel>? source, string? sheetName)
        {
            if (source == null || string.IsNullOrEmpty(sheetName)) throw new NullReferenceException("Something went wrong here!");

            IstantiateExcel(sheetName);

            SetHeaders(sheetName);

            WriteData(source);

            await SaveAndClose(sheetName);
        }
    }
}