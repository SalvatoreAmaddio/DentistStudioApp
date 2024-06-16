﻿using Backend.ExtensionMethods;
using Backend.Model;
using FrontEnd.Model;
using System.Data.Common;
using System.Security.Principal;

namespace DentistStudioApp.Model
{
    [Table(nameof(JobTitle))]
    public class JobTitle : AbstractModel
    {
        long _jobtitleid;
        string _title = string.Empty;

        [PK]
        public long JobTitleID { get => _jobtitleid; set => UpdateProperty(ref value, ref _jobtitleid); }

        [Field]
        public string Title { get => _title; set => UpdateProperty(ref value, ref _title); }
        public JobTitle() => AfterUpdate += OnAfterUpdate;

        private void OnAfterUpdate(object? sender, FrontEnd.Events.AfterUpdateArgs e)
        {
            if (e.Is(nameof(Title)))
                _title = e.ConvertNewValueTo<string>().FirstLetterCapital();
        }

        public JobTitle(long jobtitleid) : this() => _jobtitleid = jobtitleid;

        public JobTitle(DbDataReader reader) : this()
        {
            _jobtitleid = reader.GetInt64(0);
            _title = reader.GetString(1);
        }

        public override bool AllowUpdate()
        {
            return true;
        }

        public override ISQLModel Read(DbDataReader reader) => new JobTitle(reader);

        public override string? ToString() => Title;

    }
}
