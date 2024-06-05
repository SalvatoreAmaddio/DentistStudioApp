﻿using Backend.Database;
using Backend.ExtensionMethods;
using Backend.Model;
using Backend.Utils;
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

        public Task FetchPatientAsync()
        {
            _patient = (Patient?)DatabaseManager.Find<Patient>()?.MasterSource.FirstOrDefault(s => s.Equals(_patient));
            return Task.CompletedTask;
        }

        public async Task CountServices() 
        {

            IAbstractDatabase? appointmentDB = DatabaseManager.Find<Appointment>();
            if (appointmentDB == null) throw new NullReferenceException();
            long? count = await appointmentDB.CountRecordsAsync($"SELECT Count(*) FROM {nameof(Appointment)} WHERE {nameof(TreatmentID)} = {TreatmentID}");
            if (count != null) 
                _totalServices = (int)count;
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