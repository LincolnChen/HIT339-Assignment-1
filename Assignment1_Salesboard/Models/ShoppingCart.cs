using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Assignment1_Salesboard.Models
{
    public class ShoppingCart
    {
        public int Id { get; set; }
        public string CartId { get; set; }
        public int Item { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string Seller { get; set; }

    }
}
