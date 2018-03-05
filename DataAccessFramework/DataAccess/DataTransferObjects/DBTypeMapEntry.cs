using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VKG.CodeFactory.DataAccess.DataTransferObjects
{
    internal struct DbTypeMapEntry
    {
        internal Type Type;
        internal DbType DbType;
        internal SqlDbType SqlDbType;
        internal DbTypeMapEntry(Type type, DbType dbType, SqlDbType sqlDbType)
        {
            this.Type = type;
            this.DbType = dbType;
            this.SqlDbType = sqlDbType;
        }
    };
}
