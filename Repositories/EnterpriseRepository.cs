using Ghenterprise_Backend.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace Ghenterprise_Backend.Repositories
{
    public class EnterpriseRepository:BaseRepository
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
                enterprise.Date_Created = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                List<Enterprise_Has_Tag> entTags = new List<Enterprise_Has_Tag>();
                List<Enterprise_Has_Category> entCats = new List<Enterprise_Has_Category>();

                var query = "BEGIN;";
                
                //Enterprise creation query
                query += InsertQuery(new Enterprise[] { enterprise });

                //Enterprise Tag Connection creation
                if (enterprise.Tags != null || enterprise.Tags.Count != 0)
                {
                    //Insertion of non existing tags
                    query += InsertQuery(enterprise.Tags.Where((t) => t.Id == null).ToArray());

                    //Insertions of enterprise & tag connections
                    foreach (var item in enterprise.Tags)
                    {
                        entTags.Add(new Enterprise_Has_Tag
                        {
                            Enterprise_ID = enterprise.Id,
                            Tag_ID = item.Id
                        });
                    }

                    query += InsertQuery<Enterprise_Has_Tag>(entTags.ToArray());
                }

                if (enterprise.Location.City.Id == null)
                {
                    query += InsertQuery(new City[] { enterprise.Location.City } );
                    Debug.WriteLine(enterprise.Location.City.Id);
                }

                if (enterprise.Location.Street.Id == null)
                {
                    query += InsertQuery(new Street[] { enterprise.Location.Street });
                    Debug.WriteLine(enterprise.Location.Street.Id);
                }

                query += InsertQuery(new Location[] { enterprise.Location });

                query += InsertQuery(new Enterprise_Has_Location[]
                {
                    new Enterprise_Has_Location
                    {
                        Enterprise_ID = enterprise.Id,
                        Location_ID = enterprise.Location.Id
                    }
                });

                //enterprise_has_category connection
                if (enterprise.Categories != null || enterprise.Categories.Count != 0 )
                {
                    query += InsertQuery(enterprise.Categories.Where((t) => t.Id == null).ToArray());

                    foreach (var item in enterprise.Categories)
                    {
                        entCats.Add(new Enterprise_Has_Category
                        {
                            Enterprise_ID = enterprise.Id,
                            Category_ID = item.Id
                        });
                    }

                    query += InsertQuery(entCats.ToArray());
                }

                //user_has_enterprise connection
                query += InsertQuery(new User_Has_Enterprise[]
                {
                    new User_Has_Enterprise
                    {
                        User_ID = UserId,
                        Enterprise_ID = enterprise.Id
                    }
                });
                
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

        public int SubscribeToEnterprise(int userId, string entId)
        {
            return ssh.executeQuery(() =>
            {
                int rowsAffected = 0;

                var query = InsertQuery(new User_Has_Subscription[]
                {
                    new User_Has_Subscription
                    {
                        User_ID = userId,
                        Enterprise_ID = entId
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

        private List<Enterprise> MysqlReaderToEterprise(string query)
        {
            DataTable table = new DataTable();
            List<Enterprise> entList = new List<Enterprise>();
            int index = -1;

            using (MySqlConnection conn = new MySqlConnection(ConnString))
            {
                conn.Open();
                using (MySqlCommand command = new MySqlCommand(query, conn))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read() == true)
                        {
                            if (entList.Count == 0 || reader.GetString(0) != entList[index].Id)
                            {
                                entList.Add(new Enterprise
                                {
                                    Id = reader.IsDBNull(0) ? "" : reader.GetString(0),
                                    Name = reader.IsDBNull(1) ? "" : reader.GetString(1),
                                    Description = reader.IsDBNull(2) ? "" : reader.GetString(2),
                                    Date_Created = reader.IsDBNull(3) ? "" : reader.GetString(3)
                                });
                                index++;
                            }


                            if (entList[index].Tags == null)
                            {
                                entList[index].Tags = new List<Tag>();
                            }

                            if (entList[index].Categories == null)
                            {
                                entList[index].Categories = new List<Category>();
                            }


                            if (!reader.IsDBNull(4) && !reader.IsDBNull(5))
                            {
                                entList[index].Tags.Add(new Tag
                                {
                                    Id = reader.GetString(4),
                                    Name = reader.GetString(5)
                                });
                            }

                            if (!reader.IsDBNull(6) && !reader.IsDBNull(7))
                            {
                                entList[index].Categories.Add(new Category
                                {
                                    Id = reader.GetString(6),
                                    Name = reader.GetString(7)
                                });
                            }

                            if (!reader.IsDBNull(8) && !reader.IsDBNull(9))
                            {
                                entList[index].Location = new Location
                                {
                                    Id = reader.GetString(8),
                                    Street_Number = reader.GetInt16(9)
                                };
                            }

                            if (!reader.IsDBNull(10) && !reader.IsDBNull(11))
                            {
                                entList[index].Location.Street = new Street
                                {
                                    Id = reader.GetString(10),
                                    Name = reader.GetString(11)
                                };
                            }

                            if (!reader.IsDBNull(12) && !reader.IsDBNull(13))
                            {
                                entList[index].Location.City = new City
                                {
                                    Id = reader.GetString(12),
                                    Name = reader.GetString(13)
                                };
                            }
                        }
                    }
                }
            }

            return entList;
        }

        public List<Enterprise> GetEnterprisesByOwner(int ownerID)
        {
            return ssh.executeQuery(() =>
            {
                var query = String.Format("SELECT  e.id, e.name, e.description, e.date_created, t.id, t.name, c.id, c.name, l.id, l.street_number, s.id, s.name, cit.id, cit.name " +
                    "FROM Ghenterprise.enterprise e " +
                    "left outer join Ghenterprise.enterprise_has_tag eht " +
                    "on eht.enterprise_id = e.id " +
                    "left outer join Ghenterprise.tag t " +
                    "on eht.tag_id = t.id " +
                    "left outer join Ghenterprise.enterprise_has_category ehc " +
                    "on ehc.enterprise_id = e.id " +
                    "left outer join Ghenterprise.category c " +
                    "on ehc.category_id = c.id " +
                    "left outer join Ghenterprise.enterprise_has_location ehl " +
                    "on ehl.enterprise_id = e.id " +
                    "left outer join Ghenterprise.location l " +
                    "on ehl.location_id = l.id " +
                    "left outer join Ghenterprise.street s " +
                    "on l.street_id = s.id " +
                    "left outer join Ghenterprise.city cit " +
                    "on l.city_id = cit.id " +
                    "left outer join Ghenterprise.user_has_enterprise uhe " +
                    "on uhe.enterprise_id = e.id " +
                    "where uhe.user_id = {0};",
                    ownerID);

                return MysqlReaderToEterprise(query);
            });
            
        }

        public List<Enterprise> GetSubsciptionEnterprise(int userID)
        {
            return ssh.executeQuery(() =>
            {
                var query = String.Format("SELECT  e.id, e.name, e.description, e.date_created, t.id, t.name, c.id, c.name, l.id, l.street_number, s.id, s.name, cit.id, cit.name " +
                    "FROM Ghenterprise.enterprise e " +
                    "left outer join Ghenterprise.enterprise_has_tag eht " +
                    "on eht.enterprise_id = e.id " +
                    "left outer join Ghenterprise.tag t " +
                    "on eht.tag_id = t.id " +
                    "left outer join Ghenterprise.enterprise_has_category ehc " +
                    "on ehc.enterprise_id = e.id " +
                    "left outer join Ghenterprise.category c " +
                    "on ehc.category_id = c.id " +
                    "left outer join Ghenterprise.enterprise_has_location ehl " +
                    "on ehl.enterprise_id = e.id " +
                    "left outer join Ghenterprise.location l " +
                    "on ehl.location_id = l.id " +
                    "left outer join Ghenterprise.street s " +
                    "on l.street_id = s.id " +
                    "left outer join Ghenterprise.city cit " +
                    "on l.city_id = cit.id " +
                    "left outer join Ghenterprise.user_has_subscription uhs " +
                    "on uhs.enterprise_id = e.id " +
                    "where uhs.user_id = {0};",
                    userID);


                return MysqlReaderToEterprise(query);
            });

        }

        public int UpdateEnterprise(Enterprise enterprise)
        {
            return ssh.executeQuery(() =>
            {
                int rowsAffected = 0;

                var query = "BEGIN;";

                query += UpdateQuery(new Enterprise[] { enterprise });
                if (enterprise.Location != null )
                {

                    if (enterprise.Location.Street != null)
                    {

                        if (enterprise.Location.Street.Id == null)
                        {

                            query += InsertQuery(new Street[] { enterprise.Location.Street });
                        }
                    }

                    if (enterprise.Location.City == null)
                    {

                        if (enterprise.Location.City.Id == null)
                        {

                            query += InsertQuery(new City[] { enterprise.Location.City });
                        }
                    }

                    if (enterprise.Location.Id == null)
                    {

                        query += InsertQuery(new Location[] { enterprise.Location });
                        query += InsertQuery(new Enterprise_Has_Location[] {
                            new Enterprise_Has_Location
                            {
                                Enterprise_ID = enterprise.Id,
                                Location_ID = enterprise.Location.Id
                            }
                        });
                    } else
                    {
                        Debug.WriteLine("9");

                        query += UpdateQuery(new Location[] { enterprise.Location });
                    }
                }

                if (enterprise.Tags != null && enterprise.Tags.Count > 0)
                {
                    Tag[] nullIdTags = enterprise.Tags.Where(t => t.Id == null).ToArray();
                    Tag[] idTags = enterprise.Tags.Where(t => t.Id != null).ToArray();

                    query += DeleteQuery(new Enterprise_Has_Tag[]
                         {
                            new Enterprise_Has_Tag
                            {
                                Enterprise_ID = enterprise.Id,
                                Tag_ID = null
                            }
                         }, new string[] { "enterprise_id" });

                    if (nullIdTags != null && nullIdTags.Length > 0)
                    {
                        query += InsertQuery(nullIdTags);
                    }
                    query += InsertQuery(enterprise.Tags.Select(
                                n =>
                                    new Enterprise_Has_Tag
                                    {
                                        Enterprise_ID = enterprise.Id,
                                        Tag_ID = n.Id
                                    }
                            ).ToArray());
                }

                if (enterprise.Categories != null && enterprise.Categories.Count > 0)
                {

                    query += DeleteQuery(new Enterprise_Has_Category[]
                         {
                            new Enterprise_Has_Category
                            {
                                Enterprise_ID = enterprise.Id,
                                Category_ID = null
                            }
                         }, new string[] { "enterprise_id" });

                    query += InsertQuery(enterprise.Categories.Select( c =>
                        new Enterprise_Has_Category
                        {
                            Enterprise_ID = enterprise.Id,
                            Category_ID = c.Id
                        }
                    ).ToArray());
                }

                query += "COMMIT;";

                Debug.WriteLine(query);

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