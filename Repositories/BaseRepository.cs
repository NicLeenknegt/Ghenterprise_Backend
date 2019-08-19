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

        private PropertyInfo[] GetNonClassProperties<T>(T t)
        {
            return t.GetType().GetProperties().Where((p) => 
                p.Name.ToLower() == "id" || 
                    ( p.GetValue(t, null) != null && 
                        (
                            p.GetValue(t, null).GetType() == typeof(int) || 
                            p.GetValue(t, null).GetType() == typeof(string)
                        )
                    )
            ).ToArray();
        }

        internal string InsertQuery<T>(T[] t)
        {
            if (t == null || t.Length == 0)
            {
                return "";
            }

            var insertVar = " ( ";
            var insertVal = " ";
            string tableName = t[0].GetType().Name.ToLower();
            PropertyInfo[] insertVarProp = GetNonClassProperties(t[0]);

            foreach (var prop in insertVarProp)
            {
                insertVar += prop.Name + ((insertVarProp.Last().Name == prop.Name)? " ) ": " , ");
            }

            for (int i = 0; i < t.Length; i++)
            {
                insertVal += " ( ";
                PropertyInfo[] insertValProp = GetNonClassProperties(t[i]);

                foreach (var prop in insertValProp)
                {

                    if (prop.Name.ToLower() == "id")
                    {
                        prop.SetValue(t[i], CreateTableID(tableName));
                    }

                    insertVal += "'" + prop.GetValue(t[i], null) + "'" + ((insertValProp.Last().Name == prop.Name) ? " ) " : " , ");
                }

                if ((i + 1) != t.Length)
                {
                    insertVal += "  , ";
                }
            }



            var query = String.Format("INSERT INTO {0} {1} VALUES {2};", tableName,  insertVar, insertVal);

            Debug.WriteLine(tableName.ToUpper() + " INSERT QUERY BEGIN");
            Debug.WriteLine(query);
            Debug.WriteLine(tableName.ToUpper() + " INSERT QUERY END");
            return query;
        }
    }
}