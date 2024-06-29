using Backend.Database;
using Backend.ExtensionMethods;
using Backend.Model;
using FrontEnd.Model;
using FrontEnd.Source;
using System.Data.Common;

namespace DentistStudioApp.Model
{
    [Table(nameof(Treatment))]
    public class Treatment : AbstractModel<Treatment>
    {
        #region backing fields
        private long _treatmentId;
        private DateTime? _startDate;
        private DateTime? _endDate;
        private Patient? _patient;
        private int _serviceCount;
        private bool _invoiced;
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
        [Field]
        public bool Invoiced { get => _invoiced; set => UpdateProperty(ref value, ref _invoiced); }
        public int ServiceCount { get => _serviceCount; private set => UpdateProperty(ref value, ref _serviceCount); }
        #endregion

        #region Constructor
        public Treatment()
        {
            SelectQry = this.Select()
                .All()
                .Fields("count(Service.ServiceID) AS ServiceCount")
                .From().LeftJoin(nameof(Appointment), "TreatmentID")
                .LeftJoin(nameof(Appointment),nameof(Service), "ServiceID")
                .GroupBy().Fields("Treatment.TreatmentID")
                .Statement();
        }
        public Treatment(long treatmentid) : this() => _treatmentId = treatmentid;
        public Treatment(DbDataReader reader) : this()
        {
            _treatmentId = reader.GetInt64(0);
            _startDate = reader.TryFetchDate(1);
            _endDate = reader.TryFetchDate(2);
            _patient = new Patient(reader.GetInt64(3));
            _invoiced = reader.GetBoolean(4);
            _serviceCount = reader.TryFetchInt32(5);
        }
        #endregion

        public async Task SetPatientAsync()
        {
            IAbstractDatabase? db = DatabaseManager.Find<Patient>() ?? throw new NullReferenceException();
            string? sql = new Patient().Select().All().From().Where().EqualsTo("PatientID", "@patientID").Limit().Statement();
            List<QueryParameter> para = [];
            para.Add(new("patientID", this.Patient?.PatientID));
            RecordSource<Patient> result = await RecordSource<Patient>.CreateFromAsyncList(db.RetrieveAsync(sql, para).Cast<Patient>());
            this._patient = result.FirstOrDefault();
        }
        public async Task<object?> GetTotalCost() 
        {
            IAbstractDatabase? appointmentDB = DatabaseManager.Find<Appointment>() ?? throw new NullReferenceException();
            List<QueryParameter> para = [];
            string? sql = new Appointment().Sum("Service.Cost").From().InnerJoin(new Service()).InnerJoin(new Treatment()).Where().EqualsTo("Treatment.TreatmentID", "@id").Statement();
            para.Add(new("id", TreatmentID));
            return await appointmentDB.AggregateQueryAsync(sql,para);
        }
        public override string? ToString() => $"TreatmentID: {TreatmentID} - {Patient} - {IsDirty}";
    }

}