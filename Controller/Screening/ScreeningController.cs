using Backend.Model;
using Backend.Utils;
using DentistStudioApp.Model;
using FrontEnd.Controller;
using FrontEnd.Forms;
using FrontEnd.Source;
using System.IO;
using System.Windows.Input;
using Backend.ExtensionMethods;

namespace DentistStudioApp.Controller
{
    public class ScreeningController : AbstractFormController<Screening>
    {
        public override int DatabaseIndex => 16;
        public ICommand FilePickedCMD { get; }
        
        public ScreeningController() 
        {
            FilePickedCMD = new Command<FilePickerCatch>(PickPicture);
            AfterRecordNavigation += OnAfterRecordNavigation;
        }

        private void OnAfterRecordNavigation(object? sender, Backend.Events.AllowRecordMovementArgs e)
        {
            if (e.NewRecord)
            {
                TeethScreen? teethScreen = (TeethScreen?)ParentRecord;
                if (CurrentRecord != null)
                {
                    CurrentRecord.TeethScreen = teethScreen;
                    CurrentRecord.Clean();
                }
            }
        }

        private void PickPicture(FilePickerCatch? filePicked)
        {
            if (CurrentRecord == null || filePicked == null) return;
            if (CurrentRecord.IsDirty)
                if (!PerformUpdate()) return;

            if (filePicked.FileRemoved)
            {
                string temp = CurrentRecord.ScreenPath;
                CurrentRecord.ScreenPath = "pack://application:,,,/Images/placeholder.jpg";
                Sys.AttemptFileDelete(temp);
                return;
            }

            if (string.IsNullOrEmpty(filePicked.FilePath)) return;

            FileTransfer fileTransfer = new()
            {
                SourceFilePath = filePicked.FilePath,
                DestinationFolder = Path.Combine(Sys.AppPath(), "PatientScreening"),
                NewFileName = $"{CurrentRecord.TeethScreen?.Patient?.PatientID}_{CurrentRecord?.TeethScreen?.Patient?.FirstName}_{CurrentRecord?.TeethScreen?.Patient?.LastName}_TEETH_SCREEN.{filePicked.Extension}"
            };

            fileTransfer.Copy();

            CurrentRecord.ScreenPath = fileTransfer.DestinationFilePath;
        }

        public override async void OnSubFormFilter()
        {
            ReloadSearchQry();
            SearchQry.AddParameter("teethScreenID", ParentRecord?.GetPrimaryKey()?.GetValue());
            RecordSource<Screening> results = await CreateFromAsyncList(SearchQry.Statement(), SearchQry.Params());
            AsRecordSource().ReplaceRange(results);
            GoFirst();
        }

        public override AbstractClause InstantiateSearchQry() =>
            new Screening()
            .Select()
                .All()
            .From()
            .Where().EqualsTo("TeethScreenID", "@teethScreenID")
            .OrderBy().Field("ScreeningID DESC");
    }
}
