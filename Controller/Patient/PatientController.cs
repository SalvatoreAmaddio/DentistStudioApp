using Backend.Database;
using Backend.ExtensionMethods;
using Backend.Source;
using DentistStudioApp.Model;
using DentistStudioApp.View;
using FrontEnd.Controller;
using FrontEnd.Dialogs;
using System.Windows.Input;

namespace DentistStudioApp.Controller
{
    public class PatientController : AbstractFormController<Patient>
    {
        private IAbstractDatabase? surveyDB = DatabaseManager.Find<Survey>();
       
        public TreatmentListController Treatments { get; } = new();
        private Survey? Survey { get; set; }
        public RecordSource Genders { get; private set; } = new(DatabaseManager.Find<Gender>()!);
        public RecordSource Titles { get; private set; } = new(DatabaseManager.Find<JobTitle>()!);
        public override int DatabaseIndex => 0;

        public ICommand AddSurveyCMD { get; }

        public ICommand AddInvoiceCMD { get; }

        public PatientController() : base()
        {
            AddSurveyCMD = new CMDAsync(OpenSurvey);    
            AddInvoiceCMD = new CMDAsync(AddInvoice);
        }

        private async Task AddInvoice() 
        {
            if (CurrentRecord == null) 
            {
                Failure.Allert("Record is null");
                return;
            }

            long? count = await Task.Run(CurrentRecord.TreatmentCount);

            if (!count.HasValue || count.Value == 0)
            {
                Failure.Allert("There are no treatments to invoice.");
                return;
            }

            if (CurrentRecord.IsNewRecord())
            {
                Failure.Allert("This record does not exist yet");
                return;
            }

            if (CurrentRecord.IsDirty) 
                if (!PerformUpdate()) return;

            InvoiceForm win = new(CurrentRecord);
            win.ShowDialog();
        }

        /// <summary>
        /// Creates a new Survey.
        /// </summary>
        /// <returns>A Survey</returns>
        /// <exception cref="Exception"></exception>
        private Task<Survey> CreateNewSurveyAsync()
        {
            if (surveyDB == null || CurrentRecord == null) throw new NullReferenceException();

            Survey survey = new(CurrentRecord);
            surveyDB.Model = survey;
            surveyDB.Crud(CRUD.INSERT);
            survey.IsDirty = false;
            return Task.FromResult(survey);
        }

        private Task<Survey?> FetchSurvey() 
        {
           if (surveyDB == null) throw new NullReferenceException();
           string sql = new Survey().Where().EqualsTo("PatientID","@id").LIMIT().Statement();
           List<QueryParameter> param = [];
           param.Add(new("id",CurrentRecord?.PatientID));
           return Task.FromResult(surveyDB.Retrieve(sql, param).Cast<Survey>().FirstOrDefault());
        }

        private Task<IEnumerable<SurveyData>> FetchPatientSurveyData()
        {
            IAbstractDatabase? surveyDataDB = DatabaseManager.Find<SurveyData>();
            if (surveyDataDB == null || Survey == null) throw new NullReferenceException();
            return Task.FromResult(surveyDataDB.Retrieve($"SELECT * FROM SurveyData WHERE SurveyID = {Survey.SurveyID}").Cast<SurveyData>());
        }

        private async Task<IEnumerable<SurveyData>> AddPatientSurveyData() 
        {
            IAbstractDatabase? surveyDataDB = DatabaseManager.Find<SurveyData>();
            Survey = await CreateNewSurveyAsync();
            if (surveyDataDB == null) throw new NullReferenceException();
            await surveyDataDB.ExecuteQueryAsync($"INSERT INTO SurveyData (SurveyID, SurveyQuestionID) SELECT '{Survey.SurveyID}' As SurveyID, SurveyQuestionID FROM SurveyQuestion;");
            return surveyDataDB.Retrieve($"SELECT * FROM SurveyData WHERE SurveyID = {Survey.SurveyID}").Cast<SurveyData>();
        }

        public async Task OpenSurvey() 
        {
            if (CurrentRecord == null) return;
            
            if (CurrentRecord.IsNewRecord() || CurrentRecord.IsDirty) 
                if (!PerformUpdate()) return;

            IsLoading = true;

            Survey = await Task.Run(FetchSurvey);

            IEnumerable<SurveyData> records = (Survey == null) ? await Task.Run(AddPatientSurveyData) : await Task.Run(FetchPatientSurveyData);

            IsLoading = false;

            Survey!.Patient = CurrentRecord;
            Survey.IsDirty = false;

            SurveyDataFormList surveyWindow = new(Survey, records);
            surveyWindow.ShowDialog();
        }
    }
}