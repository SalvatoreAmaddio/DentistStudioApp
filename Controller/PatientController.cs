using Backend.Database;
using Backend.Model;
using Backend.Source;
using DentistStudioApp.Model;
using FrontEnd.Controller;
using FrontEnd.Dialogs;
using System.Windows.Input;

namespace DentistStudioApp.Controller
{
    public class PatientController : AbstractFormController<Patient>
    {
        public RecordSource Genders { get; private set; } = new(DatabaseManager.Find<Gender>()!);
        public RecordSource Titles { get; private set; } = new(DatabaseManager.Find<JobTitle>()!);
        public override int DatabaseIndex => 0;

        public ICommand AddSurveyCMD { get; }

        public PatientController() : base()
        {
            AddSurveyCMD = new CMDAsync(AddSurvey);        
        }

        public async Task<bool> AddSurvey() 
        {
            if (CurrentRecord == null) return false;

            if (CurrentRecord.IsNewRecord()) 
            {
                bool result = this.PerformUpdate();
                if (!result) return false;
            }
            
            IAbstractDatabase? surveyDB = DatabaseManager.Find<Survey>();
            bool taskResult = false;
            bool exist = surveyDB.MasterSource.Any(s=>((Survey)s).Patient.Equals(CurrentRecord));
            if (exist) goto OpenWin;
            Survey survey = new(CurrentRecord);
            surveyDB.Model = survey;
            surveyDB.Crud(CRUD.INSERT);
            var x = survey.SurveyID;
            List<ISQLModel>? questions = DatabaseManager.Find<SurveyQuestion>()?.MasterSource.ToList();
            IAbstractDatabase? surveyDataDB = DatabaseManager.Find<SurveyData>();

            taskResult = await Task.Run(() =>
            {
                foreach (SurveyQuestion question in questions)
                {
                    question.Category = (SurveyQuestionCategory?)(DatabaseManager.Find<SurveyQuestionCategory>()?.MasterSource.First());
                    SurveyData surveyData = new(survey, question);
                    surveyDataDB.Model = surveyData;
                    surveyDataDB.Crud(CRUD.INSERT);
                    var y = surveyData.SurveyDataID;
                }
                return true;
            });

            OpenWin:
            View.Survey surveyWindow = new();
            surveyWindow.ShowDialog();
            return taskResult;
        }
    }
}
