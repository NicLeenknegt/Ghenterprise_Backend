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
    public class LocationController : ApiController
    {
        public LocationRepository LocRepo { get; set; }

        public LocationController()
        {
            LocRepo = new LocationRepository();
        }

        [HttpGet]
        [Route("api/Location/City")]
        public HttpResponseMessage GetAllCities()
        {
            List<City> cityList = new List<City>();
            try
            {
                cityList = LocRepo.GetAllCities();
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
            return Request.CreateResponse(HttpStatusCode.OK, cityList);
        }

        [HttpGet]
        [Route("api/Location/Street")]
        public HttpResponseMessage GetAllStreets()
        {
            List<Street> streetList = new List<Street>();
            try
            {
                streetList = LocRepo.GetAllStreets();
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
            return Request.CreateResponse(HttpStatusCode.OK, streetList);
        }

        [HttpPost]
        [Route("api/Location/City")]
        public HttpResponseMessage InsertCity(City city)
        {
            int affectedRows = 0;
            try
            {
                affectedRows = LocRepo.InsertCity(city);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
            return Request.CreateResponse(HttpStatusCode.OK, affectedRows);
        }

        [HttpPost]
        [Route("api/Location/Street")]
        public HttpResponseMessage InsertStreet(Street street)
        {
            int affectedRows = 0;
            try
            {
                affectedRows = LocRepo.InsertStreet(street);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
            return Request.CreateResponse(HttpStatusCode.OK, affectedRows);
        }
    }
}
