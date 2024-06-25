using DentistStudioApp.Model;
using FrontEnd.Controller;

namespace DentistStudioApp.Controller
{
    public class TeethScreenController : AbstractFormController<TeethScreen>
    {
        public ScreeningController ScreeningController { get; } = new();
        public override int DatabaseIndex => 15;

        public TeethScreenController(TeethScreen teethScreen)
        {
            CurrentRecord = teethScreen;
            AddSubControllers(ScreeningController);
        }
    }
}
