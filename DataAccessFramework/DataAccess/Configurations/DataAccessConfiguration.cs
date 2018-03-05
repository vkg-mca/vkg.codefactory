using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.String;
using System.Configuration;

namespace VKG.CodeFactory.DataAccess.Configurations
{
    internal class DataAccessConfiguration
    {
        private static string _connectionString;
        internal static string SqlConnectionString
        {
            get
            {
                if (IsNullOrWhiteSpace(_connectionString))
                {
                    if (!(null == ConfigurationManager.ConnectionStrings["SqlConnectionString"]))
                        _connectionString = ConfigurationManager.ConnectionStrings["SqlConnectionString"].ConnectionString;
                    else
                        throw new ConfigurationErrorsException("SqlConnectionString must be defined in the configuration file");
                }
                return _connectionString;
            }
        }
    }
}

