using Ghenterprise_Backend.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ghenterprise_Backend.Repositories
{
    public class LocationRepository:BaseRepository
    {
        public string ConnString { get; set; }

        public LocationRepository()
        {
            ConnString = "server=127.0.0.1;port=22;database=Ghenterprise;Uid=jari;Pwd=pazaak;";
        }

        public List<City> GetAllCities()
        {
            return ssh.executeQuery(() =>
            {
                List<City> cities = new List<City>();

                var query = "SELECT * FROM city";

                using(MySqlConnection conn = new MySqlConnection(ConnString))
                {
                    conn.Open();
                    using(MySqlCommand command = new MySqlCommand(query, conn))
                    {
                        using(MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read() == true)
                            {
                                cities.Add(new City
                                {
                                    Id = reader.GetString(0),
                                    Name = reader.GetString(1)
                                });
                            }
                        }
                    }
                }

                return cities;
            });
        }

         public List<Street> GetAllStreets()
        {
            return ssh.executeQuery(() =>
            {
                List<Street> streets = new List<Street>();

                var query = "SELECT * FROM street";

                using(MySqlConnection conn = new MySqlConnection(ConnString))
                {
                    conn.Open();
                    using(MySqlCommand command = new MySqlCommand(query, conn))
                    {
                        using(MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read() == true)
                            {
                                streets.Add(new Street
                                {
                                    Id = reader.GetString(0),
                                    Name = reader.GetString(1)
                                });
                            }
                        }
                    }
                }

                return streets;
            });
        }

        public int InsertCity(City city)
        {
            return ssh.executeQuery(() => 
            {
                int affectedRows = 0;
                var query = InsertQuery(new City[]
                {
                    city
                });

                using (MySqlConnection conn = new MySqlConnection(ConnString))
                {
                    conn.Open();
                    using (MySqlCommand command = new MySqlCommand(query, conn))
                    {
                        affectedRows = command.ExecuteNonQuery();
                    }
                }

                return affectedRows;
            });
        }

        public int InsertStreet(Street street)
        {
            return ssh.executeQuery(() =>
            {
                int affectedRows = 0;
                var query = InsertQuery(new Street[]
                {
                    street
                });

                using (MySqlConnection conn = new MySqlConnection(ConnString))
                {
                    conn.Open();
                    using (MySqlCommand command = new MySqlCommand(query, conn))
                    {
                        affectedRows = command.ExecuteNonQuery();
                    }
                }

                return affectedRows;
            });
        }
    }
}