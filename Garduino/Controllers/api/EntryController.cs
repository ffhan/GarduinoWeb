using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Garduino.Data;
using Garduino.Data.Interfaces;
using Garduino.Hubs;
using Garduino.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Controller = Microsoft.AspNetCore.Mvc.Controller;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Garduino.Controllers.api
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/Entry")]
    public class EntryController : Controller
    {
        private readonly IMeasureRepository _repository;
        private readonly IDeviceRepository _deviceRepository;
        private readonly IHubContext<DeviceHub> _hubContext;

        public EntryController(IDeviceRepository deviceRepository, IMeasureRepository repository,
            UserManager<ApplicationUser> userManager, IHubContext<DeviceHub> hubContext)
        {
            _repository = repository;
            _deviceRepository = deviceRepository;
            _hubContext = hubContext;
        }

        // GET: api/Entry
        [HttpGet("{deviceId}")]
        public async Task<IEnumerable<Entry>> GetMeasure([FromRoute] Guid deviceId)
        {
            Device dev = await GetDeviceAsync(deviceId);
            if (dev == null) return null;
            return _repository.GetAll(dev);
        }

        [HttpGet("{deviceId}/{dateTime}")]
        public async Task<IActionResult> GetMeasureFromDate([FromRoute] Guid deviceId, [FromRoute] DateTime dateTime)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            Device dev = await GetDeviceAsync(deviceId);
            if (dev == null) return NotFound("Device not found");
            var measure = await _repository.GetAsync(dateTime, dev);
            if (measure is null) return NotFound();
            return Ok(measure);
        }

        [HttpGet("{deviceId}/{dateTime1}&{dateTime2}")]
        public async Task<IActionResult> GetMeasureFromRange([FromRoute] Guid deviceId, [FromRoute] DateTime dateTime1, 
            [FromRoute] DateTime dateTime2)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            Device dev = await GetDeviceAsync(deviceId);
            if (dev == null) return NotFound("Device not found");
            var measure = await _repository.GetRangeAsync(dateTime1, dateTime2, dev);

            if (measure == null) return NotFound();

            return Ok(measure);
        }


        [HttpPut]
        public async Task<IActionResult> PutMeasure([FromBody] Entry entry)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            Entry mes = await _repository.GetAsync(entry.Id);

            if (!await _repository.UpdateAsync(mes.Id, entry)) return NoContent();
            return Ok();
        }

        // PUT: api/Entry/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMeasure([FromRoute] Guid id, [FromBody] Entry entry)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (!await _repository.UpdateAsync(id, entry)) return NoContent();
            return Ok();
        }

        public class MeasureDevice
        {
            public Guid deviceId { get; set; }
            public Entry Entry { get; set; }
        }

        // POST: api/Entry
        [HttpPost]
        public async Task<IActionResult> PostMeasure([FromBody] MeasureDevice measureDevice)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Device dev = await _deviceRepository.GetAsync(measureDevice.deviceId);
            if (dev == null) return NotFound(measureDevice.deviceId);
            await _hubContext.Clients.Group(GetUserName()).InvokeAsync("newEntry", dev.Name);
            if(await _repository.AddAsync(measureDevice.Entry, dev)) return Ok(
                await _repository.GetAsync(measureDevice.Entry.DateTime, dev));
            return BadRequest();
        }

        // DELETE: api/Entry/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMeasure([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if(!await _repository.DeleteAsync(id)) return BadRequest();
            return Ok();
        }

        [HttpDelete("deleteall")] //ONLY FOR DEVELOPMENT!
        public async Task<IActionResult> DeleteAll()
        {
            if(await _repository.DeleteAllAsync()) return Ok();
            return BadRequest();
        }

        private async Task<Device> _GetDeviceAsync(Guid deviceId) => await _deviceRepository.GetAsync(deviceId);

        private async Task<Device> GetDeviceAsync(Guid? deviceId)
        {
            if (deviceId == null) return null;
            var device = await _GetDeviceAsync(deviceId.Value);
            return device ?? null;
        }

        private string GetUserName() => User.Identity.Name;
    }
}