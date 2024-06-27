using Backend.Model;
using Backend.Utils;
using DentistStudioApp.Model;
using FrontEnd.Controller;
using FrontEnd.Forms;
using FrontEnd.Source;
using System.IO;
using System.Windows.Input;
using Backend.ExtensionMethods;
using FrontEnd.Utils;

namespace DentistStudioApp.Controller
{
    public class TeethScreenDataController : AbstractFormController<TeethScreenData>
    {
        public override int DatabaseIndex => 16;
        public ICommand FilePickedCMD { get; }

        public TeethScreenDataController() 
        {
            FilePickedCMD = new Command<FilePickerCatch>(PickPicture);
            AfterRecordNavigation += OnAfterRecordNavigation;
        }

        #region Event Subscriptions
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
        #endregion

        private void PickPicture(FilePickerCatch? filePicked)
        {
            if (filePicked == null) return;
            
            if (CurrentRecord == null) 
            {
                GoNew();
            }

            if (filePicked.FileRemoved)
            {
                string? temp = CurrentRecord?.ScreenPath;
                CurrentRecord!.ScreenPath = Helper.LoadFromStrings("uploadImagePath");
                Sys.AttemptFileDelete(temp);
                PerformUpdate();
                return;
            }

            if (string.IsNullOrEmpty(filePicked.FilePath)) return;

            CurrentRecord?.Dirt();

            Sys.CreateFolder(Path.Combine(Sys.AppPath(), "PatientScreening"));

            FileTransfer fileTransfer = new()
            {
                SourceFilePath = filePicked.FilePath,
                DestinationFolder = Path.Combine(Sys.AppPath(), "PatientScreening"),
                NewFileName = $"{CurrentRecord?.TeethScreen?.Patient?.PatientID}_{CurrentRecord?.TeethScreen?.Patient?.FirstName}_{CurrentRecord?.TeethScreen?.Patient?.LastName}_TEETH_SCREEN_ON_{CurrentRecord?.TeethScreen?.DOS}.{filePicked.Extension}"
            };

            fileTransfer.Copy();

            CurrentRecord!.ScreenPath = fileTransfer.NewFileName;
            PerformUpdate();
        }

        public override async void OnSubFormFilter()
        {
            ReloadSearchQry();
            SearchQry.AddParameter("teethScreenID", ParentRecord?.GetPrimaryKey()?.GetValue());
            RecordSource<TeethScreenData> results = await CreateFromAsyncList(SearchQry.Statement(), SearchQry.Params());
            AsRecordSource().ReplaceRange(results);
            GoFirst();
        }

        public override AbstractClause InstantiateSearchQry() =>
            new TeethScreenData()
            .Select()
                .All()
            .From()
            .Where().EqualsTo("TeethScreenID", "@teethScreenID")
            .OrderBy().Field("TeethScreenDataID DESC");

    }
}
