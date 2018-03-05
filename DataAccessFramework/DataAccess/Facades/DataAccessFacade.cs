using VKG.CodeFactory.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VKG.CodeFactory.DataAccess.DataTransferObjects;
using System.Data;
using VKG.CodeFactory.DataAccess.Factories;
using VKG.CodeFactory.DataAccess.Interfacess;

namespace VKG.CodeFactory.DataAccess.Facades
{
    public class DataAccessFacade : IDataAccessFacade
    {
        protected DatabaseType DatabaseType { get; private set; }
        public DataAccessFacade() { }
        
        public DataAccessFacade(DatabaseType databaseType)
        {
            DatabaseType = databaseType;
        }

        #region internal methods
        public async Task<DataSet> ExecuteDataSet(DataAccessRequest dataAccessRequest)
            => await NativeFacadeFactory.CreateFacade(DatabaseType).ExecuteNativeDataSet(dataAccessRequest);
        public async Task<DataTable> ExecuteDataTable(DataAccessRequest dataAccessRequest) 
            => await NativeFacadeFactory.CreateFacade(DatabaseType).ExecuteNativeDataTable(dataAccessRequest);
        public async Task<IDataReader> ExecuteReader(DataAccessRequest dataAccessRequest)
            => await NativeFacadeFactory.CreateFacade(DatabaseType).ExecuteNativeReader(dataAccessRequest);
        public async Task<object> ExecuteScalar(DataAccessRequest dataAccessRequest)
            => await NativeFacadeFactory.CreateFacade(DatabaseType).ExecuteNativeScalar(dataAccessRequest);
        public async Task<int> ExecuteNonQuery(DataAccessRequest dataAccessRequest)
            => await NativeFacadeFactory.CreateFacade(DatabaseType).ExecuteNativeNonQuery(dataAccessRequest);
        public async Task<IDisconnectedDataReader> ExecuteDisconnectedReader(DataAccessRequest dataAccessRequest)
            => await NativeFacadeFactory.CreateFacade(DatabaseType).ExecuteNativeDisconnectedReader(dataAccessRequest);
        #endregion

        #region protected methods
        protected virtual async Task<DataSet> ExecuteNativeDataSet(DataAccessRequest dataAccessRequest) { throw new NotImplementedException(); }
        protected virtual async Task<DataTable> ExecuteNativeDataTable(DataAccessRequest dataAccessRequest) { throw new NotImplementedException(); }
        protected virtual async Task<IDataReader> ExecuteNativeReader(DataAccessRequest dataAccessRequest) { throw new NotImplementedException(); }
        protected virtual async Task<IDisconnectedDataReader> ExecuteNativeDisconnectedReader(DataAccessRequest dataAccessRequest) { throw new NotImplementedException(); }
        protected virtual async Task<int> ExecuteNativeNonQuery(DataAccessRequest dataAccessRequest) { throw new NotImplementedException(); }
        protected virtual async Task<object> ExecuteNativeScalar(DataAccessRequest dataAccessRequest) { throw new NotImplementedException(); }

        public void Dispose()
        {
           
        }
        #endregion
    }
}
