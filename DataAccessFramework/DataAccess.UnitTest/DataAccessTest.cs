using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VKG.CodeFactory.DataAccess.Interfaces;
using VKG.CodeFactory.DataAccess.Facades;
using VKG.CodeFactory.DataAccess.DataTransferObjects;
using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;

namespace VKG.CodeFactory.DataAccess.UnitTest
{
    [TestClass]
    public class DataAccessTest
    {
        [TestMethod]
        public void DataAccessTest_GetDataReader()
        {
            IDataReader reader = null;

            Task task = Task.Factory.StartNew(async ()
             =>
             {
                 using (IDataAccessFacade facade = new DataAccessFacade(DatabaseType.SqlServer))
                 {
                     reader = await facade.ExecuteReader(CreateDataAccessRequest());
                 }
             });
            task.Wait(); 

            Assert.IsFalse(null == reader, "Failed");
            int recordCount = 0;
            while (reader.Read())
            {
                recordCount++;
            }
            Trace.WriteLine($"Record Count={recordCount}, Field Count={reader.FieldCount}");
        }

        [TestMethod]
        public void DataAccessTest_GetDataSet()
        {
            DataSet dataset = null;

            Task task = Task.Factory.StartNew(async ()
             =>
            {
                using (IDataAccessFacade facade = new DataAccessFacade(DatabaseType.SqlServer))
                {
                    dataset = await facade.ExecuteDataSet(CreateDataAccessRequest());
                }
            });
            task.Wait();

            Assert.IsFalse(null == dataset, "Failed");
            Trace.WriteLine($"Data={dataset.GetXml()}");
        }

        private DataAccessRequest CreateDataAccessRequest()
        {
            return new DataAccessRequest()
            {
                Parameters = new List<DataAccessParameter>()
                             {
                            new DataAccessParameter () { Name ="@Category", Type =ParameterType.VarChar , Value ="AP",Direction =ParameterDirection.Input},
                            new DataAccessParameter () { Name ="@AggregateId", Type =ParameterType.VarChar , Value ="2014150101",Direction =ParameterDirection.Input},
                            new DataAccessParameter () { Name ="@MessageType", Type =ParameterType.VarChar , Value ="AccountBetSellMessage",Direction =ParameterDirection.Input},
                             },
                ProcedureName = "[dbo].[sp_SearchEvents]"
            };
        }
    }
}
