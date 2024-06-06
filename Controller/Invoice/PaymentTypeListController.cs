using DentistStudioApp.Model;
using FrontEnd.Controller;
using FrontEnd.Events;
using Backend.ExtensionMethods;

namespace DentistStudioApp.Controller
{
    public class PaymentTypeListController : AbstractFormListController<PaymentType>
    {
        public override string SearchQry { get; set; } = new PaymentType().Where().Like("LOWER(PaymentBy)", "@name").Statement();
        public override int DatabaseIndex => 13;

        public PaymentTypeListController() 
        {
            OpenWindowOnNew = false;
            AfterUpdate += OnAfterUpdate;
        }

        private void OnAfterUpdate(object? sender, AfterUpdateArgs e)
        {
            if (!e.Is(nameof(Search))) return;
            OnSearchPropertyRequery(sender);
        }

        public override void OnOptionFilter(FilterEventArgs e)
        {
        }

        public async override Task<IEnumerable<PaymentType>> SearchRecordAsync()
        {
            QueryBuiler.AddParameter("name", Search.ToLower() + "%");
            return await CreateFromAsyncList(QueryBuiler.Query, QueryBuiler.Params);
        }

        protected override void Open(PaymentType? model) { }

    }
}