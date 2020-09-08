using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Assignment1_Salesboard.Models;

namespace Assignment1_Salesboard.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Assignment1_Salesboard.Models.Items> Items { get; set; }
        public DbSet<Assignment1_Salesboard.Models.Sales> Sales { get; set; }
        public DbSet<Assignment1_Salesboard.Models.ShoppingCart> ShoppingCart { get; set; }




        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }






 






    }
}
