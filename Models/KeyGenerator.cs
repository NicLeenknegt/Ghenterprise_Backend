using Ghenterprise_Backend.Repositories;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Ghenterprise_Backend.Models
{
    public class KeyGenerator
    {
        internal static readonly char[] chars =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
        internal static readonly SSH ssh = new SSH();
        private static readonly string connString = "server=127.0.0.1;port=22;database=Ghenterprise;Uid=jari;Pwd=pazaak;";

        private static string GetUniqueKey(int size)
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

        public string CreateTableID(string tableName)
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
    }
}