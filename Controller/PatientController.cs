using Backend.Database;
using Backend.Source;
using DentistStudioApp.Model;
using FrontEnd.Controller;
using FrontEnd.Dialogs;

namespace DentistStudioApp.Controller
{
    public class PatientController : AbstractFormController<Patient>
    {
        public RecordSource Genders { get; private set; } = new(DatabaseManager.Find<Gender>()!);
        public RecordSource Titles { get; private set; } = new(DatabaseManager.Find<JobTitle>()!);
        public override int DatabaseIndex => 0;

        public void AddSurvey() 
        { 
            if (CurrentRecord == null) return;

            if (CurrentRecord.IsNewRecord()) 
            {
                bool result = this.PerformUpdate();
                if (!result) return;
            }

            Survey survey = new(CurrentRecord);
            IAbstractDatabase? surveyDB = DatabaseManager.Find<Survey>();
            surveyDB.Model = survey;
            surveyDB.Crud(CRUD.INSERT);

            MasterSource? questions = DatabaseManager.Find<SurveyQuestion>()?.MasterSource;

            foreach(SurveyQuestion question in questions) 
            {
                question.Category = (SurveyQuestionCategory?)(DatabaseManager.Find<SurveyQuestionCategory>()?.MasterSource.First());
                SurveyData surveyData = new(survey,question);
                IAbstractDatabase? surveyDataDB = DatabaseManager.Find<SurveyData>();
                surveyDataDB.Model = survey;
                surveyDataDB.Crud(CRUD.INSERT);
            }
        }
    }
}
