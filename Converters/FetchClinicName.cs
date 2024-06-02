using Backend.Database;
using DentistStudioApp.Model;
using System.Globalization;
using System.Windows.Data;

namespace DentistStudioApp.Converters
{
    public class FetchClinicName : IValueConverter
    {
        private IEnumerable<Clinic>? Clinics => DatabaseManager.Find<Clinic>()?.MasterSource.Cast<Clinic>();
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Clinics?.FirstOrDefault(s=>s.Equals(value));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
