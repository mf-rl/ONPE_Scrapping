using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using PE_Scrapping.Entidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using static PE_Scrapping.Funciones.Constantes;

namespace PE_Scrapping
{
    class Program
    {
        static IWebDriver driver;
        static string uri_base = "https://www.resultadossep.eleccionesgenerales2021.pe/SEP2021/Actas/Ubigeo/";
        static string save_path = @"D:\VPK\Actas";
        static string connection_string = "Data Source=.;" +
          "Initial Catalog=Stuff_PE;" +
          "User id=sa;" +
          "Password=K4sp4r0v;";
        static void Main(string[] args)
        {
            Console.WriteLine("Iniciando.");
            //driver = new ChromeDriver("runtimes");
            //driver.Manage().Window.Minimize();


            string opt = string.Empty;
            while (!opt.Equals(ProcesarUbigeo) && !opt.Equals(ProcesarPrimeraV) && !opt.Equals(ProcesarSegundaV))
            {
                Console.WriteLine("Opción?");
                Console.WriteLine("0 - Cargar ubigeos");
                Console.WriteLine("1 - Cargar data 1ra V");
                Console.WriteLine("2 - Cargar data 2da V");
                opt = Console.ReadLine();
            }

            switch (opt)
            {
                case ProcesarUbigeo:
                    break;
                case ProcesarPrimeraV:
                    break;
                case ProcesarSegundaV:
                    break;
            }


            //driver.Close();
            Console.WriteLine("Finalizado.");
            Console.ReadKey();
        }

        
        //static void ProcessTables(List<Mesa> mesas, string uri_base)
        //{
        //    mesas.ForEach(m =>
        //    {
        //        driver.FindElement(By.CssSelector("body")).SendKeys(Keys.Control + "t");
        //        driver.SwitchTo().Window(driver.WindowHandles.Last());
        //        driver.Navigate().GoToUrl(string.Concat(uri_base, m.Numero));
        //    });
        //    Thread.Sleep(3000);
        //}

        static Mesa GetTableData(string uri_path, Mesa mesa, string ambito, string departamento, string provincia, string distrito, string local)
        {
            Mesa _mesa = new()
            {
                Numero = mesa.Numero,
                Procesada = mesa.Procesada,
                amb_cod = ambito,
                dep_cod = departamento,
                pro_cod = provincia,
                dis_cod = distrito,
                loc_cod = local,
                save_success = false
            };
            driver.Navigate().GoToUrl(uri_path);
            Thread.Sleep(3000);
            string pageSource = driver.PageSource;
            int failed_tries = 0;
            do
            {
                try
                {
                    var doc = new HtmlDocument();
                    doc.LoadHtml(pageSource);
                    HtmlNodeCollection actaLink = doc.DocumentNode.SelectNodes("//a[@href]");
                    string aLink = string.Empty;
                    actaLink.ToList().ForEach(l =>
                    {
                        string hrefValue = l.GetAttributeValue("href", string.Empty);
                        aLink = hrefValue.Contains("https://presentacionsep2021-actas-resultados") ? hrefValue : aLink;
                    });
                    if (!Directory.Exists(Path.Combine(save_path, ambito, departamento, provincia, distrito, local)))
                    {
                        Directory.CreateDirectory(Path.Combine(save_path, ambito, departamento, provincia, distrito, local));
                    }
                    if (!string.IsNullOrEmpty(aLink))
                    {
                        aLink = HttpUtility.HtmlDecode(aLink);
                        using (var client = new WebClient())
                        {
                            client.DownloadFile(aLink, Path.Combine(save_path, ambito, departamento, provincia, distrito, local, string.Concat(mesa.Numero, ".pdf")));
                        }
                    }
                    HtmlNodeCollection nodeCol = doc.DocumentNode.SelectNodes("//table[@id=\"tableMovil\"]");
                    var rows = nodeCol.Descendants("tr").ToList();
                    int row = 0;
                    rows.ForEach(r =>
                    {
                        row++;
                        if (row > 2)
                        {
                            var columns = r.ChildNodes.Where(w => w.Name.ToLower().Contains("td")).ToArray();
                            switch (columns[1].InnerText.Trim())
                            {
                                case "PARTIDO POLITICO NACIONAL PERU LIBRE":
                                    _mesa.PL = columns[2].InnerText.Trim();
                                    break;
                                case "FUERZA POPULAR":
                                    _mesa.FP = columns[2].InnerText.Trim();
                                    break;
                                case "VOTOS EN BLANCO":
                                    _mesa.VB = columns[2].InnerText.Trim();
                                    break;
                                case "VOTOS NULOS":
                                    _mesa.VN = columns[2].InnerText.Trim();
                                    break;
                                case "VOTOS IMPUGNADOS":
                                    _mesa.VI = columns[2].InnerText.Trim();
                                    break;
                            }
                        }
                    });
                    _mesa.save_success = true;
                }
                catch
                {
                    _mesa.save_success = false;
                    failed_tries++;
                }
            } while (!_mesa.save_success && failed_tries <=3);            
            return _mesa;
        }
        static List<Mesa> GetTables(string uri_path)
        {
            List<Mesa> res = new List<Mesa>();
            driver.Navigate().GoToUrl(uri_path);
            Thread.Sleep(3000);
            string pageSource = driver.PageSource;
            var doc = new HtmlDocument();
            doc.LoadHtml(pageSource);
            HtmlNodeCollection nodeCol = doc.DocumentNode.SelectNodes("//div[@class=\"mesas procesada_sin\"]");
            if (nodeCol != null)
                nodeCol.ToList().ForEach(t =>
                {
                    var table = t.InnerText;
                    res.Add(new Mesa {
                        Numero = table.Trim(),
                        Procesada = true
                    });
                });
            nodeCol = doc.DocumentNode.SelectNodes("//div[@class=\"mesas procesadas_sin\"]");
            if (nodeCol != null)
                nodeCol.ToList().ForEach(t =>
                {
                    var table = t.InnerText;
                    res.Add(new Mesa
                    {
                        Numero = table,
                        Procesada = false
                    });
                });
            return res;
        }
        static List<Ubigeo> GetItems(int node_index, string select_name, string uri_path)
        {
            driver.Navigate().GoToUrl(uri_path);
            Thread.Sleep(3000);
            string pageSource = driver.PageSource;
            var doc = new HtmlDocument();
            doc.LoadHtml(pageSource);
            HtmlNodeCollection nodeCol = doc.DocumentNode.SelectNodes("//select[@name=\""+ select_name+"\"]");
            pageSource = nodeCol[0].InnerHtml;
            doc.LoadHtml(pageSource);
            nodeCol = doc.DocumentNode.SelectNodes("//option");
            List<Ubigeo> ubigeos = new();
            nodeCol.ToList().ForEach(n =>
            {
                Ubigeo ubigeo = new()
                {
                    ubigeo_id = n.Attributes["value"].Value,
                    ubigeo_dc = n.InnerHtml,
                    ubigeo_lv = node_index
                };
                ubigeos.Add(ubigeo);
            });
            return ubigeos.Where(f => !f.ubigeo_id.Equals("0")).ToList();
        }
        static void SaveUbigeo(List<Ubigeo> ubigeos)
        {
            ubigeos.ForEach(u =>
            {
                using (var conn = new SqlConnection(connection_string))
                using (var command = new SqlCommand("insert_Ubigeo", conn)
                {
                    CommandType = CommandType.StoredProcedure,
                })
                {
                    command.Parameters.Add(new SqlParameter("@ubigeo_id", u.ubigeo_id));
                    command.Parameters.Add(new SqlParameter("@ubigeo_dc", u.ubigeo_dc));
                    command.Parameters.Add(new SqlParameter("@ubigeo_lv", u.ubigeo_lv));
                    conn.Open();
                    command.ExecuteNonQuery();
                }
            });


            
        }
        static void SaveMesa(Mesa mesa)
        {
            using (var conn = new SqlConnection(connection_string))
            using (var command = new SqlCommand("insert_Mesa", conn)
            {
                CommandType = CommandType.StoredProcedure
            })
            {
                command.Parameters.Add(new SqlParameter("@Numero", mesa.Numero));
                command.Parameters.Add(new SqlParameter("@Procesada", mesa.Procesada));
                command.Parameters.Add(new SqlParameter("@FP", mesa.FP));
                command.Parameters.Add(new SqlParameter("@PL", mesa.PL));
                command.Parameters.Add(new SqlParameter("@VB", mesa.VB));
                command.Parameters.Add(new SqlParameter("@VN", mesa.VN));
                command.Parameters.Add(new SqlParameter("@VI", mesa.VI));
                command.Parameters.Add(new SqlParameter("@amb_cod", mesa.amb_cod));
                command.Parameters.Add(new SqlParameter("@dep_cod", mesa.dep_cod));
                command.Parameters.Add(new SqlParameter("@pro_cod", mesa.pro_cod));
                command.Parameters.Add(new SqlParameter("@dis_cod", mesa.dis_cod));
                command.Parameters.Add(new SqlParameter("@loc_cod", mesa.loc_cod));
                command.Parameters.Add(new SqlParameter("@save_success", mesa.save_success));
                conn.Open();
                command.ExecuteNonQuery();
            }
        }
    }
}
