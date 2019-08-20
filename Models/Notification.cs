using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ghenterprise_Backend.Models
{
    public class Notification
    {
        public string Id { get; set; }
        public string Enterprise_Id { get; set; }
        public string Event_Id { get; set; }
        public string Promotion_Id { get; set; }
    }
}