using Backend.Database;
using Backend.Source;
using DentistStudioApp.Model;
using FrontEnd.Controller;
using System.Windows.Input;

namespace DentistStudioApp.Controller
{
    public class PatientController : AbstractFormController<Patient>
    {
        public TreatmentControllerList Treatments { get; } = new();
        private Survey? Survey { get; set; }
        public RecordSource Genders { get; private set; } = new(DatabaseManager.Find<Gender>()!);
        public RecordSource Titles { get; private set; } = new(DatabaseManager.Find<JobTitle>()!);
        public override int DatabaseIndex => 0;

        public ICommand AddSurveyCMD { get; }

        public PatientController() : base()
        {
            AddSurveyCMD = new CMDAsync(OpenSurvey);        
        }

        /// <summary>
        /// Creates a new Survey.
        /// </summary>
        /// <returns>A Survey</returns>
        /// <exception cref="Exception"></exception>
        private Task<Survey> CreateNewSurveyAsync()
        {
            IAbstractDatabase? surveyDB = DatabaseManager.Find<Survey>();

            if (surveyDB == null || CurrentRecord == null) throw new Exception();

            Survey survey = new(CurrentRecord);
            surveyDB.Model = survey;
            surveyDB.Crud(CRUD.INSERT);
            survey.IsDirty = false;
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
            List<Task> fetchingCategoryTasks = [];

            foreach (SurveyQuestion question in questions) 
                fetchingCategoryTasks.Add(Task.Run(question.FetchCategory));

            await Task.WhenAll(fetchingCategoryTasks);
            return questions;
        }

        private async Task CompleteSurveyData(IEnumerable<SurveyData> surveyData)
        {
            List<Task> fetchingSurveyQuestionTasks = [];

            foreach (SurveyData sd in surveyData)
                fetchingSurveyQuestionTasks.Add(Task.Run(sd.FetchSurveyQuestionAsync));

            await Task.WhenAll(fetchingSurveyQuestionTasks);
        }

        private Task AddSurveyData(IAbstractDatabase surveyDataDB, IEnumerable<SurveyQuestion> questions)
        {
            if (Survey == null) throw new NullReferenceException();
            foreach (SurveyQuestion question in questions)
            {
                SurveyData surveyData = new(Survey, question);
                surveyDataDB.Model = surveyData;
                surveyDataDB.Crud(CRUD.INSERT);
                surveyData.IsDirty = false;
            }
            return Task.CompletedTask;
        }

        private Task<Survey?> FetchSurvey() 
        {
           return Task.FromResult(DatabaseManager.Find<Survey>()?.MasterSource.Cast<Survey>().FirstOrDefault(s => s.Patient.Equals(CurrentRecord)));            
        }

        private Task<IEnumerable<SurveyData>> FetchSurveyData(IAbstractDatabase surveyDataDB) =>
        Task.FromResult(surveyDataDB.MasterSource.Cast<SurveyData>().ToList().Where(s => s.Survey.Equals(Survey)));
        
        private async Task<IEnumerable<SurveyData>> AddPatientSurvey() 
        {
            Task<IEnumerable<SurveyQuestion>> questionsTask = Task.Run(PrepareQuestionsAsync);
            IAbstractDatabase? surveyDataDB = DatabaseManager.Find<SurveyData>() ?? throw new NullReferenceException();
            Survey = await Task.Run(CreateNewSurveyAsync);
            IEnumerable<SurveyQuestion> questions = await questionsTask;
            await Task.Run(() => AddSurveyData(surveyDataDB, questions));
            return await Task.Run(() => FetchSurveyData(surveyDataDB));
        }

        private async Task<IEnumerable<SurveyData>> FetchPatientSurvey()
        {
            IAbstractDatabase? surveyDataDB = DatabaseManager.Find<SurveyData>();
            if (surveyDataDB == null || Survey == null) throw new NullReferenceException();
            IEnumerable<SurveyData> results = await Task.Run(() => FetchSurveyData(surveyDataDB));
            await Task.Run(()=>CompleteSurveyData(results));
            return results;
        }

        public async Task OpenSurvey() 
        {
            if (CurrentRecord == null) return;
            
            if (CurrentRecord.IsNewRecord() || CurrentRecord.IsDirty) 
                if (!PerformUpdate()) return;

            IEnumerable<SurveyData> results = [];

            IsLoading = true;

            Survey = await Task.Run(FetchSurvey);
            
            if (Survey == null) 
                results = await Task.Run(AddPatientSurvey);
            else
                results = await Task.Run(FetchPatientSurvey);

            IsLoading = false;
            Survey.Patient = CurrentRecord;
            Survey.IsDirty = false;
            View.SurveyDataFormList surveyWindow = new(Survey, results);
            surveyWindow.ShowDialog();
        }
    }
}
