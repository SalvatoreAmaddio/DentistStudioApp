﻿using Backend.Database;
using Backend.ExtensionMethods;
using Backend.Model;
using Backend.Utils;
using FrontEnd.Model;
using System.Data.Common;
using System.IO;

namespace DentistStudioApp.Model
{
    [Table(nameof(Patient))]
    public class Patient : AbstractModel<Patient>
    {
        #region backing fields
        private long _patientID;
        private string _firstName = string.Empty;
        private string _lastName = string.Empty;
        private DateTime? _dob;
        private string _phoneNumber = string.Empty;
        private string _email = string.Empty;
        private string _picturePath = string.Empty;
        private JobTitle? _jobTitle;
        private Gender? _gender;
        #endregion

        #region properties
        [PK] //This attribute tells this property represents the Primary Key of the Employee Table.
        public long PatientID { get => _patientID; set => UpdateProperty(ref value, ref _patientID); }

        [Mandatory] //This attribute tells this property cannot be null or empty if it is a string.
        [Field] //This attribute tells this property represents a Field of the Employee Table.
        public string FirstName { get => _firstName; set => UpdateProperty(ref value, ref _firstName); }

        [Mandatory] //This attribute tells this property cannot be null or empty if it is a string.
        [Field] //This attribute tells this property represents a Field of the Employee Table.
        public string LastName { get => _lastName; set => UpdateProperty(ref value, ref _lastName); }

        [Mandatory] //This attribute tells this property cannot be null or empty if it is a string.
        [Field] //This attribute tells this property represents a Field of the Employee Table.
        public DateTime? DOB { get => _dob; set => UpdateProperty(ref value, ref _dob); }

        [Mandatory] //This attribute tells this property cannot be null or empty if it is a string.
        [FK] //This attribute tells this property represents a Foreign Key of the Employee Table.
        public Gender? Gender { get => _gender; set => UpdateProperty(ref value, ref _gender); }

        [Mandatory] //This attribute tells this property cannot be null or empty if it is a string.
        [FK] //This attribute tells this property represents a Foreign Key of the Employee Table.
        public JobTitle? JobTitle { get => _jobTitle; set => UpdateProperty(ref value, ref _jobTitle); }

        [Mandatory] //This attribute tells this property cannot be null or empty if it is a string.
        [Field] //This attribute tells this property represents a Field of the Employee Table.
        public string Email { get => _email; set => UpdateProperty(ref value, ref _email); }

        [Mandatory] //This attribute tells this property cannot be null or empty if it is a string.
        [Field] //This attribute tells this property represents a Field of the Employee Table.
        public string PhoneNumber { get => _phoneNumber; set => UpdateProperty(ref value, ref _phoneNumber); }

        [Field] //This attribute tells this property represents a Field of the Employee Table.
        public string PicturePath { get => _picturePath; set => UpdateProperty(ref value, ref _picturePath); }
        
        public string Title 
        { 
            get 
            {
                if (Gender == null) return string.Empty;
                if (Gender.GenderID == 1) return "Mr.";
                if (Gender.GenderID == 2) return "Ms.";
                return string.Empty;
            }
        }
        #endregion

        #region Constructor
        public Patient()
        {
            AfterUpdate += OnAfterUpdate;
            BeforeRecordDelete += OnBeforeRecordDelete;
        }

        public Patient(long id) : this() => _patientID = id;
        public Patient(DbDataReader reader) : this()
        {
            _patientID = reader.GetInt64(0);
            _firstName = reader.GetString(1);
            _lastName = reader.GetString(2);
            _dob = reader.TryFetchDate(3);
            _gender = new(reader.GetInt64(4));
            _jobTitle = new(reader.GetInt64(5));
            _phoneNumber = reader.GetString(6);
            _email = reader.GetString(7);
            _picturePath = reader.TryFetchString(8);
        }
        #endregion

        #region Subscription Events
        private void OnBeforeRecordDelete(object? sender, EventArgs e) => Sys.AttemptFileDelete(Path.Combine(Sys.AppPath(), "PatientScreening", PicturePath));
        private void OnAfterUpdate(object? sender, FrontEnd.Events.AfterUpdateArgs e)
        {
            if (e.Is(nameof(FirstName)))
                _firstName = e.ConvertNewValueTo<string>().FirstLetterCapital();
            if (e.Is(nameof(LastName)))
                _lastName = e.ConvertNewValueTo<string>().FirstLetterCapital();
        }
        #endregion

        public async Task<long?> TreatmentCount()
        {
            IAbstractDatabase? treatmentDB = DatabaseManager.Find<Treatment>() ?? throw new NullReferenceException();
            List<QueryParameter> para = [];
            para.Add(new QueryParameter("id", PatientID));
            para.Add(new QueryParameter("invoiced", false));
            return await treatmentDB.CountRecordsAsync($"SELECT * FROM {nameof(Treatment)} WHERE PatientID = @id AND Invoiced = @invoiced", para);
        }
        public override string ToString() => $"{FirstName} {LastName}";
    }

    [Table(nameof(Patient))]
    public class PatientReport : AbstractModel<PatientReport>
    {
        [PK]
        public long PatientID { get; }

        [Field]
        public string FirstName { get; } = string.Empty;

        [Field]
        public string LastName { get; } = string.Empty;

        [Field]
        public string DOB { get; } = string.Empty;

        [FK]
        public Gender? Gender { get; }

        [FK]
        public JobTitle? JobTitle { get; }

        [Field]
        public string PhoneNumber { get; } = string.Empty;

        [Field]
        public string Email { get; } = string.Empty;

        public PatientReport() { }

        public PatientReport(DbDataReader reader) 
        {
            PatientID = reader.GetInt64(0);
            FirstName = reader.GetString(1);
            LastName = reader.GetString(2);
            DateTime? date = reader.TryFetchDate(3);
            
            if (date.HasValue)
                DOB = date.Value.ToString("dd/MM/yyyy");

            Gender = new(reader.GetInt64(4));
            JobTitle = new(reader.GetInt64(5));
            PhoneNumber = reader.GetString(6);
            Email = reader.GetString(7);
        }
    }

    public class PatientWithTreatment : AbstractModel<PatientWithTreatment>
    {
        [PK]
        public long PatientID { get; }
        [Field]
        public string FirstName { get; } = string.Empty;
        [Field]
        public string LastName { get; } = string.Empty;
        [Field]
        public string StartDate { get; } = string.Empty;
        [Field]
        public string EndDate { get; } = string.Empty;
        [Field]
        public string DOA { get; } = string.Empty;
        [Field]
        public string TOA { get; } = string.Empty;
        [FK]
        public Service? Service { get; }
        [FK]
        public Dentist? Dentist { get; }

        [Field]
        public string ClinicName { get; } = string.Empty;

        [Field]
        public int RoomNumber { get; }
        [Field]
        public string Attended { get; } = string.Empty;

        public PatientWithTreatment()
        {
            SelectQry = "SELECT Patient.PatientiD, Patient.FirstName, Patient.LastName, Treatment.StartDate, Treatment.EndDate, Appointment.DOA, Appointment.TOA, Appointment.ServiceID, Appointment.DentistID, Clinic.ClinicName, Appointment.RoomNumber, Appointment.Attended FROM Patient INNER JOIN Treatment on Patient.PatientiD = Treatment.PatientID INNER JOIN Appointment on Treatment.TreatmentID = Appointment.TreatmentID INNER JOIN Dentist ON Appointment.DentistID = Dentist.DentistID INNER JOIN Clinic ON Dentist.ClinicID = Clinic.ClinicID";
        }

        public PatientWithTreatment(DbDataReader reader) : this()
        {
            PatientID = reader.GetInt64(0);
            FirstName = reader.GetString(1);
            LastName = reader.GetString(2);

            DateTime? startDate = reader.TryFetchDate(3);
            if (startDate.HasValue)
                StartDate = startDate.Value.ToString("dd/MM/yyyy");

            DateTime? endDate = reader.TryFetchDate(4);
            if (endDate.HasValue)
                EndDate = endDate.Value.ToString("dd/MM/yyyy");

            DateTime? doa = reader.TryFetchDate(5);
            if (doa.HasValue)
                DOA = doa.Value.ToString("dd/MM/yyyy");
            
            TimeSpan? toa = reader.TryFetchTime(6);
            if (toa.HasValue)
                TOA = new DateTime(toa.Value.Ticks).ToString("h:mm tt");

            Service = new(reader.GetInt64(7));
            Dentist = new(reader.GetInt64(8));
            ClinicName = reader.GetString(9);
            RoomNumber = reader.GetInt32(10);
            Attended = (reader.GetBoolean(11)) ? "YES" : "NO";
        }

    }
}