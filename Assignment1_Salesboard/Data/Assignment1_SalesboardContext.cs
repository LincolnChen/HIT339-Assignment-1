using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Assignment1_Salesboard.Models;

namespace Assignment1_Salesboard.Data
{
    public class Assignment1_SalesboardContext : DbContext
    {
        public Assignment1_SalesboardContext (DbContextOptions<Assignment1_SalesboardContext> options)
            : base(options)
        {
        }

        public DbSet<Assignment1_Salesboard.Models.Sales> Sales { get; set; }

        public DbSet<Assignment1_Salesboard.Models.Items> Items { get; set; }
    }
}
