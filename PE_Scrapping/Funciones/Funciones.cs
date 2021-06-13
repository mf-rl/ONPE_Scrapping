using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using OpenQA.Selenium;
using PE_Scrapping.Entidades;
using System.Text.Json;
using System.Threading;
using System.Text.Json.Serialization;

namespace PE_Scrapping.Funciones
{
    public class Funciones
    {
        IWebDriver _driver;
        EndPointSet _endPointSet;
        AppConfig _config;
        string _opcion;
        string _seleccion;
        string _mesa_seleccion;
        JsonSerializerOptions _settings;
        List<Department> dep = new();
        List<Province> pro = new();
        List<District> dis = new();
        Transacciones _fn;
        Ubigeo _ubigeos;
        public Funciones()
        {

        }
        public Funciones(IWebDriver driver, AppConfig config, string opcion, string seleccion, string mesa_seleccion)
        {
            _driver = driver;
            _config = config;
            _fn = new Transacciones(_config.ConnectionString);
            _endPointSet = opcion.Equals(Constantes.ProcesarPrimeraV) ? _config.Api.First : _config.Api.Second;
            _opcion = opcion;
            _seleccion = seleccion;
            _mesa_seleccion = mesa_seleccion;
            _settings = new JsonSerializerOptions
            {
                NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString
            };
        }
        public void GetData()
        {
            var tr = new Transacciones(_config.ConnectionString);
            if (!Directory.Exists(Path.Combine(_config.SavePath, _endPointSet.Title)))
            {
                Directory.CreateDirectory(Path.Combine(_config.SavePath, _endPointSet.Title));
            }
            Console.WriteLine("Limpiando tablas...");
            tr.LimpiarData(_opcion);

            Console.WriteLine("Obteniendo data de Ubigeo - {0}", _endPointSet.Title);
            var json = SendApiRequest(_endPointSet.BaseUri + _endPointSet.Ubigeo, _endPointSet.BodyTag);

            _ubigeos = JsonSerializer.Deserialize<Ubigeo>(json, _settings);
            tr.GuardarUbigeos(_ubigeos, _opcion);

            if (_seleccion.Equals(Constantes.TodasLasMesas))
            {
                ProcessAmbit(_ubigeos.ubigeos.nacional);
                ProcessAmbit(_ubigeos.ubigeos.extranjero);
            }
            else
            {
                Console.WriteLine("Obteniendo resultados en Mesa N° {0}", _mesa_seleccion);
                ObtenerDetalleMesa(_fn, _mesa_seleccion);
            }
        }
        public string SendApiRequest(string url, string tag)
        {
            bool success = false;
            string json = string.Empty;            
            while (!success)
            {
                try
                {
                    _driver.Navigate().GoToUrl(url);
                    Thread.Sleep(_config.MilisecondsWait);
                    json = _driver.FindElement(By.TagName(tag)).Text;
                    success = true;
                }
                catch { }
            }            
            return json;
        }
        private string UbigeoAuxiliar(string ambitoSt, object ambito)
        {
            string ambito_desc = string.Empty;
            switch (ambitoSt)
            {
                case Constantes.AmbitoNacional:
                    Nacional nacional = (Nacional)ambito;
                    dep = nacional.departments;
                    pro = nacional.provinces;
                    dis = nacional.districts;
                    ambito_desc = Constantes.AmbitoNacional;
                    break;
                case Constantes.AmbitoExtranjero:
                    Extranjero extranjero = (Extranjero)ambito;
                    dep = extranjero.continents;
                    pro = extranjero.countries;
                    dis = extranjero.states;
                    ambito_desc = Constantes.AmbitoExtranjero;
                    break;
            }
            return ambito_desc;
        }

        public void ProcessAmbit(object ambito)
        {
            string ambito_desc = string.Empty;
            string json = string.Empty;
            ambito_desc = UbigeoAuxiliar(ambito.GetType().Name, ambito);
            switch (ambito.GetType().Name)
            {
                case Constantes.AmbitoNacional:
                    Nacional nacional = (Nacional)ambito;
                    dep = nacional.departments;
                    pro = nacional.provinces;
                    dis = nacional.districts;
                    ambito_desc = Constantes.AmbitoNacional;
                    break;
                case Constantes.AmbitoExtranjero:
                    Extranjero extranjero = (Extranjero)ambito;
                    dep = extranjero.continents;
                    pro = extranjero.countries;
                    dis = extranjero.states;
                    ambito_desc = Constantes.AmbitoExtranjero;
                    break;
            }

            int index_dep = 0;
            Console.WriteLine("Procesando ámbito: {0}", ambito_desc);
            dep.ForEach(d =>
            {
                index_dep++;
                Console.WriteLine("{0}.- {1}", index_dep.ToString(), d.DESC_DEP);
                int index_pro = 0;
                List<Province> level2 = pro.Where(f => f.CDGO_PADRE.Equals(d.CDGO_DEP)).ToList();
                level2.ForEach(dd =>
                {
                    index_pro++;
                    Console.WriteLine("{0}.{1}.- {2}", index_dep.ToString(), index_pro.ToString(), dd.DESC_PROV);
                    int index_dis = 0;
                    List<District> level3 = dis.Where(f => f.CDGO_PADRE.Equals(dd.CDGO_PROV)).ToList();
                    level3.ForEach(ddd =>
                    {
                        index_dis++;
                        Console.WriteLine("{0}.{1}.{2}.- {3}", index_dep.ToString(), index_pro.ToString(), index_dis.ToString(), ddd.DESC_DIST);
                        int index_loc = 0;
                        Local locales = ObtenerLocales(ddd.CDGO_DIST);
                        _fn.GuardarLocales(locales, _opcion);
                        locales.locales.ForEach(l =>
                        {
                            index_loc++;
                            Console.WriteLine("{0}.{1}.{2}.{3}.- Local: {4}", index_dep.ToString(), index_pro.ToString(), index_dis.ToString(), index_loc.ToString(), l.TNOMB_LOCAL);
                            int index_mes = 0;
                            Mesa mesas = ObtenerMesas(ddd.CDGO_DIST, l.CCODI_LOCAL);
                            _fn.GuardarMesas(mesas, l.CCODI_LOCAL, l.CCODI_UBIGEO, _opcion);
                            mesas.mesasVotacion.ForEach(m =>
                            {
                                index_mes++;
                                Console.WriteLine("{0}.{1}.{2}.{3}.{4}.- Mesa N° {5}", index_dep.ToString(), index_pro.ToString(), index_dis.ToString(), index_loc.ToString(), index_mes.ToString(), m.NUMMESA);
                                if (m.PROCESADO == 0)
                                    Console.WriteLine("---->Mesa no procesada.");
                                else
                                {
                                    ObtenerDetalleMesa(_fn, m.NUMMESA, d.DESC_DEP, dd.DESC_PROV, ddd.DESC_DIST, l.CCODI_LOCAL);
                                }
                            });
                        });
                    });
                });
            });          
        }
        private Mesa ObtenerMesas(string codigo_distrito, string codigo_local)
        {
            var json = SendApiRequest(_endPointSet.BaseUri + _endPointSet.Table
                                    .Replace("{ubigeo_code}", codigo_distrito)
                                    .Replace("{locale_code}", codigo_local)
                                    , _endPointSet.BodyTag);
            Mesa mesas = JsonSerializer.Deserialize<Mesa>(json, _settings);
            return mesas;
        }
        private Local ObtenerLocales(string codigo_distrito)
        {
            var json = SendApiRequest(_endPointSet.BaseUri + _endPointSet.Locale.Replace("{ubigeo_code}", codigo_distrito), _endPointSet.BodyTag);
            Local locales = JsonSerializer.Deserialize<Local>(json, _settings);
            return locales;
        }
        private void ObtenerDetalleMesa(Transacciones funciones, string numero_mesa,
            string departamento = null, string provincia = null, string distrito = null, string local = null)
        {
            var json = SendApiRequest(_endPointSet.BaseUri + _endPointSet.TableDetail
                                            .Replace("{table_code}", numero_mesa)
                                            , _endPointSet.BodyTag);
            MesaDetalle mesaDetalle = JsonSerializer.Deserialize<MesaDetalle>(json, _settings);
            funciones.GuardarMesaDetalle(mesaDetalle, numero_mesa, _opcion);

            if (string.IsNullOrEmpty(distrito))
            {
                Console.WriteLine("Obteniendo detalle de ubigeo para descarga de acta.");

                if (mesaDetalle.procesos.generalPre.presidencial.CCODI_UBIGEO.StartsWith("9"))
                    UbigeoAuxiliar(Constantes.AmbitoExtranjero, _ubigeos.ubigeos.extranjero);
                else
                    UbigeoAuxiliar(Constantes.AmbitoNacional, _ubigeos.ubigeos.nacional);


                District _distrito = dis.FirstOrDefault(f => f.CDGO_DIST.Equals(mesaDetalle.procesos.generalPre.presidencial.CCODI_UBIGEO));
                if (_distrito != null)
                {
                    distrito = _distrito != null ? _distrito.DESC_DIST : string.Empty;
                    Province _provincia = pro.FirstOrDefault(f => f.CDGO_PROV.Equals(_distrito.CDGO_PADRE));
                    if (_provincia != null)
                    {
                        provincia = _provincia.DESC_PROV;
                        Department _departamento = dep.FirstOrDefault(f => f.CDGO_DEP.Equals(_provincia.CDGO_PADRE));
                        if (_departamento != null)
                        {
                            departamento = _departamento.DESC_DEP;
                        }
                    }
                    Local _local = ObtenerLocales(_distrito.CDGO_DIST);
                    if (_local != null)
                    {
                        _local.locales.ForEach(l =>
                        {
                            Mesa mesas = ObtenerMesas(l.CCODI_UBIGEO, l.CCODI_LOCAL);
                            if (mesas.mesasVotacion.Any(f => f.NUMMESA.Equals(numero_mesa)))
                            {
                                local = l.CCODI_LOCAL;
                            }
                        }); 
                    }
                } else
                {
                    distrito = string.Empty;
                    provincia = string.Empty;
                    distrito = string.Empty;
                    local = string.Empty;
                }                
            }

            if (_config.DownloadFiles)
            {
                if (mesaDetalle.procesos.generalPre != null)
                    DescargarActa(mesaDetalle.procesos.generalPre.imageActa,
                        string.Concat("PRE_", departamento, "_", provincia, "_", distrito, "_", local, "_", numero_mesa, ".pdf"));
                if (mesaDetalle.procesos.generalCon != null)
                    DescargarActa(mesaDetalle.procesos.generalCon.imageActa,
                        string.Concat("CON_", departamento, "_", provincia, "_", distrito, "_", local, "_", numero_mesa, ".pdf"));
                if (mesaDetalle.procesos.generalPar != null)
                    DescargarActa(mesaDetalle.procesos.generalPar.imageActa,
                        string.Concat("PAR_", departamento, "_", provincia, "_", distrito, "_", local, "_", numero_mesa, ".pdf"));
            }
        }
        public void DescargarActa(string url_acta, string save_file)
        {
            Uri uriResult;
            bool result = Uri.TryCreate(url_acta, UriKind.Absolute, out uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            if (result)
                using (var client = new WebClient())
                {
                    client.DownloadFile(url_acta, Path.Combine(_config.SavePath, _endPointSet.Title, save_file));
                }
        }
    }
}
