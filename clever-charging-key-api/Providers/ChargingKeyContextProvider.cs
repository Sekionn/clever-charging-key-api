using clever_charging_key_api.Models;
using Microsoft.EntityFrameworkCore;

namespace clever_charging_key_api.Providers
{
    /// <summary>
    /// This provider is used to handle updating data in the database for chargingkeys
    /// </summary>
    public class ChargingKeyContextProvider
    {
        private readonly ChargingKeyContext _context;
        public ChargingKeyContextProvider(ChargingKeyContext context) {
            _context = context;
        }

        public async Task<int> SaveChanges()
        {
            return await _context.SaveChangesAsync();
        }

        public void UpdateItem(ChargingKey chargingKey)
        {
            _context.Entry(chargingKey).State = EntityState.Modified;
        }

        public async Task<int> SaveItemToDb(ChargingKey chargingKey)
        {
            _context.keys.Add(chargingKey);

            return await SaveChanges();
        }

        public DbSet<ChargingKey> GetChargingKeys() {  return _context.keys; }
    }
}
