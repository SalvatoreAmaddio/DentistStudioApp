using DentistStudioApp.Model;
using FrontEnd.Controller;
using FrontEnd.Events;
using Backend.ExtensionMethods;
using Backend.Model;

namespace DentistStudioApp.Controller
{
    public class PaymentTypeListController : AbstractFormListController<PaymentType>
    {
        public override int DatabaseIndex => 13;

        public PaymentTypeListController() 
        {
            OpenWindowOnNew = false;
            AfterUpdate += OnAfterUpdate;
        }

        private void OnAfterUpdate(object? sender, AfterUpdateArgs e)
        {
            if (!e.Is(nameof(Search))) return;
            OnSearchPropertyRequeryAsync(sender);
        }

        public override void OnOptionFilterClicked(FilterEventArgs e)
        {
        }

        public async override Task<IEnumerable<PaymentType>> SearchRecordAsync()
        {
            SearchQry.AddParameter("name", Search.ToLower() + "%");
            return await CreateFromAsyncList(SearchQry.Statement(), SearchQry.Params());
        }

        protected override void Open(PaymentType? model) { }

        public override IWhereClause InstantiateSearchQry()
        {
            return new PaymentType().Where().Like("LOWER(PaymentBy)", "@name");
        }
    }
}