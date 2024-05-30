using Backend.Database;
using Backend.Source;
using DentistStudioApp.Model;
using FrontEnd.Controller;
using FrontEnd.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentistStudioApp.Controller
{
    public class PatientControllerList : AbstractFormListController<Patient>
    {
        public override string SearchQry { get; set; } = string.Empty;

        public RecordSource Genders { get; private set; } = new(DatabaseManager.Find<Gender>()!); //Fetch the data to be displayed in a ComboBox control.
        public RecordSource Titles { get; private set; } = new(DatabaseManager.Find<JobTitle>()!); //Fetch the data to be displayed in a ComboBox control.
        public override int DatabaseIndex => 0;

        public override void OnOptionFilter(FilterEventArgs e)
        {
        }

        public override Task<IEnumerable<Patient>> SearchRecordAsync()
        {
            throw new NotImplementedException();
        }

        protected override void Open(Patient? model)
        {
        }
    }
}
