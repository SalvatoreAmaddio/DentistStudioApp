using DentistStudioApp.Model;
using FrontEnd.Controller;
using FrontEnd.Events;

namespace DentistStudioApp.Controller
{
    public class PaymentTypeListController : AbstractFormListController<PaymentType>
    {
        public override string SearchQry { get; set; } = $"SELECT * FROM {nameof(PaymentType)} WHERE PaymentBy = @name";
        public override int DatabaseIndex => 13;

        public PaymentTypeListController() 
        {
            OpenWindowOnNew = false;
            AfterUpdate += OnAfterUpdate;
        }

        private async void OnAfterUpdate(object? sender, AfterUpdateArgs e)
        {
            if (!e.Is(nameof(Search))) return;
            var results = await Task.Run(SearchRecordAsync);
            AsRecordSource().ReplaceRange(results);
            GoFirst();
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