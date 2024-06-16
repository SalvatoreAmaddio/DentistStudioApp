using Backend.ExtensionMethods;
using Backend.Model;
using FrontEnd.Model;
using System.Data.Common;

namespace DentistStudioApp.Model
{
    [Table(nameof(PaymentType))]
    public class PaymentType : AbstractModel
    {
        #region backing fields
        long _paymentTypeID;
        string _paymentBy = string.Empty;
        #endregion

        #region properties
        [PK]
        public long PaymentTypeID { get => _paymentTypeID; set => UpdateProperty(ref value, ref _paymentTypeID); }
        [Field]
        public string PaymentBy { get => _paymentBy; set => UpdateProperty(ref value, ref _paymentBy); }
        #endregion

        public PaymentType() => AfterUpdate += OnAfterUpdate;

        public PaymentType(long id) : this() => _paymentTypeID = id;

        public PaymentType(DbDataReader reader) : this()
        {
            _paymentTypeID = reader.GetInt64(0);
            _paymentBy = reader.GetString(1);
        }

        private void OnAfterUpdate(object? sender, FrontEnd.Events.AfterUpdateArgs e)
        {
            if (e.Is(nameof(PaymentBy)))
                _paymentBy = e.ConvertNewValueTo<string>().FirstLetterCapital();
        }
        public override ISQLModel Read(DbDataReader reader) => new PaymentType(reader);

        public override string? ToString() => $"{PaymentBy}";

    }
}