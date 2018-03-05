using VKG.CodeFactory.DataAccess.DataTransferObjects;
using VKG.CodeFactory.DataAccess.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VKG.CodeFactory.DataAccess.Factories
{
    internal static class NativeManagerFactory
    {
        private static DataAccessManager manager;

        internal static DataAccessManager CreateManager(DatabaseType databaseType)
        {
            switch (databaseType)
            {
                case DatabaseType.SqlServer:
                    if (null == manager)
                        manager = new SqlDataAccessManager();
                    else
                        if (!(manager.GetType() == typeof(SqlDataAccessManager)))
                        manager = new SqlDataAccessManager();
                    break;
                default:
                    manager = null;
                    break;
            }
            return manager;
        }
    }
}
