using DentistStudioApp.Model;
using FrontEnd.Controller;
using FrontEnd.Utils;

namespace DentistStudioApp.Controller
{
    public class SurveyController : AbstractFormController<Survey>
    {
        public override int DatabaseIndex => 3;

        protected override bool Delete(Survey? model)
        {
            bool result = base.Delete(model);
            if (result)
                Helper.GetActiveWindow()?.Close();
            return result;
        }
    }
}
