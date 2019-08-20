﻿using Ghenterprise_Backend.Models;
using Ghenterprise_Backend.Repositories;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Ghenterprise_Backend.Controllers
{
    public class EnterpriseController : ApiController
    {
        public EnterpriseRepository EnterRepo { get; set; }

        public EnterpriseController()
        {
            EnterRepo = new EnterpriseRepository();
        }

        [HttpPost]
        public HttpResponseMessage SaveEnterprise([FromUri] int userID, [FromBody] Enterprise enterprise)
        {
            int affectedRows = 0;
            try
            {
                affectedRows = EnterRepo.SaveEnterprise(userID, enterprise);
            }
            catch(Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
            return Request.CreateResponse(HttpStatusCode.OK, affectedRows);
        }

        [HttpGet]
        public HttpResponseMessage GetEnterpriseByOwner([FromUri] int ownerID)
        {
            List<Enterprise> entList = new List<Enterprise>();
            try
            {
                entList = EnterRepo.GetEnterprisesByOwner(ownerID);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
            return Request.CreateResponse(HttpStatusCode.OK, entList);
        }

        [HttpGet]
        public HttpResponseMessage GetEnterpriseSubscriptions([FromUri] int userID)
        {
            List<Enterprise> entList = new List<Enterprise>();
            try
            {
                entList = EnterRepo.GetSubsciptionEnterprise(userID);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
            return Request.CreateResponse(HttpStatusCode.OK, entList);
        }

        [HttpPost]
        public HttpResponseMessage SubscribeToEnterprise([FromUri] int userID, [FromUri] string entID)
        {
            Debug.WriteLine(entID);
            int affectedRows = 0;
            try
            {
                affectedRows = EnterRepo.SubscribeToEnterprise(userID, entID);
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
            int affectedRows = 0;
            try
            {
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
                affectedRows = EnterRepo.DeleteEnterpise(enterpriseID);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
            return Request.CreateResponse(HttpStatusCode.OK, affectedRows);
        }

        [HttpDelete]
        public HttpResponseMessage DeleteSubscriptionByEnterpriseId([FromUri] int userID, [FromUri] string enterpriseID)
        {
            int affectedRows = 0;
            try
            {
                affectedRows = EnterRepo.DeleteSubscriptionByEnterpriseId(userID, enterpriseID);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
            return Request.CreateResponse(HttpStatusCode.OK, affectedRows);
        }

        [HttpDelete]
        public HttpResponseMessage DeleteAllSubscriptions([FromUri] int userID)
        {
            int affectedRows = 0;
            try
            {
                affectedRows = EnterRepo.DeleteAllSubscriptions(userID);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
            return Request.CreateResponse(HttpStatusCode.OK, affectedRows);
        }
    }
}
