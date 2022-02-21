using System.Collections.Generic;
using Competition3.Data.Map;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Competition3.Data
{
    public class AppDbContext : IdentityDbContext<User, Role, int>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new UserMap());
            builder.ApplyConfiguration(new RoleMap());
            builder.ApplyConfiguration(new PaymentMap());

            builder.Entity<IdentityUserRole<int>>()
                .HasKey(x => new {x.UserId, x.RoleId});
            builder.Entity<IdentityUserLogin<int>>()
                .HasKey(x => new {x.UserId, x.ProviderKey, x.LoginProvider});
            builder.Entity<IdentityUserToken<int>>()
                .HasKey(x => new {x.UserId, x.Name});

            builder.Entity<IdentityUserRole<int>>().HasData(new List<IdentityUserRole<int>>
            {
                new() {RoleId = 2,UserId = 1}
            });
        }
    }
}