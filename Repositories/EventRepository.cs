using Ghenterprise_Backend.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
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

                if (backendEvent.Location != null && backendEvent.Location.Id == null)
                {
                    if (backendEvent.Location.Street != null && backendEvent.Location.Street.Id == null)
                    {
                        query += InsertQuery(new Street[]
                        {
                            backendEvent.Location.Street
                        });
                    }

                    if (backendEvent.Location.City != null && backendEvent.Location.City.Id == null)
                    {
                        query += InsertQuery(new City[]
                        {
                            backendEvent.Location.City
                        });
                    }

                    query += InsertQuery(new Location[] 
                    {
                        backendEvent.Location
                    });
                }

                query += InsertQuery(new Event_Has_Location[]
                {
                    new Event_Has_Location
                    {
                        Event_Id = backendEvent.Id,
                        Location_Id = backendEvent.Location.Id
                    }
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

                if (backendEvent.Location.Street.Id == null)
                {
                    query += InsertQuery(new Street[]
                    {
                        backendEvent.Location.Street
                    });
                }

                if (backendEvent.Location.City.Id == null)
                {
                    query += InsertQuery(new City[]
                    {
                        backendEvent.Location.City
                    });
                }

                query += UpdateQuery(new Location[]
                {
                    backendEvent.Location
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
    }
}