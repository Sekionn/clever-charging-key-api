using clever_charging_key_api.Models;
using clever_charging_key_api.Providers;
using Microsoft.EntityFrameworkCore;

namespace clever_charging_key_api.Services
{
    public class ChargingKeyService
    {
        private readonly ChargingKeyContextProvider _chargingKeyProvider;
        public ChargingKeyService(ChargingKeyContextProvider chargingKeyProvider) {
            _chargingKeyProvider = chargingKeyProvider;
        }

        public ChargingKey CreateNewKey(KeyTag id)
        {
            return new ChargingKey(GenerateId(), id);
        }

        public string GenerateId()
        {
            var finishedId = $"dk-{RandomIdString()}-clever";

            if (ChargingKeyExists(finishedId))
            {
                finishedId = GenerateId();
            }

            return finishedId;
        }

        public string RandomIdString()
        {
            Random rnd = new Random();
            string id = "{";
            for (int i = 0; i < 10; i++)
            {
                id += rnd.Next(0, 10).ToString();
            }
            id += "}";
            return id;
        }

        public async ValueTask<ChargingKey?> FindKeyById(string id)
        {
            return await _chargingKeyProvider.GetChargingKeys().FindAsync(id);
        }

        public bool ChargingKeyExists(string id)
        {
            return _chargingKeyProvider.GetChargingKeys().Any(e => e.Id == id);
        }

        public async Task<int> SaveChanges()
        {
            return await _chargingKeyProvider.SaveChanges();
        }

        public void UpdateItem(ChargingKey chargingKey)
        {
            _chargingKeyProvider.UpdateItem(chargingKey);
        }

        public async Task<int> SaveItemToDb(ChargingKey chargingKey)
        {
            return await _chargingKeyProvider.SaveItemToDb(chargingKey);
        }
    }
}
