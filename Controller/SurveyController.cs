using DentistStudioApp.Model;
using FrontEnd.Controller;
using FrontEnd.Dialogs;
using FrontEnd.Events;
using System.ComponentModel;

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

        public override void OnWindowClosing(object? sender, CancelEventArgs e)
        {
            bool dirty = AsRecordSource().Any(s => s.IsDirty);
            if (dirty) 
                PerformUpdate(); 
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
