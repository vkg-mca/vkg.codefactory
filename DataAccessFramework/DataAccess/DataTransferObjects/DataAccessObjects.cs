using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace VKG.CodeFactory.DataAccess.DataTransferObjects
{
    /// <summary>
    /// This class represent to set and get property for DataAccessRequest
    /// </summary>
    public class DataAccessRequest
    {
        private string _connectionString;
        private SqlConnection _connection;

        /// <summary>
        /// This property is used to get and set ProcedureName
        /// </summary>
        public string ProcedureName { get; set; }

        /// <summary>
        /// This property is used to get and set Parameters
        /// </summary>
        public List<DataAccessParameter> Parameters { get; set; }
        public SqlTransaction Transaction { get; set; }
        public string ConnectionString
        {
            get { return _connectionString; }
            set
            {
                if (!(null == _connection || string.IsNullOrWhiteSpace(value)))
                    throw new ArgumentNullException("Both ConnectionString and Connection properties cannot be set, either of them should be set");
                _connectionString = value;
            }
        }

        public SqlConnection Connection
        {
            get { return _connection; }
            set
            {
                if (!(string.IsNullOrWhiteSpace(_connectionString) || null == value))
                    throw new ArgumentNullException("Both ConnectionString and Connection properties cannot be set, either of them should be set");
                _connection = value;
            }
        }
    }

    /// <summary>
    /// This class represent to set parameters for DataAccessParameter
    /// </summary>
    public class DataAccessParameter
    {
        /// <summary>
        /// DataAccessParameter Constructor
        /// </summary>
        public DataAccessParameter()
        {
            DatabaseType = DatabaseType.SqlServer;
        }

        /// <summary>
        /// Parameterize DataAccessParameter Constructor
        /// </summary>
        /// <param name="name">Name of the Parameter</param>
        /// <param name="type">Type of the Parameter</param>
        /// <param name="direction">Direction of the Parameter</param>
        public DataAccessParameter(string name, ParameterType type, ParameterDirection direction):this()
        {
            Name = name;
            Type = type;
            Direction = direction;
        }

        /// <summary>
        /// Parameterize DataAccessParameter Constructor
        /// </summary>
        /// <param name="name">Name of the Parameter</param>
        /// <param name="size">Size of Parameter</param>
        /// <param name="type">Type of the Parameter</param>
        /// <param name="direction">Direction of the Parameter</param>
        /// <param name="value">Value of the Parameter</param>
        public DataAccessParameter(string name,int size,ParameterType type,ParameterDirection direction,object value) : this()
        {
            Name = name;
            Size = size;
            Type = type;
            Direction = direction;
            Value = value;
        }

        /// <summary>
        /// Parameterize DataAccessParameter Constructor
        /// </summary>
        /// <param name="name">Name of the Parameter</param>
        /// <param name="type">Type of the Parameter</param>
        /// <param name="direction">Direction of the Parameter</param>
        /// <param name="value">Value of the Parameter</param>
        public DataAccessParameter(string name, ParameterType type, ParameterDirection direction, object value) : this()
        {
            Name = name;
            Type = type;
            Direction = direction;
            Value = value;
        }

        /// <summary>
        /// This property is used to get and set Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// This property is used to get and set Size
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// This property is used to get and set Value
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// This property is used to get and set Type
        /// </summary>
        public ParameterType Type { get; set; }

        /// <summary>
        /// This property is used to get and set DatabaseType
        /// </summary>
        public DatabaseType DatabaseType { get; set; }

        /// <summary>
        /// This property is used to get and set Direction
        /// </summary>
        public ParameterDirection Direction { get; set; }
    }

    /// <summary>
    /// This enum represent DatabaseType
    /// </summary>
    public enum DatabaseType
    {
        SqlServer,
        Oracle
    }

    /// <summary>
    /// This enum represent ParameterType
    /// </summary>
    [Serializable]
    public enum ParameterType
    {
        BigInt,
        Binary,
        Bit,
        Char,
        DateTime,
        Decimal,
        Float,
        Image,
        Int,
        Money,
        NChar,
        NText,
        NVarChar,
        Real,
        UniqueIdentifier,
        SmallDateTime,
        SmallInt,
        SmallMoney,
        Text,
        Timestamp,
        TinyInt,
        VarBinary,
        VarChar,
        Variant,
        Xml,
        Udt,
        Structured,
        Date,
        Time,
        DateTime2,
        DateTimeOffset,
        //Remaining DbType
        AnsiString,
        Byte ,
        Boolean ,
        Currency,
        Double,
        Guid ,
        Int16 ,
        Int32 ,
        Int64 ,
        Object,
        SByte ,
        Single,
        String,
        UInt16 ,
        UInt32 ,
        UInt64 ,
        VarNumeric ,
        AnsiStringFixedLength ,
        StringFixedLength ,
    }
}
