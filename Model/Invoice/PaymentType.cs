using Backend.Model;
using FrontEnd.Model;
using System.Data.Common;

namespace DentistStudioApp.Model
{
    public class PaymentType : AbstractModel
    {
        #region backing fields
        long _paymentTypeID;
        string _paymentBy = string.Empty;
        #endregion

        #region properties
        public long PaymentTypeID { get => _paymentTypeID; set => UpdateProperty(ref value, ref _paymentTypeID); }
        public string PaymentBy { get => _paymentBy; set => UpdateProperty(ref value, ref _paymentBy); }
        #endregion

        public PaymentType() { }

        public PaymentType(long id) => _paymentTypeID = id;

        public PaymentType(DbDataReader reader) 
        {
            _paymentTypeID = reader.GetInt64(0);
            _paymentBy = reader.GetString(1);
        }

        public override ISQLModel Read(DbDataReader reader) => new PaymentType(reader);

        public override string? ToString() => $"{PaymentBy}";

    }
}