using Backend.Database;
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
        public ICommand PatientWithTreatmentReportCMD { get; }
        public ICommand ServiceReportCMD { get; }
        public ICommand DentistReportCMD { get; }
        public ICommand ClinicReportCMD { get; }
        public ICommand InvoiceReportCMD { get; }
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
            ServiceReportCMD = new CMDAsync(ServiceReport);
            DentistReportCMD = new CMDAsync(DentistReport);
            PatientWithTreatmentReportCMD = new CMDAsync(PatientWithTreatmentReport);
            ClinicReportCMD = new CMDAsync(ClinicReport);
            InvoiceReportCMD = new CMDAsync(InvoiceReport);
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

        private static async Task<RecordSource<M>> FetchData<M>(string sql) where M : IAbstractModel, new()
        {
            IAbstractDatabase? db = DatabaseManager.Find<M>() ?? throw new NullReferenceException();
            return await RecordSource<M>.CreateFromAsyncList(db.RetrieveAsync(sql).Cast<M>());
        }

        private async Task InvoiceReport()
        {
            MainTab.CurrentTabController()?.SetLoading(true);
            string sql = new InvoiceReport().SelectQry;
            await RunTasks<InvoiceReport>(sql, nameof(Invoice));
        }

        private async Task PatientReport()
        {
            MainTab.CurrentTabController()?.SetLoading(true);
            string sql = new PatientReport().Select().All().From().Statement();
            await RunTasks<PatientReport>(sql, nameof(Patient));
        }

        private async Task PatientWithTreatmentReport()
        {
            MainTab.CurrentTabController()?.SetLoading(true);

            Task<RecordSource<PatientWithTreatment>> patientDataTask = FetchData<PatientWithTreatment>(new PatientWithTreatment().SelectQry);

            Task<Excel> excelTask = Task.Run(() => InstantiateExcel(nameof(PatientWithTreatment)));

            RecordSource<PatientWithTreatment> patientData = await patientDataTask;

            Excel excel = await excelTask;

            await PrintReport(nameof(PatientWithTreatment), excel, patientData.Cast<ISQLModel>().ToList());
        }

        private async Task ServiceReport()
        {
            MainTab.CurrentTabController()?.SetLoading(true);
            string sql = new Service().Select().From().Statement();
            await RunTasks<Service>(sql, nameof(Service));
        }

        private async Task DentistReport()
        {
            MainTab.CurrentTabController()?.SetLoading(true);
            string sql = new Dentist().Select().From().Statement();
            await RunTasks<Dentist>(sql, nameof(Dentist));
        }

        private async Task ClinicReport()
        {
            MainTab.CurrentTabController()?.SetLoading(true);
            string sql = new Clinic().Select().From().Statement();
            await RunTasks<Clinic>(sql, nameof(Clinic));
        }

        private async Task RunTasks<M>(string sql, string sheetName) where M : IAbstractModel, new()
        {
            Task<RecordSource<M>> dataTask = FetchData<M>(sql);
            Task<Excel> excelTask = InstantiateExcel(sheetName);

            RecordSource<M> data = await dataTask;
            Excel excel = await excelTask;
            await PrintReport(sheetName, excel, data.Cast<ISQLModel>().ToList());
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
            string[] headers = null!;
            switch (sheetName)
            {
                case nameof(Patient):
                    headers = ["Patient ID", "First Name", "Last Name", "DOB", "Gender", "Job Title", "Phone Number", "Email"];
                    break;
                case nameof(PatientWithTreatment):
                    headers = ["Patient ID", "First Name", "Last Name", "Start Date", "End Date", "Date of Appointment", "Time of Appointment", "Service", "Dentist", "Clinic", "Room Number", "Attended"];
                    break;
                case nameof(Service):
                    headers = ["Service ID", "Service Name", "Cost"];
                    break;
                case nameof(Dentist):
                    headers = ["Dentist ID", "First Name", "Last Name", "Clinic"];
                    break;
                case nameof(Clinic):
                    headers = ["Clinic ID", "Clinic Name"];
                    break;
                case nameof(Invoice):
                    headers = ["Invoice ID", "Patient", "Date", "Amount", "Discount", "Deposit", "Total Due", "Payment Type", "Paid"];
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