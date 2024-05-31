using Backend.Model;
using FrontEnd.Model;
using System.Data.Common;

namespace DentistStudioApp.Model
{
    [Table(nameof(Survey))]
    public class Survey : AbstractModel
    {
        #region backing fields
        long _surveyId;
        DateTime? _dos;
        Patient? _patient;
        #endregion

        #region Properties
        [PK]
        public long SurveyID { get => _surveyId; set => UpdateProperty(ref value, ref _surveyId); }

        [Mandatory]
        [Field]
        public DateTime? DOS { get => _dos; set => UpdateProperty(ref value, ref _dos); }

        [Mandatory]
        [FK]
        public Patient? Patient { get => _patient; set=>UpdateProperty(ref value, ref _patient); }
        #endregion


        #region Constructor
        public Survey() { }

        public Survey(long id) => _surveyId = id;
        public Survey(Patient patient) 
        {
            _patient = patient;
            _dos = DateTime.Now;
        }

        public Survey(DbDataReader reader) 
        {
            _surveyId = reader.GetInt64(0);
            _dos = reader.GetDateTime(1);
            _patient = new(reader.GetInt64(2));
        }
        #endregion

        public override ISQLModel Read(DbDataReader reader) => new Survey(reader);

        public override string ToString() => $"Survey ID: {SurveyID} - {Patient}";

    }
}
