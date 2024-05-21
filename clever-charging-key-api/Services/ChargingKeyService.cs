using clever_charging_key_api.Models;
using clever_charging_key_api.Providers;
using Microsoft.EntityFrameworkCore;

namespace clever_charging_key_api.Services
{
    /// <summary>
    /// Created this class, since i don't think you should handle the primary logic directly in the endpoint methods.
    /// </summary>
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

        /// <summary>
        /// this method calls a method to return the 10 digits for the id
        /// </summary>
        /// <returns>the 10 digits with the required, prefix and postfix</returns>
        public string GenerateId()
        {
            var finishedId = $"dk-{RandomIdString()}-clever";

            if (ChargingKeyExists(finishedId))
            {
                finishedId = GenerateId();
            }

            return finishedId;
        }

        /// <summary>
        /// this method generates the 10 digits for the charging key id
        /// </summary>
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

        /// <summary>
        /// this method calls the provider to find the charging key in the database, by using provided id.
        /// </summary>
        public async ValueTask<ChargingKey?> FindKeyById(string id)
        {
            return await _chargingKeyProvider.GetChargingKeys().FindAsync(id);
        }

        /// <summary>
        /// this method gets the chargingkeys and finds out if there is a charging key with the provided id.
        /// </summary>
        public bool ChargingKeyExists(string id)
        {
            return _chargingKeyProvider.GetChargingKeys().Any(e => e.Id == id);
        }

        /// <summary>
        /// calls provider to save the changes in the database
        /// </summary>
        /// <returns></returns>
        public async Task<int> SaveChanges()
        {
            return await _chargingKeyProvider.SaveChanges();
        }

        /// <summary>
        /// calls provider to update the charging key, that is provided.
        /// </summary>
        public void UpdateItem(ChargingKey chargingKey)
        {
            _chargingKeyProvider.UpdateItem(chargingKey);
        }

        /// <summary>
        /// calls provider to add the newly created charging key to the database.
        /// </summary>
        public async Task<int> SaveItemToDb(ChargingKey chargingKey)
        {
            return await _chargingKeyProvider.SaveItemToDb(chargingKey);
        }
    }
}
