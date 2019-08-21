using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ghenterprise_Backend.Models
{
    public class User_Has_Notification
    {
        public int User_Id { get; set; }
        public bool Seen { get; set; }
        public string Notification_Id { get; set; }
    }
}