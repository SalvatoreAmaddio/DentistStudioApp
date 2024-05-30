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
        }

        public void Filter(Survey survey) 
        {
            var results = AsRecordSource().Where(s=>s.Survey.Equals(survey)).ToList();
            AsRecordSource().ReplaceRange(results);
        }

        public override void OnOptionFilter(FilterEventArgs e)
        {
        }

        public override Task<IEnumerable<SurveyData>> SearchRecordAsync()
        {
            throw new NotImplementedException();
        }

        protected override void Open(SurveyData? model)
        {
        }
    }
}
