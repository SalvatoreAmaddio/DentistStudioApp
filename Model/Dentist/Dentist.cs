using Backend.Model;
using FrontEnd.Model;
using System.Data.Common;

namespace DentistStudioApp.Model
{
    [Table(nameof(Dentist))]
    public class Dentist : AbstractModel
    {
        #region backing fields
        private long _dentistId;
        private string _firstName = string.Empty;
        private string _lastName = string.Empty;
        private Clinic? _clinic;
        bool _active = false;
        #endregion

        #region Properties
        [PK]
        public long DentistID { get => _dentistId; set => UpdateProperty(ref value, ref _dentistId); }
        [Field]
        public string FirstName { get => _firstName; set => UpdateProperty(ref value, ref _firstName); }
        [Field]
        public string LastName { get => _lastName; set => UpdateProperty(ref value, ref _lastName); }
        [FK]
        public Clinic? Clinic { get => _clinic; set => UpdateProperty(ref value, ref _clinic); }
        [Field]
        public bool Active { get => _active; set => UpdateProperty(ref value, ref _active); }
        #endregion

        #region Constructor
        public Dentist(DbDataReader reader) 
        { 
            _dentistId = reader.GetInt64(0);
            _firstName = reader.GetString(1);
            _lastName = reader.GetString(2);
            _clinic = new Clinic(reader.GetInt64(3));
            _active = reader.GetBoolean(4);
        }

        public Dentist() { }

        public Dentist(long id) => _dentistId = id;
        #endregion

        public override ISQLModel Read(DbDataReader reader) => new Dentist(reader);

        public override string? ToString() => $"{FirstName} {LastName} at {Clinic}";

    }
}
