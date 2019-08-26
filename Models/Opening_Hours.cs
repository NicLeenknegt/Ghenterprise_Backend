using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ghenterprise_Backend.Models
{
    public class Opening_Hours
    {
        public string Id { get; set; }
        public int Day_Of_Week { get; set; }
        public string Start { get; set; }
        public string End { get; set; }
        public string Enterprise_Id { get; set; }
    }
}