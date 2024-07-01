using Backend.Database;
using Backend.ExtensionMethods;
using Backend.Model;
using FrontEnd.Model;
using System.Data.Common;
using FrontEnd.Events;

namespace DentistStudioApp.Model
{

    [Table(nameof(Invoice))]
    public class Invoice : AbstractModel<Invoice>
    {
        #region backing fields
        private double _amount;
        private double _deposit;
        private long _invoiceid;
        private DateTime? _doi = DateTime.Today;
        private double _discount;
        private PaymentType? _paymentType = new();
        private bool _paid;
        #endregion

        #region Properties
        [PK]
        public long InvoiceID { get => _invoiceid; set => UpdateProperty(ref value, ref _invoiceid); }
        [Field]
        public DateTime? DOI { get => _doi; set => UpdateProperty(ref value, ref _doi); }
        [Field]
        public double Discount { get => _discount; set => UpdateProperty(ref value, ref _discount); }
        [Field]
        public bool Paid { get => _paid; set => UpdateProperty(ref value, ref _paid); }
        [FK]
        public PaymentType? PaymentType { get => _paymentType; set => UpdateProperty(ref value, ref _paymentType); }
        [Field]
        public double Amount { get => _amount; set => UpdateProperty(ref value, ref _amount); }
        [Field]
        public double Deposit { get => _deposit; set => UpdateProperty(ref value, ref _deposit); }
        public double TotalDue => Amount - Deposit;
        #endregion

        #region Constructors
        public Invoice() 
        {
            _paymentType = (PaymentType?)DatabaseManager.Find<PaymentType>()?.Retrieve(_paymentType.Select().All().From().Limit().Statement()).FirstOrDefault();
            AfterUpdate += OnAfterUpdate;
            SelectQry = this.Select()
                        .All()
                        .From()
                        .OrderBy().Field("DOI DESC").Field($"Invoice.{nameof(InvoiceID)} DESC").Statement();
        }
        public Invoice(long id) : this() => _invoiceid = id;
        public Invoice(DbDataReader reader) : this()
        {
            _invoiceid = reader.GetInt64(0);
            _doi = reader.TryFetchDate(1);
            _discount = reader.GetDouble(2);
            _paymentType = new(reader.GetInt64(3));
            _paid = reader.GetBoolean(4);
            _amount = reader.TryFetchDouble(5);
            _deposit = reader.TryFetchDouble(6);
        }
        #endregion

        private void OnAfterUpdate(object? sender, AfterUpdateArgs e)
        {
            if (e.Is(nameof(Amount)) || e.Is(nameof(Deposit)) || e.Is(nameof(Discount)))
                RaisePropertyChanged(nameof(TotalDue));
        }

        public void SetAmount(double amount)
        { 
            _amount += amount;
            RaisePropertyChanged(nameof(Amount));
            RaisePropertyChanged(nameof(TotalDue));
        }

        public void RemoveAmount(double amount)
        {
            _amount -= amount;
            RaisePropertyChanged(nameof(Amount));
            RaisePropertyChanged(nameof(TotalDue));
        }

        public override string ToString() => $"{InvoiceID} - Date: {DOI} - Amount: {Amount} - Deposit {Deposit} - Paid: {Paid} - PaymentType: {PaymentType}";

    }

    [Table(nameof(Invoice))]
    public class InvoiceReport : AbstractModel<InvoiceReport>
    {
        [PK]
        public long InvoiceID { get; }

        [FK]
        public Patient? Patient { get; }

        [Field]
        public DateTime? DOI { get; }

        [Field]
        public double Amount { get; }

        [Field]
        public double Discount { get; }

        [Field]
        public double Deposit { get; }

        [Field]
        public double TotalDue { get; }

        [FK]
        public PaymentType? PaymentType { get; }

        [Field]
        public string Paid { get; } = string.Empty;

        public InvoiceReport() 
        {
            SelectQry = "SELECT Invoice.*, (Invoice.Amount - Invoice.Deposit) As TotalDue, Treatment.PatientID FROM Invoice INNER JOIN InvoicedTreatment on Invoice.InvoiceID = InvoicedTreatment.InvoiceID INNER JOIN Treatment ON InvoicedTreatment.TreatmentID = Treatment.TreatmentID;";
        }

        public InvoiceReport(DbDataReader reader) : this()
        {
            InvoiceID = reader.GetInt64(0);
            DOI = reader.TryFetchDate(1);
            Discount = reader.GetDouble(2);
            PaymentType = new(reader.GetInt64(3));
            Paid = (reader.GetBoolean(4)) ? "YES" : "NO";
            Amount = reader.TryFetchDouble(5);
            TotalDue = reader.GetDouble(6);
            Deposit = reader.TryFetchDouble(7);
            Patient = new(reader.GetInt64(8));
        }
    }
}