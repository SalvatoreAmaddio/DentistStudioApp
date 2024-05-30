using Backend.Model;
using FrontEnd.Model;
using System.Data.Common;

namespace DentistStudioApp.Model
{
    [Table(nameof(SurveyData))]
    public class SurveyData : AbstractModel
    {
        #region backing fields
        long _surveyDataID;
        Survey? _survey;
        SurveyQuestion? _surveyQuestion;
        #endregion

        #region Properties
        [PK]
        public long SurveyDataID { get => _surveyDataID; set => UpdateProperty(ref value, ref _surveyDataID); }

        [FK]
        [Mandatory]
        public Survey? Survey { get => _survey; set => UpdateProperty(ref value, ref _survey); }

        [FK]
        [Mandatory]
        public SurveyQuestion? SurveyQuestion { get => _surveyQuestion; set => UpdateProperty(ref value, ref _surveyQuestion); }
        #endregion

        #region Constructor
        public SurveyData() { }

        public SurveyData(DbDataReader reader) 
        {
            _surveyDataID = reader.GetInt64(0);
            _survey = new Survey(reader);
            _surveyQuestion = new SurveyQuestion(reader);
        }
        #endregion

        public override ISQLModel Read(DbDataReader reader) => new SurveyData(reader);

        public override string? ToString() => $"{Survey} {SurveyQuestion}";

    }
}
