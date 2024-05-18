using Microsoft.EntityFrameworkCore;

namespace clever_charging_key_api.Models
{
    public class ChargingKeyContext : DbContext
    {
        public ChargingKeyContext (DbContextOptions<ChargingKeyContext> options) : base(options) { }

        public DbSet<ChargingKey> keys { get; set; } = null!;
    }
}
