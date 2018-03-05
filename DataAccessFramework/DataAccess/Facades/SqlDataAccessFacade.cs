using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VKG.CodeFactory.DataAccess.DataTransferObjects;
using VKG.CodeFactory.DataAccess.Interfaces;
using VKG.CodeFactory.DataAccess.Facades;
using VKG.CodeFactory.DataAccess.Factories;
using VKG.CodeFactory.DataAccess.Interfacess;

namespace VKG.CodeFactory.DataAccess.Facades
{
    public class SqlDataAccessFacade : DataAccessFacade, IDisposable
    {
        protected override async Task<DataSet> ExecuteNativeDataSet(DataAccessRequest dataAccessRequest)
            => await NativeManagerFactory.CreateManager(DatabaseType).ExecuteDataSet(dataAccessRequest);

        protected override async Task<DataTable> ExecuteNativeDataTable(DataAccessRequest dataAccessRequest)
        => await NativeManagerFactory.CreateManager(DatabaseType).ExecuteDataTable(dataAccessRequest);

        protected override async Task<int> ExecuteNativeNonQuery(DataAccessRequest dataAccessRequest)
        => await NativeManagerFactory.CreateManager(DatabaseType).ExecuteNonQuery(dataAccessRequest);

        protected override async Task<IDataReader> ExecuteNativeReader(DataAccessRequest dataAccessRequest)
        => await NativeManagerFactory.CreateManager(DatabaseType).ExecuteReader(dataAccessRequest);
        protected override async Task<IDisconnectedDataReader> ExecuteNativeDisconnectedReader(DataAccessRequest dataAccessRequest)
      => await NativeManagerFactory.CreateManager(DatabaseType).ExecuteDisconnectedReader(dataAccessRequest);

        protected override async Task<object> ExecuteNativeScalar(DataAccessRequest dataAccessRequest)
         => await NativeManagerFactory.CreateManager(DatabaseType).ExecuteScalar(dataAccessRequest);

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
