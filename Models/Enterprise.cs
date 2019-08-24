using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Ghenterprise_Backend.Models
{
    public class Enterprise
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Date_Created { get; set; }
        public List<Category> Categories { get; set; }
        public List<Tag> Tags { get; set; }
        public Location Location { get; set; }
        public List<Event> Events { get; set; }
        public List<Promotion> Promotions { get; set; }
    }
}