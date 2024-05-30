using Backend.Model;
using FrontEnd.Model;
using System.Data.Common;

namespace DentistStudioApp.Model
{
    [Table(nameof(SurveyQuestion))]
    public class SurveyQuestion : AbstractModel
    {
        #region backing fields
        long _surveyQuestionID;
        string _question = string.Empty;
        SurveyQuestionCategory? _category;
        #endregion

        #region Properties
        [PK]
        public long SurveyQuestionID { get => _surveyQuestionID; set=> UpdateProperty(ref value, ref _surveyQuestionID); }

        [Field]
        public string Question { get => _question; set => UpdateProperty(ref value, ref _question); }

        [FK]
        [Mandatory]
        public SurveyQuestionCategory? Category { get => _category; set=>UpdateProperty(ref value, ref _category); }
        #endregion

        #region Constructor
        public SurveyQuestion() { }

        public SurveyQuestion(DbDataReader reader) 
        {
            _surveyQuestionID = reader.GetInt64(0);
            _question = reader.GetString(1);
            _category = new(reader.GetInt64(2));
        }
        #endregion

        public override ISQLModel Read(DbDataReader reader) => new SurveyQuestion(reader);

        public override string ToString() => $"{Question} - {Category}";

    }
}
