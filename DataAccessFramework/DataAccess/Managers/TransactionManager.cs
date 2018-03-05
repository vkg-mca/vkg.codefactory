using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VKG.CodeFactory.DataAccess.Managers
{
    internal class TransactionManager:IDisposable
    {
        private readonly IDbConnection _connection;
        internal IDbTransaction Transaction { get; private set; }

        internal TransactionManager(IDbConnection connection)
        {
            _connection = connection;
            StartTransaction();
        }

        internal TransactionManager(IDbConnection connection,IsolationLevel isolationLevel)
        {
            StartTransaction(isolationLevel);
        }

        private void StartTransaction()
        {
            Transaction = _connection.BeginTransaction(); 
        }

        private void StartTransaction(IsolationLevel isolationLevel)
        {
            Transaction = _connection.BeginTransaction(isolationLevel);
        }

        internal void CommitTransaction()
        {
            Transaction.Commit();
        }

        internal void RollbackTransaction()
        {
            Transaction.Rollback();
        }
        public void Dispose()
        {
            CommitTransaction();
        }
    }
}
