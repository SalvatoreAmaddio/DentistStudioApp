﻿using Backend.ExtensionMethods;
using Backend.Model;
using DentistStudioApp.Model;
using FrontEnd.Controller;
using FrontEnd.Events;

namespace DentistStudioApp.Controller
{
    public class GenderListController : AbstractFormListController<Gender>
    {
        public GenderListController()
        {
            AfterUpdate += OnAfterUpdate;
            OpenWindowOnNew = false;
        }

        private async void OnAfterUpdate(object? sender, AfterUpdateArgs e)
        {
            if (e.Is(nameof(Search))) 
            {
                await OnSearchPropertyRequeryAsync(sender);
            }
        }

        public override void OnOptionFilterClicked(FilterEventArgs e) { }
        protected override void Open(Gender model) { }

        public override async Task<IEnumerable<Gender>> SearchRecordAsync()
        {
            SearchQry.AddParameter("name", Search.ToLower() + "%");
            return await CreateFromAsyncList(SearchQry.Statement(), SearchQry.Params());
        }

        public override AbstractClause InstantiateSearchQry() =>
        new Gender()
            .Select().All()
            .From()
            .Where().Like("LOWER(Identity)", "@name");
    }
}