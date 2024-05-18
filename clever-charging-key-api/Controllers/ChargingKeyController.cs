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

        [HttpPost("Create")]
        public async Task<ActionResult<ChargingkeyCreatedDTO>> CreateKey(string chargingTag)
        {
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

            return Ok(new ChargingkeyCreatedDTO { Id = chargingKey.Id });
        }

        [HttpGet("Verify")]
        public async Task<ActionResult<ChargingkeyVerifyDTO>> VerifyKey([FromQuery] string id)
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

            var chargingKey = await _chargingKeyService.FindKeyById(id);

            if (chargingKey == null) return NotFound();
            
            return Ok(new ChargingkeyVerifyDTO { Exists = true, Blocked = chargingKey.Blocked });
        }

        
        [HttpPut("Block/{id}")]
        public async Task<ActionResult<ChargingkeyBlockDTO>> BlockKey(string id)
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

            var chargingKey = await _chargingKeyService.FindKeyById(id) ?? throw new ArgumentNullException(nameof(id));

            if (chargingKey.Blocked) return Ok(new ChargingkeyBlockDTO { Blocked = chargingKey.Blocked, Id = chargingKey.Id });

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

            return Ok(new ChargingkeyBlockDTO { Blocked = chargingKey.Blocked, Id = chargingKey.Id});
        }
    }
}
