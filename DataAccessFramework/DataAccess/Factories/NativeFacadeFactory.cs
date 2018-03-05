using VKG.CodeFactory.DataAccess.DataTransferObjects;
using VKG.CodeFactory.DataAccess.Facades;
using VKG.CodeFactory.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VKG.CodeFactory.DataAccess.Factories
{
    internal static class NativeFacadeFactory
    {
        private static DataAccessFacade facade;
      
        internal static DataAccessFacade CreateFacade(DatabaseType databaseType)
        {
            switch (databaseType)
            {
                case DatabaseType.SqlServer:
                    if(null== facade)
                        facade = new SqlDataAccessFacade();
                    else
                        if(!(facade.GetType()==typeof(SqlDataAccessFacade)))
                        facade = new SqlDataAccessFacade();
                    break;
                default:
                    facade = null;
                    break;
            }
            return facade;
        }
    }
}
