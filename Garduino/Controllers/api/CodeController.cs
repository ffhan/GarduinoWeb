﻿using System;
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
using Microsoft.AspNetCore.Sockets;
using Microsoft.EntityFrameworkCore;

namespace Garduino.Controllers.api
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/Code")]
    public class CodeController : Controller
    {
        private readonly ICodeRepository _repository;
        private readonly IDeviceRepository _deviceRepository;
        private readonly IHubContext<DeviceHub> _hubContext;
        //TODO: when transfering to ASP.NET just use _hubContext.Caller

        public CodeController(IDeviceRepository deviceRepository, ICodeRepository repository, 
            IHubContext<DeviceHub> hubContext
            )
        {
            _deviceRepository = deviceRepository;
            _repository = repository;
            _hubContext = hubContext;
        }

        // GET: api/Code
        [HttpGet("all/{deviceId}")]
        public async Task<IEnumerable<Code>> GetAll([FromRoute] Guid deviceId)
        {
            Device dev = await GetDeviceAsync(deviceId);
            if (dev == null) return null;
            var t = _repository.GetAll(dev);
            return t;
        }

        [HttpGet("active/{deviceId}")]
        public async Task<IEnumerable<Code>> GetActiveCode([FromRoute] Guid deviceId)
        {
            Device dev = await GetDeviceAsync(deviceId);
            if (dev == null) return null;
            return _repository.GetActive(dev);
        }

        [HttpGet("latest/{deviceId}")]
        public async Task<Code> GetLatestCode([FromRoute] Guid deviceId, bool invoke = true)
        {
            Device dev = await GetDeviceAsync(deviceId);
            if (dev == null) return null;
            Code code = _repository.GetLatest(dev);
            if (code == null) return null;
            if(invoke) {
                await _hubContext.Clients.Group(GetUserName()).InvokeAsync("codeFetched", dev.Name,
                string.IsNullOrWhiteSpace(code.ActionName) ? code.Action.ToString() : code.ActionName);
                
            }
            return code;
        }

        public class TimeDevice
        {
            public DateTime dateTime { get; set; }
            public Guid deviceId { get; set; }
        }

        [HttpPut("latest")]
        public async Task<IActionResult> CompleteCode([FromBody] TimeDevice timeDevice) //TODO: Complete & Fix.
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            Code latest = await GetLatestCode(timeDevice.deviceId, false);
            if (latest == null) return NotFound();
            await _repository.CompleteAsync(latest, timeDevice.dateTime);
            await _hubContext.Clients.Group(GetUserName()).InvokeAsync("codeDone", latest.Device.Name,
                string.IsNullOrWhiteSpace(latest.ActionName) ? latest.Action.ToString() : latest.ActionName);
            return Ok();
        }

        // GET: api/Code/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCode([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Code code = await _repository.GetAsync(id);

            if (code == null)
            {
                return NotFound();
            }

            return Ok(code);
        }

        // PUT: api/Code/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCode([FromRoute] Guid id, [FromBody] Code code)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (!await _repository.UpdateAsync(id, code)) return NoContent();
            return Ok();
        }

        public class CodeDevice
        {
            public Code code { get; set; }
            public Guid deviceId { get; set; }
        }

        // POST: api/Code
        [HttpPost]
        public async Task<IActionResult> PostCode([FromBody] CodeDevice codeDevice)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!await _repository.AddAsync(codeDevice.code, await GetDeviceAsync(codeDevice.deviceId))) return BadRequest();
            return Ok();
        }

        // DELETE: api/Code/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCode([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (await _repository.DeleteAsync(id)) return Ok();
            return BadRequest();
        }

        [HttpDelete("deleteall")] //ONLY FOR DEVELOPMENT!
        public async Task<IActionResult> DeleteAll()
        {
            if (await _repository.DeleteAllAsync()) return Ok();
            return BadRequest();
        }

        private async Task<Guid> GetDeviceIdAsync(Guid codeId)
        {
            Code code = await _repository.GetAsync(codeId);
            Guid deviceId = code.Device.Id;
            return deviceId;
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