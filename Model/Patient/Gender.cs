using Backend.ExtensionMethods;
using Backend.Model;
using FrontEnd.Model;
using System.Data.Common;

namespace DentistStudioApp.Model
{
    [Table(nameof(Gender))]
    public class Gender : AbstractModel
    {
        #region Backing Fields
        private long _genderid;
        private string _identity = string.Empty;
        #endregion

        #region Properties
        [PK]
        public long GenderID { get => _genderid; set => UpdateProperty(ref value, ref _genderid); }

        [Field]
        [Mandatory]
        public string Identity { get => _identity; set => UpdateProperty(ref value, ref _identity); }
        #endregion

        #region Constructors
        public Gender() => AfterUpdate += OnAfterUpdate;
        public Gender(long genderid) : this() => _genderid = genderid; 
        public Gender(DbDataReader reader) : this()
        {
            _genderid = reader.GetInt64(0);
            _identity = reader.GetString(1);
        }
        #endregion
        private void OnAfterUpdate(object? sender, FrontEnd.Events.AfterUpdateArgs e)
        {
            if (e.Is(nameof(Identity)))
                _identity = e.ConvertNewValueTo<string>().FirstLetterCapital();
        }
        public override ISQLModel Read(DbDataReader reader) => new Gender(reader);
        public override string? ToString() => Identity;
    }
}
