using Backend.Database;
using Backend.Model;
using Backend.Source;
using DentistStudioApp.Model;
using FrontEnd.Controller;
using System.Windows.Documents;
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

        /// <summary>
        /// Creates a new Survey.
        /// </summary>
        /// <returns>A Survey</returns>
        /// <exception cref="Exception"></exception>
        private Task<Survey> CreateNewSurveyAsync()
        {
            IAbstractDatabase? surveyDB = DatabaseManager.Find<Survey>();

            if (surveyDB == null) throw new Exception();
            if (CurrentRecord == null) throw new Exception();

            Survey survey = new(CurrentRecord);
            surveyDB.Model = survey;
            surveyDB.Crud(CRUD.INSERT);
            return Task.FromResult(survey);
        }

        /// <summary>
        /// Returns a <see cref="IEnumerable{SurveyQuestion}"/> of <see cref="SurveyQuestion"/>.
        /// </summary>
        /// <returns>a <see cref="Task{List{SurveyQuestion}}"/></returns>
        private async Task<IEnumerable<SurveyQuestion>> PrepareQuestionsAsync()
        {
            IAbstractDatabase? surveyQuestionDb = DatabaseManager.Find<SurveyQuestion>();

            IEnumerable<SurveyQuestion>? questions = surveyQuestionDb?.MasterSource.Cast<SurveyQuestion>().ToList();

            foreach (SurveyQuestion question in questions)
                await Task.Run(question.FetchCategory);

            return questions;
        }

        private Task AddSurveyData(Survey survey, IAbstractDatabase surveyDataDB, IEnumerable<SurveyQuestion> questions)
        {
            foreach (SurveyQuestion question in questions)
            {
                SurveyData surveyData = new(survey, question);
                surveyDataDB.Model = surveyData;
                surveyDataDB.Crud(CRUD.INSERT);
            }
            return Task.CompletedTask;
        }

        private Task<Survey?> FetchSurvey() 
        {
           return Task.FromResult(DatabaseManager.Find<Survey>()?.MasterSource.Cast<Survey>().FirstOrDefault(s => s.Patient.Equals(CurrentRecord)));            
        }

        public Task<IEnumerable<SurveyData>> FetchSurveyData(IAbstractDatabase surveyDataDB, Survey survey) =>
        Task.FromResult(surveyDataDB.MasterSource.Cast<SurveyData>().ToList().Where(s => s.Survey.Equals(survey)));

        public async Task AddSurvey() 
        {
            if (CurrentRecord == null) return;
            
            if (CurrentRecord.IsNewRecord()) 
            {
                bool result = this.PerformUpdate();
                if (!result) return;
            }

            Task<IEnumerable<SurveyQuestion>> questionsTask = Task.Run(PrepareQuestionsAsync);

            Survey? survey = await Task.Run(FetchSurvey);

            IsLoading = true;

            IAbstractDatabase? surveyDataDB = DatabaseManager.Find<SurveyData>();

            if (surveyDataDB == null) return;

            if (survey == null) 
            {
                survey = await Task.Run(CreateNewSurveyAsync);                
            }

            IEnumerable<SurveyQuestion> questions = await questionsTask;

            await Task.Run(() => AddSurveyData(survey, surveyDataDB, questions));

            IEnumerable<SurveyData> results = await Task.Run(()=>FetchSurveyData(surveyDataDB, survey));

            IsLoading = false;

            View.SurveyDataFormList surveyWindow = new(results);
            surveyWindow.ShowDialog();
        }
    }
}
