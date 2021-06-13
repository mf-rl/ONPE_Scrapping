using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using PE_Scrapping.Entidades;
using PE_Scrapping.Funciones;
using System;
using System.Threading;

namespace PE_Scrapping
{
    class Program
    {
        static IWebDriver driver;
        static void Main(string[] args)
        {
            Console.WriteLine("Iniciando.");

            var cfg = InitOptions<AppConfig>();

            driver = new ChromeDriver(cfg.ChromeDriverPath);
            driver.Manage().Window.Minimize();
            Thread.Sleep(5000);
            string opt = string.Empty;
            while (!opt.Equals(Constantes.ProcesarPrimeraV) && !opt.Equals(Constantes.ProcesarSegundaV))
            {
                Console.WriteLine("Digitar opción y presionar <Enter>:");
                Console.WriteLine("1 - Cargar data 1ra V");
                Console.WriteLine("2 - Cargar data 2da V");
                opt = Console.ReadLine();
            }

            var fn = new Funciones.Funciones(driver, cfg, opt);
            fn.GetData();

            driver.Close();
            Console.WriteLine("Finalizado.");
            Console.ReadKey();
        }
        private static T InitOptions<T>() where T : new()
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
