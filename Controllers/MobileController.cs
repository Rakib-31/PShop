using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PShop.Models;
using PShop.Helper;

namespace PShop.Controllers
{
    public class MobileController : Controller
    {
        // GET: Mobile
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetMobile()
        {
            using (PSHOPEntityDataContext dbContext = new PSHOPEntityDataContext())
            {
                 
                List<MobileModel> mobiles  = (List<MobileModel>) dbContext.MobileTbls.Select(x => x);
                return Json(new
                {
                    success = true,
                    data = mobiles,
                    Message = "success performed."
                }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult GetMobile(int id)
        {
            using (PSHOPEntityDataContext dbContext = new PSHOPEntityDataContext())
            {

                var model = dbContext.MobileTbls.Where(x => x.Id == id).Select(x => x).FirstOrDefault();
                return Json(new
                {
                    success = true,
                    data = model,
                    Message = "success performed."
                }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}