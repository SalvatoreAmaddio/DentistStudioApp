﻿using DentistStudioApp.Model;
using FrontEnd.Controller;
using FrontEnd.Events;
using Backend.ExtensionMethods;
using FrontEnd.Source;
using Backend.Database;
using FrontEnd.FilterSource;
using Backend.Model;
using DentistStudioApp.View;

namespace DentistStudioApp.Controller
{
    public class InvoiceListController : AbstractFormListController<Invoice>
    {
        public RecordSource<PaymentType> PaymentTypes { get; private set; } = new(DatabaseManager.Find<PaymentType>()!);
        public override int DatabaseIndex => 12;
        public SourceOption PaymentTypesOptions { get; private set; }
        public SourceOption PaidOptions { get; private set; }
        public SourceOption DatesOptions { get; private set; }

        public InvoiceListController()
        {
            AfterUpdate += OnAfterUpdate;
            PaymentTypesOptions = new(PaymentTypes, "PaymentBy");
            PaidOptions = new PrimitiveSourceOption(this, "Paid");
            DatesOptions = new PrimitiveSourceOption(this, "DOI");
            AllowNewRecord = false;
        }

        private async void OnAfterUpdate(object? sender, AfterUpdateArgs e)
        {
            if (e.Is(nameof(Search))) 
            {
                await OnSearchPropertyRequeryAsync(sender);
            }
        }

        public override void OnOptionFilterClicked(FilterEventArgs e)
        {
            ReloadSearchQry();
            PaymentTypesOptions.Conditions<WhereClause>(SearchQry);
            PaidOptions.Conditions<WhereClause>(SearchQry);
            DatesOptions.Conditions<WhereClause>(SearchQry);
            OnAfterUpdate(e, new(null, null, nameof(Search)));
        }

        public override async Task<IEnumerable<Invoice>> SearchRecordAsync()
        {
            SearchQry.AddParameter("name", Search.ToLower() + "%");
            SearchQry.AddParameter("name", Search.ToLower() + "%");
            return await CreateFromAsyncList(SearchQry.Statement(), SearchQry.Params());
        }

        protected override void Open(Invoice model)
        {
            InvoiceForm invoideForm = new(new(model));
            invoideForm.ShowDialog();
        }

        public override AbstractClause InstantiateSearchQry() =>
        new InvoicedTreatment()
             .Select("Invoice.*")
             .From()
             .InnerJoin("Treatment", "TreatmentID")
             .InnerJoin(new Invoice())
             .InnerJoin(nameof(Invoice), nameof(PaymentType), "PaymentTypeID")
             .InnerJoin(nameof(Treatment), nameof(Patient), "PatientID")
             .Where()
                 .OpenBracket()
                     .Like("LOWER(Patient.FirstName)", "@name")
                     .OR()
                     .Like("LOWER(Patient.LastName)", "@name")
                 .CloseBracket()
             .OrderBy().Field("DOI DESC").Field("Invoice.InvoiceID DESC");
    }
}