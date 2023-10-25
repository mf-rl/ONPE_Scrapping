using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace PE_Scrapping.Funciones
{
    public static class HttpHandler
    {
        static string error_root = string.Empty;

        public static async Task<string> SendApiRequest(string url) //, string tag)
        {
            bool success = false;
            string json = string.Empty;
            while (!success)
            {
                using (HttpClient client = new())
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36");
                    try
                    {
                        HttpResponseMessage response = await client.GetAsync(url);

                        if (response.IsSuccessStatusCode)
                        {
                            json = await response.Content.ReadAsStringAsync();
                            success = true;
                        }
                        else
                        {
                            Console.WriteLine($"HTTP request failed with status code: {response.StatusCode} for {url}");
                        }

                    }
                    catch { /*Do nothing. Just retry if it fails */ }
                }
            }
            return json;
        }
        public static async Task DownloadFile(string url_file, string save_file, string path, string folder)
        {
            bool result = Uri.TryCreate(url_file, UriKind.Absolute, out Uri uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            if (result)
            {
                string full_path = Path.Combine(string.Format(path, "ACTAS"), folder);
                if (!Directory.Exists(full_path))
                {
                    Directory.CreateDirectory(full_path);
                }
                full_path = Path.Combine(full_path, save_file);

                using (HttpClient client = new())
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36");
                    bool success = false;
                    int intento = 0;
                    while (!success && intento <= 5)
                    {
                        try
                        {
                            HttpResponseMessage response = await client.GetAsync(url_file);

                            if (response.IsSuccessStatusCode)
                            {
                                byte[] fileBytes = await response.Content.ReadAsByteArrayAsync();
                                File.WriteAllBytes(full_path, fileBytes);
                            }

                            success = true;
                        }
                        catch (Exception ex)
                        {
                            ErrorLog(string.Concat("Error descargando acta.: ", full_path), path);
                            ErrorLog(ex.Message, path);
                            Console.WriteLine("Error de conexión al intentar descargar acta. Reintentando...");
                            intento++;
                            if (intento < 5)
                            {
                                Console.WriteLine("Reintentando...");
                            }
                            else
                            {
                                ErrorLog("No se pudo descargar acta luego de 5 intentos.", path);
                                Console.WriteLine("No se pudo descargar acta luego de 5 intentos.");
                                success = true;
                            }
                        }
                    }
                }
            }
        }
        private static void ErrorLog(string mensaje, string path)
        {
            error_root = string.IsNullOrEmpty(error_root) ? Guid.NewGuid().ToString() : error_root;
            var ruta_guardar = string.Format(path, "LOG");
            if (!Directory.Exists(ruta_guardar)) Directory.CreateDirectory(ruta_guardar);
            string[] mensajes = { mensaje };
            File.AppendAllLines(Path.Combine(ruta_guardar, string.Concat("errors_", error_root, ".log")), mensajes);
        }
    }
}