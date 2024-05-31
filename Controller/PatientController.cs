using Backend.Database;
using Backend.Model;
using Backend.Source;
using DentistStudioApp.Model;
using FrontEnd.Controller;
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

        private async Task<Survey> Generate(IAbstractDatabase surveyDataDB, IAbstractDatabase surveyQuestionDb)
        {
            IAbstractDatabase? surveyDB = DatabaseManager.Find<Survey>();

            if (surveyDB == null) throw new Exception();
            if (CurrentRecord == null) throw new Exception();

            Survey survey = new(CurrentRecord);
            surveyDB.Model = survey;
            surveyDB.Crud(CRUD.INSERT);

            List<ISQLModel> questions = surveyQuestionDb.MasterSource.ToList();

            await Task.Run(() =>
            {
                foreach (SurveyQuestion question in questions)
                {
                    SurveyData surveyData = new(survey, question);
                    surveyDataDB.Model = surveyData;
                    surveyDataDB.Crud(CRUD.INSERT);
                }
            });

            return survey;
        }

        private async Task<Survey?> TryGetSurvey() 
        {
            return await Task.Run(() =>
            { 
                return (Survey?)DatabaseManager.Find<Survey>()?.MasterSource.FirstOrDefault(s => ((Survey)s).Patient.Equals(CurrentRecord));
            });
            
        }

        public async Task<IEnumerable<SurveyData>> FillCategories(IAbstractDatabase surveyDataDB, IAbstractDatabase surveyQuestionDb, Survey survey) 
        {
           var SurveyQuestionCategory = DatabaseManager.Find<SurveyQuestionCategory>()?.MasterSource;
           return await Task.Run(() =>
            {
                List<SurveyData> results = surveyDataDB.MasterSource.Where(s => ((SurveyData)s).Survey.Equals(survey)).Cast<SurveyData>().ToList();
                foreach (SurveyData surveyData in results)
                {

                    surveyData.SurveyQuestion = (SurveyQuestion?)surveyQuestionDb.MasterSource.FirstOrDefault(s => s.Equals(surveyData.SurveyQuestion));
                    if (surveyData.SurveyQuestion!=null)
                        surveyData.SurveyQuestion.Category = (SurveyQuestionCategory?)(SurveyQuestionCategory?.FirstOrDefault());
                    surveyData.IsDirty = false;
                }
                return results;
            });

        }
        public async Task AddSurvey() 
        {
            if (CurrentRecord == null) return;
            
            if (CurrentRecord.IsNewRecord()) 
            {
                bool result = this.PerformUpdate();
                if (!result) return;
            }

            Survey? survey = await TryGetSurvey();
            IAbstractDatabase? surveyDataDB = DatabaseManager.Find<SurveyData>();
            IAbstractDatabase? surveyQuestionDb = DatabaseManager.Find<SurveyQuestion>();

            if (surveyDataDB == null || surveyQuestionDb == null) return;

            IsLoading = true;

            if (survey == null) 
                survey = await Generate(surveyDataDB, surveyQuestionDb);

            var data = await FillCategories(surveyDataDB, surveyQuestionDb, survey);

            IsLoading = false;

            View.SurveyDataFormList surveyWindow = new(data);
            surveyWindow.ShowDialog();
        }
    }
}
