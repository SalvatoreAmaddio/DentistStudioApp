﻿using Backend.Database;
using Backend.ExtensionMethods;
using Backend.Model;
using FrontEnd.Source;
using DentistStudioApp.Model;
using FrontEnd.Controller;
using FrontEnd.Events;
using FrontEnd.FilterSource;

namespace DentistStudioApp.Controller
{
    public class SurveyDataListController : AbstractFormListController<SurveyData>
    {
        public SourceOption CategoryOptions { get; private set; }
        public RecordSource<SurveyQuestionCategory> SurveyQuestionCategories { get; private set; } = new(DatabaseManager.Find<SurveyQuestionCategory>()!);
        public SurveyController SurveyController { get; } = new();

        public SurveyDataListController()
        {
            CategoryOptions = new(SurveyQuestionCategories, "CategoryName");
            AllowNewRecord = false;
            AllowAutoSave = true;
            AfterUpdate += OnAfterUpdate;
        }

        private async void OnAfterUpdate(object? sender, AfterUpdateArgs e)
        {
            if (!e.Is(nameof(Search))) return;
            await OnSearchPropertyRequeryAsync(sender);
        }

        public override void OnOptionFilterClicked(FilterEventArgs e)
        {
            ReloadSearchQry();
            CategoryOptions.Conditions<WhereClause>(SearchQry);
            OnAfterUpdate(e, new(null, null, nameof(Search)));
        }

        public override async Task<IEnumerable<SurveyData>> SearchRecordAsync()
        {
            SearchQry.AddParameter("id", SurveyController?.CurrentRecord?.SurveyID);
            SearchQry.AddParameter("question", Search.ToLower() + "%");
            var x = SearchQry.Statement();
            return await CreateFromAsyncList(x, SearchQry.Params());
        }

        protected override void Open(SurveyData? model) { }

        public override AbstractClause InstantiateSearchQry() =>
        new SurveyData()
            .Select().All()
            .From()
            .OpenBracket()
                .InnerJoin(nameof(SurveyQuestion), "SurveyQuestionID")
            .CloseBracket()
            .InnerJoin(nameof(SurveyQuestion), nameof(SurveyQuestionCategory), "SurveyQuestionCategoryID")
            .Where()
                .EqualsTo("SurveyID", "@id")
                    .AND()
                    .OpenBracket()
                        .Like("Question", "@question")
                    .CloseBracket();
    }
}