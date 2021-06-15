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
using System.Diagnostics;
using Newtonsoft.Json.Linq;

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
        string _tipo_proceso;
        int _total_items = 0;
        int _actual_item = 0;
        JsonSerializerOptions _settings;
        List<Department> dep = new();
        List<Province> pro = new();
        List<District> dis = new();
        TransaccionLite _lite;
        Ubigeo _ubigeos;
        public Funciones()
        {

        }
        public Funciones(IWebDriver driver, AppConfig config, string opcion, string tipo_proceso, string seleccion, string mesa_seleccion)
        {
            _driver = driver;
            _config = config;
            _endPointSet = opcion.Equals(Constantes.ProcesarPrimeraV) ? _config.Api.First : _config.Api.Second;
            _opcion = opcion;
            _seleccion = seleccion;
            _mesa_seleccion = mesa_seleccion;
            _tipo_proceso = tipo_proceso;
            _settings = new JsonSerializerOptions
            {
                NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString
            };
            if (_config.SaveData) _lite = new TransaccionLite(_config.DataBaseName);
        }
        private void GuardarJSON(string json, string nombre, string carpeta = null)
        {            
            var archivo_nombre = string.Concat(nombre.Trim(), ".json");
            var ruta_guardar = Path.Combine(_config.SavePath, _endPointSet.Title, "JSON", string.IsNullOrEmpty(carpeta) ? string.Empty : carpeta);
            if (!Directory.Exists(ruta_guardar)) Directory.CreateDirectory(ruta_guardar);
            File.WriteAllText(Path.Combine(ruta_guardar, archivo_nombre), json);
        }
        public static TValue JsonToObject<TValue>(string json, JsonSerializerOptions? options = null)
        {
            JObject _jobject = JObject.Parse(json);
            TValue _object = _jobject.ToObject<TValue>();
            return _object;
        }
        public void GetData()
        {
            if (_config.DownloadFiles && !Directory.Exists(Path.Combine(_config.SavePath, _endPointSet.Title)))
                Directory.CreateDirectory(Path.Combine(_config.SavePath, _endPointSet.Title));

            if (_config.SaveJson && !Directory.Exists(Path.Combine(_config.SavePath, _endPointSet.Title, "JSON")))
                Directory.CreateDirectory(Path.Combine(_config.SavePath, _endPointSet.Title, "JSON"));

            Console.WriteLine("Limpiando tablas de ubigeo...");
            if (_config.SaveData) _lite.LimpiarDataUbigeo(_opcion);

            Console.WriteLine("Obteniendo data de Ubigeo - {0}", _endPointSet.Title);
            var json = SendApiRequest(_endPointSet.BaseUri + _endPointSet.Ubigeo, _endPointSet.BodyTag);
            if (_config.SaveJson) GuardarJSON(json, "Ubigeos");

            _ubigeos = JsonToObject<Ubigeo>(json);

            if (_config.SaveData) _lite.GuardarUbigeos(_ubigeos, _opcion);

            if (_tipo_proceso.Equals(Constantes.ProcesoTotal))
            {
                _total_items = _ubigeos.ubigeos.nacional.districts.Count() + _ubigeos.ubigeos.extranjero.states.Count();
                Console.WriteLine("Limpiando data previa...");
                if (_config.SaveData) _lite.LimpiarData(_opcion);
                ProcessAmbit(_ubigeos.ubigeos.nacional);
                ProcessAmbit(_ubigeos.ubigeos.extranjero);
            }
            else
            {
                switch (_seleccion)
                {
                    case Constantes.ProcesoMesa:
                        Console.WriteLine("Limpiando data previa de la mesa {0}...", _mesa_seleccion);
                        if (_config.SaveData) _lite.LimpiarDataMesa(_opcion, _mesa_seleccion);
                        Console.WriteLine("Obteniendo resultados en Mesa N° {0}", _mesa_seleccion);
                        ObtenerDetalleMesa(_mesa_seleccion, null, null, null, null, null);
                        break;
                    case Constantes.ProcesoUbigeo:
                        Console.WriteLine("Limpiando data previa de ubigeo {0}...", _mesa_seleccion);
                        if (_config.SaveData) _lite.LimpiarDataDetalleUbigeo(_opcion, _mesa_seleccion);
                        Console.WriteLine("Procesando ubigeo seleccionado {0}", _mesa_seleccion);
                        string dep_raiz = _mesa_seleccion.Substring(0, 2);

                        string pro_raiz = _mesa_seleccion.Substring(0, 2).PadRight(6, '0').Equals(_mesa_seleccion) ? 
                            _mesa_seleccion.Substring(0, 2) : _mesa_seleccion.Substring(0, 4);

                        string dis_raiz = _mesa_seleccion.Substring(0, 2).PadRight(6, '0').Equals(_mesa_seleccion) ? 
                            _mesa_seleccion.Substring(0, 2) :
                            _mesa_seleccion.Substring(0, 4).PadRight(6, '0').Equals(_mesa_seleccion) ?
                                _mesa_seleccion.Substring(0, 4) : _mesa_seleccion;

                        if (_mesa_seleccion.StartsWith("9"))
                        {
                            Extranjero sel_ambito = _ubigeos.ubigeos.extranjero;
                            sel_ambito.continents = sel_ambito.continents.Where(f => f.CDGO_DEP.StartsWith(dep_raiz)).ToList();
                            sel_ambito.countries = sel_ambito.countries.Where(f => f.CDGO_PROV.StartsWith(pro_raiz)).ToList();
                            sel_ambito.states = sel_ambito.states.Where(f => f.CDGO_DIST.StartsWith(dis_raiz)).ToList();
                            ProcessAmbit(sel_ambito);
                        } else
                        {
                            Nacional sel_ambito = _ubigeos.ubigeos.nacional;
                            sel_ambito.departments = sel_ambito.departments.Where(f => f.CDGO_DEP.StartsWith(dep_raiz)).ToList();
                            sel_ambito.provinces = sel_ambito.provinces.Where(f => f.CDGO_PROV.StartsWith(pro_raiz)).ToList();
                            sel_ambito.districts = sel_ambito.districts.Where(f => f.CDGO_DIST.StartsWith(dis_raiz)).ToList();
                            ProcessAmbit(sel_ambito);
                        }
                        break;
                }
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
            if (_tipo_proceso.Equals(Constantes.ProcesoParcial) && _seleccion.Equals(Constantes.ProcesoUbigeo)) {
                _total_items = dis.Count;
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
                        _actual_item++;
                        index_dis++;
                        MostrarPorcentajeAvance(_actual_item - 1, _total_items);

                        Console.WriteLine("{0}.{1}.{2}.- {3}", index_dep.ToString(), index_pro.ToString(), index_dis.ToString(), ddd.DESC_DIST);
                        int index_loc = 0;
                        Local locales = ObtenerLocales(ddd.CDGO_DIST, d.DESC_DEP, dd.DESC_PROV, ddd.DESC_DIST);
                        if (_config.SaveData) _lite.GuardarLocales(locales, _opcion);
                        locales.locales.ForEach(l =>
                        {
                            index_loc++;
                            Console.WriteLine("{0}.{1}.{2}.{3}.- Local: {4}", index_dep.ToString(), index_pro.ToString(), index_dis.ToString(), index_loc.ToString(), l.TNOMB_LOCAL);
                            int index_mes = 0;
                            Mesa mesas = ObtenerMesas(ddd.CDGO_DIST, l.CCODI_LOCAL, d.DESC_DEP, dd.DESC_PROV, ddd.DESC_DIST, l.TNOMB_LOCAL);
                            if (_config.SaveData) _lite.GuardarMesas(mesas, l.CCODI_LOCAL, l.CCODI_UBIGEO, _opcion);
                            mesas.mesasVotacion.ForEach(m =>
                            {
                                index_mes++;
                                Console.WriteLine("{0}.{1}.{2}.{3}.{4}.- Mesa N° {5}", index_dep.ToString(), index_pro.ToString(), index_dis.ToString(), index_loc.ToString(), index_mes.ToString(), m.NUMMESA);
                                if (m.PROCESADO.Equals("0"))
                                    Console.WriteLine("---->Mesa no procesada.");
                                else
                                {
                                    ObtenerDetalleMesa(m.NUMMESA, d.DESC_DEP, dd.DESC_PROV, ddd.DESC_DIST, l.CCODI_LOCAL, l.TNOMB_LOCAL);
                                }
                            });
                        });
                        MostrarPorcentajeAvance(_actual_item, _total_items);
                    });
                });
            });          
        }
        private Mesa ObtenerMesas(string codigo_distrito, string codigo_local, string departamento, string provincia, string distrito, string nombre_local)
        {
            var json = SendApiRequest(_endPointSet.BaseUri + _endPointSet.Table
                                    .Replace("{ubigeo_code}", codigo_distrito)
                                    .Replace("{locale_code}", codigo_local)
                                    , _endPointSet.BodyTag);
            var ruta_carpeta = Path.Combine(departamento, provincia, distrito);
            if (_config.SaveJson) GuardarJSON(json, string.Concat("Mesas de ", FormatFileName(nombre_local)), ruta_carpeta);
            Mesa mesas = JsonToObject<Mesa>(json);
            return mesas;
        }
        private Local ObtenerLocales(string codigo_distrito, string departamento, string provincia, string distrito)
        {
            var json = SendApiRequest(_endPointSet.BaseUri + _endPointSet.Locale.Replace("{ubigeo_code}", codigo_distrito), _endPointSet.BodyTag);
            var ruta_carpeta = Path.Combine(departamento, provincia);
            if (_config.SaveJson) GuardarJSON(json, string.Concat("Locales de ", distrito), ruta_carpeta);
            Local locales = JsonToObject<Local>(json);
            return locales;
        }
        private void ObtenerDetalleMesa(string numero_mesa,
            string departamento = null, string provincia = null, string distrito = null, string local_codigo = null, string local_nombre = null)
        {
            var json = SendApiRequest(_endPointSet.BaseUri + _endPointSet.TableDetail
                                            .Replace("{table_code}", numero_mesa)
                                            , _endPointSet.BodyTag);
            MesaDetalle mesaDetalle = JsonToObject<MesaDetalle>(json);

            if (!_tipo_proceso.Equals(Constantes.ProcesoTotal) && _seleccion.Equals(Constantes.ProcesoMesa)) _total_items = 1;

            if (_config.SaveData) _lite.GuardarMesaDetalle(mesaDetalle, numero_mesa, _opcion);

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
                    Local _local = ObtenerLocales(_distrito.CDGO_DIST, departamento, provincia, distrito);
                    if (_local != null)
                    {
                        _local.locales.ForEach(l =>
                        {
                            Mesa mesas = ObtenerMesas(l.CCODI_UBIGEO, l.CCODI_LOCAL, departamento, provincia, distrito, l.TNOMB_LOCAL);
                            if (mesas.mesasVotacion.Any(f => f.NUMMESA.Equals(numero_mesa)))
                            {
                                local_codigo = l.CCODI_LOCAL;
                                local_nombre = l.TNOMB_LOCAL;
                            }
                        }); 
                    }
                } else
                {
                    distrito = string.Empty;
                    provincia = string.Empty;
                    distrito = string.Empty;
                    local_codigo = string.Empty;
                    local_nombre = string.Empty;
                }                
            }

            var ruta_carpeta = Path.Combine(departamento, provincia, distrito, FormatFileName(local_nombre));
            if (_config.SaveJson) GuardarJSON(json, string.Concat("Detalle de Mesa Nro ", numero_mesa), ruta_carpeta);
            if (_config.DownloadFiles)
            {
                if (mesaDetalle.procesos.generalPre != null)
                    DescargarActa(mesaDetalle.procesos.generalPre.imageActa,
                        string.Concat("PRE-", numero_mesa, ".pdf"),
                        departamento, provincia, distrito, local_nombre);
                if (mesaDetalle.procesos.generalCon != null)
                    DescargarActa(mesaDetalle.procesos.generalCon.imageActa,
                        string.Concat("CON-", numero_mesa, ".pdf"),
                        departamento, provincia, distrito, local_nombre);
                if (mesaDetalle.procesos.generalPar != null)
                    DescargarActa(mesaDetalle.procesos.generalPar.imageActa,
                        string.Concat("PAR-", numero_mesa, ".pdf"),
                        departamento, provincia, distrito, local_nombre);
            }
            if (!_tipo_proceso.Equals(Constantes.ProcesoTotal) && _seleccion.Equals(Constantes.ProcesoMesa))
            {
                _actual_item = 1;
                MostrarPorcentajeAvance(_actual_item, _total_items);
            }
        }
        private void MostrarPorcentajeAvance(int index, int total)
        {
            int avance = index * 100 / total;
            Console.Title = string.Format("Procesando {0}%", avance.ToString());
        }
        public void DescargarActa(string url_acta, string save_file, string departamento, string provincia, string distrito, string local)
        {
            Uri uriResult;
            bool result = Uri.TryCreate(url_acta, UriKind.Absolute, out uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            if (result)
            {
                string full_path = Path.Combine(_config.SavePath, _endPointSet.Title, "ACTAS", departamento, provincia, distrito, FormatFileName(local));
                if (!Directory.Exists(full_path))
                {
                    Directory.CreateDirectory(full_path);
                }
                full_path = Path.Combine(full_path, save_file);
                using (var client = new WebClient())
                {
                    client.DownloadFile(url_acta, full_path);
                }
            }                
        }
        private string FormatFileName(string file_name)
        {
            var invalid = Path.GetInvalidFileNameChars();
            invalid.ToList().ForEach(c =>
            {
                file_name = file_name.Replace(c, '-');
            });
            return file_name.Trim();
        }
        public void AbrirFolder(string path)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                Arguments = path,
                FileName = "explorer.exe"
            };
            Process.Start(startInfo);
        }
    }
}
