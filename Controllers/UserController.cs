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
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace Ghenterprise_Backend.Controllers
{
    public class UserController : BaseController
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
        public HttpResponseMessage Register([FromBody] User user)
        {
            int affectedRows = 0;
            try
            {
                affectedRows = userRepo.registerUser(user);
                user.password = "User wasn't registered";
                if (affectedRows > 0)
                {
                    user.password = "User is registered";
                    user.Token = GetToken(user.id);
                }
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }

            return Request.CreateResponse(HttpStatusCode.OK, user);
        }

        [Route("api/User/check-email")]
        [HttpGet]
        public HttpResponseMessage checkEmail([FromUri] string email)
        {
            User user = new User();
            try
            {
                user.Token = (userRepo.checkEmail(user.email)) ? "email exists" : "email doesn't exist";
            }
            catch (Exception ex)
            {
                var errorReq = Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                errorReq.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                return errorReq;
            }
            var req = Request.CreateResponse(HttpStatusCode.OK, user);
            req.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return req;
        }

        [Route("api/User/login")]
        [HttpPost]
        public HttpResponseMessage login([FromBody] User user)
        {
            Response res = new Response
            {
                message = "Password invalid"
            };
            try
            {
                user = userRepo.login(user);
                if (user.password == "Password valid")
                {
                    user.Token = GetToken(user.id);
                }
            }
            catch (Exception ex)
            {
                var errorReq = Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                errorReq.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                return errorReq;
            }
            var req = Request.CreateResponse(HttpStatusCode.OK, user);
            req.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return req;
        }
    }
}
