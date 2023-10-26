using System;
using System.IO;
using System.Linq;
using PE_Scrapping.Tablas;
using Newtonsoft.Json.Linq;
using PE_Scrapping.Entidades;
using YPandar.Common.Functional;
using System.Collections.Generic;

namespace PE_Scrapping.Funciones
{
    public static class MainProcess
    {
        static InputParameters _input = new();

        static EndPointSet _endPointSet;
        static AppConfig _config;

        static string _etq_base;
        static string _appendName;

        static List<District> _dis = new();

        static List<District> TDistritos = new();
        static List<Province> TProvincias = new();
        static List<Department> TDepartamentos = new();

        static readonly List<TUbigeo> TUbigeos = new();
        static List<TLocal> TLocales = new();
        static readonly List<TMesa> TMesas = new();
        static readonly List<TActa> TActas = new();
        static readonly List<TVoto> TVotos = new();
        
        static readonly List<Locale> _locales = new();
        static readonly List<MesasVotacion> _mesas = new();
        static readonly List<MesaDetalle> _mesaDetalles = new();

        public static void ExecuteProcess(InputParameters input)
        {
            _input = input;
            _config = Configuration.Initialize<AppConfig>();
            _endPointSet = input.ElectionType.Equals(Constants.ProcesarPrimeraV) ? _config.Api.First : _config.Api.Second;

            FunctionalHandler.WriteLines(new string[]
            {
                Messages.DOUBLE_LINE(),
                Messages.DOUBLE_LINE(),
                Messages.READING_DATA
            });
            ReadData();
            if (_config.SaveData) SaveData();
        }
        public static void ReadData()
        {
            ReadUbigeoData();
            FunctionalHandler.ExecuteActionIf(
                () =>
                {
                    _dis = TDistritos;
                    ReadElectionData();
                },
                () =>
                {
                    return _input.ProcessType.Equals(Constants.ProcesoTotal);
                },
                () =>
                {
                    FunctionalHandler.ExecuteActionIf(
                        () =>
                        {
                            ReadElectionDataByUbigeo();
                        },
                        () =>
                        {
                            return _input.PartialType.Equals(Constants.ProcesoUbigeo);
                        },
                        () =>
                        {
                            ReadElectionDataByTable();                                                        
                        }
                    );
                }
            );
        }
        private static void ReadUbigeoData()
        {
            var _ubigeos = ReadApiData<Ubigeo>(_endPointSet.Ubigeo);
            TDistritos = _ubigeos.Ubigeos.Nacional.Districts.Concat(_ubigeos.Ubigeos.Extranjero.States).OrderBy(o => o.CDGO_DIST).ToList();
            TProvincias = _ubigeos.Ubigeos.Nacional.Provinces.Concat(_ubigeos.Ubigeos.Extranjero.Countries).OrderBy(o => o.CDGO_PROV).ToList();
            TDepartamentos = _ubigeos.Ubigeos.Nacional.Departments.Concat(_ubigeos.Ubigeos.Extranjero.Continents).OrderBy(o => o.CDGO_DEP).ToList();
        }
        private static void ReadElectionData()
        {
            int actualItem = 0;
            _dis.ForEach(d =>
            {
                actualItem++;
                var pro_act = TProvincias.Find(p => p.CDGO_PROV.Equals(d.CDGO_PADRE));
                var dep_act = TDepartamentos.Find(d => d.CDGO_DEP.Equals(pro_act.CDGO_PADRE));
                _etq_base = Path.Combine(dep_act.DESC_DEP, pro_act.DESC_PROV, d.DESC_DIST);
                FunctionalHandler.WriteLines(new string[]
                {
                    string.Format(Messages.READING, _etq_base)
                });
                _appendName = string.Empty;
                Local locales = ReadApiData<Local>(_endPointSet.Locale.Replace(_config.Api.RequestParameters.UbigeoCode, d.CDGO_DIST));
                _locales.AddRange(locales.Locales);
                locales.Locales.ForEach(l =>
                {
                    FunctionalHandler.WriteLines(new string[]
                    {
                        string.Concat(Constants.TAB_KEY, l.TNOMB_LOCAL)
                    });
                    _appendName = FunctionalHandler.FormatFileName(l.TNOMB_LOCAL);
                    Mesa mesas = ReadApiData<Mesa>(_endPointSet.Table.Replace(_config.Api.RequestParameters.UbigeoCode, d.CDGO_DIST).Replace(_config.Api.RequestParameters.LocaleCode, l.CCODI_LOCAL));
                    _mesas.AddRange(mesas.MesasVotacion);
                    
                    mesas.MesasVotacion.ForEach(m =>
                    {
                        TMesas.Add(new TMesa {
                            Local_codigo = l.CCODI_LOCAL, Local_ubigeo = l.CCODI_UBIGEO, Mesa_numero = m.NUMMESA, Mesa_imagen = m.IMAGEN, Mesa_procesado = m.PROCESADO,
                            Eleccion = _input.ElectionType
                        });
                        FunctionalHandler.WriteLines(new string[]
                        {
                            string.Concat(Constants.TAB_KEY, Constants.TAB_KEY, m.NUMMESA)
                        });
                        _appendName = string.Concat(FunctionalHandler.FormatFileName(l.TNOMB_LOCAL), "_Mesa Nro ", m.NUMMESA);
                        MesaDetalle mesaDetalle = ReadApiData<MesaDetalle>(_endPointSet.TableDetail.Replace(_config.Api.RequestParameters.TableCode, m.NUMMESA));
                        _mesaDetalles.Add(mesaDetalle);

                        if (_config.DownloadFiles)
                        {
                            DescargarActas(mesaDetalle, m, l);                           
                        }

                    });
                });
                MostrarPorcentajeAvance(actualItem, _dis.Count, _etq_base);
            });
        }
        private static void ReadElectionDataByUbigeo()
        {
            _input.UbigeoCode = _input.UbigeoCode.Trim();
            FunctionalHandler.ExecuteActionIf(
                ()  =>
                {
                    static string checkDistrict() => _input.UbigeoCode.EndsWith("00") ? _input.UbigeoCode.Substring(0, 4) : _input.UbigeoCode;

                    _input.UbigeoCode = _input.UbigeoCode.EndsWith("0000") ? _input.UbigeoCode.Substring(0, 2) : checkDistrict();

                    _dis = TDistritos.Where(o => o.CDGO_DIST.Contains(_input.UbigeoCode.Trim())).ToList();
                },
                () =>
                {
                    return _input.UbigeoCode.Length.Equals(6);
                },
                () =>
                {
                    FunctionalHandler.WriteLines(new string[]
                    {
                        Messages.DOUBLE_LINE(),
                        Messages.UBIGEO_NOT_FOUND,
                        Messages.DOUBLE_LINE()
                    });
                }
            );
            FunctionalHandler.ExecuteActionIf(
                () =>
                {
                    ReadElectionData();
                },
                () =>
                {
                    return _dis.Any();
                },
                () =>
                {
                    FunctionalHandler.WriteLines(new string[]
                    {
                        Messages.DOUBLE_LINE(),
                        Messages.UBIGEO_NOT_FOUND,
                        Messages.DOUBLE_LINE()
                    });
                }
            );
        }
        private static void ReadElectionDataByTable()
        {
            _etq_base = "Consulta_por_mesa";
            _appendName = string.Concat("_Mesa Nro ", _input.TableNumber);
            var tableDetail = ReadApiData<MesaDetalle>(_endPointSet.TableDetail.Replace(_config.Api.RequestParameters.TableCode, _input.TableNumber));
            FunctionalHandler.ExecuteActionIf(
                () =>
                {
                    FunctionalHandler.WriteLines(new string[]
                    {
                        string.Format(Messages.READING, string.Format(Messages.TABLE_NUMBER, _input.TableNumber))
                    });
                    _mesaDetalles.Add(tableDetail);
                    ReadApiData<Local>(_endPointSet.Locale.Replace(_config.Api.RequestParameters.UbigeoCode, tableDetail.Procesos.GeneralPre.Presidencial.CCODI_UBIGEO))
                    .Locales.ForEach(l => {
                        if (!_mesas.Any())
                        {
                            MesasVotacion table = ReadApiData<Mesa>(
                                _endPointSet.Table
                                    .Replace(_config.Api.RequestParameters.UbigeoCode, tableDetail.Procesos.GeneralPre.Presidencial.CCODI_UBIGEO)
                                    .Replace(_config.Api.RequestParameters.LocaleCode, l.CCODI_LOCAL)
                                ).MesasVotacion.Find(m => m.NUMMESA.Equals(_input.TableNumber));
                            if (table != null)
                            {
                                _mesas.Add(table);
                                _locales.Add(l);

                                if(_config.DownloadFiles) DescargarActas(tableDetail, table, l);

                                TMesas.Add(new TMesa
                                {
                                    Local_codigo = l.CCODI_LOCAL,
                                    Local_ubigeo = l.CCODI_UBIGEO,
                                    Mesa_numero = table.NUMMESA,
                                    Mesa_imagen = table.IMAGEN,
                                    Mesa_procesado = table.PROCESADO,
                                    Eleccion = _input.ElectionType
                                });
                                if (tableDetail.Procesos.GeneralPre != null)
                                {
                                    TActas.Add(new TActa
                                    {
                                        Acta_imagen = tableDetail.Procesos.GeneralPre.ImageActa,
                                        Acta_numero = tableDetail.Procesos.GeneralPre.Presidencial.CCOPIA_ACTA,
                                        Habiles_numero = tableDetail.Procesos.GeneralPre.Presidencial.NNUME_HABILM,
                                        Votantes_numero = tableDetail.Procesos.GeneralPre.Presidencial.TOT_CIUDADANOS_VOTARON,
                                        Mesa_numero = table.NUMMESA,
                                        Eleccion = _input.ElectionType,
                                        Tipo_proceso = "PRE"
                                    });
                                    TVotos.AddRange(tableDetail.Procesos.GeneralPre.Votos.Select(v => new TVoto
                                    {
                                        Mesa_numero = table.NUMMESA,
                                        Acta_numero = tableDetail.Procesos.GeneralPre.Presidencial.CCOPIA_ACTA,
                                        Auto_nombre = string.IsNullOrEmpty(v.AUTORIDAD) ? string.Empty : v.AUTORIDAD,
                                        Lista_numero = string.IsNullOrEmpty(v.NLISTA) ? string.Empty : v.NLISTA,
                                        Votos_total = v.Congresal,
                                        Eleccion = _input.ElectionType,
                                        Tipo_proceso = "PRE"
                                    }));
                                }
                                if (tableDetail.Procesos.GeneralCon != null)
                                {
                                    TActas.Add(new TActa
                                    {
                                        Acta_imagen = tableDetail.Procesos.GeneralCon.ImageActa,
                                        Acta_numero = tableDetail.Procesos.GeneralCon.Congresal.CCOPIA_ACTA,
                                        Habiles_numero = tableDetail.Procesos.GeneralCon.Congresal.NNUME_HABILM,
                                        Votantes_numero = tableDetail.Procesos.GeneralCon.Congresal.TOT_CIUDADANOS_VOTARON,
                                        Mesa_numero = table.NUMMESA,
                                        Eleccion = _input.ElectionType,
                                        Tipo_proceso = "CON"
                                    });
                                    TVotos.AddRange(tableDetail.Procesos.GeneralCon.Votos.Select(v => new TVoto
                                    {
                                        Mesa_numero = table.NUMMESA,
                                        Acta_numero = tableDetail.Procesos.GeneralCon.Congresal.CCOPIA_ACTA,
                                        Auto_nombre = string.IsNullOrEmpty(v.AUTORIDAD) ? string.Empty : v.AUTORIDAD,
                                        Lista_numero = string.IsNullOrEmpty(v.NLISTA) ? string.Empty : v.NLISTA,
                                        Votos_total = v.Congresal,
                                        Eleccion = _input.ElectionType,
                                        Tipo_proceso = "CON"
                                    }));
                                }
                                if (tableDetail.Procesos.GeneralPar != null)
                                {
                                    TActas.Add(new TActa
                                    {
                                        Acta_imagen = tableDetail.Procesos.GeneralPar.ImageActa,
                                        Acta_numero = tableDetail.Procesos.GeneralPar.Parlamento.CCOPIA_ACTA,
                                        Habiles_numero = tableDetail.Procesos.GeneralPar.Parlamento.NNUME_HABILM,
                                        Votantes_numero = tableDetail.Procesos.GeneralPar.Parlamento.TOT_CIUDADANOS_VOTARON,
                                        Mesa_numero = table.NUMMESA,
                                        Eleccion = _input.ElectionType,
                                        Tipo_proceso = "PAR"
                                    });
                                    TVotos.AddRange(tableDetail.Procesos.GeneralPar.Votos.Select(v => new TVoto
                                    {
                                        Mesa_numero = table.NUMMESA,
                                        Acta_numero = tableDetail.Procesos.GeneralPar.Parlamento.CCOPIA_ACTA,
                                        Auto_nombre = string.IsNullOrEmpty(v.AUTORIDAD) ? string.Empty : v.AUTORIDAD,
                                        Lista_numero = string.IsNullOrEmpty(v.NLISTA) ? string.Empty : v.NLISTA,
                                        Votos_total = v.Congresal,
                                        Eleccion = _input.ElectionType,
                                        Tipo_proceso = "PAR"
                                    }));
                                }
                            }
                        }
                    });
                },
                () =>
                {
                    return tableDetail != null;
                },
                () =>
                {
                    FunctionalHandler.WriteLines(new string[]
                    {
                        Messages.DOUBLE_LINE(),
                        Messages.TABLE_NOT_FOUND,
                        Messages.DOUBLE_LINE()
                    });
                }
            );

        }
        private static T ReadApiData<T>(string method)
        {
            var json = HttpHandler.SendApiRequest(_endPointSet.BaseUri + method).Result;
            var pObject = JsonToObject<T>(json);
            if (_config.SaveJson) GuardarJSON(json, pObject.GetType().Name);
            return pObject;
        }        
        private static void GuardarJSON(string json, string nombre)
        {
            var archivo_nombre = $"{nombre.Trim()}{( string.IsNullOrEmpty(_appendName) ? "" : $"_{_appendName}")}{_config.JsonFileExtension}"; //  string.Concat(nombre.Trim(), "_", _appendName, _config.JsonFileExtension);
            var ruta_guardar = Path.Combine(_config.SavePath, _endPointSet.Title, "JSON", string.IsNullOrEmpty(_etq_base) ? string.Empty : _etq_base);
            if (!Directory.Exists(ruta_guardar)) Directory.CreateDirectory(ruta_guardar);
            File.WriteAllText(Path.Combine(ruta_guardar, archivo_nombre), json);
        }
        private static TValue JsonToObject<TValue>(string json)
        {
            JObject _jobject = JObject.Parse(json);
            TValue _object = _jobject.ToObject<TValue>();
            return _object;
        }
        private static void MostrarPorcentajeAvance(decimal index, decimal total, string etiqueta)
        {
            decimal avance = index * 100 / total;
            Console.Title = string.Format("{0}% - {1}", decimal.Round(avance, 2).ToString(), etiqueta);
        }
        private static void SaveData()
        {
            FormatDataToSave();
            DataConnection.ConnectionStart(_config.DataBaseName);
            FunctionalHandler.ExecuteActionIf(
                () =>
                {
                    DataConnection.PurgeAllData(_input.ElectionType);
                },
                () =>
                {
                    return _input.ProcessType.Equals(Constants.ProcesoTotal);
                },
                () =>
                {
                    DataConnection.PurgeUbigeoData(_input.ElectionType);
                    FunctionalHandler.ExecuteActionIf(
                        () =>
                        {
                            DataConnection.PurgeDataByUbigeo(_input.ElectionType, _input.UbigeoCode);
                        },
                        () =>
                        {
                            return _input.PartialType.Equals(Constants.ProcesoUbigeo);
                        },
                        () =>
                        {
                            DataConnection.PurgeDataByTable(_input.ElectionType, _input.TableNumber);
                        }
                    );
                }
            );
            DataConnection.SaveToTable(TUbigeos, "insert_ubigeo");
            DataConnection.SaveToTable(TLocales, "insert_local");
            DataConnection.SaveToTable(TMesas, "insert_mesa");
            DataConnection.SaveToTable(TActas, "insert_acta");
            DataConnection.SaveToTable(TVotos, "insert_voto");
            DataConnection.MergeWithMaster();
        }
        private static void FormatDataToSave()
        {
            TUbigeos.AddRange(
                TDepartamentos.Select(d =>
                new TUbigeo
                {
                    Ubigeo_codigo = d.CDGO_DEP,
                    Ubigeo_descripcion = d.DESC_DEP,
                    Ubigeo_padre = d.CDGO_PADRE,
                    Nivel = 1,
                    Ambito = d.CDGO_DEP.StartsWith('9') ? "E" : "P",
                    Eleccion = _input.ElectionType
                }).Concat(
                TProvincias.Select(d =>
                new TUbigeo
                {
                    Ubigeo_codigo = d.CDGO_PROV,
                    Ubigeo_descripcion = d.DESC_PROV,
                    Ubigeo_padre = d.CDGO_PADRE,
                    Nivel = 1,
                    Ambito = d.CDGO_PROV.StartsWith('9') ? "E" : "P",
                    Eleccion = _input.ElectionType
                }).Concat(
                TDistritos.Select(d =>
                new TUbigeo
                {
                    Ubigeo_codigo = d.CDGO_DIST,
                    Ubigeo_descripcion = d.DESC_DIST,
                    Ubigeo_padre = d.CDGO_PADRE,
                    Nivel = 1,
                    Ambito = d.CDGO_DIST.StartsWith('9') ? "E" : "P",
                    Eleccion = _input.ElectionType
                }))));
            TLocales = _locales.Select(l => new TLocal
            {
                Local_codigo = l.CCODI_LOCAL,
                Local_direccion = l.TDIRE_LOCAL,
                Local_nombre = l.TNOMB_LOCAL,
                Local_ubigeo = l.CCODI_UBIGEO,
                Eleccion = _input.ElectionType
            }).ToList();
        }
        private static void DescargarActas(MesaDetalle mesaDetalle, MesasVotacion m, Locale l)
        {
            FunctionalHandler.ExecuteParallelActions(new List<Action>()
            {
                () =>
                {
                    if (mesaDetalle.Procesos.GeneralPre != null)
                    {
                        _ = HttpHandler.DownloadFile(mesaDetalle.Procesos.GeneralPre.ImageActa.Replace(string.Format("{0}-actas-resultados-prod.s3.amazonaws.com", _endPointSet.FileStorage), string.Format(_config.FilePath, _endPointSet.Id)),
                            string.Concat("PRE-", m.NUMMESA, ".pdf"),
                                Path.Combine(_config.SavePath, _endPointSet.Title, "ACTAS", _etq_base),
                                FunctionalHandler.FormatFileName(l.TNOMB_LOCAL));
                    }
                },
                () =>
                {
                    if (mesaDetalle.Procesos.GeneralCon != null)
                    {
                        _ = HttpHandler.DownloadFile(mesaDetalle.Procesos.GeneralCon.ImageActa.Replace(string.Format("{0}-actas-resultados-prod.s3.amazonaws.com", _endPointSet.FileStorage), string.Format(_config.FilePath, _endPointSet.Id)),
                            string.Concat("CON-", m.NUMMESA, ".pdf"), Path.Combine(_config.SavePath, "{0}"),
                                Path.Combine(_etq_base, FunctionalHandler.FormatFileName(l.TNOMB_LOCAL)));
                    }
                },
                () =>
                {
                    if (mesaDetalle.Procesos.GeneralPar != null)
                    {
                        _ = HttpHandler.DownloadFile(mesaDetalle.Procesos.GeneralPar.ImageActa.Replace(string.Format("{0}-actas-resultados-prod.s3.amazonaws.com", _endPointSet.FileStorage), string.Format(_config.FilePath, _endPointSet.Id)),
                            string.Concat("PAR-", m.NUMMESA, ".pdf"), Path.Combine(_config.SavePath, "{0}"),
                                Path.Combine(_etq_base, FunctionalHandler.FormatFileName(l.TNOMB_LOCAL)));
                    }
                }
            });
        }
    }
}
