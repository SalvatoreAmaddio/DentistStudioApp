using System.Globalization;
using DentistStudioApp.Model;
using System.Windows.Data;
using Backend.Database;

namespace DentistStudioApp.Converters
{
    public abstract class AbstractFetchSurveyQuestion : IValueConverter 
    {
        protected SurveyQuestion? SurveyQuestion;
        public abstract object? Convert(object value, Type targetType, object parameter, CultureInfo culture);
        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => SurveyQuestion;
    }

    public class FetchSurveyQuestion : AbstractFetchSurveyQuestion
    {
        IAbstractDatabase? db = DatabaseManager.Find<SurveyQuestion>();
        public override object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            SurveyQuestion = (SurveyQuestion)value;
            SurveyQuestion = (SurveyQuestion?)db?.Retrieve($"SELECT * FROM {nameof(SurveyQuestion)} WHERE SurveyQuestionID = {SurveyQuestion.SurveyQuestionID}").FirstOrDefault();
            return SurveyQuestion?.Question;
        }

    }

    public class FetchSurveyQuestionCategory : AbstractFetchSurveyQuestion
    {
        private IAbstractDatabase? db = DatabaseManager.Find<SurveyQuestionCategory>();
        SurveyQuestionCategory? category;
        public override object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            SurveyQuestion = (SurveyQuestion)value;
            category = (SurveyQuestionCategory?)db?.Retrieve($"SELECT SurveyQuestionCategory.* FROM SurveyQuestionCategory INNER JOIN SurveyQuestion ON SurveyQuestionCategory.SurveyQuestionCategoryID = SurveyQuestion.SurveyQuestionCategoryID WHERE SurveyQuestionID = {SurveyQuestion?.SurveyQuestionID}").FirstOrDefault();
            return category?.CategoryName;
        }
    }
}
