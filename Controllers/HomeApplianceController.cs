﻿using PShop.Helper;
using PShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PShop.Controllers
{
    public class HomeApplianceController : Controller
    {
        // GET: HomeAppliance
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult GetHomeAppliance()
        {
            using (PSHOPEntityDataContext dbContext = new PSHOPEntityDataContext())
            {

                List<MobileModel> mobiles = (List<MobileModel>)dbContext.MobileTbls.Select(x => x);
                return Json(new
                {
                    success = true,
                    data = mobiles,
                    Message = "success performed."
                }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}