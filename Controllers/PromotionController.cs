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
    public class PromotionController : ApiController
    {
        public PromotionRepository PromRepo { get; set; }

        public PromotionController()
        {
            PromRepo = new PromotionRepository();
        }

        [HttpPost]
        public HttpResponseMessage InsertPromotion([FromBody] Promotion prom)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Modelstate invalid");
            }

            int affectedRows = 0;
            try
            {
                affectedRows = PromRepo.SavePromotion(prom);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
            return Request.CreateResponse(HttpStatusCode.OK, affectedRows);
        }

        [HttpPut]
        public HttpResponseMessage UpdatePromotion([FromBody] Promotion prom)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Modelstate invalid");
            }

            int affectedRows = 0;
            try
            {
                affectedRows = PromRepo.UpdatePromotion(prom);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
            return Request.CreateResponse(HttpStatusCode.OK, affectedRows);
        }

        [HttpDelete]
        public HttpResponseMessage DeletePromotion([FromUri] string Promotion_ID)
        {
            int affectedRows = 0;
            try
            {
                affectedRows = PromRepo.DeletePromotionById(Promotion_ID);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
            return Request.CreateResponse(HttpStatusCode.OK, affectedRows);
        }

        [HttpGet]
        public HttpResponseMessage GetPromotionByEnterpriseId([FromUri] string Enterprise_ID)
        {
            List<Promotion> promList = new List<Promotion>();
            try
            {
                promList = PromRepo.GetPromotionsByEnterprise(Enterprise_ID);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
            return Request.CreateResponse(HttpStatusCode.OK, promList);
        }

        [HttpGet]
        public HttpResponseMessage GetAllPromotions()
        {
            List<Promotion> promList = new List<Promotion>();
            try
            {
                promList = PromRepo.GetAllPromotions();
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
            return Request.CreateResponse(HttpStatusCode.OK, promList);
        }
    }
}
