using Backend.Model;
using FrontEnd.Model;
using System.Data.Common;

namespace DentistStudioApp.Model
{
    [Table(nameof(InvoicedTreatment))]
    public class InvoicedTreatment : AbstractModel
    {
        #region backing fields
        private long _invoiceTreatmentId;
        private Invoice? _invoice;
        private Treatment? _treatment;
        #endregion

        #region Properties
        [PK]
        public long InvoiceTreatmentID { get => _invoiceTreatmentId; set => UpdateProperty(ref value, ref _invoiceTreatmentId); }

        [FK]
        public Invoice? Invoice { get => _invoice; set => UpdateProperty(ref value, ref _invoice); }

        [FK]
        public Treatment? Treatment { get => _treatment; set => UpdateProperty(ref value, ref _treatment); }

        #endregion

        public InvoicedTreatment() 
        { 
        
        }


        public InvoicedTreatment(DbDataReader reader)
        {
            _invoiceTreatmentId = reader.GetInt64(0);
            _invoice = new Invoice(reader.GetInt64(1));
            _treatment = new Treatment(reader.GetInt64(2));
        }

        public override ISQLModel Read(DbDataReader reader) => new InvoicedTreatment(reader);

    }
}