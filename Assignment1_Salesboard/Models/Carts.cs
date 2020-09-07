using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Assignment1_Salesboard.Models
{
    public class Carts
    {
        public int Id { get; set; }
        public int CartId { get; set; }
        public string ItemName { get; set; }
        public string ItemImage { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }



    }
}
