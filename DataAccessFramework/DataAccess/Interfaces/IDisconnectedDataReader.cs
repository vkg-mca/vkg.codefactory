using System;
using System.Data;
using System.Collections;

namespace VKG.CodeFactory.DataAccess.Interfacess
{
	/// <summary>
	/// Summary description for IDisconnectedDataReader.
	/// </summary>
	public interface IDisconnectedDataReader : IDataReader
	{
        // Collection of Output Parameters
        IDictionary OutputParams{get;}

        // Row Count will be available for disconnected readers
        Int32 RowCount{get;}

        // An overload to GetValues will pass back an object array
        Object [] GetValues();

        // Response Code
        Int32 ResponseCode{get;}

		//Does Column Exist
		Boolean ColumnExists( String columnName );

        // Error Properties
        String ErrorCode{get;}
        String ErrorDescription{get;}
        String ErrorSeverity{get;}

	}
}
