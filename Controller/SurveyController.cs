using Backend.Database;
using DentistStudioApp.Model;
using FrontEnd.Controller;
using FrontEnd.Events;

namespace DentistStudioApp.Controller
{
    public class SurveyController : AbstractFormController<Survey>
    {
        public override int DatabaseIndex => 3;
    }

    public class SurveyDataController : AbstractFormListController<SurveyData>
    {
        public override string SearchQry { get; set; } = string.Empty;

        public override int DatabaseIndex => 4;
        
        public SurveyDataController() 
        { 
            AllowNewRecord = false;
            AllowAutoSave = true;
            AfterUpdate += OnAfterUpdate;
        }

        private async void OnAfterUpdate(object? sender, AfterUpdateArgs e)
        {
            if (!e.Is(nameof(Search))) return;
            var result = await Task.Run(SearchRecordAsync);
            AsRecordSource().ReplaceRange(result);
            GoFirst();
        }

        public override void OnOptionFilter(FilterEventArgs e)
        {
        }

        public override Task<IEnumerable<SurveyData>> SearchRecordAsync()
        {
            IAbstractDatabase? surveryQuestionsDb = DatabaseManager.Find<SurveyQuestion>();
            List<SurveyQuestion>? surveyQuestions = surveryQuestionsDb?.MasterSource.Cast<SurveyQuestion>().ToList();
            IEnumerable<SurveyQuestion>? questions = surveyQuestions?.Where(s => s.Question.ToLower().StartsWith(Search.ToLower()));
            List<SurveyData> temp = [];

            foreach (SurveyData surveyData in MasterSource!)
            { 
                SurveyQuestion? question = surveyData.SurveyQuestion;

                if (questions!.Any(s=>s.Equals(question))) 
                {
                    temp.Add(surveyData);
                }
            }
            IEnumerable<SurveyData> result = temp;
            return Task.FromResult(result);
        }

        protected override void Open(SurveyData? model)
        {
        }

    }
}
