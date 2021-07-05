using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;

namespace PE_Scrapping.Funciones
{
    public static class DataConnection
    {
        private static bool IsDbRecentlyCreated = false;
        private static string _dbTemp;
        private static string _dbMaster;
        public static void ConnectionStart(string DBName)
        {
            _dbMaster = DBName;
            CheckDataBaseFile(_dbMaster);
            _dbTemp = GetDataBaseName(DBName);
            CheckDataBaseFile(_dbTemp);
        }
        private static void CheckDataBaseFile(string path)
        {
            if (!File.Exists(Path.GetFullPath(path)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                SQLiteConnection.CreateFile(path);
                IsDbRecentlyCreated = true;
            }
            if (IsDbRecentlyCreated)
            {
                using (var ctx = GetInstance(path))
                {
                    using (var command = new SQLiteCommand(ReadQuery("create_db_objects"), ctx))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
        }
        private static string GetDataBaseName(string database_path)
        {
            var database_name = string.Concat(Path.GetFileName(database_path), ".", Guid.NewGuid().ToString());
            return Path.Combine(Path.GetDirectoryName(database_path), database_name);
        }
        private static SQLiteConnection GetInstance(string DBName)
        {
            var db = new SQLiteConnection(
                string.Format("Data Source={0};Version=3;", DBName)
            );
            db.Open();
            return db;
        }
        private static string ReadQuery(string query_name)
        {
            var query_path = string.Concat(@"Script\SQLite\", query_name, ".sql");
            var query = "";
            var line = "";
            using (var reader = new StreamReader(Path.GetFullPath(query_path)))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    query += line;
                }
            }
            return query;
        }
        public static void SaveToTable<TEntity>(List<TEntity> records, string queryName)
        {
            using var db = GetInstance(_dbTemp);
            using var tx = db.BeginTransaction();
            records.ForEach(r =>
            {
                using var cmd = SetCommand(r, queryName, db);
                cmd.ExecuteNonQuery();
            });
            tx.Commit();
        }
        public static SQLiteCommand SetCommand<TEntity>(TEntity entity, string queryName, SQLiteConnection connection)
        {
            SQLiteCommand command = new(connection);
            command.CommandText = ReadQuery(queryName);
            entity.GetType().GetProperties().ToList().ForEach(p =>
            {
                command.Parameters.Add(new SQLiteParameter("@" + p.Name, p.GetValue(entity).ToString()));
            });
            return command;
        }
        public static void PurgeAllData(string opcion)
        {
            using (var db = GetInstance(_dbTemp))
            {
                using (var cmd = new SQLiteCommand(db))
                {
                    cmd.CommandText = ReadQuery("purge_data");
                    using (var transaction = db.BeginTransaction())
                    {
                        cmd.Parameters.Add(new SQLiteParameter("@eleccion", opcion));
                        cmd.ExecuteNonQuery();
                        transaction.Commit();
                    }
                }
            }
        }
        public static void PurgeUbigeoData(string opcion)
        {
            using (var db = GetInstance(_dbTemp))
            {
                using (var cmd = new SQLiteCommand(db))
                {
                    cmd.CommandText = ReadQuery("purge_data_ubigeo");
                    using (var transaction = db.BeginTransaction())
                    {
                        cmd.Parameters.Add(new SQLiteParameter("@eleccion", opcion));
                        cmd.ExecuteNonQuery();
                        transaction.Commit();
                    }
                }
            }
        }
        public static void PurgeDataByTable(string opcion, string mesa_numero)
        {
            using (var db = GetInstance(_dbTemp))
            {
                using (var cmd = new SQLiteCommand(db))
                {
                    cmd.CommandText = ReadQuery("purge_data_mesa");
                    using (var transaction = db.BeginTransaction())
                    {
                        cmd.Parameters.Add(new SQLiteParameter("@eleccion", opcion));
                        cmd.Parameters.Add(new SQLiteParameter("@mesa_numero", mesa_numero));
                        cmd.ExecuteNonQuery();
                        transaction.Commit();
                    }
                }
            }
        }
        public static void PurgeDataByUbigeo(string opcion, string mesa_numero)
        {
            string level = string.Empty;
            using (var db = GetInstance(_dbTemp))
            {
                using (var cmd = new SQLiteCommand(db))
                {
                    cmd.CommandText = ReadQuery("get_level");
                    cmd.Parameters.Add(new SQLiteParameter("@codigo_ubigeo", mesa_numero));
                    object result = cmd.ExecuteScalar();
                    level = (result == null ? string.Empty : result.ToString());

                    cmd.Parameters.Clear();
                    cmd.CommandText = ReadQuery("purge_data_detalle_ubigeo");
                    using (var transaction = db.BeginTransaction())
                    {
                        cmd.Parameters.Add(new SQLiteParameter("@level", level));
                        cmd.Parameters.Add(new SQLiteParameter("@eleccion", opcion));
                        cmd.ExecuteNonQuery();
                        transaction.Commit();
                    }
                }
            }
        }
        public static void MergeWithMaster()
        {
            try
            {
                using (var db = GetInstance(_dbMaster))
                {
                    using (var cmd = new SQLiteCommand(db))
                    {
                        cmd.CommandText = "ATTACH DATABASE '" + _dbMaster + "' AS firstDB;";
                        cmd.ExecuteNonQuery();

                        cmd.CommandText = "ATTACH DATABASE '" + _dbTemp + "' AS secondDB";
                        cmd.ExecuteNonQuery();

                        cmd.CommandText = "INSERT OR REPLACE INTO firstDB.pe_Ubigeos ("
                            + "ubigeo_codigo, ubigeo_descripcion, ubigeo_padre, eleccion, nivel, ambito) "
                            + "SELECT ubigeo_codigo, ubigeo_descripcion, ubigeo_padre, eleccion, nivel, ambito "
                            + "FROM secondDB.pe_Ubigeos";
                        cmd.ExecuteNonQuery();

                        cmd.CommandText = "INSERT OR REPLACE INTO firstDB.pe_Locales ("
                            + "local_codigo, local_ubigeo, local_nombre, local_direccion, eleccion) "
                            + "SELECT local_codigo, local_ubigeo, local_nombre, local_direccion, eleccion "
                            + "FROM secondDB.pe_Locales";
                        cmd.ExecuteNonQuery();

                        cmd.CommandText = "INSERT OR REPLACE INTO firstDB.pe_Mesas ("
                            + "local_ubigeo, local_codigo, mesa_numero, mesa_procesado, mesa_imagen, eleccion) "
                            + "SELECT local_ubigeo, local_codigo, mesa_numero, mesa_procesado, mesa_imagen, eleccion "
                            + "FROM secondDB.pe_Mesas";
                        cmd.ExecuteNonQuery();

                        cmd.CommandText = "INSERT OR REPLACE INTO firstDB.pe_Actas ("
                            + "mesa_numero, acta_numero, acta_imagen, habiles_numero, votantes_numero, eleccion, tipo_proceso) "
                            + "SELECT mesa_numero, acta_numero, acta_imagen, habiles_numero, votantes_numero, eleccion, tipo_proceso "
                            + "FROM secondDB.pe_Actas";
                        cmd.ExecuteNonQuery();

                        cmd.CommandText = "INSERT OR REPLACE INTO firstDB.pe_Votos ("
                            + "mesa_numero, acta_numero, auto_nombre, lista_numero, votos_total, eleccion, tipo_proceso) "
                            + "SELECT mesa_numero, acta_numero, auto_nombre, lista_numero, votos_total, eleccion, tipo_proceso "
                            + "FROM secondDB.pe_Votos";
                        cmd.ExecuteNonQuery();
                    }
                }
                GC.Collect();
                GC.WaitForPendingFinalizers();
                File.Delete(_dbTemp);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
