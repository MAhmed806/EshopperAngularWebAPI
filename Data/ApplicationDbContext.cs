using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using EShopperAngular.Models;

namespace EShopperAngular.Data
{
    public class ApplicationDbContext:IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        { }
        public DbSet<ProductTypes>? ProductTypes { get; set; }
        public DbSet<Products>? Products { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<ProductColors> ProductColors { get; set; }
        public DbSet<OrderDetails>? OrderDetails { get; set; }
        public DbSet<Order>? Order { get; set; }
        //public DbSet<ProductTypes> User { get; set; }
        public DbSet<ApplicationUser> ApplicationUser { get; set; }
        public DbSet<ChargeAgainstOrder> ChargeAgainstOrder { get; set;}

    }
}
