using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VKG.CodeFactory.DataAccess.DataTransferObjects
{
    public sealed class DataAccessDataType
    {
        private static HashSet<DbTypeMapEntry> _dbTypeSet = new HashSet<DbTypeMapEntry>();

        #region Constructors

        static DataAccessDataType()
        {
            _dbTypeSet.Add(new DbTypeMapEntry(typeof(bool), DbType.Boolean, SqlDbType.Bit));
            _dbTypeSet.Add(new DbTypeMapEntry(typeof(byte), DbType.Double, SqlDbType.TinyInt));
            _dbTypeSet.Add(new DbTypeMapEntry(typeof(byte[]), DbType.Binary, SqlDbType.Image));
            _dbTypeSet.Add(new DbTypeMapEntry(typeof(DateTime), DbType.DateTime, SqlDbType.DateTime));
            _dbTypeSet.Add(new DbTypeMapEntry(typeof(Decimal), DbType.Decimal, SqlDbType.Decimal));
            _dbTypeSet.Add(new DbTypeMapEntry(typeof(double), DbType.Double, SqlDbType.Float));
            _dbTypeSet.Add(new DbTypeMapEntry(typeof(Guid), DbType.Guid, SqlDbType.UniqueIdentifier));
            _dbTypeSet.Add(new DbTypeMapEntry(typeof(Int16), DbType.Int16, SqlDbType.SmallInt));
            _dbTypeSet.Add(new DbTypeMapEntry(typeof(Int32), DbType.Int32, SqlDbType.Int));
            _dbTypeSet.Add(new DbTypeMapEntry(typeof(Int64), DbType.Int64, SqlDbType.BigInt));
            _dbTypeSet.Add(new DbTypeMapEntry(typeof(object), DbType.Object, SqlDbType.Variant));
            _dbTypeSet.Add(new DbTypeMapEntry(typeof(string), DbType.String, SqlDbType.VarChar));
        }
        private DataAccessDataType() { }
        #endregion

        #region Methods

        /// <summary>
        /// Convert db type to .Net data type
        /// </summary>
        /// <param name="dbType"></param>
        /// <returns></returns>
        public static Type ToNetType(DbType dbType) => Find(dbType).Type;
        /// <summary>
        /// Convert TSQL type to .Net data type
        /// </summary>
        /// <param name="sqlDbType"></param>
        /// <returns></returns>
        public static Type ToNetType(SqlDbType sqlDbType) => Find(sqlDbType).Type;
        /// <summary>
        /// Convert .Net type to Db type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static DbType ToDbType(Type type) => Find(type).DbType;
        /// <summary>
        /// Convert TSQL data type to DbType
        /// </summary>
        /// <param name="sqlDbType"></param>
        /// <returns></returns>
        public static DbType ToDbType(SqlDbType sqlDbType) => Find(sqlDbType).DbType;
        /// <summary>
        /// Convert .Net type to TSQL data type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static SqlDbType ToSqlDbType(Type type) => Find(type).SqlDbType;
        /// <summary>
        /// Convert DbType type to TSQL data type
        /// </summary>
        /// <param name="dbType"></param>
        /// <returns></returns>
        public static SqlDbType ToSqlDbType(DbType dbType) => Find(dbType).SqlDbType;

        private static DbTypeMapEntry Find(Type type) => _dbTypeSet.Where(x => x.Type == (Nullable.GetUnderlyingType(type) ?? type)).FirstOrDefault();
        private static DbTypeMapEntry Find(DbType dbType) => _dbTypeSet.Where(x => x.DbType == dbType).FirstOrDefault();
        private static DbTypeMapEntry Find(SqlDbType sqlDbType) => _dbTypeSet.Where(x => x.SqlDbType == sqlDbType).FirstOrDefault();
        #endregion
    }
}
