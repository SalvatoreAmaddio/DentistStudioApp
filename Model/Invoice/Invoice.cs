using Backend.ExtensionMethods;
using Backend.Model;
using FrontEnd.Model;
using System.Data.Common;

namespace DentistStudioApp.Model
{

    [Table(nameof(Invoice))]
    public class Invoice : AbstractModel
    {
        #region backing fields
        private double _amount;
        private double _deposit;
        private long _invoiceid;
        private DateTime? _doi = DateTime.Today;
        private double _discount;
        private PaymentType? _paymentType = new(1);
        private bool _paid;
        #endregion

        #region properties
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

        public Invoice() 
        {
            //AfterUpdate += OnAfterUpdate;
        }

        private void OnAfterUpdate(object? sender, FrontEnd.Events.AfterUpdateArgs e)
        {
            if (e.Is(nameof(Amount)) || e.Is(nameof(Deposit)) || e.Is(nameof(Discount))) 
            {
                RaisePropertyChanged(nameof(TotalDue));
            }
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

        public override ISQLModel Read(DbDataReader reader) => new Invoice(reader);

        public override string ToString() => $"{InvoiceID}";

    }
}