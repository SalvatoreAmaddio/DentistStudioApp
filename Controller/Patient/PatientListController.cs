﻿using Backend.Database;
using Backend.ExtensionMethods;
using Backend.Model;
using FrontEnd.Source;
using DentistStudioApp.Model;
using DentistStudioApp.View;
using FrontEnd.Controller;
using FrontEnd.Events;
using FrontEnd.FilterSource;

namespace DentistStudioApp.Controller
{
    public class PatientListController : AbstractFormListController<Patient>
    {
        public SourceOption TitleOptions { get; private set; }
        public SourceOption GenderOptions { get; private set; }
        public RecordSource<Gender> Genders { get; private set; } = new(DatabaseManager.Find<Gender>()!);
        public RecordSource<JobTitle> Titles { get; private set; } = new(DatabaseManager.Find<JobTitle>()!);         

        public PatientListController() 
        {
            TitleOptions = new(Titles, "Title");
            GenderOptions = new(Genders, "Identity");
            AfterUpdate += OnAfterUpdate;
        }

        #region Event Subscriptions
        private async void OnAfterUpdate(object? sender, AfterUpdateArgs e)
        {
            if (!e.Is(nameof(Search))) return;
            await OnSearchPropertyRequeryAsync(sender);
        }
        #endregion

        public override void OnOptionFilterClicked(FilterEventArgs e)
        {
            ReloadSearchQry();
            GenderOptions.Conditions<WhereClause>(SearchQry);
            TitleOptions.Conditions<WhereClause>(SearchQry);
            OnAfterUpdate(e, new(null, null, nameof(Search)));
        }

        public override async Task<IEnumerable<Patient>> SearchRecordAsync()
        {
            SearchQry.AddParameter("name", Search.ToLower() + "%");
            SearchQry.AddParameter("name", Search.ToLower() + "%");
            return await CreateFromAsyncList(SearchQry.Statement(), SearchQry.Params());
        }

        protected override void Open(Patient? model)
        {
            PatientForm win = new(model);
            win.ShowDialog();
        }

        public override AbstractClause InstantiateSearchQry() =>
        new Patient()
            .Select()
            .From().InnerJoin(new Gender()).InnerJoin(new JobTitle())
            .Where()
                .OpenBracket()
                    .Like("LOWER(FirstName)", "@name")
                    .OR()
                    .Like("LOWER(LastName)", "@name")
                .CloseBracket();

    }
}