using PShop.Helper;
using PShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PShop.Controllers
{
    public class OrderController : Controller
    {
        // GET: Order
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetOrder(Guid id)
        {
            bool success = false;
            using (PSHOPEntityDataContext dbContext = new PSHOPEntityDataContext())
            {
                var model = dbContext.OrderTbls.Where(x => x.Id == id).FirstOrDefault();
                if(model != null)
                {
                    success = true;
                }

                return Json(new
                {
                    success,
                    data = model
                });
            }
        }
        [HttpPost]
        public JsonResult Order(List<OrderModel> model)
        {
            using(PSHOPEntityDataContext dbContext = new PSHOPEntityDataContext())
            {
                bool success = true;
                List<OrderTbl> data = new List<OrderTbl>();
                foreach(OrderModel x in model)
                {
                    data.Add(new OrderTbl { 
                        ProductName = x.ProductName,
                        BrandName = x.BrandName,
                        Category = x.Category,
                        Quantity = x.Quantity,
                        Payment = x.Payment,
                        PaymentStatus = x.PaymentStatus,
                        Color = x.Color,
                        UserId = x.UserId
                    });
                }
                dbContext.OrderTbls.InsertAllOnSubmit(data);
                try
                {
                    dbContext.SubmitChanges();
                }
                catch (Exception ex) {
                    success = false;
                    string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                    string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                    //AIManager.Instance.LogError(ex, tenant: "", controllerName, actionName);
                }

                return Json(new
                {
                    msg = "Order success",
                    success
                });
            }
        }
    }
}