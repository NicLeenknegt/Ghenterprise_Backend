using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ghenterprise_Backend.Models
{
    public class Enterprise
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string DateCreated { get; set; }
    }
}