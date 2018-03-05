using VKG.CodeFactory.DataAccess.DataTransferObjects;
using VKG.CodeFactory.DataAccess.Interfacess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VKG.CodeFactory.DataAccess.Managers
{
    internal class SqlDataAccessManager : DataAccessManager, IDisposable
    {
        #region protected methods
        protected override async Task<DataSet> ExecuteNativeDataSet(DataAccessRequest dataAccessRequest)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// This method represent to ExecuteDataTable and return data table result
        /// </summary>
        /// <param name="dataAccessRequest">DataAccessRequest object</param>
        /// <returns></returns>
        protected override async Task<DataTable> ExecuteNativeDataTable(DataAccessRequest dataAccessRequest)
        {
            DataTable dataTable = null;

            IDataReader dataReader = await ExecuteNativeReader(dataAccessRequest);
            if (!(null == dataReader))
            {
                DataTable schemaDataTable = dataReader.GetSchemaTable();
                dataTable = new DataTable();

                foreach (DataRow dataRow in schemaDataTable.Rows)
                {
                    DataColumn dataColumn = new DataColumn();
                    dataColumn.ColumnName = dataRow["ColumnName"].ToString();
                    dataColumn.DataType = Type.GetType(dataRow["DataType"].ToString());
                    dataColumn.ReadOnly = (bool)dataRow["IsReadOnly"];
                    dataColumn.AutoIncrement = (bool)dataRow["IsAutoIncrement"];
                    dataColumn.Unique = (bool)dataRow["IsUnique"];
                    dataTable.Columns.Add(dataColumn);
                }
                while (dataReader.Read())
                {
                    DataRow dataRow = dataTable.NewRow();
                    for (int i = 0; i < dataTable.Columns.Count - 1; i++)
                        dataRow[i] = dataReader[i];
                    dataTable.Rows.Add(dataRow);
                }
            }
            return dataTable;
        }
        /// <summary>
        /// This method represent to ExecuteReader and return result
        /// </summary>
        /// <param name="dataAccessRequest">DataAccessRequest object</param>
        /// <returns></returns>
        protected override async Task<IDataReader> ExecuteNativeReader(DataAccessRequest dataAccessRequest)
        {
            IDataReader dataReader = null;
            DbCommand dbCommand = null;
            dbCommand = await BuildCommand(dataAccessRequest);
            if (null == dataAccessRequest.Connection)
            {
                using (ConnectionManager connMgr = new ConnectionManager(dbCommand, dataAccessRequest.ConnectionString, true))
                {
                    dataReader = dbCommand.ExecuteReader();
                }
            }
            else
            {
                using (ConnectionManager connMgr = new ConnectionManager(dbCommand, dataAccessRequest.Connection, true))
                {
                    dataReader = dbCommand.ExecuteReader();
                }
            }

            return dataReader;
        }

        /// <summary>
        /// This method executes disconnected reader and returns result
        /// </summary>
        /// <param name="dataAccessRequest">DataAccessRequest object</param>
        /// <returns></returns>
        protected override async Task<IDisconnectedDataReader> ExecuteNativeDisconnectedReader(DataAccessRequest dataAccessRequest)
        {
            IDataReader dataReader = null;
            IDisconnectedDataReader disConnectedReader = null;
            DbCommand dbCommand = null;
            dbCommand = await BuildCommand(dataAccessRequest);
            if (null == dataAccessRequest.Connection)
            {
                using (ConnectionManager connMgr = new ConnectionManager(dbCommand, dataAccessRequest.ConnectionString, true))
                {
                    dataReader = dbCommand.ExecuteReader();
                    disConnectedReader = new DisconnectedSqlDataReader(dataReader);
                    dataReader.Close();
                }
            }
            else
            {
                using (ConnectionManager connMgr = new ConnectionManager(dbCommand, dataAccessRequest.Connection, true))
                {
                    dataReader = dbCommand.ExecuteReader();
                    disConnectedReader = new DisconnectedSqlDataReader(dataReader);
                    dataReader.Close();
                }
            }

            return disConnectedReader;
        }



        /// <summary>
        /// This method represent to ExecuteNonQuery and return status
        /// </summary>
        /// <param name="dataAccessRequest">DataAccessRequest object</param>
        /// <returns></returns>
        protected override async Task<int> ExecuteNativeNonQuery(DataAccessRequest dataAccessRequest)
        {
            DbCommand dbCommand = null;
            int newId = 0;
            dbCommand = await BuildCommand(dataAccessRequest);
            if (null == dataAccessRequest.Connection)
            {
                using (ConnectionManager connMgr = new ConnectionManager(dbCommand, dataAccessRequest.ConnectionString))
                {
                    newId = dbCommand.ExecuteNonQuery();
                }
            }
            else
            {
                using (ConnectionManager connMgr = new ConnectionManager(dbCommand, dataAccessRequest.Connection))
                {
                    newId = dbCommand.ExecuteNonQuery();
                }
            }
               
            return newId;
        }
        /// <summary>
        /// This method represent to ExecuteScalar and return data
        /// </summary>
        /// <param name="dataAccessRequest">DataAccessRequest object</param>
        /// <returns></returns>
        protected override async Task<object> ExecuteNativeScalar(DataAccessRequest dataAccessRequest)
        {
            DbCommand dbCommand = null;
            object result;

            dbCommand = await BuildCommand(dataAccessRequest);
            if (null == dataAccessRequest.Connection)
            {
                using (ConnectionManager connMgr = new ConnectionManager(dbCommand, dataAccessRequest.ConnectionString))
                {
                    result = dbCommand.ExecuteScalar();
                }
            }
            else
            {
                using (ConnectionManager connMgr = new ConnectionManager(dbCommand, dataAccessRequest.Connection))
                {
                    result = dbCommand.ExecuteScalar();
                }
            }
            
            return result;
        }
        #endregion

        /// <summary>
        /// This method represent to Build query Command
        /// </summary>
        /// <param name="dataAccessRequest">DataAccessRequest object</param>
        /// <returns></returns>
        private async Task<DbCommand> BuildCommand(DataAccessRequest dataAccessRequest)
        {
            SqlCommand sqlCommand = new SqlCommand() {Transaction = dataAccessRequest.Transaction, CommandType = CommandType.StoredProcedure, CommandText = dataAccessRequest.ProcedureName };

            foreach (SqlParameter parameter in BuildParameter(dataAccessRequest?.Parameters))
                sqlCommand.Parameters.Add(parameter);
            return sqlCommand;
        }

        /// <summary>
        /// This method represent to Build Parameter
        /// </summary>
        /// <param name="dataAccessParameterList">List<DataAccessParameter> object</param>
        /// <returns></returns>
        private IEnumerable<SqlParameter> BuildParameter(List<DataAccessParameter> dataAccessParameterList)
        {
            foreach (DataAccessParameter dataAccessParameter in dataAccessParameterList)
                yield return (new SqlParameter()
                {
                    SqlDbType = (SqlDbType)Enum.Parse(typeof(SqlDbType), dataAccessParameter.Type.ToString()),
                    ParameterName = dataAccessParameter.Name,
                    Size = dataAccessParameter.Size,
                    Direction = (ParameterDirection)Enum.Parse(typeof(ParameterDirection), dataAccessParameter.Direction.ToString()),
                    Value = dataAccessParameter.Value
                });
        }
    }
}
