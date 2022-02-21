using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Competition3.Data.Map
{
    public class UserMap : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(x => x.FullName)
                .HasMaxLength(50)
                .IsRequired()
                .IsUnicode();

            builder.Ignore(x => x.EmailConfirmed);
            builder.Ignore(x => x.LockoutEnabled);
            builder.Ignore(x => x.LockoutEnd);
            builder.Ignore(x => x.AccessFailedCount);
            builder.Ignore(x => x.PhoneNumberConfirmed);
            builder.Ignore(x => x.TwoFactorEnabled);

            var hasher = new Microsoft.AspNetCore.Identity.PasswordHasher<User>();
            builder.HasData(new User
            {
                Id = 1,
                FullName = "Admin",
                Email = "Admin@gmail.com",
                NormalizedEmail = "ADMIN@GMAIL.COM",
                PhoneNumber = "09131234567",
                UserName = "Admin",
                NormalizedUserName = "ADMIN",
                PasswordHash = hasher.HashPassword(null, "Admin1234"),
                SecurityStamp = Guid.NewGuid().ToString(),
            });
        }
    }
}