using System;
using Microsoft.Extensions.Configuration;

namespace PE_Scrapping.Funciones
{
    public static class Configuration
    {
        public static T Initialize<T>() where T : new()
        {
            var config = InitConfig();
            return config.Get<T>();
        }
        private static IConfigurationRoot InitConfig()
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var builder = new ConfigurationBuilder()
                .AddJsonFile($"appSettings.json", true, true)
                .AddJsonFile($"appSettings.{env}.json", true, true)
                .AddEnvironmentVariables();

            return builder.Build();
        }
    }
}
