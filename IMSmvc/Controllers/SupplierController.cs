﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IMSmvc.Controllers
{
    public class SupplierController : Controller
    {
        // GET: Supplier
        //public ActionResult Index()
        //{
        //    return View();
        //}
        public ActionResult Product(int id)
        {
            ViewBag.Message = id;
            return View(id);
        }
        public ActionResult Order()
        {
            return View();
        }
        public ActionResult ReturnTrack()
        {
            return View();
        }
    }
}