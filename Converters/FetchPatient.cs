using Backend.Database;
using Backend.ExtensionMethods;
using DentistStudioApp.Model;
using System.Globalization;
using System.Windows.Data;

namespace DentistStudioApp.Converters
{
    public class FetchPatient : IValueConverter
    {
        private Invoice? invoice;
        IAbstractDatabase? db => DatabaseManager.Find<Patient>();

        private readonly string sql = new InvoicedTreatment()
        .Select("Patient.*")
        .From()
        .InnerJoin(nameof(Treatment), "TreatmentID")
        .InnerJoin(nameof(Treatment), nameof(Patient),"PatientID")
        .Where().EqualsTo("InvoicedTreatment.InvoiceID", "@id")
        .Limit().Statement();

        List<QueryParameter> para = [];

        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            invoice = (Invoice)value;
            para.Add(new("id",invoice.InvoiceID));
            return (Patient?)db?.Retrieve(sql, para).FirstOrDefault();
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => invoice;
    }
}
