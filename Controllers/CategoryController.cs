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
    public class CategoryController : ApiController
    {

        public CategoryRepository CatRepo { get; set; }

        public CategoryController()
        {
            CatRepo = new CategoryRepository();
        }

        [HttpPost]
        public HttpResponseMessage SaveCategories([FromBody] Category[] catArray)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Modelstate invalid");
            }

            int affectedRows = 0;
            try
            {
                affectedRows = CatRepo.SaveCategory(catArray);
            } catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
            return Request.CreateResponse(HttpStatusCode.OK, affectedRows);
        } 

        [HttpGet]
        public HttpResponseMessage GetAllCategories()
        {
            List<Category> catList = new List<Category>();
            try
            {
                catList = CatRepo.GetAllCategories();
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
            return Request.CreateResponse(HttpStatusCode.OK, catList);
        }
    }
}
