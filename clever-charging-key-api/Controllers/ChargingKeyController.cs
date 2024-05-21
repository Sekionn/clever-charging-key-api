using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using clever_charging_key_api.Models;
using clever_charging_key_api.Services;
using clever_charging_key_api.Providers;

namespace clever_charging_key_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChargingKeyController : ControllerBase
    {
        private readonly ChargingKeyService _chargingKeyService;

        public ChargingKeyController(ChargingKeyService chargingKeyService)
        {
            _chargingKeyService = chargingKeyService;
        }

        /// <summary>
        /// this method creates a chargingKey, 
        /// in a complete project there would also be a check for,
        /// if the client that is creating the key is a valid user with the correct permissions,
        /// that if it is not correct it would also throw an exception.
        /// </summary>
        /// <param name="chargingTag">
        /// since specified it is a string, 
        /// but i would have made it an enum,
        /// since it is easier to change the names later on.
        /// </param>
        /// <returns> the id of the newly created chargingKey</returns>
        /// <exception cref="ArgumentNullException"></exception>
        [HttpPost("Create")]
        public async Task<ActionResult<ChargingKeyCreatedDTO>> CreateKey(string chargingTag) 
        {
            // Should check for user credentials before checking aswell

            if (string.IsNullOrEmpty(chargingTag)) throw new ArgumentNullException(nameof(chargingTag));

            if (!Enum.IsDefined(typeof(KeyTag), chargingTag)) return BadRequest("Not a valid key type");

            ChargingKey chargingKey = _chargingKeyService.CreateNewKey((KeyTag)Enum.Parse(typeof(KeyTag), chargingTag));

            try
            {
                await _chargingKeyService.SaveItemToDb(chargingKey);
            }
            catch (DbUpdateException)
            {
                if (_chargingKeyService.ChargingKeyExists(chargingKey.Id))
                {
                    return Conflict("Key allready exists");
                }
                else
                {
                    throw;
                }
            }

            return Ok(new ChargingKeyCreatedDTO { Id = chargingKey.Id });
        }

        /// <summary>
        /// This method verifies whether the charing key exists and is blocked. 
        /// The parameter id is recieved from the query in the endpoint.
        /// </summary>
        /// <param name="id">the is is used ti find the correct key in the database</param>
        /// <returns>chargingkeyVerifyDTO that contains exists and blocking properties,
        /// and if it isn't found it sends a notFound actionResult.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        [HttpGet("Verify")]
        public async Task<ActionResult<ChargingKeyVerifyDTO>> VerifyKey([FromQuery] string id)
        {
            // Should check for user credentials before checking aswell

            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

            var chargingKey = await _chargingKeyService.FindKeyById(id);

            if (chargingKey == null) return NotFound();
            
            return Ok(new ChargingKeyVerifyDTO { Exists = true, Blocked = chargingKey.Blocked });
        }

        /// <summary>
        /// This method tries to find the id that it received and returns.
        /// </summary>
        /// <param name="id"></param>
        /// <returns> whether or not it is blocked</returns>
        /// <exception cref="ArgumentNullException"></exception>
        [HttpPut("Block/{id}")]
        public async Task<ActionResult<ChargingKeyBlockDTO>> BlockKey(string id)
        {
            // Should check for user credentials before checking aswell

            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

            var chargingKey = await _chargingKeyService.FindKeyById(id) ?? throw new ArgumentNullException(nameof(id));

            if (chargingKey.Blocked) return Ok(new ChargingKeyBlockDTO { Blocked = chargingKey.Blocked, Id = chargingKey.Id });

            _chargingKeyService.UpdateItem(chargingKey);
            
            chargingKey.Blocked = true;

            try
            {
                await _chargingKeyService.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_chargingKeyService.ChargingKeyExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(new ChargingKeyBlockDTO { Blocked = chargingKey.Blocked, Id = chargingKey.Id});
        }
    }
}
