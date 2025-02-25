﻿using Backend.Database;
using Backend.ExtensionMethods;
using Backend.Model;
using FrontEnd.Model;
using FrontEnd.Source;
using System.Data.Common;

namespace DentistStudioApp.Model
{
    [Table(nameof(Service))]
    public class Service : AbstractModel<Service>
    {
        #region backing fields
        private long _serviceid;
        private string _serviceName = string.Empty;
        private double _cost;
        #endregion

        #region Properties
        [PK]
        public long ServiceID { get => _serviceid; set => UpdateProperty(ref value, ref _serviceid); }
        [Field]
        public string ServiceName { get => _serviceName; set => UpdateProperty(ref value, ref _serviceName); }
        [Field]
        public double Cost { get => _cost; set => UpdateProperty(ref value, ref _cost); }
        #endregion

        #region Constructor
        public Service() => AfterUpdate += OnAfterUpdate;
        public Service(long id) : this() => _serviceid = id;
        public Service(DbDataReader reader) : this()
        {
            _serviceid = reader.GetInt64(0);
            _serviceName = reader.GetString(1);
            _cost = reader.GetDouble(2);
        }
        #endregion

        private void OnAfterUpdate(object? sender, FrontEnd.Events.AfterUpdateArgs e)
        {
            if (e.Is(nameof(ServiceName)))
                _serviceName = e.ConvertNewValueTo<string>().FirstLetterCapital();
        }
        public override string? ToString() => $"{ServiceName}";
    }
}