using Backend.Model;
using FrontEnd.Model;
using System.Data.Common;

namespace DentistStudioApp.Model
{
    [Table(nameof(InvoicedTreatment))]
    public class InvoicedTreatment : AbstractModel<InvoicedTreatment>
    {
        #region backing fields
        private long _invoicedTreatmentId;
        private Invoice? _invoice;
        private Treatment? _treatment;
        #endregion

        #region Properties
        [PK]
        public long InvoicedTreatmentID { get => _invoicedTreatmentId; set => UpdateProperty(ref value, ref _invoicedTreatmentId); }

        [FK]
        public Invoice? Invoice { get => _invoice; set => UpdateProperty(ref value, ref _invoice); }

        [FK]
        public Treatment? Treatment { get => _treatment; set => UpdateProperty(ref value, ref _treatment); }

        #endregion

        #region Constructors
        public InvoicedTreatment() { }

        public InvoicedTreatment(Invoice? invoice, Treatment? treatment)
        {
            this._invoice = invoice;
            this._treatment = treatment;
        }

        public InvoicedTreatment(DbDataReader reader)
        {
            _invoicedTreatmentId = reader.GetInt64(0);
            _invoice = new Invoice(reader.GetInt64(1));
            _treatment = new Treatment(reader.GetInt64(2));
        }
        #endregion
        public override string ToString() => $"{InvoicedTreatmentID} - {Invoice} - {Treatment}";

    }
}