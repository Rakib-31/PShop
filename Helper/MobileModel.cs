using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PShop.Helper
{
    public class MobileModel
    {
        public int Id { get; set; }
        public string MobileName { get; set; }
        public string BrandName { get; set; }
        public int MobileInfoId { get; set; }
        public double Price { get; set; }
        public double Discount { get; set; }
        public int Ram { get; set; }
        public int Rom { get; set; }
        public double Size { get; set; }
        public string SKU { get; set; }
    }
}