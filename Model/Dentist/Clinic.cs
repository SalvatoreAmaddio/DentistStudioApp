using Backend.ExtensionMethods;
using Backend.Model;
using FrontEnd.Model;
using System.Data.Common;

namespace DentistStudioApp.Model
{
    [Table(nameof(Clinic))]
    public class Clinic : AbstractModel<Clinic>
    {
        #region backign fields 
        private long _clinicId;
        private string _clinicName = string.Empty;
        #endregion

        #region Properties 
        [PK]
        public long ClinicID { get => _clinicId; set => UpdateProperty(ref value, ref _clinicId); }
        [Field]
        public string ClinicName { get => _clinicName; set => UpdateProperty(ref value, ref _clinicName); }
        #endregion

        #region Constructor
        public Clinic() => AfterUpdate += OnAfterUpdate;
        public Clinic(long clinicID) : this() => _clinicId = clinicID;
        public Clinic(DbDataReader reader) : this()
        {
            _clinicId = reader.GetInt64(0);
            _clinicName = reader.GetString(1);
        }
        #endregion

        private void OnAfterUpdate(object? sender, FrontEnd.Events.AfterUpdateArgs e)
        {
            if (e.Is(nameof(ClinicName)))
                _clinicName = e.ConvertNewValueTo<string>().FirstLetterCapital();
        }
        public override string? ToString() => $"{ClinicName}";

    }
}
