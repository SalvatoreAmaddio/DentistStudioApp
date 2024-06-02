using Backend.Model;
using Backend.Utils;
using FrontEnd.Model;
using System.Data.Common;

namespace DentistStudioApp.Model
{
    [Table(nameof(Appointment))]
    public class Appointment : AbstractModel
    {
        #region backing fields
        private long _appointmentID;
        private DateTime? _doa;
        private TimeSpan? _toa;
        private string _notes = string.Empty;
        private bool _attended;
        private int _roomNumber;
        private Dentist? _dentist;
        private Service? _service;
        private Treatment? _treatment;
        #endregion

        #region Properties
        [PK]
        public long AppointmentID { get => _appointmentID; set => UpdateProperty(ref value, ref _appointmentID); }

        [Field]
        public DateTime? DOA { get => _doa; set => UpdateProperty(ref value, ref _doa); }

        [Field]
        public TimeSpan? TOA { get => _toa; set => UpdateProperty(ref value, ref _toa); }

        [Field]
        public string Notes { get => _notes; set => UpdateProperty(ref value, ref _notes); }

        [Field]
        public bool Attended { get => _attended; set => UpdateProperty(ref value, ref _attended); }

        [Field]
        public int RoomNumber { get => _roomNumber; set => UpdateProperty(ref value, ref _roomNumber); }

        [FK]        
        public Dentist? Dentist { get => _dentist; set => UpdateProperty(ref value, ref _dentist); }

        [FK]
        public Service? Service { get => _service; set => UpdateProperty(ref value, ref _service); }

        [FK]
        public Treatment? Treatment { get => _treatment; set => UpdateProperty(ref value, ref _treatment); }
        #endregion

        #region Constructor
        public Appointment() { }

        public Appointment(DbDataReader reader) 
        {
            _appointmentID = reader.GetInt64(0);
            _doa = Sys.GetDate(reader,1);
            _toa = Sys.GetTime(reader, 2);
            _notes = reader.GetString(3);
            _attended = reader.GetBoolean(4);
            _roomNumber = reader.GetInt32(5);
            _dentist = new Dentist(reader.GetInt64(6));
            _service = new Service(reader.GetInt64(7));
            _treatment = new Treatment(reader.GetInt64(8));
        }
        #endregion

        public override ISQLModel Read(DbDataReader reader) => new Appointment(reader);

    }
}
