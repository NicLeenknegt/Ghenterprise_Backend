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
        public string ConnString { get; set; }
        public SSH Ssh { get; set; }
        public KeyGenerator KeyGen { get; set; }
        public CategoryRepository CatRepo { get; set; }

        public CategoryController()
        {
            ConnString = "server=127.0.0.1;port=22;database=Ghenterprise;Uid=jari;Pwd=pazaak;";
            Ssh = new SSH();
            KeyGen = new KeyGenerator();
            CatRepo = new CategoryRepository();
        }

        [HttpPost]
        public HttpResponseMessage SaveCategories([FromBody] Category[] catArray)
        {
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
