using Backend.Model;
using FrontEnd.Model;
using System.Data.Common;

namespace DentistStudioApp.Model
{
    [Table(nameof(Screening))]
    public class Screening : AbstractModel
    {
        #region Backing Fields
        private long _screeningId;
        private TeethScreen? _teethScreen;
        private string _screenPath = string.Empty;
        #endregion

        #region Properties
        [PK]
        public long ScreeningID { get => _screeningId; set => UpdateProperty(ref value, ref _screeningId); }
        [FK]
        public TeethScreen? TeethScreen { get => _teethScreen; set => UpdateProperty(ref value, ref _teethScreen); }
        [Field]
        public string ScreenPath { get => _screenPath; set => UpdateProperty(ref value, ref _screenPath); }
        #endregion

        #region Constructors
        public Screening() { }
        public Screening(DbDataReader reader) 
        {
            _screeningId = reader.GetInt64(0);
            _teethScreen = new TeethScreen(reader.GetInt64(1));
            _screenPath = reader.GetString(2);
        }
        #endregion

        public override ISQLModel Read(DbDataReader reader) => new Screening(reader);

        public override string ToString() => $"{TeethScreen} - {ScreenPath}";

    }
}
