using Backend.Model;
using FrontEnd.Model;
using System.Data.Common;

namespace DentistStudioApp.Model
{
    [Table(nameof(Treatment))]
    public class Treatment : AbstractModel
    {
        #region backing fields
        private long _treatmentId;
        private DateTime? _startDate;
        private DateTime? _endDate;
        private Patient? _patient;
        #endregion

        #region Properties
        [PK]
        public long TreatmentID { get => _treatmentId; set => UpdateProperty(ref value, ref _treatmentId); }
        [Field]
        public DateTime? StartDate { get => _startDate; set => UpdateProperty(ref value, ref _startDate); }
        [Field]
        public DateTime? EndDate { get => _endDate; set => UpdateProperty(ref value, ref _endDate); }
        [FK]
        public Patient? Patient { get => _patient; set => UpdateProperty(ref value, ref _patient); }
        #endregion

        #region Constructor
        public Treatment() { }

        public Treatment(DbDataReader reader) 
        {
            _treatmentId = reader.GetInt64(0);
            _startDate = reader.GetDateTime(1);
            _endDate = reader.GetDateTime(2);   
            _patient = new Patient(reader.GetInt64(4));
        }
        #endregion

        public override ISQLModel Read(DbDataReader reader) => new Treatment(reader);

        public override string? ToString() => $"TreatmentID: {TreatmentID} - {Patient}";

    }
}
