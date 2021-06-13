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
            Console.WriteLine("=".PadRight(51, '='));
            Console.WriteLine("RESULTADOS ELECCIONES 2021 - PERÚ".PadLeft(41, ' '));
            string opt = string.Empty;
            string sel = string.Empty;
            string mesa_sel = string.Empty;
            while (!opt.Equals(Constantes.ProcesarPrimeraV) && !opt.Equals(Constantes.ProcesarSegundaV))
            {
                Console.WriteLine("=".PadRight(51, '='));
                Console.WriteLine("Seleccionar proceso:");
                Console.WriteLine("1: Resultados 1ra Vuelta");
                Console.WriteLine("2: Resultados 2da Vuelta");
                Console.WriteLine("=".PadRight(51, '='));
                Console.WriteLine("Digitar opción y presionar <Enter>:");
                Console.Write("Respuesta: ");
                opt = Console.ReadLine();
            }

            while (!sel.Equals(Constantes.TodasLasMesas) && !sel.Equals(Constantes.MesaSeleccionada))
            {
                Console.WriteLine("=".PadRight(51, '='));
                Console.WriteLine("Seleccionar tipo de proceso:");
                Console.WriteLine("1: Todo");
                Console.WriteLine("2: Específico");
                Console.WriteLine("=".PadRight(51, '='));
                Console.WriteLine("Digitar opción y presionar <Enter>:");
                Console.Write("Respuesta: ");
                sel = Console.ReadLine();
            }

            if (sel.Equals(Constantes.MesaSeleccionada))
            {
                Console.WriteLine("=".PadRight(51, '='));
                while (string.IsNullOrEmpty(mesa_sel))
                {
                    Console.WriteLine("Ingresar número de mesa:");
                    Console.Write("Respuesta: ");
                    mesa_sel = Console.ReadLine();
                }
            }

            var cfg = InitOptions<AppConfig>();
            Console.WriteLine("=".PadRight(51, '='));
            Console.WriteLine("Inicializando ChromeDriver...");
            Console.WriteLine("=".PadRight(51, '='));
            driver = new ChromeDriver(cfg.ChromeDriverPath);
            driver.Manage().Window.Minimize();
            Console.WriteLine("=".PadRight(51, '='));
            Console.WriteLine("ChromeDriver Iniciado.");
            Console.WriteLine("=".PadRight(51, '='));
            Thread.Sleep(5000);

            var fn = new Funciones.Funciones(driver, cfg, opt, sel, mesa_sel);
            fn.GetData();

            driver.Close();
            Console.WriteLine("Finalizado. :)");
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
