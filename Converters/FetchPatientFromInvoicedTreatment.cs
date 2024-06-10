using Backend.Database;
using Backend.ExtensionMethods;
using DentistStudioApp.Model;
using FrontEnd.Model;
using System.Globalization;
using System.Windows.Data;

namespace DentistStudioApp.Converters
{
    public abstract class AbstractFetchModel<M,D> : IValueConverter where M : AbstractModel, new() where D : AbstractModel, new()
    {
        protected M? Record; 
        protected IAbstractDatabase? Db => DatabaseManager.Find<D>();
        protected abstract string Sql { get; }
        protected readonly List<QueryParameter> para = [];
        public abstract object? Convert(object value, Type targetType, object parameter, CultureInfo culture);
        public virtual object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Record;

    }

    public class FetchPatientFromInvoicedTreatment : AbstractFetchModel<Invoice, Patient>
    {
        protected override string Sql => new InvoicedTreatment()
        .Select("Patient.*")
        .From()
        .InnerJoin(nameof(Treatment), "TreatmentID")
        .InnerJoin(nameof(Treatment), nameof(Patient),"PatientID")
        .Where().EqualsTo("InvoicedTreatment.InvoiceID", "@id")
        .Limit().Statement();

        public override object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Record = (Invoice)value;
            para.Add(new("id", Record.InvoiceID));
            return (Patient?)Db?.Retrieve(Sql, para).FirstOrDefault();
        }

    }

    public class FetchPatientFromAppointmentTreatment : AbstractFetchModel<Appointment, Patient>
    {
        protected override string Sql => new Patient()
        .Select("Patient.*")
        .From()
        .InnerJoin(nameof(Treatment), "PatientID")
        .Where().EqualsTo("Treatment.TreatmentID", "@id")
        .Limit().Statement();

        public override object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Record = (Appointment?)value;
            para.Add(new("id", Record?.Treatment?.TreatmentID));
            return (Patient?)Db?.Retrieve(Sql, para).FirstOrDefault();
        }
    }

}
