using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PShop.Helper
{
    public class CartModel
    {
        public Guid Id { get; set; }

        public string ProductName { get; set; }

        public string BrandName { get; set; }

        public string Category { get; set; }

        public int Quantity { get; set; }

        public string Color { get; set; }

        public string UserId { get; set; }
    }
}