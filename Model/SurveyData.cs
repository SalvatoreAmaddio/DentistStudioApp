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
        bool _have = false;
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

        [Field]
        public bool Have { get => _have; set => UpdateProperty(ref value, ref _have); }
        #endregion

        #region Constructor
        public SurveyData() { }

        public SurveyData(Survey survey, SurveyQuestion surveyQuestion) 
        { 
            Survey = survey;
            SurveyQuestion = surveyQuestion;    
        }

        public SurveyData(DbDataReader reader) 
        {
            _surveyDataID = reader.GetInt64(0);
            _survey = new Survey(reader.GetInt64(1));
            _surveyQuestion = new SurveyQuestion(reader.GetInt64(2));
            _have = reader.GetBoolean(3);
        }
        #endregion

        public override ISQLModel Read(DbDataReader reader) => new SurveyData(reader);

        public override string? ToString() => $"{Survey} {SurveyQuestion}";

    }
}
