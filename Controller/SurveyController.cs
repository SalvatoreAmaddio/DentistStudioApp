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
            IEnumerable<SurveyQuestion>? questions = DatabaseManager.Find<SurveyQuestion>()?.MasterSource.Cast<SurveyQuestion>().ToList().Where(s => s.Question.ToLower().StartsWith(Search.ToLower()));
            List<SurveyData> temp = [];

            foreach(SurveyData surveyData in MasterSource) 
            { 
                if (questions.Any(s=>s.Equals(surveyData.SurveyQuestion))) 
                    temp.Add(surveyData);
            }
            IEnumerable<SurveyData> result = temp;
            return Task.FromResult(result);
        }

        protected override void Open(SurveyData? model)
        {
        }

    }
}
