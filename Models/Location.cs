using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ghenterprise_Backend.Models
{
    public class Location
    {
        public string Id { get; set; }
        public int Street_Number { get; set; }
        public City City { get; set; }
        public Street Street { get; set; }
        public string street_id {
            get
            {
                return Street.Id;
            }
        }
        public string city_id {
            get
            {
                return City.Id;
            }
        }
    }
}