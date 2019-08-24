using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Ghenterprise_Backend.Models
{
    public class User_Has_Subscription
    {
        [Required]
        public string User_ID { get; set; }
        [Required]
        public string Enterprise_ID { get; set; }
    }
}