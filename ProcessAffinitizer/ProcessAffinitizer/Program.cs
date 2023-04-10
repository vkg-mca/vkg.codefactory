using Microsoft.Extensions.Configuration;
using NLog;
using NLog.Extensions.Logging;
using System.Diagnostics;
using System.Reflection.Metadata;

namespace ProcessAffinity
{
    public class Program
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        static void Main(string[] args)
        {
            
            bool isAffinitySetSuccess = ProcessAffinity.Setup(out string affinitySetResult);
            var message = $"ProcessAffinity=>status:{isAffinitySetSuccess},result:{affinitySetResult}";
            logger.Info (message);
            Console.WriteLine(message);
            Console.ReadLine();
            
        }
    }
}