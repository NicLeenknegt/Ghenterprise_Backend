using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Renci.SshNet;
using System.IO;
using System.Text;

namespace Ghenterprise_Backend.Controllers
{
    public class UserController : ApiController
    {

        [HttpGet]
        public HttpResponseMessage get()
        {
            return Request.CreateResponse(HttpStatusCode.OK, 0);
        }

        [HttpPost]
        public HttpResponseMessage Register([FromBody] User user)
        {
            int affectedRows = 0;

            try
            {
                System.Diagnostics.Debug.WriteLine("0");
                var filepath = @"D:\school\18-19\Sem_1\Mobile_App_voor_Windows\Server_Keys\keys\private";
                System.Diagnostics.Debug.WriteLine("1");
                var key = File.ReadAllText(filepath);
                System.Diagnostics.Debug.WriteLine("2");
                var buf = new MemoryStream(Encoding.UTF8.GetBytes(key));
                System.Diagnostics.Debug.WriteLine("3");
                var privateKeyFile = new PrivateKeyFile(filepath,"pazaak");
                System.Diagnostics.Debug.WriteLine("4");


                ConnectionInfo connectionInfo = new ConnectionInfo("ghenterpriseapp.westeurope.cloudapp.azure.com", 22 , "jari", 
                    new AuthenticationMethod[] {
                        new PrivateKeyAuthenticationMethod("jari", new []
                        {
                            new PrivateKeyFile(filepath, "pazaak")
                        })
                    }
                );
                connectionInfo.Timeout = TimeSpan.FromSeconds(30);
                System.Diagnostics.Debug.WriteLine("5");
                using (SshClient client = new SshClient(connectionInfo))
                {
                    System.Diagnostics.Debug.WriteLine("2");
                    client.Connect();
                    System.Diagnostics.Debug.WriteLine("3");
                    ForwardedPortLocal port = new ForwardedPortLocal("127.0.0.1",22, "13.94.138.165", 3306);
                    System.Diagnostics.Debug.WriteLine("4");
                    client.AddForwardedPort(port);
                    System.Diagnostics.Debug.WriteLine("5");
                    System.Diagnostics.Debug.WriteLine("CHECK");
                    port.Start();

                    System.Diagnostics.Debug.WriteLine("CHECK_2");
                    using (MySqlConnection conn = new MySqlConnection("server=127.0.0.1;port=22;database=Ghenterprise;Uid=jari;Pwd=pazaak;"))
                    {
                        System.Diagnostics.Debug.WriteLine("6");
                        
                        conn.Open();
                        System.Diagnostics.Debug.WriteLine("7");

                        using (MySqlCommand command = new MySqlCommand(String.Format("INSERT INTO user (firstname, lastname, email, password) VALUES( '{0}', '{1}', '{2}', '{3}');", user.firstname, user.lastname, user.email, user.password), conn))
                        {
                            System.Diagnostics.Debug.WriteLine("8");
                            affectedRows = command.ExecuteNonQuery();
                            System.Diagnostics.Debug.WriteLine("9");
                        }
                        System.Diagnostics.Debug.WriteLine("10");
                    }
                    System.Diagnostics.Debug.WriteLine("11");
                    client.Disconnect();
                    System.Diagnostics.Debug.WriteLine("12");
                }
                
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
            return Request.CreateResponse(HttpStatusCode.OK, affectedRows);
        }
    }

    public class User
    {
        public String firstname { get; set;  }
        public String lastname { get; set;  }
        public String email { get; set;  }
        public String password { get; set;  }
    }
}
