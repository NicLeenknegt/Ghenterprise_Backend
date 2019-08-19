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

    public class CategoryRepository
    {
        public string ConnString { get; set; }
        public SSH Ssh { get; set; }
        public KeyGenerator KeyGen { get; set; }

        public CategoryRepository()
        {
            ConnString = "server=127.0.0.1;port=22;database=Ghenterprise;Uid=jari;Pwd=pazaak;";
            Ssh = new SSH();
            KeyGen = new KeyGenerator();
        }

        public int SaveCategory(Category[] catArray)
        {
            return Ssh.executeQuery<int>(() =>
            {
                var insert = "INSERT INTO category (id, name) ";
                var values = "VALUES ";
                int rowsAffected = 0;

                foreach (var cat in catArray)
                {
                    values += String.Format(" ( '{0}' , '{1}' ) ", KeyGen.CreateTableID("category"), cat.Name);
                    if (cat != catArray.Last())
                    {
                        values += ",";
                    }
                }
                Debug.WriteLine(values);

                using(MySqlConnection conn = new MySqlConnection(ConnString))
                {
                    conn.Open();
                    using (MySqlCommand command = new MySqlCommand(insert + values + ";", conn))
                    {
                        rowsAffected = command.ExecuteNonQuery();
                    }
                }

                return rowsAffected;
            });
            

        }

        public List<Category> GetAllCategories()
        {
            return Ssh.executeQuery<List<Category>>(() =>
            {
                List<Category> catList = new List<Category>();
                DataTable table = new DataTable();
                var query = "SELECT * FROM category;";

                using(MySqlConnection conn = new MySqlConnection(ConnString))
                {
                    conn.Open();
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn))
                    {
                        adapter.Fill(table);

                        foreach(DataRow row in table.Rows)
                        {
                            catList.Add(new Category()
                            {
                                Id = row.ItemArray[0].ToString(),
                                Name = row.ItemArray[1].ToString()
                            });
                        }
                    }
                }

                return catList;
            });
        }
    }
}