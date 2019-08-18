using Ghenterprise_Backend.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ghenterprise_Backend.Repositories
{
    public class EnterpriseRepository
    {
        public string ConnString { get; set; }
        public SSH Ssh { get; set; }
        public KeyGenerator KeyGen { get; set; }

        public EnterpriseRepository()
        {
            ConnString = "server=127.0.0.1;port=22;database=Ghenterprise;Uid=jari;Pwd=pazaak;";
            Ssh = new SSH();
            KeyGen = new KeyGenerator();
        }

        public int SaveEnterprise(int UserId, Enterprise enterprise)
        {
            return Ssh.executeQuery<int>(() => {
                int rowsAffected = 0;
                enterprise.Id = KeyGen.CreateTableID("enterprise");
                enterprise.DateCreated = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                var query = "BEGIN;";
                query += String.Format("INSERT INTO enterprise (id, name, description, date_created) VALUES ('" + enterprise.Id + "', '" + enterprise.Name + "', '" + enterprise.Description + "', '" + enterprise.DateCreated + "');");
                query += String.Format("INSERT INTO user_has_enterprise (enterprise_id, user_id) VALUES ('" + enterprise.Id + "', '" + UserId + "');");
                
                query += "COMMIT;";

                using(MySqlConnection conn = new MySqlConnection(ConnString))
                {
                    conn.Open();
                    using (MySqlCommand command = new MySqlCommand(query,conn))
                    {
                        rowsAffected = command.ExecuteNonQuery();
                    }
                }

                return rowsAffected;
            });
        }
    }
}