using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace Assignment1_Salesboard.Models
{
    public class Items
    {
        public int Id { get; set; }
        public string ItemName { get; set; }

        [Display(Name = "Item Description"), StringLength(255)]
        public string ItemDescription { get; set; }

        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string Seller { get; set; }
    }
}
