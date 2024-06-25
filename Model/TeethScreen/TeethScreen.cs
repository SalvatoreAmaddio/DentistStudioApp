using Backend.ExtensionMethods;
using Backend.Model;
using FrontEnd.Model;
using System.Data.Common;

namespace DentistStudioApp.Model
{
    public class TeethScreen : AbstractModel
    {
        #region backing fields
        private long _teethScreenId;
        private DateTime? _dos;
        private Patient? _patient;
        #endregion

        #region Properties
        public long TeethScreenID { get => _teethScreenId; set => UpdateProperty(ref value, ref _teethScreenId); }
        public DateTime? DOS { get => _dos; set => UpdateProperty(ref value, ref _dos); }
        public Patient? Patient { get => _patient; set => UpdateProperty(ref value, ref _patient); }
        #endregion

        #region Constructors
        public TeethScreen() { }
        public TeethScreen(long _id) => this._teethScreenId = _id;
        public TeethScreen(Patient _patient) => this._patient = _patient; 
        public TeethScreen(DbDataReader reader) 
        {
            _teethScreenId = reader.GetInt64(0);
            _dos = reader.TryFetchDate(1);
            _patient = new(reader.GetInt64(2));
        }
        #endregion

        public override ISQLModel Read(DbDataReader reader) => new TeethScreen(reader);

        public override string ToString() => $"Teeth Screen on {DOS} for {Patient}";

    }
}