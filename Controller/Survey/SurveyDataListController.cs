using Backend.Database;
using Backend.ExtensionMethods;
using Backend.Model;
using Backend.Source;
using DentistStudioApp.Model;
using FrontEnd.Controller;
using FrontEnd.Events;
using FrontEnd.FilterSource;

namespace DentistStudioApp.Controller
{
    public class SurveyDataListController : AbstractFormListController<SurveyData>
    {
        public SourceOption CategoryOptions { get; private set; }

        public RecordSource SurveyQuestionCategories { get; private set; } = new(DatabaseManager.Find<SurveyQuestionCategory>()!);
        public SurveyController SurveyController { get; } = new();

        public override int DatabaseIndex => 4;

        public SurveyDataListController()
        {
            CategoryOptions = new(SurveyQuestionCategories, "CategoryName");
            AllowNewRecord = false;
            AllowAutoSave = true;
            AfterUpdate += OnAfterUpdate;
        }

        private void OnAfterUpdate(object? sender, AfterUpdateArgs e)
        {
            if (!e.Is(nameof(Search))) return;
            OnSearchPropertyRequery(sender);
        }

        public override void OnOptionFilterClicked(FilterEventArgs e)
        {
            OnAfterUpdate(e, new(null, null, nameof(Search)));
        }

        private bool Filter(SurveyData data, IEnumerable<SurveyQuestion> questions)
        {
            Survey? s = data.Survey;
            SurveyQuestion? q = data.SurveyQuestion;
            bool condition1 = (s == null) ? false : s.Equals(SurveyController.CurrentRecord);
            bool condition2 = questions.Any(s => s.Equals(q));
            bool condition3 = true;

            if (CategoryOptions.Selected().Any())
                condition3 = CategoryOptions.Selected().Any(s => s.Equals(data.SurveyQuestion.Category));

            return condition1 && condition2 && condition3;
        }

        public override Task<IEnumerable<SurveyData>> SearchRecordAsync()
        {
            //new SurveyData()
            //    .InnerJoin(nameof(SurveyQuestion), nameof(SurveyData), "SurveyQuestionID")
            //    .InnerJoin(nameof(Survey), nameof(SurveyData), "SurveyID")
            //    .Where()
            //    .EqualsTo("Survey.PatientID", "id");

            IAbstractDatabase? surveryQuestionsDb = DatabaseManager.Find<SurveyQuestion>();
            List<SurveyQuestion>? surveyQuestions = surveryQuestionsDb?.MasterSource.Cast<SurveyQuestion>().ToList();
            IEnumerable<SurveyQuestion>? questions = surveyQuestions?.Where(s => s.Question.ToLower().StartsWith(Search.ToLower()));

            if (questions == null) throw new NullReferenceException();
            if (MasterSource == null) throw new NullReferenceException();

            IEnumerable<SurveyData> result = MasterSource.Where(s => Filter(s, questions));

            return Task.FromResult(result);
        }

        protected override void Open(SurveyData? model)
        {
        }

        public override SelectBuilder InstantiateSearchQry()
        {
            return null;
        }
    }

}
