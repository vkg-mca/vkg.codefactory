using VKG.CodeFactory.DataAccess.Configurations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VKG.CodeFactory.DataAccess.Managers
{
    internal class ConnectionManager : IDisposable
    {
        internal IDbConnection Connection { get; private set; }
        internal ConnectionState ConnectionState { get { return Connection.State; } }
        private readonly bool _keepConnectionOpen;

        internal ConnectionManager(IDbCommand command, IDbConnection connection, bool keepConnectionOpen = true)
        {
            _keepConnectionOpen = keepConnectionOpen;
            Connection = connection;
            OpenConnection();
            SetCommand(command);
        }
        internal ConnectionManager(IDbCommand command, string connectionString, bool keepConnectionOpen = false)
        {
            _keepConnectionOpen = keepConnectionOpen;
            OpenConnection(connectionString);
            SetCommand(command);
        }
        private void SetCommand(IDbCommand command)
        {
            command.Connection = Connection;
        }
        private void OpenConnection(string connectionString = "", SqlCredential credential = null)
        {
            if (null == Connection)
            {
                Connection = new SqlConnection(string.IsNullOrWhiteSpace(connectionString) ? DataAccessConfiguration.SqlConnectionString : connectionString);
            }
            OpenConnection();
        }
        private void OpenConnection()
        {
            if (!(Connection.State == ConnectionState.Open))
                if (!(Connection.State == ConnectionState.Open))
                    while (Connection.State == ConnectionState.Connecting) { }
            if (!(Connection.State == ConnectionState.Open))
                Connection.Open();
        }
        /// <summary>
        /// This method represent to close database connection
        /// </summary>
        private void CloseConnection()
        {
            if (!(null == Connection))
            {
                if (!(Connection.State == ConnectionState.Closed))
                {
                    Connection.Close();
                }
            }
        }

        public void Dispose()
        {
            if (!_keepConnectionOpen)
                CloseConnection();
        }
    }
}
