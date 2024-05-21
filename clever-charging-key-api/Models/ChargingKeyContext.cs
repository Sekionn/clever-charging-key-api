using Microsoft.EntityFrameworkCore;

namespace clever_charging_key_api.Models
{
    /// <summary>
    /// This context class is for creating the in memory database
    /// </summary>
    public class ChargingKeyContext : DbContext
    {
        public ChargingKeyContext (DbContextOptions<ChargingKeyContext> options) : base(options) { }

        public DbSet<ChargingKey> keys { get; set; } = null!;
    }
}
