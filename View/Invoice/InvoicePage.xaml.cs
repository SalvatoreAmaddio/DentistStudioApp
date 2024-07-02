using DentistStudioApp.Model;
using FrontEnd.Reports;
using System.Windows.Controls;
using System.Windows;
using Backend.Utils;
using System.Windows.Media;
using DentistStudioApp.Converters;

namespace DentistStudioApp.View
{
    /// <summary>
    /// Interaction logic for Invoice.xaml
    /// </summary>
    public partial class InvoicePage : ReportPage, IClonablePage
    {
        private readonly Invoice _invoice;
        private IEnumerable<AppointmentServices> _services;
        private Patient _patient;
        public InvoicePage(Invoice invoice, Patient patient, IEnumerable<AppointmentServices> services)
        {
            InitializeComponent();
            _invoice = invoice;
            _services = services;
            _patient = patient;

            InvoiceID.Content = $"Invoice# {_invoice.InvoiceID}";
    
            if (_invoice.DOI != null)
                DOI.Content = $"{_invoice.DOI.Value.ToString("dd/MM/yyyy")}";

            PatientName.Content = $"{_patient.Title} {_patient.FirstName} {_patient.LastName}".Trim();
            PatientEmail.Content = $"Email: {_patient.Email}";
            PatientPhone.Content = $"Phone: {_patient.PhoneNumber}";

            AddTreatments();

            Total.Content = _invoice.Amount.ToString("c2");
            Deposit.Content = _invoice.Deposit.ToString("c2");
            TotalDue.Content = _invoice.TotalDue.ToString("c2");

            if (_invoice.Discount == 0)
                DiscountRow.Height = new(0);
            else Discount.Content = _invoice.DiscountAmount.ToString("c2");

            ToBePaid.Content = (_invoice.Paid) ? "Paid by" : "To be paid by";
            PaymentMethod.Content = _invoice.PaymentType?.PaymentBy;

            PaymentMethod.Content = new FetchPaymentMethod().Convert(_invoice);
        }
        
        private void AddTreatments() 
        {
            int row = 1;
            foreach (AppointmentServices appointment in _services)
            {
                Items.RowDefinitions.Add(new() { Height = new(30) });

                Label treatmentLabel = new() { Content = appointment.Service.ServiceName, HorizontalAlignment = HorizontalAlignment.Stretch };
                
                if (Sys.IsEven(row)) 
                {
                    treatmentLabel.Background = (SolidColorBrush)FindResource("DefaultGrey");
                }

                Label doaLabel = new() { Content = appointment.DOA.Value.ToString("dd/MM/yyyy"), HorizontalAlignment = HorizontalAlignment.Stretch, HorizontalContentAlignment = HorizontalAlignment.Center };
                Label costLabel = new() { Content = appointment.Service.Cost.ToString("c2"), HorizontalAlignment = HorizontalAlignment.Stretch, HorizontalContentAlignment = HorizontalAlignment.Right };

                Items.Children.Add(treatmentLabel);
                Items.Children.Add(doaLabel);
                Items.Children.Add(costLabel);

                Grid.SetRow(treatmentLabel, row);
                Grid.SetRow(doaLabel, row);
                Grid.SetRow(costLabel, row);

                Grid.SetColumnSpan(treatmentLabel, 3);
                Grid.SetColumnSpan(doaLabel, 3);
                Grid.SetColumnSpan(costLabel, 3);
                row++;
            }
        }
        public ReportPage CloneMe() => new InvoicePage(_invoice, _patient, _services);
    }
}