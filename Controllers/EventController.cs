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
    public class EventController : ApiController
    {
        private EventRepository EventRepo { get; set; }

        public EventController()
        {
            EventRepo = new EventRepository();
        }

        [HttpPost]
        public HttpResponseMessage InsertEvent([FromBody] Event backendEvent)
        {
            int affectedRows = 0;
            try
            {
                affectedRows = EventRepo.SaveEvent(backendEvent);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
            return Request.CreateResponse(HttpStatusCode.OK, affectedRows);
        }

        [HttpPut]
        public HttpResponseMessage UpdateEvent([FromBody] Event backendEvent)
        {
            int affectedRows = 0;
            try
            {
                affectedRows = EventRepo.UpdateEvent(backendEvent);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
            return Request.CreateResponse(HttpStatusCode.OK, affectedRows);
        }
    }
}
