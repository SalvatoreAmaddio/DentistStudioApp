using Backend.Database;
using Backend.ExtensionMethods;
using DentistStudioApp.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace DentistStudioApp.Converters
{
    public class FetchPatient : IValueConverter
    {
        IAbstractDatabase? db => DatabaseManager.Find<Patient>();

        string sql = new InvoicedTreatment()
        .SelectFields("Patient.*")
        .OpenBracket()
            .InnerJoin("Treatment", "TreatmentID")
        .CloseBracket()
            .InnerJoin(nameof(Patient),nameof(Treatment),"PatientID", "PatientID")
        .Where().EqualsTo("InvoicedTreatment.InvoiceID", "@id")
        .LIMIT().Statement();
    
               
        List<QueryParameter> para = [];

        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Invoice invoice = (Invoice)value;
            para.Add(new("id",invoice.InvoiceID));
            return (Patient?)db?.Retrieve(sql, para).FirstOrDefault();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
