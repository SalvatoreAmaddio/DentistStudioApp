using Backend.Model;
using Backend.Utils;
using FrontEnd.Model;
using System.Data.Common;
using System.IO;

namespace DentistStudioApp.Model
{
    [Table(nameof(TeethScreenData))]
    public class TeethScreenData : AbstractModel<TeethScreenData>
    {
        #region Backing Fields
        private long _teethScreenDataId;
        private TeethScreen? _teethScreen;
        private string _screenPath = string.Empty;
        #endregion

        #region Properties
        [PK]
        public long TeethScreenDataID { get => _teethScreenDataId; set => UpdateProperty(ref value, ref _teethScreenDataId); }
        [FK]
        public TeethScreen? TeethScreen { get => _teethScreen; set => UpdateProperty(ref value, ref _teethScreen); }
        [Field]
        public string ScreenPath { get => _screenPath; set => UpdateProperty(ref value, ref _screenPath); }
        #endregion

        #region Constructors
        public TeethScreenData() 
        {
            BeforeRecordDelete += OnBeforeRecordDelete;
        }

        public TeethScreenData(DbDataReader reader) : this()
        {
            _teethScreenDataId = reader.GetInt64(0);
            _teethScreen = new TeethScreen(reader.GetInt64(1));
            _screenPath = reader.GetString(2);
        }
        #endregion

        private void OnBeforeRecordDelete(object? sender, EventArgs e) => Sys.AttemptFileDelete(Path.Combine(Sys.AppPath(), "PatientScreening", ScreenPath));

        public override string ToString() => $"{TeethScreen} - {ScreenPath}";
    }
}