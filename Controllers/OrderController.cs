using PShop.Helper;
using PShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Security.Cryptography;
using System.Text;

namespace PShop.Controllers
{
    public class OrderController : Controller
    {
        // GET: Order
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult GetOrder(string userId)
        {
            bool success = false;
            using (PSHOPEntityDataContext dbContext = new PSHOPEntityDataContext())
            {
                var model = dbContext.OrdersTbls.Where(x => x.UserId == userId).ToList();
                if (model != null)
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

        public IQueryable<OrderDetails> OrderDetails(int id)
        {
            using (PSHOPEntityDataContext dbContext = new PSHOPEntityDataContext())
            {
                var model = from o in dbContext.OrdersTbls.Where(x => x.Id == id)
                            from op in dbContext.OrderProductTbls
                            where (o.OrderId == op.OrderId)
                            select new OrderDetails
                            {
                                OrderId = o.OrderId.ToString(),
                                DeliveryDate = o.DeliveryDate,
                                OrderDate = o.OrderDate,
                                PaymentStatus = o.PaymentStatus,
                                TotalPayment = o.TotalPayment,
                                Category = op.Category,
                                ProductId = op.ProductId,
                                ShippedDate = o.ShippedDate,
                                OrderStatus = o.OrderStatus
                            };
                return model;
            } 
        }
        public OrderDetails GetOrderedMobileDetails(OrderDetails temp)
        {
            using (PSHOPEntityDataContext dbContext = new PSHOPEntityDataContext())
            {
                OrderDetails product = dbContext.MobileTbls.Where(x => x.Id == temp.ProductId).Select(x =>
                    new OrderDetails
                    {
                        OrderId = temp.OrderId.ToString(),
                        DeliveryDate = temp.DeliveryDate,
                        OrderDate = temp.OrderDate,
                        PaymentStatus = temp.PaymentStatus,
                        TotalPayment = temp.TotalPayment,
                        Category = temp.Category,
                        ProductId = temp.ProductId,
                        ShippedDate = temp.ShippedDate,
                        OrderStatus = temp.OrderStatus,
                        ProductName = x.MobileName,
                        BrandName = x.BrandName
                    }
                ).FirstOrDefault();
                return product;
            }
                
        }
        public JsonResult GetSingularOrder(int id)
        {
            List<OrderDetails> Products = new List<OrderDetails>();
            bool success = false;
            using (PSHOPEntityDataContext dbContext = new PSHOPEntityDataContext())
            {
                var model = OrderDetails(id);
                if(model != null)
                {
                    success = true;
                    List<OrderDetails> orders = model.ToList();
                    
                    OrderDetails product = new OrderDetails();
                    foreach(OrderDetails temp in orders)
                    {
                        if (temp.Category == "Mobile")
                        {
                            product = GetOrderedMobileDetails(temp);
                        }
                        if(product != null)
                        {
                            Products.Add(product);
                        }
                       
                    }
                }

                return Json(new
                {
                    success,
                    data = Products
                });
            }
        }
        //public string MakeOrderId()
        //{
        //    int length = 7;

        //    string str_build = "";
        //    Random random = new Random();

        //    char letter;

        //    for (int i = 0; i < length; i++)
        //    {
        //        double flt = random.NextDouble();
        //        int shift = Convert.ToInt32(Math.Floor(25 * flt));
        //        letter = Convert.ToChar(shift + 65);
        //        str_build += letter;
        //    }
        //    byte[] tmpHash;
        //    byte [] tmpSource = ASCIIEncoding.ASCII.GetBytes(str_build);

        //    tmpHash = new MD5CryptoServiceProvider().ComputeHash(tmpSource);
        //    return tmpHash.ToString();
        //}
        public OrderProductTbl MakeListData(int id, string Category, int Quantity, string Color)
        {
            using (PSHOPEntityDataContext dbContext = new PSHOPEntityDataContext())
            {
                OrderProductTbl data = new OrderProductTbl();
                if (Category == "Mobile")
                {
                    data = dbContext.MobileDetailTbls.Where(x => x.Id == id).Select(x => new OrderProductTbl
                    {
                        ProductId = x.Id,
                        Category = Category,
                        Quantity = Quantity,
                        Color = Color,
                        ProductPrice = x.Price
                    }).FirstOrDefault();

                }

                return data;
            }
        }
        [HttpPost]
        public JsonResult Order(string UserId, int id, string Category = "", int Quantity = 0, string Color = "")
        {
            using(PSHOPEntityDataContext dbContext = new PSHOPEntityDataContext())
            {
                bool success = false;
                string msg = "Order not completed.";
                try
                {
                    List<OrderProductTbl> data = new List<OrderProductTbl>();
                    OrderProductTbl OrderData = new OrderProductTbl();
                    double TotalPayment = 0.0;
                    var OrderId = Guid.NewGuid();
                
                    if (id != 0)
                    {
                        OrderData = MakeListData(id, Category, Quantity, Color);
                    
                        if(OrderData != null)
                        {
                            TotalPayment += OrderData.ProductPrice;
                            OrderData.OrderId = OrderId;
                            data.Add(OrderData);
                            success = true;
                            msg = "Order successfully placed.";
                        }
                    }
                    else
                    {
                        var model = dbContext.CartsTbls.Where(x => x.UserId == UserId).ToList();
                        foreach (var x in model)
                        {
                            OrderData = MakeListData(x.ProductId, x.Category, x.Quantity, x.Color);
                            if (OrderData != null)
                            {
                                TotalPayment += OrderData.ProductPrice;
                                OrderData.OrderId = OrderId;
                                data.Add(OrderData);
                                success = true;
                                msg = "Order successfully placed.";
                            } 
                        }
                    }

                    dbContext.OrderProductTbls.InsertAllOnSubmit(data);

                    List<OrdersTbl> Orders = new List<OrdersTbl>();

                    var OrderDetails = new OrdersTbl
                    {
                        UserId = UserId,
                        OrderId = OrderId,
                        TotalPayment = TotalPayment,
                        PaymentStatus = false,
                        OrderStatus = true,
                        OrderDate = DateTime.UtcNow,
                    };

                    Orders.Add(OrderDetails);
                    dbContext.OrdersTbls.InsertAllOnSubmit(Orders);
                    dbContext.SubmitChanges();
                    //clear cart
                }
                catch (Exception ex) {
                    success = false;
                    string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                    string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                    //AIManager.Instance.LogError(ex, tenant: "", controllerName, actionName);
                }

                return Json(new
                {
                    msg,
                    success
                });
            }
        }
        [HttpPost]
        public JsonResult CancelOrder(int id)
        {
            using(PSHOPEntityDataContext dbContext = new PSHOPEntityDataContext())
            {
                bool success = true;
                var model = dbContext.OrdersTbls.Where(x => x.Id == id).FirstOrDefault();
                if (model != null) {
                    model.OrderStatus = false;
                }
                try
                {
                    dbContext.SubmitChanges();
                }
                catch (Exception ex)
                {
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
        public JsonResult DeleteOrder(int id)
        {
            using (PSHOPEntityDataContext dbContext = new PSHOPEntityDataContext())
            {
                bool success = true;
                var model = dbContext.OrdersTbls.Where(x => x.Id == id).FirstOrDefault();
                
                if (model != null)
                {
                    var orderProductmodel = dbContext.OrderProductTbls.Where(x => x.OrderId == model.OrderId).ToList();
                    dbContext.OrderProductTbls.DeleteAllOnSubmit(orderProductmodel);
                    dbContext.OrdersTbls.DeleteOnSubmit(model);
                }
                try
                {
                    dbContext.SubmitChanges();
                }
                catch (Exception ex)
                {
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