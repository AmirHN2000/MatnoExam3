using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Competition3.Data.Map
{
    public class PaymentMap:IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.Property(x => x.SystemCode)
                .IsRequired()
                .HasMaxLength(100);
            builder.Property(x => x.RefCode)
                .IsRequired(false)
                .HasMaxLength(50);
            builder.Property(x => x.Description)
                .IsRequired()
                .IsUnicode()
                .HasMaxLength(200);

            builder.HasOne(x => x.User)
                .WithMany(x => x.Payments)
                .HasForeignKey(x => x.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}