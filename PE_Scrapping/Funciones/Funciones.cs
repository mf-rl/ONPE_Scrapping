using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PE_Scrapping.Entidades;

namespace PE_Scrapping.Funciones
{
    public class Funciones
    {
        static void ObtenerDataUbigeo()
        {
            //driver = new ChromeDriver("runtimes");
            //driver.Manage().Window.Minimize();

            //List<Ubigeo> ambito = GetItems(1, "cod_ambito", string.Concat(uri_base, "T"));
            //SaveUbigeo(ambito);
            //Console.WriteLine(string.Format("Guardado: Ambito(s) -> {0}", ambito.Count.ToString()));
            //ambito.ForEach(a =>
            //{
            //    Console.WriteLine(string.Format("Procesando {0}", a.ubigeo_dc));
            //    List<Ubigeo> departamentos = GetItems(2, "cod_depa", string.Concat(uri_base, a.ubigeo_id));
            //    SaveUbigeo(departamentos);
            //    Console.WriteLine(string.Format("Guardado: Departamento(s) -> {0}", departamentos.Count.ToString()));
            //    departamentos.ForEach(d =>
            //    {
            //        Console.WriteLine(string.Format("Procesando {0} - {1}", a.ubigeo_dc, d.ubigeo_dc));
            //        List<Ubigeo> provincia = GetItems(3, "cod_prov", string.Concat(uri_base, a.ubigeo_id, "/", d.ubigeo_id));
            //        SaveUbigeo(provincia);
            //        Console.WriteLine(string.Format("Guardado: Provincia(s) -> {0}", provincia.Count.ToString()));
            //        provincia.ForEach(p =>
            //        {
            //            Console.WriteLine(string.Format("Procesando {0} - {1} - {2}", a.ubigeo_dc, d.ubigeo_dc, p.ubigeo_dc));
            //            List<Ubigeo> distrito = GetItems(4, "cod_dist", string.Concat(uri_base, a.ubigeo_id, "/", d.ubigeo_id, "/", p.ubigeo_id));
            //            SaveUbigeo(distrito);
            //            Console.WriteLine(string.Format("Guardado: Distrito(s) -> {0}", distrito.Count.ToString()));
            //            distrito.ForEach(dd =>
            //            {
            //                Console.WriteLine(string.Format("Procesando {0} - {1} - {2} - {3}", a.ubigeo_dc, d.ubigeo_dc, p.ubigeo_dc, dd.ubigeo_dc));
            //                List<Ubigeo> local = GetItems(5, "cod_local", string.Concat(uri_base, a.ubigeo_id, "/", d.ubigeo_id, "/", p.ubigeo_id, "/", dd.ubigeo_id));
            //                SaveUbigeo(local);
            //                //Console.WriteLine(string.Format("Guardado: Local(s) -> {0}", local.Count.ToString()));
            //                //local.ForEach(l =>
            //                //{
            //                //    Console.WriteLine(string.Format("Procesando {0} - {1} - {2} - {3} - {4}", a.ubigeo_dc, d.ubigeo_dc, p.ubigeo_dc, dd.ubigeo_dc, l.ubigeo_dc));
            //                //    List<Mesa> tables = GetTables(string.Concat(uri_base, a.ubigeo_id, "/", d.ubigeo_id, "/", p.ubigeo_id, "/", dd.ubigeo_id, "/", l.ubigeo_id));
            //                //    //ProcessTables(tables, string.Concat(uri_base, a.ubigeo_id, "/", d.ubigeo_id, "/", p.ubigeo_id, "/", dd.ubigeo_id, "/", l.ubigeo_id, "/"));
            //                //    tables.ForEach(t =>
            //                //    {
            //                //        Console.WriteLine(string.Format("Procesando {0} - {1} - {2} - {3} - {4} / Mesa {5}", a.ubigeo_dc, d.ubigeo_dc, p.ubigeo_dc, dd.ubigeo_dc, l.ubigeo_dc, t.Numero));
            //                //        Mesa data = GetTableData(
            //                //            string.Concat(uri_base, a.ubigeo_id, "/", d.ubigeo_id, "/", p.ubigeo_id, "/", dd.ubigeo_id, "/", l.ubigeo_id, "/", t.Numero),
            //                //            t,
            //                //            a.ubigeo_id,
            //                //            d.ubigeo_id,
            //                //            p.ubigeo_id,
            //                //            dd.ubigeo_id,
            //                //            l.ubigeo_id
            //                //        );
            //                //        SaveMesa(data);
            //                //        Console.WriteLine(string.Format("Mesa {0} guardada.", data.Numero));
            //                //    });
            //                //});
            //            });
            //        });
            //    });
            //});

            //driver.Close();
            //Console.WriteLine("Finalizado. :)");
            //Console.ReadKey();
        }
    }
}
