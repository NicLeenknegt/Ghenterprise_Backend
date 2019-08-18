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
using System.ComponentModel.DataAnnotations;
using Ghenterprise_Backend.Models;
using Ghenterprise_Backend.Repositories;
using System.Net.Http.Headers;

namespace Ghenterprise_Backend.Controllers
{
    public class UserController : ApiController
    {
        public UserRepository userRepo { get; set; }

        public UserController()
        {
            userRepo = new UserRepository();
        }

        [HttpGet]
        public HttpResponseMessage get()
        {
            return Request.CreateResponse(HttpStatusCode.OK, 0);
        }

        [Route("api/User/register")]
        [HttpPost]
        public HttpResponseMessage Register([FromBody] RegistrationUser user)
        {
            int affectedRows = 0;

            try
            {
                affectedRows = userRepo.registerUser(user);
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }

            return Request.CreateResponse(HttpStatusCode.OK, affectedRows);
        //    try
        //    {
        //        System.Diagnostics.Debug.WriteLine("0");
        //        var filepath = @"D:\school\18-19\Sem_1\Mobile_App_voor_Windows\Server_Keys\keys\private";
        //        System.Diagnostics.Debug.WriteLine("1");
        //        var key = File.ReadAllText(filepath);
        //        var buf = new MemoryStream(Encoding.UTF8.GetBytes(key));
        //        var privateKeyFile = new PrivateKeyFile(filepath,"pazaak");


        //        ConnectionInfo connectionInfo = new ConnectionInfo("ghenterpriseapp.westeurope.cloudapp.azure.com", 22 , "jari", 
        //            new AuthenticationMethod[] {
        //                new PrivateKeyAuthenticationMethod("jari", new []
        //                {
        //                    new PrivateKeyFile(filepath, "pazaak")
        //                })
        //            }
        //        );
        //        connectionInfo.Timeout = TimeSpan.FromSeconds(30);
        //        using (SshClient client = new SshClient(connectionInfo))
        //        {
        //            client.Connect();
        //            ForwardedPortLocal port = new ForwardedPortLocal("127.0.0.1",22, "13.94.138.165", 3306);
               
        //            client.AddForwardedPort(port);
        //            port.Start();
                   
        //            using (MySqlConnection conn = new MySqlConnection("server=127.0.0.1;port=22;database=Ghenterprise;Uid=jari;Pwd=pazaak;"))
        //            {
        //                conn.Open();

        //                var insertVar = " ( ";
        //                var insertVal = " ( ";

        //                foreach (var prop in user.GetType().GetProperties())
        //                {
        //                    System.Diagnostics.Debug.WriteLine(prop.Name + " + " + prop.GetValue(user, null));
        //                    insertVar += prop.Name + ((user.GetType().GetProperties().Last().Name == prop.Name)? " ) ": " , ");
        //                    insertVal += "'" + prop.GetValue(user, null) + "'" + ((user.GetType().GetProperties().Last().Name == prop.Name) ? " ) " : " , ");
        //                }
        //                System.Diagnostics.Debug.WriteLine(insertVar + " -- " + insertVal);
        //                System.Diagnostics.Debug.WriteLine(user.GetType().Name);

        //                var query = String.Format("INSERT INTO {0} {1} VALUES {2}", user.GetType().Name.ToLower(),  insertVar, insertVal);
        //                System.Diagnostics.Debug.WriteLine(query);
        //                query += ";";
        //                using (MySqlCommand command = new MySqlCommand(query, conn))
        //                {
        //                    affectedRows = command.ExecuteNonQuery();
        //                }
        //            }
        //            client.Disconnect();
        //        }
                
        //    }
        //    catch(Exception ex)
        //    {
        //        System.Diagnostics.Debug.WriteLine(ex.StackTrace);
        //        return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
        //    }
        //    return Request.CreateResponse(HttpStatusCode.OK, affectedRows);
        }

        [Route("api/User/check-email")]
        [HttpGet]
        public HttpResponseMessage checkEmail([FromUri] String email)
        {
            Response res = new Response
            {
                message = "email doesn't exist"
            };
            try
            {
                res.message = (userRepo.checkEmail(email)) ? "email exists" : "email doesn't exist";
            }
            catch (Exception ex)
            {
                var errorReq = Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                errorReq.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                return errorReq;
            }
            var req = Request.CreateResponse(HttpStatusCode.OK, res);
            req.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return req;
        }

        [Route("api/User/login")]
        [HttpGet]
        public HttpResponseMessage login([FromBody] LoginUser user)
        {
            if(!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Modelstate invalid");
            }

            Response res = new Response
            {
                message = "Password invalid"
            };
            try
            {
                res.message = userRepo.login(user);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }
    }
}
