using DentistStudioApp.Model;
using FrontEnd.Reports;

namespace DentistStudioApp.View
{
    /// <summary>
    /// Interaction logic for Invoice.xaml
    /// </summary>
    public partial class InvoicePage : ReportPage, IClonablePage
    {
        private readonly Invoice _invoice;
        public InvoicePage(Invoice invoice)
        {
            InitializeComponent();
            _invoice = invoice;

            InvoiceID.Content = $"Invoice# {_invoice.InvoiceID}";
            DOI.Content = $"{_invoice.DOI}";

            ToBePaid.Content = (_invoice.Paid) ? "Paid by" : "To be paid by";
            PaymentMethod.Content = _invoice.PaymentType?.PaymentBy;
           
            Total.Content = _invoice.Amount;
            Deposit.Content = _invoice.Deposit;
            TotalDue.Content = _invoice.TotalDue;
            Discount.Content = _invoice.Discount;
        }

        public ReportPage CloneMe() => new InvoicePage(_invoice);
    }
}
