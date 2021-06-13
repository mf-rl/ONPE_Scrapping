using PE_Scrapping.Entidades;
using System.Data;
using System.Data.SqlClient;

namespace PE_Scrapping.Funciones
{
    public class Transacciones
    {
        string _connectionString = string.Empty;
        public Transacciones(string ConnectionString)
        {
            _connectionString = ConnectionString;
        }
        public void GuardarLocales(Local locales)
        {
            DataTable tbl = new();
            tbl.Columns.Add(new DataColumn("local_codigo", typeof(string)));
            tbl.Columns.Add(new DataColumn("local_ubigeo", typeof(string)));
            tbl.Columns.Add(new DataColumn("local_nombre", typeof(string)));
            tbl.Columns.Add(new DataColumn("local_direccion", typeof(string)));

            locales.locales.ForEach(o =>
            {
                DataRow dr = tbl.NewRow();
                dr["local_codigo"] = o.CCODI_LOCAL;
                dr["local_ubigeo"] = o.CCODI_UBIGEO;
                dr["local_nombre"] = o.TNOMB_LOCAL;
                dr["local_direccion"] = o.TDIRE_LOCAL;
                tbl.Rows.Add(dr);
            });

            SqlConnection con = new(_connectionString);

            SqlBulkCopy objbulk = new(con);

            objbulk.DestinationTableName = "pe_Locales";
            objbulk.ColumnMappings.Add("local_codigo", "local_codigo");
            objbulk.ColumnMappings.Add("local_ubigeo", "local_ubigeo");
            objbulk.ColumnMappings.Add("local_nombre", "local_nombre");
            objbulk.ColumnMappings.Add("local_direccion", "local_direccion");

            con.Open();

            objbulk.WriteToServer(tbl);
            con.Close();
        }
        public void GuardarMesas(Mesa mesas, string codigo_local, string codigo_ubigeo)
        {
            DataTable tbl = new();
            tbl.Columns.Add(new DataColumn("local_ubigeo", typeof(string)));
            tbl.Columns.Add(new DataColumn("local_codigo", typeof(string)));
            tbl.Columns.Add(new DataColumn("mesa_numero", typeof(string)));
            tbl.Columns.Add(new DataColumn("mesa_procesado", typeof(int)));
            tbl.Columns.Add(new DataColumn("mesa_imagen", typeof(string)));

            mesas.mesasVotacion.ForEach(o =>
            {
                DataRow dr = tbl.NewRow();
                dr["local_ubigeo"] = codigo_ubigeo;
                dr["local_codigo"] = codigo_local;
                dr["mesa_numero"] = o.NUMMESA;
                dr["mesa_procesado"] = o.PROCESADO;
                dr["mesa_imagen"] = o.IMAGEN;
                tbl.Rows.Add(dr);
            });

            SqlConnection con = new(_connectionString);

            SqlBulkCopy objbulk = new(con);

            objbulk.DestinationTableName = "pe_Mesas";
            objbulk.ColumnMappings.Add("local_ubigeo", "local_ubigeo");
            objbulk.ColumnMappings.Add("local_codigo", "local_codigo");
            objbulk.ColumnMappings.Add("mesa_numero", "mesa_numero");
            objbulk.ColumnMappings.Add("mesa_procesado", "mesa_procesado");
            objbulk.ColumnMappings.Add("mesa_imagen", "mesa_imagen");

            con.Open();

            objbulk.WriteToServer(tbl);
            con.Close();
        }
        public void GuardarMesaDetalle(MesaDetalle mesaDetalle, string numero_mesa)
        {
            SqlConnection con = new(_connectionString);
            SqlBulkCopy objbulk = new(con);
            con.Open();

            DataTable tbl_acta = new();
            tbl_acta.Columns.Add(new DataColumn("mesa_numero", typeof(string)));
            tbl_acta.Columns.Add(new DataColumn("acta_numero", typeof(string)));
            tbl_acta.Columns.Add(new DataColumn("acta_imagen", typeof(string)));
            tbl_acta.Columns.Add(new DataColumn("habiles_numero", typeof(int)));
            tbl_acta.Columns.Add(new DataColumn("votantes_numero", typeof(int)));            
            DataRow dr_acta = tbl_acta.NewRow();
            dr_acta["mesa_numero"] = numero_mesa;
            dr_acta["acta_numero"] = mesaDetalle.procesos.generalPre.presidencial.CCOPIA_ACTA;
            dr_acta["acta_imagen"] = mesaDetalle.procesos.generalPre.imageActa;
            dr_acta["habiles_numero"] = mesaDetalle.procesos.generalPre.presidencial.NNUME_HABILM;
            dr_acta["votantes_numero"] = mesaDetalle.procesos.generalPre.presidencial.TOT_CIUDADANOS_VOTARON;
            tbl_acta.Rows.Add(dr_acta);
            objbulk.DestinationTableName = "pe_Actas";
            objbulk.ColumnMappings.Add("mesa_numero", "mesa_numero");
            objbulk.ColumnMappings.Add("acta_numero", "acta_numero");
            objbulk.ColumnMappings.Add("acta_imagen", "acta_imagen");
            objbulk.ColumnMappings.Add("habiles_numero", "habiles_numero");
            objbulk.ColumnMappings.Add("votantes_numero", "votantes_numero");
            objbulk.WriteToServer(tbl_acta);

            objbulk = new(con);
            DataTable tbl_votos = new();
            tbl_votos.Columns.Add(new DataColumn("mesa_numero", typeof(string)));
            tbl_votos.Columns.Add(new DataColumn("acta_numero", typeof(string)));
            tbl_votos.Columns.Add(new DataColumn("auto_nombre", typeof(string)));
            tbl_votos.Columns.Add(new DataColumn("lista_numero", typeof(int)));
            tbl_votos.Columns.Add(new DataColumn("votos_total", typeof(int)));
            mesaDetalle.procesos.generalPre.votos.ForEach(v =>
            {
                DataRow dr_voto = tbl_votos.NewRow();
                dr_voto["mesa_numero"] = numero_mesa;
                dr_voto["acta_numero"] = mesaDetalle.procesos.generalPre.presidencial.CCOPIA_ACTA;
                dr_voto["auto_nombre"] = v.AUTORIDAD;
                dr_voto["lista_numero"] = string.IsNullOrEmpty(v.NLISTA) ? "0" : v.NLISTA;
                dr_voto["votos_total"] = v.congresal;
                tbl_votos.Rows.Add(dr_voto);
            });
            objbulk.DestinationTableName = "pe_Votos";
            objbulk.ColumnMappings.Add("mesa_numero", "mesa_numero");
            objbulk.ColumnMappings.Add("acta_numero", "acta_numero");
            objbulk.ColumnMappings.Add("auto_nombre", "auto_nombre");
            objbulk.ColumnMappings.Add("lista_numero", "lista_numero");
            objbulk.ColumnMappings.Add("votos_total", "votos_total");
            objbulk.WriteToServer(tbl_votos);
            con.Close();
        }
        public void GuardarUbigeos(Ubigeo ubigeos)
        {
            DataTable tbl = new();
            tbl.Columns.Add(new DataColumn("ubigeo_codigo", typeof(string)));
            tbl.Columns.Add(new DataColumn("ubigeo_descripcion", typeof(string)));
            tbl.Columns.Add(new DataColumn("ubigeo_padre", typeof(string)));

            ubigeos.ubigeos.nacional.departments.ForEach(o =>
            {
                DataRow dr = tbl.NewRow();
                dr["ubigeo_codigo"] = o.CDGO_DEP;
                dr["ubigeo_descripcion"] = o.DESC_DEP;
                dr["ubigeo_padre"] = o.CDGO_PADRE;
                tbl.Rows.Add(dr);
            });
            ubigeos.ubigeos.nacional.provinces.ForEach(o =>
            {
                DataRow dr = tbl.NewRow();
                dr["ubigeo_codigo"] = o.CDGO_PROV;
                dr["ubigeo_descripcion"] = o.DESC_PROV;
                dr["ubigeo_padre"] = o.CDGO_PADRE;
                tbl.Rows.Add(dr);
            });
            ubigeos.ubigeos.nacional.districts.ForEach(o =>
            {
                DataRow dr = tbl.NewRow();
                dr["ubigeo_codigo"] = o.CDGO_DIST;
                dr["ubigeo_descripcion"] = o.DESC_DIST;
                dr["ubigeo_padre"] = o.CDGO_PADRE;
                tbl.Rows.Add(dr);
            });

            ubigeos.ubigeos.extranjero.continents.ForEach(o =>
            {
                DataRow dr = tbl.NewRow();
                dr["ubigeo_codigo"] = o.CDGO_DEP;
                dr["ubigeo_descripcion"] = o.DESC_DEP;
                dr["ubigeo_padre"] = o.CDGO_PADRE;
                tbl.Rows.Add(dr);
            });
            ubigeos.ubigeos.extranjero.countries.ForEach(o =>
            {
                DataRow dr = tbl.NewRow();
                dr["ubigeo_codigo"] = o.CDGO_PROV;
                dr["ubigeo_descripcion"] = o.DESC_PROV;
                dr["ubigeo_padre"] = o.CDGO_PADRE;
                tbl.Rows.Add(dr);
            });
            ubigeos.ubigeos.extranjero.states.ForEach(o =>
            {
                DataRow dr = tbl.NewRow();
                dr["ubigeo_codigo"] = o.CDGO_DIST;
                dr["ubigeo_descripcion"] = o.DESC_DIST;
                dr["ubigeo_padre"] = o.CDGO_PADRE;
                tbl.Rows.Add(dr);
            });

            SqlConnection con = new(_connectionString);

            SqlBulkCopy objbulk = new(con);

            objbulk.DestinationTableName = "pe_Ubigeos";
            objbulk.ColumnMappings.Add("ubigeo_codigo", "ubigeo_codigo");
            objbulk.ColumnMappings.Add("ubigeo_descripcion", "ubigeo_descripcion");
            objbulk.ColumnMappings.Add("ubigeo_padre", "ubigeo_padre");

            con.Open();
            
            objbulk.WriteToServer(tbl);
            con.Close();
        }
        public void LimpiarData()
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("pe_PurgeData", conn)
            {
                CommandType = CommandType.StoredProcedure
            })
            {
                conn.Open();
                command.ExecuteNonQuery();
            }
        }
    }
}
