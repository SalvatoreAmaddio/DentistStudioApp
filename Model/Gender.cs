using Backend.Database;
using Backend.Model;
using FrontEnd.Model;
using System.Data.Common;

namespace DentistStudioApp.Model
{
    [Table(nameof(Gender))]
    public class Gender : AbstractModel
    {
        long _genderid;
        string _genderName = string.Empty;

        [PK]
        public long GenderID { get => _genderid; set => UpdateProperty(ref value, ref _genderid); }

        [Field]
        public string GenderName { get => _genderName; set => UpdateProperty(ref value, ref _genderName); }
        
        public Gender() { }

        public Gender(long genderid) => _genderid = genderid; 

        public Gender(DbDataReader reader) 
        {
            _genderid = reader.GetInt64(0);
            _genderName = reader.GetString(1);
        }

        public override bool AllowUpdate()
        {
            return true;
        }

        public override ISQLModel Read(DbDataReader reader) => new Gender(reader);

        public override void SetParameters(List<QueryParameter>? parameters)
        {
            parameters?.Add(new(nameof(GenderID),GenderID));
            parameters?.Add(new(nameof(GenderName), GenderName));
        }

        public override string? ToString() => GenderName;

    }
}
