using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Transactions;

namespace DistDataAcquisition.DAO
{
    public class DatabaseHelper
    {
        //private string _table;
        //private string _columns;
        //private string _values;

        public SqlConnection MyBdConnection { get; set; }
        public string ConnectionString { get; set; }

        public DatabaseHelper()
        {
            this.ConnectionString = Settings.ConnectionString;
            this.MyBdConnection = new SqlConnection(this.ConnectionString);
        }

        public DatabaseHelper(string nomeStringConexao)
        {
            this.ConnectionString = nomeStringConexao;
            this.MyBdConnection = new SqlConnection(this.ConnectionString);
        }

        public static DatabaseHelper Create()
        {
            return new DatabaseHelper();
        }

        public static DatabaseHelper Create(string nomeStringConexao)
        {
            return new DatabaseHelper(nomeStringConexao);
        }

        public int Save<T>(T obj)
        {
            try
            {

                List<SqlParameter> list = this.MountCustomerParameter<T>(obj);

                string table = obj.GetType().Name;
                string values = string.Join(",", list.Select(c => c.ParameterName));
                string columns = values.Replace("@", "");

                string query = string.Format("INSERT INTO {0} ({1}) VALUES ({2})", table, columns, values);

                int id = this.ExecuteScalar(query, list.ToArray());

                return id;
            }
            catch(Exception e)
            {
                throw e;
            }
            
        }

       

        public Task<int> SaveAsync<T>(T obj)
        {
            return Task.Run(() =>
            {
                List<SqlParameter> list = this.MountCustomerParameter<T>(obj);

                string table = obj.GetType().Name;
                string values = string.Join(",", list.Select(c => c.ParameterName));
                string columns = values.Replace("@", "");

                string query = string.Format("INSERT INTO {0} ({1}) VALUES ({2})", table, columns, values);

                //this.ExecuteNonQuery(query, list.ToArray());
                return this.ExecuteScalar(query, list.ToArray());
            });
        }

        public void Update<K, T>(K id, T obj)
        {
            try
            {

                List<SqlParameter> list = this.MountCustomerParameterUpdate<T>(obj);
                string table = obj.GetType().Name;
                string values = string.Empty;

                foreach (SqlParameter c in list)
                {
                    values += string.Format("{0} = {1},", c.ParameterName.Replace("@", ""), c.ParameterName);
                }

                values = values.Remove(values.Length - 1);
                list.Add(this.BuildParameter("id", id));

                string query = string.Format("UPDATE {0} SET {1} WHERE {2} = @id", table, values, table + "ID");

                this.ExecuteNonQuery(query, list.ToArray());
            }
            catch(Exception e)
            {
                throw e;
            }
            
        }

        public void UpdateAsync<K, T>(K id, T obj)
        {
            Task.Run(() =>
            {

                List<SqlParameter> list = this.MountCustomerParameterUpdate<T>(obj);
                string table = obj.GetType().Name;
                string values = string.Empty;

                foreach (SqlParameter c in list)
                {
                    values += string.Format("{0} = {1},", c.ParameterName.Replace("@", ""), c.ParameterName);
                }

                values = values.Remove(values.Length - 1);
                list.Add(this.BuildParameter("id", id));

                string query = string.Format("UPDATE {0} SET {1} WHERE {2} = @id", table, values, table + "ID");

                this.ExecuteNonQuery(query, list.ToArray());
            });
        }

        public void Delete(string query, params SqlParameter[] parameters)
        {
            this.ExecuteNonQuery(query, parameters);
        }

        /// <summary>
        /// Método para Queries sem Join ou ObjetosVO
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public List<T> GetList<T>(string query, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(this.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    command.CommandText = query;
                    command.Parameters.AddRange(parameters);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        return this.DataReaderMapToList<T>(reader);
                    }
                }
            }
        }

        /// <summary>
        /// Método para Queries sem Join ou ObjetosVO Async
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public async Task<List<T>> GetListAsync<T>(string query, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(this.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    await conn.OpenAsync();
                    command.CommandText = query;
                    command.Parameters.AddRange(parameters);
                    using (SqlDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.KeyInfo))
                    {
                        return this.DataReaderMapToList<T>(reader);
                    }
                }
            }
        }

        /// <summary>
        /// Método para Queries com JOIN. Retorna o Objeto Completo
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public List<T> GetCompleteList<T>(string query, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(this.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    command.CommandText = query;
                    command.Parameters.AddRange(parameters);
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.KeyInfo))
                    {
                        return this.DataReaderMapToList<T>(reader, query);
                    }
                }
            }
        }

        public async Task<List<T>> GetCompleteListAsync<T>(string query, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(this.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    await conn.OpenAsync();
                    command.CommandText = query;
                    command.Parameters.AddRange(parameters);

                    using (SqlDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.KeyInfo))
                    {
                        return this.DataReaderMapToList<T>(reader, query);
                    }
                }
            }
        }

        public int? GetInt(string query, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(this.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    command.CommandText = query;
                    command.Parameters.AddRange(parameters);
                    return (int?)command.ExecuteScalar();
                }
            }
        }

        public SqlParameter BuildParameter(string nome, object valor)
        {
            SqlParameter parametro = new SqlParameter(this.GetCorrectParameterName(nome), valor ?? DBNull.Value);
            return parametro;
        }
        public SqlParameter BuildParameter(string nome, object valor, string tipo)
        {
            SqlParameter parametro = new SqlParameter(this.GetCorrectParameterName(nome), valor);
            parametro.DbType = (DbType)Enum.Parse(typeof(DbType), tipo, true); ;
            return parametro;
        }
        public SqlParameter BuildParameter(string nome, object valor, string tipo, int size)
        {
            SqlParameter parametro = new SqlParameter(this.GetCorrectParameterName(nome), valor);
            parametro.DbType = (DbType)Enum.Parse(typeof(DbType), tipo, true); ;
            parametro.Size = size;
            return parametro;
        }

        public async Task ExecuteNonQueryAsync(string query, params SqlParameter[] parameters)
        {
            Exception erro = null;
            try
            {
                using (var ts = new TransactionScope(TransactionScopeOption.Required))
                {
                    using (SqlConnection conn = new SqlConnection(this.ConnectionString))
                    {
                        using (SqlCommand command = new SqlCommand(query, conn))
                        {
                            await conn.OpenAsync();
                            command.CommandText = query;
                            command.Parameters.AddRange(parameters);
                            await command.ExecuteNonQueryAsync();
                        }
                    }

                    ts.Complete();
                }
            }
            catch (Exception ex)
            {
                erro = ex;
            }

            if (erro != null)
            {
                throw erro;
            }
        }

        public void ExecuteNonQuery(string query, params SqlParameter[] parameters)
        {
            Exception erro = null;
            try
            {
                using (var ts = new TransactionScope(TransactionScopeOption.Required))
                {
                    using (SqlConnection conn = new SqlConnection(this.ConnectionString))
                    {
                        using (SqlCommand command = new SqlCommand(query, conn))
                        {
                            conn.Open();
                            command.CommandText = query;
                            if (parameters != null && parameters.Length > 0) command.Parameters.AddRange(parameters);
                            command.ExecuteNonQuery();
                        }
                    }

                    ts.Complete();
                }
            }
            catch (Exception ex)
            {
                erro = ex;
            }

            if (erro != null)
            {
                throw erro;
            }
        }
        public int ExecuteScalar(string query, params SqlParameter[] parameters)
        {
            Exception erro = null;
            Int32 id = 0;
            try
            {
                using (var ts = new TransactionScope(TransactionScopeOption.Required))
                {
                    using (SqlConnection conn = new SqlConnection(this.ConnectionString))
                    {
                        using (SqlCommand command = new SqlCommand(query, conn))
                        {
                            try
                            {
                                conn.Open();
                                command.CommandText = query + ";SELECT CAST(scope_identity() AS int);";
                                command.Parameters.AddRange(parameters);
                                id = (Int32)command.ExecuteScalar();
                            }
                            catch (DbException ex)
                            {
                                throw new InvalidOperationException(ex.Message + " - " + command.CommandText, ex);
                            }
                        }
                    }

                    ts.Complete();
                }
            }
            catch (Exception ex)
            {
                erro = ex;
            }

            if (erro != null)
            {
                throw erro;
            }

            return (int)id;
        }
        public void ExecuteCommands(params SqlCommand[] commands)
        {
            Exception erro = null;
            SqlTransaction trans = null;
            try
            {
                this.MyBdConnection.Open();
                trans = this.MyBdConnection.BeginTransaction();
                for (int i = 0; i < commands.Length; i++)
                {
                    commands[i].Transaction = trans;
                    commands[i].ExecuteNonQuery();
                }
                trans.Commit();
                this.MyBdConnection.Close();
            }
            catch (Exception ex)
            {
                trans.Rollback();
                erro = ex;
            }
            finally
            {
                this.MyBdConnection.Close();
            }

            if (erro != null)
            {
                throw erro;
            }
        }

        public SqlDataReader ExecuteDataReader(string query, params SqlParameter[] parameters)
        {
            SqlCommand command = this.MyBdConnection.CreateCommand();
            command.CommandText = query;
            command.Parameters.AddRange(parameters);
            return command.ExecuteReader();
        }

        private IDataReader DataSetToReader(DataTable dt)
        {
            return dt.CreateDataReader();
        }

        #region Colocar em outra classe

        private List<SqlParameter> MountCustomerParameter<T>(T obj)
        {
            List<SqlParameter> sqlparams = new List<SqlParameter>();

            foreach (PropertyInfo prop in obj.GetType().GetProperties())
            {
                object[] identity = prop.GetCustomAttributes(typeof(Identity), true);
                object[] anotherObject = prop.GetCustomAttributes(typeof(AnotherObject), true);

                if (identity.Length == 0 && anotherObject.Length == 0 && this.GetPropValue(obj, prop.Name) != null)
                {
                    if (prop.PropertyType.Name.Contains("Nullable"))
                    {
                        sqlparams.Add(this.BuildParameter(prop.Name, this.GetPropValue(obj, prop.Name)));
                    }
                    else
                    {
                        sqlparams.Add(this.BuildParameter(prop.Name, this.GetPropValue(obj, prop.Name), prop.PropertyType.Name));
                    }
                }
            }

            return sqlparams;
        }

        private List<SqlParameter> MountCustomerParameterUpdate<T>(T obj)
        {
            List<SqlParameter> sqlparams = new List<SqlParameter>();

            foreach (PropertyInfo prop in obj.GetType().GetProperties())
            {
                object[] identity = prop.GetCustomAttributes(typeof(Identity), true);
                object[] anotherObject = prop.GetCustomAttributes(typeof(AnotherObject), true);

                if (identity.Length == 0 && anotherObject.Length == 0)
                {
                    sqlparams.Add(this.BuildParameter(prop.Name, this.GetPropValue(obj, prop.Name)));
                }
            }

            return sqlparams;
        }

        private List<T> DataReaderMapToList<T>(IDataReader reader)
        {
            List<T> list = new List<T>();

            try
            {
                T obj = default(T);
                while (reader.Read())
                {
                    obj = Activator.CreateInstance<T>();

                    foreach (PropertyInfo prop in obj.GetType().GetProperties())
                    {
                        try { prop.SetValue(obj, reader[prop.Name], null); }
                        catch { }

                    }
                    list.Add(obj);
                }
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }

            return list;
        }

        private List<T> DataReaderMapToListOld<T>(IDataReader reader, string query)
        {
            List<T> list = new List<T>();
            DataTable schemaTable = reader.GetSchemaTable();
            string instance = string.Empty;
            string name = string.Empty;

            try
            {
                while (reader.Read())
                {
                    T obj = default(T);
                    obj = Activator.CreateInstance<T>();
                    Dictionary<string, object> dic = new Dictionary<string, object>();

                    int count = 0;

                    foreach (DataRow myField in schemaTable.Rows)
                    {
                        dic.Add(myField[schemaTable.Columns[11]].ToString() + "." + myField[schemaTable.Columns[0]].ToString(), reader[count]);
                        count++;
                    }

                    foreach (PropertyInfo p in obj.GetType().GetProperties())
                    {
                        PropertyInfo prop = p;

                        object[] another = prop.GetCustomAttributes(typeof(AnotherObject), true);

                        if (another.Length == 0)
                        {
                            instance = prop.DeclaringType.Name;

                            try
                            {
                                name = instance + "." + prop.Name;
                                prop.SetValue(obj, dic[name], null);
                            }
                            catch { }

                        }
                        else
                        {
                            instance = prop.PropertyType.Name;

                            if (query.Contains(instance))
                            {
                                object item = Activator.CreateInstance(Type.GetType(prop.PropertyType.FullName));

                                foreach (PropertyInfo prop2 in item.GetType().GetProperties())
                                {
                                    try
                                    {
                                        name = instance + "." + prop2.Name;
                                        prop2.SetValue(item, dic[name], null);
                                    }
                                    catch { }
                                }

                                prop.SetValue(obj, item);
                            }
                        }
                    }
                    list.Add(obj);

                }


            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }

            return list;
        }

        private List<T> DataReaderMapToList<T>(IDataReader reader, string query)
        {
            List<T> list = new List<T>();
            DataTable schemaTable = reader.GetSchemaTable();
            string instance = string.Empty;
            string name = string.Empty;

            try
            {
                while (reader.Read())
                {
                    T obj = default(T);
                    obj = Activator.CreateInstance<T>();
                    Dictionary<string, object> dic = new Dictionary<string, object>();

                    int count = 0;

                    foreach (DataRow myField in schemaTable.Rows)
                    {

                        string tablename = myField[schemaTable.Columns[11]].ToString();
                        string fieldname = myField[schemaTable.Columns[0]].ToString();
                        string key = "";

                        if (!string.IsNullOrEmpty(tablename))
                        {
                            key = tablename + "." + fieldname;
                        }
                        else
                        {
                            key = fieldname;
                        }

                        dic.Add(key, reader[count]);
                        count++;
                    }

                    foreach (PropertyInfo p in obj.GetType().GetProperties())
                    {
                        PropertyInfo prop = p;

                        object[] another = prop.GetCustomAttributes(typeof(AnotherObject), true);

                        if (another.Length == 0)
                        {
                            instance = prop.DeclaringType.Name;

                            try
                            {
                                name = instance + "." + prop.Name;
                                prop.SetValue(obj, dic[name], null);
                            }
                            catch { }

                        }
                        else
                        {
                            this.Recursividade(ref prop, ref dic, ref obj, query);
                        }
                    }
                    list.Add(obj);
                }
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }

            return list;
        }

        private void Recursividade<T>(ref PropertyInfo prop, ref Dictionary<string, object> dic, ref T obj, string query)
        {
            string instance = prop.PropertyType.Name;
            string name = "";
            bool first = true;

            if (query.Contains(instance))
            {
                object item = Activator.CreateInstance(Type.GetType(prop.PropertyType.FullName));

                foreach (PropertyInfo p in item.GetType().GetProperties())
                {
                    PropertyInfo prop2 = p;

                    try
                    {
                        if (first)
                        {
                            first = false;
                            name = instance + "." + prop2.Name;
                            prop2.SetValue(item, dic[name], null);

                        }
                        else
                        {
                            object[] another = prop2.GetCustomAttributes(typeof(AnotherObject), true);
                            if (another.Length > 0)
                            {
                                prop.SetValue(obj, item);
                                this.Recursividade(ref prop2, ref dic, ref item, query);
                            }
                            else
                            {
                                name = instance + "." + prop2.Name;
                                prop2.SetValue(item, dic[name], null);
                            }
                        }
                    }
                    catch { }
                }

                prop.SetValue(obj, item);
            }
        }

        private string GetCorrectParameterName(string parameterName)
        {
            if (parameterName[0] != '@')
            {
                parameterName = "@" + parameterName;
            }

            return parameterName;
        }

        private object GetPropValue(object src, string propName)
        {
            return src.GetType().GetProperty(propName).GetValue(src, null);
        }


        #endregion

    }
}