using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;



namespace Assignment1_Salesboard.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public int Age { get; set; }

        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }


    }
}
