using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Garduino.Data;
using Garduino.Data.Interfaces;
using Garduino.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Controller = Microsoft.AspNetCore.Mvc.Controller;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Garduino.Controllers.api
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/Device")]
    public class DeviceController : Controller
    {
        private readonly IDeviceRepository _repository;
        private readonly IUserRepository _userRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public DeviceController(IDeviceRepository repository, IUserRepository userRepository,
            UserManager<ApplicationUser> userManager)
        {
            _repository = repository;
            _userRepository = userRepository;
            _userManager = userManager;
        }

        // GET: api/Device
        [HttpGet]
        public async Task<IEnumerable<Device>> GetDevice()
        {
            return _repository.GetAll(await GetCurrentUserAsync());
        }

        [HttpGet("call/{name}")]
        public async Task<Guid?> Call(string name)
        {
            Device device = await _repository.GetAsync(name, await GetCurrentUserAsync());
            return device?.Id;
        }

        // GET: api/Device/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDevice([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var device = await _repository.GetAsync(id);

            if (device == null)
            {
                return NotFound();
            }

            return Ok(device);
        }

        // PUT: api/Device/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDevice([FromRoute] Guid id, [FromBody] Device device)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Device dev = await _repository.GetAsync(id);
            if (id != dev.Id)
            {
                return BadRequest();
            }

            if (await _repository.UpdateAsync(id, device)) return Ok();
            return InternalServerError();
        }

        // POST: api/Device
        [HttpPost]
        public async Task<IActionResult> PostDevice([FromBody] Device device)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (await _repository.AddAsync(device, await GetCurrentUserAsync()))
            {
                return CreatedAtAction("GetDevice", new { id = device.Id }, device);
            }
            return InternalServerError();
        }

        // DELETE: api/Device/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDevice([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (await _repository.DeleteAsync(id)) return Ok();
            return InternalServerError();
        }

        private async Task<string> _GetCurrentUserIdAsync() =>
            (await _userManager.Users.FirstOrDefaultAsync(g => g.Email.Equals(User.FindFirst(ClaimTypes.NameIdentifier).Value)))?.Id;

        private async Task<User> GetCurrentUserAsync()
        {
            User user = await _userRepository.GetAsync(await _GetCurrentUserIdAsync());
            return user;
        }

        private IActionResult InternalServerError() => StatusCode(StatusCodes.Status500InternalServerError);
    }
}