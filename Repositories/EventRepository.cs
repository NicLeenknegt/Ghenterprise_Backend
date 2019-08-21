using Ghenterprise_Backend.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Ghenterprise_Backend.Repositories
{
    public class EventRepository:BaseRepository
    {
        public string ConnString { get; set; }
        public SSH Ssh { get; set; }

        public EventRepository()
        {
            ConnString = "server=127.0.0.1;port=22;database=Ghenterprise;Uid=jari;Pwd=pazaak;";
            Ssh = new SSH();
        }

        public int SaveEvent(Event backendEvent)
        {
            return Ssh.executeQuery(() =>
            {
                int rowsAffected = 0;
                var query = "BEGIN;";

                query += InsertQuery(new Event[]
                {
                    backendEvent
                });


                query += InsertQuery(new Enterprise_Has_Event[]
                {
                    new Enterprise_Has_Event
                    {
                        Enterprise_Id = backendEvent.Enterprise.Id,
                        Event_Id = backendEvent.Id
                    }
                });

                query += InsertQuery(new Notification[]
                {
                    new Notification
                    {
                        Event_Id = backendEvent.Id,
                        Enterprise_Id = backendEvent.Enterprise.Id
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

        public int UpdateEvent(Event backendEvent)
        {
            return ssh.executeQuery(() =>
            {
                int rowsAffected = 0;
                var query = "BEGIN;";

                query += UpdateQuery(new Event[] 
                {
                    backendEvent
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

        public int DeleteEvent(string EventId)
        {
            return ssh.executeQuery(() => 
            {
                int rowsAffected = 0;

                var query = DeleteQuery(new Event[]
                {
                    new Event
                    {
                        Id = EventId
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

        public List<Event> GetEventsOfEnterprise(string EnterpriseId)
        {
            return ssh.executeQuery(() =>
            {

                var query = String.Format("SELECT  e.id,  e.description, e.start_date, e.end_date, l.Id, l.street_number, l.street_id, s.name, l.city_id, c.name " +
                        "FROM Ghenterprise.event e " +
                        "left outer join Ghenterprise.enterprise_has_event ehe " +
                        "on ehe.event_id = e.id " +
                        "left outer join Ghenterprise.enterprise_has_location ehl " +
                        "on ehl.enterprise_id = ehe.enterprise_id " +
                        "left outer join Ghenterprise.location l " +
                        "on ehl.location_id = l.id " +
                        "left outer join Ghenterprise.city c " +
                        "on l.city_id = c.id " +
                        "left outer join Ghenterprise.street s  " +
                        "on l.street_id = s.id " +
                        "where ehe.enterprise_id = '{0}';",
                        EnterpriseId);

                DataTable table = new DataTable();
                List<Event> eventList = new List<Event>();

                using(MySqlConnection conn = new MySqlConnection(ConnString))
                {
                    conn.Open();
                    using(MySqlCommand command = new MySqlCommand(query, conn))
                    {
                        using(MySqlDataReader reader = command.ExecuteReader())
                        {
                            while(reader.Read() == true)
                            {
                                eventList.Add(new Event
                                {
                                    Id = reader.GetString(0),
                                    Description = reader.GetString(1),
                                    Start_Date = reader.GetString(2),
                                    End_Date = reader.GetString(3),
                                    Location = new Location
                                    {
                                        Id = reader.GetString(4),
                                        Street_Number = reader.GetInt16(5),
                                        Street = new Street
                                        {
                                            Id = reader.GetString(6),
                                            Name = reader.GetString(7)
                                        },
                                        City = new City
                                        {
                                            Id = reader.GetString(8),
                                            Name = reader.GetString(9)
                                        }
                                    }
                                });
                            }
                        }
                    }
                }

                return eventList;
            });
            
        }
    }
}