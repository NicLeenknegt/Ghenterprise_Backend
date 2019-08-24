using Ghenterprise_Backend.Models;
using Ghenterprise_Backend.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Ghenterprise_Backend.Controllers
{
    public class NotificationController : ApiController
    {
        public NotificationRepository notRepo { get; set; }

        public NotificationController()
        {
            notRepo = new NotificationRepository();
        }

        [HttpPost]
        public HttpResponseMessage InsertUserNotifications([FromUri] int User_ID)
        {
            int affectedRows = 0;
            try
            {
                affectedRows = notRepo.InsertUserNotifications(User_ID);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
            return Request.CreateResponse(HttpStatusCode.OK, affectedRows);
        } 

        [HttpPost]
        public HttpResponseMessage SetUserNoticationsAsSeen([FromBody] List<User_Has_Notification> uhnList)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Modelstate invalid");
            }

            int affectedRows = 0;
            try
            {
                affectedRows = notRepo.SetUserNotificationAsSeen(uhnList);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
            return Request.CreateResponse(HttpStatusCode.OK, affectedRows);
        }

        [HttpGet]
        public HttpResponseMessage GetNotificationsByUser([FromUri] int User_ID)
        {
            List<Notification> notList = new List<Notification>();
            try
            {
                notList = notRepo.GetNotificationsByUser(User_ID);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
            return Request.CreateResponse(HttpStatusCode.OK, notList);
        } 
    }
}
