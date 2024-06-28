using Backend.ExtensionMethods;
using Backend.Model;
using FrontEnd.Model;
using System.Data.Common;

namespace DentistStudioApp.Model
{
    [Table(nameof(JobTitle))]
    public class JobTitle : AbstractModel<JobTitle>
    {
        #region Backing Fields
        private long _jobtitleid;
        private string _title = string.Empty;
        #endregion

        #region Properties
        [PK]
        public long JobTitleID { get => _jobtitleid; set => UpdateProperty(ref value, ref _jobtitleid); }

        [Field]
        [Mandatory]
        public string Title { get => _title; set => UpdateProperty(ref value, ref _title); }
        #endregion

        #region Constructors
        public JobTitle() => AfterUpdate += OnAfterUpdate;
        public JobTitle(long jobtitleid) : this() => _jobtitleid = jobtitleid;
        public JobTitle(DbDataReader reader) : this()
        {
            _jobtitleid = reader.GetInt64(0);
            _title = reader.GetString(1);
        }
        #endregion

        private void OnAfterUpdate(object? sender, FrontEnd.Events.AfterUpdateArgs e)
        {
            if (e.Is(nameof(Title)))
                _title = e.ConvertNewValueTo<string>().FirstLetterCapital();
        }

        public override string? ToString() => Title;
    }
}