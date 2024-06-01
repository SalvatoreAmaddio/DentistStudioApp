using Backend.Model;
using FrontEnd.Model;
using System.Data.Common;

namespace DentistStudioApp.Model
{
    [Table(nameof(Clinic))]
    public class Clinic : AbstractModel
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
        public Clinic() { }

        public Clinic(long clinicID) => _clinicId = clinicID;
        public Clinic(DbDataReader reader) 
        {
            _clinicId = reader.GetInt64(0);
            _clinicName = reader.GetString(1);
        }
        #endregion

        public override ISQLModel Read(DbDataReader reader) => new Clinic(reader);

        public override string? ToString() => $"{ClinicName}";

    }
}
