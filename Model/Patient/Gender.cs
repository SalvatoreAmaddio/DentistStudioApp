using Backend.Database;
using Backend.ExtensionMethods;
using Backend.Model;
using FrontEnd.Model;
using System.Data.Common;

namespace DentistStudioApp.Model
{
    [Table(nameof(Gender))]
    public class Gender : AbstractModel
    {
        long _genderid;
        string _identity = string.Empty;

        [PK]
        public long GenderID { get => _genderid; set => UpdateProperty(ref value, ref _genderid); }

        [Field]
        public string Identity { get => _identity; set => UpdateProperty(ref value, ref _identity); }

        public Gender() => AfterUpdate += OnAfterUpdate;

        private void OnAfterUpdate(object? sender, FrontEnd.Events.AfterUpdateArgs e)
        {
            if (e.Is(nameof(Identity)))
                _identity = e.ConvertNewValueTo<string>().FirstLetterCapital();
        }

        public Gender(long genderid) : this() => _genderid = genderid; 

        public Gender(DbDataReader reader) : this()
        {
            _genderid = reader.GetInt64(0);
            _identity = reader.GetString(1);
        }

        public override bool AllowUpdate()
        {
            return true;
        }

        public override ISQLModel Read(DbDataReader reader) => new Gender(reader);

        public override void SetParameters(List<QueryParameter>? parameters)
        {
            parameters?.Add(new(nameof(GenderID),GenderID));
            parameters?.Add(new(nameof(Identity), Identity));
        }

        public override string? ToString() => Identity;

    }
}
