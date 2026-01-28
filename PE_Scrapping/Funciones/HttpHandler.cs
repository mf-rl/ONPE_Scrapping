using Microsoft.Playwright;
using PE_Scrapping.Entidades;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace PE_Scrapping.Funciones
{
    public static class HttpHandler
    {
        static string error_root = string.Empty;
        
        private static IPlaywright? _playwright;
        private static IBrowser? _browser;
        private static bool _isInitialized = false;
        
        private static async Task InitializePlaywrightAsync()
        {
            if (!_isInitialized)
            {
                _playwright = await Playwright.CreateAsync();
                _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
                {
                    Headless = true,
                    Args = new[] { 
                        "--disable-blink-features=AutomationControlled",
                        "--disable-dev-shm-usage",
                        "--no-sandbox"
                    }
                });
                _isInitialized = true;
            }
        }
        
        public static async Task<string> SendApiRequest(string url)
        {
            await InitializePlaywrightAsync();
            
            var context = await _browser!.NewContextAsync(new BrowserNewContextOptions
            {
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/144.0.0.0 Safari/537.36",
                ViewportSize = new ViewportSize { Width = 1920, Height = 1080 },
                Locale = "es-PE",
                TimezoneId = "America/Lima",
                ExtraHTTPHeaders = new Dictionary<string, string>
                {
                    ["Accept-Language"] = "es-PE,es;q=0.9,en;q=0.8",
                    ["Accept"] = "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,*/*;q=0.8"
                }
            });
            
            var page = await context.NewPageAsync();
            
            try
            {
                //Console.WriteLine($"Fetching {url} using Playwright...");
                
                // Navigate to the URL
                await page.GotoAsync(url, new PageGotoOptions
                {
                    WaitUntil = WaitUntilState.NetworkIdle,
                    Timeout = 30000
                });
                
                // Wait a bit for any Cloudflare challenges to complete
                await page.WaitForTimeoutAsync(3000);
                
                // Try to get JSON from <pre> tag (how browsers display JSON)
                var preElement = await page.QuerySelectorAsync("pre");
                if (preElement != null)
                {
                    var json = await preElement.TextContentAsync();
                    if (!string.IsNullOrWhiteSpace(json) && (json.TrimStart().StartsWith("{") || json.TrimStart().StartsWith("[")))
                    {
                        //Console.WriteLine($"✓ Successfully fetched data ({json.Length} bytes)");
                        return json;
                    }
                }
                
                // Fallback: get body text
                var bodyText = await page.TextContentAsync("body");
                if (!string.IsNullOrWhiteSpace(bodyText) && (bodyText.TrimStart().StartsWith("{") || bodyText.TrimStart().StartsWith("[")))
                {
                    Console.WriteLine($"✓ Successfully fetched data ({bodyText.Length} bytes)");
                    return bodyText;
                }
                
                var preview = bodyText != null ? bodyText.Substring(0, Math.Min(200, bodyText.Length)) : "empty";
                throw new HttpRequestException($"No valid JSON found. Page content: {preview}");
            }
            finally
            {
                await page.CloseAsync();
                await context.CloseAsync();
            }
        }

        public static async Task DownloadFile(
            string urlFile,
            string saveFile,
            string path,
            string folder)
        {
            var context = await _browser!.NewContextAsync(new BrowserNewContextOptions
            {
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/144.0.0.0 Safari/537.36",
                Locale = "es-PE",
                TimezoneId = "America/Lima",
                AcceptDownloads = true
            });

            var page = await context.NewPageAsync();

            try
            {
                if (!Uri.TryCreate(urlFile, UriKind.Absolute, out var pdfUri))
                    return;

                string fullPath = Path.Combine(string.Format(path, "ACTAS"), folder);
                Directory.CreateDirectory(fullPath);
                fullPath = Path.Combine(fullPath, saveFile);

                if (File.Exists(fullPath))
                {
                    Console.WriteLine($"✓ Exists: {saveFile}");
                    return;
                }

                string origin = $"{pdfUri.Scheme}://{pdfUri.Host}";

                await page.GotoAsync(
                    origin,
                    new PageGotoOptions
                    {
                        WaitUntil = WaitUntilState.DOMContentLoaded,
                        Timeout = 30000
                    });

                var download = await page.RunAndWaitForDownloadAsync(async () =>
                {
                    await page.EvaluateAsync(
                        @"url => {
                    const a = document.createElement('a');
                    a.href = url;
                    a.target = '_blank';
                    document.body.appendChild(a);
                    a.click();
                    a.remove();
                }",
                        urlFile
                    );
                });

                await download.SaveAsAsync(fullPath);

                Console.WriteLine($"✓ Downloaded {saveFile}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Error downloading {saveFile}: {ex.Message}");
            }
            finally
            {
                await page.CloseAsync();
                await context.CloseAsync();
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