using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace Assignment1_Salesboard.Models
{
    public class Items
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string ItemName { get; set; }
        [Required]

        [Display(Name = "Item Description"), StringLength(255)]
        public string ItemDescription { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public decimal Price { get; set; }
        public string Seller { get; set; }
    }
}
