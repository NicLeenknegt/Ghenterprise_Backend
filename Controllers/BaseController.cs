using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Web.Http;
using System.Xml.Linq;

namespace Ghenterprise_Backend.Controllers
{
    public class BaseController : ApiController
    {
        private static global::System.Resources.ResourceManager resMan;
        protected string ValidateToken()
        {
            string User_ID = "";
            try
            {

                Debug.WriteLine("Token validation");
                if (Request.Headers.Contains("username"))
                {
                    
                    var key = Encoding.ASCII.GetBytes(GetSecret());
                    var handler = new JwtSecurityTokenHandler();
                    var validations = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                    var claims = handler.ValidateToken(Request.Headers.GetValues("username").First(), validations, out var tokenSecure);
                    User_ID = claims.Identity.Name;
                }
            }
            catch (Exception)
            {
                Debug.WriteLine("Token validation failed");
                throw new Exception("Token validation failed");
            }
            return User_ID;
        }

        protected string GetToken(string User_ID)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(GetSecret());
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, User_ID)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private string GetSecret()
        {
            XDocument doc = XDocument.Load("c:\\env.xml");
            return doc.Root.Value.Trim();
        }
    }
}
