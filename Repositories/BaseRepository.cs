using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Ghenterprise_Backend.Repositories
{
    public class BaseRepository
    {

        internal static readonly char[] chars =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
        internal static readonly SSH ssh = new SSH();
        private static readonly string connString = "server=127.0.0.1;port=22;database=Ghenterprise;Uid=jari;Pwd=pazaak;";

        internal string CreateTableID(string tableName)
        {

            Boolean idExists = false;
            string id = "";
            do
            {
                id = GetUniqueKey(12);
                var query = String.Format("SELECT * FROM {0} WHERE id = '{1}' LIMIT 1;", tableName, id);

                using (MySqlConnection conn = new MySqlConnection(connString))
                {
                    conn.Open();

                    using (MySqlCommand command = new MySqlCommand(query, conn))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            idExists = reader.Read();
                        }
                    }
                }

            } while (idExists);

            return id;
        }

        private string GetUniqueKey(int size)
        {
            byte[] data = new byte[4 * size];
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetBytes(data);
            }
            StringBuilder result = new StringBuilder(size);
            for (int i = 0; i < size; i++)
            {
                var rnd = BitConverter.ToUInt32(data, i * 4);
                var idx = rnd % chars.Length;

                result.Append(chars[idx]);
            }

            return result.ToString();
        }

        private List<string> GetPropNames<T>(T t, Boolean createID = true)
        {
            List<string> propVal = new List<string>();

            foreach (var prop in GetNonClassProperties(t))
            {
                if (prop.Name.ToLower() == "id" && createID)
                {
                    propVal.Add(prop.Name);
                }
                else if (prop.Name.ToLower() != "id")
                {
                    propVal.Add(prop.Name);
                }
                    
            }

            return propVal;
        }

        private List<List<string>> GetPropValue<T>(T[] t, Boolean createID = true)
        {
            List<List<string>> propVar = new List<List<string>>();

            foreach (var row in t)
            {
                List<string> propVarRow = new List<string>();

                foreach (var prop in GetNonClassProperties(row))
                {

                    if (prop.Name.ToLower() == "id" && createID)
                    {
                        prop.SetValue(row, CreateTableID(row.GetType().Name.ToLower()));
                        propVarRow.Add(string.Format("'{0}'", prop.GetValue(row, null)));
                    }
                    else if (prop.Name.ToLower() != "id")
                    {
                        string value = "";

                        if (prop.PropertyType == typeof(bool))
                        {
                            Debug.WriteLine("1");
                            value = (bool) prop.GetValue(row, null) ? "'1'" : "'0'";
                            Debug.WriteLine("2");
                        } else
                        {
                            value = string.Format("'{0}'", prop.GetValue(row, null));
                        }


                        propVarRow.Add(value);
                    }
                }

                propVar.Add(propVarRow);
            }

            return propVar;
        }

        private PropertyInfo[] GetNonClassProperties<T>(T t)
        {
            return t.GetType().GetProperties().Where((p) => 
                p.Name.ToLower() == "id" || 
                    ( p.GetValue(t, null) != null && 
                        (
                            p.GetValue(t, null).GetType() == typeof(int) || 
                            p.GetValue(t, null).GetType() == typeof(string) ||
                            p.GetValue(t, null).GetType() == typeof(bool)
                        )
                    )
            ).ToArray();
        }

        private string GetValueOfProperty<T>(T t, string propertyName = "id")
        {
            string result = "";
            try
            {
                PropertyInfo prop = t.GetType().GetProperties().Where(p => p.Name.ToLower() == propertyName.ToLower()).First();
                
                if (prop.GetValue(t).GetType() == typeof(string) || prop.GetValue(t).GetType() == typeof(String))
                {
                    result = string.Format("'{0}'", prop.GetValue(t));
                } else
                {
                    result = string.Format("{0}", prop.GetValue(t));
                }
            }
            catch (Exception)
            {
                throw new Exception("Error in BaseRepository.cs: Property ' " + propertyName + " ' does not exist in " + t.GetType().Name);
            }
            return result;
        }

        internal string InsertQuery<T>(T[] t)
        {
            if (t == null || t.Length == 0)
            {
                return "";
            }

            var insertVar = "";
            var insertVal = "";
            string tableName = t[0].GetType().Name.ToLower();

            insertVar += string.Format("( {0} )", string.Join(" ,", GetPropNames(t[0]).ToArray()));

            insertVal += string.Join(" , ",
                GetPropValue(t).Select(
                    item =>
                        string.Format(" ( {0} )",
                            string.Join(" ,", item.ToArray()))
                    ));
             
            var query = String.Format("INSERT INTO {0} {1} VALUES {2};", tableName,  insertVar, insertVal);

            Debug.WriteLine(tableName.ToUpper() + " INSERT QUERY BEGIN");
            Debug.WriteLine(query);
            Debug.WriteLine(tableName.ToUpper() + " INSERT QUERY END");
            return query;
        }

        internal string UpdateQuery<T>(T[] t, string[] searchProperties = null)
        {
            if (t == null || t.Length == 0)
            {
                return "";
            }

            searchProperties = searchProperties ?? new string[] { "id" };

            var query = "";
            List<string> columnNames = GetPropNames(t[0], false);
            List<List<string>> columnValues = GetPropValue(t, false);


            for (int i = 0; i < t.Length; i++)
            {
                query += string.Format("UPDATE {0} ", t[i].GetType().Name.ToLower());
                query += "SET ";
                query += string.Join(" , ", columnValues[i].Select(
                        (v, ind) =>
                            string.Format(" {0} = {1} ", columnNames[ind], v)
                    ));

                query += "  WHERE ";

                query += string.Join(" AND ", searchProperties.Select( 
                        (p) =>
                            string.Format(" {0} = {1} ", 
                                p,
                                GetValueOfProperty(t[i], p))
                    ));
            }

            query += ";";

            Debug.WriteLine(t[0].GetType().Name.ToUpper() + " UPDATE QUERY BEGIN");
            Debug.WriteLine(query);
            Debug.WriteLine(t[0].GetType().Name.ToUpper() + " UPDATE QUERY END");

            return query;
        }

        internal string DeleteQuery<T>(T[] t, string[] searchProperties = null)
        {
            if (t == null || t.Length == 0)
            {
                return "";
            }

            searchProperties = searchProperties ?? new string[] { "id" };

            var query = "";

            foreach (var item in t)
            {
                query += string.Format("DELETE FROM {0} ", item.GetType().Name.ToLower());
                query += "WHERE ";
                query += string.Join(" AND ", searchProperties.Select(
                        (p) =>
                            string.Format(" {0} = {1} ",
                                p,
                                GetValueOfProperty(item, p))
                    ));
            }

            query += ";";

            Debug.WriteLine(t[0].GetType().Name.ToUpper() + " DELETE QUERY BEGIN");
            Debug.WriteLine(query);
            Debug.WriteLine(t[0].GetType().Name.ToUpper() + " DELETE QUERY END");

            return query;
        }
    }
}