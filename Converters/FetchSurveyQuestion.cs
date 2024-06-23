using System.Globalization;
using DentistStudioApp.Model;
using FrontEnd.Converters;
using FrontEnd.Model;
using Backend.ExtensionMethods;

namespace DentistStudioApp.Converters
{
    public abstract class AbstractFetchSurveyQuestion<M, D> : AbstractFetchModel<M, D> where M : AbstractModel, new() where D : AbstractModel, new()
    {
        public override object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Record;
    }

    public class FetchSurveyQuestion : AbstractFetchSurveyQuestion<SurveyQuestion, SurveyQuestion>
    {
        protected override string Sql =>
            new SurveyQuestion()
            .Select()
            .From()
            .Where().EqualsTo("SurveyQuestionID", "@id")
            .Limit()
            .Statement();
        public override object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Record = (SurveyQuestion)value;
            para.Add(new("id", Record?.SurveyQuestionID));
            return ((SurveyQuestion?)Db?.Retrieve(Sql,para).FirstOrDefault())?.Question;
        }

    }

    public class FetchSurveyQuestionCategory : AbstractFetchSurveyQuestion<SurveyQuestion, SurveyQuestionCategory>
    {
        protected override string Sql => 
            new SurveyQuestionCategory()
            .Select()
            .From().InnerJoin("SurveyQuestion", "SurveyQuestionCategoryID")
            .Where().EqualsTo("SurveyQuestionID", "@id")
            .Limit()
            .Statement();

        public override object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Record = (SurveyQuestion)value;
            para.Add(new("id", Record?.SurveyQuestionID));
            return ((SurveyQuestionCategory?)Db?.Retrieve(Sql, para).FirstOrDefault())?.CategoryName;
        }
    }
}