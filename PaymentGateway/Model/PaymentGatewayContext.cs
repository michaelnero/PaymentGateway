using Microsoft.EntityFrameworkCore;

namespace PaymentGateway.Model
{
    public class PaymentGatewayContext : DbContext
    {
        public PaymentGatewayContext(DbContextOptions<PaymentGatewayContext> options)
            : base(options)
        {

        }

        public DbSet<Charge> Charges { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Charge>()
                .HasIndex(o => o.IdempotentKey).IsUnique();
        }
    }
}
