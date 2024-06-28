using Backend.ExtensionMethods;
using Backend.Model;
using FrontEnd.Model;
using System.Data.Common;

namespace DentistStudioApp.Model
{
    [Table(nameof(TeethScreen))]
    public class TeethScreen : AbstractModel<TeethScreen>
    {
        #region backing fields
        private long _teethScreenId;
        private DateTime? _dos = DateTime.Today;
        private Patient? _patient;
        #endregion

        #region Properties
        [PK]
        public long TeethScreenID { get => _teethScreenId; set => UpdateProperty(ref value, ref _teethScreenId); }
        
        [Field]
        public DateTime? DOS { get => _dos; set => UpdateProperty(ref value, ref _dos); }

        [FK]
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

        public override string ToString() => $"Teeth Screen on {DOS} for {Patient}";
    }
}