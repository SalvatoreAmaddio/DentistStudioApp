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
        #endregion

        public Invoice() 
        { 
            
        }

        public Invoice(DbDataReader reader)
        {
            _invoiceid = reader.GetInt64(0);
            _doi = reader.TryFetchDate(1);
            _discount = reader.GetDouble(2);
            _paymentType = new(reader.GetInt64(3));
            _paid = reader.GetBoolean(4);
        }

        public override ISQLModel Read(DbDataReader reader) => new Invoice(reader);
    }
}