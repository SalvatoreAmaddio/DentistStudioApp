using Backend.Database;
using Backend.ExtensionMethods;
using Backend.Model;
using FrontEnd.Model;
using FrontEnd.Source;
using System.Data.Common;

namespace DentistStudioApp.Model
{
    [Table(nameof(Appointment))]
    public class Appointment : AbstractModel<Appointment>
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
            _doa = reader.TryFetchDate(1);
            _toa = reader.TryFetchTime(2);
            _notes = reader.GetString(3);
            _attended = reader.GetBoolean(4);
            _roomNumber = reader.GetInt32(5);
            _dentist = new Dentist(reader.GetInt64(6));
            _service = new Service(reader.GetInt64(7));
            _treatment = new Treatment(reader.GetInt64(8));
        }
        #endregion

        public async Task SetTreatmentAsync()
        {
            IAbstractDatabase? db = DatabaseManager.Find<Treatment>() ?? throw new NullReferenceException();
            string? sql = new Treatment().Select().All().From().Where().EqualsTo("TreatmentID", "@treatmentID").Limit().Statement();
            List<QueryParameter> para = [];
            para.Add(new("treatmentID", this.Treatment?.TreatmentID));
            RecordSource<Treatment> result = await RecordSource<Treatment>.CreateFromAsyncList(db.RetrieveAsync(sql, para).Cast<Treatment>());
            this._treatment = result.FirstOrDefault();
            if (_treatment != null)
                await this._treatment.SetPatientAsync();
        }

        public override string ToString() => $"Date: {DOA} - Time: {TOA} - Service: {Service} - Dentist: {Dentist} - Room Number: {RoomNumber} Attended: {Attended}";
    }


    [Table(nameof(Appointment))]
    public class AppointmentServices : AbstractModel<AppointmentServices>
    {
        [PK]
        public long AppointmentID { get; }
        [Field]
        public DateTime? DOA { get; }

        [FK]
        public Service? Service { get; }

        public AppointmentServices() 
        { 
            
        }

        public AppointmentServices(DbDataReader reader) : this()
        {
           AppointmentID = reader.GetInt64(0);
           DOA = reader.TryFetchDate(1);
           Service = new(reader.GetInt64(2))
           {
            ServiceName = reader.GetString(3),
            Cost = reader.GetDouble(4)
           };
        }

        public static async Task<IEnumerable<AppointmentServices>> GetInvoicedServices(long invoiceid)
        {
            string? sql = new Appointment()
                            .Select(nameof(AppointmentID), nameof(DOA), "Service.ServiceID", "Service.ServiceName", "Service.Cost")
                            .From()
                            .InnerJoin(nameof(Treatment), "TreatmentID")
                            .InnerJoin(nameof(InvoicedTreatment), "TreatmentID")
                            .InnerJoin(nameof(Service), "ServiceID")
                            .Where().EqualsTo("InvoicedTreatment.InvoiceID", "@invoiceID").Statement();

            List<QueryParameter> para = [];
            para.Add(new("invoiceID", invoiceid));
            return await RecordSource<AppointmentServices>.CreateFromAsyncList(new SQLiteDatabase<AppointmentServices>().RetrieveAsync(sql, para).Cast<AppointmentServices>());
        }
    }
}