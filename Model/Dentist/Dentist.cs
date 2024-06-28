using Backend.ExtensionMethods;
using Backend.Model;
using FrontEnd.Model;
using System.Data.Common;

namespace DentistStudioApp.Model
{
    [Table(nameof(Dentist))]
    public class Dentist : AbstractModel<Dentist>
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

        public string FullName { get => $"{FirstName} {LastName}"; }
        #endregion

        #region Constructor
        public Dentist() => AfterUpdate += OnAfterUpdate;

        public Dentist(DbDataReader reader) : this()
        { 
            _dentistId = reader.GetInt64(0);
            _firstName = reader.GetString(1);
            _lastName = reader.GetString(2);
            _clinic = new Clinic(reader.GetInt64(3));
            _active = reader.GetBoolean(4);
        }

        public Dentist(long id) : this() => _dentistId = id;
        #endregion
        
        private void OnAfterUpdate(object? sender, FrontEnd.Events.AfterUpdateArgs e)
        {
            if (e.Is(nameof(FirstName)))
                _firstName = e.ConvertNewValueTo<string>().FirstLetterCapital();
            if (e.Is(nameof(LastName)))
                _lastName = e.ConvertNewValueTo<string>().FirstLetterCapital();
        }
        public override string? ToString() => $"{FirstName} {LastName}";

    }
}
