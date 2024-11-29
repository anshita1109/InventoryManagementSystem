using IMSmvc.Models;
//using Intuit.Ipp.Data;
//using Intuit.Ipp.OAuth2PlatformClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;


namespace IMSmvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient _httpClient;
        //public ActionResult Index()
        //{
        //    return View();
        //}
        public HomeController()
        {
            _httpClient = new HttpClient();
        }
        [Authorize]

        public async Task<ActionResult> Index()
        {

            SessionModel sessionModel = new SessionModel();
            Session.Add("UserName", User.Identity.Name);

            SupplierDTO userResponse = null;
            if (User.Identity.Name == "admin@gmail.com")
            {

                sessionModel.Role = "Admin";
                sessionModel.Address = "dsfdfsdfds";
                sessionModel.Name = User.Identity.Name;

                Session.Add("SessionModel", sessionModel);


                Session.Add("Role", "Admin");
                ViewBag.Role = "Admin";
                // Logic for admin users

            }


            else
            {
                HttpResponseMessage httpResponseMessage = await _httpClient.GetAsync($"https://localhost:7175/api/Suppliers/supplierContact?contactinfo=" + User.Identity.Name);
                string responseData = await httpResponseMessage.Content.ReadAsStringAsync();

                userResponse = JsonConvert.DeserializeObject<SupplierDTO>(responseData);

                Session.Add("Role", "Supplier");
                ViewBag.Role = "Supplier";
                //return RedirectToAction("Product", "Supplier",new { id=userResponse.SupplierId});
                ViewBag.Message = userResponse.SupplierId;
            }
           
            return View();
        }

    }
}