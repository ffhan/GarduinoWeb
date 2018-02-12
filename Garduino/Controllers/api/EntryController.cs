using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Garduino.Data;
using Garduino.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Controller = Microsoft.AspNetCore.Mvc.Controller;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Garduino.Controllers.api
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/Entry")]
    public class EntryController : Controller
    {
        private readonly IMeasureRepository _repository;
        private readonly UserManager<ApplicationUser> _userManager;

        public EntryController(IMeasureRepository repository, UserManager<ApplicationUser> userManager)
        {
            _repository = repository;
            _userManager = userManager;
        }

        // GET: api/Entry
        [HttpGet]
        public async Task<IEnumerable<Measure>> GetMeasure()
        {
            return _repository.GetAll(await GetCurrentUser());
        }

        [HttpGet("device={device}")]
        public async Task<IEnumerable<Measure>> GetMeasure([FromRoute] string device)
        {
            return _repository.GetDevice(device, await GetCurrentUser());
        }

        [HttpGet("date={dateTime}")]
        public async Task<IActionResult> GetMeasure([FromRoute] DateTime dateTime)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var measure = await _repository.GetAsync(dateTime, await GetCurrentUser());

            try
            {
                if (measure is null) return NotFound();
            }
            catch (Exception e)
            {
                return NotFound();
            }
            

            return Ok(measure);
        }

        [HttpGet("cmp={dateTime1}&{dateTime2}")]
        public async Task<IActionResult> GetMeasure([FromRoute] DateTime dateTime1, [FromRoute] DateTime dateTime2)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var measure = await _repository.GetRangeAsync(dateTime1, dateTime2, await GetCurrentUser());

            if (measure == null) return NotFound();

            return Ok(measure);
        }

        // GET: api/Entry/5
        [HttpGet("id={id}")]
        public async Task<IActionResult> GetMeasure([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var measure = await _repository.GetAsync(id, await GetCurrentUser());

            if (measure == null)
            {
                return NotFound();
            }

            return Ok(measure);
        }

        [HttpPut]
        public async Task<IActionResult> PutMeasure([FromBody] Measure measure)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            Guid id = await _repository.GetId(measure, await GetCurrentUser());

            if (!await _repository.UpdateAsync(id, measure, await GetCurrentUser())) return NoContent();
            return Ok();
        }

        // PUT: api/Entry/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMeasure([FromRoute] Guid id, [FromBody] Measure measure)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (!await _repository.UpdateAsync(id, measure, await GetCurrentUser())) return NoContent();
            return Ok();
        }

        // POST: api/Entry
        [HttpPost]
        public async Task<IActionResult> PostMeasure([FromBody] Measure measure)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (await _repository.AddAsync(measure, await GetCurrentUser())) return Ok();
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

            if (await _repository.DeleteAsync(id, await GetCurrentUser())) return Ok(await _repository.GetAsync(id,
                await GetCurrentUser()));
            return BadRequest();
        }

        [HttpGet("cmp/{dateTime1}&{dateTime2}")]
        public async Task<IActionResult> Compare(DateTime dateTime1, DateTime dateTime2)
        {
            if (await _repository.GetAsync(dateTime1, await GetCurrentUser()) == await _repository.GetAsync(dateTime2,
                await GetCurrentUser())) return Ok();
            return BadRequest();
        }

        [HttpDelete("deleteall")] //ONLY FOR DEVELOPMENT!
        public async Task<IActionResult> DeleteAll()
        {
            if(await _repository.DeleteAllAsync()) return Ok();
            return BadRequest();
        }

        private async Task<string> GetCurrentUserIdAsync()
        {
            var userId = await _userManager.Users.FirstOrDefaultAsync(g => g.Email.Equals(User.FindFirst(ClaimTypes.NameIdentifier).Value));
            return userId?.Id;
        }
        private async Task<ApplicationUser> GetCurrentUser() => await _userManager.GetUserAsync(HttpContext.User);
    }
}