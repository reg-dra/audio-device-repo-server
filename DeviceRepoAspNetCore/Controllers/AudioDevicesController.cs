using DeviceRepoAspNetCore.Models;
using DeviceRepoAspNetCore.Services;
using Microsoft.AspNetCore.Mvc;

namespace DeviceRepoAspNetCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AudioDevicesController(IAudioDeviceStorage storage) : ControllerBase
    {
        [HttpGet]
        public IEnumerable<DeviceMessage> GetAll() => storage.GetAll();

        [HttpPost]
        public IActionResult Add([FromBody] DeviceMessage deviceMessage)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var clonedWithHashedHost = deviceMessage.Clone();
            clonedWithHashedHost.HostName = CryptService.ComputeChecksum(deviceMessage.HostName);

            storage.Add(clonedWithHashedHost);
            return CreatedAtAction(
                nameof(GetByKey), 
                new { pnpId = clonedWithHashedHost.PnpId, hostName = clonedWithHashedHost.HostName }, 
                deviceMessage
                );
        }

        [HttpGet("{pnpId}/{hostName}")]
        public IActionResult GetByKey(string pnpId, string hostName)
        {
            var device = storage.GetAll().FirstOrDefault(
                d => d.PnpId == pnpId && d.HostName == hostName
                );
            if (device == null)
            {
                return NotFound();
            }

            return Ok(device);
        }

        [HttpDelete("{pnpId}/{hostName}")]
        public IActionResult Remove(string pnpId, string hostName)
        {
            storage.Remove(pnpId, hostName);
            return NoContent();
        }

        [HttpPut("{pnpId}/{hostName}/volume/render")]
        public IActionResult UpdateVolumeRender(string pnpId, string hostName, [FromBody] int volume)
        {
            storage.UpdateVolume(pnpId, hostName, volume, true);
            return NoContent();
        }

        [HttpPut("{pnpId}/{hostName}/volume/capture")]
        public IActionResult UpdateVolumeCapture(string pnpId, string hostName, [FromBody] int volume)
        {
            storage.UpdateVolume(pnpId, hostName, volume, false);
            return NoContent();
        }

        [HttpGet("search")]
        public IEnumerable<DeviceMessage> Search(
            [FromQuery] string query)
        {
            return storage.Search(query);
        }
    }
}