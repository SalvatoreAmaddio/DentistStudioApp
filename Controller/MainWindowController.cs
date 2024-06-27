﻿using Backend.Database;
using Backend.Exceptions;
using Backend.ExtensionMethods;
using Backend.Model;
using Backend.Office;
using Backend.Utils;
using DentistStudioApp.Model;
using DentistStudioApp.View;
using FrontEnd.Controller;
using FrontEnd.Dialogs;
using FrontEnd.ExtensionMethods;
using FrontEnd.Forms;
using FrontEnd.Model;
using FrontEnd.Source;
using FrontEnd.Utils;
using System.Windows.Controls;
using System.Windows.Input;

namespace DentistStudioApp.Controller
{
    public class MainWindowController
    {
        private readonly MainWindow _mainWin;
        private Curtain Curtain => _mainWin.Curtain;
        private TabControl MainTab => _mainWin.MainTab;

        #region Commands
        public ICommand OpenCurtainCMD { get; }
        public ICommand OpenSurveyQuestionsCMD { get; }
        public ICommand OpenSurveyQuestionCategoryCMD { get; }
        public ICommand PatientReportCMD { get; }
        #endregion

        public MainWindowController(MainWindow mainWin)
        {
            _mainWin = mainWin;
            _mainWin.DataContext = this;

            Helper.ManageTabClosing(MainTab);
            Curtain.SoftwareInfo = new SoftwareInfo("Salvatore Amaddio", "www.salvatoreamaddio.co.uk", "Mister J", "2024");

            OpenCurtainCMD = new CMD(OpenCurtain);
            OpenSurveyQuestionsCMD = new CMD(OpenSurveyQuestions);
            OpenSurveyQuestionCategoryCMD = new CMD(OpenSurveyQuestionCategory);
            PatientReportCMD = new CMDAsync(PatientReport);
        }
     
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
        private void OpenCurtain() => Curtain.Open();

        private static async Task<RecordSource<M>> FetchData<M>(string sql) where M : AbstractModel, new()
        {
            IAbstractDatabase? db = DatabaseManager.Find<M>() ?? throw new NullReferenceException();
            return await RecordSource<M>.CreateFromAsyncList(db.RetrieveAsync(sql).Cast<M>());
        }

        private async Task PatientReport()
        {
            MainTab.CurrentTabController()?.SetLoading(true);

            string sql = new Patient().Select().AllExcept("PicturePath").From().Statement();
            Task<RecordSource<Patient>> dataTask = FetchData<Patient>(sql);
            Task<Excel> excelTask = InstantiateExcel("Patient");

            RecordSource<Patient> data = await dataTask;
            Excel excel = await excelTask;
            await PrintReport("Patient", excel, data.Cast<ISQLModel>().ToList());
        }

        public async Task PrintReport(string sheetName, Excel excel, List<ISQLModel> data)
        {
            try
            {
                await Task.Run(() => WriteExcel(data, excel, sheetName));
            }
            catch (WorkbookException ex)
            {
                Failure.Allert(ex.Message, "Carefull!");
                return;
            }
            finally
            {
                MainTab.CurrentTabController()?.SetLoading(false);
            }

            SuccessDialog.Display("Report Successfully Exported!");
        }

        private Task<Excel> InstantiateExcel(string sheetName)
        {
            Excel _excel = new();
            _excel.Create();
            _excel.Worksheet?.SetName(sheetName);
            SetHeaders(sheetName, ref _excel);
            return Task.FromResult(_excel);
        }

        private void SetHeaders(string sheetName, ref Excel excel)
        {
            string[] headers = [];
            switch (sheetName)
            {
                case nameof(Patient):
                    headers = ["Patient ID", "First Name", "Last Name", "DOB", "Gender", "Job Title", "Phone Number", "Email"];
                    break;
            }

            excel.Worksheet?.PrintHeader(headers);
        }


        private static Task SaveAndClose(ref Excel excel, string sheetName)
        {
            try
            {
                excel.Save($"{Sys.Desktop}\\{sheetName}.xlsx");
            }
            catch (WorkbookException ex)
            {
                return Task.FromException(ex);
            }
            finally
            {
                excel.Close();
            }
            return Task.CompletedTask;
        }

        private static async Task WriteExcel(List<ISQLModel> source, Excel excel, string sheetName)
        {
            excel.Worksheet?.PrintData(source);
            await SaveAndClose(ref excel, sheetName);
        }
    }
}