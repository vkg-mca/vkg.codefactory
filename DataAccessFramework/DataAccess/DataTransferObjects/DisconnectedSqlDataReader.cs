using System;
using System.Data;
using System.Collections;
using System.Diagnostics;
using VKG.CodeFactory.DataAccess.Interfacess;
using static System.String;

namespace VKG.CodeFactory.DataAccess.DataTransferObjects
{
	/// <summary>
	/// This class flushes an IDataReader into it's own data strucure
	/// to remove the need for an active connection throughout it's
	/// lifetime. Also includes collection of Output parameters which
	/// are not associated with SqlDataReader
	/// </summary>
	public class DisconnectedSqlDataReader : IDisconnectedDataReader
	{
		// List of result sets
        private ArrayList resultSetList = new ArrayList();

        // List of column name lists for result sets
        private ArrayList resultSetColumnNameList = new ArrayList();

        // List of column name hashtables for result sets
        private ArrayList resultSetColumnNameHashList = new ArrayList();

		// Index into result set list
        private Int32 resultSetIndex = 0;

		// Index into list of rowsets, -1 since an initial
		// read must be done to reproduce SqlDataReader behaviour
        private Int32 listIndex = -1;

		// Hashtable of output parameters
        private IDictionary outputParams = null;


		#region Constructors & Destructors
		/// <summary>
		/// Constructor which takes an IDataReader
		/// </summary>
		/// <param name="connectedReader"></param>
        public DisconnectedSqlDataReader(IDataReader connectedReader)
        {
			//Assert PreConditions
			Debug.Assert( null != connectedReader, "Data Reader cannot be null");

            do
            {
                // Create a list for this result set
                ArrayList newList = new ArrayList();

                // Get the field count for each result set once
                Int32 fldCount = connectedReader.FieldCount;

                // fill the list with an object array per row
                while(connectedReader.Read())
                {
                    Object [] newOA = new Object [fldCount];

                    connectedReader.GetValues(newOA);

                    newList.Add(newOA);
                }

                resultSetList.Add(newList);

                /*======================================================================
                 * 
                 * This is only done once per result set, to collect column name info
                 * 
                 * ===================================================================*/

                // Create a column name array for this result set
                String[] newColumnNameArr = new String[fldCount];
                
                // And a hashtable to hold the indexes
                Hashtable newColumnNameHash = new Hashtable(fldCount);

                for (Int32 i=0; i<fldCount;i++)
                {
                    // Store the ordinal of this column against the column name
                    // this allows for easy retrieval by name
                    String colName = connectedReader.GetName(i);
                    newColumnNameArr[i] = colName;
                    newColumnNameHash.Add(colName,i);
                }      
                   
                // Add the column name structures to the result set
                resultSetColumnNameList.Add(newColumnNameArr); 
                resultSetColumnNameHashList.Add(newColumnNameHash);

            }
            while(connectedReader.NextResult());

        }

		#endregion


		/// <summary>
		/// This internal method allows Output parameter collection
		/// to be added to the object after creation
		/// </summary>
		/// <param name="outParams"></param>
        internal void SetOutputParameters(IDictionary outParams)=> outputParams = outParams;
        

        #region IDisconnectedDataReader Members

		/// <summary>
		/// This overload method of GetValues passes back one of it's
		/// objects rather than the IDataReader method which needs to 
		/// be passed an object array to fill
		/// </summary>
		/// <returns></returns>
        public Object [] GetValues()=> (Object[])((IList)resultSetList[resultSetIndex])[listIndex];

        /// <summary>
        /// Number of rows in this result set
        /// Not available with IDataReader because all rows may not yet
        /// have been read.
        /// Read-Only
        /// </summary>
        public Int32 RowCount => ((IList)resultSetList[resultSetIndex]).Count;

        /// <summary>
        /// Determines whether or not a particular column exists?
        /// </summary>
        public Boolean ColumnExists( String columnName )
		{
			Boolean result = false;
			//Get ColumnList Hashtable which for some strange reason is the first part of another
			//Hashtable???
			Hashtable columns = (Hashtable)resultSetColumnNameHashList[0];

			result = columns.Contains( columnName );

			return result;
		}

        /// <summary>
        /// This is the stored procedure RETURN value
        /// </summary>
        public Int32 ResponseCode=> (Int32)outputParams["ResponseCode"];

        /// <summary>
        /// This is the output parameter collection
        /// </summary>
        public IDictionary OutputParams=> outputParams;

        /// <summary>
        /// Not implemented for this reader since SQL exceptions will be 
        /// raised as general DB exceptions
        /// </summary>
        public String ErrorCode => Empty ;

        /// <summary>
        /// This IDisconnectedDataReader method is not used for SQL data reader
        /// </summary>
        public String ErrorSeverity => Empty;

        /// <summary>
        /// This IDisconnectedDataReader method is not used for SQL data reader
        /// </summary>
        public String ErrorDescription => Empty;

        #endregion

        #region IDataReader Members

        /// <summary>
        /// The number of rows changed, inserted, or deleted; 0 if no rows were affected 
        /// or the statement failed; and -1 for SELECT statements.
        /// </summary>
        public Int32 RecordsAffected => 0;


        /// <summary>
        /// This reader will always be closed since it is disconnected
        /// </summary>
        public Boolean IsClosed => true;

        /// <summary>
        /// This method moves to the next result set, returning false if there are no more
        /// sets
        /// </summary>
        /// <returns></returns>
        public Boolean NextResult()
        {
            bool nextResult = false;

            if (resultSetIndex == resultSetList.Count-1)
                nextResult= false;
            else
            {
                resultSetIndex++;
                listIndex = -1;
                nextResult= true;
            }
            return nextResult;
        }

        /// <summary>
        /// This method does nothing since the reader is already closed
        /// </summary>
        public void Close()
        {
            return;
        }

        /// <summary>
        /// This method moves to the next row, returning false if there are no more rows
        /// </summary>
        /// <returns></returns>
        public Boolean Read()
        {
            bool readStatus = false;
            if (listIndex == ((IList)resultSetList[resultSetIndex]).Count-1)
                readStatus= false;
            else
            {
                listIndex++;
                readStatus= true;
            }
            return readStatus;
        }

        /// <summary>
        /// The .NET Framework Data Provider for SQL Server does not support nesting and always returns zero.
        /// </summary>
        public Int32 Depth => 0;

        /// <summary>
        /// This method is not implemented
        /// </summary>
        /// <returns></returns>
        public DataTable GetSchemaTable()
        {
            throw new NotImplementedException("DisconnectedSqlDataReader does not implement GetSchemaTable() method");
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            // TODO:  Add DisconnectedSqlDataReader.Dispose implementation
        }

        #endregion

        #region IDataRecord Members

        /// <summary>
        /// Return the value at ordinal specified, cast to an requested type
        /// </summary>
        /// <param name="i">position of value in row</param>
        /// <returns></returns>
        public Int32 GetInt32(int i)=> (Int32)this.GetValue(i);

        /// <summary>
        /// This indexer allows the row data to be accessed by name, slower than ordinal access
        /// </summary>
        public Object this[string name]=> this.GetValue(this.GetOrdinal(name));

        /// <summary>
        /// This indexer is equivalent to GetValue(int i)
        /// </summary>
        public Object this[int i]=> this.GetValue(i);

        /// <summary>
        /// Get the object in the current row at position i
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public Object GetValue(int i)=> ((Object[])((IList)resultSetList[resultSetIndex])[listIndex])[i];

        /// <summary>
        /// Returns true if the object at position i in the current row is a null value
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public Boolean IsDBNull(int i)
        {
            return(this.GetValue(i)==DBNull.Value);
        }

        /// <summary>
        /// GetBytes method should return the number of available bytes in the field specified
        /// Not implemented for DisconnectedSqlDataReader
        /// </summary>
        /// <param name="i"></param>
        /// <param name="fieldOffset"></param>
        /// <param name="buffer"></param>
        /// <param name="bufferoffset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException("DisconnectedSqlDataReader does not implement GetBytes() method");
        }

        /// <summary>
        /// Return the value at ordinal specified, cast to an requested type
        /// </summary>
        /// <param name="i">position of value in row</param>
        /// <returns></returns>
        public Byte GetByte(int i)=> (Byte)this.GetValue(i);

        /// <summary>
        /// this method should return the Type that is the data type of the object at the
        /// specified position.
        /// NOT IMPLEMENTED for DisconnectedSqlDataReader since we do not capture the type 
        /// information when the reader is flushed
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public Type GetFieldType(int i)
        {
            throw new NotImplementedException("DisconnectedSqlDataReader does not implement GetFieldType() method");
        }

        /// <summary>
        /// Return the value at ordinal specified, cast to an requested type
        /// </summary>
        /// <param name="i">position of value in row</param>
        /// <returns></returns>
        public Decimal GetDecimal(int i) => (Decimal)this.GetValue(i);

        /// <summary>
        /// Gets all attribute columns in the collection for the current row.
        /// </summary>
        /// <param name="values">An array of Object into which to copy the attribute columns.</param>
        /// <returns>The number of instances of Object in the array.</returns>
        public Int32 GetValues(object[] values)
        {
            for(Int32 i=0;i<this.FieldCount-1;i++)
            {
                values[i] = this.GetValue(i);
            }

            return this.FieldCount;

        }

        /// <summary>
        /// Gets the name of the specified column.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public String GetName(int i)=> ((String[])resultSetColumnNameList[resultSetIndex])[i];

        /// <summary>
        /// Gets the number of columns in the current row.
        /// </summary>
        public Int32 FieldCount
        {
            get
            {
                Int32 retVal = -1;

                // When not positioned in a valid recordset, 0; 
                // otherwise the number of columns in the current row. The default is -1.
                if (listIndex<0)
                {
                    retVal = 0;
                }
                else
                {
                    retVal = ((Object[])((IList)resultSetList[resultSetIndex])[listIndex]).Length;
                }

                return retVal;
            }
        }

        /// <summary>
        /// Return the value at ordinal specified, cast to an requested type
        /// </summary>
        /// <param name="i">position of value in row</param>
        /// <returns></returns>
        public Int64 GetInt64(int i)=> (Int64)this.GetValue(i);

        /// <summary>
        /// Return the value at ordinal specified, cast to an requested type
        /// </summary>
        /// <param name="i">position of value in row</param>
        /// <returns></returns>
        public Double GetDouble(int i) => (Double)this.GetValue(i);

        /// <summary>
        /// Return the value at ordinal specified, cast to an requested type
        /// </summary>
        /// <param name="i">position of value in row</param>
        /// <returns></returns>
        public Boolean GetBoolean(int i)=> (Boolean)this.GetValue(i);

        /// <summary>
        /// Return the value at ordinal specified, cast to an requested type
        /// </summary>
        /// <param name="i">position of value in row</param>
        /// <returns></returns>
        public Guid GetGuid(int i)=> (Guid)this.GetValue(i);

        /// <summary>
        /// Return the value at ordinal specified, cast to an requested type
        /// </summary>
        /// <param name="i">position of value in row</param>
        /// <returns></returns>
        public DateTime GetDateTime(int i) => (DateTime)this.GetValue(i);

        /// <summary>
        /// This method gets the ordinal position in the row for the column name specified 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Int32 GetOrdinal(string name)=> (Int32)((IDictionary)resultSetColumnNameHashList[resultSetIndex])[name];

        /// <summary>
        /// Gets the name of the source data type.
        /// NOT IMPLEMENTED for DisconnectedSqlDataReader
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public String GetDataTypeName(int i)
        {
            throw new NotImplementedException("DisconnectedSqlDataReader does not implement GetDataTypeName() method");
        }

        /// <summary>
        /// Return the value at ordinal specified, cast to an requested type
        /// </summary>
        /// <param name="i">position of value in row</param>
        /// <returns></returns>
        public Single GetFloat(int i)=> (Single)this.GetValue(i);

        /// <summary>
        /// Interface spec says 
        /// "Gets an IDataReader to be used when the field points to more remote structured data."
        /// but not sure what exactly this means and SqlDataReader doesn't implement this so we're not going to
        /// NOT IMPLEMENTED by DisconnectedSqlDataReader 
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public IDataReader GetData(int i)
        {
            throw new NotImplementedException("DisconnectedSqlDataReader does not implement GetData() method");
        }

        /// <summary>
        /// Reads a stream of characters from the specified column offset into the buffer as an array, starting at the given buffer offset.
        /// NOT IMPLEMENTED by DisconnectedSqlDataReader 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="fieldoffset"></param>
        /// <param name="buffer"></param>
        /// <param name="bufferoffset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException("DisconnectedSqlDataReader does not implement GetChars() method");
        
        }

        /// <summary>
        /// Return the value at ordinal specified, cast to an requested type
        /// </summary>
        /// <param name="i">position of value in row</param>
        /// <returns></returns>
        public String GetString(int i)=> (String)this.GetValue(i);

        /// <summary>
        /// Return the value at ordinal specified, cast to an requested type
        /// </summary>
        /// <param name="i">position of value in row</param>
        /// <returns></returns>
        public Char GetChar(int i)=> (Char)this.GetValue(i);

        /// <summary>
        /// Return the value at ordinal specified, cast to an requested type
        /// </summary>
        /// <param name="i">position of value in row</param>
        /// <returns></returns>
        public Int16 GetInt16(int i)=> (Int16)this.GetValue(i);

        #endregion
    }
}
