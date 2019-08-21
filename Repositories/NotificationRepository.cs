using Ghenterprise_Backend.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace Ghenterprise_Backend.Repositories
{
    public class NotificationRepository:BaseRepository
    {
        public string ConnString { get; set; }

        public NotificationRepository()
        {
            ConnString = "server=127.0.0.1;port=22;database=Ghenterprise;Uid=jari;Pwd=pazaak;";
            
        }

        public int InsertUserNotifications(int UserId)
        {
            return ssh.executeQuery(() => 
            {
                var query = string.Format("INSERT INTO Ghenterprise.user_has_notification ( user_id , seen, notification_id ) " +
                    "SELECT  '{0}', 0, n.Id " +
                    "FROM Ghenterprise.notification n " +
                    "LEFT OUTER JOIN Ghenterprise.user_has_subscription uhs " +
                    "ON uhs.enterprise_id = n.enterprise_id " +
                    "WHERE   uhs.user_id = '{0}' ; "
                    , UserId);

                int rowsAffected = 0;

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

        public int SetUserNotificationAsSeen(List<User_Has_Notification> uhnList)
        {
            return ssh.executeQuery(() =>
            {
                int rowsAffected = 0;
                foreach (var uhn in uhnList)
                {
                    uhn.Seen = true;
                }

                var query = UpdateQuery(uhnList.ToArray(), new string[]
                {
                    "User_Id","Notification_Id"
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

        public List<Notification> GetNotificationsByUser(int UserId)
        {
            return ssh.executeQuery(() => 
            {
                List<Notification> notList = new List<Notification>();

                var query = string.Format("SELECT n.id, n.event_id, e.description, e.start_date, e.end_date, n.promotion_id, p.description, p.start_date, p.end_date " +
                    "FROM notification n " +
                    "LEFT OUTER JOIN event e " +
                    "ON e.id = n.event_id " +
                    "LEFT OUTER JOIN promotion p " +
                    "ON p.id = n.promotion_id " +
                    "LEFT OUTER JOIN user_has_notification uhn " +
                    "ON uhn.notification_id = n.id " +
                    "WHERE uhn.user_id = {0};", 
                    UserId);

                using (MySqlConnection conn = new MySqlConnection(ConnString))
                {
                    conn.Open();
                    using (MySqlCommand command = new MySqlCommand(query, conn))
                    {
                        using(MySqlDataReader reader = command.ExecuteReader())
                        {
                            while(reader.Read() == true)
                            {
                                Notification not = new Notification
                                {
                                    Id = reader.GetString(0)
                                };

                                if (!reader.IsDBNull(1))
                                {
                                    not.Event = new Event
                                    {
                                        Id = reader.GetString(1),
                                        Description = reader.GetString(2),
                                        Start_Date = reader.GetString(3),
                                        End_Date = reader.GetString(4)
                                    };
                                }

                                if (!reader.IsDBNull(5))
                                {
                                    not.Promotion = new Promotion
                                    {
                                        Id = reader.GetString(5),
                                        Description = reader.GetString(6),
                                        Start_Date = reader.GetString(7),
                                        End_Date = reader.GetString(8)
                                    };
                                }

                                notList.Add(not);
                            }
                        }
                    }
                }

                return notList;
            });
        }
    }
}