using Ghenterprise_Backend.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ghenterprise_Backend.Repositories
{
    public class PromotionRepository:BaseRepository
    {
        public string ConnString { get; set; }

        public PromotionRepository()
        {
            ConnString = "server=127.0.0.1;port=22;database=Ghenterprise;Uid=jari;Pwd=pazaak;";
        }

        public int SavePromotion(Promotion promotion)
        {
            return ssh.executeQuery(() =>
            {
                int rowsAffected = 0;

                var query = "BEGIN;";

                query += InsertQuery(new Promotion[] 
                {
                    promotion
                });

                query += InsertQuery(new Enterprise_Has_Promotion[]
                {
                    new Enterprise_Has_Promotion
                    {
                        Enterprise_Id = promotion.Enterprise.Id,
                        Promotion_Id = promotion.Id
                    }
                });

                query += InsertQuery(new Notification[]
                {
                    new Notification
                    {
                        Enterprise_Id = promotion.Enterprise.Id,
                        Promotion_Id = promotion.Id
                    }
                });

                query += "COMMIT;";

                using (MySqlConnection conn = new MySqlConnection(ConnString))
                {
                    conn.Open();
                    using (MySqlCommand command = new MySqlCommand(query, conn))
                    {
                        rowsAffected = command.ExecuteNonQuery();
                    }
                }

                return rowsAffected;
            });
        }

        public int UpdatePromotion(Promotion promotion)
        {
            return ssh.executeQuery(() =>
            {
                int rowsAffected = 0;

                var query = UpdateQuery(new Promotion[]
                {
                    promotion
                });

                using (MySqlConnection conn = new MySqlConnection(ConnString))
                {
                    conn.Open();
                    using (MySqlCommand command = new MySqlCommand(query, conn))
                    {
                        rowsAffected = command.ExecuteNonQuery();
                    }
                }

                return rowsAffected;
            });
        }

        public List<Promotion> GetPromotionsByEnterprise(string enterprise_Id)
        {
            return ssh.executeQuery(() =>
            {
                var query = string.Format("SELECT p.id, p.name, p.description, p.start_date, p.end_date " +
                    "FROM Ghenterprise.promotion p " +
                    "LEFT OUTER JOIN Ghenterprise.enterprise_has_promotion ehp " +
                    "ON p.id = ehp.promotion_id " +
                    "WHERE ehp.enterprise_id = '{0}'", 
                    enterprise_Id);
                List<Promotion> promList = new List<Promotion>();

                using(MySqlConnection conn = new MySqlConnection(ConnString))
                {
                    conn.Open();
                    using(MySqlCommand command = new MySqlCommand(query, conn))
                    {
                        using(MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read() == true)
                            {
                                promList.Add(new Promotion
                                {
                                    Id = reader.GetString(0),
                                    Name = reader.GetString(1),
                                    Description = reader.GetString(2),
                                    Start_Date = reader.GetString(3),
                                    End_Date = reader.GetString(4)
                                });
                            }
                        }
                    }
                }

                return promList;
            });
        }


        public List<Promotion> GetAllPromotionsByOwner(string Owner_ID)
        {
            return ssh.executeQuery(() =>
            {
                var query = string.Format("SELECT p.id, p.name, p.description, p.start_date, p.end_date, e.id, e.name, e.description " +
                    "FROM Ghenterprise.promotion p " +
                    "LEFT OUTER JOIN Ghenterprise.enterprise_has_promotion ehp " +
                    "ON p.id = ehp.promotion_id " +
                    "LEFT OUTER JOIN Ghenterprise.enterprise e " +
                    "ON e.id = ehp.enterprise_id " +
                    "LEFT OUTER JOIN Ghenterprise.user_has_enterprise uhe " +
                    "on uhe.enterprise_id = e.id " +
                    "where uhe.user_id = '{0}'",
                    Owner_ID);
                List<Promotion> promList = new List<Promotion>();

                using (MySqlConnection conn = new MySqlConnection(ConnString))
                {
                    conn.Open();
                    using (MySqlCommand command = new MySqlCommand(query, conn))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read() == true)
                            {
                                promList.Add(new Promotion
                                {
                                    Id = reader.GetString(0),
                                    Name = reader.GetString(1),
                                    Description = reader.GetString(2),
                                    Start_Date = reader.GetString(3),
                                    End_Date = reader.GetString(4),
                                    Enterprise = new Enterprise
                                    {
                                        Id = reader.GetString(5),
                                        Name = reader.GetString(6),
                                        Description = reader.GetString(7)
                                    }
                                });
                            }
                        }
                    }
                }

                return promList;
            });
        }

        public Promotion GetPromotioById(string Prom_ID)
        {
            return ssh.executeQuery(() =>
            {
                var query = string.Format("SELECT p.id, p.name, p.description, p.start_date, p.end_date, e.id, e.name, e.description " +
                    "FROM Ghenterprise.promotion p " +
                    "LEFT OUTER JOIN Ghenterprise.enterprise_has_promotion ehp " +
                    "ON p.id = ehp.promotion_id " +
                    "LEFT OUTER JOIN Ghenterprise.enterprise e " +
                    "ON e.id = ehp.enterprise_id  " +
                    "where p.id = '{0}';" ,
                    Prom_ID);
                List<Promotion> promList = new List<Promotion>();

                using (MySqlConnection conn = new MySqlConnection(ConnString))
                {
                    conn.Open();
                    using (MySqlCommand command = new MySqlCommand(query, conn))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read() == true)
                            {
                                promList.Add(new Promotion
                                {
                                    Id = reader.GetString(0),
                                    Name = reader.GetString(1),
                                    Description = reader.GetString(2),
                                    Start_Date = reader.GetString(3),
                                    End_Date = reader.GetString(4),
                                    Enterprise = new Enterprise
                                    {
                                        Id = reader.GetString(5),
                                        Name = reader.GetString(6),
                                        Description = reader.GetString(7)
                                    }
                                });
                            }
                        }
                    }
                }

                return promList[0];
            });
        }

        public List<Promotion> GetPromotionBySubscription(string Sub_Id)
        {
            return ssh.executeQuery(() =>
            {
                var query = string.Format("SELECT p.id, p.name, p.description, p.start_date, p.end_date, e.id, e.name, e.description " +
                    "FROM Ghenterprise.promotion p " +
                    "LEFT OUTER JOIN Ghenterprise.enterprise_has_promotion ehp " +
                    "ON p.id = ehp.promotion_id " +
                    "LEFT OUTER JOIN Ghenterprise.enterprise e " +
                    "ON e.id = ehp.enterprise_id " + 
                    "Left outer join Ghenterprise.user_has_subscription uhs " +
                    "on uhs.enterprise_id = e.id" +
                    "Where uhs.user_id = '{0}';",
                    Sub_Id);
                List<Promotion> promList = new List<Promotion>();

                using (MySqlConnection conn = new MySqlConnection(ConnString))
                {
                    conn.Open();
                    using (MySqlCommand command = new MySqlCommand(query, conn))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read() == true)
                            {
                                promList.Add(new Promotion
                                {
                                    Id = reader.GetString(0),
                                    Name = reader.GetString(1),
                                    Description = reader.GetString(2),
                                    Start_Date = reader.GetString(3),
                                    End_Date = reader.GetString(4),
                                    Enterprise = new Enterprise
                                    {
                                        Id = reader.GetString(5),
                                        Name = reader.GetString(6),
                                        Description = reader.GetString(7)
                                    }
                                });
                            }
                        }
                    }
                }

                return promList;
            });
        }

        public List<Promotion> GetAllPromotions()
        {
            return ssh.executeQuery(() =>
            {
                var query = string.Format("SELECT p.id, p.name, p.description, p.start_date, p.end_date, e.id, e.name, e.description " +
                    "FROM Ghenterprise.promotion p " +
                    "LEFT OUTER JOIN Ghenterprise.enterprise_has_promotion ehp " +
                    "ON p.id = ehp.promotion_id " +
                    "LEFT OUTER JOIN Ghenterprise.enterprise e " +
                    "ON e.id = ehp.enterprise_id ");
                List<Promotion> promList = new List<Promotion>();

                using (MySqlConnection conn = new MySqlConnection(ConnString))
                {
                    conn.Open();
                    using (MySqlCommand command = new MySqlCommand(query, conn))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read() == true)
                            {
                                promList.Add(new Promotion
                                {
                                    Id = reader.GetString(0),
                                    Name = reader.GetString(1),
                                    Description = reader.GetString(2),
                                    Start_Date = reader.GetString(3),
                                    End_Date = reader.GetString(4),
                                    Enterprise = new Enterprise
                                    {
                                        Id = reader.GetString(5),
                                        Name = reader.GetString(6),
                                        Description = reader.GetString(7)
                                    }
                                });
                            }
                        }
                    }
                }

                return promList;
            });
        }

        public int DeletePromotionById(string Promotion_id)
        {
            return ssh.executeQuery(() =>
            {
                int rowsAffected = 0;

                var query = DeleteQuery(new Promotion[]
                {
                    new Promotion
                    {
                        Id = Promotion_id
                    }
                });

                using (MySqlConnection conn = new MySqlConnection(ConnString))
                {
                    conn.Open();
                    using (MySqlCommand command = new MySqlCommand(query, conn))
                    {
                        rowsAffected = command.ExecuteNonQuery();
                    }
                }

                return rowsAffected;
            });
        }
    }
}