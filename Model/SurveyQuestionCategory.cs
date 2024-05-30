using Backend.Model;
using FrontEnd.Model;
using System.Data.Common;

namespace DentistStudioApp.Model
{
    [Table(nameof(SurveyQuestionCategory))]
    public class SurveyQuestionCategory : AbstractModel
    {
        #region backing fields
        long _surveyQuestionCategoryID;
        string _categoryName = string.Empty;
        #endregion

        #region Properties
        [PK]
        public long SurveyQuestionCategoryID { get => _surveyQuestionCategoryID; set => UpdateProperty(ref value, ref _surveyQuestionCategoryID); }

        [Mandatory]
        [Field]
        public string CategoryName { get => _categoryName;set=>UpdateProperty(ref value, ref _categoryName); }
        #endregion

        #region Constructor
        public SurveyQuestionCategory() { }
        public SurveyQuestionCategory(long id) => _surveyQuestionCategoryID = id;

        public SurveyQuestionCategory(DbDataReader reader) 
        {
            _surveyQuestionCategoryID = reader.GetInt64(0);
            _categoryName = reader.GetString(1);
        }
        #endregion

        public override ISQLModel Read(DbDataReader reader) => new SurveyQuestionCategory(reader);

        public override string ToString() => CategoryName;

    }
}
