using Microsoft.Extensions.Configuration;

namespace ProcessAffinity
{
    internal class AppSetting
    {
        public AppSetting()
        {

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false);

            IConfiguration config = builder.Build();

            var section = config.GetSection("AppSettings");
            EnableProcessAffinity = bool.TryParse(section["EnableProcessAffinity"], out var enableProcessAffinity) 
                ? enableProcessAffinity: 
                false;
            ProcessAffinity = section["ProcessAffinity"];
        }
        public bool EnableProcessAffinity { get; internal set; }
        public string? ProcessAffinity { get; internal set; }
    }
}