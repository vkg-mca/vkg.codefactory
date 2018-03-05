using VKG.CodeFactory.DataAccess.DataTransferObjects;
using VKG.CodeFactory.DataAccess.Interfacess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using static System.String;

namespace VKG.CodeFactory.DataAccess.Managers
{
    /// <summary>
    /// This class represent to access database related activity
    /// </summary>
    internal abstract class DataAccessManager : IDisposable
    {
        #region private member variables
        #endregion

        #region constructors
        internal DataAccessManager()
        {

        }
        #endregion

        #region properties
        protected internal IDbConnection Connection { get; private set; }
        protected internal IDbTransaction Transaction { get; private set; }
        #endregion

        #region internal methods
        /// <summary>
        /// DataAccessManager constructor
        /// </summary>
        /// <param name="dbConnection">SqlConnection object</param>
        /// /// <param name="transaction">SqlTransaction object</param>
        internal DataAccessManager(IDbConnection dbConnection, IDbTransaction transaction)
        {
            Connection = dbConnection;
            Transaction = transaction;
        }
        /// <summary>
        /// This method represent to ExecuteDataSet and return data set result
        /// </summary>
        /// <param name="dataAccessRequest">DataAccessRequest object</param>
        /// <returns></returns>
        internal async Task<DataSet> ExecuteDataSet(DataAccessRequest dataAccessRequest) 
            => await ExecuteNativeDataSet(dataAccessRequest);
        /// <summary>
        /// This method represent to ExecuteDataTable and return data table result
        /// </summary>
        /// <param name="dataAccessRequest">DataAccessRequest object</param>
        /// <returns></returns>
        internal async Task<DataTable> ExecuteDataTable(DataAccessRequest dataAccessRequest) 
            => await ExecuteNativeDataTable(dataAccessRequest);
        /// <summary>
        /// This method represent to ExecuteReader and return result
        /// </summary>
        /// <param name="dataAccessRequest">DataAccessRequest object</param>
        /// <returns></returns>
        internal async Task<IDataReader> ExecuteReader(DataAccessRequest dataAccessRequest) 
            => await ExecuteNativeReader(dataAccessRequest);

        internal async Task<IDisconnectedDataReader> ExecuteDisconnectedReader(DataAccessRequest dataAccessRequest)
           => await ExecuteNativeDisconnectedReader(dataAccessRequest);
        /// <summary>
        /// This method represent to ExecuteScalar and return data
        /// </summary>
        /// <param name="dataAccessRequest">DataAccessRequest object</param>
        /// <returns></returns>
        internal async Task<object> ExecuteScalar(DataAccessRequest dataAccessRequest) 
            => await ExecuteNativeScalar(dataAccessRequest);
        /// <summary>
        /// This method represent to ExecuteNonQuery and return status
        /// </summary>
        /// <param name="dataAccessRequest">DataAccessRequest object</param>
        /// <returns></returns>
        internal async Task<int> ExecuteNonQuery(DataAccessRequest dataAccessRequest) 
            => await ExecuteNativeNonQuery(dataAccessRequest);
        #endregion


        #region protected methods
        protected abstract Task<DataSet> ExecuteNativeDataSet(DataAccessRequest dataAccessRequest);
        protected abstract Task<DataTable> ExecuteNativeDataTable(DataAccessRequest dataAccessRequest);
        protected abstract Task<IDataReader> ExecuteNativeReader(DataAccessRequest dataAccessRequest);
        protected abstract Task<IDisconnectedDataReader> ExecuteNativeDisconnectedReader(DataAccessRequest dataAccessRequest);
        protected abstract Task<int> ExecuteNativeNonQuery(DataAccessRequest dataAccessRequest);
        protected abstract Task<object> ExecuteNativeScalar(DataAccessRequest dataAccessRequest);
        #endregion

        /// <summary>
        /// This method represent for dispose instances
        /// </summary>
        public void Dispose()
        {

        }
    }
}
