using Microsoft.Extensions.Configuration;

namespace SearchWorker;

public static class ConfigurationHelper
{
    private static IConfiguration? configuration;

    public static IConfiguration Instance =>
        configuration ?? (configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appettings.json").Build());
}