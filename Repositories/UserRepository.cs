using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Web;
using Ghenterprise_Backend.Models;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using MySql.Data.MySqlClient;

namespace Ghenterprise_Backend.Repositories
{
    public class UserRepository:BaseRepository
    {
        public string connString { get; set; }
        public SSH ssh { get; set; }

        public UserRepository()
        {
            connString = "server=127.0.0.1;port=22;database=Ghenterprise;Uid=jari;Pwd=pazaak;";
            ssh = new SSH();
        }

        public int registerUser(User user)
        {
            return ssh.executeQuery(() => {
                int rowsAffected = 0;

                using (MySqlConnection conn = new MySqlConnection(connString))
                {
                    conn.Open();
                    var query = InsertQuery(new User[]
                    {
                        user
                    });

                    using (MySqlCommand command = new MySqlCommand(query, conn))
                    {
                        rowsAffected = command.ExecuteNonQuery();
                    }
                    
                }

                return rowsAffected;
            });

        }

        public Boolean checkEmail(string email)
        {
            System.Diagnostics.Debug.WriteLine(email);
            return ssh.executeQuery<Boolean>(() =>
            {
                var query = String.Format("SELECT * FROM user WHERE email = '{0}' LIMIT 1;", email);
                Boolean emailExists = false;

                using (MySqlConnection conn = new MySqlConnection(connString))
                {
                    conn.Open();

                    using(MySqlCommand command = new MySqlCommand(query, conn))
                    {
                       using(MySqlDataReader reader = command.ExecuteReader())
                       {
                            emailExists = reader.Read();
                       }
                    }
                }
                return emailExists;
            });
        }

        public User login(User user)
        {
            return ssh.executeQuery(() => 
            {
                var query = String.Format("SELECT id, password FROM user WHERE email = '{0}' LIMIT 1;", user.email);
                String response = "Password invalid";

                using(MySqlConnection conn = new MySqlConnection(connString))
                {
                    conn.Open();

                    using(MySqlCommand command = new MySqlCommand(query, conn))
                    {
                        using(MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read() == true)
                                {
                                    user.id = reader.GetString(0);
                                    user.password = (reader.GetString(1) == user.password) ? "Password valid" : "Password invalid";
                                }
                            } else
                            {
                                user.password = "User doesn't exist";
                            }
                            
                        }
                    }
                    conn.Close();
                }   
                return user;
            });
        }
    }
}