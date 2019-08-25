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
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Modelstate invalid");
            }

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
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Modelstate invalid");
            }

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

        [HttpDelete]
        public HttpResponseMessage DeleteEvent([FromUri] string Event_ID)
        {
            int affectedRows = 0;
            try
            {
                affectedRows = EventRepo.DeleteEvent(Event_ID);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
            return Request.CreateResponse(HttpStatusCode.OK, affectedRows);
        }

        [HttpGet]
        public HttpResponseMessage GetEventsOfEnterprise([FromUri] string Enterprise_ID)
        {
            List<Event> eventList = new List<Event>();
            try
            {
                eventList = EventRepo.GetEventsOfEnterprise(Enterprise_ID);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
            return Request.CreateResponse(HttpStatusCode.OK, eventList);
        }

        [HttpGet]
        public HttpResponseMessage GetEventById([FromUri] string Event_ID)
        {
            Event backendEvent = new Event();
            try
            {
                backendEvent = EventRepo.GetEventById(Event_ID);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
            return Request.CreateResponse(HttpStatusCode.OK, backendEvent);
        }

        [HttpGet]
        public HttpResponseMessage GetAllEvents()
        {
            List<Event> eventList = new List<Event>();
            try
            {
                eventList = EventRepo.GetAllEvents();
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
            return Request.CreateResponse(HttpStatusCode.OK, eventList);
        }


    }
}
