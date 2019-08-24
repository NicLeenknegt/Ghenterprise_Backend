using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Ghenterprise_Backend.Models
{
    public class RegistrationUser
    {
        public int id { get; set; }
        [Required]
        public String firstname { get; set; }
        [Required]
        public String lastname { get; set; }
        [Required]
        public String email { get; set; }
        [Required]
        public String password { get; set; }
    }

    public class LoginUser
    {
        [Required]
        public String email { get; set; }
        [Required]
        public String password { get; set; }
    }

    public class User
    {
        public string id { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string Token { get; set; }

    }
}