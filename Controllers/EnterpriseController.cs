using Ghenterprise_Backend.Models;
using Ghenterprise_Backend.Repositories;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace Ghenterprise_Backend.Controllers
{
    public class EnterpriseController : BaseController
    {
        public EnterpriseRepository EnterRepo { get; set; }

        public EnterpriseController()
        {
            EnterRepo = new EnterpriseRepository();
        }

        public HttpResponseMessage GetAllEnterprise()
        {
            List<Enterprise> entList = new List<Enterprise>();

            try
            {
                entList = EnterRepo.GetAllEnterprise();
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
            return Request.CreateResponse(HttpStatusCode.OK, entList);
        }

        [HttpGet]
        public HttpResponseMessage GetEnerpriseById([FromUri] string enterprise_id)
        {
            Enterprise entList = new Enterprise();

            try
            {
                entList = EnterRepo.GetEnterpriseById(enterprise_id);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
            return Request.CreateResponse(HttpStatusCode.OK, entList);
        }

        [HttpPost]
        public HttpResponseMessage SaveEnterprise( [FromBody] Enterprise enterprise)
        {
            //if (!ModelState.IsValid)
            //{
            //    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Modelstate invalid");
            //}
            Debug.WriteLine("Save enterprise");
            int affectedRows = 0;

            try
            {
                string User_ID = ValidateToken();
                affectedRows = EnterRepo.SaveEnterprise(User_ID, enterprise);
            }
            catch(Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
            return Request.CreateResponse(HttpStatusCode.OK, affectedRows);
        }

        [HttpGet]
        [Route("api/Enterprise/Owner")]
        public HttpResponseMessage GetEnterpriseByOwner()
        {
            List<Enterprise> entList = new List<Enterprise>();
            try
            {
                string Owner_ID = ValidateToken();
                entList = EnterRepo.GetEnterprisesByOwner(Owner_ID);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
            return Request.CreateResponse(HttpStatusCode.OK, entList);
        }

        [HttpGet]
        [Route("api/Enterprise/Subscription")]
        public HttpResponseMessage GetEnterpriseSubscriptions()
        {
            List<Enterprise> entList = new List<Enterprise>();
            try
            {
                string User_ID = ValidateToken();
                entList = EnterRepo.GetSubsciptionEnterprise(User_ID);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
            return Request.CreateResponse(HttpStatusCode.OK, entList);
        }

        [HttpPost]
        public HttpResponseMessage SubscribeToEnterprise([FromUri] string entID)
        {
            int affectedRows = 0;
            try
            {
                string User_ID = ValidateToken();
                affectedRows = EnterRepo.SubscribeToEnterprise(User_ID, entID);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
            return Request.CreateResponse(HttpStatusCode.OK, affectedRows);
        }

        [HttpPut]
        public HttpResponseMessage UpdateEnterprise([FromBody] Enterprise enterprise)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Modelstate invalid");
            }

            int affectedRows = 0;
            try
            {
                ValidateToken();
                affectedRows = EnterRepo.UpdateEnterprise(enterprise);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
            return Request.CreateResponse(HttpStatusCode.OK, affectedRows);
        }
        
        [HttpDelete]
        public HttpResponseMessage DeleteEnterprise([FromUri] string enterpriseID)
        {
            int affectedRows = 0;
            try
            {
                ValidateToken();
                affectedRows = EnterRepo.DeleteEnterpise(enterpriseID);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
            return Request.CreateResponse(HttpStatusCode.OK, affectedRows);
        }

        [HttpDelete]
        public HttpResponseMessage DeleteSubscriptionByEnterpriseId( [FromUri] string enterpriseID)
        {
            int affectedRows = 0;
            try
            {
                string User_ID = ValidateToken();
                affectedRows = EnterRepo.DeleteSubscriptionByEnterpriseId(User_ID, enterpriseID);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
            return Request.CreateResponse(HttpStatusCode.OK, affectedRows);
        }

        [HttpDelete]
        public HttpResponseMessage DeleteAllSubscriptions()
        {
            int affectedRows = 0;
            try
            {
                string User_ID = ValidateToken();
                affectedRows = EnterRepo.DeleteAllSubscriptions(User_ID);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
            return Request.CreateResponse(HttpStatusCode.OK, affectedRows);
        }

                
    }
}
