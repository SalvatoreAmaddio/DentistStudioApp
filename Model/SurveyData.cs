using Backend.Database;
using Backend.Model;
using FrontEnd.Model;
using System.Data.Common;

namespace DentistStudioApp.Model
{
    [Table(nameof(SurveyData))]
    public class SurveyData : AbstractModel
    {
        #region backing fields
        private long _surveyDataID;
        private Survey? _survey;
        private SurveyQuestion? _surveyQuestion;
        private bool _have = false;
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
            _survey = survey;
            _surveyQuestion = surveyQuestion;
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

        public async Task FetchSurveyQuestionAsync()
        {
            _surveyQuestion = (SurveyQuestion?)DatabaseManager.Find<SurveyQuestion>()?.MasterSource.FirstOrDefault(s => s.Equals(_surveyQuestion));
            if (_surveyQuestion != null) 
                await _surveyQuestion.FetchCategory();
        }

        public override string? ToString() => $"{Survey} {SurveyQuestion}";

    }
}
