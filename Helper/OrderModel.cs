using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PShop.Helper
{
    public class OrderModel
    {
        public int Id { get; set; }
        public string OrderId { get; set; }
        public bool? PaymentStatus { get; set; }

        public double TotalPayment { get; set; }

        public bool OrderStatus { get; set; }

        public DateTime? OrderDate { get; set; }

        public DateTime? DeliveryDate { get; set; }

        public DateTime? ShippedDate { get; set; }

        public string UserId { get; set; }
    }

    public class OrderDetails : OrderModel
    {
        public string Category { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string BrandName { get; set; }
    }
}