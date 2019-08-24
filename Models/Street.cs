using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Ghenterprise_Backend.Models
{
    public class Street
    {
        public string Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}