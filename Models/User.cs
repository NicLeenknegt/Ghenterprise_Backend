using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Ghenterprise_Backend.Models
{
    public class User
    {
        public int id { get; set; }
        [Required]
        public String firstname { get; set; }
        public String lastname { get; set; }
        public String email { get; set; }
        public String password { get; set; }
    }
}