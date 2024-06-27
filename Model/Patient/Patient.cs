using Backend.Database;
using Backend.ExtensionMethods;
using Backend.Model;
using Backend.Utils;
using FrontEnd.Model;
using System.Data.Common;
using System.IO;

namespace DentistStudioApp.Model
{
    [Table(nameof(Patient))]
    public class Patient : AbstractModel
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
        public override ISQLModel Read(DbDataReader reader) => new Patient(reader);
        public override string ToString() => $"{FirstName} {LastName}";
    }
}