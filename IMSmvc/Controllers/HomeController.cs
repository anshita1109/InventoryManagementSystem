using IMSmvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IMSmvc.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Home()
        {
            return View();
        }
        [Authorize]

        public ActionResult Index()
        {

            SessionModel sessionModel = new SessionModel();
            Session.Add("UserName", User.Identity.Name);

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
                Session.Add("Role", "Supplier");
                ViewBag.Role = "Supplier";


            }
            return View();
        }
    }
}