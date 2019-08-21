using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ghenterprise_Backend.Models
{
    public class Promotion
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public string Start_Date { get; set; }
        public string End_Date { get; set; }
        public Enterprise Enterprise { get; set; }
    }
}