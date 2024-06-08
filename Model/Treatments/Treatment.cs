using Backend.Database;
using Backend.ExtensionMethods;
using Backend.Model;
using FrontEnd.Model;
using FrontEnd.Source;
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
        private int _totalServices;
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

        public int TotalServices { get => _totalServices; private set => UpdateProperty(ref value, ref _totalServices); }
        #endregion

        #region Constructor
        public Treatment() { }
        public Treatment(long treatmentid) => _treatmentId = treatmentid;
        public Treatment(DbDataReader reader) 
        {
            _treatmentId = reader.GetInt64(0);
            _startDate = reader.TryFetchDate(1);
            _endDate = reader.TryFetchDate(2);
            _patient = new Patient(reader.GetInt64(3));
            _invoiced = reader.GetBoolean(4);
        }
        #endregion

        public override ISQLModel Read(DbDataReader reader) => new Treatment(reader);

        public override string? ToString() => $"TreatmentID: {TreatmentID} - {Patient}";

        public async Task CountServices() 
        {
            IAbstractDatabase? appointmentDB = DatabaseManager.Find<Appointment>();
            if (appointmentDB == null) throw new NullReferenceException();
            string sql = new Appointment().CountAll().Where().EqualsTo(nameof(TreatmentID), TreatmentID.ToString()).Statement();
            long? count = await appointmentDB.CountRecordsAsync(sql);
            if (count != null) 
                _totalServices = (int)count;
        }

        //SELECT Treatment.*
        //FROM Treatment LEFT JOIN InvoicedTreatment ON Treatment.TreatmentID = InvoicedTreatment.TreatmentID
        //WHERE InvoicedTreatment.InvoiceID IS NULL AND Treatment.PatientID = 1
        public static async Task<IEnumerable<Treatment>> GetToInvoice(long patientID)
        {
            IAbstractDatabase? treatmentDB = DatabaseManager.Find<Treatment>() ?? throw new NullReferenceException();
            Treatment treatment = new();
            string? sql = treatment.From().LeftJoin(nameof(InvoicedTreatment),"TreatmentID").Where().EqualsTo("Treatment.PatientID", "@patientID").AND().IsNull("InvoicedTreatment.InvoiceID").Statement();
            List<QueryParameter> para = [];
            para.Add(new("patientID", patientID));
            return await RecordSource<Treatment>.CreateFromAsyncList(treatmentDB.RetrieveAsync(sql, para).Cast<Treatment>());
        }

        //SELECT Treatment.*
        //FROM Treatment INNER JOIN InvoicedTreatment ON Treatment.TreatmentID = InvoicedTreatment.TreatmentID
        //WHERE InvoicedTreatment.InvoiceID IS NULL AND Treatment.PatientID = 1
        public static async Task<IEnumerable<Treatment>> GetInvoiced(long patientID, long invoiceID)
        {
            IAbstractDatabase? treatmentDB = DatabaseManager.Find<Treatment>() ?? throw new NullReferenceException();
            Treatment treatment = new();
            string? sql = treatment.From().InnerJoin(nameof(InvoicedTreatment), "TreatmentID").Where().EqualsTo("Treatment.PatientID", "@patientID").AND().EqualsTo("InvoicedTreatment.InvoiceID","@invoiceID").Statement();
            List<QueryParameter> para = [];
            para.Add(new("patientID", patientID));
            para.Add(new("invoiceID", invoiceID));
            return await RecordSource<Treatment>.CreateFromAsyncList(treatmentDB.RetrieveAsync(sql, para).Cast<Treatment>());
        }
        public async Task<object?> GetTotalCost() 
        {
            IAbstractDatabase? appointmentDB = DatabaseManager.Find<Appointment>() ?? throw new NullReferenceException();
            List<QueryParameter> para = [];
            string? sql = new Appointment().Sum("Service.Cost").From().InnerJoin(new Service()).InnerJoin(new Treatment()).Where().EqualsTo("Treatment.TreatmentID", "@id").Statement();
            para.Add(new("id", TreatmentID));
            return await appointmentDB.AggregateQueryAsync(sql,para);
        }

        public void UpdateTotalServiceCount(ArithmeticOperation arithmeticOperation)
        {
            if (arithmeticOperation == ArithmeticOperation.ADD)
                _totalServices++;
            else
                _totalServices--;
            RaisePropertyChanged(nameof(TotalServices));
        }

    }

    public enum ArithmeticOperation 
    { 
        ADD = 0,
        SUBTRACT = 1,
    }
}